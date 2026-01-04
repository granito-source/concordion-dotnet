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

namespace Concordion.Test.Api;

[TestFixture]
public class ResourceTest {
    [Test]
    public void Test_If_Resource_Ends_Without_Slash_Can_Tell_You_Its_Parent_Successfully()
    {
        Assert.That(new Resource(@"\abc").Parent?.Path, Is.EqualTo(@"\"));
    }

    [Test]
    public void Test_If_Resource_Ends_With_Slash_Can_Tell_You_Its_Parent_Successfully()
    {
        Assert.That(new Resource(@"\abc\").Parent?.Path, Is.EqualTo(@"\"));
    }

    [Test]
    public void Test_If_Nested_Resource_Ends_Without_Slash_Can_Tell_You_Its_Parent_Successfully()
    {
        Assert.That(new Resource(@"\abc\def").Parent?.Path,
            Is.EqualTo(@"\abc\"));
    }

    [Test]
    public void Test_If_Nested_Resource_Ends_With_Slash_Can_Tell_You_Its_Parent_Successfully()
    {
        Assert.That(new Resource(@"\abc\def\").Parent?.Path,
            Is.EqualTo(@"\abc\"));
    }

    [Test]
    public void Test_If_Triple_Nested_Resource_Ends_Without_Slash_Can_Tell_You_Its_Parent_Successfully()
    {
        Assert.That(new Resource(@"\abc\def\ghi").Parent?.Path,
            Is.EqualTo(@"\abc\def\"));
    }

    [Test]
    public void Test_If_Triple_Nested_Resource_Ends_With_Slash_Can_Tell_You_Its_Parent_Successfully()
    {
        Assert.That(new Resource(@"\abc\def\ghi\").Parent?.Path,
            Is.EqualTo(@"\abc\def\"));
    }

    [Test]
    [Ignore("failing on Linux, needs investigation")]
    public void Test_If_Parent_Of_Root_Is_Null()
    {
        Assert.That(new Resource(@"\").Parent, Is.Null);
    }

    [Test]
    public void Test_If_Paths_Point_To_File_And_Are_Identical_Can_Calculate_Relative_Path()
    {
        var from = new Resource(@"\spec\x.html");
        var to = new Resource(@"\spec\x.html");

        Assert.That(from.GetRelativePath(to), Is.EqualTo("x.html"));
    }

    [Test]
    public void Test_If_Paths_Are_Not_Identical_Can_Calculate_Relative_Path()
    {
        var from = new Resource(@"\spec\");
        var to = new Resource(@"\spec\blah");

        Assert.That(from.GetRelativePath(to), Is.EqualTo(@"blah"));
    }

    [Test]
    public void Test_If_Paths_Are_Not_Identical_And_End_In_Slashes_Can_Calculate_Relative_Path()
    {
        var from = new Resource(@"\a\b\c\");
        var to = new Resource(@"\a\b\x\");

        Assert.That(from.GetRelativePath(to), Is.EqualTo(@"..\x\"));
    }

    [Test]
    public void Test_If_Paths_Are_Weird_And_End_In_Slashes_Can_Calculate_Relative_Path()
    {
        var from = new Resource(@"\x\b\c\");
        var to = new Resource(@"\a\b\x\");

        Assert.That(from.GetRelativePath(to), Is.EqualTo(@"..\..\..\a\b\x\"));
    }

    [Test]
    public void Test_If_Paths_Share_Common_Root_And_End_In_Text_File_Can_Calculate_Relative_Path()
    {
        var from = new Resource(@"\a\b\c\file.txt");
        var to = new Resource(@"\a\x\x\file.txt");

        Assert.That(from.GetRelativePath(to), Is.EqualTo(@"..\..\x\x\file.txt"));
    }

    [Test]
    public void Test_If_Path_To_Image_And_Path_To_Html_File_Can_Calculate_Relative_Path()
    {
        var from = new Resource(@"\spec\concordion\breadcrumbs\Breadcrumbs.html");
        var to = new Resource(@"\image\concordion-logo.png");

        Assert.That(from.GetRelativePath(to),
            Is.EqualTo(@"..\..\..\image\concordion-logo.png"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_From_Another_Resource_File_Successfully()
    {
        var resourcePath = @"\blah.html";
        var relativePath = @"david.html";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\david.html"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Root_Path_From_Another_Resource_File_Successfully()
    {
        var resourcePath = @"\";
        var relativePath = @"david.html";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\david.html"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Directory_From_Another_Resource_File_Successfully()
    {
        var resourcePath = @"\blah\x";
        var relativePath = @"david.html";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\blah\david.html"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_From_Another_Resource_File_Successfully()
    {
        var resourcePath = @"\blah\x\y";
        var relativePath = @"david.html";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\blah\x\david.html"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_From_Another_Resource_File_In_A_Directory_Successfully()
    {
        var resourcePath = @"\blah\x\y";
        var relativePath = @"z\david.html";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\blah\x\z\david.html"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_From_Another_Resource_File_In_A_SubDirectory_Successfully()
    {
        var resourcePath = @"\blah\docs\example.html";
        var relativePath = @"..\style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\blah\style.css"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_And_File_From_Another_Resource_File_In_A_Directory_Successfully()
    {
        var resourcePath = @"\blah\docs\example.html";
        var relativePath = @"..\..\style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\style.css"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_And_File_From_Another_Resource_File_In_A_Directory2_Successfully()
    {
        var resourcePath = @"\blah\docs\work\example.html";
        var relativePath = @"..\..\style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\blah\style.css"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_And_File_From_Another_Resource_File_In_A_Directory3_Successfully()
    {
        var resourcePath = @"\blah\docs\work\example.html";
        var relativePath = @"..\style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\blah\docs\style.css"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_And_File_From_Another_Resource_File_In_A_Directory4_Successfully()
    {
        var resourcePath = @"\blah\example.html";
        var relativePath = @"..\style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\style.css"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_And_File_From_Another_Resource_File_In_A_Directory5_Successfully()
    {
        var resourcePath = @"\blah\";
        var relativePath = @"..\style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\style.css"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_And_File_From_Another_Resource_File_In_A_Directory6_Successfully()
    {
        var resourcePath = @"\blah";
        var relativePath = @"style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\style.css"));
    }

    [Test]
    public void Test_Can_Get_Relative_Resource_With_Multiple_Directory_And_File_From_Another_Resource_File_In_A_Directory7_Successfully()
    {
        var resourcePath = @"\blah\docs\work\";
        var relativePath = @"..\css\style.css";

        Assert.That(
            new Resource(resourcePath).GetRelativeResource(relativePath).Path,
            Is.EqualTo(@"\blah\docs\css\style.css"));
    }

    [Test]
    [Ignore("failing on Linux, needs investigation")]
    public void Test_Throws_Exception_If_Relative_Path_Points_Above_Root()
    {
        var from = new Resource(@"\spec\concordion\breadcrumbs\Breadcrumbs.html");

        Assert.Throws<Exception>(() =>
            from.GetRelativeResource(@"..\..\..\..\concordion-logo.png"));
    }

    [Test]
    [Ignore("failing on Linux, needs investigation")]
    public void Test_Can_Strip_Drive_Letter_Successfully()
    {
        Assert.That(new Resource(@"C:\blah\").Path, Is.EqualTo(@"\blah\"));
    }
}
