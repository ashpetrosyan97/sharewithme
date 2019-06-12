import {Component, OnInit, ViewChild} from '@angular/core';
import {MatDialog, MatDialogConfig, MatPaginator, MatSort, MatTableDataSource} from "@angular/material";
import {VideoPlayerComponent} from "@app/components/video-player/video-player.component";
import {FileService} from "@app/service/file.service";
import {DataStoreService} from "@app/service/data-store.service";

@Component({
    selector: 'app-shared-files',
    templateUrl: './shared-files.component.html',
    styleUrls: ['./shared-files.component.scss']
})
export class SharedFilesComponent implements OnInit {

    displayedColumns: string[] = ['name', 'size', 'duration', 'owner'];
    dataSource: MatTableDataSource<any>;

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    constructor(private fileService: FileService,
                private dataStore: DataStoreService, public dialog: MatDialog) {
        this.reloadData()
    }

    ngOnInit() {
    }

    reloadData() {
        this.fileService.getSharedFiles()
            .subscribe((response) => {
                this.dataStore.sharedFiles = [...response.data.files];
                this.dataSource = new MatTableDataSource(response.data.files);
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
            })
    }

    view(elm) {
        this.fileService.getFile(elm.id)
            .subscribe((response) => {
                const dialogConfig = new MatDialogConfig();
                dialogConfig.disableClose = false;
                dialogConfig.autoFocus = true;
                dialogConfig.data = {
                    src: response.data.path,
                    name: response.data.name
                };
                this.dialog.open(VideoPlayerComponent, dialogConfig);
            })
    }
}
