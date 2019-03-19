import * as React from "react";
import Card from "@material-ui/core/Card";
import CardHeader from "@material-ui/core/CardHeader";
import { CardHeaderProps } from "@material-ui/core/CardHeader";
import CardContent from "@material-ui/core/CardContent";

import { PackageLink } from "./links";

import { without$ } from "../util";

function humanizedValue(number: number): string {
    const K = 1000;
    const M = 1000 * 1000;

    if (number < K)
        return number.toString();
    if (number < M) {
        const base = number / K;
        return base.toFixed(base < 5 ? 1 : 0) + "K";
    }
    const base = number / M;
    return base.toFixed(base < 5 ? 1 : 0) + "M";
}

export interface PackageTileProps extends PackageKey {
    iconUrl?: string;
    title: string;
    description?: string;
    downloads?: number;
}

export const PackageTile: React.StatelessComponent<PackageTileProps> = (props) => {
    const { title, description, downloads } = props;
    const { packageId, packageVersion } = without$(props);
    const iconUrl = props.iconUrl || 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNgYAAAAAMAASsJTYQAAAAASUVORK5CYII=';
    const idver = <code>{packageVersion ? packageId + " " + packageVersion : packageId}</code>;
    const titleProps: CardHeaderProps = {
        title: title ? <span>{title} &mdash; {idver}</span> : <span>{idver}</span>
    };
    if (downloads)
        titleProps.subheader = humanizedValue(downloads) + " downloads";
    const headerProps: CardHeaderProps = {
        avatar: <img src={iconUrl} alt={'Icon for ' + packageId}></img>,
        title: <CardHeader {...titleProps}/>
    };
    return (
    <PackageLink {...props}>
        <Card>
            <CardHeader {...headerProps} />
            <CardContent>{description}</CardContent>
        </Card>
    </PackageLink>
    );
}
