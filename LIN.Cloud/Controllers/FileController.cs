namespace LIN.Cloud.Controllers;

[Route("[controller]")]
[ServiceFilter(typeof(IdentityTokenAttribute))]
public class FileController(IFileRepository fileManager) : ControllerBase
{

    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create(IFormFile modelo)
    {

        // Modelo es null
        if (modelo == null)
        {
            return new CreateResponse()
            {
                Message = "Parámetros inválidos,",
                Response = Responses.InvalidParam
            };
        }


        // Obtiene el tamaño en disco
        long sizeInBytes = modelo.Length;

        decimal sizeInMB = (decimal)sizeInBytes / (decimal)(1024 * 1024);


        // Valida el tamaño disponible del usuario
        //if ((FileSize + sizeInMB) > MaxFileSize)
        //{
        //    return new CreateResponse()
        //    {
        //        Response = Responses.InsufficientStorage,
        //        Message = "No tienes suficiente espacio."
        //    };
        //}


        // Crear el archivo
        var result = await fileManager.Save(modelo);

        // Si no se pudo crear
        if (!result)
        {
            return new CreateResponse()
            {
                Response = Responses.Undefined,
                Message = "No se creo el archivo."
            };
        }

        // Registra el cambio
        //FileSize += sizeInMB;


        return new CreateResponse()
        {
            Response = Responses.Success,
            Message = $"Se creo el archivo con peso final de '{sizeInMB:0.00}' mb"
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