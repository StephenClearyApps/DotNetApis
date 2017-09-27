import * as React from "react";
import * as moment from "moment";

import { LogMessage } from "../reducers/packageDocReducer";

const shortNbsp = '\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0';
const fullNbsp = '\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0';
export function Timestamp(props: { timestamp: number, fullTimestamp: boolean }) {
    const { timestamp, fullTimestamp } = props;
    if (fullTimestamp)
        return <span>{timestamp ? moment(timestamp).format('YYYY-MM-DD HH:mm:ss.SSS') : "---------- ------------"}</span>;
    else
        return <span>{timestamp ? moment(timestamp).format('HH:mm:ss.SSS') : '------------'}</span>;
}