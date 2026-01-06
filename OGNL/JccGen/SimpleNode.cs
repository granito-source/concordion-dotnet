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

using System.Text;

namespace OGNL.JccGen;

/**
 * @author Luke Blanshard (blanshlu@netscape.net)
 * @author Drew Davidson (drew@ognl.org)
 */
public abstract class SimpleNode(int i) : Node {
    protected Node? parent;

    protected Node[]? children;

    protected int id = i;

    protected OgnlParser? parser;

    private bool constantValueCalculated;

    private bool hasConstantValue;

    private object? constantValue;

    public SimpleNode(OgnlParser p, int i) : this(i)
    {
        parser = p;
    }

    public virtual void jjtOpen()
    {
    }

    public virtual void jjtClose()
    {
    }

    public void jjtSetParent(Node n)
    {
        parent = n;
    }

    public Node? jjtGetParent()
    {
        return parent;
    }

    public void jjtAddChild(Node n, int i)
    {
        if (children == null)
            children = new Node[i + 1];
        else if (i >= children.Length) {
            var c = new Node[i + 1];

            Array.Copy(children, 0, c, 0, children.Length);
            children = c;
        }

        children[i] = n;
    }

    public Node jjtGetChild(int i)
    {
        return children[i];
    }

    public int jjtGetNumChildren()
    {
        return children?.Length ?? 0;
    }

    /* You can override these two methods in subclasses of SimpleNode to
       customize the way the node appears when the tree is dumped.  If
       your output uses more than one line you should override
       ToString(string), otherwise overriding ToString() is probably all
       you need to do. */

    public override string ToString()
    {
        return OgnlParserTreeConstants.jjtNodeName[id];
    }

// OGNL additions

    public string ToString(string prefix)
    {
        return prefix + OgnlParserTreeConstants.jjtNodeName[id] + " " + ToString();
    }

    /* Override this method if you want to customize how the node dumps
       out its children. */

    public void dump(TextWriter writer, string prefix)
    {
        writer.WriteLine(ToString(prefix));

        if (children != null) {
            for (var i = 0; i < children.Length; ++i) {
                var n = (SimpleNode)children[i];

                if (n != null) {
                    n.dump(writer, prefix + "  ");
                }
            }
        }
    }

    public int getIndexInParent()
    {
        var result = -1;

        if (parent != null) {
            var icount = parent.jjtGetNumChildren();

            for (var i = 0; i < icount; i++) {
                if (parent.jjtGetChild(i) == this) {
                    result = i;

                    break;
                }
            }
        }

        return result;
    }

    public Node getNextSibling()
    {
        Node result = null;
        var i = getIndexInParent();

        if (i >= 0) {
            var icount = parent.jjtGetNumChildren();

            if (i < icount) {
                result = parent.jjtGetChild(i + 1);
            }
        }

        return result;
    }

    private static string getDepthString(int depth)
    {
        var result = new StringBuilder("");

        while (depth > 0) {
            depth--;
            result.Append("  ");
        }

        return result.ToString();
    }

    protected object evaluateGetValueBody(OgnlContext context, object source)
    {
        object result;

        context.setCurrentObject(source);
        context.setCurrentNode(this);

        if (!constantValueCalculated) {
            constantValueCalculated = true;
            hasConstantValue = isConstant(context);

            if (hasConstantValue) {
                constantValue = getValueBody(context, source);
            }
        }

        return hasConstantValue ? constantValue : getValueBody(context, source);
    }

    protected void evaluateSetValueBody(OgnlContext context, object target, object value)
    {
        context.setCurrentObject(target);
        context.setCurrentNode(this);
        setValueBody(context, target, value);
    }

    public object getValue(OgnlContext context, object source)
    {
        if (context.getTraceEvaluations()) {
            var pool = OgnlRuntime.getEvaluationPool();
            object result = null;
            Exception evalException = null;
            var evaluation = pool.create(this, source);

            context.pushEvaluation(evaluation);

            try {
                result = evaluateGetValueBody(context, source);
            } catch (OgnlException ex) {
                evalException = ex;

                throw;
            } catch (Exception ex) {
                evalException = ex;

                throw;
            } finally {
                var eval = context.popEvaluation();

                eval.setResult(result);

                if (evalException != null) {
                    eval.setException(evalException);
                }

                if ((evalException == null) && (context.getRootEvaluation() == null) &&
                    !context.getKeepLastEvaluation()) {
                    pool.recycleAll(eval);
                }
            }

            return result;
        }

        return evaluateGetValueBody(context, source);
    }

    /** Subclasses implement this method to do the actual work of extracting the
        appropriate value from the source object. */
    protected abstract object? getValueBody(OgnlContext context,
        object source);

    public void setValue(OgnlContext context, object target, object? value)
    {
        if (context.getTraceEvaluations()) {
            var pool = OgnlRuntime.getEvaluationPool();
            Exception evalException = null;
            var evaluation = pool.create(this, target, true);

            context.pushEvaluation(evaluation);

            try {
                evaluateSetValueBody(context, target, value);
            } catch (OgnlException ex) {
                evalException = ex;
                ex.setEvaluation(evaluation);

                throw;
            } catch (Exception ex) {
                evalException = ex;

                throw;
            } finally {
                var eval = context.popEvaluation();

                if (evalException != null) {
                    eval.setException(evalException);
                }

                if ((evalException == null) && (context.getRootEvaluation() == null) &&
                    !context.getKeepLastEvaluation()) {
                    pool.recycleAll(eval);
                }
            }
        } else {
            evaluateSetValueBody(context, target, value);
        }
    }

    /** Subclasses implement this method to do the actual work of setting the
        appropriate value in the target object.  The default implementation
        // throws an <code>InappropriateExpressionException</code>, meaning that it
        cannot be a set expression.
     */
    protected virtual void setValueBody(OgnlContext context,
        object target, object value)
    {
        throw new InappropriateExpressionException(this);
    }

    /**
        Returns true iff this node is constant without respect to the children.
     */
    public virtual bool isNodeConstant(OgnlContext context)
    {
        return false;
    }

    public virtual bool isConstant(OgnlContext context)
    {
        return isNodeConstant(context);
    }

    public virtual bool isNodeSimpleProperty(OgnlContext context)
    {
        return false;
    }

    public virtual bool isSimpleProperty(OgnlContext context)
    {
        return isNodeSimpleProperty(context);
    }

    public virtual bool isSimpleNavigationChain(OgnlContext context)
    {
        return isSimpleProperty(context);
    }

    /** This method may be called from subclasses' jjtClose methods.  It flattens the
        tree under this node by eliminating any children that are of the same class as
        this node and copying their children to this node. */
    protected virtual void flattenTree()
    {
        var shouldFlatten = false;
        var newSize = 0;

        for (var i = 0; i < children.Length; ++i)
            if (children[i].GetType() == GetType()) {
                shouldFlatten = true;
                newSize += children[i].jjtGetNumChildren();
            } else
                ++newSize;

        if (shouldFlatten) {
            var newChildren = new Node[newSize];
            var j = 0;

            for (var i = 0; i < children.Length; ++i) {
                var c = children[i];

                if (c.GetType() == GetType())
                    for (var k = 0; k < c.jjtGetNumChildren(); ++k)
                        newChildren[j++] = c.jjtGetChild(k);
                else
                    newChildren[j++] = c;
            }

            if (j != newSize)
                throw new Exception("Assertion error: " + j + " != " + newSize);

            children = newChildren;
        }
    }
}
