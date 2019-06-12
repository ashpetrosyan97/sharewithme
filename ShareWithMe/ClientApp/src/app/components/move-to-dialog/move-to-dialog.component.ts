import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatTreeFlatDataSource, MatTreeFlattener} from '@angular/material';
import {FlatTreeControl} from '@angular/cdk/tree';
import { of } from 'rxjs';

@Component({
    selector: 'app-move-to-dialog',
    templateUrl: './move-to-dialog.component.html',
    styleUrls: ['./move-to-dialog.component.scss']
})
export class MoveToDialogComponent {
    treeControl: FlatTreeControl<TreeNode>;

    treeFlattener: MatTreeFlattener<FileNode, TreeNode>;

    dataSource: MatTreeFlatDataSource<FileNode, TreeNode>;

    title: string;
    action: string;

    constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
        this.title = data.title;
        this.action = data.action;
        this.treeFlattener = new MatTreeFlattener(
            this.transformer,
            this.getLevel,
            this.isExpandable,
            this.getChildren);

        this.treeControl = new FlatTreeControl<TreeNode>(this.getLevel, this.isExpandable);
        this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
        this.dataSource.data = [...data.directoryTree];
    }


    transformer(node: FileNode, level: number) {
        return {
            name: node.name,
            id: node.id,
            type: node.type,
            level: level,
            expandable: node.children.length > 0
        };
    }

    getLevel(node: TreeNode) {
        return node.level;
    }

    isExpandable(node: TreeNode) {
        return node.expandable;
    };

    getChildren(node) {
        return of(node.children);
    }

    hasChild(index: number, node: TreeNode) {
        return node.expandable;
    }

    selectedDir: any;

    handleRadioChange(node) {
        this.selectedDir = node;
    }

}

export interface FileNode {
    name: string;
    id: number;
    type: number;
    children?: FileNode[];
}

export interface TreeNode {
    name: string;
    id: number;
    type: number;
    level: number;
    expandable: boolean;
}
