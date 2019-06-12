import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/observable/throw';

import {LoadingOverlayRef, LoadingService} from './loading.service';
import {URLs} from "@app/constants/URLS";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
    constructor(private loadingService: LoadingService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let loadingRef: LoadingOverlayRef;

        if (req.url.includes(URLs.files.download) || req.url.includes(URLs.files.upload)) {
            return next.handle(req)
        }

        Promise.resolve(null).then(() => loadingRef = this.loadingService.open());
        return next.handle(req).do(event => {
            if (event instanceof HttpResponse && loadingRef) {
                loadingRef.close();
            }
        }).catch(error => {
            if (loadingRef) {
                loadingRef.close();
            }

            return Observable.throw(error);
        });
    }
}
