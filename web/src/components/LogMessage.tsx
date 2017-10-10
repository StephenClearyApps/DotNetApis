import * as React from "react";
import * as moment from "moment";

import { Timestamp } from "./Timestamp";
import { LogMessage as LogMessageType } from "../api";

export interface LogMessageProps {
    message: LogMessageType;
    fullTimestamp: boolean;
}

export const LogMessage: React.StatelessComponent<LogMessageProps> = ({ message, fullTimestamp }) => {
    const { type, timestamp } = message;
    return (
    <div className={"message" + (type ? (" " + type) : "")}>
        <Timestamp timestamp={timestamp} fullTimestamp={fullTimestamp} /> {message.message}
    </div>);
}