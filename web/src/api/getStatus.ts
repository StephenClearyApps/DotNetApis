import { getJson } from "./util";
import { MessageBase } from "./messages";

export type Status = "Requested" | "Succeeded" | "Failed";
export interface StatusResponse extends MessageBase {
    status: Status;
    logUri: string;
    jsonUri: string;
}

export const getStatus = (packageId: string, packageVersion: string, targetFramework: string, timestamp: string) =>
    getJson<StatusResponse>("http://localhost:7071/api/0/status", { packageId, packageVersion, targetFramework, timestamp });