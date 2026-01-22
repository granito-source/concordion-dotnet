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

using System.Diagnostics;

namespace OGNL.Parser;

internal class AstMethod(int id) : SimpleNode(id) {
    public string? MethodName { private get; set; }

    protected override object? GetValueBody(OgnlContext context,
        object source)
    {
        Debug.Assert(MethodName != null, nameof(MethodName) + " != null");

        var args = OgnlRuntime.ObjectArrayPool.Create(GetNumChildren());

        try {
            var root = context.Root;

            for (var i = 0; i < args.Length; ++i)
                args[i] = Children[i].GetValue(context, root);

            return OgnlRuntime.CallMethod(context, source, MethodName, null, args) ??
                OgnlRuntime.GetNullHandler(OgnlRuntime.GetTargetType(source))
                    .NullMethodResult(context, source, MethodName, args);
        } finally {
            OgnlRuntime.ObjectArrayPool.Recycle(args);
        }
    }

    public override string ToString()
    {
        var result = MethodName;

        result += "(";

        for (var i = 0; i < Children.Length; i++) {
            if (i > 0)
                result += ", ";

            result += Children[i];
        }

        result += ")";

        return result;
    }
}
