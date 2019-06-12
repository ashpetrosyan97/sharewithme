import {FormControl} from '@angular/forms';


export class PasswordValidator {
    public static Validate(AC: FormControl) {
        let password = AC.parent && AC.parent.controls['password'].value;
        let confirmPassword = AC.value;
        if (password === confirmPassword) {
            return null; // All ok, passwords match!!!
        } else {
            return {'not_match': true};
        }
    }
}
