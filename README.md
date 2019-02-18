# Ocelot.Extensions
Ocelot Extensions is the project containing some extension to the popular open-source API Gateway engine **Ocelot**.  
This project started from the need to extend Ocelot in a plugin fashion, without touching the main codebase and without propose expensive pull request for minor features.
Initially the project is focused on Configuration extensions, providing logic to make the _ocelot.json_ configuration file external to the project, to be centrally maintained and updated.
## Features
At the time being Ocelot Extensions provides this set of features:
* Configuration injection (`Ocelot.Extensions.Configuration`)
  * From Azure Storage (either with Blob or File service)
    * with both Shared Key or SAS authentication methods
  * From Google Cloud Storage
    * using the interoperability Access Key authentication method

## Roadmap
Among issues are coming after the initial release, I would like to add support for this scenarios:
* Configuration injection (`Ocelot.Extensions.Configuration`)
  * Support for Amazon S3, SQL-like RDBMS, plain HTTP and Redis
* Pipeline extensions
* Analytics

## Dependencies
Ocelot Extensions is designed to keep itself simple without any (when possible) external dependencies.  
This means, for example, that the Azure Storage access or the Google Cloud Storage one, has been made without the official library and, specifically, by using directly the native REST API of the appropriate services.
So, at the time being, the only dependency of `Ocelot.Extensions.Configuration` is the Ocelot main library itself.  
This also reduce the issues related to legal compliance (with third party libraries) and with reference consistency.

## Ocelot.Extensions.Configuration
This extension lets you define an external source (now in Azure Storage and Google Cloud storage only) to host the ocelot.json configuration. The extension will poll the external source every `CheckingInterval` seconds.
### Why inject configuration from outside?
Because in High-available and High-reliable systems, we have multiple computation nodes (for example the API Gateway engine) that should be stateless. So, in case configuration changed, we don't need anymore to deploy those changes on each node affected by the change.
Also Ocelot implementations running in containers can benefit from this approach, since it makes the Ocelot deployment completely stateless.
### There are already many configuration intergrations using the Provider.* pattern, why this is different?
I do believe that an API Gateway should be as fastest as possibile, so the configuration of routing and rules should be, if possibile, in memory and/or on disk, and cached. The idea of Ocelot author to make it a JSON file is awesome, while the external lookup against other services, IMHO, can decrease the Gateway performance, adding another layer of complexity (a new application building block) to manage and maintain.

### Cloud costs implications
Accessing the various storage supported by the library has a cost. With the minimum value of `CheckingInterval` of 1 second, we are making about 30M of calls per year, just to check news. This volume generates a cloud costs of about 10-15$ per year, plus the bandwidth.
I already optimized the library to download the new configuration only if it changed, so the bandwidth used is minimal. 
While with many cloud providers, the bandwidth consumed inside the datacenter is free of charge, consider this implication when adopting this extension.

### How to install
Basic runtime requirements are the same as the main project Ocelot. This means the library here is .NET Standard.  
In the next few days there will be a Nuget package:  
`Install-Package Ocelot.Extensions.Configuration`  <= Not yet available  
Or via the .NET Core CLI:  
`dotnet all package Ocelot.Extensions.Configuration` <= Not yet available

### Usage

Since it is an Ocelot add-on, locate the point when you make the `AddOcelot()` call in the `ConfigureServices` section and add the `.WithConfigurationRepository()` extension:  
``` csharp
.ConfigureServices((ctx, s) => {
                s.AddOcelot()
                    .WithConfigurationRepository();                
            })
```
You can also do that in the `Startup.cs` file.

#### appsettings.json
This extensions requires the following configuration section in the running host:
``` json
"Ocelot.Extensions": {
    "Configuration": {
      "RepositoryType": "AzureStorage",
      "CheckingInterval": 10,
      "AzureStorage": {
        "Type": "[Blob|File]",
        "AccessType": "[SharedKey|SignedUri]",
        "AccountName": "<accountName>",
        "AccountKey": "<accountKey>",
        "ResourceUri": "[<container|share/blob|file>|<full SAS URI>]"
      },
      "GoogleCloudStorage": {
        "BucketName": "<bucketName>",
        "ObjectName": "<objectName>",
        "AccessKey": "<accessKey>",
        "Secret": "<accessSecret>"
      }
    }
  }
```
Depending on the RepositoryType choosen (`AzureStorage, GoogleCloudStorage`) it will use the appropriate sub-section with access details.

### Known issues
For now, the configuration extension overrides the default `IFileConfigurationSetter` provider of Ocelot. This behaviour has been overridden to overcome the limitation of a fixed `FileConfiguration` schema for the `ocelot.json` configuration file. In the future, I will integrate better the existing Ocelot options (InMemory, Consul) with the extension.

### Sample: PollingConfigurationFromAzure
This sample show how to integrate the `Ocelot.Extensions.Configuration` into your existing Ocelot project.

## Documentation
Since an official set of pages are not yet coming in the next weeks, please open issues and use GitHub tools to ask for support.