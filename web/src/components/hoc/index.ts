export * from 'recompose';

export interface Hoc<TOuterProps, TInnerProps = TOuterProps> {
    (Component: React.ComponentType<TInnerProps>): React.ComponentType<TOuterProps>;
}

export interface PassthroughHoc<TRequiredProps = {}> {
    <TProps>(Component: React.ComponentType<TProps>): React.ComponentType<TProps & TRequiredProps>;
}

export interface ExtendingHoc<TInjectedProps, TRequiredProps = {}> {
    <TProps>(Component: React.ComponentType<TProps & TInjectedProps>): React.ComponentType<TProps & TRequiredProps>;
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
