using System.IO;

public static class Utils
{
    public static void RemoveDirectoryRecursive(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, recursive: true);
    }

    public static bool IsOnlyDigits(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        foreach (var c in s)
            if (c < '0' || c > '9') return false;
        return true;
    }

    public static bool ResolveHostname(string host)
    {
        return host == "localhost" || host.EndsWith(".com");
    }
}