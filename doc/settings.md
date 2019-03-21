# Function Settings

The following settings are used by DotNetApis Azure Functions:

## `ABLY_API_KEY`

The Ably `Log Writer` key used by background services to report progress updates.

## `StorageConnectionString`

The connection string used by Azure Functions to connect to Azure Storage for application data storage.

## `SPA_APP`

The hostname where the SPA is hosted. Must not contain a protocol; must contain a hostname; may also contain a path; must not contain a trailing slash. E.g., `stephenclearyapps.github.io/DotNetApis`.

# Web App Settings

The following settings are used by the SPA frontend; these are set in `webpack.config.js`.

## `BACKEND`

The backend API, including a trailing slash. E.g., `https://dotnetapis2.azurewebsites.net/api/`.
