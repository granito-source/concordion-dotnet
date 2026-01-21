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

public class SetterWithConversionTest : OgnlTestCase {
    private static readonly Root Root = new();

    private static readonly object?[][] Tests = [
        // Property set with conversion
        [Root, "IntValue", 0, 6.5, 6],

        // C# use ODD round, so value is 1026
        [Root, "IntValue", 6, 1025.87645, 1026],
        [Root, "IntValue", 1026, "654", 654],
        [Root, "StringValue", null, 25, "25"],
        [Root, "StringValue", "25", 100.25f, "100.25"],
        [Root, "anotherStringValue", "foo", 0, "0"],
        [Root, "anotherStringValue", "0", 0.5, "0.5"],
        [Root, "anotherIntValue", 123, "5", 5],
        [Root, "anotherIntValue", 5, 100.25, 100]
    ];

    /*===================================================================
        Public static methods
      ===================================================================*/
    public override TestSuite suite()
    {
        var result = new TestSuite();

        for (var i = 0; i < Tests.Length; i++) {
            if (Tests[i].Length == 3) {
                result.addTest(new SetterWithConversionTest((string)Tests[i][1], Tests[i][0], (string)Tests[i][1],
                    Tests[i][2]));
            } else {
                if (Tests[i].Length == 4) {
                    result.addTest(new SetterWithConversionTest((string)Tests[i][1], Tests[i][0], (string)Tests[i][1],
                        Tests[i][2], Tests[i][3]));
                } else {
                    if (Tests[i].Length == 5) {
                        result.addTest(new SetterWithConversionTest((string)Tests[i][1], Tests[i][0],
                            (string)Tests[i][1], Tests[i][2], Tests[i][3], Tests[i][4]));
                    } else {
                        throw new Exception("don't understand TEST format");
                    }
                }
            }
        }

        return result;
    }

    /*===================================================================
        Constructors
      ===================================================================*/
    public SetterWithConversionTest()
    {
    }

    public SetterWithConversionTest(string name) : base(name)
    {
    }

    public SetterWithConversionTest(string name, object root,
        string expressionString, object expectedResult, object setValue,
        object expectedAfterSetResult) : base(name, root,
        expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
    }

    public SetterWithConversionTest(string name, object root,
        string expressionString, object expectedResult, object setValue) :
        base(name, root, expressionString, expectedResult, setValue)
    {
    }

    public SetterWithConversionTest(string name, object root,
        string expressionString, object expectedResult) : base(name, root,
        expressionString, expectedResult)
    {
    }
}
