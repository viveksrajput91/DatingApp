import { NgModule } from '@angular/core';
import { ActivatedRoute, RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { NotFoundComponent } from './_error/not-found/not-found.component';
import { ServerErrorComponent } from './_error/server-error/server-error.component';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUserUnsavedChangeGuard } from './_guards/prevent-user-unsaved-change.guard';

const routes: Routes = [
  {path:"",component:HomeComponent},
  {path:"",canActivate:[AuthGuard], runGuardsAndResolvers:'always', children:[
    {path:"members",component:MemberListComponent},
    {path:"members/:userName",component:MemberDetailComponent},
    {path:"member/edit",component:MemberEditComponent,canDeactivate:[PreventUserUnsavedChangeGuard]},
    {path:"lists",component:ListsComponent},
    {path:"messages",component:MessagesComponent}
  ]},
  {path:"**",component:NotFoundComponent,pathMatch:"full"},
  {path:"not-found",component:NotFoundComponent},
  {path:"server-error",component:ServerErrorComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
