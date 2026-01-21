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

/// <summary>
///This class defines the execution context for an OGNL expression
///</summary>
///@author Luke Blanshard (blanshlu@netscape.net)
///@author Drew Davidson (drew@ognl.org)
///
public class OgnlContext : IDictionary {
    public const string TypeConverterKey = "_typeConverter";

    private const string RootKey = "root";

    private const string ContextKey = "context";

    private const string ThisContextKey = "this";

    private const string LastEvaluationKey = "_lastEvaluation";

    private const string TypeResolverKey = "_typeResolver";

    private const string MemberAccessKey = "_memberAccess";

    private const string TraceEvaluationsKey = "_traceEvaluations";

    private const string KeepLastEvaluationKey = "_keepLastEvaluation";

    private const bool DefaultTraceEvaluations = false;

    private const bool DefaultKeepLastEvaluation = false;

    public static readonly TypeResolver DefaultTypeResolver =
        new DefaultTypeResolver();

    private static readonly TypeConverter DefaultTypeConverter =
        new DefaultTypeConverter();

    private static readonly MemberAccess DefaultMemberAccess =
        new DefaultMemberAccess(false);

    public int Count => values.Count;

    public object SyncRoot => values.SyncRoot;

    public bool IsSynchronized => values.IsSynchronized;

    public ICollection Keys => values.Keys;

    public ICollection Values => values.Values;

    public bool IsReadOnly => values.IsReadOnly;

    public bool IsFixedSize => values.IsFixedSize;

    public object? this[object key] {
        get {
            if (key.Equals(ThisContextKey))
                return CurrentObject;

            if (key.Equals(RootKey))
                return Root;

            if (key.Equals(ContextKey))
                return this;

            if (key.Equals(TraceEvaluationsKey))
                return TraceEvaluations;

            if (key.Equals(LastEvaluationKey))
                return lastEvaluation;

            if (key.Equals(KeepLastEvaluationKey))
                return KeepLastEvaluation;

            if (key.Equals(TypeResolverKey))
                return TypeResolver;

            if (key.Equals(TypeConverterKey))
                return TypeConverter;

            if (key.Equals(MemberAccessKey))
                return MemberAccess;

            return values[key];
        }

        set {
            if (key.Equals(ContextKey))
                throw new ArgumentException(
                    $"can't change {ContextKey} in context");

            if (key.Equals(ThisContextKey))
                CurrentObject = value;
            else if (key.Equals(RootKey))
                Root = value;
            else if (key.Equals(TraceEvaluationsKey))
                TraceEvaluations = OgnlOps.BooleanValue(value);
            else if (key.Equals(LastEvaluationKey))
                lastEvaluation = (Evaluation?)value;
            else if (key.Equals(KeepLastEvaluationKey))
                KeepLastEvaluation = OgnlOps.BooleanValue(value);
            else if (key.Equals(TypeResolverKey))
                TypeResolver = (TypeResolver)(value ?? DefaultTypeResolver);
            else if (key.Equals(TypeConverterKey))
                TypeConverter = (TypeConverter)(value ?? DefaultTypeConverter);
            else if (key.Equals(MemberAccessKey))
                MemberAccess = (MemberAccess)(value ?? DefaultMemberAccess);
            else
                values[key] = value;
        }
    }

    public object? Root { get; set; }

    public object? CurrentObject { get; set; }

    public Node? CurrentNode { get; set; }

    public Evaluation? RootEvaluation { get; private set; }

    public TypeResolver TypeResolver { get; set; } = DefaultTypeResolver;

    public TypeConverter TypeConverter { get; set; } = DefaultTypeConverter;

    public MemberAccess MemberAccess { get; set; } = DefaultMemberAccess;

    public bool TraceEvaluations { get; private set; } = DefaultTraceEvaluations;

    public bool KeepLastEvaluation { get; private set; } = DefaultKeepLastEvaluation;

    private readonly Hashtable values = new(23);

    private Evaluation? currentEvaluation;

    private Evaluation? lastEvaluation;

    public void SetAllValues(IDictionary dictionary)
    {
        foreach (DictionaryEntry entry in dictionary)
            values[entry.Key] = entry.Value;
    }

    ///<summary>
    /// This method can be called when the last evaluation has been used
    /// and can be returned for reuse in the free pool maintained by the
    /// runtime.  This is not a necessary step, but is useful for keeping
    /// memory usage down.  This will recycle the last evaluation and then
    /// set the last evaluation to null.
    ///</summary>
    public void RecycleLastEvaluation()
    {
        OgnlRuntime.EvaluationPool.recycleAll(lastEvaluation);
        lastEvaluation = null;
    }

    ///<summary>
    /// Pushes a new Evaluation onto the stack.  This is done
    /// before a node evaluates.  When evaluation is complete
    /// it should be popped from the stack via <code>popEvaluation()</code>.
    ///</summary>
    public void PushEvaluation(Evaluation value)
    {
        if (currentEvaluation != null)
            currentEvaluation.addChild(value);
        else
            RootEvaluation = value;

        currentEvaluation = value;
    }

    ///<summary>
    /// Pops the current Evaluation off of the top of the stack.
    /// This is done after a node has completed its evaluation.
    ///</summary>
    public Evaluation? PopEvaluation()
    {
        var result = currentEvaluation;

        currentEvaluation = result?.getParent();

        if (currentEvaluation == null) {
            lastEvaluation = KeepLastEvaluation ? result : null;
            RootEvaluation = null;
            CurrentNode = null;
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
        Root = null;
        CurrentObject = null;
        RootEvaluation = null;
        currentEvaluation = null;
        lastEvaluation = null;
        CurrentNode = null;
        TypeResolver = DefaultTypeResolver;
        TypeConverter = DefaultTypeConverter;
        MemberAccess = DefaultMemberAccess;
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
        if (key.Equals(ContextKey))
            throw new ArgumentException("can't remove " +
                ContextKey + " from context");

        if (key.Equals(TraceEvaluationsKey))
            throw new ArgumentException("can't remove " +
                TraceEvaluationsKey + " from context");

        if (key.Equals(KeepLastEvaluationKey))
            throw new ArgumentException("can't remove " +
                KeepLastEvaluationKey + " from context");

        if (key.Equals(ThisContextKey))
            CurrentObject = null;
        else if (key.Equals(RootKey))
            Root = null;
        else if (key.Equals(LastEvaluationKey))
            lastEvaluation = null;
        else if (key.Equals(TypeResolverKey))
            TypeResolver = DefaultTypeResolver;
        else if (key.Equals(TypeConverterKey))
            TypeConverter = DefaultTypeConverter;
        else if (key.Equals(MemberAccessKey))
            MemberAccess = DefaultMemberAccess;
        else
            values.Remove(key);
    }
}
