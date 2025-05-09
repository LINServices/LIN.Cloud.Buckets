namespace LIN.Cloud.Buckets.Controllers;

[Route("[controller]")]
[ServiceFilter(typeof(IdentityKeyAttribute))]
public class FileController(IFileRepository fileManager, BucketService bucketService) : ControllerBase
{

    /// <summary>
    /// Crear un nuevo archivo en el repositorio.
    /// </summary>
    /// <param name="modelo">Modelo del archivo.</param>
    /// <param name="path">Carpeta.</param>
    /// <param name="aleatoryName">�Tendr� nombre aleatorio?</param>
    /// <param name="public">�Sera un archivo publico por defecto?</param>
    /// <returns>Retorna la direcci�n del archivo.</returns>
    [HttpPost]
    public async Task<HttpReadOneResponse<dynamic>> Create(IFormFile modelo, [FromQuery] string? path = null, [FromQuery] bool aleatoryName = false, [FromQuery] bool @public = false)
    {

        // Modelo es null
        if (modelo is null)
            return new()
            {
                Message = "Par�metros inv�lidos.",
                Response = Responses.InvalidParam
            };

        // Obtener el tama�o en bytes.
        double sizeInKb = modelo.Length.BytesaKB();

        // Si no queda espacio.
        if (bucketService.Bucket.MaxSize < bucketService.Bucket?.ActualSize + sizeInKb)
            return new()
            {
                Message = $"No tienes espacio suficiente en el contenedor {bucketService.Bucket?.Name}",
                Response = Responses.InsufficientStorage
            };

        // Crear el archivo.
        var (response, name) = await fileManager.Save(modelo, path, aleatoryName);

        // Si no se pudo crear.
        if (response.Response != Responses.Success)
        {
            return new()
            {
                Response = Responses.Undefined,
                Message = "No se creo el archivo.",
                Errors = response.Errors
            };
        }

        // Si se puede marcar publico
        string key = string.Empty;
        if (@public)
        {
            // Obtener la ruta publica.
            key = await bucketService.GetPublic(Path.Combine(path ?? string.Empty, name), -1);
        }

        return new ReadOneResponse<dynamic>()
        {
            Model = new
            {
                publicPath = key,
                name
            },
            Response = Responses.Success,
            Message = $"Se creo el archivo."
        };
    }


    /// <summary>
    /// Obtener el mapa de los archivos.
    /// </summary>
    [HttpGet("map")]
    public HttpReadOneResponse<StorageMap> GetMap([FromQuery] string? path = null)
    {

        // Obtener el mapa.
        var result = fileManager.GetMap(path);

        // Respuestas.
        return new ReadOneResponse<StorageMap>()
        {
            Response = Responses.Success,
            Model = result
        };
    }



    [HttpGet("file")]
    public async Task<HttpReadOneResponse<FileModel>> Getf([FromQuery] string route)
    {

        // Crear el archivo
        var result = fileManager.Get(route);

        return new ReadOneResponse<FileModel>()
        {
            Response = Responses.Success,
            Model = result
        };


    }


    [HttpGet]
    public async Task<IActionResult> GetFile([FromQuery] string route)
    {
        // Crear el archivo
        var result = fileManager.Get(route);

        return File(result.Data, result.MimeType, result.Name);
    }

}