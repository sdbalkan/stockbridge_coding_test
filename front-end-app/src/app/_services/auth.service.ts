import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../_models/user';
import { map } from 'rxjs/operators';
import { AuthorizeResponse } from '../_models/authorize-response';

const AUTH_API = 'http://66.70.229.82:8181/Authorize';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject: BehaviorSubject<User| null>;
  public user: Observable<User | null>;

  constructor(private http: HttpClient) {
    let strUser = localStorage.getItem('user')    
    let user;
    if (strUser) {
      user = JSON.parse(strUser);
    }

    this.userSubject = new BehaviorSubject<User | null>(user);
    this.user = this.userSubject.asObservable();
  }

  login(email: string, password: string) {
    return this.http.post<AuthorizeResponse>(AUTH_API, { email, password }, httpOptions)
      .pipe(map(result => {
        console.log(result);

        let user = new User();
        user.token = result?.data?.token;
        user.username = email;
        
        console.log(user);

        localStorage.setItem('user', JSON.stringify(user));
        this.userSubject.next(user);

        return user;
      }));;
  }

  logout() {
    localStorage.removeItem('user');
    this.userSubject.next(null);
  }
}
