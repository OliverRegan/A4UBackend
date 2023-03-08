namespace Audit4You.Backend.Ports.Services;

public interface IFileService
{
	Task<string> UploadFileToDisk(IFormFile file);
}