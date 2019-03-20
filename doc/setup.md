# Developing locally

## One-time setup

### Local dev setup

- Clone the project locally.
- Edit `.git/info/exclude` and add a line containing `docs`
- Run `git update-index --assume-unchanged docs/index.html`
- From the `web` directory, run `npm install` and `npm run dev`. This will generate a debug build of the frontend.

### Upload reference assemblies

#### Collect reference assemblies

- Each set of reference assemblies should be in a NuGet short name folder, e.g., `sl5/`, `wp8/`, `wpa81/`, `win81/`, `net47/`.
- Only the highest current version of reference assemblies is necessary; if you have `net47`, then you don't need to upload `net461`.
- Include all "Facade" assemblies as well as the true reference assemblies.

#### Copy reference assemblies

AzCopy works great for copying the uploading the reference assemblies. AzCopy v10 requires a SAS:

```
$context = New-AzStorageContext -Local
New-AzStorageContainer reference -Permission Off -Context $context
$sas = New-AzStorageContainerSASToken -Name reference -Permission rwdl -ExpiryTime (Get-Date).AddDays(1) -Context $context -FullUri
azcopy.exe copy C:\refdlls\* $sas --from-to LocalBlob --no-guess-mime-type --recursive
```

Older versions of AzCopy:
- `azcopy /Source:C:\refdlls /Dest:http://127.0.0.1:10000/devstoreaccount1/reference /DestKey:Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw== /S /DestType:blob`
- `azcopy /Source:C:\refdlls /Dest:https://dotnetapisstorage.blob.core.windows.net/reference /DestKey:{key} /S`

#### Process reference assemblies

Execute a `POST` to `/api/ops` with the request body `{ "type": "ProcessReferenceXmldoc" }` to process the reference dlls.

If in a production environment (not local), you'll need to specify a function key; `POST` to `/api/ops?code=<functionkey>` instead.

### Running the app

- From the `web` directory, run `npm serve`. This will start a web server serving the frontend.
- Open the solution in the `service` directory and run the `FunctionApp` project.
- Browse to [http://localhost:7071](http://localhost:7071) - this should load the SPA app talking to your local Azure Functions instance.

# Deploying DotNetApis to a new instance

- Clone the project in GitHub.
- Enable GitHub Pages (`/docs` folder on master branch). It should host at `https://username.github.io/DotNetApis/`
- Create an Azure Function app and an Azure Storage account.
- Set the required [application settings](./settings.md) for the app:
  - `StorageConnectionString` should be the connection string for the Azure Storage account you just created.
  - `SPA_APP` should be your GitHub Pages address, including the `https://` prefix and without a trailing slash. E.g., `https://username.github.io/DotNetApis`
  - Turn on `Diagnostic logs` - `Detailed error messages`.
- Set up your source control to autodeploy to the Azure Function app.
- Follow the instructions for [uploading reference assemblies](#upload-reference-assemblies), above.
