export interface ResponseModel {
    code: number,
    message: string,
    data: any,
    success: boolean,
    errors: Array<string>,
    unAuthorized: boolean
}
