using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Types.Cloud.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Persistence.Data;

public class BucketData(DataContext context)
{
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
                LastID = bucket.Id
            };
        }
        catch
        {

        }
        return new();
    }



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


    public async Task<ReadOneResponse<BucketModel>> ReadByProject(int id)
    {
        try
        {
            var bucket = await (from b in context.Buckets
                                where b.ProjectId == id
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





}