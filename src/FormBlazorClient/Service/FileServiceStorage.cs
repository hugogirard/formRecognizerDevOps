using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace FormBlazorClient.Service;

public class FileServiceStorage : IFileServiceStorage
{
    private readonly BlobContainerClient _container;
    
    public FileServiceStorage(IConfiguration configuration)
    {
        _container = new BlobContainerClient(configuration["UploadStorage"],
                                             configuration["Container"]);
    }

    public async Task<Uri> UploadAsync(Stream content)
    {
        string filename = $"{Guid.NewGuid().ToString()}.pdf";

        BlobClient blob = _container.GetBlobClient(filename);

        await blob.UploadAsync(content);

        // Generate a SAS token valid for 1 hour
        var sasBuilder = new BlobSasBuilder()
        {            
            BlobContainerName = blob.GetParentBlobContainerClient().Name,
            BlobName = blob.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Specify read and write permissions for the SAS.
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blob.GenerateSasUri(sasBuilder);

    }
}
