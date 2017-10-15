import { getPlainJson } from "./util";
import { LogMessage } from "./messages";

export const getPackageLog = (url: string) => getPlainJson<LogMessage[]>(url);