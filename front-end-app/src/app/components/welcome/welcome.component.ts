import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { GreetingService } from 'src/app/_services/greeting.service';
import { LogoutService } from 'src/app/_services/logout.service';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css']
})
export class WelcomeComponent implements OnInit {
  greeting: string = ''

  constructor(private greetingService: GreetingService, private authService: AuthService, private logoutService: LogoutService) {
    this.logoutService.connect();
  }

  ngOnInit(): void {
    this.greetingService.getGreeting().subscribe(result => this.greeting = result);
    this.logoutService.messages?.subscribe(msg => {
      console.log("Received from Websocket: " + JSON.stringify(msg));
    });
  }

  logout() {
    this.logoutService.messages?.unsubscribe();
    this.authService.logout();
  }
}
