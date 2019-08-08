import {Inject, Injectable} from '@angular/core';
import * as signalR from '@aspnet/signalr';
import {URLs} from '@app/constants/URLS';
import {DataStoreService} from '@app/service/data-store.service';
import {BehaviorSubject, from, Observable} from 'rxjs';
import 'rxjs/add/observable/fromPromise';

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: signalR.HubConnection;

    constructor(private dataStorage: DataStoreService, @Inject('BASE_URL') public baseUrl: string) {
    }

    public isConnected: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    public connect(): Observable<any> {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${this.baseUrl}progressHub`, {
                transport: signalR.HttpTransportType.LongPolling,
                accessTokenFactory(): string | Promise<string> {
                    return localStorage.getItem('token');
                }
            })
            .build();
        
        if (!this.isConnected.getValue()) {
            return Observable.fromPromise(
                this.hubConnection
                    .start()
                    .then(() => {
                        console.log('Connection started');
                        this.storageInfoListener();
                        this.uploadListener();
                        this.isConnected.next(true);
                    })
                    .catch(err => console.log('Error while starting connection: ' + err)));
        } else {
            return Observable.fromPromise(new Promise(resolve => resolve(true)));
        }

    };

    public disconnect(): Observable<any> {
        return Observable.fromPromise(this.hubConnection.stop());
    }

    public uploadListener() {
        this.hubConnection.on('SendUploadPercent', (uid, percent) => {
            let current = this.dataStorage.syncingFiles.find(x => x.uid === uid);
            if (percent === 100) {
                this.dataStorage.syncingFiles.splice(this.dataStorage.syncingFiles.indexOf(current), 1);
            } else {
                current.uploaded = percent;
            }
        });
    }

    public storageInfoListener() {
        this.hubConnection.on('SendStorageInfo', (size, percent) => {
            this.dataStorage.setStorageInfo({size, percent});
        });
    }

}
