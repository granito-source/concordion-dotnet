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
using System.Reflection;
using OGNL.Test.Objects;
using OGNL.Test.Util;

namespace OGNL.Test;

public class ArrayElementsTest : OgnlTestCase {
    private static readonly string[] StringArray = ["hello", "world"];

    private static readonly int[] IntArray = [10, 20];

    private static readonly Root Root = new();

    private static readonly object[][] Tests = new object[][] {
        // Array elements test
        [StringArray, "length", (2)],
        [StringArray, "#root[1]", "world"],
        [IntArray, "#root[1]", (20)],
        [IntArray, "#root[1]", (20), "50", (50)],
        [IntArray, "#root[1]", (50), new string[] { "50", "100" }, (50)],
        [Root, "IntValue", (0), new string[] { "50", "100" }, (50)],
        [Root, "Array", Root.getArray(), new string[] { "50", "100" }, new int[] { 50, 100 }],
    };

    public override TestSuite suite()
    {
        var result = new TestSuite();

        for (var i = 0; i < Tests.Length; i++) {
            if (Tests[i].Length == 3) {
                result.addTest(
                    new ArrayElementsTest((string)Tests[i][1], Tests[i][0], (string)Tests[i][1], Tests[i][2]));
            } else {
                if (Tests[i].Length == 4) {
                    result.addTest(new ArrayElementsTest((string)Tests[i][1], Tests[i][0], (string)Tests[i][1],
                        Tests[i][2], Tests[i][3]));
                } else {
                    if (Tests[i].Length == 5) {
                        result.addTest(new ArrayElementsTest((string)Tests[i][1], Tests[i][0], (string)Tests[i][1],
                            Tests[i][2], Tests[i][3], Tests[i][4]));
                    } else {
                        throw new Exception("don't understand TEST format");
                    }
                }
            }
        }

        return result;
    }

    public ArrayElementsTest()
    {
    }

    public ArrayElementsTest(string name) : base(name)
    {
    }

    public ArrayElementsTest(string name, object root, string expressionString, object expectedResult, object setValue,
        object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
    }

    public ArrayElementsTest(string name, object root, string expressionString, object expectedResult, object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {
    }

    public ArrayElementsTest(string name, object root, string expressionString, object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {
    }

    [SetUp]
    public override void setUp()
    {
        base.setUp();

        context.setTypeConverter(new ArrayDefaultTypeConverter());
    }

    class ArrayDefaultTypeConverter : DefaultTypeConverter {
        public override object convertValue(IDictionary context, object target, MemberInfo member, string propertyName,
            object value, Type toType)
        {
            if (value.GetType().IsArray) {
                if (!toType.IsArray) {
                    value = ((Array)value).GetValue(0);
                }
            }

            return base.convertValue(context, target, member, propertyName, value, toType);
        }
    }
}
