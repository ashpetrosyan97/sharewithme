import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material';
import {Directory} from "@app/helpers/Directory";

@Component({
    selector: 'app-rename-dialog',
    templateUrl: './rename-dialog.component.html',
    styleUrls: ['./rename-dialog.component.scss']
})
export class RenameDialogComponent implements OnInit {
    constructor(@Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<RenameDialogComponent>, public dialog: MatDialog) {
        this.folderName = data.folderName;
    }

    valid: boolean = true;
    folderName: string;

    handleInput(e) {
        this.folderName = e.target.value;
        this.valid = Directory.ValidateFolderName(name);
    }

    handleKeydown(e) {
        if (e.which === 13 && this.valid && this.folderName !== this.data.folderName) {
            this.dialogRef.close(this.folderName)
        }
        if (e.which === 27) {
            this.dialog.closeAll()
        }
    }

    ngOnInit() {
    }
}
