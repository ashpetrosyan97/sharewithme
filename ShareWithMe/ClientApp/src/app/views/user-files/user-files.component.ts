import {Component, OnInit} from '@angular/core';
import {Observable} from 'rxjs';
import {FileElement} from "@app/components/file-explorer/model/file-element";
import {FileService} from "@app/service/file.service";
import {MatDialog, MatDialogConfig} from "@angular/material";
import {ResponseModel} from "@app/models/responseModel";
import {DownloadFileModel, UploadFileModel} from "@app/models/SyncFileModel";
import {MessageBoxComponent} from "@app/components/message-box/message-box.component";
import {DataStoreService} from "@app/service/data-store.service";
import {v4 as uuid} from 'uuid';
import {VideoPlayerComponent} from "@app/components/video-player/video-player.component";
import {ShareFileComponent} from "@app/components/share-file/share-file.component";
import {PropertiesComponent} from "@app/components/properties/properties.component";

@Component({
    selector: 'app-user-files',
    templateUrl: './user-files.component.html',
    styleUrls: ['./user-files.component.scss']
})
export class UserFilesComponent implements OnInit {

    public fileElements: Observable<FileElement[]>;

    constructor(public fileService: FileService, public dialog: MatDialog, private dataStore: DataStoreService) {
    }

    currentRoot: FileElement;
    currentPath: string;
    canNavigateUp = false;

    async ngOnInit() {
        this.reloadData();
    }

    reloadData() {
        this.fileService.clear();
        this.fileService.getAll()
            .subscribe((response) => {
                this.dataStore.allFiles = [...response.data.files];
                response.data.files.forEach(elm => {
                    let fileElement: FileElement = {
                        id: elm.id,
                        type: elm.type,
                        name: elm.name,
                        parentId: elm.parentId === 0 ? 'root' : elm.parentId
                    };
                    this.fileService.add(fileElement);
                });
                this.updateFileElementQuery();
            })
    }

    addFolder(folder: { name: string }) {
        this.fileService.createDir({
            type: 0,
            name: folder.name,
            parentId: this.currentRoot ? this.currentRoot.id : 'root'
        }).subscribe((response: ResponseModel) => {
            if (response.success) {
                this.reloadData();
            } else {
                this.OpenModal(response.errors, response.message);
            }
        });
    }

    removeElement(element: FileElement) {
        this.fileService.delete(element)
            .subscribe((response: ResponseModel) => {
                if (response.success) {
                    this.reloadData();
                } else {
                    this.OpenModal(response.errors, response.message);
                }
            });
    }

    navigateToFolder(element: FileElement) {
        this.currentRoot = element;
        this.updateFileElementQuery();
        this.currentPath = UserFilesComponent.pushToPath(this.currentPath, element.name);
        this.canNavigateUp = true;
    }

    navigateUp() {
        if (this.currentRoot && this.currentRoot.parentId === 'root') {
            this.currentRoot = null;
            this.canNavigateUp = false;
            this.updateFileElementQuery();
        } else {
            this.currentRoot = this.fileService.get(this.currentRoot.parentId);
            this.updateFileElementQuery();
        }
        this.currentPath = UserFilesComponent.popFromPath(this.currentPath);
    }

    upload(file) {
        let model: UploadFileModel = {
            uid: uuid(),
            type: 1,
            parentId: this.currentRoot ? this.currentRoot.id : 'root',
            name: file.name,
            file: file,
            action: "upload"
        };
        this.dataStore.syncingFiles.push(model);
        this.fileService.upload(model)
            .subscribe((response) => {
                if (response && response.success) {
                    this.reloadData();
                } else if (response) {
                    this.OpenModal(response.errors, response.message);
                }
            })
    }

    download(elm) {
        let model: DownloadFileModel = {
            action: "download",
            uid: uuid(),
            id: elm.id,
            name: elm.name,
            loaded: 0,
        };
        this.dataStore.syncingFiles.push(model);
        this.fileService.download(model)
    }

    view(elm) {
        this.fileService.getFile(elm.id)
            .subscribe((response) => {
                const dialogConfig = new MatDialogConfig();
                dialogConfig.disableClose = false;
                dialogConfig.autoFocus = true;
                dialogConfig.data = {
                    src: response.data.path,
                    name: response.data.name,
                };
                this.dialog.open(VideoPlayerComponent, dialogConfig);
            })
    }

    elementShared(elm) {
        this.fileService.getFile(elm.id).subscribe((res) => {
            let oldData = [...res.data.usersSharedFiles.map((e) => {
                return e.user
            })];
            const dialogConfig = new MatDialogConfig();
            dialogConfig.disableClose = false;
            dialogConfig.autoFocus = true;
            dialogConfig.data = {
                sharedUsers: [...oldData]
            };

            const dialogRef = this.dialog.open(ShareFileComponent, dialogConfig);

            dialogRef.afterClosed()
                .subscribe((result: Array<any>) => {
                    if (result && (!oldData.every(item => result.includes(item)) || result.length !== oldData.length))
                        this.fileService.share({fileId: elm.id, userId: [...result.map(u => u.id)]})
                            .subscribe((data) => console.log(data));
                });
        })
    }

    properties(elm) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        dialogConfig.data = {id: elm.id};
        const dialogRef = this.dialog.open(PropertiesComponent, dialogConfig);
        return dialogRef.afterClosed()
    }

    moveElement(event: { element: FileElement; moveTo: FileElement }) {
        let elm = {...event.element, parentId: event.moveTo.id};
        this.fileService.move(elm)
            .subscribe((response) => {
                if (response.success) {
                    this.reloadData();
                } else {
                    this.OpenModal(response.errors, response.message);
                }
            })
    }

    renameElement(element: FileElement) {
        this.fileService.rename(element)
            .subscribe((response: ResponseModel) => {
                if (response.success) {
                    this.reloadData();
                } else {
                    this.OpenModal(response.errors, response.message);
                }
            });
    }

    updateFileElementQuery() {
        this.fileElements = this.fileService.queryInFolder(this.currentRoot ? this.currentRoot.id : 'root');
    }

    static pushToPath(path: string, folderName: string) {
        let p = path ? path : '';
        p += `${folderName}/`;
        return p;
    }

    static popFromPath(path: string) {
        let p = path ? path : '';
        let split = p.split('/');
        split.splice(split.length - 2, 1);
        p = split.join('/');
        return p;
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
