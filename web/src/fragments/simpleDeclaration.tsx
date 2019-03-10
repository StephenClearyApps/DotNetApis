import * as React from "react";

import { PackageContext, FormatContext, Styles } from "../util";
import { IEntity } from "../structure";
import { declaration as partialDeclaration } from "./partial";

export function simpleDeclaration(pkgContext: PackageContext, entity: IEntity, ns?: string): React.ReactNode[] {
    const context = new FormatContext(pkgContext, Styles.MEMBER);
    return React.Children.toArray(partialDeclaration(context, entity, ns));
}
