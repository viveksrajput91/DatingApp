import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  pageNumber=1;
  pageSize=5;
  container='Unread';
  pagination:Pagination;
  messages:Message[];
  loading=false;

  constructor(private messagesService:MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages()
  {
    this.loading=true;
    this.messagesService.GetMessages(this.pageNumber,this.pageSize,this.container).subscribe(response=>{
      this.messages=response.result;
      this.pagination=response.pagination;
      this.loading=false;
    })
  }

  deleteMessage(id:number)
  {
    this.messagesService.deleteMessage(id).subscribe(()=>{
      this.messages.splice(this.messages.findIndex(m=>m.id===id),1);
    })
  }

  pageChanged(event:any)
  {
    this.pageNumber=event.page;
    this.loadMessages();
  }
}
