namespace LIN.Cloud.Controllers;

[Route("[controller]")]
[ServiceFilter(typeof(IdentityTokenAttribute))]
public class FileController(IFileRepository fileManager, BucketService bucketService) : ControllerBase
{

    /// <summary>
    /// Crear un nuevo archivo en un contenedor.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create(IFormFile modelo, [FromQuery] bool aleatoryName = false)
    {

        // Modelo es null
        if (modelo is null)
            return new CreateResponse()
            {
                Message = "Parámetros inválidos,",
                Response = Responses.InvalidParam
            };

        // Obtener el tamaño en bytes.
        double sizeInKb = modelo.Length.BytesaKB();

        // Si no queda espacio.
        if (bucketService.Bucket?.MaxSize < (bucketService.Bucket?.ActualSize + sizeInKb))
            return new CreateResponse()
            {
                Message = $"No tienes espacio suficiente en el contenedor {bucketService.Bucket.Name}",
                Response = Responses.Unauthorized
            };

        // Crear el archivo
        var (created, name) = await fileManager.Save(modelo, aleatoryName);

        // Si no se pudo crear
        if (!created)
        {
            return new CreateResponse()
            {
                Response = Responses.Undefined,
                Message = "No se creo el archivo."
            };
        }

        return new CreateResponse()
        {
            LastUnique = name,
            Response = Responses.Success,
            Message = $"Se creo el archivo."
        };
    }



    [HttpGet("map")]
    public async Task<HttpReadOneResponse<StorageMap>> GetMap()
    {

        // Crear el archivo
        var result = fileManager.GetMap();

        return new ReadOneResponse<StorageMap>()
        {
            Response = Responses.Success,
            Model = result
        };


    }



    [HttpGet("file")]
    public async Task<HttpReadOneResponse<FileModel>> GetMap([FromQuery] string route)
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
    public async Task<IActionResult> k([FromQuery] string route)
    {

        // Crear el archivo
        var result = fileManager.Get(route);

        return File(result.Data, result.MimeType, result.Name);

    }

}