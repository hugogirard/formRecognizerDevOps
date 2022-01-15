using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace FormBlazorClient.Service;

public class FileServiceStorage : IFileServiceStorage
{
    private readonly BlobContainerClient _container;
    private UserDelegationKey _userDelegationKey;

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

        BlobServiceClient blobServiceClient = blob.GetParentBlobContainerClient().GetParentBlobServiceClient();

        if (DateTimeOffset.UtcNow > _userDelegationKey?.SignedExpiresOn)
        {
            // Get a user delegation key for the Blob service that's valid for 7 hours.
            // You can use the key to generate any number of shared access signatures 
            // over the lifetime of the key.
            _userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow,
                                                                                   DateTimeOffset.UtcNow.AddHours(7));
        }

        // Generate a SAS token valid for 1 hour
        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blob.BlobContainerName,
            BlobName = blob.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Specify read and write permissions for the SAS.
        sasBuilder.SetPermissions(BlobSasPermissions.Read);


        BlobUriBuilder blobUriBuilder = new BlobUriBuilder(blob.Uri)
        {
            // Specify the user delegation key.
            Sas = sasBuilder.ToSasQueryParameters(_userDelegationKey,
                                                  blobServiceClient.AccountName)
        };

        return blobUriBuilder.ToUri();
    }
}
