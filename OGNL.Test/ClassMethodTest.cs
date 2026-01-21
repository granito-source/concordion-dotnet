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

using OGNL.Test.Objects;
using OGNL.Test.Util;

namespace OGNL.Test;

public class ClassMethodTest : OgnlTestCase {
    private static readonly CorrectedObject Corrected = new();

    private static readonly object[][] Tests = [
        // Methods on Class
        [Corrected, "GetType().Name", Corrected.GetType().Name],
        [Corrected, "GetType().GetInterfaces()", Corrected.GetType().GetInterfaces()],
        [Corrected, "GetType().GetInterfaces().length", Corrected.GetType().GetInterfaces().Length],
        [
            null, "@System.AppDomain@CurrentDomain.Type.GetInterfaces()",
            AppDomain.CurrentDomain.GetType().GetInterfaces()
        ],
        [null, "@System.AppDomain@CurrentDomain.Type.Type.Name", AppDomain.CurrentDomain.GetType().GetType().Name]
    ];

    public override TestSuite suite()
    {
        var result = new TestSuite();

        for (var i = 0; i < Tests.Length; i++) {
            result.addTest(new ClassMethodTest((string)Tests[i][1], Tests[i][0], (string)Tests[i][1], Tests[i][2]));
        }

        return result;
    }

    public ClassMethodTest()
    {
    }

    public ClassMethodTest(string name) : base(name)
    {
    }

    public ClassMethodTest(string name, object root, string expressionString, object expectedResult, object setValue,
        object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
    }

    public ClassMethodTest(string name, object root, string expressionString, object expectedResult, object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {
    }

    public ClassMethodTest(string name, object root, string expressionString, object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {
    }

    [Test]
    public void Test4()
    {
        suite()[4].RunTest();
    }
}
