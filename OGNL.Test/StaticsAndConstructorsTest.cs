using OGNL.Test.Objects;
using OGNL.Test.Util;

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
namespace OGNL.Test;

public class StaticsAndConstructorsTest : OgnlTestCase {
    private static readonly Root Root = new();

    private static readonly object[][] Tests = [
        ["@System.Type@GetType(\"object\")", typeof(object)],
        ["@System.Int32@MaxValue", int.MaxValue],
        ["@@Max(3,4)", 4],
        ["new System.Text.StringBuilder().Append(55).ToString()", "55"],
        ["Type", Root.GetType()],
        [$"@{typeof(Root).FullName}@class.Type", Root.GetType().GetType()],
        ["Type.GetType()", Root.GetType().GetType()],
        [$"@{typeof(Root).FullName}@class.GetType()", Root.GetType().GetType()],
        [$"@{typeof(Root).FullName}@class.Name", Root.GetType().Name],
        ["Type.GetElementType()", Root.GetType().GetElementType()],
        ["Type.ElementType", Root.GetType().GetElementType()],
        ["Type.Type", Root.GetType().GetType()],
        ["getStaticInt()", Root.getStaticInt()],
        [$"@{typeof(Root).FullName}@getStaticInt()", Root.getStaticInt()]
    ];

    public override TestSuite suite()
    {
        var result = new TestSuite();

        for (var i = 0; i < Tests.Length; i++) {
            result.addTest(new StaticsAndConstructorsTest((string)Tests[i][0] + " (" + Tests[i][1] + ")", Root,
                (string)Tests[i][0], Tests[i][1]));
        }

        return result;
    }

    public StaticsAndConstructorsTest()
    {
    }

    public StaticsAndConstructorsTest(string name) : base(name)
    {
    }

    public StaticsAndConstructorsTest(string name, object root, string expressionString, object expectedResult,
        object setValue, object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
    }

    public StaticsAndConstructorsTest(string name, object root, string expressionString, object expectedResult,
        object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {
    }

    public StaticsAndConstructorsTest(string name, object root, string expressionString, object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {
    }
}
