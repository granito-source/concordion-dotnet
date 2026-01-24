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
[SuppressMessage("Structure", "NUnit1028:The non-test method is public")]
[SuppressMessage("Performance", "CA1822")]
public class MethodWithConversionTest : OgnlFixture {
    private static readonly object[][] Tests = [
        ["Int('300')", 300],
        ["Int(300L)", 300],
        ["Int(300.5f)", 300],
        ["Int(300.5d)", 300],
        ["Long('300')", 300],
        ["Long(300)", 300],
        ["Long(300.5f)", 300],
        ["Long(300.5d)", 300],
        ["Float('300.5')", 300.5f],
        ["Float(300)", 300f],
        ["Float(300L)", 300f],
        ["Float(300.5d)", 300.5f],
        ["Double('300.5')", 300.5d],
        ["Double(300)", 300d],
        ["Double(300L)", 300d],
        ["Double(300.5f)", 300.5d],
        ["String(300)", "300"],
        ["String(300L)", "300"],
        ["String(300.5f)", "300.5"],
        ["String(300.5d)", "300.5"],
    ];

    public int Int(int x)
    {
        return x;
    }

    public long Long(long x)
    {
        return x;
    }

    public float Float(float x)
    {
        return x;
    }

    public double Double(double x)
    {
        return x;
    }

    public string String(string x)
    {
        return x;
    }

    [Test, TestCaseSource(nameof(Tests))]
    public void Evaluates(string expression, object? expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    [Test]
    public void ThrowsExceptionWhenConversionIsNotPossible()
    {
        Assert.Throws<MethodFailedException>(() => Get("Int('300.5')"));
    }
}
