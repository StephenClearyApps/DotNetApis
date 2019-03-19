import * as React from "react";

import { PackageContext } from "../../util";
import { IXmldocNode, isStringXmldocNode, isSeeXmldocNode, isLinkXmldocNode, XmlXmldocNodeKind } from "../../structure";
import { locationLink } from "../../fragments";

interface XmldocProps extends PackageContext {
    data?: IXmldocNode;
}

export const XmldocNode: React.StatelessComponent<XmldocProps> = (props) => {
    const { data } = props;
    if (!data)
        return null;
    if (isStringXmldocNode(data))
        return <span>{data}</span>;
    else {
        const children = data.c ? data.c.map((x, i) => <XmldocNode {...props} data={x} key={i} />) : [];
        if (isSeeXmldocNode(data))
            return <code>{locationLink(props, data.a && data.a.l, children)}</code>;
        else if (isLinkXmldocNode(data))
            return <a href={data.a.h} target='_blank'>{children}</a>;
        switch (data.k) {
            case XmlXmldocNodeKind.INLINE_CODE: return <code>{children}</code>;
            case XmlXmldocNodeKind.BLOCK_CODE: return <pre><code>{children}</code></pre>;
            case XmlXmldocNodeKind.SPAN: return <span>{children}</span>;
            case XmlXmldocNodeKind.BOLD: return <b>{children}</b>;
            case XmlXmldocNodeKind.ITALIC: return <i>{children}</i>;
            case XmlXmldocNodeKind.ORDERED_LIST: return <ol>{children}</ol>;
            case XmlXmldocNodeKind.UNORDERED_LIST: return <ul>{children}</ul>;
            case XmlXmldocNodeKind.LIST_ITEM: return <li>{children}</li>;
            default: return <div>{children}</div>;
        }
    }
}