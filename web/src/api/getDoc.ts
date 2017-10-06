import { MessageBase } from "./messages";
import { getJsonResponse } from "./util";

export interface InProgressResponse extends MessageBase {
    _type: "InProgressResponse";
    timestamp: string;
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

export async function getDoc({ packageId, packageVersion, targetFramework }: PackageKey): Promise<InProgressResponse | RedirectResponse> {
    const response = await getJsonResponse<InProgressResponse | RedirectResponse>("http://localhost:7071/api/0/doc", { packageId, packageVersion, targetFramework });
    if (response.status === 202)
        return {...response.json as InProgressResponse, _type: "InProgressResponse"};
    return {...response.json as RedirectResponse, _type: "RedirectResponse"};
}