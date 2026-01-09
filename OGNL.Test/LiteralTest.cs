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

public class LiteralTest : OgnlFixture {
    private static readonly object?[][] Tests = [
        ["12345", 12345],
        ["0xfE", 254],
        ["01000", 512],
        ["1234L", 1234L],
        ["12.34", 12.34d],
        [".1234", .1234d],
        ["12.34f", 12.34f],
        ["12.", 12d],
        ["12e+1d", 120d],
        ["'x'", 'x'],
        ["'\\n'", '\n'],
        ["'\\u048c'", '\u048c'],
        ["'\\47'", '\x27'],
        ["'\\367'", '\xF7'],
        ["\"hello world\"", "hello world"],
        ["\"\\u00a0\\u0068ell\\'o\\\\\\n\\r\\f\\t\\b\\\"\\167orld\\\"\"",
            "\u00a0hell'o\\\n\r\f\t\b\"world\""],
        ["null", null],
        ["true", true],
        ["false", false],
        ["{ false, true, null, 0, 1. }",
            new object?[] { false, true, null, 0, 1d }],
        ["'HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"'",
            "HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\""]
    ];

    [Test, TestCaseSource(nameof(Tests))]
    public void CreatesValueFromLiteral(string expression, object? expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    [Test]
    public void ThrowsExceptionWhenStringIsNotClosed()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<ExpressionSyntaxException>(() => Get("'unclosed"));
            Assert.Throws<ExpressionSyntaxException>(() => Get("\"unclosed"));
        }
    }

    [Test]
    public void ThrowsExceptionWhenUnknownEscapeSequence()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<ExpressionSyntaxException>(() => Get("'hello\\x'"));
            Assert.Throws<ExpressionSyntaxException>(() => Get("\"hello\\x\""));
        }
    }
}
