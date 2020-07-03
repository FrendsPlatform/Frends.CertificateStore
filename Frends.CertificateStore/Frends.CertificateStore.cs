using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Frends.CertificateStore
{
    public class LocalStoreResult
    { 
        public bool Success { get; set; }
        public CryptographicException Error { get; set; }
    }

    public class X509CertificateResultRaw
    {
        public byte[] CertificateRawData { get; set; }
    }

    public class X509CertificateCollectionResult
    {
        public X509Certificate2Collection Certificates { get; set; }
    }

    public enum CertificateStoreLocation
    {
        CurrentUser = 1,
        LocalMachine = 2
    }

    public class CertificateStoreInput
    {
        [DefaultValue(CertificateStoreLocation.CurrentUser)]
        public CertificateStoreLocation CertificateStoreType { get; set; }
    }

    public class X509Cert
    {
        public X509ExtensionCollection Extensions { get; set; }
        public X500DistinguishedName SubjectName { get; set; }
        public string SerialNumber { get; set; }
        public byte[] RawData { get; set; }
        public X500DistinguishedName IssuerName { get; set; }
        public string FriendlyName { get; set; }
        public bool Archived { get; set; }
        public int Version { get; set; }
        public string Thumbprint { get; set; }
        public Oid SignatureAlgorithm { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
    }

    public static class CertificateStore
    {
        /// <summary>
        /// Get X509Certificate raw data from a base64 string.
        /// </summary>
        /// <param name="base64">The base64 string.</param>
        /// <returns>Object { byte[] CertificateRawData }</returns>
        public static async Task<X509CertificateResultRaw> GetCertificateFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var cert = new X509Certificate2(bytes);

            await Task.CompletedTask;

            return new X509CertificateResultRaw
            {
                CertificateRawData = cert.RawData
            };
        }

        /// <summary>
        /// Get all certificates in selected Certificate Store.
        /// </summary>
        /// <returns>List [ Object { X509Cert Certificate } ]</returns>
        public static async Task<List<X509Cert>> GetAllCertificatesInSelectedStore(CertificateStoreInput input)
        {
            X509Certificate2Collection certificates;
            using (X509Store store = new X509Store(StoreName.My, (StoreLocation)input.CertificateStoreType))
            {
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    certificates = store.Certificates;
                }
                catch (CryptographicException ce)
                {
                    certificates = null;
                    throw ce;
                }
            }

            List<X509Cert> certificatesToReturn = new List<X509Cert>();
            if (certificates != null)
            {
                foreach (var cert in certificates)
                {
                    certificatesToReturn.Add(new X509Cert
                    {
                        Archived = cert.Archived,
                        FriendlyName = cert.FriendlyName,
                        Extensions = cert.Extensions,
                        IssuerName = cert.IssuerName,
                        RawData = cert.RawData,
                        SerialNumber = cert.SerialNumber,
                        SignatureAlgorithm = cert.SignatureAlgorithm,
                        SubjectName = cert.SubjectName,
                        Thumbprint = cert.Thumbprint,
                        Version = cert.Version,
                        NotAfter = cert.NotAfter,
                        NotBefore = cert.NotBefore
                    });
                }
            }

            await Task.CompletedTask;

            return certificatesToReturn;
        }

        /// <summary>
        /// Add certificate via raw data to Local User Certificate Store
        /// </summary>
        /// <param name="certificateRawData">Raw data of the certificate as byte[] or string.</param>
        /// <param name="password">Password for the certificate.</param>
        /// <returns>Object { bool Success, CryptographicException Error }</returns>
        public static async Task<LocalStoreResult> AddToLocalUserViaRawData(dynamic certificateRawData, [PasswordPropertyText] string password)
        {
            byte[] certificateStringAsByteArray;
            if (certificateRawData is string)
            {
                certificateStringAsByteArray = Encoding.UTF8.GetBytes(certificateRawData as string);
            } else
            {
                certificateStringAsByteArray = certificateRawData as byte[];
            }

            if (certificateStringAsByteArray == null)
            {
                throw new ArgumentException("certificateRawData must be a byte array", nameof(certificateRawData));
            }

            var certificate = new X509Certificate2(certificateStringAsByteArray, new NetworkCredential("", password).SecurePassword, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet);

            AddCertificateToLocalUserStore(certificate);

            await Task.CompletedTask;

            return new LocalStoreResult
            {
                Success = true
            };
        }

        /// <summary>
        /// Add certificate via file to Local User Certificate Store
        /// </summary>
        /// <param name="path">Path to the certificate file.</param>
        /// <param name="password">Password for the certificate</param>
        /// <returns>Object { bool Success, CryptographicException Error }</returns>
        public static async Task<LocalStoreResult> AddToLocalUserViaFile(string path, [PasswordPropertyText] string password)
        {
            var certificate = new X509Certificate2(File.ReadAllBytes(path), new NetworkCredential("", password).SecurePassword, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet);

            AddCertificateToLocalUserStore(certificate);

            await Task.CompletedTask;

            return new LocalStoreResult
            {
                Success = true
            };
        }

        /// <summary>
        /// Remove certificate by name from Local User Certificate Store
        /// </summary>
        /// <param name="certificateName">The name of the certificate to remove.</param>
        /// <returns>Object { bool Success, CryptographicException Error }</returns>
        public static async Task<LocalStoreResult> RemoveFromLocalUser(string certificateName)
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
                    return new LocalStoreResult
                    {
                        Success = false,
                        Error = ce
                    };
                }
            }

            await Task.CompletedTask;

            return new LocalStoreResult
            {
                Success = true
            };
        }

        private static LocalStoreResult AddCertificateToLocalUserStore(X509Certificate2 certificate)
        {
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                try
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(certificate);
                    return new LocalStoreResult
                    {
                        Success = true
                    };
                }
                catch (CryptographicException ce)
                {
                    return new LocalStoreResult
                    {
                        Success = false,
                        Error = ce
                    };
                }
            }
        }

    }
}
