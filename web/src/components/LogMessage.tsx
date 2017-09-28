import * as React from "react";
import * as moment from "moment";

import { Timestamp } from "./Timestamp";
import { LogMessage } from "../api";

const base01 = '#586e75'; // Comments / secondary content
const base0 = '#839496'; // Default
const base1 = '#93a1a1'; // Emphasized content
const blue = '#268bd2';
const yellow = '#b58900';
const red = '#dc322f';

export function LogMessage(props: { message: LogMessage, fullTimestamp: boolean }) {
    const { type, timestamp, message } = props.message;
    return (
    <div className={"message" + (type ? (" " + type) : "")}>
        <Timestamp timestamp={timestamp} fullTimestamp={props.fullTimestamp} /> {message}
    </div>);
}