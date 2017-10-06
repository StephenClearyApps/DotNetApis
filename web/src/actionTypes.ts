import { PackageDoc } from "./util/packageDoc";
import { LogMessage } from "./api";

export const ActionTypes = {
    TICK: 'TICK',

    GET_DOC_BEGIN: 'GET_DOC_BEGIN',
    MAP_PACKAGE_KEY: 'MAP_PACKAGE_KEY',
    GET_DOC_REDIRECTING: 'GET_DOC_REDIRECTING',
    GET_DOC_PROCESSING: 'GET_DOC_PROCESSING',
    GET_DOC_PROGRESS: 'GET_DOC_PROGRESS',
    GET_DOC_END: 'GET_DOC_END',
    GET_DOC_ERROR: 'GET_DOC_ERROR'
};

export type TickAction = PayloadAction<{ timestamp: number }>;
export const getTick = (timestamp: number): TickAction =>
    ({ type: ActionTypes.TICK, payload: { timestamp }});

export type GetDocBeginAction = MetaAction<{ requestPackageKey: PackageKey }>;
export const getDocBegin = (requestPackageKey: PackageKey): GetDocBeginAction =>
    ({ type: ActionTypes.GET_DOC_BEGIN, meta: { requestPackageKey } });

export type MapPackageKeyAction = PayloadAction<{ requestPackageKey: PackageKey, normalizedPackageKey: PackageKey }>;
export const mapPackageKey = (requestPackageKey: PackageKey, normalizedPackageKey: PackageKey): MapPackageKeyAction =>
    ({ type: ActionTypes.MAP_PACKAGE_KEY, payload: { requestPackageKey, normalizedPackageKey } });

export type GetDocRedirectingAction = MetaPayloadAction<{ requestPackageKey: PackageKey }, { log: LogMessage[] }>;
export const getDocRedirecting = (requestPackageKey: PackageKey, log: LogMessage[]): GetDocRedirectingAction =>
    ({ type: ActionTypes.GET_DOC_REDIRECTING, meta: { requestPackageKey }, payload: { log } });
    
export type GetDocProcessingAction = MetaPayloadAction<{ requestPackageKey: PackageKey }, { log: LogMessage[] }>;
export const getDocProcessing = (requestPackageKey: PackageKey, log: LogMessage[]): GetDocProcessingAction =>
    ({ type: ActionTypes.GET_DOC_PROCESSING, meta: { requestPackageKey }, payload: { log } });

export type GetDocProgressAction = MetaPayloadAction<{ requestPackageKey: PackageKey }, { logMessage: LogMessage }>;
export const getDocProgress = (requestPackageKey: PackageKey, logMessage: LogMessage): GetDocProgressAction =>
    ({ type: ActionTypes.GET_DOC_PROGRESS, meta: { requestPackageKey }, payload: { logMessage }});

export type GetDocEndAction = MetaPayloadAction<{ requestPackageKey: PackageKey }, { data: PackageDoc }>;
export const getDocEnd = (requestPackageKey: PackageKey, data: PackageDoc): GetDocEndAction =>
    ({ type: ActionTypes.GET_DOC_END, meta: { requestPackageKey }, payload: { data } });

export type GetDocErrorAction = MetaErrorAction<{ requestPackageKey: PackageKey }>;
export const getDocError = (requestPackageKey: PackageKey, error: Error): GetDocErrorAction =>
    ({ type: ActionTypes.GET_DOC_ERROR, meta: { requestPackageKey }, payload: error, error: true });