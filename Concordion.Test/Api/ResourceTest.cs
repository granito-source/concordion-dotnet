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

using System.Diagnostics.CodeAnalysis;
using Concordion.Api;

namespace Concordion.Test.Api;

[TestFixture]
public class ResourceTest {
    [Test]
    public void RequiresAbsolutePath()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<ArgumentException>(() => Resource(""));
            Assert.Throws<ArgumentException>(() => Resource("one"));
            Assert.Throws<ArgumentException>(() => Resource("one/two"));
        }
    }

    [Test]
    public void CanTellItsOwnPath()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.That(Resource("/").Path, Is.EqualTo("/"));
            Assert.That(Resource("/dev/null").Path, Is.EqualTo("/dev/null"));
            Assert.That(Resource("/var/run/").Path, Is.EqualTo("/var/run/"));
        }
    }

    [Test]
    public void CanTellItsOwnName()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.That(Resource("/").Name, Is.EqualTo(""));
            Assert.That(Resource("/dev/null").Name, Is.EqualTo("null"));
            Assert.That(Resource("/var/run/").Name, Is.EqualTo("run"));
        }
    }

    [Test]
    public void CanTellItsOwnReducedPathWhenAssemblyIsSet()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.That(Resource("/", "Fixtures").ReducedPath,
                Is.EqualTo("/"));
            Assert.That(Resource("/Fixtures/", "Fixtures").ReducedPath,
                Is.EqualTo("/"));
            Assert.That(
                Resource("/Fixtures/file.txt", "Fixtures").ReducedPath,
                Is.EqualTo("/file.txt"));
            Assert.That(
                Resource("/path/Fixtures/", "Fixtures").ReducedPath,
                Is.EqualTo("/path/"));
            Assert.That(
                Resource("/path/Fixtures/file.txt", "Fixtures")
                    .ReducedPath,
                Is.EqualTo("/path/file.txt"));
            Assert.That(
                Resource("/path/Test/Fixtures/", "Test.Fixtures")
                    .ReducedPath,
                Is.EqualTo("/path/"));
            Assert.That(
                Resource("/path/Test/Fixtures/file.txt", "Test.Fixtures")
                    .ReducedPath,
                Is.EqualTo("/path/file.txt"));
        }
    }

    [Test]
    public void CanTellItsParent()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.That(ParentPathOf("/"), Is.Null);
            Assert.That(ParentPathOf("/abc"), Is.EqualTo("/"));
            Assert.That(ParentPathOf("/abc/def"), Is.EqualTo("/abc/"));
            Assert.That(ParentPathOf("/abc/def/"), Is.EqualTo("/abc/"));
            Assert.That(ParentPathOf("/abc/def/ghi"), Is.EqualTo("/abc/def/"));
        }
    }

    [Test]
    public void CanCalculateRelativePath()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.That(RelativePath("/", "/spec/x.html"),
                Is.EqualTo("spec/x.html"));
            Assert.That(RelativePath("/spec/x.html", "/spec/y.html"),
                Is.EqualTo("y.html"));
            Assert.That(RelativePath("/spec/", "/spec/blah"),
                Is.EqualTo("blah"));
            Assert.That(RelativePath("/a/b/c/", "/a/b/x/"),
                Is.EqualTo("../x/"));
            Assert.That(RelativePath("/x/b/c/", "/a/b/x/"),
                Is.EqualTo("../../../a/b/x/"));
            Assert.That(RelativePath("/a/b/c/file.txt", "/a/x/x/file.txt"),
                Is.EqualTo("../../x/x/file.txt"));
            Assert.That(RelativePath("/a/file.txt", "/file.txt"),
                Is.EqualTo("../file.txt"));
            Assert.That(RelativePath("/a/b/c/file.txt", "/file.txt"),
                Is.EqualTo("../../../file.txt"));
            Assert.That(
                RelativePath("/spec/concordion/breadcrumbs/Breadcrumbs.html",
                    "/image/concordion-logo.png"),
                Is.EqualTo("../../../image/concordion-logo.png"));
        }
    }

    [Test]
    public void CanCreateRelativeResource()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.That(RelativeResource("/blah.html", "david.html"),
                Is.EqualTo("/david.html"));
            Assert.That(RelativeResource("/", "david.html"),
                Is.EqualTo("/david.html"));
            Assert.That(RelativeResource("/blah/x", "david.html"),
                Is.EqualTo("/blah/david.html"));
            Assert.That(RelativeResource("/blah/x/y", "david.html"),
                Is.EqualTo("/blah/x/david.html"));
            Assert.That(RelativeResource("/blah/x/y", "z/david.html"),
                Is.EqualTo("/blah/x/z/david.html"));
            Assert.That(
                RelativeResource("/blah/docs/example.html", "../style.css"),
                Is.EqualTo("/blah/style.css"));
            Assert.That(
                RelativeResource("/blah/docs/example.html", "../../style.css"),
                Is.EqualTo("/style.css"));
            Assert.That(
                RelativeResource("/blah/docs/work/example.html",
                    "../../style.css"),
                Is.EqualTo("/blah/style.css"));
            Assert.That(
                RelativeResource("/blah/docs/work/example.html",
                    "../style.css"),
                Is.EqualTo("/blah/docs/style.css"));
            Assert.That(
                RelativeResource("/blah/example.html", "../style.css"),
                Is.EqualTo("/style.css"));
            Assert.That(RelativeResource("/blah/", "../style.css"),
                Is.EqualTo("/style.css"));
            Assert.That(RelativeResource("/blah", "style.css"),
                Is.EqualTo("/style.css"));
            Assert.That(
                RelativeResource("/blah/docs/work/", "../css/style.css"),
                Is.EqualTo("/blah/docs/css/style.css"));
        }
    }

    [Test]
    public void TrimsParentDirectoriesAboveRoot()
    {
        Assert.That(
            RelativeResource("/blah/docs/example.html", "../../../style.css"),
            Is.EqualTo("/style.css"));
    }

    [Test]
    public void ReturnsHashCodeBasedOnPath()
    {
        var resource = Resource("/file.txt");

        using (Assert.EnterMultipleScope()) {
            Assert.That(resource.GetHashCode(),
                Is.EqualTo(Resource("/file.txt").GetHashCode()));
            Assert.That(resource.GetHashCode(),
                Is.Not.EqualTo("/file.png".GetHashCode()));
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public void FulfilsEqualsContractBasedOnPath()
    {
        const string path = "/some/path/file.txt";
        var resource = Resource(path);

        using (Assert.EnterMultipleScope()) {
            Assert.That(resource.Equals(resource), Is.True);
            Assert.That(resource.Equals(path), Is.False);
            Assert.That(resource.Equals(Resource(path)), Is.True);
            Assert.That(resource.Equals(Resource(path, "Fixture")), Is.True);
            Assert.That(resource.Equals(Resource("/different")), Is.False);
            Assert.That(resource.Equals(null), Is.False);
        }
    }

    private string RelativeResource(string resourcePath, string relativePath)
    {
        return Resource(resourcePath).GetRelativeResource(relativePath).Path;
    }

    private string RelativePath(string from, string to)
    {
        return Resource(from).GetRelativePath(Resource(to));
    }

    private string? ParentPathOf(string path)
    {
        return Resource(path).Parent?.Path;
    }

    private Resource Resource(string path)
    {
        return new Resource(path);
    }

    private Resource Resource(string path, string assemblyName)
    {
        return new Resource(path, assemblyName);
    }
}
