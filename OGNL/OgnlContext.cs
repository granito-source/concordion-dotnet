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
using OGNL.JccGen;

namespace OGNL;

/// <summary>
///This class defines the execution context for an OGNL expression
///</summary>
///@author Luke Blanshard (blanshlu@netscape.net)
///@author Drew Davidson (drew@ognl.org)
///
public class OgnlContext : IDictionary {
    public static string CONTEXT_CONTEXT_KEY = "context";

    public static string ROOT_CONTEXT_KEY = "root";

    public static string THIS_CONTEXT_KEY = "this";

    public static string TRACE_EVALUATIONS_CONTEXT_KEY = "_traceEvaluations";

    public static string LAST_EVALUATION_CONTEXT_KEY = "_lastEvaluation";

    public static string KEEP_LAST_EVALUATION_CONTEXT_KEY = "_keepLastEvaluation";

    public static string CLASS_RESOLVER_CONTEXT_KEY = "_classResolver";

    public static string TYPE_CONVERTER_CONTEXT_KEY = "_typeConverter";

    public static string MEMBER_ACCESS_CONTEXT_KEY = "_memberAccess";

    static string PROPERTY_KEY_PREFIX = "ognl";

    static bool DEFAULT_TRACE_EVALUATIONS = false;

    static bool DEFAULT_KEEP_LAST_EVALUATION = false;

    public static ClassResolver DEFAULT_CLASS_RESOLVER = new DefaultClassResolver();

    public static TypeConverter DEFAULT_TYPE_CONVERTER = new DefaultTypeConverter();

    public static MemberAccess DEFAULT_MEMBER_ACCESS = new DefaultMemberAccess(false);

    private object? root;

    private object? currentObject;

    private Node? currentNode;

    private bool traceEvaluations = DEFAULT_TRACE_EVALUATIONS;

    private Evaluation? rootEvaluation;

    private Evaluation? currentEvaluation;

    private Evaluation? lastEvaluation;

    private bool keepLastEvaluation = DEFAULT_KEEP_LAST_EVALUATION;

    private readonly IDictionary values = new Hashtable(23);

    private ClassResolver classResolver = DEFAULT_CLASS_RESOLVER;

    private TypeConverter typeConverter = DEFAULT_TYPE_CONVERTER;

    private MemberAccess memberAccess = DEFAULT_MEMBER_ACCESS;

    ///
    ///Constructs a new OgnlContext with the default class resolver, type converter and
    ///member access.
    ///
    public OgnlContext()
    {
    }

    ///<summary>
    /// Constructs a new OgnlContext with the given class resolver, type converter and
    /// member access.  If any of these parameters is null the default will be used.
    /// </summary>
    ///
    public OgnlContext(ClassResolver? classResolver,
        TypeConverter? typeConverter, MemberAccess? memberAccess)
    {
        if (classResolver != null)
            this.classResolver = classResolver;

        if (typeConverter != null)
            this.typeConverter = typeConverter;

        if (memberAccess != null)
            this.memberAccess = memberAccess;
    }

    public OgnlContext(IDictionary values)
    {
        this.values = values;
    }

    public OgnlContext(ClassResolver? classResolver,
        TypeConverter? typeConverter, MemberAccess? memberAccess,
        IDictionary values) :
        this(classResolver, typeConverter, memberAccess)
    {
        this.values = values;
    }

    public void setValues(IDictionary value)
    {
        Util.putAll(value, values);
    }

    public IDictionary getValues()
    {
        return values;
    }

    public void setClassResolver(ClassResolver value)
    {
        classResolver = value;
    }

    public ClassResolver getClassResolver()
    {
        return classResolver;
    }

    public void setTypeConverter(TypeConverter value)
    {
        typeConverter = value;
    }

    public TypeConverter getTypeConverter()
    {
        return typeConverter;
    }

    public void setMemberAccess(MemberAccess value)
    {
        memberAccess = value;
    }

    public MemberAccess getMemberAccess()
    {
        return memberAccess;
    }

    public void setRoot(object? value)
    {
        root = value;
    }

    public object? getRoot()
    {
        return root;
    }

    public bool getTraceEvaluations()
    {
        return traceEvaluations;
    }

    public void setTraceEvaluations(bool value)
    {
        traceEvaluations = value;
    }

    public Evaluation? getLastEvaluation()
    {
        return lastEvaluation;
    }

    public void setLastEvaluation(Evaluation? value)
    {
        lastEvaluation = value;
    }

    ///<summary>
    /// This method can be called when the last evaluation has been used
    /// and can be returned for reuse in the free pool maintained by the
    /// runtime.  This is not a necessary step, but is useful for keeping
    /// memory usage down.  This will recycle the last evaluation and then
    /// set the last evaluation to null.
    ///</summary>
    public void recycleLastEvaluation()
    {
        OgnlRuntime.getEvaluationPool().recycleAll(lastEvaluation);
        lastEvaluation = null;
    }

    ///<summary>
    /// Returns true if the last evaluation that was done on this
    /// context is retained and available through <code>getLastEvaluation()</code>.
    /// The default is true.
    ///</summary>
    public bool getKeepLastEvaluation()
    {
        return keepLastEvaluation;
    }

    ///<summary>
    /// Sets whether the last evaluation that was done on this
    /// context is retained and available through <code>getLastEvaluation()</code>.
    /// The default is true.
    ///</summary>
    public void setKeepLastEvaluation(bool value)
    {
        keepLastEvaluation = value;
    }

    public void setCurrentObject(object? value)
    {
        currentObject = value;
    }

    public object? getCurrentObject()
    {
        return currentObject;
    }

    public void setCurrentNode(Node? value)
    {
        currentNode = value;
    }

    public Node? getCurrentNode()
    {
        return currentNode;
    }

    ///
    /// Gets the current Evaluation from the top of the stack.
    /// This is the Evaluation that is in process of evaluating.
    ///
    public Evaluation? getCurrentEvaluation()
    {
        return currentEvaluation;
    }

    public void setCurrentEvaluation(Evaluation? value)
    {
        currentEvaluation = value;
    }

    ///<summary>
    /// Gets the root of the evaluation stack.
    /// This Evaluation contains the node representing
    /// the root expression and the source is the root
    /// source object.
    ///</summary>
    public Evaluation? getRootEvaluation()
    {
        return rootEvaluation;
    }

    public void setRootEvaluation(Evaluation? value)
    {
        rootEvaluation = value;
    }

    ///<summary>
    /// Returns the Evaluation at the relative index given.  This should be
    /// zero or a negative number as a relative reference back up the evaluation
    /// stack.  Therefore getEvaluation(0) returns the current Evaluation.
    ///</summary>
    public Evaluation? getEvaluation(int relativeIndex)
    {
        Evaluation? result = null;

        if (relativeIndex <= 0) {
            result = currentEvaluation;

            while (++relativeIndex < 0 && result != null)
                result = result.getParent();
        }

        return result;
    }

    ///<summary>
    /// Pushes a new Evaluation onto the stack.  This is done
    /// before a node evaluates.  When evaluation is complete
    /// it should be popped from the stack via <code>popEvaluation()</code>.
    ///</summary>
    public void pushEvaluation(Evaluation value)
    {
        if (currentEvaluation != null)
            currentEvaluation.addChild(value);
        else
            setRootEvaluation(value);

        setCurrentEvaluation(value);
    }

    ///<summary>
    /// Pops the current Evaluation off of the top of the stack.
    /// This is done after a node has completed its evaluation.
    ///</summary>
    public Evaluation? popEvaluation()
    {
        var result = currentEvaluation;

        setCurrentEvaluation(result?.getParent());

        if (currentEvaluation == null) {
            setLastEvaluation(getKeepLastEvaluation() ? result : null);
            setRootEvaluation(null);
            setCurrentNode(null);
        }

        return result;
    }

    public override bool Equals(object? o)
    {
        return values.Equals(o);
    }

    public override int GetHashCode()
    {
        return values.GetHashCode();
    }

    public void CopyTo(Array array, int index)
    {
        values.CopyTo(array, index);
    }

    public int Count => values.Count;

    public object SyncRoot => values.SyncRoot;

    public bool IsSynchronized => values.IsSynchronized;

    public bool Contains(object key)
    {
        return values.Contains(key);
    }

    public void Add(object key, object? value)
    {
        values.Add(key, value);
    }

    public void Clear()
    {
        values.Clear();
        setRoot(null);
        setCurrentObject(null);
        setRootEvaluation(null);
        setCurrentEvaluation(null);
        setLastEvaluation(null);
        setCurrentNode(null);
        setClassResolver(DEFAULT_CLASS_RESOLVER);
        setTypeConverter(DEFAULT_TYPE_CONVERTER);
        setMemberAccess(DEFAULT_MEMBER_ACCESS);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return values.Values.GetEnumerator();
    }

    public IDictionaryEnumerator GetEnumerator()
    {
        return values.GetEnumerator();
    }

    public void Remove(object key)
    {
        if (key.Equals(CONTEXT_CONTEXT_KEY))
            throw new ArgumentException("can't remove " +
                CONTEXT_CONTEXT_KEY + " from context");

        if (key.Equals(TRACE_EVALUATIONS_CONTEXT_KEY))
            throw new ArgumentException("can't remove " +
                TRACE_EVALUATIONS_CONTEXT_KEY + " from context");

        if (key.Equals(KEEP_LAST_EVALUATION_CONTEXT_KEY))
            throw new ArgumentException("can't remove " +
                KEEP_LAST_EVALUATION_CONTEXT_KEY + " from context");

        if (key.Equals(THIS_CONTEXT_KEY))
            setCurrentObject(null);
        else if (key.Equals(ROOT_CONTEXT_KEY))
            setRoot(null);
        else if (key.Equals(LAST_EVALUATION_CONTEXT_KEY))
            setLastEvaluation(null);
        else if (key.Equals(CLASS_RESOLVER_CONTEXT_KEY))
            setClassResolver(null);
        else if (key.Equals(TYPE_CONVERTER_CONTEXT_KEY))
            setTypeConverter(null);
        else if (key.Equals(MEMBER_ACCESS_CONTEXT_KEY))
            setMemberAccess(null);
        else
            values.Remove(key);
    }

    public ICollection Keys => values.Keys;

    public ICollection Values => values.Values;

    public bool IsReadOnly => values.IsReadOnly;

    public bool IsFixedSize => values.IsFixedSize;

    public object? this[object key] {
        get {
            if (key.Equals(THIS_CONTEXT_KEY))
                return getCurrentObject();

            if (key.Equals(ROOT_CONTEXT_KEY))
                return getRoot();

            if (key.Equals(CONTEXT_CONTEXT_KEY))
                return this;

            if (key.Equals(TRACE_EVALUATIONS_CONTEXT_KEY))
                return getTraceEvaluations();

            if (key.Equals(LAST_EVALUATION_CONTEXT_KEY))
                return getLastEvaluation();

            if (key.Equals(KEEP_LAST_EVALUATION_CONTEXT_KEY))
                return getKeepLastEvaluation();

            if (key.Equals(CLASS_RESOLVER_CONTEXT_KEY))
                return getClassResolver();

            if (key.Equals(TYPE_CONVERTER_CONTEXT_KEY))
                return getTypeConverter();

            if (key.Equals(MEMBER_ACCESS_CONTEXT_KEY))
                return getMemberAccess();

            return values[key];
        }

        set {
            if (key.Equals(THIS_CONTEXT_KEY))
                setCurrentObject(value);
            else if (key.Equals(ROOT_CONTEXT_KEY))
                setRoot(value);
            else {
                if (key.Equals(CONTEXT_CONTEXT_KEY))
                    throw new ArgumentException("can't change " + CONTEXT_CONTEXT_KEY + " in context");

                if (key.Equals(TRACE_EVALUATIONS_CONTEXT_KEY))
                    setTraceEvaluations(OgnlOps.booleanValue(value));
                else if (key.Equals(LAST_EVALUATION_CONTEXT_KEY))
                    setLastEvaluation((Evaluation?)value);
                else if (key.Equals(KEEP_LAST_EVALUATION_CONTEXT_KEY)) {
                    setKeepLastEvaluation(OgnlOps.booleanValue(value));
                } else if (key.Equals(CLASS_RESOLVER_CONTEXT_KEY)) {
                    setClassResolver((ClassResolver)value);
                } else if (key.Equals(TYPE_CONVERTER_CONTEXT_KEY)) {
                    setTypeConverter((TypeConverter)value);
                } else {
                    if (!key.Equals(MEMBER_ACCESS_CONTEXT_KEY)) {
                        throw new ArgumentException("unknown reserved key '" + key + "'");
                    }

                    setMemberAccess((MemberAccess)value);
                }

                var flag = false;

                foreach (var obj2 in values.Keys) {
                    if (obj2.Equals(key)) {
                        flag = true;

                        break;
                    }
                }

                if (flag) {
                    values[key] = value;
                } else {
                    values.Add(key, value);
                }
            }
        }
    }
}
