import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material";
import {FileService} from "@app/service/file.service";

@Component({
    selector: 'app-properties',
    templateUrl: './properties.component.html',
    styleUrls: ['./properties.component.scss']
})
export class PropertiesComponent {
    fileInfo: any;

    constructor(@Inject(MAT_DIALOG_DATA) public data: any, private fileService: FileService) {
        fileService.getFile(data.id).subscribe((response) => this.fileInfo = response.data);

    }

}
