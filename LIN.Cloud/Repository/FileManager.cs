﻿namespace LIN.Cloud.Repository;


public class FileManager
{

    /// <summary>
    /// Usuario
    /// </summary>
    private string User { get; set; }



    /// <summary>
    /// Ruta de la carpeta del usuario
    /// </summary>
    private string UserPath { get; set; }



    /// <summary>
    /// Iniciar instancia del file manager
    /// </summary>
    /// <param name="user">Usuario</param>
    public FileManager(string user)
    {
        this.User = user;
        UserPath = Path.Combine(FileConst.Path, User);
    }




    /// <summary>
    /// Asegura de tener las carpetas creadas
    /// </summary>
    public bool EnsureCreated()
    {
        try
        {
            // Si no existe carpeta del usuario
            if (!Directory.Exists(UserPath))
                Directory.CreateDirectory(UserPath);

            // Carpetas necesarias
            string[] baseFolders = { "storage" };

            // Crear la carpetas si no existen
            foreach (var folder in baseFolders)
            {
                string dataRoute = Path.Combine(UserPath, folder);
                if (!Directory.Exists(dataRoute))
                    Directory.CreateDirectory(dataRoute);
            }

            return true;
        }
        catch
        {
            return false;
        }

    }



    /// <summary>
    /// Guarda un archivo en el storage
    /// </summary>
    /// <param name="data">Elemento a guardar</param>
    /// <param name="subFolder">Folder de storage donde se guardara</param>
    /// <param name="fileName">Nombre del archivo</param>
    public bool Save(byte[] data, string subFolder, string fileName)
    {
        try
        {
            var folderRoute = Path.Combine(UserPath, "storage", subFolder);
            Directory.CreateDirectory(folderRoute);

            var fileRoute = Path.Combine(folderRoute, fileName);

            if (File.Exists(fileRoute))
            {
                return false;
            }

            File.WriteAllBytes(fileRoute, data);

            return true;
        }
        catch
        {
            return false;
        }
    }



    public byte[] Get(string file)
    {
        try
        {
            var folderRoute = Path.Combine(UserPath, file);


            if (!File.Exists(folderRoute))
            {
                return Array.Empty<byte>();
            }

            var data = File.ReadAllBytes(folderRoute);

            return data;
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }




    public StorageMap GetMap()
    {
        try
        {

            StorageMap map = new();
            map.Folder = new();


            BuildDirectory(map, map.Folder, new string[] { UserPath + "/storage" });

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
