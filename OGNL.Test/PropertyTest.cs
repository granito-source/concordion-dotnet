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

public class PropertyTest : OgnlTestCase
{
    private static Root             ROOT = new();

    private static object[][]       TESTS = [
        [ROOT, "Map", ROOT.getMap()],
        [ROOT, "Map.test", ROOT],
        [ROOT, "Map[\"test\"]", ROOT],
        [ROOT, "Map[\"te\" + \"st\"]", ROOT],
        [ROOT, "Map[(\"s\" + \"i\") + \"ze\"]", ROOT.getMap() [(Root.SIZE_STRING)]],
        [ROOT, "Map[\"size\"]", ROOT.getMap() [(Root.SIZE_STRING)]],
        [ROOT, "Map[@Test.org.ognl.test.objects.Root@SIZE_STRING]", ROOT.getMap()[(Root.SIZE_STRING)]],
        [ROOT.getMap(), "list", ROOT.getList()],
        [ROOT, "Map.array[0]", (ROOT.getArray()[0])],
        [ROOT, "Map.list[1]", ROOT.getList() [1]],
        [ROOT, "Map[^]", (99)],
        [ROOT, "Map[$]", null],
        [ROOT.getMap(), "array[$]", (ROOT.getArray()[ROOT.getArray().Length-1])],
        [ROOT, "[\"Map\"]", ROOT.getMap()],
        [ROOT.getArray(), "length", (ROOT.getArray().Length)],
        [ROOT, "getMap().list[|]", ROOT.getList()[(ROOT.getList().Count/2)]],
        [ROOT, "Map.(array[2] + size).ToString()", (ROOT.getArray()[2] + ROOT.getMap().Count).ToString ()],
        [ROOT, "Map.(#this)", ROOT.getMap()],
        [ROOT, "Map.(#this != null ? #this['size'] : null)", ROOT.getMap() [(Root.SIZE_STRING)]],
        [ROOT, "Map[^].(#this == null ? 'empty' : #this)", (99)],
        [ROOT, "Map[$].(#this == null ? 'empty' : #this)", "empty"],
        [ROOT, "Map[$].(#root == null ? 'empty' : #root)", ROOT]
    ];

    /*===================================================================
        Public static methods
      ===================================================================*/
    public override TestSuite suite()
    {
        var       result = new TestSuite();

        for (var i = 0; i < TESTS.Length; i++) 
        {
            result.addTest(new PropertyTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2]));
        }
        return result;
    }

    /*===================================================================
        Constructors
      ===================================================================*/
    public PropertyTest()
    {
	  
    }

    public PropertyTest(string name) : base(name)
    {
	    
    }

    public PropertyTest(string name, object root, string expressionString, object expectedResult, object setValue, object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
        
    }

    public PropertyTest(string name, object root, string expressionString, object expectedResult, object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {
        
    }

    public PropertyTest(string name, object root, string expressionString, object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {
        
    }
}