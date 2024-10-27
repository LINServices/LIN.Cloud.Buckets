namespace LIN.Cloud.Services;

public class BucketService
{

    /// <summary>
    /// Ruta default.
    /// </summary>
    public static string Default { get; set; } = "wwwroot/data";


    /// <summary>
    /// Bucket.
    /// </summary>
    private readonly int Bucket = 0;


    /// <summary>
    /// Nuevo servicio de bucket.
    /// </summary>
    public BucketService(IHttpContextAccessor accessor)
    {
        // Contexto HTTP.
        var httpContext = accessor.HttpContext;

        // Obtiene el valor.
        bool can = httpContext!.Request.Query.TryGetValue("key", out Microsoft.Extensions.Primitives.StringValues value);

        // Información del token.
        bool findBucket = true;

        // Error de autenticación.
        if (!can || !findBucket)
        {
        }

        Bucket = 1;
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
        Path = System.IO.Path.Combine(Default, "storage", Bucket.ToString());

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
    public async Task<bool> Save(IFormFile file)
    {

        // Ruta del archivo.
        var filePath = System.IO.Path.Combine(Path, file.FileName);

        // Validar si existe el archivo.
        if (File.Exists(filePath))
            return false;

        // Guardar el archivo en el servidor.
        using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
        await file.CopyToAsync(stream);

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