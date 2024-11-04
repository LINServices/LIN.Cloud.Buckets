using LIN.Cloud.Persistence.Data;

namespace LIN.Cloud.Controllers;

[Route("[controller]")]
public class BucketController(BucketData bucketData, BucketIdentityData bucketIdentityData) : ControllerBase
{


    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create(BucketModel modelo, int identity)
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

        var response = await bucketData.Create(modelo);

        var responseIdentity = await bucketIdentityData.Create(new BucketIdentityModel()
        {
            BucketModel = modelo,
            BucketId = response.LastID,
            IdentityId = identity,
        });

        return new()
        {
            Response = Responses.Success
        };

    }




    [HttpPost("create/key")]
    public async Task<HttpCreateResponse> CreateKey(int bucket)
    {

        string key = Global.Utilities.KeyGenerator.Generate(20, "pk.");
        var response = await bucketIdentityData.CreateKey(new()
        {
            BucketId = bucket,
            Key = key
        });

        return new CreateResponse()
        {
            Response = Responses.Success,
            LastUnique = key
        };

    }




}