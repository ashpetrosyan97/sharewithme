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
import {SignalRService} from "@app/service/signal-r.service";


@Injectable({
    providedIn: 'root'
})
export class AuthService {

    constructor(private http: HttpClient, public jwtHelper: JwtHelperService, private router: Router, public signalR: SignalRService, @Inject('BASE_URL') public baseUrl: string) {
    }


    register(model: CreateUserModel): Observable<ResponseModel> {
        return this.http.post(this.baseUrl + URLs.user.register, Mapper.MapToFormData(model))
            .pipe(map((data: ResponseModel) => data))
    }

    login(model: AuthenticationModel): Observable<ResponseModel> {
        return this.http.post(this.baseUrl + URLs.user.auth, Mapper.MapToFormData(model))
            .pipe(map((data: ResponseModel) => data))
    }

    logout() {
        this.signalR.disconnect()
            .subscribe(_ => {
                localStorage.clear();
                this.router.navigate(['auth']);
            })
    }

    resetPassword(model) {
        return this.http.post(this.baseUrl + URLs.user.resetPassword, Mapper.MapToFormData(model))
            .pipe(map((data: ResponseModel) => data))
    }

    get(username): Observable<ResponseModel> {
        return this.http.get(`${this.baseUrl}${URLs.user.get}${username}`, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        })
            .pipe(map((data: ResponseModel) => data))
    }

    getAll() {
        return this.http.get(`${this.baseUrl}${URLs.user.getAll}`, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data))
    }

    Edit(model: updateUserModel): Observable<ResponseModel> {
        return this.http.put(this.baseUrl + URLs.user.update, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data))
    }

    updateProfileImage(model): Observable<ResponseModel> {
        return this.http.put(this.baseUrl + URLs.user.updateProfileImage, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data))
    }

    getUserInfo() {
        return this.http.get(this.baseUrl + URLs.user.currentUserDetails, {
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
