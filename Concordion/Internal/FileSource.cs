using Concordion.Api;

namespace Concordion.Internal;

public class FileSource(string baseDirectory) : Source {
    private readonly string baseDirectory = Path.GetFullPath(baseDirectory);

    public TextReader CreateReader(Resource resource)
    {
        return new StreamReader(new FileStream(ExistingFilePath(resource), FileMode.Open));
    }

    public Stream CreateStream(Resource resource)
    {
        return new FileStream(ExistingFilePath(resource), FileMode.Open);
    }

    public bool CanFind(Resource resource)
    {
        return ExistingFilePath(resource) != null;
    }

    private string? ExistingFilePath(Resource resource)
    {
        var filePath = Path.Combine(baseDirectory, resource.Path);

        if (File.Exists(filePath))
            return filePath;

        filePath = Path.Combine(baseDirectory, resource.ReducedPath);

        return File.Exists(filePath) ? filePath : null;
    }
}
