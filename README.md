# Frends.CertificateStore
FRENDS Task for certificate stores.

- [Frends.CertificateStore](#frendscertificatestore)
- [Installing](#installing)
- [Tasks](#tasks)
  - [GetCertificateFromBase64](#GetCertificateFromBase64)
  - [GetAllCertificatesInSelectedStore](#GetAllCertificatesInSelectedStore)
  - [AddToLocalUserViaFile](#AddToLocalUserViaFile)
  - [AddToLocalUserViaRawData](#AddToLocalUserViaRawData)
  - [RemoveFromLocalUser](#RemoveFromLocalUser)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)

Installing
==========

You can install the task via FRENDS UI Task view, by searching for packages. You can also download the latest NuGet package from https://www.myget.org/feed/frends/package/nuget/Frends.CertificateStore and import it manually via the Task view.

Tasks
=====

## GetCertificateFromBase64
GetCertificateFromBase64 can be used to get a certificates raw data from a Base64 string.

Input:

| Property          | Type                                   | Description										 | Example                                   |
|-------------------|----------------------------------------|---------------------------------------------------|-------------------------------------------|
| Base64            | string                                 | The Base64 to decode                              | `#result[Base64]` or `TUI<...>=`          |

Result:

| Property           | Type														| Description                                                             |
|--------------------|----------------------------------------------------------|-------------------------------------------------------------------------|
| Result             | Object { byte[] CertificateRawData }						| CertificateRawData will contain the decoded raw data of the certificate |


## GetAllCertificatesInSelectedStore
GetAllCertificatesInSelectedStore can be used to get all certificates from a selected store.

Input:

| Property              | Type                                       | Description										 |
|-----------------------|--------------------------------------------|---------------------------------------------------|
| CertificateStoreInput | enum { CurrentUser = 1, LocalMachine = 2 } | The store to get certificates from                |

Result:

| Property           | Type																																																															   | Description                                             |
|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------|
| Result             | List [ X509Cert { X509ExtensionCollection Extensions, X500DistinguishedName SubjectName, string SerialNumber, byte[] RawData, X500DistinguishedName IssuerName, string FriendlyName, bool Archived, int Version, string Thumbprint, Oid SignatureAlgorithm } ]  | X509Cert will contain information about the certificate |


## AddToLocalUserViaFile
AddToLocalUser can be used to add a certificate to the Local User Certificate Store via a file.

Input:

| Property          | Type                                   | Description										 | Example                                   |
|-------------------|----------------------------------------|---------------------------------------------------|-------------------------------------------|
| Path              | string                                 | The path to the certificate to be added.          | `#result[Path]` or `f:\path\to\cert.cer`  |
| Password          | string                                 | A password for the certificate, can be left empty | `password`                                |

Result:

| Property          | Type						| Description                                       |
|-------------------|---------------------------|---------------------------------------------------|
| Result            | Object { bool Success }	| Success will be true if the certificate was added |


## AddToLocalUserViaRawData
AddToLocalUserViaRawData can be used to add a certificate to the Local User Certificate Store via raw data.

Input:

| Property           | Type                                   | Description									      | Example                                   |
|--------------------|----------------------------------------|---------------------------------------------------|-------------------------------------------|
| CertificateRawData | byte[] OR string                       | The raw data of the certificate to be added.      | `#result[RawData]` or `MII<...>=`         |
| Password           | string                                 | A password for the certificate, can be left empty | `password`                                |

Result:

| Property          | Type						| Description                                       |
|-------------------|---------------------------|---------------------------------------------------|
| Result            | Object { bool Success }	| Success will be true if the certificate was added |


## RemoveFromLocalUser
RemoveFromLocalUser can be used to remove a certificate from the Local User Certificate Store.

Input:

| Property          | Type                                   | Description										   | Example                             |
|-------------------|----------------------------------------|-----------------------------------------------------|-------------------------------------|
| CertificateName   | string                                 | The certificates to be removed by its subject name. | `#result[SubjectName]` or `certname`|

Result:

| Property          | Type                    | Description                                                      |
|-------------------|-------------------------|------------------------------------------------------------------|
| Result            | Object { bool Success } | Success will be true if the certificate was removed successfully |



License
=======
This project is licensed under the MIT License - see the LICENSE file for details

Building
========

Clone a copy of the repo

`git clone https://github.com/FrendsPlatform/Frends.CertificateStore.git`

Restore dependencies

`dotnet restore`

Rebuild the project

`dotnet build` 

Run tests

`dotnet test Frends.CertificateStore.Tests`

Create a nuget package

`dotnet pack Frends.CertificateStore`

Contributing
============
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!