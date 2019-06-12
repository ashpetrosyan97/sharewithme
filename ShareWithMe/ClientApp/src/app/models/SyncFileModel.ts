export interface SyncFileModel {
    uid: string,
    action: string
    loaded?: number,
    name: string
}

export interface UploadFileModel extends SyncFileModel {
    file: Blob,
    type: number,
    parentId: string,
}


export interface DownloadFileModel extends SyncFileModel {
    id: number,
}
