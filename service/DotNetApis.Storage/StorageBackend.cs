using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace DotNetApis.Storage
{
    public interface IStorageBackend
    {
        bool SupportsConcurrency { get; }
    }

    public sealed class AzureStorageBackend : IStorageBackend
    {
        private const string DevelopmentAccountName = "devstoreaccount1";
        private const string DevelopmentAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        private readonly CloudStorageAccount _account;

        public AzureStorageBackend(CloudStorageAccount account)
        {
            _account = account;
        }

        public bool IsDevelopmentStorage => _account.Credentials.AccountName == DevelopmentAccountName && _account.Credentials.ExportBase64EncodedKey() == DevelopmentAccountKey;

        public bool SupportsConcurrency => !IsDevelopmentStorage;
    }
}
