using LIN.Cloud.Repository.Abstractions;

namespace LIN.Cloud.Repository;

public class FileManager(BucketService bucketService) : IFileRepository
{
    public async Task<bool> Save(IFormFile data)
    {
        try
        {

            // Guardar el archivo.
            bool saveResult = await bucketService.Save(data);

            return true;
        }
        catch
        {
            return false;
        }
    }



    public FileModel? Get(string file)
    {
        try
        {

           var fileData =  bucketService.Get(file);

            return fileData;
        }
        catch
        {
            return new();
        }
    }




    public StorageMap GetMap()
    {
        try
        {

            StorageMap map = new();
            map.Folder = new();


            BuildDirectory(map, map.Folder, new string[] {bucketService.Path });

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
            BuildFiles(map,newFolder, GetFiles(directory));

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
