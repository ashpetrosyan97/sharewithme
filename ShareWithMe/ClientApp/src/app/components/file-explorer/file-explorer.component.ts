import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {MatMenuTrigger} from '@angular/material/menu';
import {MatDialog, MatDialogConfig} from '@angular/material/dialog';
import {FileElement} from './model/file-element';
import {NewFolderDialogComponent} from './modals/new-folder-dialog/new-folder-dialog.component';
import {RenameDialogComponent} from './modals/rename-dialog/rename-dialog.component';
import {DataStoreService} from '@app/service/data-store.service';
import {MoveToDialogComponent} from '../move-to-dialog/move-to-dialog.component';
import {Directory} from '@app/helpers/Directory';

@Component({
    selector: 'file-explorer',
    templateUrl: './file-explorer.component.html',
    styleUrls: ['./file-explorer.component.scss']
})
export class FileExplorerComponent implements OnInit {
    constructor(public dialog: MatDialog, private dataStore: DataStoreService) {
    }

    ngOnInit(): void {
    }

    @Input() fileElements: FileElement[];
    @Input() canNavigateUp: string;
    @Input() path: string;
    @Output() folderAdded = new EventEmitter<{ name: string }>();
    @Output() elementRemoved = new EventEmitter<FileElement>();
    @Output() elementRenamed = new EventEmitter<{ name: string }>();
    @Output() navigatedDown = new EventEmitter<FileElement>();
    @Output() elementMoved = new EventEmitter<{ element: FileElement; moveTo: FileElement }>();
    @Output() navigatedUp = new EventEmitter();
    @Output() fileSelected = new EventEmitter<any>();
    @Output() downloadFile = new EventEmitter<FileElement>();
    @Output() viewFile = new EventEmitter<FileElement>();
    @Output() shareElement = new EventEmitter<FileElement>();
    @Output() elementProps = new EventEmitter<FileElement>();


    deleteElement(element: FileElement) {
        this.elementRemoved.emit(element);
    }

    navigate(element: FileElement) {
        if (element.type === 0) {
            this.navigatedDown.emit(element);
        }
    }

    navigateUp() {
        this.navigatedUp.emit();
    }

    moveElement(element: FileElement, moveTo: FileElement) {
        this.elementMoved.emit({element: element, moveTo: moveTo});
    }

    openNewFolderDialog() {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        dialogConfig.role = 'dialog';
        let dialogRef = this.dialog.open(NewFolderDialogComponent, dialogConfig);
        dialogRef.afterClosed()
            .subscribe(res => {
                if (res) {
                    this.folderAdded.emit({name: res});
                    this.dialog.closeAll();
                }
            });
    }

    openRenameDialog(element: FileElement) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        dialogConfig.data = {
            folderName: element.name,
        };
        let dialogRef = this.dialog.open(RenameDialogComponent, dialogConfig);
        dialogRef.afterClosed().subscribe(res => {
            if (res) {
                let fileElement: FileElement = {...element, name: res};
                this.elementRenamed.emit(fileElement);
                this.dialog.closeAll();
            }
        });
    }

    openMoveToDialog(element: FileElement) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = true;
        dialogConfig.autoFocus = true;
        dialogConfig.data = {
            title: 'Select moving directory',
            action: 'Move',
            directoryTree: [...Directory.filterDirectories(this.dataStore.allFiles, element.id)],
        };
        const dialogRef = this.dialog.open(MoveToDialogComponent, dialogConfig);
        dialogRef.afterClosed()
            .subscribe(result => {
                if (result) {
                    this.moveElement(element, result);
                }
            });

    }

    openMenu(event: MouseEvent, element: FileElement, viewChild: MatMenuTrigger) {
        event.preventDefault();
        viewChild.openMenu();
    }

    download(elm) {
        this.downloadFile.emit(elm);
    }

    view(elm) {
        this.viewFile.emit(elm);
    }

    share(elm) {
        this.shareElement.emit(elm);
    }

    properties(elm) {
        this.elementProps.emit(elm);
    }

    handleFileChange(e) {
        this.fileSelected.emit(e.target.files[0]);
    }
}
