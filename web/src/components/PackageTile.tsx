import * as React from "react";
import { Card, CardHeader, CardTitle, CardText } from "material-ui/Card";
import { CardHeaderProps } from "material-ui";

import { PackageLink } from "./links/PackageLink";

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
    description: string;
    downloads?: number;
}

export const PackageTile: React.StatelessComponent<PackageTileProps> = (props) => {
    const { packageId, packageVersion, title, description, downloads } = props;
    const iconUrl = props.iconUrl || 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNgYAAAAAMAASsJTYQAAAAASUVORK5CYII=';
    const headerProps: CardHeaderProps = {
        avatar: <img width='64' height='64' src={iconUrl} alt={'Icon for ' + packageId}></img>,
        title: packageVersion ? packageId + " " + packageVersion : packageId
    };
    if (downloads)
        headerProps.subtitle = humanizedValue(downloads);
    return (
    <PackageLink {...props}>
        <Card>
            <CardHeader {...headerProps} />
            <CardTitle title={title} />
            <CardText>{description}</CardText>
        </Card>
    </PackageLink>
    );
}
