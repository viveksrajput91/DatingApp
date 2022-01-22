import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static:true}) memberTabs:TabsetComponent;
  member:Member;
  messages:Message[]=[];
  galleryOptions:NgxGalleryOptions[];
  galleryImages:NgxGalleryImage[];

  
  constructor(private memberService:MembersService,private route:ActivatedRoute,private messagesService:MessageService) { }

  ngOnInit(): void {

    this.route.data.subscribe(data=>{
      this.member=data.member;
    })

    this.route.queryParams.subscribe(queryParams=>{
      queryParams.tab ? this.SelectTab(queryParams.tab):this.SelectTab(0);
    })
    
    this.galleryOptions=[{
      width:"500px",
      height:"500px",
      thumbnailsColumns:4,
      imageAnimation:NgxGalleryAnimation.Slide,
      imagePercent:100,
      preview:false
    }]

    this.galleryImages=this.getImages();

  }

  getImages():NgxGalleryImage[]
  {
    const imageDetails=[];
    for(const photo of this.member.photos)
    {
      imageDetails.push({
        small:photo?.url,
        medium:photo?.url,
        big:photo?.url
      })
    }

    return imageDetails;
  }


  loadMessageThread()
  {
    this.messagesService.GetMessageThread(this.member.userName).subscribe(msgs=>{
      this.messages=msgs;
    })
  }

  ActivatedTab(event:TabDirective)
  {
    if(event.heading==='Messages' && this.messages.length===0)
    {
      this.loadMessageThread();
    }
  }

  SelectTab(tabNumber:number)
  {
    this.memberTabs.tabs[tabNumber].active=true;
  }

}
