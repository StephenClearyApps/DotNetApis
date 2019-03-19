import * as React from "react";
import Tabs from '@material-ui/core/Tabs';

import { RouteComponentProps, createRouterProps } from "./hoc";
import { HashSettings } from "../logic";

export interface HashTabsProps {
    defaultTabValue?: string;
    hashPrefix?: string;
    content: {
        [key: string]: JSX.Element | null
    };
}

const HashTabsComponent: React.StatelessComponent<HashTabsProps & RouteComponentProps<{}>> =
({ location, history, hashPrefix, defaultTabValue, content, children }) => {
    const hash = new HashSettings(location, history, hashPrefix);
    const value = hash.getSetting("tab") || defaultTabValue;
    const onChange: (event: object, value: string) => void =
        (_, value) => hash.setSetting("tab", value === defaultTabValue ? "" : value);
    return <div><Tabs value={value} onChange={onChange}>{children}</Tabs><div>{value == undefined ? null : content[value]}</div></div>;
};

export const HashTabs = createRouterProps()(HashTabsComponent);