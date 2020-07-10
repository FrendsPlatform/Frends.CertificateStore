using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Frends.CertificateStore.Tests
{
    [TestFixture]
    class TestClass
    {

        [TearDown]
        public static void TearDown()
        {
            var removal = CertificateStore.RemoveFromLocalUser("frends");
            var removal2 = CertificateStore.RemoveFromLocalUser("login.yahoo.com");
            Assert.IsTrue(removal.Success);
            Assert.IsTrue(removal2.Success);
        }

        [Test]
        public static void ShouldAddRawCertificateDataToLocalUserStore()
        {
            var rawData = ConvertBase64StringToValidRawCertificateData();
            var res = CertificateStore.AddToLocalUserViaRawData(rawData.CertificateRawData, "password");
            Assert.IsNotNull(res);
        }

        [Test]
        public static void ShouldGetCorrectCertificateInUserStore()
        {
            var input = new CertificateStoreInput() { CertificateStoreType = CertificateStoreLocation.CurrentUser };
            CreateCertificateAndAddToLocalUserCertificateStore();
            var localCerts = CertificateStore.GetAllCertificatesInSelectedStore(input);
            Assert.IsNotNull(localCerts.Find(x => x.IssuerName.Name == "CN=frends"));
        }

        [Test]
        public static void ShouldAddCertificateToLocalUserCertificateStore()
        {
            CreateCertificateAndAddToLocalUserCertificateStore();
        }

        [Test]
        public static void ShouldRemoveExistingCertificateFromLocalUserCertificateStore()
        {
            CreateCertificateAndAddToLocalUserCertificateStore();
            var res = CertificateStore.RemoveFromLocalUser("frends");
            Assert.IsTrue(res.Success);
            var localCertificates = GetLocalUserCertificates();
            var generatedCertificateInStore = localCertificates.Find(X509FindType.FindByIssuerName, "frends", false);
            Assert.That(generatedCertificateInStore.Count, Is.EqualTo(0));
        }

        private static X509CertificateResultRaw ConvertBase64StringToValidRawCertificateData()
        {
            var base64 = "TUlJRjd6Q0NCTmVnQXdJQkFnSVJBTmRWajlyMThSQmJzaE1vSzNCM0thTXdEUVlKS29aSWh2Y05BUUVGQlFBdwpnWmN4Q3pBSkJnTlZCQVlUQWxWVE1Rc3dDUVlEVlFRSUV3SlZWREVYTUJVR0ExVUVCeE1PVTJGc2RDQk1ZV3RsCklFTnBkSGt4SGpBY0JnTlZCQW9URlZSb1pTQlZVMFZTVkZKVlUxUWdUbVYwZDI5eWF6RWhNQjhHQTFVRUN4TVkKYUhSMGNEb3ZMM2QzZHk1MWMyVnlkSEoxYzNRdVkyOXRNUjh3SFFZRFZRUURFeFpWVkU0dFZWTkZVa1pwY25OMApMVWhoY21SM1lYSmxNQjRYRFRFeE1ETXhOVEF3TURBd01Gb1hEVEUwTURNeE5ESXpOVGsxT1Zvd2dkOHhDekFKCkJnTlZCQVlUQWxWVE1RNHdEQVlEVlFRUkV3VXpPRFEzTnpFUU1BNEdBMVVFQ0JNSFJteHZjbWxrWVRFUU1BNEcKQTFVRUJ4TUhSVzVuYkdsemFERVhNQlVHQTFVRUNSTU9VMlZoSUZacGJHeGhaMlVnTVRBeEZEQVNCZ05WQkFvVApDMGR2YjJkc1pTQk1kR1F1TVJNd0VRWURWUVFMRXdwVVpXTm9JRVJsY0hRdU1TZ3dKZ1lEVlFRTEV4OUliM04wClpXUWdZbmtnUjFSSklFZHliM1Z3SUVOdmNuQnZjbUYwYVc5dU1SUXdFZ1lEVlFRTEV3dFFiR0YwYVc1MWJWTlQKVERFWU1CWUdBMVVFQXhNUGJHOW5hVzR1ZVdGb2IyOHVZMjl0TUlJQklqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQwpBUThBTUlJQkNnS0NBUUVBb2FRRlBlMkZSWk9LR0UzR0F3Qlg0a0IzOEJ6cjBCbmZJbDBJZjlFSFBFR0pSaGVqCkNmcjgrS2tFMFphUHE5ZFBQUG10R0tsMGdjUlhDam9tRnM1aVBydy9iQ0h1azQzTERhQWZtcGJRajYzMWs1T0MKN25JTW9YVVZvM3VFVnJpdC8xSVJjWVM4T2pBTGZwaW80YWcvTjFMUThYeHZrTmhGQ3F3NWNtcGgxYnZEalBuQwp6Ti85T25HNXI3emNPdHdNdHJIUzBZbTdRYmJ5M2xmVkZkLzgvZUl4eGQvS3dkaVBMTC93RGx0eDREUnh3OFZOCmZYclUrdTB3U3kvcXRpNmVrenppT3ZoQ29ocnUzTi9ORDZuMmVZUWFqbXdDdG9ibHYxRnFadmp6bk5OWkRIdWwKbVhqTmZKbjZ4cFpIMkRMVWRIWU9kMHNnZEtTM2lYV1NTclJiVlFJREFRQUJvNElCNmpDQ0FlWXdId1lEVlIwagpCQmd3Rm9BVW9YSmZKaHNvbUVPVlhRYzMxWVdXblV2U3cwVXdIUVlEVlIwT0JCWUVGSVpKUmZ3ekdUUFVCTzBuClllN29BY2tNZnk5K01BNEdBMVVkRHdFQi93UUVBd0lGb0RBTUJnTlZIUk1CQWY4RUFqQUFNQjBHQTFVZEpRUVcKTUJRR0NDc0dBUVVGQndNQkJnZ3JCZ0VGQlFjREFqQkdCZ05WSFNBRVB6QTlNRHNHRENzR0FRUUJzakVCQWdFRApCREFyTUNrR0NDc0dBUVVGQndJQkZoMW9kSFJ3Y3pvdkwzTmxZM1Z5WlM1amIyMXZaRzh1WTI5dEwwTlFVekI3CkJnTlZIUjhFZERCeU1EaWdOcUEwaGpKb2RIUndPaTh2WTNKc0xtTnZiVzlrYjJOaExtTnZiUzlWVkU0dFZWTkYKVWtacGNuTjBMVWhoY21SM1lYSmxMbU55YkRBMm9EU2dNb1l3YUhSMGNEb3ZMMk55YkM1amIyMXZaRzh1Ym1WMApMMVZVVGkxVlUwVlNSbWx5YzNRdFNHRnlaSGRoY21VdVkzSnNNSEVHQ0NzR0FRVUZCd0VCQkdVd1l6QTdCZ2dyCkJnRUZCUWN3QW9ZdmFIUjBjRG92TDJOeWRDNWpiMjF2Wkc5allTNWpiMjB2VlZST1FXUmtWSEoxYzNSVFpYSjIKWlhKRFFTNWpjblF3SkFZSUt3WUJCUVVITUFHR0dHaDBkSEE2THk5dlkzTndMbU52Ylc5a2IyTmhMbU52YlRBdgpCZ05WSFJFRUtEQW1nZzlzYjJkcGJpNTVZV2h2Ynk1amIyMkNFM2QzZHk1c2IyZHBiaTU1WVdodmJ5NWpiMjB3CkRRWUpLb1pJaHZjTkFRRUZCUUFEZ2dFQkFEMVh5VWdrWE81a2dmV3V2bFVwRnY4cUw0VHQyZmlqQThnd1pydkkKMUlFdElmY0k5NnlXUXBwQmRYcTZYUkFqeTVKQ1lmcUsybTFsTkJubHFkWXRFM2pYZ1VTU3FXNkFZeFhML2pVZgpBdEdLRkxDb3pKUWdPNmdhOEYwMlVOc05ydWxrNVBhTmFYMHd5QlFYQUVybHBqWDdmUTBpblhsMlVpeThsd2FJCm1oWDBjK2J4OVppbHpRZEVrYmluYUdkRjBuSVJZT0p4dDFCVjRvcXBEZGFTN2dRcWl6Q2dvZ1ZHTkcyU3hqdXEKVGFEUXF3RVpDakszNk9QUDhkS1hTWHVzcEpmMzhGZXVZM2VhZjViYVRmMiszQWMyNHlXOWlYbU9LUklUaTRnSAorMnZicE0yekxTZnAxTXBnMTRWVCszVEdYRFdNY0IvNXNyZVNKeURIbE5WbkZEQT0=";
            var res = CertificateStore.GetCertificateFromBase64(base64);
            Assert.IsNotNull(res.CertificateRawData);
            return res;
        }

        private static void CreateCertificateAndAddToLocalUserCertificateStore()
        {
            MakeCert();
            var res = CertificateStore.AddToLocalUserViaFile("certificate.cer", "CertificatePassword");
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
