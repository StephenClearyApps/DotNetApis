// Fragments are like Components, but return React fragments instead of proper elements.
// These formatting functions generally:
// 1) Take regular parameters instead of "props".
// 2) Return a fragment.
// 3) Take a FormatContext parameter.
// Note that if a fragment passes another fragment as a child of an element, then it *must* be wrapped in React.Children.toArray at that point.

// Note: not all types should be exported from this folder!
// The exported types ("public api") take a package parameter instead of a FormatContext parameter.

export * from "./declarationLocation";
export * from "./declaration";
export * from "./title";
export * from "./locationLink";
export * from "./parameterDeclaration";
export * from "./simpleDeclaration";
