/*
 * Copyright 2026 Alexei Yashkov
 * Copyright 2010-2015 concordion.org
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

using Concordion.Api;
using Concordion.Internal;
using static System.Text.Encoding;

namespace Concordion.Test.Internal;

[TestFixture]
public class FileTargetTest {
    private readonly TestFileTarget target = new("/base/directory");

    [Test]
    public void CanWriteStringContentToFile()
    {
        const string content = "file content";
        var resource = new Resource("path/to/file.txt");

        target.Write(resource, content);

        using (Assert.EnterMultipleScope()) {
            Assert.That(target.CreateDirectoryCalls,
                Is.EquivalentTo(["/base/directory/path/to"]));
            Assert.That(target.Files, Has.Count.EqualTo(1));
        }

        var stream = target.Files["/base/directory/path/to/file.txt"];

        using (Assert.EnterMultipleScope()) {
            Assert.That(stream.IsClosed, Is.True);
            Assert.That(stream.ToArray(), Is.EqualTo(UTF8.GetBytes(content)));
        }
    }

    [Test]
    public void UsesResourcePathToWriteWhenItIsAbsolute()
    {
        const string content = "file content";
        var resource = new Resource("/path/to/file.txt");

        target.Write(resource, content);

        using (Assert.EnterMultipleScope()) {
            Assert.That(target.CreateDirectoryCalls,
                Is.EquivalentTo(["/path/to"]));
            Assert.That(target.Files, Has.Count.EqualTo(1));
        }

        var stream = target.Files["/path/to/file.txt"];

        using (Assert.EnterMultipleScope()) {
            Assert.That(stream.IsClosed, Is.True);
            Assert.That(stream.ToArray(), Is.EqualTo(UTF8.GetBytes(content)));
        }
    }

    [Test]
    public void CanCopyStreamContentToFile()
    {
        var content = "text content"u8.ToArray();
        var resource = new Resource("path/to/file.txt");

        target.CopyTo(resource, new MemoryStream(content));

        using (Assert.EnterMultipleScope()) {
            Assert.That(target.CreateDirectoryCalls,
                Is.EquivalentTo(["/base/directory/path/to"]));
            Assert.That(target.Files, Has.Count.EqualTo(1));
        }

        var stream = target.Files["/base/directory/path/to/file.txt"];

        using (Assert.EnterMultipleScope()) {
            Assert.That(stream.IsClosed, Is.True);
            Assert.That(stream.ToArray(), Is.EqualTo(content));
        }
    }

    [Test]
    public void UsesResourcePathToCopyWhenItIsAbsolute()
    {
        var content = "text content"u8.ToArray();
        var resource = new Resource("/path/to/file.txt");

        target.CopyTo(resource, new MemoryStream(content));

        using (Assert.EnterMultipleScope()) {
            Assert.That(target.CreateDirectoryCalls,
                Is.EquivalentTo(["/path/to"]));
            Assert.That(target.Files, Has.Count.EqualTo(1));
        }

        var stream = target.Files["/path/to/file.txt"];

        using (Assert.EnterMultipleScope()) {
            Assert.That(stream.IsClosed, Is.True);
            Assert.That(stream.ToArray(), Is.EqualTo(content));
        }
    }

    [Test]
    public void SkipsCopyingWhenTargetIsFreshEnough()
    {
        var content = "text content"u8.ToArray();
        var resource = new Resource("path/to/fresh.txt");

        target.CopyTo(resource, new MemoryStream(content));

        using (Assert.EnterMultipleScope()) {
            Assert.That(target.CreateDirectoryCalls, Is.Empty);
            Assert.That(target.Files, Is.Empty);
        }
    }

    [Test]
    public void DoesCopyStreamWhenTargetIsStale()
    {
        var content = "text content"u8.ToArray();
        var resource = new Resource("path/to/stale.txt");

        target.CopyTo(resource, new MemoryStream(content));

        using (Assert.EnterMultipleScope()) {
            Assert.That(target.CreateDirectoryCalls,
                Is.EquivalentTo(["/base/directory/path/to"]));
            Assert.That(target.Files, Has.Count.EqualTo(1));
        }

        var stream = target.Files["/base/directory/path/to/stale.txt"];

        using (Assert.EnterMultipleScope()) {
            Assert.That(stream.IsClosed, Is.True);
            Assert.That(stream.ToArray(), Is.EqualTo(content));
        }
    }

    private class TestStream : MemoryStream {
        public bool IsClosed;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            IsClosed = true;
        }
    }

    private class TestFileTarget(string baseDir) : FileTarget(baseDir) {
        public readonly List<string> CreateDirectoryCalls = [];

        public readonly Dictionary<string, TestStream> Files = new();

        protected override void CreateDirectory(string path)
        {
            CreateDirectoryCalls.Add(path);
        }

        protected override Stream NewFileStream(string file, FileMode mode)
        {
            Assert.That(mode, Is.EqualTo(FileMode.Create));

            var stream = new TestStream();

            Files[file] = stream;

            return stream;
        }

        protected override DateTime GetLastWriteTime(string file)
        {
            if (file.Contains("fresh"))
                return DateTime.Now.AddMilliseconds(-1000);

            return file.Contains("stale") ?
                DateTime.Now.AddMilliseconds(-2001) :
                DateTime.MinValue;
        }
    }
}
