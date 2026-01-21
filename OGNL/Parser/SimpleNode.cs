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

    public Node? GetChild(int i)
    {
        return Children[i];
    }

    public int GetNumChildren()
    {
        return Children.Length;
    }

    /* You can override these two methods in subclasses of SimpleNode to
       customize the way the node appears when the tree is dumped.  If
       your output uses more than one line you should override
       ToString(string), otherwise overriding ToString() is probably all
       you need to do. */
    public override string ToString()
    {
        return ParserTreeConstants.JjtNodeName[Id];
    }

    // OGNL additions

    private string ToString(string prefix)
    {
        return prefix + ParserTreeConstants.JjtNodeName[Id] + " " +
            ToString();
    }

    /* Override this method if you want to customize how the node dumps
       out its children. */
    public void Dump(TextWriter writer, string prefix)
    {
        writer.WriteLine(ToString(prefix));

        foreach (var child in Children)
            if (child is SimpleNode simpleNode)
                simpleNode.Dump(writer, prefix + "  ");
    }

    private object EvaluateGetValueBody(OgnlContext context, object source)
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
        object value)
    {
        context.CurrentObject = target;
        context.CurrentNode = this;
        SetValueBody(context, target, value);
    }

    public object? GetValue(OgnlContext context, object source)
    {
        if (!context.TraceEvaluations)
            return EvaluateGetValueBody(context, source);

        var pool = OgnlRuntime.EvaluationPool;
        object? result = null;
        Exception? evalException = null;
        var evaluation = pool.create(this, source);

        context.PushEvaluation(evaluation);

        try {
            return EvaluateGetValueBody(context, source);
        } catch (OgnlException ex) {
            evalException = ex;
            ex.setEvaluation(evaluation);

            throw;
        } catch (Exception ex) {
            evalException = ex;

            throw;
        } finally {
            var eval = context.PopEvaluation();

            eval.setResult(result);

            if (evalException != null)
                eval.setException(evalException);

            if (evalException == null && context.RootEvaluation == null &&
                !context.KeepLastEvaluation)
                pool.recycleAll(eval);
        }
    }

    /**
     * Subclasses implement this method to do the actual work of
     * extracting the appropriate value from the source object.
     */
    protected abstract object? GetValueBody(OgnlContext context,
        object source);

    public void SetValue(OgnlContext context, object target, object? value)
    {
        if (context.TraceEvaluations) {
            var pool = OgnlRuntime.EvaluationPool;
            Exception evalException = null;
            var evaluation = pool.create(this, target, true);

            context.PushEvaluation(evaluation);

            try {
                EvaluateSetValueBody(context, target, value);
            } catch (OgnlException ex) {
                evalException = ex;
                ex.setEvaluation(evaluation);

                throw;
            } catch (Exception ex) {
                evalException = ex;

                throw;
            } finally {
                var eval = context.PopEvaluation();

                if (evalException != null) {
                    eval.setException(evalException);
                }

                if (evalException == null && context.RootEvaluation == null &&
                    !context.KeepLastEvaluation) {
                    pool.recycleAll(eval);
                }
            }
        } else {
            EvaluateSetValueBody(context, target, value);
        }
    }

    /** Subclasses implement this method to do the actual work of setting the
        appropriate value in the target object.  The default implementation
        // throws an <code>InappropriateExpressionException</code>, meaning that it
        cannot be a set expression.
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

    public virtual bool IsConstant(OgnlContext context)
    {
        return IsNodeConstant(context);
    }

    protected virtual bool IsNodeSimpleProperty(OgnlContext context)
    {
        return false;
    }

    public bool IsSimpleProperty(OgnlContext context)
    {
        return IsNodeSimpleProperty(context);
    }

    public virtual bool IsSimpleNavigationChain(OgnlContext context)
    {
        return IsSimpleProperty(context);
    }

    /** This method may be called from subclasses' jjtClose methods.  It flattens the
        tree under this node by eliminating any children that are of the same class as
        this node and copying their children to this node. */
    protected void FlattenTree()
    {
        var shouldFlatten = false;
        var newSize = 0;

        for (var i = 0; i < Children.Length; ++i)
            if (Children[i].GetType() == GetType()) {
                shouldFlatten = true;
                newSize += Children[i].GetNumChildren();
            } else
                ++newSize;

        if (shouldFlatten) {
            var newChildren = new Node[newSize];
            var j = 0;

            for (var i = 0; i < Children.Length; ++i) {
                var c = Children[i];

                if (c.GetType() == GetType())
                    for (var k = 0; k < c.GetNumChildren(); ++k)
                        newChildren[j++] = c.GetChild(k);
                else
                    newChildren[j++] = c;
            }

            if (j != newSize)
                throw new Exception("Assertion error: " + j + " != " + newSize);

            Children = newChildren;
        }
    }
}
