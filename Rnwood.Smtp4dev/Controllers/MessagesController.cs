﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Microsoft.AspNetCore.Mvc;

using Rnwood.Smtp4dev.ApiModel;
using Rnwood.Smtp4dev.DbModel;
using Rnwood.Smtp4dev.Hubs;

using Message = Rnwood.Smtp4dev.DbModel.Message;

namespace Rnwood.Smtp4dev.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        public MessagesController(Smtp4devDbContext dbContext, MessagesHub messagesHub)
        {
            _dbContext = dbContext;
            _messagesHub = messagesHub;
        }

        private Smtp4devDbContext _dbContext;
        private MessagesHub _messagesHub;

        [HttpGet]
        public IEnumerable<ApiModel.MessageSummary> GetSummaries(string sortColumn = "receivedDate", bool sortIsDescending = true)
        {
            var orderBy = Sort(sortColumn);
            return sortIsDescending ? 
                _dbContext.Messages.Select(m => new ApiModel.MessageSummary(m)).OrderByDescending(m => orderBy(m)) : 
                _dbContext.Messages.Select(m => new ApiModel.MessageSummary(m)).OrderBy(m => orderBy(m));
        }

        [HttpGet("{id}")]
        public ApiModel.Message GetMessage(Guid id)
        {
            Message result = _dbContext.Messages.FirstOrDefault(m => m.Id == id);
            return new ApiModel.Message(result);
        }

        [HttpGet("{id}/source")]
        public FileStreamResult DownloadMessage(Guid id)
        {
            Message result = _dbContext.Messages.FirstOrDefault(m => m.Id == id);
            return new FileStreamResult(new MemoryStream(result.Data), "message/rfc822") { FileDownloadName = $"{id}.eml" };
        }

        [HttpGet("{id}/part/{cid}/content")]
        public FileStreamResult GetPartContent(Guid id, string cid)
        {
            Message result = _dbContext.Messages.FirstOrDefault(m => m.Id == id);

            return ApiModel.Message.GetPartContent(result, cid);
        }

        [HttpGet("{id}/part/{cid}/source")]
        public string GetPartSource(Guid id, string cid)
        {
            Message result = _dbContext.Messages.FirstOrDefault(m => m.Id == id);

            return ApiModel.Message.GetPartSource(result, cid);
        }

        [HttpGet("{id}/html")]
        public string GetMessageHtml(Guid id)
        {
            Message result = _dbContext.Messages.FirstOrDefault(m => m.Id == id);

            string html = ApiModel.Message.GetHtml(result);

            if (html == null)
            {
                html = "<pre>" + HtmlAgilityPack.HtmlDocument.HtmlEncode(ApiModel.Message.GetText(result)) + "</pre>";
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection imageElements = doc.DocumentNode.SelectNodes("//img[starts-with(@src, 'cid:')]");

            if (imageElements != null)
            {
                foreach (HtmlNode imageElement in imageElements)
                {
                    string cid = imageElement.Attributes["src"].Value.Replace("cid:", "", StringComparison.OrdinalIgnoreCase);
                    imageElement.Attributes["src"].Value = $"/api/Messages/{id.ToString()}/part/{cid}/content";
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {

            _dbContext.Messages.RemoveRange(_dbContext.Messages.Where(m => m.Id == id));
            _dbContext.SaveChanges();

            await _messagesHub.OnMessagesChanged();

        }

        [HttpDelete("*")]
        public async Task DeleteAll()
        {

            _dbContext.Messages.RemoveRange(_dbContext.Messages);
            _dbContext.SaveChanges();

            await _messagesHub.OnMessagesChanged();

        }

        private static Func<MessageSummary, object> Sort(string column)
        {
            switch (column.ToLower())
            {
                case "subject":
                    return (m) => m.Subject;
                case "from":
                    return (m) => m.From;
                case "to":
                    return (m) => m.To;
                default:
                    return (m) => m.ReceivedDate;
            }
        }
    }
}
