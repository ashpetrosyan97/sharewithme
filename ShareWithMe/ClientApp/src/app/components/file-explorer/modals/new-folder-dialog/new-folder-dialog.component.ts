import {Component, OnInit} from '@angular/core';
import {MatDialog, MatDialogRef} from "@angular/material";
import {Directory} from "@app/helpers/Directory";

@Component({
    selector: 'app-new-folder-dialog',
    templateUrl: './new-folder-dialog.component.html',
    styleUrls: ['./new-folder-dialog.component.scss']
})
export class NewFolderDialogComponent implements OnInit {
    constructor(public dialogRef: MatDialogRef<NewFolderDialogComponent>, public dialog: MatDialog) {
    }

    valid: boolean = false;
    folderName: string;

    handleInput(e) {
        this.folderName = e.target.value;
        this.valid = Directory.ValidateFolderName(e.target.value);
    }

    handleKeydown(e) {
        if (e.which === 13 && this.valid) {
            this.dialogRef.close(this.folderName)
        }
        if (e.which === 27) {
            this.dialog.closeAll()
        }
    }

    ngOnInit() {
    }
}
