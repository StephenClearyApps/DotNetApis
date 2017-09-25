import { IPackage } from './util/structure/packages';

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

export type Status = "Requested" | "Succeeded" | "Failed";
export interface StatusResponse {
    status: Status;
    logUri: string;
}

export function isInProgressResponse<T>(response: InProgressResponse | T): response is InProgressResponse {
    return (response as any).operationId !== undefined;
}

function check422(response: Response) {
    if (response.status === 422) {
        window.location.reload(true);
        throw new Error(response.statusText);
    }
}

function checkResponseError(response: Response, json: ErrorDetails) {
    if (!response.ok) {
        const error: ResponseError = { status: response.status, message: response.statusText, details: json };
        throw error;
    }
}

async function get<T>(url: string, query: { [key:string]: string }) {
    const response = await fetch(url + "?" + encodeQuery({ ...query, jsonVersion }));
    check422(response);
    const json = await response.json();
    checkResponseError(response, json);
    return json as T;
}

export const getDoc = ({ packageId, packageVersion, targetFramework }: PackageKey) =>
    get<InProgressResponse | IPackage>("http://localhost:7071/api/0/doc", { packageId, packageVersion, targetFramework});

export async function getStatus(packageId: string, packageVersion: string, targetFramework: string, timestamp: string): Promise<StatusResponse> {
    const response = await fetch("http://localhost:7071/api/0/status?" + encodeQuery({ jsonVersion, packageId, packageVersion, targetFramework, timestamp }));
    check422(response);
    if (response.status === 404) {
        return {
            status: "Requested",
            logUri: null
        }
    }
    const json = await response.json();
    checkResponseError(response, json);
    return json as StatusResponse;
}