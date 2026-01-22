//--------------------------------------------------------------------------
//	Copyright (c) 1998-2004, Drew Davidson, Luke Blanshard and Foxcoming
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

using System.Diagnostics;

namespace OGNL.Parser;

internal class AstStaticField(int id) : SimpleNode(id) {
    public string? TypeName { private get; set; }

    public string? FieldName { private get; set; }

    protected override object GetValueBody(OgnlContext context,
        object source)
    {
        Debug.Assert(TypeName != null, nameof(TypeName) + " != null");
        Debug.Assert(FieldName != null, nameof(FieldName) + " != null");

        return OgnlRuntime.GetStaticField(context, TypeName, FieldName);
    }

    protected override bool IsNodeConstant(OgnlContext context)
    {
        Debug.Assert(TypeName != null, nameof(TypeName) + " != null");
        Debug.Assert(FieldName != null, nameof(FieldName) + " != null");

        try {
            var type = OgnlRuntime.TypeForName(context, TypeName);

            /*
             * Check for virtual static field "class"; this cannot
             * interfere with normal static fields because it is a
             * reserved word. It is considered constant.
             */
            if (FieldName.Equals("class"))
                return true;

            var field = type.GetField(FieldName);

            if (field == null) {
                var property = type.GetProperty(FieldName);

                if (property == null)
                    throw new MissingFieldException(
                        $"Field or Property {FieldName} of type {TypeName} is not found.");

                return property.GetAccessors()[0].IsStatic
                    ? false
                    : throw new MissingFieldException(
                        $"Property {FieldName} of type {TypeName} is not static.");
            }

            if (!field.IsStatic)
                throw new OgnlException(
                    $"Field {FieldName} of type {TypeName} is not static");

            return field.IsLiteral;
        } catch (OgnlException) {
            throw;
        } catch (Exception ex) {
            throw new OgnlException(
                $"Could not get static field {FieldName} from class {TypeName}",
                ex);
        }
    }

    public override string ToString()
    {
        return $"@{TypeName}@{FieldName}";
    }
}
