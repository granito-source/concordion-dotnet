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

/// <summary>
/// Default type conversion. Converts among numeric types and also strings.
/// </summary>
public class DefaultTypeConverter : TypeConverter {
    public object? ConvertValue(object? value, Type toType)
    {
        if (value != null) {
            if (value.GetType() == toType)
                return value;

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

            if (toType == typeof(int))
                return (int)OgnlOps.LongValue(value);

            if (toType == typeof(uint))
                return (uint)OgnlOps.UlongValue(value);

            if (toType == typeof(double))
                return OgnlOps.DoubleValue(value);

            if (toType == typeof(bool))
                return OgnlOps.BooleanValue(value);

            if (toType == typeof(byte))
                return (byte)OgnlOps.UlongValue(value);

            if (toType == typeof(char))
                return (char)OgnlOps.UlongValue(value);

            if (toType == typeof(short))
                return (short)OgnlOps.LongValue(value);

            if (toType == typeof(ushort))
                return (ushort)OgnlOps.UlongValue(value);

            if (toType == typeof(long))
                return OgnlOps.LongValue(value);

            if (toType == typeof(ulong))
                return OgnlOps.UlongValue(value);

            if (toType == typeof(float))
                return OgnlOps.FloatValue(value);

            if (toType == typeof(decimal))
                return OgnlOps.BigDecValue(value);

            if (toType == typeof(string))
                return OgnlOps.StringValue(value);

            if (toType.IsEnum)
                return OgnlOps.EnumValue(value, toType);
        } else {
            if (toType.IsPrimitive)
                return OgnlRuntime.GetPrimitiveDefaultValue(toType);

            if (toType.IsEnum)
                return Enum.GetValues(toType).GetValue(0);
        }

        return null;
    }
}
