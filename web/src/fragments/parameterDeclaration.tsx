import * as React from "react";

import { PackageContext } from "../util";
import { ReactFragment, FormatContext, Styles } from "./util";
import { IEntity, IParameter } from "../structure";
import { parameter } from "./parameter";

export function parameterDeclaration(pkgContext: PackageContext, entity: IEntity, value: IParameter): ReactFragment {
    return parameter(new FormatContext(pkgContext, Styles.DECLARATION), value, [<br/>]);
}
