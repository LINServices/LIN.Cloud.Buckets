using LIN.Cloud.Identity.Persistence.Contexts;
using LIN.Types.Cloud.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Persistence.Data;

public class PublicFilesData(DataContext context)
{

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



    public async Task<ReadOneResponse<PublicFileModel>> Read(string id)
    {
        try
        {
            var bucket = await (from b in context.PublicFiles
                                where b.Key == id
                                where b.Expires < DateTime.UtcNow
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


}