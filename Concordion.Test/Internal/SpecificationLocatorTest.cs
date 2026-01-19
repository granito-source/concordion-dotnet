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

using Concordion.Internal;

namespace Concordion.Test.Internal;

[TestFixture]
public class SpecificationLocatorTest {
    private readonly ClassNameBasedSpecificationLocator locator = new();

    [Test]
    public void RemovesTestSuffix()
    {
        var resource = locator
            .LocateSpecification(new DummyWithTestInNameTest());

        Assert.That(resource.Path,
            Is.EqualTo("/Concordion/Test/Internal/DummyWithTestInName.html"));
    }

    [Test]
    public void RemovesFixtureSuffix()
    {
        var resource = locator
            .LocateSpecification(new DummyWithFixtureInNameFixture());

        Assert.That(resource.Path,
            Is.EqualTo("/Concordion/Test/Internal/DummyWithFixtureInName.html"));
    }
}

public class DummyWithFixtureInNameFixture;

public class DummyWithTestInNameTest;
