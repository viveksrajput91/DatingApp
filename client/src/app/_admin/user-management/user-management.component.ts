import { Component, OnInit } from '@angular/core';
import { updateLocale } from 'ngx-bootstrap/chronos';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/_modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  users:Partial<User[]>;
  ModalRef:BsModalRef;

  constructor(private adminService:AdminService,private bsModalService:BsModalService) { }

  ngOnInit(): void {
    this.loadUserWithRoles();
  }

  loadUserWithRoles()
  {
    this.adminService.getUserWithRoles().subscribe(user=>{
      this.users=user;
    })
  }

  openRolesModal(user:User)
  {
    const configOption={
      class:'modal-dialog-centered',
      initialState:{
        user,
        roles:this.getRolesArray(user)
      }
    };

    this.ModalRef = this.bsModalService.show(RolesModalComponent, configOption);
    this.ModalRef.content.updateSelectedRoles.subscribe(values=>{

      console.log(values);
      console.log([...values]);

      const rolesToUpdate={
        roles:values.filter(el=>el.checked===true).map(el=>el.name)
      };

      if(rolesToUpdate)
      {
        this.adminService.updateUserRoles(user.username,rolesToUpdate.roles).subscribe(()=>{
        
          user.roles=rolesToUpdate.roles;
        })
      }
    })
  }

  getRolesArray(user:User)
  {
    const roles:any[]=[];
    const userRoles=user.roles;
    const availableRoles:any[]=[
      {name :'Admin',value:'Admin' },
      {name :'Moderator',value:'Moderator' },
      {name :'Member',value:'Member' }
    ];

    availableRoles.forEach(role=>{
      let isMatch=false;
      for(const userRole of userRoles)
      {
        if(role.name===userRole)
        {
          isMatch=true;
          role.checked=true;
          roles.push(role);
          break;
        }
      }

      if(isMatch===false)
      {
        role.checked=false;
        roles.push(role);
      }
    });

    return roles;
  }

}
