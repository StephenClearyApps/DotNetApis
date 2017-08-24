# External Services

DotNetApis uses the following external servies:

## Ably

Ably is used for progress reports of background operations. Each background operation is identified with a GUID and publishes progress updates on the `log:{guid}` channel.

There are two API keys used with Ably: `Log Viewer` (has `Subscribe` and `History` access to `log:*`) and `Log Writer` (has `Publish` access to `log:*`).

The web front-end uses the `Log Viewer` API key directly, as messages sent over the `log:*` channels are not private. Background processes use the `Log Writer` key, which is kept private.

## Azure Functions

Most of the logic lives in an Azure Function (consumption plan).

## Azure Storage

All of the persistent application data storage lives in Azure Blobs and Azure Tables.

A second Azure Storage account is also used for Azure Functions to store data in Azure Queues.