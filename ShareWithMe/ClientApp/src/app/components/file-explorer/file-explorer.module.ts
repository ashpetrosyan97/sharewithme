import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {MatToolbarModule} from '@angular/material/toolbar';
import {FlexLayoutModule} from '@angular/flex-layout';
import {MatIconModule} from '@angular/material/icon';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatMenuModule} from '@angular/material/menu';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MatDialogModule} from '@angular/material/dialog';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {FormsModule} from '@angular/forms';
import {NewFolderDialogComponent} from './modals/new-folder-dialog/new-folder-dialog.component';
import {RenameDialogComponent} from './modals/rename-dialog/rename-dialog.component';
import {FileExplorerComponent} from './file-explorer.component';
import {MatProgressSpinnerModule} from '@angular/material';
import {LongPressDirective} from '@app/long-press.directive';

@NgModule({
    imports: [
        CommonModule,
        MatToolbarModule,
        FlexLayoutModule,
        MatIconModule,
        MatGridListModule,
        MatMenuModule,
        BrowserAnimationsModule,
        MatDialogModule,
        MatInputModule,
        FormsModule,
        MatButtonModule,
        MatProgressSpinnerModule,
    ],
    declarations: [FileExplorerComponent, NewFolderDialogComponent, RenameDialogComponent,LongPressDirective],
    exports: [FileExplorerComponent],
    entryComponents: [NewFolderDialogComponent, RenameDialogComponent]
})
export class FileExplorerModule {
}
