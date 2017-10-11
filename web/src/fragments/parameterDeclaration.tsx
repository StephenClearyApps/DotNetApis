import * as React from "react";

import { PackageDoc } from "../util";
import { ReactFragment, FormatContext, Styles } from "./util";
import { IEntity, IParameter } from "../structure";
import { parameter } from "./parameter";

export function parameterDeclaration(pkg: PackageDoc, entity: IEntity, value: IParameter): ReactFragment {
    return parameter(new FormatContext(pkg, Styles.DECLARATION), value, [<br/>]);
}
