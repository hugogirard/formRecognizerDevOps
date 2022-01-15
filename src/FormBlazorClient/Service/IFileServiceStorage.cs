
namespace FormBlazorClient.Service
{
    public interface IFileServiceStorage
    {
        Task<Uri> UploadAsync(Stream content);
    }
}