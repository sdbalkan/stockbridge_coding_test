import { ApiResponse } from "./apiResponse";

export class AuthorizeResponse extends ApiResponse {
    data: {
        userGroup: number;
        token: string | undefined;
    } | undefined;
}