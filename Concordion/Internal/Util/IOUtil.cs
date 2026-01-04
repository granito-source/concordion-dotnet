namespace Concordion.Internal.Util;

public class IOUtil
{
    private const int BufferSize = 4096;

    public static void Copy(TextReader inputReader, TextWriter outputWriter)
    {
        var buffer = new char[BufferSize];
        int len;

        while ((len = inputReader.Read(buffer, 0, BufferSize)) != -1)
        {
            outputWriter.Write(buffer, 0, len);
        }
    }
}
