import { ApiResponse } from "./api-response";

export class AuthorizeResponse extends ApiResponse {
    data: {
        userGroup: number;
        token: string | undefined;
    } | undefined;
}