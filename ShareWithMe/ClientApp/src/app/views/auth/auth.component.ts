import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {AuthService} from "@app/service/auth.service";
import {MatDialog, MatDialogConfig} from "@angular/material";
import {PasswordValidator} from "@app/helpers/PasswordValidator";
import {AuthenticationModel} from "@app/models/authenticationModel";
import {CreateUserModel} from "@app/models/createUserModel";
import {MessageBoxComponent} from "@app/components/message-box/message-box.component";
import {ResponseModel} from "@app/models/responseModel";
import {Observable} from "rxjs";
import {SignalRService} from "@app/service/signal-r.service";

@Component({
    selector: 'app-login',
    templateUrl: './auth.component.html',
    styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {

    signInForm: FormGroup;
    signUpForm: FormGroup;
    hasAccount: boolean = true;
    color = 'primary';

    constructor(private router: Router,
                private signalR: SignalRService,
                private authService: AuthService,
                public dialog: MatDialog) {
    }

    ngOnInit() {
        this.signInForm = new FormGroup({
            'username': new FormControl(null, Validators.required),
            'password': new FormControl(null, Validators.required)
        });
        this.signUpForm = new FormGroup({
            'name': new FormControl(null, Validators.required),
            'surname': new FormControl(null, Validators.required),
            'username': new FormControl(null, Validators.required),
            'email': new FormControl(null, Validators.required),
            'country': new FormControl(null, Validators.required),
            'phoneNumber': new FormControl(null, Validators.required),
            'accountId': new FormControl(1, Validators.required),
            'password': new FormControl('', Validators.required),
            'confirm': new FormControl(null, [PasswordValidator.Validate, Validators.required])
        });
    }

    hasError(control: string, error: string): boolean {
        return this.signUpForm.controls[control].hasError(error);
    }

    onPasswordInput() {
        this.signUpForm.controls['confirm'].updateValueAndValidity();
    }

    LogIn() {
        let model: AuthenticationModel = {
            password: this.signInForm.value.password,
            userName: this.signInForm.value.username
        };
        this.authService.login(model).subscribe((response: ResponseModel) => {
            if (response['success'] === true) {
                localStorage.setItem('token', response.data['accessToken']);
                this.signalR.connect().subscribe(() => this.router.navigate(['', 'files']));
            } else {
                this.OpenModal(response.errors, response.message);
            }
        })
    }

    Register() {
        let model: CreateUserModel = {
            accountId: this.signUpForm.value['accountId'],
            country: this.signUpForm.value['country'],
            email: this.signUpForm.value['email'],
            name: this.signUpForm.value['name'],
            password: this.signUpForm.value['password'],
            phoneNumber: this.signUpForm.value['phoneNumber'],
            surname: this.signUpForm.value['surname'],
            username: this.signUpForm.value['username']
        };
        this.authService.register(model).subscribe((data: ResponseModel) => {
            if (data.code === 200) {
                let input: AuthenticationModel = {
                    password: this.signUpForm.value.password,
                    userName: this.signUpForm.value.username
                };
                this.authService.login(input)
                    .subscribe((response: ResponseModel) => {
                        localStorage.setItem('token', response.data['accessToken']);
                        this.signalR.connect().subscribe(() => this.router.navigate(['', 'files']));
                    })
            } else {
                this.OpenModal(data.errors, 'Registration failed');
            }
        })
    }

    Switch() {
        this.hasAccount = !this.hasAccount;
        this.color = this.hasAccount ? 'primary' : 'warn';
    }

    OpenModal(messages: Array<string>, title: string): Observable<any> {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = true;
        dialogConfig.autoFocus = true;
        dialogConfig.data = {
            title: title,
            content: messages,
        };
        const dialogRef = this.dialog.open(MessageBoxComponent, dialogConfig);
        return dialogRef.afterClosed()
    }
}
