import {Component, OnInit} from '@angular/core';
import {MatDialog, MatDialogConfig} from '@angular/material';
import {EditUserComponent} from '../edit-user/edit-user.component';
import {AuthService} from "@app/service/auth.service";


@Component({
    selector: 'app-user-info',
    templateUrl: './user-info.component.html',
    styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {

    constructor(private authService: AuthService, public dialog: MatDialog) {
    }

    userInfoModel: UserInfoModel = {
        name: '',
        surname: '',
        country: '',
        username: '',
        email: '',
        phoneNumber: ''
    };
    keys = Object.keys(this.userInfoModel);
    user: any;

    ngOnInit() {
        this.updateUserData();
    }

    Edit() {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = true;
        dialogConfig.autoFocus = true;
        dialogConfig.data = {
            userData: this.user
        };
        const dialogRef = this.dialog.open(EditUserComponent, dialogConfig);
        return new Promise((resolve, reject) => {
            dialogRef.afterClosed()
                .subscribe(result => resolve(result),
                    (err) => reject(err));
        }).then(() => this.updateUserData());
    }

    updateUserData() {
        this.authService.getUserInfo()
            .subscribe((response) => {
                this.user = response.data.user;
                for (let key in this.userInfoModel) {
                    this.userInfoModel[key] = this.user[key];
                }
            })
    }

}

interface UserInfoModel {
    email: string
    name: string
    country: string
    phoneNumber: string
    surname: string
    username: string
}
