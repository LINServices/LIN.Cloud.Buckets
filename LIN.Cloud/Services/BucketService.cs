namespace LIN.Cloud.Services;

public class BucketService(LIN.Cloud.Persistence.Data.BucketData bucketData)
{

    /// <summary>
    /// Ruta default.
    /// </summary>
    public static string Default { get; set; } = "wwwroot/data";


    /// <summary>
    /// BucketProjectId.
    /// </summary>
    public BucketModel? Bucket { get; set; }


    /// <summary>
    /// Establecer el contenedor.
    /// </summary>
    /// <param name="bucket">Contenedor.</param>
    public void SetData(BucketModel bucket)
    {
        Bucket = bucket;
        SetData();
    }


    /// <summary>
    /// Ruta principal.
    /// </summary>
    public string Path { get; set; } = string.Empty;


    /// <summary>
    /// Establecer la data del bucket.
    /// </summary>
    public void SetData()
    {

        // Ruta del bucket.
        Path = System.IO.Path.Combine(Default, "storage", Bucket.Id.ToString());

        // Crear el directorio.
        Create();
    }


    /// <summary>
    /// Crear bucket si no existe.
    /// </summary>
    private void Create()
    {
        Directory.CreateDirectory(Path);
    }


    /// <summary>
    /// Guardar archivo.
    /// </summary>
    /// <param name="file">Archivo.</param>
    public async Task<bool> Save(IFormFile file, string name)
    {

        // Ruta del archivo.
        var filePath = System.IO.Path.Combine(Path, name);

        // Validar si existe el archivo.
        if (File.Exists(filePath))
            return false;

        // Guardar el archivo en el servidor.
        using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
        await file.CopyToAsync(stream);

        // Actualizar 
        await bucketData.UpdateSize(Bucket?.Id ?? 0, file.Length.BytesaKB());

        return true;

    }


    /// <summary>
    /// Obtener archivo.
    /// </summary>
    /// <param name="path">Ruta al archivo.</param>
    public FileModel? Get(string path)
    {
        // Ruta del archivo.
        var filePath = System.IO.Path.Combine(Path, path);

        // Validar si existe el archivo.
        if (!File.Exists(filePath))
            return null;

        // Obtener información.
        FileInfo fileInformation = new(filePath);

        // Obtener bytes.
        byte[] data = File.ReadAllBytes(filePath);

        return new()
        {
            Data = data,
            Name = fileInformation.Name,
            MimeType = GetMimeType(fileInformation.FullName)
        };
    }


    /// <summary>
    /// Obtener tipo MIME.
    /// </summary>
    /// <param name="filePath">Ruta.</param>
    public static string GetMimeType(string filePath)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out string? mimeType))
        {
            mimeType = "application/octet-stream"; // Tipo MIME por defecto
        }
        return mimeType;
    }

}