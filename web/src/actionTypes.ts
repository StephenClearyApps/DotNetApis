import { PackageDoc } from "./util/packageDoc";

export const ActionTypes = {
    GET_DOC_BEGIN: 'GET_DOC_BEGIN',
    GET_DOC_PROCESSING: 'GET_DOC_PROCESSING',
    GET_DOC_PROGRESS: 'GET_DOC_PROGRESS',
    GET_DOC_END: 'GET_DOC_END',
    GET_DOC_ERROR: 'GET_DOC_ERROR'
};

export type GetDocBeginAction = MetaAction<{ key: PackageKey }>;
export const getDocBegin = (key: PackageKey): GetDocBeginAction => ({ type: ActionTypes.GET_DOC_BEGIN, meta: { key } });

export type GetDocProcessingAction = MetaPayloadAction<{ key: PackageKey, normalized: PackageKey }, { log: string[] }>;
export const getDocProcessing = (key: PackageKey, normalized: PackageKey, log: string[]): GetDocProcessingAction =>
    ({ type: ActionTypes.GET_DOC_PROCESSING, meta: { key, normalized }, payload: { log } });

export type GetDocProgressAction = MetaPayloadAction<{ normalized: PackageKey }, { type: string, timestamp: number, message: string }>;
export const getDocProgress = (normalized: PackageKey, type: string, timestamp: number, message: string): GetDocProgressAction =>
    ({ type: ActionTypes.GET_DOC_PROGRESS, meta: { normalized }, payload: { type, timestamp, message }});

export type GetDocEndAction = MetaPayloadAction<{ key: PackageKey }, { data: PackageDoc }>;
export const getDocEnd = (key: PackageKey, data: PackageDoc): GetDocEndAction =>
    ({ type: ActionTypes.GET_DOC_END, meta: { key }, payload: { data } });

export type GetDocErrorAction = MetaErrorAction<{ key: PackageKey }>;
export const getDocError = (key: PackageKey, error: Error): GetDocErrorAction =>
    ({ type: ActionTypes.GET_DOC_ERROR, meta: { key }, payload: error, error: true });