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

using System.Diagnostics.CodeAnalysis;

namespace OGNL;

/// <summary>
/// Implementation of PropertyAccessor that uses numbers and dynamic
/// subscripts as properties to index into arrays.
/// </summary>
/// @author Luke Blanshard (blanshlu@netscape.net)
/// @author Drew Davidson (drew@ognl.org)
public class ArrayPropertyAccessor : ObjectPropertyAccessor {
    private static object? getElement(Array array, int index)
    {
        return index >= 0 ? array.GetValue(index) : null;
    }

    private static void setElement(Array array, int index, object? value)
    {
        if (index >= 0)
            array.SetValue(value, index);
    }

    [return: NotNullIfNotNull("value")]
    private static object? convert(OgnlContext context, Array target,
        object name, object? value)
    {
        if (value == null)
            return null;

        return context
            .getTypeConverter()
            .convertValue(context, target, null, name.ToString(), value,
                target.GetType().GetElementType()!);
    }

    /// <summary>
    /// Specific property is: length.
    /// </summary>
    /// <returns></returns>
    public override object? getProperty(OgnlContext context,
        object target, object name)
    {
        var array = (Array)target;

        switch (name) {
            case string:
                return name.Equals("length") ? array.GetLength(0) :
                    base.getProperty(context, target, name);
            case DynamicSubscript dynamic:
                var len = array.GetLength(0);

                switch (dynamic.getFlag()) {
                    case DynamicSubscript.ALL:
                        var copy = Array.CreateInstance(
                            array.GetType().GetElementType()!, len);

                        Array.Copy(array, 0, copy, 0, len);

                        return copy;
                    case DynamicSubscript.FIRST:
                        return getElement(array, len > 0 ? 0 : -1);
                    case DynamicSubscript.MID:
                        return getElement(array, len > 0 ? len / 2 : -1);
                    case DynamicSubscript.LAST:
                        return getElement(array, len > 0 ? len - 1 : -1);
                }

                break;
            case ValueType:
                return getElement(array, Convert.ToInt32(name));
        }

        throw new NoSuchPropertyException(target, name);
    }

    public override void setProperty(OgnlContext context, object target,
        object name, object? value)
    {
        var array = (Array)target;

        switch (name) {
            case string:
                base.setProperty(context, target, name, value);

                break;
            case DynamicSubscript dynamic:
                var converted = convert(context, array, name, value);
                var len = array.GetLength(0);

                switch (dynamic.getFlag()) {
                    case DynamicSubscript.ALL:
                        if (converted == null)
                            throw new OgnlException("source array is null");

                        Array.Copy((Array)converted, 0, array, 0, len);

                        break;
                    case DynamicSubscript.FIRST:
                        setElement(array, len > 0 ? 0 : -1, converted);

                        break;
                    case DynamicSubscript.MID:
                        setElement(array, len > 0 ? len / 2 : -1, converted);

                        break;
                    case DynamicSubscript.LAST:
                        setElement(array, len > 0 ? len - 1 : -1, converted);

                        break;
                }

                break;
            case ValueType:
                setElement(array, Convert.ToInt32(name),
                    convert(context, array, name, value));

                break;
        }

        throw new NoSuchPropertyException(target, name);
    }
}
