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

namespace OGNL.Test;

[TestFixture]
public class ObjectIndexedPropertyTest : OgnlFixture {
    private readonly Hashtable attributes = new();

    [SetUp]
    public void SetUp()
    {
        this["foo"] = "bar";
        this["bar"] = "baz";

        var sub = new ObjectIndexedPropertyTest {
            ["bar"] = "baz"
        };

        this["other"] = sub;
    }

    public object? this[object name] {
        get => attributes[name];

        set => attributes[name] = value;
    }

    private static readonly object[][] GetTests = [
        ["Item['foo']", "bar"],
        ["['foo']", "bar"],
        ["Item['bar']", "baz"],
        ["['bar']", "baz"],
        ["Item[Item['foo']]", "baz"],
        ["[['foo']]", "baz"],
        ["Item['other'].Item['bar']", "baz"],
        ["['other']['bar']", "baz"]
    ];

    private static readonly object[][] SetTests = [
        ["Item['foo']", "zap"],
        ["['bar']", "zap"],
        ["Item['new']", "zap"],
        ["['nuevo']", "zap"],
        ["Item[Item['foo']]", "zap"],
        ["[['foo']]", "zap"],
        // ["Item['other'].Item['bar']", "zap"], XXX: this fails, investigate
        ["['other']['bar']", "zap"]
    ];

    [Test, TestCaseSource(nameof(GetTests))]
    public void Evaluates(string expression, object? expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    [Test, TestCaseSource(nameof(SetTests))]
    public void Mutates(string expression, object value)
    {
        Set(expression, value);

        Assert.That(Get(expression), Is.EqualTo(value));
    }

    [Test]
    public void DoesNotSupportDynamicIndexes()
    {
        using (Assert.EnterMultipleScope()) {
            Assert.Throws<OgnlException>(() => Get("Item[^]"));
            // Assert.Throws<OgnlException>(() => Get("[^]")); XXX: this behaves differently!
            Assert.Throws<OgnlException>(() => Get("Item[|]"));
            // Assert.Throws<OgnlException>(() => Get("[|]")); XXX: this behaves differently!
            Assert.Throws<OgnlException>(() => Get("Item[$]"));
            // Assert.Throws<OgnlException>(() => Get("[$]")); XXX: this behaves differently!
        }
    }
}
