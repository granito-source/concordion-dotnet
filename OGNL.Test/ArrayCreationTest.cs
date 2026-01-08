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

namespace OGNL.Test;

[TestFixture]
public class ArrayCreationTest : OgnlFixture {
    private static readonly object[][] Tests = [
        ["new string[] { \"one\", \"two\" }", new[] { "one", "two" }],
        ["new string[] { 1, 2 }", new[] { "1", "2" }],
        ["new int[] { \"1\", 2, \"3\" }", new[] { 1, 2, 3 }],
        ["new string[4]", new string[4]],
        ["new object[2]", new object[2]],
        ["new Char[2]", new char[2]],
        ["new OGNL.Test.ArrayCreationTest[1]", new ArrayCreationTest[1]],
        ["new decimal[] { new Decimal(42) }", new decimal[] { new(42) }]
    ];

    [Test, TestCaseSource(nameof(Tests))]
    public void CreatesArray(string expression, object expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    [Test]
    public void AllowsUsingContextVariablesInInitializers()
    {
        Assert.That(Get("new object[] { #root, #this }"),
            Is.EqualTo(new object[] { this, this }));
    }

    [Test]
    public void ThrowsExceptionWhenBothSizeAndInitializersArePresent()
    {
        Assert.Throws<ExpressionSyntaxException>(() =>
            Get("new string[2] { \"uno\", \"dos\" }"));
    }
}
