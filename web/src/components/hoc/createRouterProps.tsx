import { withRouter, RouteComponentProps } from "react-router";

import { ReactComponent } from ".";

/** A fixed "withRouter" that properly passes through TProps and provides a strongly-typed RouteComponentProps */
export const createRouterProps =
<TRouteProps extends {}>() =>
<TProps extends {}>(Component: ReactComponent<TProps & RouteComponentProps<TRouteProps>>) =>
withRouter<TProps>(Component);
