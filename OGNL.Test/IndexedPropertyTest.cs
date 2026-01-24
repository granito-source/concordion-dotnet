//--------------------------------------------------------------------------
//  Copyright (c) 2004, Drew Davidson ,  Luke Blanshard and Foxcoming
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
[SuppressMessage("Performance", "CA1822")]
public class IndexedPropertyTest : OgnlFixture {
    private readonly string[] values = ["one", "two", "three"];

    public int Index => 1;

    public string this[int index] {
        get => values[index] + "X";

        set => values[index] = value.EndsWith('X') ? value[..^1] : value;
    }

    private static readonly object[][] GetTests = [
        ["[0]", "oneX"],
        ["[Index]", "twoX"],
        ["Item[0]", "oneX"],
        ["Item[Index]", "twoX"],

    ];

    private static readonly object[][] SetTests = [
        ["[0]", "unoX", "unoX"],
        ["[Index]", "dosX", "dosX"],
        ["Item[0]", "unoX", "unoX"],
        ["Item[Index]", "dosX", "dosX"]
    ];

    [Test, TestCaseSource(nameof(GetTests))]
    public void Evaluates(string expression, object? expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    [Test, TestCaseSource(nameof(SetTests))]
    public void Mutates(string expression, object value, object? expected)
    {
        Set(expression, value);

        Assert.That(Get(expression), Is.EqualTo(expected));
    }
}
