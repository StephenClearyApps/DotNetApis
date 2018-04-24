import * as React from "react";
import { format } from "date-fns";

export interface TimestampProps {
    timestamp?: number;
    fullTimestamp: boolean;
}

export const Timestamp: React.StatelessComponent<TimestampProps> = ({ timestamp, fullTimestamp }) => {
    if (fullTimestamp)
        return <span>{timestamp ? format(timestamp, 'YYYY-MM-DD HH:mm:ss.SSS') : "---------- ------------"}</span>;
    else
        return <span>{timestamp ? format(timestamp, 'HH:mm:ss.SSS') : '------------'}</span>;
}