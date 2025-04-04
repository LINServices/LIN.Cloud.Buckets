using LIN.Cloud.Buckets.Repository.Abstractions;
using LIN.Cloud.Buckets.Services;
using System.Net.Sockets;

namespace LIN.Cloud.Buckets.Repository;

public class FileRepository(BucketService bucketService) : IFileRepository
{

    /// <summary>
    /// Guardar archivo.
    /// </summary>
    public async Task<(ReadAllResponse<int>, string)> Save(IFormFile data, string? path,bool aleatoryName)
    {
        try
        {
            // Guardar el archivo.
            string name = aleatoryName ? Guid.NewGuid().ToString() : data.FileName;
            var response = await bucketService.Save(data, path ?? string.Empty, name);
            return (response, name);
        }
        catch(Exception ex)
        {
            return (new()
            {
                Errors = [new() { Description = ex.Message}]
            }, string.Empty);
        }
    }


    /// <summary>
    /// Obtener archivo.
    /// </summary>
    /// <param name="file">Archivo.</param>
    public FileModel? Get(string file)
    {
        try
        {
            var fileData = bucketService.Get(file);
            return fileData;
        }
        catch
        {
            return new();
        }
    }


    /// <summary>
    /// Obtener el mapa de carpetas.
    /// </summary>
    public StorageMap GetMap(string? path = null)
    {
        try
        {

            StorageMap map = new()
            {
                Folder = new()
            };

            BuildDirectory(map, map.Folder, [Path.Combine(bucketService.Path, path ?? "")]);

            return map;
        }
        catch
        {
            return new();
        }
    }






    private string[] GetFolders(string path)
    {
        return Directory.GetDirectories(path);
    }

    private string[] GetFiles(string path)
    {
        return Directory.GetFiles(path);
    }




    private void BuildDirectory(StorageMap map, Folder folder, string[] directories)
    {


        foreach (string directory in directories)
        {

            var info = Path.GetFileName(directory);

            var newFolder = new Folder()
            {
                Name = info
            };



            BuildDirectory(map, newFolder, GetFolders(directory));
            BuildFiles(map, newFolder, GetFiles(directory));

            folder.Folders.Add(newFolder);

        }
    }

    private void BuildFiles(StorageMap map, Folder folder, string[] files)
    {

        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            decimal size = fileInfo.Length / (decimal)(1024.0 * 1024.0);
            folder.Files.Add(new()
            {
                Name = fileInfo.Name,
                SizeMB = size
            });
            map.Size += size;

        }

    }



}
