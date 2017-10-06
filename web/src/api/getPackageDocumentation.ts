import { getPlainJson } from "./util";
import { IPackage } from "../util/structure/packages";

export const getPackageDocumentation = (url: string) => getPlainJson<IPackage>(url);