using LIN.Types.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LIN.Cloud.Services;

public class IdentityTokenAttribute(BucketService bucketService, Persistence.Data.BucketData bucketData) : ActionFilterAttribute
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

        // Información del token.

        var billing = await LIN.Access.Developer.Controllers.Billings.Create(value.ToString(), 0);

        // Error de autenticación.
        if (!can || billing.Response != Responses.Success)
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
                        Description = "El llave proporcionado en el header <key> es invalido."
                    }
                ],
                Response = Responses.Unauthorized
            });
            return;
        }

        // Obtener el bucket.
        var bucket = await bucketData.ReadByProject(billing.Model.ProjectId);

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