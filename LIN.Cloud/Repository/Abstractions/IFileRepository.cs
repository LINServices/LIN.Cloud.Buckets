namespace LIN.Cloud.Repository.Abstractions;

public interface IFileRepository
{
    public Task<(bool, string)> Save(IFormFile data, bool aleatoryName);

    public FileModel? Get(string file);

    public StorageMap GetMap();

}