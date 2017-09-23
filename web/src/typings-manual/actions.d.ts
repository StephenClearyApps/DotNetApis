interface Action {
    type: string;
}

interface ActionPayload<TPayload> {
    payload: TPayload;
}

interface ActionError {
    error: boolean;
}

interface ActionMeta<TMeta> {
    meta: TMeta;
}

type PayloadAction<TPayload> = Action & ActionPayload<TPayload>;
type ErrorAction = Action & ActionError & ActionPayload<Error>;
type MetaAction<TMeta> = Action & ActionMeta<TMeta>;
type MetaPayloadAction<TMeta, TPayload> = Action & ActionPayload<TPayload> & ActionMeta<TMeta>;
type MetaErrorAction<TMeta> = Action & ActionError & ActionPayload<Error> & ActionMeta<TMeta>;