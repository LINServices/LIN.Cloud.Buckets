using Microsoft.Extensions.Primitives;

namespace LIN.Cloud.Controllers;

[Route("[controller]")]
public class PublicFilesController(BucketService bucketService, Persistence.Data.PublicFilesData publicFilesData) : ControllerBase
{


    /// <summary>
    /// Obtener la ruta publica para un archivo.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpGet("token")]
    [ServiceFilter(typeof(IdentityTokenAttribute))]
    public async Task<HttpReadOneResponse<StringValues>> GetPublic([FromQuery] string path, [FromQuery] int minutes)
    {
        // Obtener la ruta publica.
        var key = await bucketService.GetPublic(path, minutes);

        // Respuestas.
        return new ReadOneResponse<StringValues>()
        {
            Response = Responses.Success,
            Model = key
        };
    }


    /// <summary>
    /// Acceder a un archivo.
    /// </summary>
    [HttpGet("{path}")]
    public async Task<IActionResult> File([FromRoute] string path)
    {

        // Validar la llave.
        var exist = await publicFilesData.Read(path);

        if (exist.Response != Responses.Success)
            return NotFound();

        bucketService.SetData(new()
        {
            Id = exist.Model.Bucket
        });

        var file = bucketService.Get(exist.Model.Path);

        return File(file.Data, file.MimeType, file.Name);

    }

}