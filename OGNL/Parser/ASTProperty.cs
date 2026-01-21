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

/**
 * @author Luke Blanshard (blanshlu@netscape.net)
 * @author Drew Davidson (drew@ognl.org)
 */
internal class ASTProperty(int id) : SimpleNode(id) {
    private bool indexedAccess = false;

    public void setIndexedAccess(bool value)
    {
        indexedAccess = value;
    }

    /**
        Returns true iff this property is itself an index reference.
     */
    public bool isIndexedAccess()
    {
        return indexedAccess;
    }

    /**
        Returns true if this property is described by an IndexedPropertyDescriptor
        and that if followed by an index specifier it will call the index get/set
        methods rather than go through property accessors.
     */
    public int getIndexedPropertyType(OgnlContext context, object source)
    {
        if (!isIndexedAccess()) {
            var property = getProperty(context, source);

            if (property is string) {
                return OgnlRuntime.GetIndexedPropertyType(context, source == null ? null : source.GetType(),
                    (string)property);
            }
        }

        return OgnlRuntime.IndexedPropertyNone;
    }

    public object getProperty(OgnlContext context, object source)
    {
        return Children[0].GetValue(context, context.Root);
    }

    protected override object? GetValueBody(OgnlContext context,
        object source)
    {
        if (indexedAccess && Children[0] is ASTSequence) {
            // As property [index1, index2]...
            // Use Indexer.
            var indexParameters = ((ASTSequence)Children[0]).getValues(context, context.Root);

            /* return IndexerAccessor.getIndexerValue (source, indexParameters) ; */
            return OgnlRuntime.GetIndexerValue(context, source, "Indexer", indexParameters);
        }

        object? result;

        var property = getProperty(context, source);
        Node indexSibling;

        result = OgnlRuntime.GetProperty(context, source, property);

        if (result == null) {
            result = OgnlRuntime.GetNullHandler(OgnlRuntime.GetTargetClass(source))
                .nullPropertyValue(context, source, property);
        }

        return result;
    }

    protected override void SetValueBody(OgnlContext context,
        object target, object? value)
    {
        if (indexedAccess && Children[0] is ASTSequence) {
            // As property [index1, index2]...
            // Use Indexer.
            var indexParameters = ((ASTSequence)Children[0]).getValues(context, context.Root);

            /*IndexerAccessor.setIndexerValue (target, value ,indexParameters) ;*/
            OgnlRuntime.SetIndexerValue(context, target, "Indexer", value, indexParameters);

            return;
        }

        OgnlRuntime.SetProperty(context, target, getProperty(context, target), value);
    }

    protected override bool IsNodeSimpleProperty(OgnlContext context)
    {
        return Children != null && Children.Length == 1 &&
            ((SimpleNode)Children[0]).IsConstant(context);
    }

    public override string ToString()
    {
        string result;

        if (isIndexedAccess()) {
            result = "[" + Children[0] + "]";
        } else {
            result = ((ASTConst)Children[0]).getValue().ToString();
        }

        return result;
    }
}
