import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Photo } from '../_models/Photo';


@Injectable({
  providedIn: 'root'
})
export class MembersService {

  members:Member[]=[];

  baseUrl=environment.baseUrl;

  constructor(private http:HttpClient) { }

  getMembers(){
    if(this.members.length>0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl+"users").pipe(
      map(members=>
        {
          this.members=members;
          return this.members;
        })
    );
  }

  getMember(userName:string)
  {
    const member=this.members.find(x=>x.userName===userName);
    if(member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + "users/" + userName);
    
  }

  updateMember(member:Member)
  {
    return this.http.put(this.baseUrl + "users",member).pipe(
      map(()=>{
        const indexno=this.members.indexOf(member);
        this.members[indexno]=member;
      })
    );
  }

  setMainPhoto(photoId:number)
  {
    return this.http.put(this.baseUrl+"users/set-main-photo/"+photoId,{});
  }

  deletePhoto(photoId:number)
  {
    return this.http.delete(this.baseUrl + "users/delete-photo/" + photoId);
  }
}
