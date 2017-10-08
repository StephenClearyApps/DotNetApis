import { getPlainJson } from "./util";
import { IPackage } from "../structure";

export const getPackageDocumentation = (url: string) => getPlainJson<IPackage>(url);