import * as React from "react";

import { PackageTileProps, PackageTile } from "./PackageTile";

const packages: PackageTileProps[] = [
    {
        packageId: "Nito.AsyncEx",
        iconUrl: "https://raw.githubusercontent.com/StephenCleary/AsyncEx/master/AsyncEx.128.png",
        title: "Async and Task Helpers",
        description: "A helper library for the Task-Based Asynchronous Pattern (TAP)."
    }
];

export const FrontPagePackages: React.StatelessComponent<{}> = () => {
    return (
        <div>
            {packages.map(p => <PackageTile {...p} key={p.packageId}/>)}
        </div>
    );
}