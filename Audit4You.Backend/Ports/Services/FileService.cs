namespace Audit4You.Backend.Ports.Services;

public class FileService : IFileService
{
	private readonly IConfiguration _configuration;
	public FileService(IConfiguration configuration) { _configuration = configuration; }

	public async Task<string> UploadFileToDisk(IFormFile file)
	{
		// TODO: move to database or memory
		// var location  = _configuration["FileStoragePath"];
		var location = Directory.GetCurrentDirectory();
		var filename  = $"{Path.GetRandomFileName()}-{ Path.GetFileName(file.FileName)}";
		var wholePath = Path.Combine(location, filename);
		await using var stream = File.Create(wholePath);
		await file.CopyToAsync(stream);
		// {Path.GetRandomFileName()}-
		return wholePath;
	}
}