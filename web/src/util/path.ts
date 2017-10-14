export function normalizePath(path: string): string {
    return path.replace(/\\/g, '/');
}

export function fileName(path: string): string {
    const index = path.lastIndexOf('/');
    if (index === -1)
        return path;
    return path.substr(index + 1);
}
