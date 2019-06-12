import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material";
import {URLs} from "@app/constants/URLS";

@Component({
    selector: 'app-video-player',
    templateUrl: './video-player.component.html',
    styleUrls: ['./video-player.component.scss']
})
export class VideoPlayerComponent implements OnInit {

    url: string = "";
    filename: string = "";

    constructor(@Inject(MAT_DIALOG_DATA) public data: any, @Inject('BASE_URL') baseUrl: string) {
        this.url = `${baseUrl}${data.src}`;
        this.filename = data.name;
    }

    ngOnInit() {
    }

}
