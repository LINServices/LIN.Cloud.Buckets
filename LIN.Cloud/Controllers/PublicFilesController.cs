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

        string extension = "";
        if (path.Contains('.'))
        {
            var separator = path.Split('.');
            path = separator[0];
            extension = "." + separator[1];
        }

        // Validar la llave.
        var exist = await publicFilesData.Read(path);

        // Si no se encontró.
        if (exist.Response != Responses.Success)
            return NoContent();

        // Iniciar el servicio de bucket.
        bucketService.SetData(new()
        {
            Id = exist.Model.Bucket
        });

        // Obtener el archivo.
        var file = bucketService.Get(exist.Model.Path);

        // Si el archivo no se encontró.
        if (file is null)
            return NoContent();

        return File(file.Data, file.MimeType, file.Name + extension);

    }

}