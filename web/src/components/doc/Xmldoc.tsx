import * as React from "react";

import { PackageDoc } from "../../util/packageDoc";
import { IXmldocNode, isStringXmldocNode, isSeeXmldocNode, isLinkXmldocNode, XmlXmldocNodeKind } from "../../util/structure/xmldoc";
import { LocationLink } from "../links";

interface XmldocProps {
    data: IXmldocNode;
    pkg: PackageDoc;
}

export const Xmldoc: React.StatelessComponent<XmldocProps> = ({ data, pkg }) => {
    if (!data)
        return null;
    if (isStringXmldocNode(data))
        return <span>{data}</span>;
    else {
        const children = data.c.map((x, i) => <Xmldoc data={x} key={i} pkg={pkg} />);
        if (isSeeXmldocNode(data))
            return <code><LocationLink location={data.a.l} pkg={pkg}>{children}</LocationLink></code>;
        else if (isLinkXmldocNode(data))
            return <a href={data.a.h} target='_blank'>{children}</a>;
        switch (data.k) {
            case XmlXmldocNodeKind.INLINE_CODE: return <code>{children}</code>;
            case XmlXmldocNodeKind.BLOCK_CODE: return <pre><code>{children}</code></pre>;
            case XmlXmldocNodeKind.SPAN: return <span>{children}</span>;
            case XmlXmldocNodeKind.BOLD: return <b>{children}</b>;
            case XmlXmldocNodeKind.ITALIC: return <i>{children}</i>;
            default: return <div>{children}</div>;
        }
    }
}