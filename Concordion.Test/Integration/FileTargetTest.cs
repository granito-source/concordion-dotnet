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
using Moq;

namespace Concordion.Test.Integration;

[TestFixture]
public class FileTargetTest {
    [Test]
    [Ignore("failing on Linux, needs investigation")]
    public void Test_Can_Get_File_Path_Successfully()
    {
        var resource = new Mock<Resource>("blah\\blah.txt");

        resource.Setup(x => x.Path).Returns("blah\\blah.txt");

        var target = new FileTarget(@"c:\temp");

        Assert.That(target.GetTargetPath(resource.Object),
            Is.EqualTo(@"c:\temp\blah\blah.txt"));
    }
}
