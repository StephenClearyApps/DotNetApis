import * as React from 'react';
import { CircularProgress } from 'material-ui';

export function LoadingMessage(props: { message: string }) {
    return (
        <div style={{ textAlign: "center" }}>
            <div>{props.message}</div>
            <div><CircularProgress/></div>
        </div>
    );
}
