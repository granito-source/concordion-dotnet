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

[TestFixture]
public class NumericConversionTest : OgnlTestCase {
    private readonly object value;

    private readonly Type toClass;

    private readonly object expectedValue;

    private readonly int scale;

    private static readonly object[][] Tests = new object[][] {
        /* To typeof (int) */
        ["55", typeof(int), 55],
        [55, typeof(int), 55],
        [(double)55, typeof(int), 55],
        [true, typeof(int), 1],
        [(byte)55, typeof(int), 55],
        [(char)55, typeof(int), 55],
        [(short)55, typeof(int), 55],
        [55L, typeof(int), 55],
        [55f, typeof(int), 55],

        // { new BigInteger("55"), typeof (int), (55) },
        [(decimal)55, typeof(int), 55],

        /* To typeof (double) */
        ["55.1234", typeof(double), 55.1234],
        [55, typeof(double), (double)55],
        [(double)55.1234, typeof(double), 55.1234],
        [true, typeof(double), (double)1],
        [(byte)55, typeof(double), (double)55],
        [(char)55, typeof(double), (double)55],
        [(short)55, typeof(double), (double)55],
        [55L, typeof(double), (double)55],
        [55.1234f, typeof(double), 55.1234, 4],

        // { new BigInteger("55"), typeof (double), (double)(55) },
        [(decimal)55.1234, typeof(double), 55.1234],

        /* To typeof (bool) */
        ["true", typeof(bool), true],
        [55, typeof(bool), true],
        [(double)55, typeof(bool), true],
        [true, typeof(bool), true],
        [(byte)55, typeof(bool), true],
        [(char)55, typeof(bool), true],
        [(short)55, typeof(bool), true],
        [55L, typeof(bool), true],
        [55f, typeof(bool), true],

        // { new BigInteger("55"), typeof (bool), true },
        [(decimal)55, typeof(bool), true],

        /* To typeof (byte) */
        ["55", typeof(byte), (byte)55],
        [55, typeof(byte), (byte)55],
        [(double)55, typeof(byte), (byte)55],
        [true, typeof(byte), (byte)1],
        [(byte)55, typeof(byte), (byte)55],
        [(char)55, typeof(byte), (byte)55],
        [(short)55, typeof(byte), (byte)55],
        [55L, typeof(byte), (byte)55],
        [55f, typeof(byte), (byte)55],

        // { new BigInteger("55"), typeof (byte), ((byte)55) },
        [(decimal)55, typeof(byte), (byte)55],

        /* To typeof (char) */
        ["55", typeof(char), (char)55],
        [55, typeof(char), (char)55],
        [(double)55, typeof(char), (char)55],
        [true, typeof(char), (char)1],
        [(byte)55, typeof(char), (char)55],
        [(char)55, typeof(char), (char)55],
        [(short)55, typeof(char), (char)55],
        [55L, typeof(char), (char)55],
        [55f, typeof(char), (char)55],

        // { new BigInteger("55"), typeof (char), ((char)55) },
        [(decimal)55, typeof(char), (char)55],

        /* To typeof (short) */
        ["55", typeof(short), (short)55],
        [55, typeof(short), (short)55],
        [(double)55, typeof(short), (short)55],
        [true, typeof(short), (short)1],
        [(byte)55, typeof(short), (short)55],
        [(char)55, typeof(short), (short)55],
        [(short)55, typeof(short), (short)55],
        [55L, typeof(short), (short)55],
        [55f, typeof(short), (short)55],

        // { new BigInteger("55"), typeof (short), ((short)55) },
        [(decimal)55, typeof(short), (short)55],

        /* To typeof (long) */
        ["55", typeof(long), (long)55],
        [55, typeof(long), (long)55],
        [(double)55, typeof(long), (long)55],
        [true, typeof(long), (long)1],
        [(byte)55, typeof(long), (long)55],
        [(char)55, typeof(long), (long)55],
        [(short)55, typeof(long), (long)55],
        [(long)55, typeof(long), (long)55],
        [55f, typeof(long), (long)55],

        // { new BigInteger("55"), typeof (long), (long)(55) },
        [(decimal)55, typeof(long), (long)55],

        /* To typeof (float) */
        ["55.1234", typeof(float), (float)55.1234],
        [55, typeof(float), (float)55],
        [(double)55.1234, typeof(float), (float)55.1234],
        [true, typeof(float), (float)1],
        [(byte)55, typeof(float), (float)55],
        [(char)55, typeof(float), (float)55],
        [(short)55, typeof(float), (float)55],
        [(long)55, typeof(float), (float)55],
        [(float)55.1234, typeof(float), (float)55.1234],

        // { new BigInteger("55"), typeof (float), (float)(55) },
        [(decimal)55.1234, typeof(float), (float)55.1234],

        /* To BigInteger.class */
        /*{ "55", BigInteger.class, new BigInteger("55") },
        { (55), BigInteger.class, new BigInteger("55") },
        { (double)(55), BigInteger.class, new BigInteger("55") },
        { true, BigInteger.class, new BigInteger("1") },
        { ((byte)55), BigInteger.class, new BigInteger("55") },
        { ((char)55), BigInteger.class, new BigInteger("55") },
        { ((short)55), BigInteger.class, new BigInteger("55") },
        { (long)(55), BigInteger.class, new BigInteger("55") },
        { (float)(55), BigInteger.class, new BigInteger("55") },
        { new BigInteger("55"), BigInteger.class, new BigInteger("55") },
        {  (decimal)(55"), BigInteger.class, new BigInteger("55") },
*/
        /* To typeof (decimal) */
        ["55.1234", typeof(decimal), (decimal)55.1234],
        [55, typeof(decimal), (decimal)55],
        [(double)55.1234, typeof(decimal), (decimal)55.1234, 4],
        [true, typeof(decimal), (decimal)1],
        [(byte)55, typeof(decimal), (decimal)55],
        [(char)55, typeof(decimal), (decimal)55],
        [(short)55, typeof(decimal), (decimal)55],
        [(long)55, typeof(decimal), (decimal)55],
        [(float)55.1234, typeof(decimal), (decimal)55.1234, 4],

        // { new BigInteger("55"), typeof (decimal),  (decimal)(55) },
        [(decimal)55.1234, typeof(decimal), (decimal)55.1234],
    };

    /*===================================================================
        Public static methods
      ===================================================================*/
    public override TestSuite suite()
    {
        var result = new TestSuite();

        foreach (var t in Tests)
            result.addTest(new NumericConversionTest(t[0], (Type)t[1],
                t[2], t.Length > 3 ? (int)t[3] : -1));

        return result;
    }

    /*===================================================================
        Constructors
      ===================================================================*/
    public NumericConversionTest()
    {
    }

    public NumericConversionTest(object value, Type toClass,
        object expectedValue, int scale) :
        base(value + " [" + value.GetType().Name + "] -> " + toClass.Name + " == " + expectedValue + " [" +
            expectedValue.GetType().Name + "]" + (scale >= 0 ? " (to within " + scale + " decimal places)" : ""))
    {
        this.value = value;
        this.toClass = toClass;
        this.expectedValue = expectedValue;
        this.scale = scale;
    }

    /*===================================================================
        Overridden methods
      ===================================================================*/
    protected internal override void RunTest() // throws OgnlException
    {
        object result;

        result = OgnlOps.ConvertValue(value, toClass);

        if (!isEqual(result, expectedValue)) {
            if (scale >= 0) {
                double scalingFactor = Math.Pow(10, scale),
                    v1 = Convert.ToDouble(value) * scalingFactor,
                    v2 = Convert.ToDouble(expectedValue) * scalingFactor;

                Assert.That((int)v1 == (int)v2, Is.True);
            } else {
                Assert.Fail();
            }
        }
    }
}
