//--------------------------------------------------------------------------
//	Copyright (c) 1998-2004, Drew Davidson ,  Luke Blanshard and Foxcoming
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

namespace OGNL.Parser;

internal class AstChain(int id) : SimpleNode(id) {
    public override void Close()
    {
        FlattenTree();
    }

    protected override object? GetValueBody(OgnlContext context,
        object source)
    {
        var result = source;

        for (int i = 0, ilast = Children.Length - 1; i <= ilast; ++i) {
            var handled = false;

            if (i < ilast && Children[i] is AstProperty) {
                var propertyNode = (AstProperty)Children[i];
                var indexType = propertyNode.GetIndexedPropertyType(context, result);

                if (indexType != OgnlRuntime.IndexedPropertyNone && Children[i + 1] is AstProperty) {
                    var indexNode = (AstProperty)Children[i + 1];

                    if (indexNode.IndexedAccess) {
                        var index = indexNode.GetProperty(context);

                        if (index is DynamicSubscript) {
                            if (indexType == OgnlRuntime.IndexedPropertyInt) {
                                var array = propertyNode.GetValue(context, result);
                                var len = ((Array)array).Length;

                                switch (((DynamicSubscript)index).GetFlag()) {
                                    case DynamicSubscript.AllElements:
                                        result = Array.CreateInstance(array.GetType().GetElementType(), len);
                                        Array.Copy((Array)array, 0, (Array)result, 0, len);
                                        handled = true;
                                        i++;

                                        break;
                                    case DynamicSubscript.FirstElement:
                                        index = len > 0 ? 0 : -1;

                                        break;
                                    case DynamicSubscript.MidElement:
                                        index = len > 0 ? len / 2 : -1;

                                        break;
                                    case DynamicSubscript.LastElement:
                                        index = len > 0 ? len - 1 : -1;

                                        break;
                                }
                            } else {
                                if (indexType == OgnlRuntime.IndexedPropertyObject) {
                                    throw new OgnlException("DynamicSubscript '" + indexNode +
                                        "' not allowed for object indexed property '" +
                                        propertyNode + "'");
                                }
                            }
                        }

                        if (!handled) {
                            result = OgnlRuntime.GetIndexedProperty(
                                context, result,
                                propertyNode.GetProperty(context).ToString(),
                                index);
                            handled = true;
                            i++;
                        }
                    }
                }
            }

            if (!handled)
                result = Children[i].GetValue(context, result);
        }

        return result;
    }

    protected override void SetValueBody(OgnlContext context,
        object target, object? value)
    {
        var handled = false;

        for (int i = 0, ilast = Children.Length - 2; i <= ilast; ++i) {
            if (i == ilast) {
                if (Children[i] is AstProperty) {
                    var propertyNode = (AstProperty)Children[i];
                    var indexType = propertyNode.GetIndexedPropertyType(context, target);

                    if (indexType != OgnlRuntime.IndexedPropertyNone && Children[i + 1] is AstProperty) {
                        var indexNode = (AstProperty)Children[i + 1];

                        if (indexNode.IndexedAccess) {
                            var index = indexNode.GetProperty(context);

                            if (index is DynamicSubscript) {
                                if (indexType == OgnlRuntime.IndexedPropertyInt) {
                                    var array = propertyNode.GetValue(context, target);
                                    var len = ((Array)array).Length;

                                    switch (((DynamicSubscript)index).GetFlag()) {
                                        case DynamicSubscript.AllElements:
                                            Array.Copy((Array)target, 0, (Array)value, 0, len);
                                            handled = true;
                                            i++;

                                            break;
                                        case DynamicSubscript.FirstElement:
                                            index = len > 0 ? 0 : -1;

                                            break;
                                        case DynamicSubscript.MidElement:
                                            index = len > 0 ? len / 2 : -1;

                                            break;
                                        case DynamicSubscript.LastElement:
                                            index = len > 0 ? len - 1 : -1;

                                            break;
                                    }
                                } else {
                                    if (indexType == OgnlRuntime.IndexedPropertyObject) {
                                        throw new OgnlException("DynamicSubscript '" + indexNode +
                                            "' not allowed for object indexed property '" +
                                            propertyNode + "'");
                                    }
                                }
                            }

                            if (!handled) {
                                OgnlRuntime.SetIndexedProperty(context, target,
                                    propertyNode.GetProperty(context).ToString(), index, value);
                                handled = true;
                                i++;
                            }
                        }
                    }
                }
            }

            if (!handled)
                target = Children[i].GetValue(context, target);
        }

        if (!handled)
            Children[^1].SetValue(context, target, value);
    }

    public override bool IsSimpleNavigationChain(OgnlContext context)
    {
        if (Children.Length <= 0)
            return false;

        var result = true;

        for (var i = 0; result && i < Children.Length; i++)
            result = Children[i] is SimpleNode simpleNode &&
                simpleNode.IsSimpleProperty(context);

        return result;
    }

    public override string ToString()
    {
        return Children
            .Where((t, i) => i > 0 && t is not AstProperty { IndexedAccess: true })
            .Aggregate("", (current, t) => current + t);
    }
}
