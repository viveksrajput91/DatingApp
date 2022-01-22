import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { HtmlParser } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { LikesParams } from '../_models/LikesParams';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/PaginatedResult';
import { Photo } from '../_models/Photo';
import { User } from '../_models/user';
import { UserParams } from '../_models/UserParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginatedResultHelper';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  
  members:Member[]=[];
  baseUrl=environment.baseUrl;
  MemberCache=new Map();
  user:User;
  userParams:UserParams;

  constructor(private http:HttpClient,private accountService:AccountService) {
    this.accountService.CurrentUser$.pipe(take(1)).subscribe(user=>{
      this.user=user;
      this.userParams=new UserParams(user);
    });
   }

   addLike(userName:string)
   {
     return this.http.post(this.baseUrl + "likes/" + userName,{});
   }

   getLikes(likesParams:LikesParams)
   {
     let httpParams=new HttpParams();
     httpParams=httpParams.append('pageNumber',likesParams.PageNumber);
     httpParams=httpParams.append('pageSize',likesParams.PageSize);
     httpParams=httpParams.append('Predicate',likesParams.Predicate);

     return getPaginatedResult<Partial<Member[]>>(this.baseUrl + "likes",httpParams,this.http);
   }

   getUserParams()
   {
     return this.userParams;
   }

   setUserParams(params:UserParams)
   {
     this.userParams=params;
   }

   resetUserParams()
   {
     this.userParams=new UserParams(this.user);
     return this.userParams;
   }

  getMembers(userParams:UserParams)
  {
    
    var key=Object.values(userParams).join('-');
    var response = this.MemberCache.get(key);
    if(response)
    {
      return of(response);
    }

    let httpParams=getPaginationHeaders(userParams.pageNumber,userParams.pageSize);

    httpParams=httpParams.append('minAge',userParams.minAge.toString());
    httpParams=httpParams.append("maxAge",userParams.maxAge.toString());
    httpParams=httpParams.append("currentUserName",userParams.currentUserName)
    httpParams=httpParams.append("gender",userParams.gender.toString());
    httpParams=httpParams.append('orderBy',userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + "users",httpParams,this.http).pipe(map(response=>{
      this.MemberCache.set(key,response);
      return response;
    }));
  }

 
  getMember(userName:string)
  {
    var Member=[...this.MemberCache.values()].reduce((arr,elem)=>arr.concat(elem.result),[]).find((member:Member)=>member.userName===userName);
    if(Member) return of(Member);
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
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId,{});
  }

  deletePhoto(photoId:number)
  {
    return this.http.delete(this.baseUrl + "users/delete-photo/" + photoId);
  }
}
