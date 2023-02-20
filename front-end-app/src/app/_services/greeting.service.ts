import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { first, map } from 'rxjs/operators';
import { GreetingResponse } from '../_models/greetingResponse';

const GREETING_API = 'http://66.70.229.82:8181/GetGreeting';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class GreetingService {

  constructor(private http: HttpClient) { }

  getGreeting(): Observable<string> {
    return this.http.get<GreetingResponse>(GREETING_API, httpOptions).pipe(map(result => result.data || ''));
  }
}
