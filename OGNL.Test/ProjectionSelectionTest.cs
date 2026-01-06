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

public class ProjectionSelectionTest : OgnlTestCase
{
    private static Root             ROOT = new();

    private static object[][]       TESTS = [
        // Projection, selection
        [ROOT, "Array.{Type}", Util.asList(new Type[]{typeof (int),typeof (int),typeof (int),typeof (int)})],
        [ROOT, "Map.array.{? #this > 2 }", Util.asList(new int[]{(3), (4)})],
        [ROOT, "Map.array.{^ #this > 2 }", Util.asList(new int[]{(3)})],
        [ROOT, "Map.array.{$ #this > 2 }", Util.asList(new int[]{(4)})],
        [ROOT, "Map.array[*].{?true} instanceof System.Collections.ICollection", true]

        // { null, "#fact=1, 30H.{? #fact = #fact * (#this+1), false }, #fact", 265252859812191058636308480000000L },
    ];

    /*===================================================================
        Public static methods
      ===================================================================*/
    public override TestSuite suite()
    {
        var       result = new TestSuite();

        for (var i = 0; i < TESTS.Length; i++) 
        {
            result.addTest(new ProjectionSelectionTest((string)TESTS[i][1], TESTS[i][0], (string)TESTS[i][1], TESTS[i][2]));
        }
        return result;
    }

    [Test]
    public void Test1 ()
    {
        suite () [1].runTest ();
    }
    /*===================================================================
        Constructors
      ===================================================================*/
    public ProjectionSelectionTest()
    {
	
    }

    public ProjectionSelectionTest(string name) : base(name)
    {
	    
    }

    public ProjectionSelectionTest(string name, object root, string expressionString, object expectedResult, object setValue, object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
        
    }

    public ProjectionSelectionTest(string name, object root, string expressionString, object expectedResult, object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {
        
    }

    public ProjectionSelectionTest(string name, object root, string expressionString, object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {
        
    }
}