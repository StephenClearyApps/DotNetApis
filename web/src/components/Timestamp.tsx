import * as React from "react";
import * as moment from "moment";

export interface TimestampProps {
    timestamp: number;
    fullTimestamp: boolean;
}

export const Timestamp: React.StatelessComponent<TimestampProps> = ({ timestamp, fullTimestamp }) => {
    if (fullTimestamp)
        return <span>{timestamp ? moment(timestamp).format('YYYY-MM-DD HH:mm:ss.SSS') : "---------- ------------"}</span>;
    else
        return <span>{timestamp ? moment(timestamp).format('HH:mm:ss.SSS') : '------------'}</span>;
}