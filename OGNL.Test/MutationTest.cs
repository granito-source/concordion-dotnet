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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace OGNL.Test;

[TestFixture]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class MutationTest : OgnlFixture {
    public readonly IDictionary dictionary = new Hashtable();

    public readonly IList array = new[] { "foo", "bar", "baz" };

    [Test]
    public void ThrowsExceptionWhenNotAProperty()
    {
        Assert.Throws<InappropriateExpressionException>(() => Set("0", 42));
    }

    [Test]
    public void AllowsSettingDictionaryValues()
    {
        Set("dictionary.key", 42);

        Assert.That(Get("dictionary.key"), Is.EqualTo(42));
    }

    [Test]
    public void AllowsSettingDictionaryValuesWithFallbackKey()
    {
        Set("dictionary.key", 42);
        Set("dictionary.(missing || key)", 212);

        Assert.Multiple(() =>
        {
            Assert.That(Get("dictionary.key"), Is.EqualTo(212));
            Assert.That(Get("dictionary.missing"), Is.Null);
        });
    }

    [Test]
    public void DoesNotChangeDictionaryWhenFallbackKeyIsMissing()
    {
        Set("dictionary.key", 42);
        Set("dictionary.(key || missing)", 212);

        Assert.Multiple(() =>
        {
            Assert.That(Get("dictionary.key"), Is.EqualTo(42));
            Assert.That(Get("dictionary.missing"), Is.Null);
        });
    }

    [Test]
    public void AllowsSettingDictionaryValuesConditionally()
    {
        Set("dictionary.key", 42);
        Set("dictionary.(key && conditional)", 212);

        Assert.Multiple(() =>
        {
            Assert.That(Get("dictionary.key"), Is.EqualTo(42));
            Assert.That(Get("dictionary.conditional"), Is.EqualTo(212));
        });
    }

    [Test]
    public void DoesNotSetConditionalPropertyWhenMissingKey()
    {
        Set("dictionary.(missing && conditional)", 42);

        Assert.Multiple(() =>
        {
            Assert.That(Get("dictionary.missing"), Is.Null);
            Assert.That(Get("dictionary.conditional"), Is.Null);
        });
    }

    [Test]
    public void AllowsCallingDictionaryMethodsInKeyExpression()
    {
        Set("dictionary.(key || Add(\"key\", 451), key)", 42);

        Assert.That(Get("dictionary.key"), Is.EqualTo(42));
    }

    [Test]
    public void AllowsUsingThisInDictionaryKeyExpressions()
    {
        Set("dictionary.key", 42);
        Set("dictionary[0]=\"dictionary.key\", dictionary[0](#this)", 212);

        Assert.That(Get("dictionary.key"), Is.EqualTo(212));
    }

    [Test]
    public void ThrowsExceptionWhenNoDictionaryKey()
    {
        Assert.Throws<NoSuchPropertyException>(() => Set("dictionary", 42));
    }

    [Test]
    public void AllowsSettingArrayValuesUsingIntegerIndexes()
    {
        Set("array[0]", "42");
        Set("array[1]", "451");
        Set("array[2]", "1984");

        Assert.Multiple(() =>
        {
            Assert.That(Get("array[0]"), Is.EqualTo("42"));
            Assert.That(Get("array[1]"), Is.EqualTo("451"));
            Assert.That(Get("array[2]"), Is.EqualTo("1984"));
        });
    }

    [Test]
    public void ThrowsExceptionWhenArrayIndexIsNegative()
    {
        Assert.Throws<IndexOutOfRangeException>(() =>
            Set("array[-1]", "negative"));
    }

    [Test]
    public void ThrowsExceptionWhenArrayIndexIsOutOfBounds()
    {
        Assert.Throws<IndexOutOfRangeException>(() =>
            Set("array[3]", "out of bounds"));
    }

    [Test]
    public void AllowsSettingArrayValuesUsingSpecialIndexes()
    {
        Set("array[^]", "42");
        Set("array[|]", "451");
        Set("array[$]", "1984");

        Assert.Multiple(() =>
        {
            Assert.That(Get("array[0]"), Is.EqualTo("42"));
            Assert.That(Get("array[1]"), Is.EqualTo("451"));
            Assert.That(Get("array[2]"), Is.EqualTo("1984"));
        });
    }

    [Test]
    public void AllowsSettingWholeArray()
    {
        Set("array[*]", new[] { "42", "451", "1984" });

        Assert.Multiple(() =>
        {
            Assert.That(Get("array[0]"), Is.EqualTo("42"));
            Assert.That(Get("array[1]"), Is.EqualTo("451"));
            Assert.That(Get("array[2]"), Is.EqualTo("1984"));
        });
    }

    [Test]
    public void AllowsSourceArrayToBeShorter()
    {
        Set("array[*]", new[] { "42", "451" });

        Assert.Multiple(() =>
        {
            Assert.That(Get("array[0]"), Is.EqualTo("42"));
            Assert.That(Get("array[1]"), Is.EqualTo("451"));
            Assert.That(Get("array[2]"), Is.EqualTo("baz"));
        });
    }

    [Test]
    public void AllowsSourceArrayToBeLonger()
    {
        Set("array[*]", new[] { "42", "451", "1984", "90210" });

        Assert.Multiple(() =>
        {
            Assert.That(Get("array[0]"), Is.EqualTo("42"));
            Assert.That(Get("array[1]"), Is.EqualTo("451"));
            Assert.That(Get("array[2]"), Is.EqualTo("1984"));
        });
    }
}
