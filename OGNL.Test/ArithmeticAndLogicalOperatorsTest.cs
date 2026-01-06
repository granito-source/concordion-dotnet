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

public class ArithmeticAndLogicalOperatorsTest : OgnlTestCase {
    private static readonly object[][] Tests = [
        // Double-valued arithmetic expressions
        ["-1d", (double)-1],
        ["+1d", (double)1],
        ["--1f", 1f],
        ["2*2.0", (double)4],
        ["5/2.", 2.5],
        ["5+2D", (double)7],
        ["5f-2F", (float)3],
        ["5.+2*3", (double)11],
        ["(5.+2)*3", (double)21],

        // BigDecimal-valued arithmetic expressions
        ["-1b", (decimal)-1],
        ["+1b", (decimal)1],
        ["--1b", (decimal)1],
        ["2*2.0b", (decimal)4.0],
        ["5/2.B", (decimal)2.5],
        ["5.0B/2", (decimal)2.5],
        ["5+2b", (decimal)7],
        ["5-2B", (decimal)3],
        ["5.+2b*3", (decimal)11],
        ["(5.+2b)*3", (decimal)21],

        // Integer-valued arithmetic expressions
        ["-1", -1],
        ["+1", 1],
        ["--1", 1],
        ["2*2", 4],
        ["5/2", 2],
        ["5+2", 7],
        ["5-2", 3],
        ["5+2*3", 11],
        ["(5+2)*3", 21],
        ["~1", ~1],
        ["5%2", 1],
        ["5<<2", 20],
        ["5>>2", 1],
        ["5>>1+1", 1],
        ["-5>>2", -5 >> 2],
        ["-5L>>2", -5L >> 2],
        ["5. & 3", (double)1],
        ["5 ^3", 6],
        ["5l&3|5^3", 7L],
        ["5&(3|5^3)", 5],

        // Logical expressions
        ["!1", false],
        ["!null", true],
        ["5<2", false],
        ["5>2", true],
        ["5<=5", true],
        ["5>=3", true],

        // new object [] { "5<-5>>>2", true },
        ["5==5.0", true],
        ["5!=5.0", false],
        ["null in {true,false,null}", true],
        ["null not in {true,false,null}", false],
        ["null in {true,false,null}.ToArray()", true],
        ["5 in {true,false,null}", false],
        ["5 not in {true,false,null}", true],
        ["5 instanceof System.Int32", true],
        ["5. instanceof System.Int32", false],

        // Logical expressions (string versions)
        ["2 or 0", 2],
        ["1 and 0", 0],
        ["1 bor 0", 1],
        ["1 xor 0", 1],
        ["1 band 0", 0],
        ["1 eq 1", true],
        ["1 neq 1", false],
        ["1 lt 5", true],
        ["1 lte 5", true],
        ["1 gt 5", false],
        ["1 gte 5", false],
        ["1 lt 5", true],
        ["1 shl 2", 4],
        ["4 shr 2", 1],
        ["4 ushr 2", 1],
        ["not null", true],
        ["not 1", false],

        ["#x > 0", true],
        ["#x < 0", false],
        ["#x == 0", false],
        ["#x == 1", true],
        ["0 > #x", false],
        ["0 < #x", true],
        ["0 == #x", false],
        ["1 == #x", true],
        ["\"1\" > 0", true],
        ["\"1\" < 0", false],
        ["\"1\" == 0", false],
        ["\"1\" == 1", true],
        ["0 > \"1\"", false],
        ["0 < \"1\"", true],
        ["0 == \"1\"", false],
        ["1 == \"1\"", true],
        ["#x + 1", "11"],
        ["1 + #x", "11"],
        ["#y == 1", true],
        ["#y == \"1\"", true],
        ["#y + \"1\"", "11"],
        ["\"1\" + #y", "11"]
    ];

    public override TestSuite suite()
    {
        var result = new TestSuite();

        for (var i = 0; i < Tests.Length; i++) {
            result.addTest(new ArithmeticAndLogicalOperatorsTest((string)Tests[i][0] + " (" + Tests[i][1] + ")",
                null, (string)Tests[i][0], Tests[i][1]));
        }

        return result;
    }

    public ArithmeticAndLogicalOperatorsTest()
    {
    }

    public ArithmeticAndLogicalOperatorsTest(string name) : base(name)
    {
    }

    public ArithmeticAndLogicalOperatorsTest(string name, object root, string expressionString,
        object expectedResult, object setValue, object expectedAfterSetResult)
        : base(name, root, expressionString, expectedResult, setValue, expectedAfterSetResult)
    {
    }

    public ArithmeticAndLogicalOperatorsTest(string name, object root, string expressionString,
        object expectedResult, object setValue)
        : base(name, root, expressionString, expectedResult, setValue)
    {
    }

    public ArithmeticAndLogicalOperatorsTest(string name, object root, string expressionString,
        object expectedResult)
        : base(name, root, expressionString, expectedResult)
    {
    }

    [SetUp]
    public override void setUp()
    {
        base.setUp();
        context.Add("x", "1");
        context.Add("y", (decimal)1);
    }

    [Test]
    public void Test0()
    {
        suite()[0].runTest();
    }

    [Test]
    public void Test3()
    {
        suite()[3].runTest();
    }

    [Test]
    public void Test13()
    {
        suite()[13].runTest();
    }

    [Test]
    public void Test43()
    {
        suite()[43].runTest();
    }

    [Test]
    public void Test47()
    {
        suite()[47].runTest();
    }
}
