import { withRouter, RouteComponentProps } from "react-router";

import { Hoc } from ".";

export { RouteComponentProps } from "react-router";

/** A fixed "withRouter" that properly passes through TProps and provides a strongly-typed RouteComponentProps */
export function createRouterProps<TRouteProps>(): Hoc<RouteComponentProps<TRouteProps>> {
    return withRouter as any;
}
