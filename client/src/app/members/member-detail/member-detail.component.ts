import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member:Member;
  galleryOptions:NgxGalleryOptions[];
  galleryImages:NgxGalleryImage[];
  
  constructor(private memberService:MembersService,private route:ActivatedRoute) { }

  ngOnInit(): void {

    this.loadMember();
    
    this.galleryOptions=[{
      width:"500px",
      height:"500px",
      thumbnailsColumns:4,
      imageAnimation:NgxGalleryAnimation.Slide,
      imagePercent:100,
      preview:false
    }]
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

  loadMember()
  {
    return this.memberService.getMember(this.route.snapshot.paramMap.get("userName"))
          .subscribe(next=>{
            this.member=next;
            this.galleryImages=this.getImages();
          });
  }

}
