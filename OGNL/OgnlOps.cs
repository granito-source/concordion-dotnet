//--------------------------------------------------------------------------
//	Copyright (c) 1998-2004, Drew Davidson and Luke Blanshard
//  Copyright (c) 2026, Alexei Yashkov
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

using static OGNL.NumericTypes;

namespace OGNL;

///
/// This is a class with static methods that define the operations of OGNL.
///
internal static class OgnlOps {
    private static int CompareWithConversion(object? v1, object? v2)
    {
        if (v1 == v2)
            return 0;

        var t1 = GetNumericType(v1);
        var t2 = GetNumericType(v2);
        var type = GetNumericType(t1, t2, true);

        switch (type) {
            case BigInt:
                return BigIntValue(v1).CompareTo(BigIntValue(v2));
            case BigDec:
                return BigDecValue(v1).CompareTo(BigDecValue(v2));
            case NonNumeric:
                if (t1 != NonNumeric || t2 != NonNumeric)
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

            case Float:
            case NumericTypes.Double:
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

    public static ulong UlongValue(object? value)
    {
        return value == null ? 0L : Convert.ToUInt64(value);
    }

    public static double DoubleValue(object? value)
    {
        return value switch {
            null => 0.0d,
            char c => c,
            _ => Convert.ToDouble(value)
        };
    }

    public static float FloatValue(object? value)
    {
        return value switch {
            null => 0.0f,
            char c => c,
            _ => Convert.ToSingle(value)
        };
    }

    private static long BigIntValue(object? value)
    {
        return value == null ? 0L : Convert.ToInt64(value);
    }

    public static decimal BigDecValue(object? value)
    {
        return value switch {
            null => 0L,
            char c => c,
            _ => Convert.ToDecimal(value)
        };
    }

    public static string StringValue(object? value, bool trim = false)
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
            return NonNumeric;

        var c = value.GetType();

        if (c == typeof(int) || c == typeof(uint))
            return Int;

        if (c == typeof(double))
            return NumericTypes.Double;

        if (c == typeof(bool))
            return Bool;

        if (c == typeof(byte))
            return NumericTypes.Byte;

        if (c == typeof(char))
            return NumericTypes.Char;

        if (c == typeof(short) || c == typeof(ushort))
            return Short;

        if (c == typeof(long) || c == typeof(ulong))
            return Long;

        if (c == typeof(float))
            return Float;

        return c == typeof(decimal) ? BigDec : NonNumeric;
    }

    public static object EnumValue(object value, Type toType)
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

        if (canBeNonNumeric && (t1 == NonNumeric || t2 == NonNumeric ||
            t1 == NumericTypes.Char || t2 == NumericTypes.Char))
            return NonNumeric;

        if (t1 == NonNumeric)
            t1 = NumericTypes.Double; // Try to interpret strings as doubles...

        if (t2 == NonNumeric)
            t2 = NumericTypes.Double; // Try to interpret strings as doubles...

        if (t1 >= MinRealType) {
            if (t2 >= MinRealType)
                return Math.Max(t1, t2);

            if (t2 < Int)
                return t1;

            if (t2 == BigInt)
                return BigDec;

            return Math.Max(NumericTypes.Double, t1);
        }

        if (t2 >= MinRealType) {
            if (t1 < Int)
                return t2;

            if (t1 == BigInt)
                return BigDec;

            return Math.Max(NumericTypes.Double, t2);
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
            Bool or NumericTypes.Char or Int => value,
            Float => (float)value,
            NumericTypes.Double => (double)value,
            Long or NumericTypes.Byte or Short => value,
            _ => long.Parse(value.ToString())
        };
    }

    private static object NewReal(int type, double value)
    {
        return type == Float ? (float)value : value;
    }

    public static object BinaryOr(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BigInt or BigDec => BigIntValue(v1) | BigIntValue(v2),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) | LongValue(v2))
        };
    }

    public static object BinaryXor(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BigInt or BigDec => BigIntValue(v1) ^ BigIntValue(v2),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) ^ LongValue(v2))
        };
    }

    public static object BinaryAnd(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BigInt or BigDec => BigIntValue(v1) & BigIntValue(v2),
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
            BigInt or BigDec => BigIntValue(v1) << (int)LongValue(v2),
            _ => NewInteger(GetNumericType(v1),
                LongValue(v1) << (int)LongValue(v2))
        };
    }

    public static object ShiftRight(object? v1, object? v2)
    {
        return GetNumericType(v1) switch {
            BigInt or BigDec => BigIntValue(v1) >> (int)LongValue(v2),
            _ => NewInteger(GetNumericType(v1),
                LongValue(v1) >> (int)LongValue(v2))
        };
    }

    // TODO: This method not supported.
    public static object UnsignedShiftRight(object? v1, object? v2)
    {
        return GetNumericType(v1) switch {
            BigInt or BigDec => BigIntValue(v1) >> (int)LongValue(v2),
            <= Int => NewInteger(Int,
                (int)LongValue(v1) >> (int)LongValue(v2)),
            _ => NewInteger(GetNumericType(v1),
                LongValue(v1) >> (int)LongValue(v2))
        };
    }

    public static object Add(object? v1, object? v2)
    {
        var type = GetNumericType(v1, v2, true);

        switch (type) {
            case BigInt:
                return BigIntValue(v1) + BigIntValue(v2);
            case BigDec:
                return BigDecValue(v1) + BigDecValue(v2);
            case Float:
            case NumericTypes.Double:
                return NewReal(type, DoubleValue(v1) + DoubleValue(v2));
            case NonNumeric:
                int t1 = GetNumericType(v1),
                    t2 = GetNumericType(v2);

                if ((t1 != NonNumeric && v2 == null) ||
                    (t2 != NonNumeric && v1 == null))
                    throw new NullReferenceException();

                return StringValue(v1) + StringValue(v2);
            default:
                return NewInteger(type, LongValue(v1) + LongValue(v2));
        }
    }

    public static object Subtract(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BigInt => BigIntValue(v1) - BigIntValue(v2),
            BigDec => BigDecValue(v1) - BigDecValue(v2),
            Float or NumericTypes.Double => NewReal(GetNumericType(v1, v2),
                DoubleValue(v1) - DoubleValue(v2)),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) - LongValue(v2))
        };
    }

    public static object Multiply(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BigInt => BigIntValue(v1) * BigIntValue(v2),
            BigDec => BigDecValue(v1) * BigDecValue(v2),
            Float or NumericTypes.Double => NewReal(GetNumericType(v1, v2),
                DoubleValue(v1) * DoubleValue(v2)),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) * LongValue(v2))
        };
    }

    public static object Divide(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BigInt => BigIntValue(v1) / BigIntValue(v2),
            BigDec => BigDecValue(v1) / BigDecValue(v2),
            Float or NumericTypes.Double => NewReal(GetNumericType(v1, v2),
                DoubleValue(v1) / DoubleValue(v2)),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) / LongValue(v2))
        };
    }

    public static object Remainder(object? v1, object? v2)
    {
        return GetNumericType(v1, v2) switch {
            BigDec or BigInt => BigIntValue(v1) % BigIntValue(v2),
            _ => NewInteger(GetNumericType(v1, v2),
                LongValue(v1) % LongValue(v2))
        };
    }

    public static object Negate(object? value)
    {
        return GetNumericType(value) switch {
            BigInt => -BigIntValue(value),
            BigDec => -BigDecValue(value),
            Float or NumericTypes.Double => NewReal(GetNumericType(value),
                -DoubleValue(value)),
            _ => NewInteger(GetNumericType(value), -LongValue(value))
        };
    }

    public static object BitNegate(object? value)
    {
        return GetNumericType(value) switch {
            BigDec or BigInt => ~BigIntValue(value),
            _ => NewInteger(GetNumericType(value), ~LongValue(value))
        };
    }
}
