DotNetApis are a form of identifier that is designed to be (mostly) human-readable and safe to use in the path portion of a URL.

# Alphabet

The following characters are never allowed by ASP.NET (even if encoded), so they cannot be used at all: `<` `>` `*` `%` `&` `:` `\`

ASP.NET has additional problems with `+`.

According to RFC3986, valid characters for a URL segment are `[A-Za-z0-9]` `-` `_` `.` `~` `*` `!` `$` `&` `+` `(` `)` `,` `;` `=` `:` `@` `'` 

Combining these restrictions, the alphabet for DotNetApis is restricted to:

`[A-Za-z0-9]` `-` `_` `.` `~` `!` `$` `(` `)` `,` `;` `=` `@` `'`

(in addition to the path separation `/` character).

However, any instances of (`<` `>` `*` `%` `&` `:` `\`) will need to be escaped *using an alternative to percent-encoding*.

http://blogs.iis.net/wadeh/how-iis-blocks-characters-in-urls

## Names

Names are more restricted. The alphabet for names is:

`[A-Za-z0-9]` `_` `.` `'` `(` `)`

The `.` character appears in namespaces and implicitly-implemented interface members; the `'`, `(`, and `)` characters appear in generic members.

## Meanings

The special characters have defined meanings:

`~` Return / Pointer type
`(` `)` Parameters (regular or generic)
`,` Parameter separator
`-` ByRef
`!` Optmod
`=` ReqMod
`$` Array
`;` Array dim

# @-Escaping

Strings are are encoded to a target alphabet using the following algorithm:

* Convert the string to a byte sequence using UTF-8 (without preamble/BOM).
* For each byte that is acceptable to the target alphabet, output that byte directly.
* For each byte not in the target alphabet, output a `@` character followed by two uppercase hexadecimal digits representing the value of that byte.

Note that the target alphabet cannot contain a `@`.
