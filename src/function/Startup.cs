using System;
using DemoForm;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DemoForm;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var blobService = new BlobServiceClient(Environment.GetEnvironmentVariable("DevStorageCnxString"));
        var containerClient = blobService.GetBlobContainerClient(Environment.GetEnvironmentVariable("ModelContainer"));

        builder.Services.AddSingleton<IFormClientFactory,FormClientFactory>();
        builder.Services.AddSingleton(containerClient);
    }
}