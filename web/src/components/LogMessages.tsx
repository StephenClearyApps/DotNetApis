import * as React from "react";

import { LogMessage as LogMessageType } from "../api";
import { LogMessage } from "./LogMessage";

export function LogMessages(props: { messages: LogMessageType[], currentTimestamp: number }) {
    const { messages } = props;
    const oneDayAgo = props.currentTimestamp - 24 * 60 * 60 * 1000;
    const fullTimestamp = messages.some(x => x.timestamp && x.timestamp < oneDayAgo);
    return (
        <div className="logs">
            {messages.map((x, index) => <LogMessage key={index} message={x} fullTimestamp={fullTimestamp} />)}
        </div>
    );
}