/*
 * Copyright 2026 Alexei Yashkov
 * Copyright 2010-2015 concordion.org
 * Copyright 2009 Jeffrey Cameron
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Drawing;
using Concordion.Api;

namespace Concordion.Internal;

public class FileTarget(string baseDir) : Target {
    private const long FreshEnoughMillis = 2000; // 2 secs

    private const int BufferSize = 4096;

    private string BaseDir { get; } =
        Path.EndsInDirectorySeparator(baseDir) ? baseDir :
            baseDir + Path.DirectorySeparatorChar;

    public void Write(Resource target, string content)
    {
        MakeDirectories(target);

        using var writer = CreateWriter(target);

        writer.Write(content);
    }

    public void Write(Resource target, Bitmap image)
    {
        MakeDirectories(target);

        // image.Save(Path.Combine(BaseDir, resource.Path), ImageFormat.Png);
    }

    public void CopyTo(Resource target, Stream source)
    {
        var outputFile = GetTargetPath(target);

        // Do not overwrite if a recent copy already exists
        if (IsFreshEnough(outputFile))
            return;

        MakeDirectories(target);

        using var output = NewFileStream(outputFile, FileMode.Create);

        source.CopyTo(output, BufferSize);
    }

    public string ResolvedPathFor(Resource resource)
    {
        return GetTargetPath(resource);
    }

    protected virtual void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    protected virtual DateTime GetLastWriteTime(string file)
    {
        return File.GetLastWriteTime(file);
    }

    protected virtual Stream NewFileStream(string file, FileMode mode)
    {
        return new FileStream(file, mode);
    }

    private string GetTargetPath(Resource resource)
    {
        return Path.Combine(BaseDir, resource.Path);
    }

    private void MakeDirectories(Resource resource)
    {
        var parent = resource.Parent;

        if (parent == null)
            return;

        var dir = Path.TrimEndingDirectorySeparator(GetTargetPath(parent));

        CreateDirectory(dir);
    }

    private StreamWriter CreateWriter(Resource resource)
    {
        var stream = NewFileStream(GetTargetPath(resource), FileMode.Create);

        return new StreamWriter(stream);
    }

    private bool IsFreshEnough(string file)
    {
        var ageInMillis = DateTime.Now.Subtract(GetLastWriteTime(file));

        return ageInMillis.TotalMilliseconds < FreshEnoughMillis;
    }
}
