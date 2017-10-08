// Fragments are like Components, but return React fragments instead of proper elements.
// These formatting functions generally:
// 1) Take regular parameters instead of "props".
// 2) Return a fragment.
// 3) Take a FormatContext parameter.

// Note: not all types should be exported from this folder!
// The exported types ("public api") take packages instead of FormatContext parameters.

export * from "./declarationLocation";
export * from "./declaration";
export * from "./title";
export * from "./locationLink";