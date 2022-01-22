import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationHeaders } from './paginatedResultHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl=environment.baseUrl;
  
  constructor(private http:HttpClient) { }

  GetMessages(pageNumber:number,pageSize:number,container:string)
  {
     let httpParams=getPaginationHeaders(pageNumber,pageSize);
     httpParams=httpParams.append('container',container);
     return getPaginatedResult<Message[]>(this.baseUrl + "messages",httpParams,this.http);
  }

  GetMessageThread(userName:string)
  {
    return this.http.get<Message[]>(this.baseUrl + "Messages/thread/" + userName);
  }

  sendMessage(userName:string,content:string)
  {
    return this.http.post<Message>(this.baseUrl + "messages",{recipientUserName:userName,content});
  }

  deleteMessage(id:number)
  {
    return this.http.delete(this.baseUrl + "messages/" + id);
  }
}
