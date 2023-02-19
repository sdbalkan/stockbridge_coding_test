export class AuthorizeResponse {
    data: {
        userGroup: number;
        token: string | undefined;
    } | undefined;
    status: number | undefined;
    error: string | undefined;
}