import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {RoutesForNav, RoutesForRoot} from './app.routing';
import {AppComponent} from './app.component';
import {FlexLayoutModule} from '@angular/flex-layout';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MaterialModule} from './material-module';
import {UserFilesComponent} from "./views/user-files/user-files.component";
import {UserInfoComponent} from "./views/user-info/user-info.component";
import {AuthComponent} from "./views/auth/auth.component";
import {MessageBoxComponent} from "./components/message-box/message-box.component";
import {EditUserComponent} from "./views/edit-user/edit-user.component";
import {MoveToDialogComponent} from "./components/move-to-dialog/move-to-dialog.component";
import {UsedSpaceComponent} from "./components/used-space/used-space.component";
import {DashboardComponent} from "./components/dashboard/dashboard.component";
import {FileExplorerModule} from "./components/file-explorer/file-explorer.module";
import {JwtModule} from "@auth0/angular-jwt";
import {FileService} from "./service/file.service";
import {AuthGuardService} from "./service/auth-guard.service";
import {DataStoreService} from "./service/data-store.service";
import {FileUploaderComponent} from "./components/file-uploader/file-uploader.component";
import {ChartsModule} from "ng2-charts";
import {DeletedFilesComponent} from './views/deleted-files/deleted-files.component';
import {ResetPasswordComponent} from './views/reset-password/reset-password.component';
import {LoadingComponent} from './components/loading/loading.component';
import {LoadingService} from "@app/service/loading.service";
import {LoadingInterceptor} from "@app/service/loading-interceptor";
import {VideoPlayerComponent} from './components/video-player/video-player.component';
import {ShareFileComponent} from './components/share-file/share-file.component';
import {SharedFilesComponent} from './views/shared-files/shared-files.component';
import {MAT_CHIPS_DEFAULT_OPTIONS} from "@angular/material";
import {COMMA, ENTER} from "@angular/cdk/keycodes";
import {PropertiesComponent} from './components/properties/properties.component';
import {DurationPipe} from './duration.pipe';

@NgModule({
  declarations: [
    AppComponent,
    UserFilesComponent,
    UserInfoComponent,
    AuthComponent,
    MessageBoxComponent,
    EditUserComponent,
    FileUploaderComponent,
    MoveToDialogComponent,
    UsedSpaceComponent,
    DashboardComponent,
    DeletedFilesComponent,
    ResetPasswordComponent,
    LoadingComponent,
    VideoPlayerComponent,
    ShareFileComponent,
    SharedFilesComponent,
    PropertiesComponent,
    DurationPipe,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MaterialModule,
    FormsModule,
    FlexLayoutModule,
    HttpClientModule,
    ReactiveFormsModule,
    FileExplorerModule,
    ChartsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter
      }
    }),
    RouterModule.forRoot(RoutesForRoot),
    RouterModule.forChild(RoutesForNav)
  ],
  providers: [FileService, AuthGuardService, DataStoreService, LoadingService, {
    provide: HTTP_INTERCEPTORS,
    useClass: LoadingInterceptor,
    multi: true
  }, {
    provide: MAT_CHIPS_DEFAULT_OPTIONS,
    useValue: {
      separatorKeyCodes: [ENTER, COMMA]
    }
  }],
  bootstrap: [AppComponent],
  entryComponents: [
    PropertiesComponent,
    MessageBoxComponent,
    EditUserComponent,
    MoveToDialogComponent,
    LoadingComponent,
    VideoPlayerComponent,
    ShareFileComponent
  ]
})
export class AppModule {
}


export function tokenGetter() {
  return localStorage.getItem('token');
}
