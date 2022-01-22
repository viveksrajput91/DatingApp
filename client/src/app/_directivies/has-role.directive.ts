import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  user:User;
  @Input() appHasRole:string[];

  constructor(private viewContainerRef:ViewContainerRef,private templateRef:TemplateRef<any>,private accountService:AccountService) {
    this.accountService.CurrentUser$.pipe(take(1)).subscribe(user=>{
      this.user=user;
    })
   }

  ngOnInit(): void {
    if(this.user?.roles ===null)
    {
      this.viewContainerRef.clear();
    }
    if(this.user.roles.some(r=>this.appHasRole.includes(r)))
    {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }
    else
    {
      this.viewContainerRef.clear();
    }
  }

}