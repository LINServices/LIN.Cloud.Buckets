namespace LIN.Cloud.Buckets.Services;

public static class NumberExtensions
{
    public static double BytesaKB(this long bytes)
    {
        return (double)bytes / 1024; // Bytes a KB
    }

}
