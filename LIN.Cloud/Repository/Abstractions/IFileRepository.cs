namespace LIN.Cloud.Repository.Abstractions;

public interface IFileRepository
{
    public Task<bool> Save(IFormFile data);

    public FileModel? Get(string file);

    public StorageMap GetMap();

}