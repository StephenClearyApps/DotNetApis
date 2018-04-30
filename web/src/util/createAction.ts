// Objects that are not arrays.
type ObjectNotArray = {
    [key: string]: any;
    [key: number]: never;
};

export function createAction<TType extends string>(type: TType): { type: TType };
export function createAction<TType extends string, TPayload extends ObjectNotArray>(type: TType, payload: TPayload): { type: TType; payload: TPayload; };
export function createAction<TType extends string, TPayload extends ObjectNotArray, TMeta extends ObjectNotArray>(type: TType, payload: TPayload, meta: TMeta): { type: TType; payload: TPayload; meta: TMeta; };
export function createAction<TType extends string, TPayload extends ObjectNotArray, TMeta extends ObjectNotArray>(type: TType, payload?: TPayload | Error, meta?: TMeta) {
    return { type, payload, meta };
}

export function createMetaAction<TType extends string, TMeta extends ObjectNotArray>(type: TType, meta: TMeta) {
    return { type, meta };
}

export function createErrorAction<TType extends string>(type: TType, payload: Error): { type: TType; payload: Error; error: true; };
export function createErrorAction<TType extends string, TMeta extends ObjectNotArray>(type: TType, payload: Error, meta: TMeta): { type: TType; payload: Error; error: true; meta: TMeta; };
export function createErrorAction<TType extends string, TPayload extends ObjectNotArray, TMeta extends ObjectNotArray>(type: TType, payload?: TPayload | Error, meta?: TMeta) {
    return { type, payload, error: true as true, meta };
}
