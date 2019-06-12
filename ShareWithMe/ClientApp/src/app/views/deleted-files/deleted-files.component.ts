import {Component, ViewChild} from '@angular/core';
import {MatDialog, MatDialogConfig, MatPaginator, MatSort, MatTableDataSource} from '@angular/material';
import {DataStoreService} from "@app/service/data-store.service";
import {FileService} from "@app/service/file.service";
import {DeletedFileModel} from "@app/models/deletedFileModel";
import {Directory} from "@app/helpers/Directory";
import {MoveToDialogComponent} from "@app/components/move-to-dialog/move-to-dialog.component";
import {MessageBoxComponent} from "@app/components/message-box/message-box.component";
import {AuthService} from "@app/service/auth.service";
import {PropertiesComponent} from "@app/components/properties/properties.component";

@Component({
    selector: 'app-deleted-files',
    templateUrl: './deleted-files.component.html',
    styleUrls: ['./deleted-files.component.scss']
})
export class DeletedFilesComponent {
    displayedColumns: string[] = ['actions', 'name', 'deletionTime'];
    dataSource: MatTableDataSource<DeletedFileModel>;

    // @ts-ignore
    @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
    // @ts-ignore
    @ViewChild(MatSort, {static: true}) sort: MatSort;

    constructor(private fileService: FileService, private authService: AuthService, public dialog: MatDialog, private dataStore: DataStoreService) {
        this.reloadData();
    }

    applyFilter(filterValue: string) {
        this.dataSource.filter = filterValue.trim().toLowerCase();

        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }


    restore(element) {
        this.authService.getUserInfo()
            .subscribe((response) => {
                if (response.data.user.account.type === 1) {
                    this.OpenMessageBox('Attention', ['Restore option is available only for Pro account'])
                } else {
                    this.OpenRestoreModal()
                        .subscribe((result) => {
                            if (result) {
                                this.fileService.restore({parentId: result.id, id: element.id})
                                    .subscribe((data) => {
                                        if (data.success)
                                            this.reloadData();
                                        else
                                            this.OpenMessageBox(data.message, data.errors)
                                    })
                            }
                        })
                }
            });
    }

    OpenRestoreModal() {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = true;
        dialogConfig.autoFocus = true;
        dialogConfig.data = {
            title: "Select where to restore",
            action: "Restore",
            directoryTree: [...Directory.filterDirectories(this.dataStore.allFiles, null)],
        };
        const dialogRef = this.dialog.open(MoveToDialogComponent, dialogConfig);

        return dialogRef.afterClosed()
    }

    OpenMessageBox(title, messages) {
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

    properties(elm) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.disableClose = false;
        dialogConfig.autoFocus = true;
        dialogConfig.data = {id: elm.id};
        const dialogRef = this.dialog.open(PropertiesComponent, dialogConfig);
        return dialogRef.afterClosed()
    }


    reloadData() {
        this.fileService.getDeletedFiles()
            .add(() => {
                this.dataSource = new MatTableDataSource(this.dataStore.deletedFiles);
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
            });
    }

}
