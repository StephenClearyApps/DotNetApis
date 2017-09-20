# Deploying DotNetApis

- Clone the project in GitHub.
- Enable GitHub Pages (`/docs` folder on master branch). It should host at `https://username.github.io/DotNetApis/`
- Create an Azure Function app and an Azure Storage account.
- Set the required [application settings](./settings.md) for the app:
  - `StorageConnectionString` should be the connection string for the Azure Storage account you just created.
  - `SPA_APP` should be your GitHub Pages address, minus the `https://` prefix and without a trailing slash. E.g., `username.github.io/DotNetApis`
  - Turn on `Diagnostic logs` - `Detailed error messages`.
- Set up your source control to autodeploy to the Azure Function app.
- Upload reference assemblies and their xmldoc files to a `reference` container in the Azure Storage account.
  - Each set of reference assemblies should be in a NuGet short name folder, e.g., `sl5/`, `wp8/`, `wpa81/`, `win81/`, `net47/`.
  - Only the highest current version of reference assemblies is necessary; if you have `net47`, then you don't need to upload `net461`.
  - Include all "Facade" assemblies as well as the true reference assemblies.
  - Tip: AzCopy works great, e.g., `azcopy /Source:C:\refdlls /Dest:https://dotnetapisstorage.blob.core.windows.net/reference /DestKey:{key} /S` and `azcopy /Source:C:\refdlls /Dest:http://127.0.0.1:10000/devstoreaccount1/reference /DestKey:Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw== /S /DestType:blob`
- Execute `OpsFunction` with the request body `{ "type": "ProcessReferenceXmldoc" }` to process the reference dlls.