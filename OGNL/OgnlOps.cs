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
    ///<summary>
    ///Compares two objects for equality, even if it has to convert
    ///one of them to the other type.  If both objects are numeric
    ///they are converted to the widest type and compared.
    ///</summary>
    ///<remarks>If one is non-numeric and one is numeric the non-numeric is
    ///converted to double and compared to the double numeric value.
    ///<para>If both are non-numeric and Comparable and the
    ///types are compatible (i.e. v1 is of the same or superclass
    ///of v2's type) they are compared with Comparable.compareTo().
    ///If both values are non-numeric and not Comparable or of
    ///incompatible classes this will throw and IllegalArgumentException.
    ///</para>
    ///</remarks>
    ///<param name="v1"> First value to compare</param>
    ///<param name="v2"> second value to compare</param>
    ///
    ///<returns>
    ///      integer describing the comparison between the two objects.
    ///         A negative number indicates that v1 &lt; v2.
    ///         Positive indicates that v1 &gt; v2.
    ///         Zero indicates v1 == v2.
    ///</returns>
    ///
    /// <exception cref="ArgumentException">if the objects are both non-numeric
    ///             yet of incompatible types or do not implement Comparable.</exception>
    ///
    public static int compareWithConversion(object v1, object v2)
    {
        int result;

        if (v1 == v2) {
            result = 0;
        } else {
            int t1 = getNumericType(v1),
                t2 = getNumericType(v2),
                type = getNumericType(t1, t2, true);

            switch (type) {
                case BIGINT:
                    result = bigIntValue(v1).CompareTo(bigIntValue(v2));

                    break;

                case BIGDEC:
                    result = bigDecValue(v1).CompareTo(bigDecValue(v2));

                    break;

                case NONNUMERIC:
                    if ((t1 == NONNUMERIC) && (t2 == NONNUMERIC)) {
                        if ((v1 is IComparable) && v1.GetType().IsAssignableFrom(v2.GetType())) {
                            result = ((IComparable)v1).CompareTo(v2);

                            break;
                        } else if (v1.GetType().IsEnum && v2.GetType().IsEnum) {
                            result = longValue(v1).CompareTo(longValue(v2));
                        } else if (v1.GetType().IsEnum || v2.GetType().IsEnum) {
                            var enumType = v1.GetType();

                            if (!enumType.IsEnum) {
                                enumType = v2.GetType();
                                v1 = enumValue(v1, enumType);
                            } else {
                                v2 = enumValue(v2, enumType);
                            }

                            result = longValue(v1).CompareTo(longValue(v2));
                        } else {
                            throw new ArgumentException("invalid comparison: " + v1.GetType().Name + " and " +
                                v2.GetType().Name);
                        }
                    }

                    // else fall through
                    double dvv1 = doubleValue(v1),
                        dvv2 = doubleValue(v2);

                    return (dvv1 == dvv2) ? 0 : ((dvv1 < dvv2) ? -1 : 1);
                case FLOAT:
                case DOUBLE:
                    double dv1 = doubleValue(v1),
                        dv2 = doubleValue(v2);

                    return (dv1 == dv2) ? 0 : ((dv1 < dv2) ? -1 : 1);

                default:
                    long lv1 = longValue(v1),
                        lv2 = longValue(v2);

                    return (lv1 == lv2) ? 0 : ((lv1 < lv2) ? -1 : 1);
            }
        }

        return result;
    }

    /// <summary>
    ///Returns true if object1 is equal to object2 in either the
    ///sense that they are the same object or, if both are non-null
    ///if they are equal in the <c>Equals()</c> sense.
    ///</summary>
    ///<param name="object1" >First object to compare</param>
    ///<param name="object2">Second object to compare</param>
    ///
    ///<returns>true if object1 == object1</returns>
    ///
    public static bool isEqual(object object1, object object2)
    {
        var result = false;

        if (object1 == object2) {
            result = true;
        } else {
            if ((object1 != null) && object1.GetType().IsArray) {
                if ((object2 != null) && object2.GetType().IsArray && (object2.GetType() == object1.GetType())) {
                    result = (((Array)object1).Length == ((Array)object2).Length);

                    if (result) {
                        for (int i = 0, icount = ((Array)object1).Length; result && (i < icount); i++) {
                            result = isEqual(((Array)object1).GetValue(i), ((Array)object2).GetValue(i));
                        }
                    }
                }
            } else {
                // Check for converted equivalence first, then Equals() equivalence
                result = (object1 != null) && (object2 != null) &&
                    ((compareWithConversion(object1, object2) == 0) || object1.Equals(object2));
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
    public static bool booleanValue(object? value)
    {
        return value switch {
            null => false,
            char c => c != 0,
            _ => Convert.ToBoolean(value)
        };
    }

    ///<summary>
    ///Evaluates the given object as a long integer.
    ///</summary>
    ///<param name="value">an object to interpret as a long integer</param>
    ///<returns>the long integer value implied by the given object</returns>
    ///<exception cref="FormatException"  > if the given object can't be understood as a long integer</exception>
    ///
    public static long longValue(object? value)
    {
        return value == null ? 0L : Convert.ToInt64(value);
    }

    ///<summary>
    ///Evaluates the given object as a double-precision floating-point number.
    ///</summary>
    ///<param name="value">an object to interpret as a double</param>
    ///<returns>the double value implied by the given object</returns>
    /// <exception cref="FormatException"> if the give object can't be understood as a double</exception>
    ///
    public static double doubleValue(object? value)
    {
        return value switch {
            null => 0.0,
            char c => c,
            _ => Convert.ToDouble(value)
        };
    }

    /// <summary>
    ///Evaluates the given object as a BigInteger.
    ///</summary>
    ///<param name="value">an object to interpret as a BigInt</param>
    ///<returns>the BigInt value implied by the given object</returns>
    /// <exception cref="FormatException"> if the give object can't be understood as a BigInt</exception>
    ///
    public static long bigIntValue(object? value)
    {
        return value == null ? 0L : Convert.ToInt64(value);
    }

    ///<summary>
    ///Evaluates the given object as a BigDecimal.
    ///</summary>
    ///<param name="value">an object to interpret as a BigDecimal</param>
    ///<returns>the BigDecimal value implied by the given object</returns>
    /// <exception cref="FormatException"> if the give object can't be understood as a BigDecimal</exception>
    ///
    public static decimal bigDecValue(object? value)
    {
        return value switch {
            null => 0L,
            char c => c,
            _ => Convert.ToDecimal(value)
        };
    }

    /// <summary>
    /// Evaluates the given object as a string and trims it if the trim flag is true.
    /// </summary>
    /// <param name="value">an object to interpret as a string</param>
    /// <param name="trim"></param>
    /// <returns>the string value implied by the given object as returned by the ToString() method,
    ///        or "null" if the object is null.</returns>
    public static string stringValue(object? value, bool trim = false)
    {
        var result = value?.ToString();

        if (result == null)
            return OgnlRuntime.NULL_STRING;

        return trim ? result.Trim() : result;
    }

    ///<summary>
    ///Returns a constant from the NumericTypes interface that represents the numeric
    ///type of the given object.
    ///</summary>
    ///<param name="value">an object that needs to be interpreted as a number</param>
    ///<returns>the appropriate constant from the NumericTypes interface</returns>
    ///
    public static int getNumericType(object? value)
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

        if (c == typeof(decimal))
            return BIGDEC;

        return NONNUMERIC;
    }

    /// <summary>
    /// Returns the value converted numerically to the given class type
    /// </summary>
    /// <remarks>
    /// This method also detects when arrays are being converted and
    /// converts the components of one array to the type of the other.
    /// </remarks>
    /// <param name="value">an object to be converted to the given type</param>
    ///<param name="toType">class type to be converted to</param>
    /// <returns>converted value of the type given, or value if the value
    ///                cannot be converted to the given type.</returns>
    ///
    public static object convertValue(object? value, Type toType)
    {
        object? result = null;

        if (value != null) {
            /* If array -> array then convert components of array individually */
            if (value.GetType().IsArray && toType.IsArray) {
                var componentType = toType.GetElementType();

                result = Array.CreateInstance(componentType, ((Array)value).Length);

                for (int i = 0, icount = ((Array)value).Length; i < icount; i++) {
                    ((Array)result).SetValue(convertValue(((Array)value).GetValue(i), componentType), i);
                }
            } else {
                if ((toType == typeof(int)) || (toType == typeof(uint))) result = (int)longValue(value);
                if ((toType == typeof(double)) || (toType == typeof(double))) result = doubleValue(value);
                if ((toType == typeof(bool) || (toType == typeof(bool))))
                    result = booleanValue(value); // ? Boolean.TRUE : Boolean.FALSE;
                if ((toType == typeof(byte) || (toType == typeof(byte)))) result = (byte)longValue(value);
                if ((toType == typeof(char)) || (toType == typeof(char))) result = ((char)longValue(value));
                if ((toType == typeof(short)) || (toType == typeof(ushort))) result = (short)longValue(value);
                if ((toType == typeof(long)) || (toType == typeof(ulong))) result = (longValue(value));
                if ((toType == typeof(float)) || (toType == typeof(float))) result = (float)(doubleValue(value));

                if (toType == typeof(decimal)) result = bigDecValue(value);
                if (toType == typeof(string)) result = stringValue(value);
                if (toType.IsEnum)
                    result = enumValue(value, toType);
            }
        } else {
            if (toType.IsPrimitive) {
                result = OgnlRuntime.getPrimitiveDefaultValue(toType);
            } else

                // support Enum
            if (toType.IsEnum) {
                result = Enum.GetValues(toType).GetValue(0);
            }
        }

        return result;
    }

    public static object enumValue(object value, Type toType)
    {
        try {
            return Enum.Parse(toType, value.ToString(), true);
        } catch (Exception) {
            return Enum.ToObject(toType, (int)longValue(value));
        }
    }

    ///<summary>
    ///Returns the constant from the NumericTypes interface that best expresses the type
    ///of a numeric operation on the two given objects.
    ///</summary>
    ///<param name="v1">one argument to a numeric operator</param>
    ///<param name="v2">the other argument</param>
    ///<returns>the appropriate constant from the NumericTypes interface</returns>
    ///
    public static int getNumericType(object v1, object v2)
    {
        return getNumericType(v1, v2, false);
    }

    ///
    ///Returns the constant from the NumericTypes interface that best expresses the type
    ///of an operation, which can be either numeric or not, on the two given types.
    ///
    ///@param t1 type of one argument to an operator
    ///@param t2 type of the other argument
    ///@param canBeNonNumeric whether the operator can be interpreted as non-numeric
    ///@return the appropriate constant from the NumericTypes interface
    ///
    public static int getNumericType(int t1, int t2, bool canBeNonNumeric)
    {
        if (t1 == t2)
            return t1;

        if (canBeNonNumeric && (t1 == NONNUMERIC || t2 == NONNUMERIC || t1 == CHAR || t2 == CHAR))
            return NONNUMERIC;

        if (t1 == NONNUMERIC) t1 = DOUBLE; // Try to interpret strings as doubles...
        if (t2 == NONNUMERIC) t2 = DOUBLE; // Try to interpret strings as doubles...

        if (t1 >= MIN_REAL_TYPE) {
            if (t2 >= MIN_REAL_TYPE)
                return Math.Max(t1, t2);
            if (t2 < INT)
                return t1;
            if (t2 == BIGINT)
                return BIGDEC;

            return Math.Max(DOUBLE, t1);
        } else if (t2 >= MIN_REAL_TYPE) {
            if (t1 < INT)
                return t2;
            if (t1 == BIGINT)
                return BIGDEC;

            return Math.Max(DOUBLE, t2);
        } else
            return Math.Max(t1, t2);
    }

    ///
    ///Returns the constant from the NumericTypes interface that best expresses the type
    ///of an operation, which can be either numeric or not, on the two given objects.
    ///
    ///@param v1 one argument to an operator
    ///@param v2 the other argument
    ///@param canBeNonNumeric whether the operator can be interpreted as non-numeric
    ///@return the appropriate constant from the NumericTypes interface
    ///
    public static int getNumericType(object v1, object v2, bool canBeNonNumeric)
    {
        return getNumericType(getNumericType(v1), getNumericType(v2), canBeNonNumeric);
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
    public static object newInteger(int type, long value)
    {
        return type switch {
            BOOL or CHAR or INT => value,
            FLOAT => (float)value,
            DOUBLE => (double)value,
            LONG or BYTE or SHORT => value,
            _ => long.Parse(value.ToString())
        };
    }

    ///
    ///Returns a new Number object of an appropriate type to hold the given real value.
    ///The type of the returned object is always either Float or Double, and is only
    ///Float if the given type tag (a constant from the NumericTypes interface) is
    ///FLOAT.
    ///
    ///@param type    the nominal numeric type of the result, a constant from the NumericTypes interface
    ///@param value   the real value to convert to a Number object
    ///@return        a Number object with the given value, of type implied by the type argument
    ///
    public static object newReal(int type, double value)
    {
        if (type == FLOAT)
            return (float)value;

        return value;
    }

    public static object binaryOr(object v1, object v2)
    {
        var type = getNumericType(v1, v2);

        if (type == BIGINT || type == BIGDEC)
            return bigIntValue(v1) | (bigIntValue(v2));

        return newInteger(type, longValue(v1) | longValue(v2));
    }

    public static object binaryXor(object v1, object v2)
    {
        var type = getNumericType(v1, v2);

        if (type == BIGINT || type == BIGDEC)
            return bigIntValue(v1) ^ (bigIntValue(v2));

        return newInteger(type, longValue(v1) ^ longValue(v2));
    }

    public static object binaryAnd(object v1, object v2)
    {
        var type = getNumericType(v1, v2);

        if (type == BIGINT || type == BIGDEC)
            return bigIntValue(v1) & (bigIntValue(v2));

        return newInteger(type, longValue(v1) & longValue(v2));
    }

    public static bool equal(object? v1, object? v2)
    {
        if (v1 == null)
            return v2 == null;
        if (v1 == v2 || isEqual(v1, v2))
            return true;
        if (v1 is ValueType && v2 is ValueType)
            return Convert.ToDouble(v1) == Convert.ToDouble(v2);

        return false;
    }

    public static bool less(object v1, object v2)
    {
        return compareWithConversion(v1, v2) < 0;
    }

    public static bool greater(object v1, object v2)
    {
        return compareWithConversion(v1, v2) > 0;
    }

    public static bool operin(object v1, object v2) // throws OgnlException
    {
        if (v2 == null) // A null collection is always treated as empty
            return false;

        var elementsAccessor = OgnlRuntime.getElementsAccessor(OgnlRuntime.getTargetClass(v2));

        for (var e = elementsAccessor.getElements(v2); e.MoveNext();) {
            var o = e.Current;

            if (equal(v1, o))
                return true;
        }

        return false;
    }

    public static object shiftLeft(object v1, object v2)
    {
        var type = getNumericType(v1);

        if (type == BIGINT || type == BIGDEC)
            return bigIntValue(v1) << ((int)longValue(v2));

        return newInteger(type, longValue(v1) << (int)longValue(v2));
    }

    public static object shiftRight(object v1, object v2)
    {
        var type = getNumericType(v1);

        if (type == BIGINT || type == BIGDEC)
            return bigIntValue(v1) >> ((int)longValue(v2));

        return newInteger(type, longValue(v1) >> (int)longValue(v2));
    }

    // TODO: This method not supported.
    public static object unsignedShiftRight(object v1, object v2)
    {
        var type = getNumericType(v1);

        if (type == BIGINT || type == BIGDEC)
            return bigIntValue(v1) >> ((int)longValue(v2));
        if (type <= INT)
            return newInteger(INT, ((int)longValue(v1)) >> (int)longValue(v2));

        return newInteger(type, longValue(v1) >> (int)longValue(v2));
    }

    public static object add(object v1, object v2)
    {
        var type = getNumericType(v1, v2, true);

        switch (type) {
            case BIGINT:
                return bigIntValue(v1) + (bigIntValue(v2));
            case BIGDEC:
                return bigDecValue(v1) + (bigDecValue(v2));
            case FLOAT:
            case DOUBLE:
                return newReal(type, doubleValue(v1) + doubleValue(v2));
            case NONNUMERIC:
                int t1 = getNumericType(v1),
                    t2 = getNumericType(v2);

                if (((t1 != NONNUMERIC) && (v2 == null)) || ((t2 != NONNUMERIC) && (v1 == null))) {
                    throw new NullReferenceException();
                }

                return stringValue(v1) + stringValue(v2);
            default:
                return newInteger(type, longValue(v1) + longValue(v2));
        }
    }

    public static object subtract(object v1, object v2)
    {
        var type = getNumericType(v1, v2);

        switch (type) {
            case BIGINT:
                return bigIntValue(v1) - (bigIntValue(v2));
            case BIGDEC:
                return bigDecValue(v1) - (bigDecValue(v2));
            case FLOAT:
            case DOUBLE:
                return newReal(type, doubleValue(v1) - doubleValue(v2));
            default:
                return newInteger(type, longValue(v1) - longValue(v2));
        }
    }

    public static object multiply(object v1, object v2)
    {
        var type = getNumericType(v1, v2);

        switch (type) {
            case BIGINT:
                return bigIntValue(v1) * (bigIntValue(v2));
            case BIGDEC:
                return bigDecValue(v1) * (bigDecValue(v2));
            case FLOAT:
            case DOUBLE:
                return newReal(type, doubleValue(v1) * doubleValue(v2));
            default:
                return newInteger(type, longValue(v1) * longValue(v2));
        }
    }

    public static object divide(object v1, object v2)
    {
        var type = getNumericType(v1, v2);

        switch (type) {
            case BIGINT:
                return bigIntValue(v1) / (bigIntValue(v2));

            // TODO: a ROUND_HALF_EVEN.
            case BIGDEC:
                return bigDecValue(v1) / (bigDecValue(v2));
            case FLOAT:
            case DOUBLE:
                return newReal(type, doubleValue(v1) / doubleValue(v2));
            default:
                return newInteger(type, longValue(v1) / longValue(v2));
        }
    }

    public static object remainder(object v1, object v2)
    {
        var type = getNumericType(v1, v2);

        switch (type) {
            case BIGDEC:
            case BIGINT:
                return bigIntValue(v1) % (bigIntValue(v2));
            default:
                return newInteger(type, longValue(v1) % longValue(v2));
        }
    }

    public static object negate(object value)
    {
        var type = getNumericType(value);

        switch (type) {
            case BIGINT:
                return -bigIntValue(value);
            case BIGDEC:
                return -bigDecValue(value);
            case FLOAT:
            case DOUBLE:
                return newReal(type, -doubleValue(value));
            default:
                return newInteger(type, -longValue(value));
        }
    }

    public static object bitNegate(object value)
    {
        var type = getNumericType(value);

        return type switch {
            BIGDEC or BIGINT => ~bigIntValue(value),
            _ => newInteger(type, ~longValue(value))
        };
    }
}
