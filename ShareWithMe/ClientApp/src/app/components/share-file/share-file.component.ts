import {Component, ElementRef, Inject, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatAutocomplete, MatAutocompleteSelectedEvent} from "@angular/material";
import {AuthService} from "@app/service/auth.service";
import {COMMA, ENTER} from "@angular/cdk/keycodes";
import {FormControl} from "@angular/forms";
import {Observable} from "rxjs";
import {map, startWith} from "rxjs/operators";

@Component({
    selector: 'app-share-file',
    templateUrl: './share-file.component.html',
    styleUrls: ['./share-file.component.scss']
})
export class ShareFileComponent {
    selectable = true;
    removable = true;
    addOnBlur = true;
    separatorKeysCodes: number[] = [ENTER, COMMA];
    userCtrl = new FormControl();
    filteredUsers: Observable<Array<any>>;
    users: Array<any> = [];
    allUsers = [];
    @ViewChild('userInput') userInput: ElementRef<HTMLInputElement>;

    @ViewChild('auto') matAutocomplete: MatAutocomplete;


    constructor(@Inject(MAT_DIALOG_DATA) public data: any, private authService: AuthService) {
        this.authService.getAll()
            .subscribe(resp => {
                this.users = [...this.data.sharedUsers];
                this.allUsers = [...resp.data];
                this.sync();
                this.filteredUsers = this.userCtrl.valueChanges.pipe(
                    startWith(null),
                    map((user) => user ? this._filter(user) : [...this.allUsers]));
                this.userInput.nativeElement.blur();
            });
    }


    remove(user): void {
        this.allUsers.push(user);
        this.userCtrl.patchValue('');
        this.userInput.nativeElement.blur();
        if (user.id >= 0) {
            this.users = [...this.users.filter(u => u.id !== user.id)];
        }
    }

    selected(event: MatAutocompleteSelectedEvent): void {
        const value = event.option.value;
        this.users.push(value);
        this.sync();

        this.userInput.nativeElement.blur();

    }

    private _filter(value) {
        const filterValue = value.hasOwnProperty('name') ? value.name.toLowerCase() : value.toLowerCase();
        return this.allUsers.filter(user =>
            user.name.toLowerCase().startsWith(filterValue) ||
            user.email.toLowerCase().startsWith(filterValue) ||
            user.surname.toLowerCase().startsWith(filterValue));
    }

    sync() {
        this.allUsers = [...this.allUsers.filter(x => !this.users.some(u => u.id === x.id))];
        this.userCtrl.patchValue('');
    }


}
