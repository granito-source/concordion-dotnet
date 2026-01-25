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

using System.Collections;
using OGNL.Parser;

namespace OGNL;

///<summary>
///This class provides static methods for parsing and interpreting OGNL expressions.
///</summary>
///<example>
///The simplest use of the Ognl class is to get the value of an expression from
///an object, without extra context or pre-parsing.
///
///<code lang="C#">
///using ognl;
///
///  try
///  {
///      result = Ognl.getValue(expression, root);
///  } catch (OgnlException ex)
///  {
///     // Report error or recover
///  }
///</code>
///
///This will parse the expression given and evaluate it against the root object
///given, returning the result.  If there is an error in the expression, such
///as the property is not found, the exception is encapsulated into an
///<see href="OgnlException"/>.
///
///<para>Other more sophisticated uses of Ognl can pre-parse expressions.  This
///provides two advantages: in the case of user-supplied expressions it
///allows you to catch parse errors before evaluation, and it allows you to
///cache parsed expressions into an AST for better speed during repeated use.
///The pre-parsed expression is always returned as an <c>object</c>
///to simplify use for programs that just wish to store the value for
///repeated use and do not care that it is an AST.  If it does care
///it can always safely cast the value to an <c>AST</c> type.</para>
///
///<para>The Ognl class also takes a <I>context map</I> as one of the parameters
///to the set and get methods.  This allows you to put your own variables
///into the available namespace for OGNL expressions.  The default context
///contains only the <c>#root</c> and <c>#context</c> keys,
///which are required to be present.  The <c>addDefaultContext(object, IDictionary)</c>
///method will alter an existing <c>IDictionary</c> to put the defaults in.
///Here is an example that shows how to extract the <c>documentName</c>
///property out of the root object and append a string with the current user
///name in parens:</para>
///
///<code lang="C#">
///    private IDictionary context = new HashMap();
///
///    public void setUserName(string value)
///    {
///        context.put("userName", value);
///    }
///
///    try
///    {
///       // get value using our own custom context map
///       result = Ognl.getValue("documentName + \" (\" + ((#userName == null) ? \"&lt;nobody&gt;\" : #userName) + \")\"", context, root);
///    } catch (OgnlException ex)
///    {
///        // Report error or recover
///    }
///
///</code>
///</example>
///
///@author Luke Blanshard (blanshlu@netscape.net)
///@author Drew Davidson (drew@ognl.org)
///@version 27 June 1999
///
public static class Ognl {
    ///<summary>
    ///Parses the given OGNL expression and returns a tree representation of the
    ///expression that can be used by <c>Ognl</c> static methods.
    ///</summary>
    ///<param name="expression">the OGNL expression to be parsed</param>
    ///<returns>a tree representation of the expression</returns>
    ///<exception cref="ExpressionSyntaxException">if the expression is malformed</exception>
    ///<exception cref="OgnlException"> if there is a pathological environmental problem</exception>
    ///
    public static Node ParseExpression(string expression)
    {
        try {
            return new Parser.Parser(new StringReader(expression))
                .TopLevelExpression();
        } catch (ParseException e) {
            throw new ExpressionSyntaxException(expression, e);
        } catch (TokenMgrError e) {
            throw new ExpressionSyntaxException(expression, e);
        }
    }

    ///<summary>
    ///Creates and returns a new standard naming context for evaluating an OGNL
    ///expression.
    ///</summary>
    ///<param name="root">the root of the object graph</param>
    ///<returns>
    ///a new IDictionary with the keys <c>root</c> and <c>context</c>
    ///set appropriately</returns>
    ///
    public static OgnlContext CreateDefaultContext(object root)
    {
        return AddDefaultContext(root, null, null, null, new OgnlContext());
    }

    ///<summary>
    ///Appends the standard naming context for evaluating an OGNL expression
    ///into the context given so that cached maps can be used as a context.
    ///</summary>
    ///<param name="root"> the root of the object graph</param>
    ///<param name="context"> the context to which OGNL context will be added.</param>
    ///<returns>
    ///a new IDictionary with the keys <c>root</c> and <c>context</c>
    ///set appropriately
    ///</returns>
    private static OgnlContext AddDefaultContext(object root,
        IDictionary context)
    {
        return AddDefaultContext(root, null, null, null, context);
    }

    /// <summary>
    /// Appends the standard naming context for evaluating an OGNL expression
    /// into the context given so that cached maps can be used as a context.
    /// </summary>
    /// <param name="root"> the root of the object graph</param>
    /// <param name="memberAccess"></param>
    /// <param name="context"> the context to which OGNL context will be added.</param>
    /// <param name="typeResolver"></param>
    /// <param name="converter"></param>
    /// <returns>
    /// a new IDictionary with the keys <c>root</c> and <c>context</c>
    /// set appropriately
    /// </returns>
    private static OgnlContext AddDefaultContext(object root,
        TypeResolver? typeResolver, TypeConverter? converter,
        MemberAccess? memberAccess, IDictionary context)
    {
        OgnlContext result;

        if (context is OgnlContext ognlContext)
            result = ognlContext;
        else {
            result = new OgnlContext();
            result.SetAllValues(context);
        }

        if (typeResolver != null)
            result.TypeResolver = typeResolver;

        if (converter != null)
            result.TypeConverter = converter;

        if (memberAccess != null)
            result.MemberAccess = memberAccess;

        result.Root = root;

        return result;
    }

    private static TypeConverter? GetTypeConverter(IDictionary context)
    {
        return (TypeConverter?)context[OgnlContext.TypeConverterKey];
    }

    ///<summary>
    ///Evaluates the given OGNL expression tree to extract a value from the given root
    ///object. The default context is set for the given context and root via
    ///<c>addDefaultContext()</c>.
    ///</summary>
    ///<param name="tree"> the OGNL expression tree to evaluate, as returned by parseExpression()</param>
    ///<param name="context"> the naming context for the evaluation</param>
    ///<param name="root"> the root object for the OGNL expression</param>
    ///<returns>the result of evaluating the expression</returns>
    ///<exception cref="MethodFailedException"> if the expression called a method which failed</exception>
    ///<exception cref="NoSuchPropertyException"> if the expression referred to a nonexistent property</exception>
    ///<exception cref="InappropriateExpressionException"> if the expression can't be used in this context</exception>
    ///<exception cref="OgnlException"> if there is a pathological environmental problem</exception>
    ///
    public static object? GetValue(Node tree, IDictionary context,
        object root)
    {
        return GetValue(tree, context, root, null);
    }

    ///<summary>
    ///Evaluates the given OGNL expression tree to extract a value from the given root
    ///object. The default context is set for the given context and root via
    ///<c>addDefaultContext()</c>.
    ///</summary>
    ///<param name="tree"> the OGNL expression tree to evaluate, as returned by parseExpression()</param>
    ///<param name="context"> the naming context for the evaluation</param>
    ///<param name="root"> the root object for the OGNL expression</param>
    ///<param name="resultType"></param>
    ///<returns>the result of evaluating the expression</returns>
    ///<exception cref="MethodFailedException"> if the expression called a method which failed</exception>
    ///<exception cref="NoSuchPropertyException"> if the expression referred to a nonexistent property</exception>
    ///<exception cref="InappropriateExpressionException"> if the expression can't be used in this context</exception>
    ///<exception cref="OgnlException"> if there is a pathological environmental problem</exception>
    ///
    private static object? GetValue(Node tree, IDictionary context,
        object root, Type? resultType)
    {
        var ognlContext = AddDefaultContext(root, context);
        var result = tree.GetValue(ognlContext, root);

        return resultType != null ?
            GetTypeConverter(context)?.ConvertValue(context, root, null,
                null, result, resultType) :
            result;
    }

    ///<summary>
    ///Evaluates the given OGNL expression to extract a value from the given root
    ///object in a given context
    ///</summary>
    ///<param name="expression"> the OGNL expression</param>
    ///<param name="context"> the naming context for the evaluation</param>
    ///<param name="root"> the root object for the OGNL expression</param>
    ///<returns>the result of evaluating the expression</returns>
    ///<exception cref="MethodFailedException"> if the expression called a method which failed</exception>
    ///<exception cref="NoSuchPropertyException"> if the expression referred to a nonexistent property</exception>
    ///<exception cref="InappropriateExpressionException"> if the expression can't be used in this context</exception>
    ///<exception cref="OgnlException"> if there is a pathological environmental problem</exception>
    ///
    public static object? GetValue(string expression, IDictionary context,
        object root)
    {
        return GetValue(expression, context, root, null);
    }

    /// <summary>
    /// Evaluates the given OGNL expression to extract a value from the given root
    /// object in a given context
    /// </summary>
    /// <param name="expression"> the OGNL expression</param>
    /// <param name="context"> the naming context for the evaluation</param>
    /// <param name="root"> the root object for the OGNL expression</param>
    /// <param name="resultType"></param>
    /// <returns>the result of evaluating the expression</returns>
    /// <exception cref="MethodFailedException"> if the expression called a method which failed</exception>
    /// <exception cref="NoSuchPropertyException"> if the expression referred to a nonexistent property</exception>
    /// <exception cref="InappropriateExpressionException"> if the expression can't be used in this context</exception>
    /// <exception cref="OgnlException"> if there is a pathological environmental problem</exception>
    ///
    private static object? GetValue(string expression, IDictionary context,
        object root, Type? resultType)
    {
        return GetValue(ParseExpression(expression), context, root,
            resultType);
    }

    ///
    ///Evaluates the given OGNL expression tree to insert a value into the object graph
    ///rooted at the given root object.  The default context is set for the given
    ///context and root via <CODE>addDefaultContext()</CODE>.
    ///
    ///@param tree the OGNL expression tree to evaluate, as returned by parseExpression()
    ///@param context the naming context for the evaluation
    ///@param root the root object for the OGNL expression
    ///@param value the value to insert into the object graph
    ///@// throws MethodFailedException if the expression called a method which failed
    ///@// throws NoSuchPropertyException if the expression referred to a nonexistent property
    ///@// throws InappropriateExpressionException if the expression can't be used in this context
    ///@// throws OgnlException if there is a pathological environmental problem
    ///
    public static void SetValue(Node tree, IDictionary context,
        object root, object? value)
    {
        var ognlContext = AddDefaultContext(root, context);

        tree.SetValue(ognlContext, root, value);
    }

    ///
    ///Evaluates the given OGNL expression to insert a value into the object graph
    ///rooted at the given root object given the context.
    ///
    ///@param expression the OGNL expression to be parsed
    ///@param root the root object for the OGNL expression
    ///@param context the naming context for the evaluation
    ///@param value the value to insert into the object graph
    ///@// throws MethodFailedException if the expression called a method which failed
    ///@// throws NoSuchPropertyException if the expression referred to a nonexistent property
    ///@// throws InappropriateExpressionException if the expression can't be used in this context
    ///@// throws OgnlException if there is a pathological environmental problem
    ///
    public static void SetValue(string expression, IDictionary context,
        object root, object? value)
    {
        SetValue(ParseExpression(expression), context, root, value);
    }

    private static bool IsConstant(Node tree, OgnlContext context)
    {
        return tree.IsConstant(AddDefaultContext(string.Empty, context));
    }

    public static bool IsConstant(Node tree)
    {
        return IsConstant(tree, CreateDefaultContext(string.Empty));
    }

    public static bool IsSimpleProperty(Node tree, OgnlContext context)
    {
        return tree
            .IsSimpleProperty(AddDefaultContext(string.Empty, context));
    }

    public static bool IsSimpleNavigationChain(Node tree,
        OgnlContext context)
    {
        return tree.IsSimpleNavigationChain(
            AddDefaultContext(string.Empty, context));
    }
}
