// This must be kept in sync with DotNetApis.Structure.JsonFactory.Version.
export const jsonVersion = "0";

function encodeQuery(data: { [key:string]: string }) {
    return Object.keys(data).filter(key => typeof(data[key]) !== "undefined").map(key =>
        [key, data[key]].map(encodeURIComponent).join("=")
    ).join("&");
}

export interface ErrorDetails {
    message: string;
    exceptionMessage: string;
    exceptionType: string;
    stackTrace: string;
    operationId: string;
    requestId: string;
    log: string[];
}

export interface ResponseError {
    status: number;
    message: string;
    details: ErrorDetails;
}

export interface InProgressResponse {
    timestamp: string;
    normalizedPackageId: string;
    normalizedPackageVersion: string;
    normalizedFrameworkTarget: string;
    operationId: string;
    requestId: string;
    log: string[];
}

export function isInProgressResponse<T>(response: InProgressResponse | T): response is InProgressResponse {
    return (response as any).operationId !== undefined;
}

async function get<T>(url: string, query: { [key:string]: string }) {
    const response = await fetch(url + "?" + encodeQuery({ ...query, jsonVersion }));
    if (response.status === 422) {
        window.location.reload(true);
        throw new Error(response.statusText);
    }
    const json = await response.json();
    if (!response.ok) {
        const error: ResponseError = { status: response.status, message: response.statusText, details: json };
        throw error;
    }
    return json as T;
}

export const getDoc = ({ packageId, packageVersion, targetFramework }: PackageKey) =>
    get<InProgressResponse | PackageDoc>("http://localhost:7071/api/0/doc", { packageId, packageVersion, targetFramework});