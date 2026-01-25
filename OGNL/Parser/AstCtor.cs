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

using System.Collections;
using System.Diagnostics;

namespace OGNL.Parser;

internal class AstCtor(int id) : SimpleNode(id) {
    public string? TypeName { private get; set; }

    public bool IsArray { private get; set; }

    protected override object GetValueBody(OgnlContext context,
        object source)
    {
        Debug.Assert(TypeName != null, nameof(TypeName) + " != null");

        var root = context.Root;
        var count = GetNumChildren();
        var args = new object?[count];

        for (var i = 0; i < count; ++i)
            args[i] = Children[i].GetValue(context, root);

        if (!IsArray)
            return OgnlRuntime.CallConstructor(context, TypeName, args);

        if (args.Length != 1)
            throw new OgnlException(
                "only expect array size or fixed initializer list");

        try {
            var elementType = OgnlRuntime.TypeForName(context, TypeName);
            IList? sourceList = null;
            int size;

            if (args[0] is IList list) {
                sourceList = list;
                size = sourceList.Count;
            } else
                size = (int)OgnlOps.LongValue(args[0]);

            object result = Array.CreateInstance(elementType, size);

            if (sourceList == null)
                return result;

            var converter = context.TypeConverter;

            for (var i = 0; i < sourceList.Count; i++) {
                var o = sourceList[i];

                if (o == null || elementType.IsInstanceOfType(o))
                    ((Array)result).SetValue(o, i);
                else
                    ((Array)result).SetValue(
                        converter.ConvertValue(context, null, null,
                            null, o, elementType),
                        i);
            }

            return result;
        } catch (TypeLoadException ex) {
            throw new OgnlException(
                "array component class '{TypeName}' not found", ex);
        }
    }

    public override string ToString()
    {
        var result = "new " + TypeName;

        if (IsArray)
            return Children[0] is AstConst ?
                result + "[" + Children[0] + "]" :
                result + "[] " + Children[0];

        result += "(";

        for (var i = 0; i < Children.Length; i++) {
            if (i > 0)
                result += ", ";

            result += Children[i];
        }

        return result + ")";
    }
}
