import * as React from "react";

import { PackageDoc } from "../util";
import { ReactFragment, FormatContext, Styles } from "./util";
import { IEntity, ITopLevelEntityBase } from "../structure";
import { fullConcreteTypeReference } from "./fullConcreteTypeReference";

export function declarationLocation(pkg: PackageDoc, entity: IEntity): ReactFragment {
    const context = new FormatContext(pkg, Styles.DECLARATION);
    const parent = pkg.findEntityParent(entity.i);
    return <code>{parent ? fullConcreteTypeReference(context, parent) : (entity as ITopLevelEntityBase).s}</code>;
}
