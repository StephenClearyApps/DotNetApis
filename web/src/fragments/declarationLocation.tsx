import * as React from "react";

import { PackageContext } from "../util";
import { ReactFragment, FormatContext, Styles } from "./util";
import { IEntity, ITopLevelEntityBase } from "../structure";
import { fullConcreteTypeReference } from "./fullConcreteTypeReference";

export function declarationLocation(pkgContext: PackageContext, entity: IEntity): ReactFragment {
    const context = new FormatContext(pkgContext, Styles.DECLARATION);
    const parent = pkgContext.pkg.findEntityParent(entity.i);
    return [<code key={entity.i}>{React.Children.toArray(parent ? fullConcreteTypeReference(context, parent) : (entity as ITopLevelEntityBase).s)}</code>];
}
