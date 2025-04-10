using LIN.Cloud.Buckets.Persistence.Context;
using LIN.Types.Cloud.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Buckets.Persistence.Data;

public class BucketData(DataContext context)
{

    /// <summary>
    /// Crear contenedor de archivos.
    /// </summary>
    /// <param name="bucket">Modelo.</param>
    public async Task<CreateResponse> Create(BucketModel bucket)
    {
        bucket.Id = 0;
        try
        {
            await context.Buckets.AddAsync(bucket);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
                LastId = bucket.Id
            };
        }
        catch
        {
        }
        return new();
    }


    /// <summary>
    /// Obtener un contenedor.
    /// </summary>
    /// <param name="id">Id del contenedor.</param>
    public async Task<ReadOneResponse<BucketModel>> Read(int id)
    {
        try
        {
            var bucket = await (from b in context.Buckets
                                where b.Id == id
                                select b).FirstOrDefaultAsync();

            if (bucket is null)
            {
                return new(Responses.NotRows);
            }

            return new(Responses.Success, bucket);
        }
        catch
        {

        }
        return new();
    }


    /// <summary>
    /// Obtener un contendor.
    /// </summary>
    /// <param name="id">Id del proyecto en LIN Developers.</param>
    public async Task<ReadOneResponse<BucketModel>> ReadByProject(int id)
    {
        try
        {
            var bucket = await (from b in context.Buckets
                                where b.ProjectId == id
                                select b).FirstOrDefaultAsync();

            if (bucket is null)
                return new(Responses.NotRows);

            return new(Responses.Success, bucket);
        }
        catch
        {
        }
        return new();
    }


    /// <summary>
    /// Eliminar un contenedor.
    /// </summary>
    /// <param name="id">Id del contenedor.</param>
    public async Task<ResponseBase> Delete(int id)
    {
        try
        {
            var bucket = await (from b in context.Buckets
                                where b.Id == id
                                select b).ExecuteDeleteAsync();

            if (bucket <= 0)
            {
                return new(Responses.NotRows);
            }

            return new(Responses.Success);
        }
        catch
        {
        }
        return new();
    }


    /// <summary>
    /// Actualizar el tamaño disponible de un contenedor.
    /// </summary>
    /// <param name="id">Id del contenedor.</param>
    /// <param name="add">Tamaño para agregar.</param>
    public async Task<ResponseBase> UpdateSize(int id, double add)
    {
        try
        {
            var bucket = await (from b in context.Buckets
                                where b.Id == id
                                select b).ExecuteUpdateAsync(t => t.SetProperty(t => t.ActualSize, t => t.ActualSize + add));

            if (bucket <= 0)
                return new(Responses.NotRows);

            return new(Responses.Success);
        }
        catch
        {
        }
        return new();
    }

}