import { IEntity } from "../structure";

export function sortEntities(types: IEntity[]) {
    types.sort((x, y) => {
        if (x.n < y.n)
            return -1;
        if (x.n > y.n)
            return 1;
        return 0;
    });
}