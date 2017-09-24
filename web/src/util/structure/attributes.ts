import { ILiteral } from './literals';
import { ILocation } from './locations';

/** An argument passed to an attribute. */
export interface IAttributeArgument {
    /** Named argument name. This is undefined if this argument is a positional argument. */
    n?: string;

    /** The argument value. */
    v: ILiteral;
}

/** An attribute. */
export interface IAttribute {
    /** The target of the attribute, e.g., 'return' */
    t?: string;

    /** The simple name of the attribute type; any 'Attribute' suffix is stripped. The name is prepended with '@' if the name is a C# keyword. */
    n: string;

    /** The location of the attribute type. */
    l: ILocation;

    /** All the arguments of the attribute. */
    a?: Array<IAttributeArgument>;
}
