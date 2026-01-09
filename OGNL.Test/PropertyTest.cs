//--------------------------------------------------------------------------
//  Copyright (c) 2004, Drew Davidson and Luke Blanshard
//  Copyright (c) 2026, Alexei Yashkov
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are
//  met:
//
//  Redistributions of source code must retain the above copyright notice,
//  this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright
//  notice, this list of conditions and the following disclaimer in the
//  documentation and/or other materials provided with the distribution.
//  Neither the name of the Drew Davidson nor the names of its contributors
//  may be used to endorse or promote products derived from this software
//  without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
//  FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
//  COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
//  BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
//  OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
//  AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF
//  THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
//  DAMAGE.
//--------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace OGNL.Test;

[TestFixture]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class PropertyTest : OgnlFixture {
    public static readonly string Size = "size";

    private static readonly string[] TestArray = ["zero", "one", "two"];

    private static readonly List<string> TestList = ["zero", "one", "two"];

    private static readonly Dictionary<string, object> TestMap = new() {
        ["test"] = 42,
        ["size"] = "SIZE",
        ["array"] = TestArray
    };

    private static readonly object?[][] Tests = [
        ["Array", TestArray],
        ["[\"Array\"]", TestArray],
        ["Array[0]", "zero"],
        ["Array[^]", "zero"],
        ["Array[|]", "one"],
        ["Array[$]", "two"],
        ["List", TestList],
        ["['List']", TestList],
        ["List[0]", "zero"],
        ["List[^]", "zero"],
        ["List[|]", "one"],
        ["List[$]", "two"],
        ["Map", TestMap],
        ["[\"Map\"]", TestMap],
        ["Map.test", 42],
        ["Map['test']", 42],
        ["Map['te' + 'st']", 42],
        ["Map['size']", "SIZE"],
        ["Map[@OGNL.Test.PropertyTest@Size]", "SIZE"],
        ["Map.size", 3],
        ["Map.array[0]", "zero"],
        ["Map.(array[1] + size)", "one3"],
        ["Map.(#this)", TestMap],
        ["Map.(#this != null ? #this['size'] : null)", "SIZE"],
        ["Map[\"test\"].(#this == null ? 'empty' : #this)", 42],
        ["Map[\"missing\"].(#this == null ? 'empty' : #this)", "empty"]
    ];

    public string[] Array { get; } = TestArray;

    public List<string> List { get; } = TestList;

    public Dictionary<string, object> Map { get; } = TestMap;

    [Test, TestCaseSource(nameof(Tests))]
    public void GetsProperty(string expression, object? expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }
}
