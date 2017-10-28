import * as React from "react";
import Tabs from "material-ui/Tabs";

import { RouteComponentProps, createRouterProps } from "./hoc";
import { HashSettings } from "../logic";

export interface HashTabsProps {
    defaultTabValue: string;
    hashPrefix?: string;
}

const HashTabsComponent: React.StatelessComponent<HashTabsProps & RouteComponentProps<any>> =
({ location, history, hashPrefix, defaultTabValue, children }) => {
    const hash = new HashSettings(location, history, hashPrefix);
    const value = hash.getSetting("tab") || defaultTabValue;
    const onChange: (value: string) => void =
        value => hash.setSetting("tab", value === defaultTabValue ? "" : value);
    return <Tabs value={value} onChange={onChange}>{children}</Tabs>;
};

export const HashTabs = createRouterProps<any>()(HashTabsComponent);