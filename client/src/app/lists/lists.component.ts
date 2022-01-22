import { Component, OnInit } from '@angular/core';
import { LikesParams } from '../_models/LikesParams';
import { Member } from '../_models/member';
import { Pagination } from '../_models/Pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members:Partial<Member[]>;
  predicate='liked';
  pagination:Pagination;
  likesParams:LikesParams;

  constructor(private membersService:MembersService) { 
    this.likesParams=new LikesParams();
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes()
  {
    return this.membersService.getLikes(this.likesParams).subscribe(response =>{
      this.members=response.result;
      this.pagination=response.pagination;
    })
  }

  pageChanged(pageNumber:any)
  {
    this.likesParams.PageNumber=pageNumber.page;
    this.loadLikes();
  }

}
