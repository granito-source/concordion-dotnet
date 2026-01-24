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
public class SetterWithConversionTest : OgnlFixture {
    public bool BoolValue { get; set; }

    public int IntValue { get; set; }

    public long LongValue { get; set; }

    public string? StringValue { get; set; }

    private static readonly object[][] Tests = [
        ["BoolValue", 6.5, true],
        ["BoolValue", 0, false],
        ["BoolValue", "true", true],
        ["BoolValue", "false", false],
        ["IntValue", 6.5, 6],
        ["IntValue", 1025.87645, 1026], // C# use ODD rounding
        ["IntValue", "654", 654],
        ["LongValue", 6.5, 6L],
        ["LongValue", 1025.87645, 1026L], // C# use ODD rounding
        ["LongValue", "654", 654L],
        ["StringValue", int.MaxValue, "2147483647"],
        ["StringValue", long.MinValue, "-9223372036854775808"],
        ["StringValue", 100.1f, "100.1"],
        ["StringValue", 100.1d, "100.1"]
    ];

    [Test, TestCaseSource(nameof(Tests))]
    public void Mutates(string expression, object value, object expected)
    {
        Set(expression, value);

        Assert.That(Get(expression), Is.EqualTo(expected));
    }
}
