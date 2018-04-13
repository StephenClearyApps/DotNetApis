# Note on package versions

This project currently uses v1 of the Azure Functions runtime.

As of v1.0.11612, the Azure Functions runtime constrains user packages to [some specific versions](https://github.com/Azure/azure-functions-host/blob/v1.0.11612/src/WebJobs.Script/packages.config):

* `Microsoft.Extensions.Logging.Abstractions 1.1.1`
* `Newtonsoft.Json 9.0.1`
* `WindowsAzure.Storage 7.2.1`
