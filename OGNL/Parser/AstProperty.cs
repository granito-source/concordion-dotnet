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

internal class AstProperty(int id) : SimpleNode(id) {
    /**
     * True iff this property is itself an index reference.
     */
    public bool IndexedAccess { get; set; }

    /**
     * Returns true if this property is described by an
     * IndexedPropertyDescriptor and that if followed by an index
     * specifier it will call the index get/set methods rather than
     * go through property accessors.
     */
    public int GetIndexedPropertyType(OgnlContext context, object source)
    {
        if (!IndexedAccess) {
            var property = GetProperty(context);

            if (property is string stringProperty)
                return OgnlRuntime.GetIndexedPropertyType(context,
                    source == null ? null : source.GetType(),
                    stringProperty);
        }

        return OgnlRuntime.IndexedPropertyNone;
    }

    public object GetProperty(OgnlContext context)
    {
        return Children[0].GetValue(context, context.Root);
    }

    protected override object? GetValueBody(OgnlContext context,
        object source)
    {
        if (IndexedAccess && Children[0] is AstSequence) {
            // As property [index1, index2]...
            // Use Indexer.
            var indexParameters = ((AstSequence)Children[0])
                .GetValues(context, context.Root);

            return OgnlRuntime.GetIndexerValue(context, source, "Indexer",
                indexParameters);
        }

        var property = GetProperty(context);

        return OgnlRuntime.GetProperty(context, source, property) ??
            OgnlRuntime
                .GetNullHandler(OgnlRuntime.GetTargetType(source))
                .NullPropertyValue(context, source, property);
    }

    protected override void SetValueBody(OgnlContext context,
        object target, object? value)
    {
        if (IndexedAccess && Children[0] is AstSequence) {
            // As property [index1, index2]...
            // Use Indexer.
            var indexParameters = ((AstSequence)Children[0])
                .GetValues(context, context.Root);

            OgnlRuntime.SetIndexerValue(context, target, "Indexer",
                value, indexParameters);

            return;
        }

        OgnlRuntime.SetProperty(context, target, GetProperty(context), value);
    }

    protected override bool IsNodeSimpleProperty(OgnlContext context)
    {
        return Children.Length == 1 &&
            ((SimpleNode)Children[0]).IsConstant(context);
    }

    public override string ToString()
    {
        return IndexedAccess ? "[" + Children[0] + "]" :
            ((AstConst)Children[0]).Value.ToString();
    }
}
