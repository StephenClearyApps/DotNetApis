import * as React from "react";

import { PackageContext, FormatContext, Styles } from "../util";
import { IAttribute } from "../structure";
import { attribute, array } from "./partial";

export function attributeDeclaration(pkgContext: PackageContext, attributes: IAttribute[]): React.ReactChild[] {
    const context = new FormatContext(pkgContext, Styles.DECLARATION);
    return React.Children.toArray(array(attributes).map(x => [attribute(context, x), [<br/>]]));
}
