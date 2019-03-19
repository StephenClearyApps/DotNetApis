import * as locations from './locations';

// This must be kept in sync with DotNetApis.Structure.Xmldoc.XmlXmldocNodeKind
export const enum XmlXmldocNodeKind {
    DIV = 0,
    INLINE_CODE = 1,
    BLOCK_CODE = 2,
    SEE = 3,
    SPAN = 4,
    BOLD = 5,
    ITALIC = 6,
    LINK = 7,
    UNORDERED_LIST = 8,
    ORDERED_LIST = 9,
    LIST_ITEM = 10,
    // TODO: others
};

export interface IXmlXmldocNode {
    /** The kind of xmldoc node */
    k?: XmlXmldocNodeKind;

    /** Attributes */
    a?: Object;

    /** Children */
    c?: IXmldocNode[];
}

export interface ISeeXmldocNode extends IXmlXmldocNode {
    /** Attributes */
    a: {
        /** Location of the referenced element */
        l: locations.ILocation;
    };
}

export interface ILinkXmldocNode extends IXmlXmldocNode {
    /** Attributes */
    a: {
        /** href of the link */
        h: string;
    };
}

export type IStringXmldocNode = string;
export type IXmldocNode = IXmlXmldocNode | IStringXmldocNode;

export function isStringXmldocNode(xmldoc: IXmldocNode): xmldoc is IStringXmldocNode {
    return typeof(xmldoc) === 'string';
}

export function isSeeXmldocNode(xmldoc: IXmldocNode): xmldoc is ISeeXmldocNode {
    return !isStringXmldocNode(xmldoc) && xmldoc.k === XmlXmldocNodeKind.SEE;
}

export function isLinkXmldocNode(xmldoc: IXmldocNode): xmldoc is ILinkXmldocNode {
    return !isStringXmldocNode(xmldoc) && xmldoc.k === XmlXmldocNodeKind.LINK;
}

export interface IXmldoc {
    /** Basic docs (<summary> or <value>) */
    b?: IXmldocNode;

    /** Remarks (<remarks>) */
    m?: IXmldocNode;

    /** Examples (<example>) */
    e?: IXmldocNode[];

    /** See also (<seealso>) */
    s?: IXmldocNode[];

    /** Exceptions (<exception>) */
    x?: IXmldocNode[];

    /** Returns (<returns>) */
    r?: IXmldocNode;
}