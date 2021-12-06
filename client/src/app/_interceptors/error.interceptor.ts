import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router:Router,private toastrService:ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(httpResponse=>{
       if(httpResponse)
       {
         switch (httpResponse.status) {
           case 400:
             if(httpResponse.error.errors)
             {
               const modalStateError=[];
               for(const key in httpResponse.error.errors)
               {
                 if(httpResponse.error.errors[key])
                 {
                   modalStateError.push(httpResponse.error.errors[key]);
                 }
               }
                throw modalStateError.flat();
             }
             else
             {
              this.toastrService.error(httpResponse.statusText,httpResponse.status);
             }            
             break;
            case 401:
              this.toastrService.error(httpResponse.statusText,httpResponse.status);
              break;
            case 404:
              this.router.navigateByUrl("/not-found");
              break;
            case 500:
              const navigationExtras:NavigationExtras={state:{error:httpResponse.error}};
              this.router.navigateByUrl("/server-error",navigationExtras);
              break;
           default:
              this.toastrService.error("Something went wrong");
              console.log(httpResponse.error);
              break;
         }
       } 
       return throwError(httpResponse);
      }
      )
    )
  }
}
