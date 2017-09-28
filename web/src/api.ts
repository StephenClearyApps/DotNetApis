import { IPackage } from './util/structure/packages';

// This must be kept in sync with DotNetApis.Structure.JsonFactory.Version.
export const jsonVersion = "0";

function encodeQuery(data: { [key:string]: string }) {
    return Object.keys(data).filter(key => typeof(data[key]) !== "undefined").map(key =>
        [key, data[key]].map(encodeURIComponent).join("=")
    ).join("&");
}

export interface LogMessage {
    type?: string;
    timestamp?: number;
    message: string;
}

export interface ErrorDetails {
    message: string;
    exceptionMessage: string;
    exceptionType: string;
    stackTrace: string;
    operationId: string;
    requestId: string;
    log: LogMessage[];
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
    log: LogMessage[];
}

export type Status = "Requested" | "Succeeded" | "Failed";
export interface StatusResponse {
    status: Status;
    logUri: string;
    jsonUri: string;
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

export async function getJson<T>(url: string, query?: { [key:string]: string }) {
    const uri = query === undefined ? url : url + "?" + encodeQuery({ ...query, jsonVersion });
    const response = await fetch(uri);
    check422(response);
    const json = await response.json();
    checkResponseError(response, json);
    return json as T;
}

export const getDoc = ({ packageId, packageVersion, targetFramework }: PackageKey) =>
    getJson<InProgressResponse | IPackage>("http://localhost:7071/api/0/doc", { packageId, packageVersion, targetFramework});

export const getStatus = (packageId: string, packageVersion: string, targetFramework: string, timestamp: string) =>
    getJson<StatusResponse>("http://localhost:7071/api/0/status", { packageId, packageVersion, targetFramework, timestamp });