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
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class NumberFormatExceptionTest : OgnlFixture {
    public int IntValue { get; set; }

    public long LongValue { get; set; }

    public float FloatValue { get; set; }

    public double DoubleValue { get; set; }

    public decimal DecimalValue { get; set; }

    [Test]
    public void StripsWhitespaceWhenParsingInt()
    {
        Set("IntValue", "\t 42\t\n");

        Assert.That(IntValue, Is.EqualTo(42));
    }

    [Test]
    public void ThrowsExceptionWhenCannotParseInt()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<MethodFailedException>(() =>
                Set("IntValue", ""));
            Assert.Throws<MethodFailedException>(() =>
                Set("IntValue", " \t"));
            Assert.Throws<MethodFailedException>(() =>
                Set("IntValue", "x10x"));
        }
    }

    [Test]
    public void StripsWhitespaceWhenParsingLong()
    {
        Set("LongValue", "\t 42\t\n");

        Assert.That(LongValue, Is.EqualTo(42L));
    }

    [Test]
    public void ThrowsExceptionWhenCannotParseLong()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<MethodFailedException>(() =>
                Set("LongValue", ""));
            Assert.Throws<MethodFailedException>(() =>
                Set("LongValue", " \t"));
            Assert.Throws<MethodFailedException>(() =>
                Set("LongValue", "x10x"));
        }
    }

    [Test]
    public void StripsWhitespaceWhenParsingFloat()
    {
        Set("FloatValue", "\t 42.001\t\n");

        Assert.That(FloatValue, Is.EqualTo(42.001f));
    }

    [Test]
    public void ThrowsExceptionWhenCannotParseFloat()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<MethodFailedException>(() =>
                Set("FloatValue", ""));
            Assert.Throws<MethodFailedException>(() =>
                Set("FloatValue", " \t"));
            Assert.Throws<MethodFailedException>(() =>
                Set("FloatValue", "x10x"));
        }
    }

    [Test]
    public void StripsWhitespaceWhenParsingDouble()
    {
        Set("DoubleValue", "\t 42.001\t\n");

        Assert.That(DoubleValue, Is.EqualTo(42.001d));
    }

    [Test]
    public void ThrowsExceptionWhenCannotParseDouble()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<MethodFailedException>(() =>
                Set("DoubleValue", ""));
            Assert.Throws<MethodFailedException>(() =>
                Set("DoubleValue", " \t"));
            Assert.Throws<MethodFailedException>(() =>
                Set("DoubleValue", "x10x"));
        }
    }

    [Test]
    public void StripsWhitespaceWhenParsingDecimal()
    {
        Set("DecimalValue", "\t 42.001\t\n");

        Assert.That(DecimalValue, Is.EqualTo(42.001m));
    }

    [Test]
    public void ThrowsExceptionWhenCannotParseDecimal()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<MethodFailedException>(() =>
                Set("DecimalValue", ""));
            Assert.Throws<MethodFailedException>(() =>
                Set("DecimalValue", " \t"));
            Assert.Throws<MethodFailedException>(() =>
                Set("DecimalValue", "x10x"));
        }
    }
}
