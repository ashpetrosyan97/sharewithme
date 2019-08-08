import {Routes} from '@angular/router';
import {DashboardComponent} from "./components/dashboard/dashboard.component";
import {AuthGuardService} from "./service/auth-guard.service";
import {UserFilesComponent} from "./views/user-files/user-files.component";
import {UserInfoComponent} from "./views/user-info/user-info.component";
import {AuthComponent} from "./views/auth/auth.component";
import {DeletedFilesComponent} from "@app/views/deleted-files/deleted-files.component";
import {ResetPasswordComponent} from "@app/views/reset-password/reset-password.component";
import {SharedFilesComponent} from "@app/views/shared-files/shared-files.component";
import {LoginGuardService} from "@app/service/login-guard.service";


export const RoutesForNav: Routes = [
    {
        path: '',
        component: DashboardComponent,
        canActivate: [AuthGuardService],
        children: [
            {
                path: 'files',
                component: UserFilesComponent
            },
            {
                path: 'profile',
                component: UserInfoComponent
            },
            {
                path: 'shared',
                component: SharedFilesComponent
            },
            {
                path: 'deleted',
                component: DeletedFilesComponent
            }
        ]
    }
];
export const RoutesForRoot: Routes = [
    {path: 'auth', component: AuthComponent, canActivate: [LoginGuardService]},
    {path: 'forgotPass', component: ResetPasswordComponent, canActivate: [LoginGuardService]},
    {path: '', redirectTo: '/files', pathMatch: 'full', canActivate: [AuthGuardService]},
];
