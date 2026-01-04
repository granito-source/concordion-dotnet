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

namespace Concordion.Test;

[TestFixture]
public class StringExtensionsTest {
    [Test]
    public void Test_Can_Remove_First_Instance_In_Middle_Of_String_Successfully()
    {
        Assert.That("ShouldRemoveThis".RemoveFirst("Remove"),
            Is.EqualTo("ShouldThis"));
    }

    [Test]
    public void Test_Can_Remove_First_Instance_In_Middle_Of_String_If_Multiple_Instances_Present_Successfully()
    {
        Assert.That("ShouldRemoveRemoveThis".RemoveFirst("Remove"),
            Is.EqualTo("ShouldRemoveThis"));
    }

    [Test]
    public void Test_Can_Remove_First_Instance_At_Start_Of_String_Successfully()
    {
        Assert.That("RemoveThis".RemoveFirst("Remove"), Is.EqualTo("This"));
    }

    [Test]
    public void Test_Can_Remove_First_Instance_At_End_Of_String_Successfully()
    {
        Assert.That("ShouldRemove".RemoveFirst("Remove"),
            Is.EqualTo("Should"));
    }

    [Test]
    public void Test_Can_Return_Same_String_If_SubString_Not_Found_Successfully()
    {
        Assert.That("This".RemoveFirst("Remove"), Is.EqualTo("This"));
    }

    [Test]
    public void Test_Can_Return_Empty_String_Successfully()
    {
        Assert.That(string.Empty.RemoveFirst("Remove"),
            Is.EqualTo(string.Empty));
    }
}
