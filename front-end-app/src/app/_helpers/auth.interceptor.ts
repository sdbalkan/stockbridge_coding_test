import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

const TOKEN_HEADER_KEY = 'x-user-token';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor() { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let authReq = request;
    let strUser = localStorage.getItem('user');
    let user: User | null = null;
    
    if (strUser) {
      user = JSON.parse(strUser);
    }

    if (user?.token != null) {
      authReq = request.clone({ headers: request.headers.set(TOKEN_HEADER_KEY, user.token) });
    }

    return next.handle(authReq);
  }
}

export const authInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
];