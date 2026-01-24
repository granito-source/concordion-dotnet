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

namespace OGNL.Parser;

internal class AstMap(int id) : SimpleNode(id) {
    private static readonly Type DefaultMapClass = typeof(Hashtable);

    public string? TypeName { private get; set; }

    protected override object GetValueBody(OgnlContext context,
        object source)
    {
        IDictionary answer;

        if (TypeName == null)
            try {
                answer = (IDictionary)DefaultMapClass.GetConstructor([])
                    .Invoke([]);
            } catch (Exception ex) {
                /* This should never happen */
                throw new OgnlException(
                    $"Default {nameof(IDictionary)} class '{DefaultMapClass.Name}' instantiation error", ex);
            }
        else
            try {
                answer = (IDictionary)OgnlRuntime.TypeForName(context, TypeName)
                    .GetConstructor([])
                    .Invoke([]);
            } catch (Exception ex) {
                throw new OgnlException(
                    $"{nameof(IDictionary)} implementor '{TypeName}' not found", ex);
            }

        for (var i = 0; i < GetNumChildren(); ++i) {
            var kv = (AstKeyValue)Children[i];
            var k = kv.Key;
            var v = kv.Value;

            answer.Add(k.GetValue(context, source), v == null ? null :
                v.GetValue(context, source));
        }

        return answer;
    }

    public override string ToString()
    {
        var result = "#";

        if (TypeName != null)
            result = result + "@" + TypeName + "@";

        result += "{ ";

        for (var i = 0; i < GetNumChildren(); ++i) {
            var kv = (AstKeyValue)Children[i];

            if (i > 0)
                result += ", ";

            result = result + kv.Key + " : " + kv.Value;
        }

        return result + " }";
    }
}
