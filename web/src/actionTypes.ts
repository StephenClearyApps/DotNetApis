export const ActionTypes = {
    GET_DOC_BEGIN: 'GET_DOC_BEGIN',
    GET_DOC_PROCESSING: 'GET_DOC_PROCESSING',
    GET_DOC_END: 'GET_DOC_END',
    GET_DOC_ERROR: 'GET_DOC_ERROR'
};

export type GetDocBeginAction = MetaAction<{ key: PackageKey }>;
export function getDocBegin(key: PackageKey): GetDocBeginAction { return { type: ActionTypes.GET_DOC_BEGIN, meta: { key } }; }

export type GetDocProcessingAction = MetaPayloadAction<{ key: PackageKey }, { data: any }>; // TODO: any
export function getDocProcessing(key: PackageKey, data: any): GetDocProcessingAction { return { type: ActionTypes.GET_DOC_PROCESSING, meta: { key }, payload: { data } }; } // TODO: any

export type GetDocEndAction = MetaPayloadAction<{ key: PackageKey }, { data: PackageDoc }>;
export function getDocEnd(key: PackageKey, data: PackageDoc): GetDocEndAction { return { type: ActionTypes.GET_DOC_END, meta: { key }, payload: { data } }; } // TODO: any

export type GetDocErrorAction = MetaErrorAction<{ key: PackageKey }>;
export function getDocError(key: PackageKey, error: Error): GetDocErrorAction { return { type: ActionTypes.GET_DOC_ERROR, meta: { key }, payload: error, error: true }; }