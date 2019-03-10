import * as React from "react";

import { PackageContext, FormatContext, Styles } from "../util";
import { IEntity } from "../structure";
import { declaration as partialDeclaration } from "./partial";

export function declaration(pkgContext: PackageContext, entity: IEntity): React.ReactNode[] {
    const context = new FormatContext(pkgContext, Styles.DECLARATION);
    return React.Children.toArray(partialDeclaration(context, entity));
}
