import { ErrorDetails, ResponseError } from "./messages";

// This must be kept in sync with DotNetApis.Structure.JsonFactory.Version.
export const jsonVersion = "0";

/** Query parameters */
interface Query {
    [key:string]: string
}

/** Encodes the query for use in a URI */
function encodeQuery(data: Query) {
    return Object.keys(data).filter(key => typeof(data[key]) !== "undefined").map(key =>
        [key, data[key]].map(encodeURIComponent).join("=")
    ).join("&");
}

/** Checks the response to see if 422 is returned (indicating our client is out of date), and refreshes if so */
function check422(response: Response) {
    if (response.status === 422) {
        window.location.reload(true);
        throw new Error(response.statusText);
    }
}

/** Checks the response for an error and throws an error containing error details returned from the API */
function checkResponseError(response: Response, json: ErrorDetails) {
    if (!response.ok) {
        const error: ResponseError = { status: response.status, message: response.statusText, details: json };
        throw error;
    }
}

/** A parsed JSON response along with the response status code */
export interface JsonResponse<T> {
    /** The response status code */
    status: number;

    /** The JSON response */
    json: T;
}

/** Invokes an API method and returns the JSON response along with the response status code */
export async function getJsonResponse<T>(url: string, query?: Query): Promise<JsonResponse<T>> {
    const uri = query === undefined ? url : url + "?" + encodeQuery({ ...query, jsonVersion });
    const response = await fetch(uri);
    check422(response);
    const json = await response.json();
    checkResponseError(response, json);
    return { status: response.status, json };
}

/** Invokes an API method and returns the JSON response */
export async function getJson<T>(url: string, query?: Query): Promise<T> {
    const response = await getJsonResponse<T>(url, query);
    return response.json;
}

/** Invokes a non-api url and just returns the response as json; no special api-specific processing is done */
export async function getPlainJson<T>(url: string): Promise<T> {
    const response = await fetch(url);
    if (!response.ok) {
        const error: ResponseError = { status: response.status, message: response.statusText };
        throw error;
    }
    return await response.json();
}