//--------------------------------------------------------------------------
//  Copyright (c) 2004, Drew Davidson and Luke Blanshard
//  Copyright (c) 2026, Alexei Yashkov
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

using System.Diagnostics.CodeAnalysis;

namespace OGNL.Test;

[TestFixture]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("Performance", "CA1822")]
public class PrimitiveArrayTest : OgnlFixture {
    public int Six => 6;

    private static readonly object[][] Tests = [
        ["new bool[5]", new bool[5]],
        ["new bool[] { true, false }", new[] { true, false }],
        ["new bool[] { 0, 1, 5.5 }", new[] { false, true, true }],
        ["new char[] { 'a', 'b' }", new[] { 'a', 'b' }],
        ["new char[] { 10, 11 }", new[] { (char)10, (char)11 }],
        ["new byte[] { 1, 2 }", new byte[] { 1, 2 }],
        ["new short[] { 1, 2 }", new short[] { 1, 2 }],
        ["new int[Six]", new int[6]],
        ["new int[#root.Six]", new int[6]],
        ["new int[6]", new int[6]],
        ["new int[] { 1, 2 }", new[] { 1, 2 }],
        ["new long[] { 1, 2 }", new long[] { 1, 2 }],
        ["new float[] { 1, 2 }", new float[] { 1, 2 }],
        ["new double[] { 1, 2 }", new double[] { 1, 2 }]
    ];

    [Test, TestCaseSource(nameof(Tests))]
    public void Evaluates(string expression, object expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }
}
