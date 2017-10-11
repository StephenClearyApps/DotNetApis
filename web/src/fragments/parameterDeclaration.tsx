import * as React from "react";

import { IEntity, IParameter } from "../structure";
import { PackageContext, FormatContext, Styles } from "../util";
import { parameter } from "./partial";

export function parameterDeclaration(pkgContext: PackageContext, entity: IEntity, value: IParameter): React.ReactChild[] {
    const context = new FormatContext(pkgContext, Styles.DECLARATION);
    return React.Children.toArray(parameter(context, value, [<br/>]));
}
