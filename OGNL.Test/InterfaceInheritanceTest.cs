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

public class InterfaceInheritanceTest : OgnlTestCase
{
    private static Root             ROOT = new();

    private static object[][]       TESTS = [
        // Interface inheritence test
        [ROOT, "MyMap", ROOT.getMyMap()],
        [ROOT, "MyMap.test", ROOT],
        [ROOT.getMyMap(), "list", ROOT.getList()],
        [ROOT, "MyMap.array[0]", (ROOT.getArray()[0])],
        [ROOT, "MyMap.list[1]", ROOT.getList() [(1)]],
        [ROOT, "MyMap[^]", (99)],
        [ROOT, "MyMap[$]", null],
        [ROOT.getMyMap(), "array[$]", (ROOT.getArray()[ROOT.getArray().Length-1])],
        [ROOT, "[\"MyMap\"]", ROOT.getMyMap()],
        [ROOT, "MyMap[null]", null],
        [ROOT, "MyMap[#x = null]", null],
        [ROOT, "MyMap.(null,test)", ROOT],
        /* // Key null is Not allowed in .Net.
        new object [] { ROOT, "MyMap[null] = 25", (25) },
        new object [] { ROOT, "MyMap[null]", (25), (50), (50) },*/
        [ROOT, "MyMap[0] = 25", (25)],
        [ROOT, "MyMap[0]", (25), (50), (50)]
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
                result.addTest(new InterfaceInheritanceTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2]));
            } 
            else 
            {
                if (TESTS[i].Length == 4) 
                {
                    result.addTest(new InterfaceInheritanceTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2], TESTS[i][3]));
                } 
                else 
                {
                    if (TESTS[i].Length == 5) 
                    {
                        result.addTest(new InterfaceInheritanceTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2], TESTS[i][3], TESTS[i][4]));
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
    public InterfaceInheritanceTest()
    {
	    
    }

    public InterfaceInheritanceTest(string name) : base(name)
    {
	    
    }

    public InterfaceInheritanceTest(string name, object root, string expressionString, object expectedResult, object setValue, object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
        
    }

    public InterfaceInheritanceTest(string name, object root, string expressionString, object expectedResult, object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {
        
    }

    public InterfaceInheritanceTest(string name, object root, string expressionString, object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {
        
    }
}