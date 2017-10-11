import * as React from "react";
import { render } from "react-dom";
import { bindActionCreators, Dispatch } from "redux";
import { Provider, connect } from "react-redux";
import { BrowserRouter, withRouter } from 'react-router-dom';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import "whatwg-fetch";

import { Main } from "./components/Main";
import { store } from "./store";
import { actions, TimeActions } from "./actions";

// Load the stylesheet
import "./site.css";

/** Binds action creators that are in one level of namespacing */
function mapDispatchToProps(dispatch: Dispatch<any>) {
    const result : any = { };
    for (let key of Object.keys(actions)) {
        result[key] = bindActionCreators((actions as any)[key], dispatch);
    }
    return result;
}

window.onload = () => {
    const ConnectedMain = withRouter(connect(x => x, mapDispatchToProps)(Main));
    render(
        <Provider store={store}>
            <BrowserRouter>
                <MuiThemeProvider>
                    <ConnectedMain />
                </MuiThemeProvider>
            </BrowserRouter>
        </Provider>, document.getElementById("app"));
    TimeActions.startTicks(store.dispatch);
};
