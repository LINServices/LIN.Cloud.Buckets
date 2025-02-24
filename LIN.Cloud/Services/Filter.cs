using LIN.Types.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LIN.Cloud.Services;

public class IdentityKeyAttribute(BucketService bucketService, Persistence.Data.BucketData bucketData) : ActionFilterAttribute
{

    /// <summary>
    /// Filtro del token.
    /// </summary>
    /// <param name="context">Contexto HTTP.</param>
    /// <param name="next">Siguiente.</param>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        // Contexto HTTP.
        var httpContext = context.HttpContext;

        // Obtiene el valor.
        bool can = httpContext.Request.Headers.TryGetValue("key", out Microsoft.Extensions.Primitives.StringValues value);

        // Obtener la ip del cliente.
        var ip = httpContext.Connection.RemoteIpAddress;

        // Información del token.
        var validate = await Access.Developer.Controllers.Project.Validate(value.ToString(), ip?.MapToIPv4().ToString() ?? string.Empty);

        // Error de autenticación.
        if (!can || validate.Response != Responses.Success)
        {
            httpContext.Response.StatusCode = 401;
            await httpContext.Response.WriteAsJsonAsync(new ResponseBase()
            {
                Message = "Llave invalida.",
                Errors = 
                [
                    new ErrorModel()
                    {
                        Tittle = "Llave invalido",
                        Description = "La llave proporcionado es invalida."
                    }
                ],
                Response = Responses.Unauthorized
            });
            return;
        }

        // Obtener el bucket.
        var bucket = await bucketData.ReadByProject(validate.Model);

        if (bucket.Response != Responses.Success)
        {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsJsonAsync(new ResponseBase()
            {
                Message = "Bucket not found.",
                Errors = [],
                Response = Responses.Unauthorized
            });
            return;
        }

        bucketService.SetData(bucket.Model);

        await base.OnActionExecutionAsync(context, next);

    }

}