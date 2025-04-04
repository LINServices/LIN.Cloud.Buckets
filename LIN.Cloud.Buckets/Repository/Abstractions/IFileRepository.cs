namespace LIN.Cloud.Buckets.Repository.Abstractions;

public interface IFileRepository
{
    public Task<(ReadAllResponse<int>, string)> Save(IFormFile data, string? path, bool aleatoryName);

    public FileModel? Get(string file);

    public StorageMap GetMap(string? path = null);

}