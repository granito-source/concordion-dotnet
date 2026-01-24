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

namespace OGNL.Test;

[TestFixture]
public class NumericConversionTest {
    private static readonly object[][] Tests = new object[][] {
        /* to bool */
        [true, typeof(bool), true],
        [(byte)55, typeof(bool), true],
        [(char)55, typeof(bool), true],
        [(short)55, typeof(bool), true],
        [55, typeof(bool), true],
        [0, typeof(bool), false],
        [55L, typeof(bool), true],
        [55.25f, typeof(bool), true],
        [55.25d, typeof(bool), true],
        [55.25m, typeof(bool), true],
        ["true", typeof(bool), true],
        ["false", typeof(bool), false],

        /* to byte */
        [true, typeof(byte), (byte)1],
        [(byte)55, typeof(byte), (byte)55],
        [(char)55, typeof(byte), (byte)55],
        [(short)55, typeof(byte), (byte)55],
        [55, typeof(byte), (byte)55],
        [55L, typeof(byte), (byte)55],
        [55.25f, typeof(byte), (byte)55],
        [55.25d, typeof(byte), (byte)55],
        [55.25m, typeof(byte), (byte)55],
        ["55", typeof(byte), (byte)55],

        /* to char */
        [true, typeof(char), (char)1],
        [(byte)55, typeof(char), (char)55],
        [(char)55, typeof(char), (char)55],
        [(short)55, typeof(char), (char)55],
        [55, typeof(char), (char)55],
        [55L, typeof(char), (char)55],
        [55.25f, typeof(char), (char)55],
        [55.25d, typeof(char), (char)55],
        [55.25m, typeof(char), (char)55],
        ["55", typeof(char), (char)55],

        /* to short */
        [true, typeof(short), (short)1],
        [(byte)55, typeof(short), (short)55],
        [(char)55, typeof(short), (short)55],
        [(short)55, typeof(short), (short)55],
        [55, typeof(short), (short)55],
        [55L, typeof(short), (short)55],
        [55.25f, typeof(short), (short)55],
        [55.25d, typeof(short), (short)55],
        [55.25m, typeof(short), (short)55],
        ["55", typeof(short), (short)55],

        /* to ushort */
        [true, typeof(ushort), (ushort)1],
        [(byte)55, typeof(ushort), (ushort)55],
        [(char)55, typeof(ushort), (ushort)55],
        [(short)55, typeof(ushort), (ushort)55],
        [55, typeof(ushort), (ushort)55],
        [55L, typeof(ushort), (ushort)55],
        [55.25f, typeof(ushort), (ushort)55],
        [55.25d, typeof(ushort), (ushort)55],
        [55.25m, typeof(ushort), (ushort)55],
        ["55", typeof(ushort), (ushort)55],

        /* to int */
        [true, typeof(int), 1],
        [(byte)55, typeof(int), 55],
        [(char)55, typeof(int), 55],
        [(short)55, typeof(int), 55],
        [55, typeof(int), 55],
        [55L, typeof(int), 55],
        [55.25f, typeof(int), 55],
        [55.25d, typeof(int), 55],
        [55.25m, typeof(int), 55],
        ["55", typeof(int), 55],

        /* to uint */
        [true, typeof(uint), (uint)1],
        [(byte)55, typeof(uint), (uint)55],
        [(char)55, typeof(uint), (uint)55],
        [(short)55, typeof(uint), (uint)55],
        [55, typeof(uint), (uint)55],
        [55L, typeof(uint), (uint)55],
        [55.25f, typeof(uint), (uint)55],
        [55.25d, typeof(uint), (uint)55],
        [55.25m, typeof(uint), (uint)55],
        ["55", typeof(uint), (uint)55],

        /* to long */
        [true, typeof(long), 1L],
        [(byte)55, typeof(long), 55L],
        [(char)55, typeof(long), 55L],
        [(short)55, typeof(long), 55L],
        [55, typeof(long), 55L],
        [55L, typeof(long), 55L],
        [55.25f, typeof(long), 55L],
        [55.25d, typeof(long), 55L],
        [55.25m, typeof(long), 55L],
        ["55", typeof(long), 55L],

        /* to ulong */
        [true, typeof(ulong), (ulong)1L],
        [(byte)55, typeof(ulong), (ulong)55L],
        [(char)55, typeof(ulong), (ulong)55L],
        [(short)55, typeof(ulong), (ulong)55L],
        [55, typeof(ulong), (ulong)55L],
        [55L, typeof(ulong), (ulong)55L],
        [55.25f, typeof(ulong), (ulong)55L],
        [55.25d, typeof(ulong), (ulong)55L],
        [55.25m, typeof(ulong), (ulong)55L],
        ["55", typeof(ulong), (ulong)55L],

        /* to float */
        [true, typeof(float), 1f],
        [(byte)55, typeof(float), 55f],
        [(char)55, typeof(float), 55f],
        [(short)55, typeof(float), 55f],
        [55, typeof(float), 55f],
        [55L, typeof(float), 55f],
        [55.25f, typeof(float), 55.25f],
        [55.25d, typeof(float), 55.25f],
        [55.25m, typeof(float), 55.25f],
        ["55", typeof(float), 55f],

        /* to double */
        [true, typeof(double), 1d],
        [(byte)55, typeof(double), 55d],
        [(char)55, typeof(double), 55d],
        [(short)55, typeof(double), 55d],
        [55, typeof(double), 55d],
        [55L, typeof(double), 55d],
        [55.25f, typeof(double), 55.25d],
        [55.25d, typeof(double), 55.25d],
        [55.25m, typeof(double), 55.25d],
        ["55", typeof(double), 55d],

        /* to decimal */
        [true, typeof(decimal), 1m],
        [(byte)55, typeof(decimal), 55m],
        [(char)55, typeof(decimal), 55m],
        [(short)55, typeof(decimal), 55m],
        [55, typeof(decimal), 55m],
        [55L, typeof(decimal), 55m],
        [55.25f, typeof(decimal), 55.25m],
        [55.25d, typeof(decimal), 55.25m],
        [55.25m, typeof(decimal), 55.25m],
        ["55", typeof(decimal), 55m]
    };

    [Test, TestCaseSource(nameof(Tests))]
    public void ConvertsValue(object value, Type type, object expected)
    {
        var converted = OgnlOps.ConvertValue(value, type);

        Assert.That(converted?.GetType(), Is.EqualTo(type));
        Assert.That(converted, Is.EqualTo(expected));
    }
}
