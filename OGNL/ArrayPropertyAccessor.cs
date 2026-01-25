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

using System.Diagnostics.CodeAnalysis;

namespace OGNL;

/// <summary>
/// Implementation of PropertyAccessor that uses numbers and dynamic
/// subscripts as properties to index into arrays.
/// </summary>
internal class ArrayPropertyAccessor : ObjectPropertyAccessor {
    [return: NotNullIfNotNull("value")]
    private static object? Convert(OgnlContext context, Array target,
        object name, object? value)
    {
        if (value == null)
            return null;

        return context.TypeConverter.ConvertValue(context, target, null,
            name.ToString(), value, target.GetType().GetElementType()!);
    }

    public override object? GetProperty(OgnlContext context,
        object target, object name)
    {
        var array = (Array)target;

        switch (name) {
            case string:
                return name.Equals("length") ? array.GetLength(0) :
                    base.GetProperty(context, target, name);
            case DynamicSubscript dynamic:
                var len = array.GetLength(0);

                switch (dynamic.GetFlag()) {
                    case DynamicSubscript.AllElements:
                        var copy = Array.CreateInstance(
                            array.GetType().GetElementType()!, len);

                        Array.Copy(array, 0, copy, 0, len);

                        return copy;
                    case DynamicSubscript.FirstElement:
                        return array.GetValue(len > 0 ? 0 : -1);
                    case DynamicSubscript.MidElement:
                        return array.GetValue(len > 0 ? len / 2 : -1);
                    case DynamicSubscript.LastElement:
                        return array.GetValue(len > 0 ? len - 1 : -1);
                }

                break;
            case ValueType:
                return array.GetValue(System.Convert.ToInt32(name));
        }

        throw new NoSuchPropertyException(target, name);
    }

    public override void SetProperty(OgnlContext context, object target,
        object name, object? value)
    {
        var array = (Array)target;

        switch (name) {
            case string:
                base.SetProperty(context, target, name, value);

                return;
            case DynamicSubscript dynamic:
                var len = array.GetLength(0);

                switch (dynamic.GetFlag()) {
                    case DynamicSubscript.AllElements:
                        if (value is not Array source)
                            throw new OgnlException("value is not an array");

                        Array.Copy(source, 0, array, 0,
                            int.Min(len, source.GetLength(0)));

                        break;
                    case DynamicSubscript.FirstElement:
                        array.SetValue(
                            Convert(context, array, name, value),
                            len > 0 ? 0 : -1);

                        break;
                    case DynamicSubscript.MidElement:
                        array.SetValue(
                            Convert(context, array, name, value),
                            len > 0 ? len / 2 : -1);

                        break;
                    case DynamicSubscript.LastElement:
                        array.SetValue(
                            Convert(context, array, name, value),
                            len > 0 ? len - 1 : -1);

                        break;
                }

                return;
            case ValueType:
                array.SetValue(Convert(context, array, name, value),
                    System.Convert.ToInt32(name));

                return;
        }

        throw new NoSuchPropertyException(target, name);
    }
}
