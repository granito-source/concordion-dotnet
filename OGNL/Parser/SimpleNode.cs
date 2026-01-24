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

public abstract class SimpleNode(int id) : Node {
    protected Node[] Children = [];

    protected readonly int Id = id;

    protected Node? Parent;

    private bool constantValueCalculated;

    private bool hasConstantValue;

    private object? constantValue;

    public virtual void Open()
    {
    }

    public virtual void Close()
    {
    }

    public void SetParent(Node node)
    {
        Parent = node;
    }

    public Node? GetParent()
    {
        return Parent;
    }

    public void AddChild(Node node, int i)
    {
        if (i >= Children.Length) {
            var children = new Node[i + 1];

            Array.Copy(Children, 0, children, 0, Children.Length);
            Children = children;
        }

        Children[i] = node;
    }

    public Node GetChild(int i)
    {
        return Children[i];
    }

    public int GetNumChildren()
    {
        return Children.Length;
    }

    public object? GetValue(OgnlContext context, object source)
    {
        return Evaluate(context, source,
            () => EvaluateGetValueBody(context, source));
    }

    public void SetValue(OgnlContext context, object target, object? value)
    {
        Evaluate(context, target, () => {
            EvaluateSetValueBody(context, target, value);

            return null;
        });
    }

    public override string ToString()
    {
        return ParserTreeConstants.JjtNodeName[Id];
    }

    public virtual bool IsConstant(OgnlContext context)
    {
        return IsNodeConstant(context);
    }

    public virtual bool IsSimpleNavigationChain(OgnlContext context)
    {
        return IsSimpleProperty(context);
    }

    public bool IsSimpleProperty(OgnlContext context)
    {
        return IsNodeSimpleProperty(context);
    }

    /**
     * Subclasses implement this method to do the actual work of
     * extracting the appropriate value from the source object.
     */
    protected abstract object? GetValueBody(OgnlContext context,
        object source);

    /**
     * Subclasses implement this method to do the actual work of setting
     * the appropriate value in the target object. The default
     * implementation throws an
     * <code>InappropriateExpressionException</code>, meaning that it
     * cannot be a set expression.
     */
    protected virtual void SetValueBody(OgnlContext context,
        object target, object? value)
    {
        throw new InappropriateExpressionException(this);
    }

    /**
     * Returns true iff this node is constant without respect to
     * the children.
     */
    protected virtual bool IsNodeConstant(OgnlContext context)
    {
        return false;
    }

    protected virtual bool IsNodeSimpleProperty(OgnlContext context)
    {
        return false;
    }

    /**
     * This method may be called from subclasses' jjtClose methods.
     * It flattens the tree under this node by eliminating any children
     * that are of the same class as this node and copying their children
     * to this node.
     * */
    protected void FlattenTree()
    {
        var shouldFlatten = false;
        var newSize = 0;

        foreach (var node in Children)
            if (node.GetType() == GetType()) {
                shouldFlatten = true;
                newSize += node.GetNumChildren();
            } else
                ++newSize;

        if (!shouldFlatten)
            return;

        var newChildren = new Node[newSize];
        var j = 0;

        foreach (var node in Children) {
            if (node.GetType() == GetType())
                for (var k = 0; k < node.GetNumChildren(); ++k)
                    newChildren[j++] = node.GetChild(k);
            else
                newChildren[j++] = node;
        }

        if (j != newSize)
            throw new Exception("Assertion error: " + j + " != " + newSize);

        Children = newChildren;
    }

    private object? EvaluateGetValueBody(OgnlContext context, object source)
    {
        context.CurrentObject = source;
        context.CurrentNode = this;

        if (!constantValueCalculated) {
            constantValueCalculated = true;
            hasConstantValue = IsConstant(context);

            if (hasConstantValue)
                constantValue = GetValueBody(context, source);
        }

        return hasConstantValue ? constantValue : GetValueBody(context, source);
    }

    private void EvaluateSetValueBody(OgnlContext context, object target,
        object? value)
    {
        context.CurrentObject = target;
        context.CurrentNode = this;
        SetValueBody(context, target, value);
    }

    private object? Evaluate(OgnlContext context, object obj,
        Func<object?> func)
    {
        if (!context.TraceEvaluations)
            return func();

        context.PushEvaluation(new Evaluation(this, obj));

        object? result = null;
        Exception? exception = null;

        try {
            result = func();
        } catch (Exception ex) {
            exception = ex;
        } finally {
            var evaluation = context.PopEvaluation();

            evaluation.Result = result;
            evaluation.Exception = exception;
        }

        return result;
    }
}
