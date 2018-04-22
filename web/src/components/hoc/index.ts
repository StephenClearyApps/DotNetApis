export * from 'recompose';

export interface Hoc<TOuterProps, TInnerProps = TOuterProps> {
    (Component: React.ComponentType<TInnerProps>): React.ComponentType<TOuterProps>;
}

export interface PassthroughHoc {
    <TProps>(Component: React.ComponentType<TProps>): React.ComponentType<TProps>;
}

export interface ExtendingHoc<TInjectedProps> {
    <TProps>(Component: React.ComponentType<TProps & TInjectedProps>): React.ComponentType<TProps>;
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
