import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { RegisterUser } from '../models/register-user';
import { CompareValidation } from '../validators/custom-validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  isRegisterFormSubmitted: boolean = false;

  constructor(private accountService: AccountService, private router: Router) {
    this.registerForm = new FormGroup({
      personName: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      phoneNumber: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required]),
      confirmPassword: new FormControl(null, [Validators.required])
    },
      {
        validators: [CompareValidation("password","confirmPassword")]
      });
  }
  //creating get properties of the controls
  get register_personNameControl(): any {
    return this.registerForm.controls["personName"];
  }

  get register_emailControl(): any {
    return this.registerForm.controls["email"];
  }

  get register_phoneNumberControl(): any {
    return this.registerForm.controls["phoneNumber"];
  }

  get register_passwordControl(): any {
    return this.registerForm.controls["password"];
  }

  get register_confirmPasswordControl(): any {
    return this.registerForm.controls["confirmPassword"];
  }

  registerSubmitted() {
    this.isRegisterFormSubmitted = true;
    if (this.registerForm.valid) {
      //calling Respective Service
      this.accountService.postRegister(this.registerForm.value)
        .subscribe({
          next: (response: RegisterUser) => {
            //When succeeded, printing response on the log
            console.log(response);

            //Making form back to the initial state
            this.isRegisterFormSubmitted = false;

            //Redirecting to cities component
            this.router.navigate(['/cities']);

            //Resetting the form, so that it remain a new form
            this.registerForm.reset();
          },
          error: (error) => {
            console.log(error)
          },
          complete: () => { }
        });
    }

  }
}