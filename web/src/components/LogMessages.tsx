import * as React from "react";

import { LogMessage as LogMessageType } from "../reducers";
import { LogMessage } from "./LogMessage";

export function LogMessages(props: { messages: LogMessageType[] }) {
    const { messages } = props;
    return (
        <div style={{fontFamily: "Menlo,Monaco,Consolas,Courier New,monospace", lineHeight: 1.42857143, wordBreak: 'break-all', wordWrap: 'break-word', backgroundColor: 'black'}}>
            {messages.map((x, index) => <LogMessage key={index} message={x} />)}
        </div>
    );
}