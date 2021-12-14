import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
@Injectable({
  providedIn: 'root'
})
export class LoadingService {

  requestCount=0;

  constructor(private spinnerService:NgxSpinnerService) {

   }

   show()
   {
     this.requestCount++;
     this.spinnerService.show(undefined,{
       type:'line-scale-party',
       bdColor:"rgba(255,255,255,0)",
       color:"#333333"
     })
   }

   hide()
   {
     this.requestCount--;
     if(this.requestCount <=0)
     {
       this.spinnerService.hide();
     }
   }

}
