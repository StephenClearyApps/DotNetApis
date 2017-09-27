import * as React from "react";
import * as moment from "moment";

import { LogMessage } from "../reducers";

export function LogMessage(props: { message: LogMessage }) {
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
        {timestamp ? moment(timestamp).toString() : null} {message}
    </div>);
}