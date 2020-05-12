#if false
[MenuItem("SERVERBUILD/Server/Modes/Server Mode")]
public static void TurnToServerMode()
{
    ExcludeFiles(true);
}

[MenuItem("SERVERBUILD/Server/Modes/Client Mode")]
public static void TurnToClientMode()
{
    ExcludeFiles(false);
}

private static void ExcludeFiles(bool exclude)
{
    const string ROOT_FOLDER = @"Assets";

    var excludeExtensions = new[] { ".png", ".jpg", ".mat", ".shader", ".FBX", ".tga", ".wav", ".tif" };

    var directories = Directory.GetDirectories(ROOT_FOLDER, "*", SearchOption.AllDirectories);
    foreach (var directory in directories)
    {
        var files = Directory.GetFiles(directory);
        foreach (var file in files)
        {
            var pathExtension = Path.GetExtension(file);
            if (!excludeExtensions.Contains(Path.GetExtension(file)) ||
                (exclude && file.Contains("~")) || (!exclude && !file.Contains("~"))) continue; // we don't want to exclude these

            if (exclude)
                File.Move(file, file.Replace(pathExtension, "") + "~" + pathExtension);
            else
                File.Move(file, file.Replace("~", ""));
        }
    }
}
#endif