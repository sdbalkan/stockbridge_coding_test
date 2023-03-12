import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { AuthService } from 'src/app/_services/auth.service';
import { LogoutService } from 'src/app/_services/logout.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  form: any = {
    username: null,
    password: null
  };
  isLoggedIn = false;
  isLoginFailed = false;
  errorMessage = '';
  loading = false;

  constructor(private router: Router, private authService: AuthService) { }

  ngOnInit(): void {
  }

  onSubmit(): void {
    const { username, password } = this.form;

    this.loading = true;
    this.authService.login(username, password).pipe(first())
      .subscribe(
        data => {
          console.log("Login successful");
          this.isLoggedIn = true;
          this.router.navigate(['/welcome']);
        },
        error => {
          console.log(error);
          this.isLoginFailed = true;
          this.errorMessage = error?.error?.message;
          this.loading = false;
        });
  }

}
