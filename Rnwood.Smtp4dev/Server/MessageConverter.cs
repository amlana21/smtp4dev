using MimeKit;
using Rnwood.Smtp4dev.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rnwood.Smtp4dev.Server
{
    public class MessageConverter
    {
        public Message Convert(Stream messageData, string from, string to)
        {
            string subject;

            try
            {

                MimeMessage mime = MimeMessage.Load(messageData, false);
                subject = mime.Subject;
            } catch (FormatException e)
            {
                subject = "<Invalid MIME message>";
            }

            messageData.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[messageData.Length];
            messageData.Read(data, 0, data.Length);

            Message message = new Message()
            {
                Id = Guid.NewGuid(),

                From = from,
                To = to,
                ReceivedDate = DateTime.Now,
                Subject = subject,
                Data = data

            };

            return message;
        }
    }
}
