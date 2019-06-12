import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {JwtHelperService} from "@auth0/angular-jwt";
import {Router} from "@angular/router";
import {CreateUserModel} from "../models/createUserModel";
import {AuthenticationModel} from "../models/authenticationModel";
import {Mapper} from "../helpers/Mapper";
import {URLs} from "../constants/URLS";
import {updateUserModel} from "../models/updateUserModel";
import {Observable} from "rxjs";
import {ResponseModel} from "@app/models/responseModel";
import {map} from "rxjs/operators/map";


@Injectable({
    providedIn: 'root'
})
export class AuthService {
    base_url:string = "";
    constructor(private http: HttpClient, public jwtHelper: JwtHelperService, private router: Router,@Inject('BASE_URL') baseUrl: string) {
        this.base_url = baseUrl;
    }


    register(model: CreateUserModel): Observable<ResponseModel> {
        return this.http.post(this.base_url + URLs.user.register, Mapper.MapToFormData(model))
            .pipe(map((data: ResponseModel) => data))
    }

    login(model: AuthenticationModel): Observable<ResponseModel> {
        return this.http.post(this.base_url + URLs.user.auth, Mapper.MapToFormData(model))
            .pipe(map((data: ResponseModel) => data))
    }

    logout() {
        localStorage.clear();
        this.router.navigate(['', 'auth']);
    }

    resetPassword(model) {
        return this.http.post(this.base_url + URLs.user.resetPassword, Mapper.MapToFormData(model))
            .pipe(map((data: ResponseModel) => data))
    }

    get(username): Observable<ResponseModel> {
        return this.http.get(`${this.base_url}${URLs.user.get}${username}`, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        })
            .pipe(map((data: ResponseModel) => data))
    }

    getAll() {
        return this.http.get(`${this.base_url}${URLs.user.getAll}`, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data))
    }

    Edit(model: updateUserModel): Observable<ResponseModel> {
        return this.http.put(this.base_url + URLs.user.update, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data))
    }

    updateProfileImage(model): Observable<ResponseModel> {
        return this.http.put(this.base_url + URLs.user.updateProfileImage, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data))
    }

    getUserInfo() {
        return this.http.get(this.base_url + URLs.user.currentUserDetails, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data))

    }

    isAuthenticated(): boolean {
        return !this.jwtHelper.isTokenExpired(localStorage.getItem('token'));
    }

}
