import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { LoginUser } from '../models/login-user';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoginFormSubmitted: boolean = false;

  constructor(private accountService: AccountService, private router: Router) {
    this.loginForm = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required]),
    })
  };
  //creating get properties of the controls
  get login_emailControl(): any {
    return this.loginForm.controls["email"];
  }

  get login_passwordControl(): any {
    return this.loginForm.controls["password"];
  }

  loginSubmitted() {
    this.isLoginFormSubmitted = true;
    if (this.loginForm.valid) {
      //calling Respective Service
      this.accountService.postLogin(this.loginForm.value)
        .subscribe({
          next: (response: LoginUser) => {
            //When succeeded, printing response on the log
            console.log(response);

            //Making form back to the initial state
            this.isLoginFormSubmitted = false;

            //storing current working username
            this.accountService.currentUserName = response.email;
            //Redirecting to cities component
            this.router.navigate(['/cities']);

            //Resetting the form, so that it remain a new form
            this.loginForm.reset();
          },
          error: (error) => {
            console.log(error)
          },
          complete: () => { }
        });
    }
  }
}
