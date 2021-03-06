import * as React from 'react';

import { Enum, Delegate, Method, Property, Event, Field, Type } from './doc';

import { isEnum, isDelegate, isMethod, isProperty, isEvent, isField, isInterface, isStruct, isClass } from '../structure';
import { PackageInjectedProps, withPackage, createRouterProps, RouteComponentProps } from './hoc';
import { State } from '../reducers';
import { Actions } from '../actions';

interface RouteParams {
    dnaid: string;
}

export type EntityDocProps = State & Actions;
const EntityDocComponent: React.StatelessComponent<EntityDocProps & PackageInjectedProps & RouteComponentProps<RouteParams>> = props => {
    const { pkg, match } = props;
    const data = pkg.findEntity(match.params.dnaid);
    if (!data)
        return <div>Could not find <code>{match.params.dnaid}</code> in documentation for {pkg.i}.</div>;
    if (isEnum(data)) {
        return <Enum {...props} data={data}/>;
    } else if (isDelegate(data)) {
        return <Delegate {...props} data={data}/>;
    } else if (isMethod(data)) {
        return <Method {...props} data={data}/>;
    } else if (isProperty(data)) {
        return <Property {...props} data={data}/>;
    } else if (isEvent(data)) {
        return <Event {...props} data={data}/>;
    } else if (isField(data)) {
        return <Field {...props} data={data}/>;
    } else if (isInterface(data) || isStruct(data) || isClass(data)) {
        return <Type {...props} data={data}/>;
    }

    throw new Error('Unrecognized entity type.');
};

export const EntityDoc = withPackage(createRouterProps<RouteParams>()(EntityDocComponent));
