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

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class CollectionPropertyTest : OgnlFixture {
    private static readonly object?[][] Tests = [
        ["emptyList.isEmpty", true],
        ["list.isEmpty", false],
        ["emptyList.size", 0],
        ["list.size", 2],
        ["list.iterator.hasNext", true],
        ["list.iterator.next", "one"],
        ["#it = list.iterator, #it.next, #it.next, #it.hasNext", false],
        ["#it = list.iterator, #it.next, #it.next", "two"],
        ["dictionary['answer']", 42],
        ["dictionary.isEmpty", false],
        ["emptyDictionary.isEmpty", true],
        ["emptyDictionary['isEmpty']", null],
        ["dictionary.size", 3],
        ["dictionary['size']", 42],
        ["dictionary.keys", (string[])["answer", "temperature", "size"]],
        ["dictionary.values", (int[])[42, 451, 42]]
    ];

    public Dictionary<string, int> emptyDictionary = new();

    public Dictionary<string, int> dictionary = new()
    {
        ["answer"] = 42,
        ["temperature"] = 451,
        ["size"] = 42
    };

    public List<string> emptyList = [];

    public List<string> list = ["one", "two"];

    [Test, TestCaseSource(nameof(Tests))]
    public void Evaluates(string expression, object? expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }
}
