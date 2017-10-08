import * as React from "react";
import * as moment from "moment";

import { Timestamp } from "./Timestamp";
import { LogMessage } from "../api";

export function LogMessage(props: { message: LogMessage, fullTimestamp: boolean }) {
    const { type, timestamp, message } = props.message;
    return (
    <div className={"message" + (type ? (" " + type) : "")}>
        <Timestamp timestamp={timestamp} fullTimestamp={props.fullTimestamp} /> {message}
    </div>);
}