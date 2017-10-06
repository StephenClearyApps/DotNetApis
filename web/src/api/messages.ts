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

export interface ErrorDetails extends MessageBase {
    message: string;
    exceptionMessage: string;
    exceptionType: string;
    stackTrace: string;
}

export interface ResponseError {
    status: number;
    message: string;
    details?: ErrorDetails;
}
