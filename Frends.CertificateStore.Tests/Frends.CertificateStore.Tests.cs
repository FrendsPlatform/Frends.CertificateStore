using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Frends.CertificateStore.Tests
{
    [TestFixture]
    class TestClass
    {

        [TearDown]
        public static async Task TearDown()
        {
            var removal = await CertificateStore.RemoveFromLocalUser("frends");
            Assert.IsTrue(removal.Success);
        }

        [Test]
        public static async Task ShouldAddCertificateToLocalUserCertificateStore()
        {
            await CreateCertificateAndAddToLocalUserCertificateStore();
        }

        [Test]
        public static async Task ShouldRemoveExistingCertificateFromLocalUserCertificateStore()
        {
            await CreateCertificateAndAddToLocalUserCertificateStore();
            var res = await CertificateStore.RemoveFromLocalUser("frends");
            Assert.IsTrue(res.Success);
            var localCertificates = GetLocalUserCertificates();
            var generatedCertificateInStore = localCertificates.Find(X509FindType.FindByIssuerName, "frends", false);
            Assert.That(generatedCertificateInStore.Count, Is.EqualTo(0));
        }

        private static async Task CreateCertificateAndAddToLocalUserCertificateStore()
        {
            MakeCert();
            var res = await CertificateStore.AddToLocalUser("certificate.cer", "CertificatePassword");
            Assert.IsTrue(res.Success);
            var localCertificates = GetLocalUserCertificates();
            var generatedCertificateInStore = localCertificates.Find(X509FindType.FindByIssuerName, "frends", false);
            Assert.IsNotNull(generatedCertificateInStore);
        }

        private static X509Certificate2Collection GetLocalUserCertificates()
        {
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                try
                {
                    store.Open(OpenFlags.ReadWrite);
                    X509Certificate2Collection certificates = store.Certificates;
                    return certificates;
                }
                catch (CryptographicException)
                {
                    return null;
                }
            }
        }

        private static void MakeCert()
        {
            var ecdsa = ECDsa.Create(); // generate asymmetric key pair
            var req = new CertificateRequest("cn=frends", ecdsa, HashAlgorithmName.SHA256);
            var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

            // Create Base 64 encoded CER (public key only)
            File.WriteAllText("certificate.cer",
                "-----BEGIN CERTIFICATE-----\r\n"
                + Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks)
                + "\r\n-----END CERTIFICATE-----");
        }
    }
}
