﻿import MessageSummary from './MessageSummary';
import Message from './Message';
import FileStreamResult from './FileStreamResult';
import axios, { AxiosRequestConfig } from "axios";
import MessageRequestConfig from './MessageRequestConfig';

export default class MessagesController {
    public _baseUrl: string;                
 
    constructor(baseUrl: string = "/"){
        this._baseUrl = baseUrl;
    }
        
    // get: api/Messages  
    public getSummaries_url(): string {
        return `${this._baseUrl}api/Messages`;
    }

    public async getSummaries(sortColumn?: string, sortIsDescending?: boolean): Promise<MessageSummary[]> {

        let config: AxiosRequestConfig = new MessageRequestConfig();
        if (sortColumn != null) {
            config.params = { sortColumn: sortColumn, sortIsDescending: sortIsDescending != null ? sortIsDescending : true };
        }
        return (await axios.get(this.getSummaries_url(), config)).data as MessageSummary[];
    }
    
    // get: api/Messages/${encodeURIComponent(id)}  
    public getMessage_url(id: string): string {
        return `${this._baseUrl}api/Messages/${encodeURIComponent(id)}`;
    }

    public async getMessage(id: string): Promise<Message> {

        return (await axios.get(this.getMessage_url(id), null || undefined)).data as Message;
    }
    
    // get: api/Messages/${encodeURIComponent(id)}/source  
    public downloadMessage_url(id: string): string {
        return `${this._baseUrl}api/Messages/${encodeURIComponent(id)}/source`;
    }

    public async downloadMessage(id: string): Promise<FileStreamResult> {

        return (await axios.get(this.downloadMessage_url(id), null || undefined)).data as FileStreamResult;
    }
    
    // get: api/Messages/${encodeURIComponent(id)}/part/${encodeURIComponent(cid)}/content  
    public getPartContent_url(id: string, cid: string): string {
        return `${this._baseUrl}api/Messages/${encodeURIComponent(id)}/part/${encodeURIComponent(cid)}/content`;
    }

    public async getPartContent(id: string, cid: string): Promise<FileStreamResult> {

        return (await axios.get(this.getPartContent_url(id, cid), null || undefined)).data as FileStreamResult;
    }
    
    // get: api/Messages/${encodeURIComponent(id)}/part/${encodeURIComponent(cid)}/source  
    public getPartSource_url(id: string, cid: string): string {
        return `${this._baseUrl}api/Messages/${encodeURIComponent(id)}/part/${encodeURIComponent(cid)}/source`;
    }

    public async getPartSource(id: string, cid: string): Promise<string> {

        return (await axios.get(this.getPartSource_url(id, cid), null || undefined)).data as string;
    }
    
    // get: api/Messages/${encodeURIComponent(id)}/html  
    public getMessageHtml_url(id: string): string {
        return `${this._baseUrl}api/Messages/${encodeURIComponent(id)}/html`;
    }

    public async getMessageHtml(id: string): Promise<string> {

        return (await axios.get(this.getMessageHtml_url(id), null || undefined)).data as string;
    }
    
    // delete: api/Messages/${encodeURIComponent(id)}  
    public delete_url(id: string): string {
        return `${this._baseUrl}api/Messages/${encodeURIComponent(id)}`;
    }

    public async delete(id: string): Promise<void> {

        return (await axios.delete(this.delete_url(id), null || undefined)).data as void;
    }
    
    // delete: api/Messages/*  
    public deleteAll_url(): string {
        return `${this._baseUrl}api/Messages/*`;
    }

    public async deleteAll(): Promise<void> {

        return (await axios.delete(this.deleteAll_url(), null || undefined)).data as void;
    }
}
