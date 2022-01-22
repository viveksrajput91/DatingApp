import { HttpClient, JsonpClientBackend } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl=environment.baseUrl;
  private CurrentUserSource=new ReplaySubject<User>(1);
  CurrentUser$ = this.CurrentUserSource.asObservable();

  constructor(private http:HttpClient) { }

  register(model:any)
  {
    return this.http.post(this.baseUrl +"account/register",model).pipe(
      map((response)=>
        {
          const user=response as User;
          if(user)
          {
            this.SetCurrentUser(user);
          }
          return user;
        })
    )
  }

  login(model:any)
  {
    return this.http.post(this.baseUrl + "account/login",model).pipe(
      map((response) => {
        const user=response as User;
        if(user)
        {
          this.SetCurrentUser(user);
        }
        return user;
      })
    )
  }

  SetCurrentUser(user:User | undefined)
  {
    user.roles=[];
    const roles=this.decodeToken(user.token).role;
    Array.isArray(roles)?user.roles=roles:user.roles.push(roles);

    localStorage.setItem("user", JSON.stringify(user));
    this.CurrentUserSource.next(user);
  }

  logout()
  {
    this.CurrentUserSource.next(undefined);
    localStorage.removeItem("user");
  }

  decodeToken(token)
  {111111111111
    return JSON.parse(atob(token.split(".")[1]));
  }
}
