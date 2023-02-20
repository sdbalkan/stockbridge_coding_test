import { Component, OnInit } from '@angular/core';
import { GreetingService } from 'src/app/_services/greeting.service';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css']
})
export class WelcomeComponent implements OnInit {
  greeting = ''

  constructor(private greetingService: GreetingService) { }

  ngOnInit(): void {
    this.greetingService.getGreeting().subscribe(result => this.greeting = result);
  }

}
