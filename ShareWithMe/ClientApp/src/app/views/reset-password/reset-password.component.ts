import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "@app/service/auth.service";
import {Router} from "@angular/router";

@Component({
    selector: 'app-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

    firstFormGroup: FormGroup;
    secondFormGroup: FormGroup;
    user: any;
    email = "";
    validEmail = true;

    constructor(private _formBuilder: FormBuilder, private authService: AuthService, private router: Router) {
    }

    ngOnInit() {
        this.firstFormGroup = this._formBuilder.group({
            username: ['', Validators.required]
        });
        this.secondFormGroup = this._formBuilder.group({
            email: ['', Validators.required]
        });
    }

    getUser(stepper) {
        this.authService.get(this.firstFormGroup.controls.username.value)
            .subscribe(data => {
                console.log(data)
                if (data.success) {
                    this.user = data.data;
                    for (let i = 0; i < this.user.email.length; i++) {
                        if (i == 0 || i >= 5) {
                            this.email += this.user.email[i]
                        } else
                            this.email += '*';
                    }
                    stepper.next()
                }
            })
    }

    verifyEmail(stepper) {
        console.log(this.secondFormGroup.controls.email.value !== this.user.email)
        if (this.secondFormGroup.controls.email.value !== this.user.email) {
            this.validEmail = false
        } else {
            stepper.next();
        }
    }

    resetPassword() {
        this.authService.resetPassword({username: this.user.username, email: this.user.email})
            .subscribe(resp => {
                if (resp.success) {
                    this.router.navigate(['', 'auth'])
                }
            })
    }

}
