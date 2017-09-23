import * as React from "react";
import { render } from "react-dom";
import { Provider, connect } from "react-redux";

import { client } from './logic/log-listener';
import { Main } from "./components/Main";
import { store } from "./store";

window.onload = () => {
    const ConnectedMain = connect(x => x)(Main);
    render(<Provider store={store}><ConnectedMain /></Provider>, document.getElementById("app"));

    client.channels.get("log:9c4a890f76eb4de79c7d464e1375a502").subscribe((message) => 
    {
        console.log(message);
    });
};
