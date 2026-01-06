using System.Collections;
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

public class SetterTest : OgnlTestCase
{
    private static Root             ROOT = new();

    private static object[][]       TESTS = [
        // Setting values
        [ROOT.getMap(), "newValue", null, 101],
        [ROOT, "SettableList[0]", "foo", "quux"],     /* absolute indexes */
        [ROOT, "SettableList[0]", "quux"],
        [ROOT, "SettableList[2]", "baz", "quux"],
        [ROOT, "SettableList[2]", "quux"],
        [ROOT, "SettableList[$]", "quux", "oompa"],   /* special indexes */
        [ROOT, "SettableList[$]", "oompa"],
        [ROOT, "SettableList[^]", "quux", "oompa"],
        [ROOT, "SettableList[^]", "oompa"],
        [ROOT, "SettableList[|]", "bar", "oompa"],
        [ROOT, "SettableList[|]", "oompa"],
        [ROOT, "Map.newValue", 101, 555],
        [ROOT, "Map", ROOT.getMap(), new Hashtable (), typeof (NoSuchPropertyException)],
        [ROOT.getMap(), "newValue2 || Add(\"newValue2\",987), newValue2", 987, 1002],
        [ROOT, "Map.(someMissingKey || newValue)", 555, 666],
        [ROOT.getMap(), "newValue || someMissingKey", 666, 666], // no setting happens!
        [ROOT, "Map.(newValue && aKey)", null, 54321],
        [ROOT, "Map.(someMissingKey && newValue)", null, null], // again, no setting
        [null, "0", 0, null, typeof (InappropriateExpressionException)], // illegal for setting, no property
        [ROOT, "Map[0]=\"Map.newValue\", Map[0](#this)", 666, 888]
    ];

    /*===================================================================
        Public static methods
      ===================================================================*/
    public override TestSuite suite()
    {
        var       result = new TestSuite();

        for (var i = 0; i < TESTS.Length; i++)
        {
            if (TESTS[i].Length == 3)
            {
                result.addTest(new SetterTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2]));
            }
            else
            {
                if (TESTS[i].Length == 4)
                {
                    result.addTest(new SetterTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2], TESTS[i][3]));
                }
                else
                {
                    if (TESTS[i].Length == 5)
                    {
                        result.addTest(new SetterTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2], TESTS[i][3], TESTS[i][4]));
                    }
                    else
                    {
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
    public SetterTest()
    {

    }

    public SetterTest(string name) : base(name)
    {

    }

    public SetterTest(string name, object root, string expressionString, object expectedResult, object setValue, object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {

    }

    public SetterTest(string name, object root, string expressionString, object expectedResult, object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {

    }

    public SetterTest(string name, object root, string expressionString, object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {

    }
}
