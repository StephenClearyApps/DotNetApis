import * as React from "react";
import { render } from "react-dom";
import { bindActionCreators, Dispatch } from "redux";
import { Provider, connect } from "react-redux";
import "whatwg-fetch";

import { client } from './logic/log-listener';
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
    render(<Provider store={store}><ConnectedMain /></Provider>, document.getElementById("app"));

    // client.channels.get("log:9c4a890f76eb4de79c7d464e1375a502").subscribe((message) => 
    // {
    //     console.log(message);
    // });
};
