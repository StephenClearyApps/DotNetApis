import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router';

import { Enum, Delegate, Method, Property, Event, Field, Type } from './doc';

import { PackageDoc } from '../util';
import { isEnum, isDelegate, isMethod, isProperty, isEvent, isField, isInterface, isStruct, isClass } from '../structure';
import { PackageInjectedProps, withAutoPackage } from './hoc';

interface RouteParams {
    dnaid: string;
}

export interface EntityDocProps extends RouteComponentProps<RouteParams>, PackageInjectedProps {
}

const EntityDocComponent: React.StatelessComponent<EntityDocProps> = ({ pkg, match }) => {
    const data = pkg.findEntity(match.params.dnaid);
    if (!data)
        return <div>Could not find <code>{match.params.dnaid}</code> in documentation for {pkg.i}.</div>;
    if (isEnum(data)) {
        return <Enum data={data} pkg={pkg}/>;
    } else if (isDelegate(data)) {
        return <Delegate data={data} pkg={pkg}/>;
    } else if (isMethod(data)) {
        return <Method data={data} pkg={pkg}/>;
    } else if (isProperty(data)) {
        return <Property data={data} pkg={pkg}/>;
    } else if (isEvent(data)) {
        return <Event data={data} pkg={pkg}/>;
    } else if (isField(data)) {
        return <Field data={data} pkg={pkg}/>;
    } else if (isInterface(data) || isStruct(data) || isClass(data)) {
        return <Type data={data} pkg={pkg}/>;
    }

    throw new Error('Unrecognized entity type.');
};

export const EntityDoc = withAutoPackage(withRouter(EntityDocComponent));
