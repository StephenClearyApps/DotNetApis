# Deploying DotNetApis

## SPA Hosting

The current SPA is set up to build into the `/docs` folder in GitHub. This is hosted via GitHub Pages on a custom domain (`dotnetapisapp.stephencleary.com`) through CloudFlare.

## Azure Functions

### Proxies

There are two proxies defined in the Azure Functions project:

- `api/{*rest}` is forwarded to `https://%WEBSITE_HOSTNAME%/api/{rest}`. All routes starting with `/api` are just passed through to the Azure Function handlers.
- `{*rest}` is forwarded to `https://dotnetapisapp.stephencleary.com`. All other requests will load the SPA.

### Functions

The actual Azure Function definitions handle all API requests.