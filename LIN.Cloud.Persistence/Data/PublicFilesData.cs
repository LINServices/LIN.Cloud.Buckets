using LIN.Cloud.Persistence.Context;
using LIN.Types.Cloud.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Persistence.Data;

public class PublicFilesData(DataContext context)
{

    /// <summary>
    /// Crear un nuevo archivo publico.
    /// </summary>
    /// <param name="file">Archivo.</param>
    public async Task<CreateResponse> Create(PublicFileModel file)
    {
        try
        {
            await context.PublicFiles.AddAsync(file);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success
            };
        }
        catch
        {
        }
        return new();
    }


    /// <summary>
    /// Obtener un archivo publico valido.
    /// </summary>
    /// <param name="id">Id del archivo.</param>
    public async Task<ReadOneResponse<PublicFileModel>> Read(string id)
    {
        try
        {
            var publicFile = await (from b in context.PublicFiles
                                where b.Key == id
                                where b.Expires == null || b.Expires > DateTime.UtcNow
                                select b).FirstOrDefaultAsync();

            if (publicFile is null)
                return new(Responses.NotRows);
            
            return new(Responses.Success, publicFile);
        }
        catch
        {
        }
        return new();
    }

}