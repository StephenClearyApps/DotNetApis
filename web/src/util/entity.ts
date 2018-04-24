import { IEntity } from "../structure";

export function sortEntities(types: IEntity[]) {
    types.sort((x, y) => {
        const xKey = x.n || x.i;
        const yKey = y.n || y.i;
        if (xKey < yKey)
            return -1;
        if (xKey > yKey)
            return 1;
        return 0;
    });
}