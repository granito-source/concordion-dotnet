//--------------------------------------------------------------------------
//  Copyright (c) 2004, Drew Davidson and Luke Blanshard
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
using System.Reflection;

namespace OGNL.Test;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ArrayElementsTest : OgnlFixture {
    private static readonly string[] StringArray = ["hello", "world"];

    private static readonly int[] IntArray = [10, 20];

    private static readonly object[][] EvaluationTests = [
        [StringArray, "length", 2],
        [StringArray, "[1]", "world"],
        [StringArray, "#root[1]", "world"],
        [StringArray, "#root", new[] { "hello", "world" }],
        [IntArray, "length", 2],
        [IntArray, "[1]", 20],
        [IntArray, "#root[1]", 20],
        [IntArray, "#root", new[] { 10, 20 }]
    ];

    private static readonly object?[][] MutationTests = [
        [StringArray, "[1]", "all", "all"],
        [StringArray, "#root[0]", "hi", "hi"],
        [StringArray, "#root[1]", new[] { "all", "folks" }, "all"],
        [IntArray, "[0]", "42", 42],
        [IntArray, "#root[1]", "50", 50],
        [IntArray, "#root[1]", new[] { "100", "200" }, 100],
        [null, "StringValue", new[] { "one", "two" }, "one"],
        [null, "IntValue", new[] { "100", "200" }, 100]
    ];

    public string StringValue { get; set; } = "zero";

    public int IntValue { get; set; }

    [SetUp]
    public void SetUp()
    {
        context.TypeConverter = new ArrayDefaultTypeConverter();
    }

    [Test, TestCaseSource(nameof(EvaluationTests))]
    public void Evaluates(object root, string expression, object expected)
    {
        WithRoot(root);

        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    [Test, TestCaseSource(nameof(MutationTests))]
    public void Mutates(object? root, string expression, object value,
        object expected)
    {
        if (root != null)
            WithRoot(root);

        Set(expression, value);

        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    private class ArrayDefaultTypeConverter : DefaultTypeConverter {
        public override object? ConvertValue(IDictionary context,
            object target, MemberInfo? member, string? propertyName,
            object? value, Type toType)
        {
            if (value != null && value.GetType().IsArray && !toType.IsArray)
                value = ((Array)value).GetValue(0);

            return base.ConvertValue(context, target, member,
                propertyName, value, toType);
        }
    }
}
