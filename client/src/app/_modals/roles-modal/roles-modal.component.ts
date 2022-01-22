import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  
  user:User;
  roles:any[];
  @Input() updateSelectedRoles=new EventEmitter();
  
  constructor(public bsModalRef:BsModalRef) { }

  ngOnInit(): void {
  }

  updateRoles()
  {
    console.log("UpdateRoles called" + this.roles);
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }
}
