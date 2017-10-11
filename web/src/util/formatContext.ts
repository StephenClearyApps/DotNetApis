import { PackageDoc, PackageContext } from ".";

export const enum Styles {
    // A full declaration listing (used in declaration blocks). Links and parameter modifiers are enabled.
    DECLARATION,

    // A single-line, partially formatted name (used in headers). Links and parameter modifiers are disabled.
    TITLE,

    // A single-line short declaration (used in member lists). Links are disabled; parameter modifiers are enabled.
    MEMBER
};

export class FormatContext {
    constructor(public pkgContext: PackageContext, public style: Styles = Styles.DECLARATION) {
    }

    get includeLinks(): boolean {
        return this.style === Styles.DECLARATION;
    }
    
    get includeParameterModifiers(): boolean {
        return this.style !== Styles.TITLE;
    }

    get pkg(): PackageDoc {
        return this.pkgContext.pkg;
    }
}