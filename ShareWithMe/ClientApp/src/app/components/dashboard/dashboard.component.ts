import {ChangeDetectorRef, Component, Inject, OnDestroy} from '@angular/core';
import {MediaMatcher} from '@angular/cdk/layout';
import {AuthService} from '@app/service/auth.service';
import {SignalRService} from '@app/service/signal-r.service';
import {DataStoreService} from '@app/service/data-store.service';

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

    mobileQuery: MediaQueryList;

    menuItems = [
        {route: 'files', name: 'Files', type: 'link', icon: 'folder'},
        {route: 'profile', name: 'Profile', type: 'link', icon: 'person'},
        {route: 'deleted', name: 'Deleted Files', type: 'link', icon: 'restore_from_trash'},
        {route: 'shared', name: 'Shared with me', type: 'link', icon: 'share'},
    ];

    private readonly _mobileQueryListener: () => void;
    FullName: string = '';
    user: any;
    imgSrc = 'assets/images/profile.png';
    base_url: string = '';

    constructor(changeDetectorRef: ChangeDetectorRef,
                media: MediaMatcher,
                private signalR: SignalRService,
                private authService: AuthService,
                @Inject('BASE_URL') baseUrl: string,
                public  dataStore: DataStoreService
    ) {
        this.mobileQuery = media.matchMedia('(min-width: 768px)');
        this._mobileQueryListener = () => changeDetectorRef.detectChanges();
        this.mobileQuery.addListener(this._mobileQueryListener);
        this.updateUserData();
        this.base_url = baseUrl;
        signalR.connect();
    }


    showHide(snav) {
        snav.toggle();
    }

    logOut() {
        this.authService.logout();
    }

    selectImage(e) {
        this.authService.updateProfileImage({id: this.user.id, img: e.target.files[0]})
            .subscribe(data => {
                if (data.success) {
                    location.reload();
                }
            });
    }

    updateUserData() {
        this.authService.getUserInfo()
            .subscribe((response) => {
                this.user = response.data.user;
                this.FullName = `${this.user.name} ${this.user.surname}`;
                if (this.user.profileImage) {
                    this.imgSrc = `${this.base_url}${this.user.profileImage}`;
                }
            });
    }
}
