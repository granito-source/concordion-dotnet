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

using System.Text.RegularExpressions;
using OGNL.Parser;

namespace OGNL;

public partial class ObjectPropertyAccessor : PropertyAccessor {
    [GeneratedRegex("^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex NameRegex();

    private static object? TryGettingProperty(OgnlContext context,
        object target, string name)
    {
        try {
            var result = OgnlRuntime.GetMethodValue(context, target,
                name, true);

            if (result == OgnlRuntime.NotFound)
                result = OgnlRuntime.GetFieldValue(context, target,
                    name, true);

            return result == OgnlRuntime.NotFound ?
                throw new NoSuchPropertyException(target, name) :
                result;
        } catch (OgnlException) {
            throw;
        } catch (Exception ex) {
            throw new OgnlException(name, ex);
        }
    }

    private static void TrySettingProperty(OgnlContext context,
        object target, string name, object? value)
    {
        try {
            if (!OgnlRuntime.SetMethodValue(context, target, name, value, true) &&
                !OgnlRuntime.SetFieldValue(context, target, name, value))
                throw new NoSuchPropertyException(target, name);
        } catch (OgnlException) {
            throw;
        } catch (Exception ex) {
            throw new OgnlException(name, ex);
        }
    }

    private static string FromObjectName(object name)
    {
        var plainName = name.ToString();

        return plainName ?? throw new OgnlException("property name is null");
    }

    private static bool IsPropertyName(string? name)
    {
        return name != null && NameRegex().IsMatch(name);
    }

    public virtual object? GetProperty(OgnlContext context, object target,
        object name)
    {
        var currentNode = context.CurrentNode;

        if (currentNode == null)
            throw new OgnlException("node is null for '" + name + "'");

        if (currentNode is not AstProperty)
            currentNode = currentNode.GetParent();

        var indexed = currentNode is AstProperty astProperty &&
            astProperty.IndexedAccess &&
            OgnlRuntime.HasGetIndexer(context, target, target.GetType(), 1);

        if (!indexed && name is string plainName && IsPropertyName(plainName))
            return TryGettingProperty(context, target, plainName);

        return OgnlRuntime.GetIndexerValue(context, target, name, [name]);
    }

    public virtual void SetProperty(OgnlContext context, object target,
        object name, object? value)
    {
        var plainName = FromObjectName(name);
        var currentNode = context.CurrentNode;

        if (currentNode == null)
            throw new OgnlException("node is null for '" + name + "'");

        if (currentNode is not AstProperty)
            currentNode = currentNode.GetParent();

        var indexed = currentNode is AstProperty astProperty &&
            astProperty.IndexedAccess &&
            OgnlRuntime.HasSetIndexer(context, target, target.GetType(), 1);

        if (indexed)
            OgnlRuntime.SetIndexerValue(context, target, name, value, [name]);
        else
            TrySettingProperty(context, target, plainName, value);
    }
}
