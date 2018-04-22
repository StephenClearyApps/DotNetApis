import { withRouter, RouteComponentProps } from "react-router";

import { ExtendingHoc } from ".";

export { RouteComponentProps } from "react-router";

/** A fixed "withRouter" that properly passes through TProps and provides a strongly-typed RouteComponentProps */
export function createRouterProps<TRouteProps>(): ExtendingHoc<RouteComponentProps<TRouteProps>> {
    return withRouter as any;
}
