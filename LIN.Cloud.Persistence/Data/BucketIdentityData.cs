using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Types.Cloud.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Persistence.Data;

public class BucketIdentityData(DataContext context)
{
    public async Task<CreateResponse> Create(BucketIdentityModel bucket)
    {
        bucket.Id = 0;
        try
        {

            context.Attach(bucket.BucketModel);


            await context.BucketIdentities.AddAsync(bucket);
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


    public async Task<CreateResponse> Create(BucketAccessModel bucket)
    {
        bucket.Id = 0;
        try
        {

            context.Attach(bucket.Bucket);


            await context.BucketAccess.AddAsync(bucket);
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

    public async Task<ResponseBase> HasAdminFor(int bucket, int identity)
    {
        try
        {
            var has = await (from b in context.BucketIdentities
                             where b.IdentityId == identity
                             && b.BucketId == bucket
                             select b).AnyAsync();


            return new(has ? Responses.Success : Responses.Unauthorized);
        }
        catch
        {

        }
        return new();
    }


    public async Task<ReadOneResponse<BucketModel>> Key(string key)
    {
        try
        {
            var has = await (from b in context.BucketAccess
                             where b.Key == key
                             select b.Bucket).FirstOrDefaultAsync();


            return new(has != null ? Responses.Success : Responses.Unauthorized, has);
        }
        catch
        {

        }
        return new();
    }

    public async Task<CreateResponse> CreateKey(BucketAccessModel bucketAccess)
    {
        try
        {

            bucketAccess.Bucket = new()
            {
                Id = bucketAccess.BucketId
            };

            context.Attach(bucketAccess.Bucket);

            context.Add(bucketAccess);
            context.SaveChanges();

            return new(Responses.Success);
        }
        catch
        {

        }
        return new();
    }





}