import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class DataStoreService {

    public syncingFiles = [];
    public allFiles = [];
    public sharedFiles = [];
    public deletedFiles = [];
    private storageInfo: BehaviorSubject<{ size, percent }> = new BehaviorSubject<{ size, percent }>({
        size: 0,
        percent: 0
    });

    getStorageInfo: Observable<{ size, percent }>;

    setStorageInfo(newVal) {
        this.storageInfo.next(newVal);
    }

    constructor() {
        this.getStorageInfo = this.storageInfo.asObservable();
    }
}
