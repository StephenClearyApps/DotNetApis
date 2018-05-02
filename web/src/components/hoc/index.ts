type Diff<T extends string, U extends string> = ({[P in T]: P} &
    {[P in U]: never} & {[x: string]: never})[T]
type Omit<T, K extends keyof T> = Pick<T, Diff<keyof T, K>>

export interface Hoc<TInjectedProps extends {} = {}, TRequiredProps extends {} = {}> {
    <TProps extends TInjectedProps>(Component: React.ComponentType<TProps>): React.ComponentType<
        {} extends TInjectedProps ?
            ({} extends TRequiredProps ? TProps : TRequiredProps & TProps)
        : ({} extends TRequiredProps ? Omit<TProps, keyof TInjectedProps> : TRequiredProps & Omit<TProps, keyof TInjectedProps>)
    >;
}

export * from './createEither';
export * from './createExecuteOnMount';
export * from './createLoadOnDemand';
export * from './createMaybe';
export * from './createRouterProps';

export * from './withLoadPackageLogOnDemand';
export * from './withLoadPackageOnDemand';
export * from './withPackage';
export * from './withPackageContext';
export * from './withPackageLog';
export * from './withPackageLogRequest';
export * from './withPackageRequest';
