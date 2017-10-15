export interface MessageBase {
    operationId: string;
    requestId: string;
    log: LogMessage[];
}

export interface LogMessage {
    type?: string;
    timestamp?: number;
    message: string;
}

export interface ExceptionDetails {
    exceptionType: string;
    exceptionMessage: string;
    stackTrace: string;
    innerException?: ExceptionDetails;
}

export interface ErrorDetails extends MessageBase, ExceptionDetails {
    // This type does have a "message", but it's always "An error has occurred". Thanks, we know that...
}

export class ResponseError extends Error {
    constructor(public status: number, message: string) {
        super();
        this.name = "ResponseError";
        this.message = message;
    }

    details?: ErrorDetails;
}

export function isResponseError(error: Error): error is ResponseError {
    return error.name === "ResponseError";
}