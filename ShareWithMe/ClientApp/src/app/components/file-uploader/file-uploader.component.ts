import {Component, OnInit} from '@angular/core';
import {DataStoreService} from "@app/service/data-store.service";

@Component({
    selector: 'app-file-uploader',
    templateUrl: './file-uploader.component.html',
    styleUrls: ['./file-uploader.component.scss']
})
export class FileUploaderComponent implements OnInit {
    panelOpenState = true;

    constructor(public dataStore: DataStoreService) {
    }

    ngOnInit() {
    }

    cancelUpload(item) {
        item.request.subscribe().unsubscribe();
        let index = this.dataStore.syncingFiles.findIndex(x => x.uid === item.uid);
        this.dataStore.syncingFiles.splice(index, 1);
    }

}
