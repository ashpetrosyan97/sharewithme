import {Inject, Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs/BehaviorSubject';
import {Observable} from 'rxjs/Observable';
import {FileElement} from '../components/file-explorer/model/file-element';
import {HttpClient, HttpEventType} from '@angular/common/http';
import {URLs} from '../constants/URLS';
import {DataStoreService} from './data-store.service';
import {Mapper} from '../helpers/Mapper';
import {map} from 'rxjs/operators';
import {ResponseModel} from '@app/models/responseModel';
import * as FileSaver from 'file-saver';
import {SignalRService} from '@app/service/signal-r.service';
import {ShareFileModel} from '@app/models/ShareFileModel';

export interface IFileService {
    add(fileElement: FileElement);

    delete(model: FileElement);

    queryInFolder(folderId: string): Observable<FileElement[]>;

    get(id: string): FileElement;

    clear(): void
}


@Injectable()
export class FileService implements IFileService {
    private map = new Map<string, FileElement>();

    constructor(private http: HttpClient, private dataStore: DataStoreService, private signalR: SignalRService, @Inject('BASE_URL') public baseUrl: string) {
    }

    getAll(): Observable<ResponseModel> {
        return this.http.get(`${this.baseUrl}${URLs.files.getAll}`, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));

    }

    getDeletedFiles() {
        this.signalR.connect();
        return this.http.get(`${this.baseUrl}${URLs.files.getDeletedFiles}`, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        })
            .pipe(map((data: ResponseModel) => data))
            .subscribe((response) => {
                this.dataStore.deletedFiles = Mapper.MapDeletedFileModel(response.data.files);
            });
    }

    getSharedFiles() {
        return this.http.get(`${this.baseUrl}${URLs.files.getSharedFiles}`, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        })
            .pipe(map((data: ResponseModel) => data));
    }

    createDir(model: FileElement): Observable<ResponseModel> {
        if (model.parentId === 'root') {
            model.parentId = '0';
        }
        return this.http.post(this.baseUrl + URLs.files.create, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));

    }

    delete(model: FileElement): Observable<ResponseModel> {
        return this.http.delete(`${this.baseUrl}${URLs.files.delete}${model.id}`, {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));
    }

    rename(model: FileElement): Observable<ResponseModel> {
        delete model.type;
        delete model.parentId;
        return this.http.put(`${this.baseUrl}${URLs.files.rename}`, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));

    }

    move(model): Observable<ResponseModel> {
        return this.http.patch(`${this.baseUrl}${URLs.files.move}`, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));
    }

    getFile(id): Observable<ResponseModel> {
        return this.http.get(`${this.baseUrl}${URLs.files.get}${id}`, {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));
    }

    restore(model): Observable<ResponseModel> {
        return this.http.put(`${this.baseUrl}${URLs.files.restore}`, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));
    }


    upload(model): Observable<any> {
        this.signalR.uploadListener();

        let current = this.dataStore.syncingFiles.find(x => x.uid === model.uid);

        return current.request = this.http.post(`${this.baseUrl}${URLs.files.upload}`, Mapper.MapToFormData(model), {
            reportProgress: true,
            observe: 'events',
            responseType: 'json',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map(event => {
            if (event.type === HttpEventType.UploadProgress) {
                current.loaded = Math.round(100 * event.loaded / event.total);
            } else if (event.type === HttpEventType.Response) {
                if (!event.body['success']) {
                    this.dataStore.syncingFiles.splice(this.dataStore.syncingFiles.indexOf(current), 1);
                }
                return event.body;
            }
        }));
    }

    download(elm) {
        let current = this.dataStore.syncingFiles.find(x => x.uid === elm.uid);
        current.request = this.http.get(`${this.baseUrl}${URLs.files.download}${elm.id}`, {
            responseType: 'blob',
            reportProgress: true,
            observe: 'events',
            headers: {
                'Content-Type': 'application/octet-stream',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).subscribe((event) => {
            if (event.type === HttpEventType.DownloadProgress) {
                current.loaded = Math.round(100 * event.loaded / event.total);
            } else if (event.type === HttpEventType.Response) {
                this.dataStore.syncingFiles.splice(this.dataStore.syncingFiles.indexOf(current), 1);
                FileSaver.saveAs(event.body, elm.name);
            }
        });

    }


    share(model: ShareFileModel) {
        return this.http.post(`${this.baseUrl}${URLs.files.share}`, Mapper.MapToFormData(model), {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).pipe(map((data: ResponseModel) => data));
    }

    add(fileElement: FileElement) {
        this.map.set(fileElement.id, {...fileElement});
        return fileElement;
    }


    private querySubject: BehaviorSubject<FileElement[]>;

    queryInFolder(folderId: string) {
        const result: FileElement[] = [];
        this.map.forEach(element => {
            if (element.parentId === folderId) {
                result.push({...element});
            }
        });
        if (!this.querySubject) {
            this.querySubject = new BehaviorSubject(result);
        } else {
            this.querySubject.next(result);
        }
        return this.querySubject.asObservable();
    }

    get(id: string) {
        return this.map.get(id);
    }

    clear(): void {
        this.map.clear();
    }
}
