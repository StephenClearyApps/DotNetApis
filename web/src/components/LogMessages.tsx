import * as React from "react";

import { LogMessage as LogMessageType } from "../api";
import { LogMessage } from "./LogMessage";

export interface LogMessagesProps {
    messages: LogMessageType[];
    currentTimestamp: number;
}

export const LogMessages: React.StatelessComponent<LogMessagesProps> = ({ messages, currentTimestamp }) => {
    const oneDayAgo = currentTimestamp - 24 * 60 * 60 * 1000;
    const fullTimestamp = messages.some(x => x.timestamp ? x.timestamp < oneDayAgo : false);
    return (
        <div className="logs">
            {messages.map((x, index) => <LogMessage key={index} message={x} fullTimestamp={fullTimestamp} />)}
        </div>
    );
}