namespace LIN.Cloud.Controllers;


[Route("file")]
public class FileController : ControllerBase
{


    /// <summary>
    /// Crea una nueva conversación.
    /// </summary>
    /// <param name="modelo">Modelo</param>
    [HttpPost("create")]    
    public async Task<HttpCreateResponse> Create([FromBody] FileModel modelo, [FromHeader] int user)
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
        long sizeInBytes = modelo.Data.Length;
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


        // File manager
        Repository.FileManager fileManager = new(user.ToString());

        // Validación de creación
        fileManager.EnsureCreated();

        // Crear el archivo
        var result = fileManager.Save(modelo.Data, modelo.Route, modelo.Name);

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
    public async Task<HttpReadOneResponse<StorageMap>> GetMap([FromHeader] int user)
    {

        
        // File manager
        Repository.FileManager fileManager = new(user.ToString());

        // Validación de creación
        fileManager.EnsureCreated();

        // Crear el archivo
        var result = fileManager.GetMap();

        return new ReadOneResponse<StorageMap>()
        {
            Response = Responses.Success,
            Model = result
        };


    }



    [HttpGet("file")]
    public async Task<HttpReadOneResponse<FileModel>> GetMap([FromQuery] string route, [FromHeader] int user)
    {


        // File manager
        Repository.FileManager fileManager = new(user.ToString());

        // Validación de creación
        fileManager.EnsureCreated();

        // Crear el archivo
        var result = fileManager.Get(route);

        return new ReadOneResponse<FileModel>()
        {
            Response = Responses.Success,
            Model = result
        };


    }




}