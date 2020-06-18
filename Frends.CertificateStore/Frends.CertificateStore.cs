using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Frends.CertificateStore
{
    public class Result
    { 
        public bool Success { get; set; }
        public CryptographicException Error { get; set; } = null;
    }

    public static class CertificateStore
    {
        /// <summary>
        /// Add certificate to Local User Certificate Store
        /// </summary>
        /// <param name="path">Path to the certificate file.</param>
        /// <param name="password">Password for the certificate</param>
        /// <returns>Object { bool Success, CryptographicException Error }</returns>
        public static async Task<Result> AddToLocalUser(string path, [PasswordPropertyText] string password)
        {
            var certificate = new X509Certificate2(File.ReadAllBytes(path), new NetworkCredential("", password).SecurePassword, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet);

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                try 
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(certificate);
                } catch (CryptographicException ce)
                {
                    return new Result
                    {
                        Success = false,
                        Error = ce
                    };
                }
            }

            await Task.CompletedTask;

            return new Result 
            {
                Success = true
            };
        }

        /// <summary>
        /// Remove certificate by name from Local User Certificate Store
        /// </summary>
        /// <param name="certificateName">The name of the certificate to remove.</param>
        /// <returns>Object { bool Success, CryptographicException Error }</returns>
        public static async Task<Result> RemoveFromLocalUser(string certificateName)
        {
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                try
                {
                    store.Open(OpenFlags.ReadWrite);
                    X509Certificate2Collection certificatesToDelete = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);
                    if (certificatesToDelete.Count != 0)
                    {
                        X509Chain ch = new X509Chain();
                        ch.Build(certificatesToDelete[0]);
                        X509Certificate2Collection allCertsInChain = new X509Certificate2Collection();

                        foreach (X509ChainElement el in ch.ChainElements)
                        {
                            allCertsInChain.Add(el.Certificate);
                        }

                        store.RemoveRange(allCertsInChain);
                    }
                } catch (CryptographicException ce)
                {
                    return new Result
                    {
                        Success = false,
                        Error = ce
                    };
                }
            }

            await Task.CompletedTask;

            return new Result
            {
                Success = true
            };
        }

    }
}
