using LIN.Cloud.Buckets.Persistence.Data;

namespace LIN.Cloud.Buckets.Controllers;

[Route("[controller]")]
public class BucketController(BucketData bucketData) : ControllerBase
{

    /// <summary>
    /// Crear un nuevo Bucket.
    /// </summary>
    /// <param name="modelo">Modelo de la cuenta de almacenamiento.</param>
    /// <param name="cloud">Token cloud.</param>
    /// <returns>Retorna el resultado de la operación.</returns>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] BucketModel modelo, [FromHeader] string cloud)
    {
        // Validar el modelo.
        if (modelo is null || string.IsNullOrWhiteSpace(modelo.Name))
            return new CreateResponse()
            {
                Message = "Parámetros inválidos,",
                Response = Responses.InvalidParam
            };

        // Validar token Cloud.
        var (authenticated, project) = Identity.Utilities.JwtCloud.Validate(cloud);

        if (!authenticated)
            return new CreateResponse()
            {
                Response = Responses.Unauthorized,
                Message = "Token cloud invalido."
            };

        // Modelo.
        modelo.ProjectId = project;

        // El tamaño llega en MB lo convertimos a KB.
        modelo.MaxSize *= 1024;

        // Crear el contenedor.
        var response = await bucketData.Create(modelo);

        return response;
    }


    /// <summary>
    /// Obtener información de una cuenta de almacenamiento.
    /// </summary>
    /// <param name="cloud">Token cloud.</param>
    [HttpGet]
    public async Task<HttpReadOneResponse<Types.Developer.Projects.BucketProject>> Read([FromHeader] string cloud)
    {

        // Validar token.
        var (authenticated, project) = Identity.Utilities.JwtCloud.Validate(cloud);

        // Si hubo un error.
        if (!authenticated)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "Token cloud es invalido."
            };

        // Obtener el contendor.
        var response = await bucketData.ReadByProject(project);

        return new ReadOneResponse<Types.Developer.Projects.BucketProject>()
        {
            Model = new()
            {
                IsProvisioned = response.Response == Responses.Success,
                BucketSize = response.Model.MaxSize,
                BucketActualSize = response.Model.ActualSize,
                Status = Types.Developer.Enumerations.ProjectStatus.Normal
            },
            Response = response.Response
        };
    }

}