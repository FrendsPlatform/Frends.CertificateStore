# Frends.CertificateStore
FRENDS Task for certificate stores.

- [Frends.CertificateStore](#frendscertificatestore)
- [Installing](#installing)
- [Tasks](#tasks)
  - [AddToLocalUser](#AddToLocalUser)
  - [RemoveFromLocalUser](#RemoveFromLocalUser)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)

Installing
==========

You can install the task via FRENDS UI Task view, by searching for packages. You can also download the latest NuGet package from https://www.myget.org/feed/frends/package/nuget/Frends.CertificateStore and import it manually via the Task view.

Tasks
=====

## AddToLocalUser
AddToLocalUser can be used to add a certificate to the Local User Certificate Store.

Input:

| Property          | Type                                   | Description										 | Example                                   |
|-------------------|----------------------------------------|---------------------------------------------------|-------------------------------------------|
| Path              | string                                 | The path to the certificate to be added.          | `#result[Path]` or `f:\path\to\cert.cer`  |
| Password          | string                                 | A password for the certificate, can be left empty | `password`                                |

Result:

| Property          | Type                                                  | Description                                       |
|-------------------|-------------------------------------------------------|---------------------------------------------------|
| Result            | Object { bool Success, CryptographicException Error } | Success will be true if the certificate was added |


## RemoveFromLocalUser
RemoveFromLocalUser can be used to remove a certificate from the Local User Certificate Store.

Input:

| Property          | Type                                   | Description										   | Example                             |
|-------------------|----------------------------------------|-----------------------------------------------------|-------------------------------------|
| CertificateName   | string                                 | The certificates to be removed by its subject name. | `#result[SubjectName]` or `certname`|

Result:

| Property          | Type                                                  | Description                                                                                                                   |
|-------------------|-------------------------------------------------------|------------------------------------------------------------------|
| Result            | Object { bool Success, CryptographicException Error } | Success will be true if the certificate was removed successfully                                                                             |



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