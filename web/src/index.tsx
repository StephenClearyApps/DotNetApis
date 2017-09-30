import * as React from "react";
import { render } from "react-dom";
import { bindActionCreators, Dispatch } from "redux";
import { Provider, connect } from "react-redux";
import { BrowserRouter } from 'react-router-dom';
import "whatwg-fetch";

import { Main } from "./components/Main";
import { store } from "./store";
import * as actions from "./actions";

/** Binds action creators that are in one level of namespacing */
function mapDispatchToProps(dispatch: Dispatch<any>) {
    const result : any = { };
    for (let key of Object.keys(actions)) {
        result[key] = bindActionCreators((actions as any)[key], dispatch);
    }
    return result;
}

window.onload = () => {
    const ConnectedMain = connect(x => x, mapDispatchToProps)(Main);
    render(<Provider store={store}><BrowserRouter><ConnectedMain /></BrowserRouter></Provider>, document.getElementById("app"));
    actions.TimeActions.startTicks(store.dispatch);
};
