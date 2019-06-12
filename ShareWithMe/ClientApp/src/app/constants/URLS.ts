export const URLs = {
    user: {
        auth: 'api/Auth/Login',
        register: 'api/User/Create',
        update: 'api/User/Update',
        get: 'api/User/get?username=',
        updateProfileImage: 'api/User/UpdateProfileImage',
        currentUserDetails: 'api/Auth/GetCurrentUserDetails',
        resetPassword: 'api/Auth/ResetPassword',
        getAll: 'api/User/GetAll',
    },
    files: {
        getAll: 'api/Files/GetAll',
        get: 'api/Files/Get?id=',
        getDeletedFiles: 'api/Files/GetDeletedFiles',
        create: 'api/Files/CreateDirectory',
        delete: 'api/Files/Delete?id=',
        rename: 'api/Files/Rename',
        move: 'api/Files/Move',
        restore: 'api/Files/Restore',
        upload: 'api/Files/UploadFile',
        download: 'api/Files/Download?id=',
        share: 'api/Files/Share',
        getSharedFiles: 'api/Files/GetSharedFiles'
    },
};
