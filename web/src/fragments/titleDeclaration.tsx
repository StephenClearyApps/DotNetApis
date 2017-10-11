import * as React from "react";

import { PackageContext, FormatContext, Styles } from "../util";
import { IEntity } from "../structure";
import { declaration as partialDeclaration } from "./partial";

export function titleDeclaration(pkgContext: PackageContext, entity: IEntity): React.ReactChild[] {
    const context = new FormatContext(pkgContext, Styles.TITLE);
    return React.Children.toArray(partialDeclaration(context, entity));
}
