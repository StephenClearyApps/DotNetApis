import * as React from "react";
import * as moment from "moment";

import { Timestamp } from "./Timestamp";
import { LogMessage } from "../reducers/packageDocReducer";

export function LogMessage(props: { message: LogMessage, fullTimestamp: boolean }) {
    const { type, timestamp, message } = props.message;
    let textColor;
    if (type === 'Information') {
        textColor = 'blue';
    } else if (type === 'Warning') {
        textColor = 'yellow';
    } else if (type === 'Error') {
        textColor = 'red';
    } else {
        textColor = 'white';
    }
    return (
    <div style={{textIndent: '-2em', marginLeft: '2em', color: textColor}}>
        <Timestamp timestamp={timestamp} fullTimestamp={props.fullTimestamp} /> {message}
    </div>);
}