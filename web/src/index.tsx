import * as React from "react";
import { render } from "react-dom";
import { bindActionCreators, Dispatch } from "redux";
import { Provider, connect } from "react-redux";
import { BrowserRouter, withRouter } from 'react-router-dom';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import "whatwg-fetch";

import { Main } from "./components/Main";
import { store } from "./store";
import { actions, Actions } from "./actions";
import { State } from "./reducers";

// Load the stylesheet
import "./site.css";

/** Binds action creators that are in one level of namespacing */
function mapDispatchToProps(dispatch: Dispatch<any>): Actions {
    const result : any = { };
    for (let key of Object.keys(actions)) {
        result[key] = bindActionCreators((actions as any)[key], dispatch);
    }
    return result;
}

window.onload = () => {
    const connector = connect<State, Actions>((x: State) => x, mapDispatchToProps);
    const ConnectedMain = withRouter(connector(Main) as any); // https://github.com/ReactTraining/react-router/blob/master/packages/react-router/docs/guides/redux.md
    render(
        <Provider store={store}>
            <BrowserRouter>
                <MuiThemeProvider>
                    <ConnectedMain />
                </MuiThemeProvider>
            </BrowserRouter>
        </Provider>, document.getElementById("app"));
    actions.TimeActions.synchronizeTime(store.dispatch);
};
