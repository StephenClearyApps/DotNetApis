import { MessageBase } from "./messages";
import { getJsonResponse } from "./util";
import { without$ } from "../util";

export interface InProgressResponse extends MessageBase {
    _type: "InProgressResponse";
    normalizedPackageId: string;
    normalizedPackageVersion: string;
    normalizedFrameworkTarget: string;
}

export interface RedirectResponse extends MessageBase {
    _type: "RedirectResponse";
    normalizedPackageId: string;
    normalizedPackageVersion: string;
    normalizedFrameworkTarget: string;
    logUri: string;
    jsonUri: string;
}

export function isInProgressResponse(response: InProgressResponse | RedirectResponse): response is InProgressResponse {
    return response._type === "InProgressResponse";
}

export async function getDoc(key: PackageKey): Promise<InProgressResponse | RedirectResponse> {
    const response = await getJsonResponse<InProgressResponse | RedirectResponse>(BACKEND + "0/doc", {...without$(key)});
    if (response.status === 202)
        return {...response.json as InProgressResponse, _type: "InProgressResponse"};
    return {...response.json as RedirectResponse, _type: "RedirectResponse"};
}