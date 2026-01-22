//--------------------------------------------------------------------------
//	Copyright (c) 1998-2004, Drew Davidson and Luke Blanshard
//  All rights reserved.
//
//	Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are
//  met:
//
//	Redistributions of source code must retain the above copyright notice,
//  this list of conditions and the following disclaimer.
//	Redistributions in binary form must reproduce the above copyright
//  notice, this list of conditions and the following disclaimer in the
//  documentation and/or other materials provided with the distribution.
//	Neither the name of the Drew Davidson nor the names of its contributors
//  may be used to endorse or promote products derived from this software
//  without specific prior written permission.
//
//	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
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

namespace OGNL;

///
///This is an abstract class with static methods that define the operations of OGNL.
///@author Luke Blanshard (blanshlu@netscape.net)
///@author Drew Davidson (drew@ognl.org)
///
public abstract class OgnlOps : NumericTypes {
    private static int CompareWithConversion(object? v1, object? v2)
    {
        if (v1 == v2)
            return 0;

        var t1 = GetNumericType(v1);
        var t2 = GetNumericType(v2);
        var type = GetNumericType(t1, t2, true);

        switch (type) {
            case BIGINT:
                return BigIntValue(v1).CompareTo(BigIntValue(v2));
            case BIGDEC:
                return BigDecValue(v1).CompareTo(BigDecValue(v2));
            case NONNUMERIC:
                if (t1 != NONNUMERIC || t2 != NONNUMERIC)
                    return DoubleValue(v1).CompareTo(DoubleValue(v2));

                if (v1 is IComparable comparable &&
                    v1.GetType().IsInstanceOfType(v2))
                    return comparable.CompareTo(v2);

                if (v1.GetType().IsEnum && v2.GetType().IsEnum)
                    return LongValue(v1).CompareTo(LongValue(v2));

                if (v1.GetType().IsEnum || v2.GetType().IsEnum) {
                    var enumType = v1.GetType();

                    if (!enumType.IsEnum) {
                        enumType = v2.GetType();
                        v1 = EnumValue(v1, enumType);
                    } else
                        v2 = EnumValue(v2, enumType);

                    return LongValue(v1).CompareTo(LongValue(v2));
                }

                throw new ArgumentException(
                    $"invalid comparison: {v1.GetType().Name} and {v2.GetType().Name}");

            case FLOAT:
            case DOUBLE:
                return DoubleValue(v1).CompareTo(DoubleValue(v2));
            default:
                return LongValue(v1).CompareTo(LongValue(v2));
        }
    }

    private static bool IsEqual(object? object1, object? object2)
    {
        var result = false;

        if (object1 == object2) {
            result = true;
        } else {
            if (object1 != null && object1.GetType().IsArray) {
                if (object2 != null && object2.GetType().IsArray && object2.GetType() == object1.GetType()) {
                    result = ((Array)object1).Length == ((Array)object2).Length;

                    if (result) {
                        for (int i = 0, icount = ((Array)object1).Length; result && i < icount; i++) {
                            result = IsEqual(((Array)object1).GetValue(i), ((Array)object2).GetValue(i));
                        }
                    }
                }
            } else {
                // Check for converted equivalence first, then Equals() equivalence
                result = object1 != null && object2 != null &&
                    (CompareWithConversion(object1, object2) == 0 || object1.Equals(object2));
            }
        }

        return result;
    }

    ///<summary>
    ///Evaluates the given object as a bool: if it is a Boolean object, it's easy; if
    ///it's a Number or a Character, returns true for non-zero objects; and otherwise
    ///returns true for non-null objects.
    ///</summary>
    ///<param name="value" >an object to interpret as a bool</param>
    ///<returns>the bool value implied by the given object</returns>
    ///
    public static bool BooleanValue(object? value)
    {
        return value switch {
            null => false,
            char c => c != 0,
            _ => Convert.ToBoolean(value)
        };
    }

    /// <summary>
    /// Evaluates the given object as a long integer.
    /// </summary>
    /// <param name="value">an object to interpret as a long integer</param>
    /// <returns>the long integer value implied by the given object</returns>
    /// <exception cref="FormatException"  > if the given object can't be
    /// understood as a long integer</exception>
    ///
    public static long LongValue(object? value)
    {
        return value == null ? 0L : Convert.ToInt64(value);
    }

    private static double DoubleValue(object? value)
    {
        return value switch {
            null => 0.0,
            char c => c,
            _ => Convert.ToDouble(value)
        };
    }

    private static long BigIntValue(object? value)
    {
        return value == null ? 0L : Convert.ToInt64(value);
    }

    private static decimal BigDecValue(object? value)
    {
        return value switch {
            null => 0L,
            char c => c,
            _ => Convert.ToDecimal(value)
        };
    }

    private static string StringValue(object? value, bool trim = false)
    {
        var result = value?.ToString();

        if (result == null)
            return OgnlRuntime.NullString;

        return trim ? result.Trim() : result;
    }

    ///<summary>
    ///Returns a constant from the NumericTypes interface that represents the numeric
    ///type of the given object.
    ///</summary>
    ///<param name="value">an object that needs to be interpreted as a number</param>
    ///<returns>the appropriate constant from the NumericTypes interface</returns>
    ///
    public static int GetNumericType(object? value)
    {
        if (value == null)
            return NONNUMERIC;

        var c = value.GetType();

        if (c == typeof(int) || c == typeof(uint))
            return INT;

        if (c == typeof(double))
            return DOUBLE;

        if (c == typeof(bool))
            return BOOL;

        if (c == typeof(byte))
            return BYTE;

        if (c == typeof(char))
            return CHAR;

        if (c == typeof(short) || c == typeof(ushort))
            return SHORT;

        if (c == typeof(long) || c == typeof(ulong))
            return LONG;

        if (c == typeof(float))
            return FLOAT;

        return c == typeof(decimal) ? BIGDEC : NONNUMERIC;
    }

    /// <summary>
    /// Returns the value converted numerically to the given class type
    /// </summary>
    /// <remarks>
    /// This method also detects when arrays are being converted and
    /// converts the components of one array to the type of the other.
    /// </remarks>
    /// <param name="value">an object to be converted to the given type</param>
    /// <param name="toType">class type to be converted to</param>
    /// <returns>converted value of the type given, or value if the value
    ///                cannot be converted to the given type.</returns>
    ///
    public static object? ConvertValue(object? value, Type toType)
    {
        if (value != null) {
            if (value.GetType().IsArray && toType.IsArray) {
                var original = (Array)value;
                var componentType = toType.GetElementType();
                var copy = Array.CreateInstance(componentType!,
                    original.Length);

                for (int i = 0, icount = original.Length; i < icount; i++)
                    copy.SetValue(ConvertValue(original.GetValue(i),
                        componentType!), i);

                return copy;
            }

            if (toType == typeof(int) || toType == typeof(uint))
                return (int)LongValue(value);

            if (toType == typeof(double) || toType == typeof(double))
                return DoubleValue(value);

            if (toType == typeof(bool) || toType == typeof(bool))
                return BooleanValue(value);

            if (toType == typeof(byte) || toType == typeof(byte))
                return (byte)LongValue(value);

            if (toType == typeof(char) || toType == typeof(char))
                return (char)LongValue(value);

            if (toType == typeof(short) || toType == typeof(ushort))
                return (short)LongValue(value);

            if (toType == typeof(long) || toType == typeof(ulong))
                return LongValue(value);

            if (toType == typeof(float) || toType == typeof(float))
                return (float)DoubleValue(value);

            if (toType == typeof(decimal))
                return BigDecValue(value);

            if (toType == typeof(string))
                return StringValue(value);

            if (toType.IsEnum)
                return EnumValue(value, toType);
        } else {
            if (toType.IsPrimitive)
                return OgnlRuntime.GetPrimitiveDefaultValue(toType);

            if (toType.IsEnum)
                return Enum.GetValues(toType).GetValue(0);
        }

        return null;
    }

    private static object EnumValue(object value, Type toType)
    {
        try {
            return Enum.Parse(toType, value.ToString() ?? string.Empty, true);
        } catch (Exception) {
            return Enum.ToObject(toType, (int)LongValue(value));
        }
    }

    private static int GetNumericType(int t1, int t2, bool canBeNonNumeric)
    {
        if (t1 == t2)
            return t1;

        if (canBeNonNumeric && (t1 == NONNUMERIC || t2 == NONNUMERIC ||
            t1 == CHAR || t2 == CHAR))
            return NONNUMERIC;

        if (t1 == NONNUMERIC)
            t1 = DOUBLE; // Try to interpret strings as doubles...

        if (t2 == NONNUMERIC)
            t2 = DOUBLE; // Try to interpret strings as doubles...

        if (t1 >= MIN_REAL_TYPE) {
            if (t2 >= MIN_REAL_TYPE)
                return Math.Max(t1, t2);

            if (t2 < INT)
                return t1;

            if (t2 == BIGINT)
                return BIGDEC;

            return Math.Max(DOUBLE, t1);
        }

        if (t2 >= MIN_REAL_TYPE) {
            if (t1 < INT)
                return t2;

            if (t1 == BIGINT)
                return BIGDEC;

            return Math.Max(DOUBLE, t2);
        }

        return Math.Max(t1, t2);
    }

    private static int GetNumericType(object? v1, object? v2,
        bool canBeNonNumeric = false)
    {
        return GetNumericType(GetNumericType(v1), GetNumericType(v2),
            canBeNonNumeric);
    }

    ///
    ///Returns a new Number object of an appropriate type to hold the given integer
    ///value.  The type of the returned object is consistent with the given type
    ///argument, which is a constant from the NumericTypes interface.
    ///
    ///@param type    the nominal numeric type of the result, a constant from the NumericTypes interface
    ///@param value   the integer value to convert to a Number object
    ///@return        a Number object with the given value, of type implied by the type argument
    ///
    public static object NewInteger(int type, long value)
    {
        return type switch {
            BOOL or CHAR or INT => value,
            FLOAT => (float)value,
            DOUBLE => (double)value,
            LONG or BYTE or SHORT => value,
            _ => long.Parse(value.ToString())
        };
    }

    private static object NewReal(int type, double value)
    {
        return type == FLOAT ? (float)value : value;
    }

    public static object BinaryOr(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BIGINT or BIGDEC => BigIntValue(v1) | BigIntValue(v2),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) | LongValue(v2))
        };
    }

    public static object BinaryXor(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BIGINT or BIGDEC => BigIntValue(v1) ^ BigIntValue(v2),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) ^ LongValue(v2))
        };
    }

    public static object BinaryAnd(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BIGINT or BIGDEC => BigIntValue(v1) & BigIntValue(v2),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) & LongValue(v2))
        };
    }

    public static bool Equal(object? v1, object? v2)
    {
        if (v1 == null)
            return v2 == null;

        if (v1 == v2 || IsEqual(v1, v2))
            return true;

        if (v1 is ValueType && v2 is ValueType)
            return Convert.ToDouble(v1).Equals(v2);

        return false;
    }

    public static bool Less(object? v1, object? v2)
    {
        return CompareWithConversion(v1, v2) < 0;
    }

    public static bool Greater(object? v1, object? v2)
    {
        return CompareWithConversion(v1, v2) > 0;
    }

    public static bool OperIn(object? v1, object? v2)
    {
        if (v2 == null) // A null collection is always treated as empty
            return false;

        var elementsAccessor = OgnlRuntime
            .GetElementsAccessor(OgnlRuntime.GetTargetType(v2));

        for (var e = elementsAccessor.GetElements(v2); e.MoveNext();) {
            var o = e.Current;

            if (Equal(v1, o))
                return true;
        }

        return false;
    }

    public static object ShiftLeft(object? v1, object? v2)
    {
        return GetNumericType(v1) switch {
            BIGINT or BIGDEC => BigIntValue(v1) << (int)LongValue(v2),
            _ => NewInteger(GetNumericType(v1),
                LongValue(v1) << (int)LongValue(v2))
        };
    }

    public static object ShiftRight(object? v1, object? v2)
    {
        return GetNumericType(v1) switch {
            BIGINT or BIGDEC => BigIntValue(v1) >> (int)LongValue(v2),
            _ => NewInteger(GetNumericType(v1),
                LongValue(v1) >> (int)LongValue(v2))
        };
    }

    // TODO: This method not supported.
    public static object UnsignedShiftRight(object? v1, object? v2)
    {
        return GetNumericType(v1) switch {
            BIGINT or BIGDEC => BigIntValue(v1) >> (int)LongValue(v2),
            <= INT => NewInteger(INT,
                (int)LongValue(v1) >> (int)LongValue(v2)),
            _ => NewInteger(GetNumericType(v1),
                LongValue(v1) >> (int)LongValue(v2))
        };
    }

    public static object Add(object? v1, object? v2)
    {
        var type = GetNumericType(v1, v2, true);

        switch (type) {
            case BIGINT:
                return BigIntValue(v1) + BigIntValue(v2);
            case BIGDEC:
                return BigDecValue(v1) + BigDecValue(v2);
            case FLOAT:
            case DOUBLE:
                return NewReal(type, DoubleValue(v1) + DoubleValue(v2));
            case NONNUMERIC:
                int t1 = GetNumericType(v1),
                    t2 = GetNumericType(v2);

                if ((t1 != NONNUMERIC && v2 == null) ||
                    (t2 != NONNUMERIC && v1 == null))
                    throw new NullReferenceException();

                return StringValue(v1) + StringValue(v2);
            default:
                return NewInteger(type, LongValue(v1) + LongValue(v2));
        }
    }

    public static object Subtract(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BIGINT => BigIntValue(v1) - BigIntValue(v2),
            BIGDEC => BigDecValue(v1) - BigDecValue(v2),
            FLOAT or DOUBLE => NewReal(GetNumericType(v1, v2),
                DoubleValue(v1) - DoubleValue(v2)),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) - LongValue(v2))
        };
    }

    public static object Multiply(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BIGINT => BigIntValue(v1) * BigIntValue(v2),
            BIGDEC => BigDecValue(v1) * BigDecValue(v2),
            FLOAT or DOUBLE => NewReal(GetNumericType(v1, v2),
                DoubleValue(v1) * DoubleValue(v2)),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) * LongValue(v2))
        };
    }

    public static object Divide(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BIGINT => BigIntValue(v1) / BigIntValue(v2),
            BIGDEC => BigDecValue(v1) / BigDecValue(v2),
            FLOAT or DOUBLE => NewReal(GetNumericType(v1, v2),
                DoubleValue(v1) / DoubleValue(v2)),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) / LongValue(v2))
        };
    }

    public static object Remainder(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BIGDEC or BIGINT => BigIntValue(v1) % BigIntValue(v2),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) % LongValue(v2))
        };
    }

    public static object Negate(object? value)
    {
        return GetNumericType(value) switch {
            BIGINT => -BigIntValue(value),
            BIGDEC => -BigDecValue(value),
            FLOAT or DOUBLE => NewReal(GetNumericType(value),
                -DoubleValue(value)),
            _ => NewInteger(GetNumericType(value), -LongValue(value))
        };
    }

    public static object BitNegate(object? value)
    {
        return GetNumericType(value) switch {
            BIGDEC or BIGINT => ~BigIntValue(value),
            _ => NewInteger(GetNumericType(value), ~LongValue(value))
        };
    }
}
