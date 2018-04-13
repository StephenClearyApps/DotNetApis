export * from 'recompose';

export type Hoc<TOuterProps, TInnerProps = TOuterProps> = (Component: React.ComponentType<TInnerProps>) => React.ComponentType<TOuterProps>;
export type ExtendingHoc<TOuterProps, TAdditionalProps> = Hoc<TOuterProps, TOuterProps & TAdditionalProps>;

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
