import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogConfig} from '@angular/material';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {AuthService} from "@app/service/auth.service";
import {PasswordValidator} from "@app/helpers/PasswordValidator";
import {updateUserModel} from "@app/models/updateUserModel";
import {Observable} from "rxjs";
import {MessageBoxComponent} from "@app/components/message-box/message-box.component";


@Component({
    selector: 'app-edit-user',
    templateUrl: './edit-user.component.html',
    styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit {

    modalTitle: string = 'Edit personal information';
    updateUserForm: FormGroup;
    user: any;

    constructor(@Inject(MAT_DIALOG_DATA) public data: any,
                private authService: AuthService,
                private dialog: MatDialog) {
        this.user = data.userData;
    }

    ngOnInit() {
        this.updateUserForm = new FormGroup({
            'name': new FormControl(this.user['name'], Validators.required),
            'surname': new FormControl(this.user['surname'], Validators.required),
            'username': new FormControl({value: this.user['username'], disabled: true}, Validators.required),
            'email': new FormControl(this.user['email'], Validators.required),
            'country': new FormControl(this.user['country'], Validators.required),
            'phoneNumber': new FormControl(this.user['phoneNumber'], Validators.required),
            'accountId': new FormControl(this.user['accountId'], Validators.required),
            'password': new FormControl('', [Validators.required, Validators.minLength(6)]),
            'confirm': new FormControl('', PasswordValidator.Validate)
        });
    }

    onPasswordInput() {
        this.updateUserForm.controls['confirm'].updateValueAndValidity();
    }

    Submit() {
        let model: updateUserModel = {
            id: this.user.id,
            accountId: this.user.accountId,
            country: this.updateUserForm.controls['country'].value,
            email: this.updateUserForm.controls['email'].value,
            name: this.updateUserForm.controls['name'].value,
            password: this.updateUserForm.controls['password'].value,
            phoneNumber: this.updateUserForm.controls['phoneNumber'].value,
            surname: this.updateUserForm.controls['surname'].value,
            username: this.updateUserForm.controls['username'].value,
        };
        this.authService.Edit(model)
            .subscribe((response) => {
                if (response.success === true)
                    this.dialog.closeAll();
                else this.OpenModal(response.errors, "Editing failed");
            })
    }

    hasError(control: string, error: string): boolean {
        return this.updateUserForm.controls[control].hasError(error);
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


