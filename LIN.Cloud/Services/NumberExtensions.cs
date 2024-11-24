namespace LIN.Cloud.Services;

public static class NumberExtensions
{

    public static double MBToBytes(this double mb)
    {
        return (long)(mb * 1024 * 1024); // 1 MB = 1024 * 1024 Bytes
    }


    public static double BytesToMB(this long bytes)
    {
        return (double)bytes / (1024 * 1024); // Bytes a MB
    }


    public static double KBaBytes(this double kb)
    {
        return (double)(kb * 1024); // 1 KB = 1024 Bytes
    }


    public static double BytesaKB(this long bytes)
    {
        return (double)bytes / 1024; // Bytes a KB
    }

}
