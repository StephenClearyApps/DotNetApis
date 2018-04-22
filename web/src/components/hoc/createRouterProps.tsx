import { withRouter, RouteComponentProps } from "react-router";

export { RouteComponentProps } from "react-router";

type RouterExtendingHoc<TRouteProps> = <TProps extends {}>(Component: React.ComponentType<TProps & RouteComponentProps<TRouteProps>>) => React.ComponentType<TProps>;

/** A fixed "withRouter" that properly passes through TProps and provides a strongly-typed RouteComponentProps */
export function createRouterProps<TRouteProps>() {
    return withRouter as RouterExtendingHoc<TRouteProps>;
}
