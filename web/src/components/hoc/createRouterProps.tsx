import { withRouter, RouteComponentProps } from "react-router";

export { RouteComponentProps } from "react-router";

/** A fixed "withRouter" that properly passes through TProps and provides a strongly-typed RouteComponentProps */
export const createRouterProps =
<TRouteProps extends {}>() =>
<TProps extends {}>(Component: React.ComponentType<TProps & RouteComponentProps<TRouteProps>>) =>
withRouter<TProps>(Component);
