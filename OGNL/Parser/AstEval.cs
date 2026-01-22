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

internal class AstEval(int id) : SimpleNode(id) {
    protected override object? GetValueBody(OgnlContext context,
        object source)
    {
        return Delegate(context, source,
            (node, obj) => node.GetValue(context, obj));
    }

    protected override void SetValueBody(OgnlContext context,
        object target, object? value)
    {
        Delegate(context, target, (node, obj) => {
            node.SetValue(context, obj, value);

            return 0;
        });
    }

    public override string ToString()
    {
        return "(" + Children[0] + ")(" + Children[1] + ")";
    }

    private T Delegate<T>(OgnlContext context, object obj,
        Func<Node, object, T> func)
    {
        var expr = Children[0].GetValue(context, obj);
        var node = expr as Node ??
            Ognl.ParseExpression(expr?.ToString() ?? string.Empty);
        var evalRoot = Children[1].GetValue(context, obj);

        Debug.Assert(evalRoot != null, nameof(evalRoot) + " != null");

        var savedRoot = context.Root;

        try {
            context.Root = evalRoot;

            return func(node, evalRoot);
        } finally {
            context.Root = savedRoot;
        }
    }
}
