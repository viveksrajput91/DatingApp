import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoadingService } from '../_services/loading.service';
import { delay, finalize } from 'rxjs/operators';

@Injectable()
export class LoadInterceptor implements HttpInterceptor {

  constructor(private loading:LoadingService) {

  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.loading.show();

    return next.handle(request).pipe(
      delay(1000),
      finalize(()=>{
        this.loading.hide()
      })
    );
  }
}
