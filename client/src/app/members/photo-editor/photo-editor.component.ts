import { Component, Input, OnInit } from '@angular/core';
import { FileItem, FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/Photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member:Member;
  uploader:FileUploader;
  baseUrl=environment.baseUrl;
  user:User;
  hasBaseDropZoneOver:false;

  constructor(private accountService:AccountService,private memberService:MembersService) {
    this.accountService.CurrentUser$.pipe(take(1)).subscribe(user=>this.user=user);
   }

  ngOnInit(): void {
    this.initializeFileUploader();
  }

  fileOverBase(e:any)
  {
    this.hasBaseDropZoneOver=e;
  }

  initializeFileUploader()
  {
    this.uploader=new FileUploader({
      url:this.baseUrl+"users/add-photo",
      authToken:"Bearer " + this.user.token,
      allowedFileType:["image"],
      autoUpload:false,
      isHTML5:true,
      maxFileSize:10*1024*1024,
      removeAfterUpload:true
    });

    this.uploader.onAfterAddingFile= (File)=>
    {
      File.withCredentials=false;
    }

    this.uploader.onSuccessItem=(item,response,status,headers) => {
      if(response)
      {
        const photo=JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

  setMainPhoto(photo:Photo)
  {
   this.memberService.setMainPhoto(photo.id).subscribe(()=>{
    this.user.photoUrl=photo.url;
    this.accountService.SetCurrentUser(this.user);
    this.member.photoUrl=photo.url;
    this.member.photos.forEach(p=>{
     if(p.isMain) p.isMain=false;
     if(p.id===photo.id) p.isMain=true; 
    });
   });
  }

  deletePhoto(photoId:number)
  {
    this.memberService.deletePhoto(photoId).subscribe(()=>{
      this.member.photos=this.member.photos.filter(x=>x.id !==photoId);
    });
  }

}
