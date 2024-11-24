using LIN.Cloud.Persistence.Data;

namespace LIN.Cloud.Controllers;

[Route("[controller]")]
public class BucketController(BucketData bucketData) : ControllerBase
{


    [HttpPost]
    public async Task<HttpCreateResponse> Create(BucketModel modelo, [FromHeader]string key)
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

        var billing = await LIN.Access.Developer.Controllers.Billings.Create(key, 0);

        if (billing.Response != Types.Responses.Responses.Success)
            return new CreateResponse();

        var response = await bucketData.Create(modelo);

        return new()
        {
            Response = Responses.Success
        };

    }


    [HttpGet]
    public async Task<HttpReadOneResponse<BucketModel>> Read([FromHeader] string key)
    {

        var billing = await LIN.Access.Developer.Controllers.Billings.Create(key, 0);

        if (billing.Response != Types.Responses.Responses.Success)
            return new();

        var response = await bucketData.ReadByProject(billing.Model.ProjectId);

        return response;

    }

}