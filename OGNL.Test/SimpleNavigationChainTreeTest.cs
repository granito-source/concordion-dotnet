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

using OGNL.Test.Util;

namespace OGNL.Test;

public class SimpleNavigationChainTreeTest : OgnlTestCase {
    private static readonly object[][] Tests = new object[][] {
        ["name", true],
        ["name[i]", false],
        ["name + foo", false],
        ["name.foo", true]
    };

    public override TestSuite suite()
    {
        var result = new TestSuite();

        foreach (var t in Tests)
            result.addTest(new SimpleNavigationChainTreeTest(
                (string)t[0] + " (" + t[1] + ")", null, (string)t[0], t[1]));

        return result;
    }

    public SimpleNavigationChainTreeTest()
    {
    }

    public SimpleNavigationChainTreeTest(string name) : base(name)
    {
    }

    public SimpleNavigationChainTreeTest(string name, object root,
        string expressionString, object expectedResult, object setValue,
        object expectedAfterSetResult) : base(name, root,
        expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
    }

    public SimpleNavigationChainTreeTest(string name, object root,
        string expressionString, object expectedResult, object setValue) :
        base(name, root, expressionString, expectedResult, setValue)
    {
    }

    public SimpleNavigationChainTreeTest(string name, object? root,
        string expressionString, object expectedResult) : base(name, root,
        expressionString, expectedResult)
    {
    }

    protected internal override void RunTest()
    {
        Assert.That(Ognl.IsSimpleNavigationChain(GetExpression(), Context) ==
            (bool)GetExpectedResult(), Is.True);
    }
}
