//--------------------------------------------------------------------------
//	Copyright (c) 1998-2004, Drew Davidson, Luke Blanshard and Foxcoming
//  Copyright (c) 2026, Alexei Yashkov
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are
//  met:
//
//  Redistributions of source code must retain the above copyright notice,
//  this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright
//  notice, this list of conditions and the following disclaimer in the
//  documentation and/or other materials provided with the distribution.
//  Neither the name of the Drew Davidson nor the names of its contributors
//  may be used to endorse or promote products derived from this software
//  without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace OGNL;

using MethodMap = IDictionary<string, IList<MethodInfo>>;
using MethodCache = Dictionary<Type, IDictionary<string, IList<MethodInfo>>>;
using FieldMap = IDictionary<string, FieldInfo>;
using FieldCache = Dictionary<Type, IDictionary<string, FieldInfo>>;

/**
 * This is a static class with static methods that define runtime
 * caching information in OGNL.
 */
public static class OgnlRuntime {
    /** Not an indexed property */
    public const int IndexedPropertyNone = 0;

    /** JavaBeans IndexedProperty */
    public const int IndexedPropertyInt = 1;

    /** OGNL ObjectIndexedProperty */
    public const int IndexedPropertyObject = 2;

    public const string NullString = "" + null;

    public static readonly object NotFound = new();

    private static readonly object[] NoArguments = [];

    private static readonly ClassCache MethodAccessors = new();

    private static readonly ClassCache PropertyAccessors = new();

    private static readonly ClassCache ElementsAccessors = new();

    private static readonly ClassCache NullHandlers = new();

    private static readonly ClassCache PropertyDescriptorCache = new();

    private static readonly MethodCache StaticMethodCache = new();

    private static readonly MethodCache InstanceMethodCache = new();

    private static readonly FieldCache FieldCache = new();

    private static readonly Hashtable PrimitiveTypes = new(101);

    private static readonly ClassCache PrimitiveDefaults = new();

    private static readonly Hashtable MethodParameterTypesCache = new(101);

    private static readonly Hashtable CtorParameterTypesCache = new(101);

    private static void Append<K, V>(this IDictionary<K, IList<V>> dictionary,
        K key, V value)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        if (!dictionary.TryGetValue(key, out var list))
            dictionary[key] = list = new List<V>();

        list.Add(value);
    }

    // XXX: replace with a standard collection
    private class ClassCache {
        /* this MUST be a power of 2 */
        private const int TableSize = 512;

        /* ...and now you see why.  The table size is used as a mask for generating hashes */
        private const int TableSizeMask = TableSize - 1;

        private readonly Entry?[] table = new Entry[TableSize];

        internal class Entry(Type key, object value) {
            internal Entry? Next;

            internal readonly Type Key = key;

            internal object Value = value;
        }

        public object? Get(Type key)
        {
            object? result = null;
            var i = key.GetHashCode() & TableSizeMask;

            for (var entry = table[i]; entry != null; entry = entry.Next) {
                if (entry.Key == key) {
                    result = entry.Value;

                    break;
                }
            }

            return result;
        }

        public void Put(Type key, object value)
        {
            var i = key.GetHashCode() & TableSizeMask;
            var entry = table[i];

            if (entry == null) {
                table[i] = new Entry(key, value);
            } else {
                if (entry.Key == key) {
                    entry.Value = value;
                } else {
                    while (true) {
                        if (entry.Key == key) {
                            /* replace value */
                            entry.Value = value;

                            break;
                        }

                        if (entry.Next == null) {
                            /* add value */
                            entry.Next = new Entry(key, value);

                            break;
                        }

                        entry = entry.Next;
                    }
                }
            }
        }
    }

    static OgnlRuntime()
    {
        PropertyAccessor p = new ArrayPropertyAccessor();
        SetPropertyAccessor(typeof(object), new ObjectPropertyAccessor());
        SetPropertyAccessor(typeof(byte[]), p);
        SetPropertyAccessor(typeof(short[]), p);
        SetPropertyAccessor(typeof(char[]), p);
        SetPropertyAccessor(typeof(int[]), p);
        SetPropertyAccessor(typeof(long[]), p);
        SetPropertyAccessor(typeof(float[]), p);
        SetPropertyAccessor(typeof(double[]), p);
        SetPropertyAccessor(typeof(object[]), p);
        SetPropertyAccessor(typeof(IList), new ListPropertyAccessor());
        SetPropertyAccessor(typeof(IDictionary), new MapPropertyAccessor());
        SetPropertyAccessor(typeof(ICollection), new SetPropertyAccessor());

        // TODO: Ignore Iterator
        // setPropertyAccessor( typeof (Iterator), new IteratorPropertyAccessor() );
        SetPropertyAccessor(typeof(IEnumerator), new EnumerationPropertyAccessor());

        ElementsAccessor e = new ArrayElementsAccessor();
        SetElementsAccessor(typeof(object), new ObjectElementsAccessor());
        SetElementsAccessor(typeof(byte[]), e);
        SetElementsAccessor(typeof(short[]), e);
        SetElementsAccessor(typeof(char[]), e);
        SetElementsAccessor(typeof(int[]), e);
        SetElementsAccessor(typeof(long[]), e);
        SetElementsAccessor(typeof(float[]), e);
        SetElementsAccessor(typeof(double[]), e);
        SetElementsAccessor(typeof(object[]), e);
        SetElementsAccessor(typeof(ICollection), new CollectionElementsAccessor());
        SetElementsAccessor(typeof(IDictionary), new MapElementsAccessor());

        // TODO: ignore Iterator
        // setElementsAccessor( typeof (Iterator), new IteratorElementsAccessor() );
        SetElementsAccessor(typeof(IEnumerator), new EnumerationElementsAccessor());
        SetElementsAccessor(typeof(ValueType), new NumberElementsAccessor());

        NullHandler nh = new ObjectNullHandler();
        SetNullHandler(typeof(object), nh);
        SetNullHandler(typeof(byte[]), nh);
        SetNullHandler(typeof(short[]), nh);
        SetNullHandler(typeof(char[]), nh);
        SetNullHandler(typeof(int[]), nh);
        SetNullHandler(typeof(long[]), nh);
        SetNullHandler(typeof(float[]), nh);
        SetNullHandler(typeof(double[]), nh);
        SetNullHandler(typeof(object[]), nh);

        MethodAccessor ma = new ObjectMethodAccessor();
        SetMethodAccessor(typeof(object), ma);
        SetMethodAccessor(typeof(byte[]), ma);
        SetMethodAccessor(typeof(short[]), ma);
        SetMethodAccessor(typeof(char[]), ma);
        SetMethodAccessor(typeof(int[]), ma);
        SetMethodAccessor(typeof(long[]), ma);
        SetMethodAccessor(typeof(float[]), ma);
        SetMethodAccessor(typeof(double[]), ma);
        SetMethodAccessor(typeof(object[]), ma);

        PrimitiveTypes["bool"] = typeof(bool);
        PrimitiveTypes["byte"] = typeof(byte);
        PrimitiveTypes["short"] = typeof(short);
        PrimitiveTypes["char"] = typeof(char);
        PrimitiveTypes["int"] = typeof(int);
        PrimitiveTypes["long"] = typeof(long);
        PrimitiveTypes["float"] = typeof(float);
        PrimitiveTypes["double"] = typeof(double);

        // Add string as primitive
        PrimitiveTypes["string"] = typeof(string);

        // Add object as primitive
        PrimitiveTypes["object"] = typeof(object);

        // Add decimal as primitive
        PrimitiveTypes["decimal"] = typeof(decimal);
        PrimitiveTypes["ulong"] = typeof(ulong);
        PrimitiveTypes["uint"] = typeof(uint);
        PrimitiveTypes["ushort"] = typeof(ushort);

        PrimitiveDefaults.Put(typeof(bool), false);
        PrimitiveDefaults.Put(typeof(byte), (byte)0);
        PrimitiveDefaults.Put(typeof(short), (short)0);
        PrimitiveDefaults.Put(typeof(char), (char)0);
        PrimitiveDefaults.Put(typeof(int), 0);
        PrimitiveDefaults.Put(typeof(long), 0L);
        PrimitiveDefaults.Put(typeof(float), 0.0f);
        PrimitiveDefaults.Put(typeof(double), 0.0D);
        PrimitiveDefaults.Put(typeof(decimal), (decimal)0);

        // TODO: match BigInteger.
        // primitiveDefaults.put(typeof (), new BigInteger("0"));
    }

    /**
     * Gets the "target" class of an object for looking up accessors
     * that are registered on the target.  If the object is a Type object
     * this will return the Type itself, else it will return object's
     * GetType() result.
     */
    [return: NotNullIfNotNull(nameof(obj))]
    public static Type? GetTargetType(object? obj)
    {
        return obj == null ? null : obj as Type ?? obj.GetType();
    }

    private static Type[] GetParameterTypes(MethodInfo m)
    {
        lock (MethodParameterTypesCache) {
            var result = (Type[]?)MethodParameterTypesCache[m];

            if (result == null)
                MethodParameterTypesCache[m] = result = GetParameterTypes0(m);

            return result;
        }
    }

    private static Type[] GetParameterTypes0(MethodInfo m)
    {
        var ps = m.GetParameters();
        var pts = new Type[ps.Length];

        for (var i = 0; i < ps.Length; i++) {
            var pt = ps[i];

            pts[i] = pt.ParameterType;
        }

        return pts;
    }

    private static Type[] GetParameterTypes(ConstructorInfo c)
    {
        lock (CtorParameterTypesCache) {
            var result = (Type[]?)CtorParameterTypesCache[c];

            if (result == null)
                CtorParameterTypesCache[c] = result = GetParameterTypes0(c);

            return result;
        }
    }

    private static Type[] GetParameterTypes0(ConstructorInfo c)
    {
        var ps = c.GetParameters();
        var pts = new Type[ps.Length];

        for (var i = 0; i < ps.Length; i++) {
            var pt = ps[i];

            pts[i] = pt.ParameterType;
        }

        return pts;
    }

    private static bool IsTypeCompatible(object? obj, Type type)
    {
        if (obj == null)
            return true;

        if (type.IsPrimitive)
            return obj.GetType() == type;

        return type.IsInstanceOfType(obj);
    }

    private static bool AreArgsCompatible(object[] args, Type[] classes)
    {
        var result = true;

        if (args.Length != classes.Length)
            result = false;
        else
            for (int index = 0, count = args.Length; result && index < count; ++index)
                result = IsTypeCompatible(args[index], classes[index]);

        return result;
    }

    private static bool IsMoreSpecific(Type[] classes1, Type[] classes2)
    {
        for (int index = 0, count = classes1.Length; index < count; ++index) {
            var c1 = classes1[index];
            var c2 = classes2[index];

            if (c1 == c2)
                continue;

            if (c1.IsPrimitive)
                return true;

            if (c1.IsAssignableFrom(c2))
                return false;

            if (c2.IsAssignableFrom(c1))
                return true;
        }

        // They are the same! So the first is not more specific than the second.
        return false;
    }

    public static Type TypeForName(OgnlContext context, string typeName)
    {
        return (Type?)PrimitiveTypes[typeName] ??
            context.TypeResolver.TypeForName(typeName);
    }

    public static bool IsInstance(OgnlContext context, object? value,
        string? typeName)
    {
        if (value == null || typeName == null)
            return false;

        try {
            return TypeForName(context, typeName).IsInstanceOfType(value);
        } catch (Exception ex) {
            throw new OgnlException($"No such class: {typeName}", ex);
        }
    }

    public static object? GetPrimitiveDefaultValue(Type forClass)
    {
        return PrimitiveDefaults.Get(forClass);
    }

    [return: NotNullIfNotNull("value")]
    private static object? GetConvertedType(OgnlContext context,
        object? value, Type type)
    {
        return context.TypeConverter.ConvertValue(value, type);
    }

    private static bool GetConvertedTypes(OgnlContext context,
        Type[] parameterTypes, object[] args, object?[] newArgs)
    {
        var result = false;

        if (parameterTypes.Length == args.Length) {
            result = true;

            for (int i = 0, ilast = parameterTypes.Length - 1; result && i <= ilast; i++) {
                var arg = args[i];
                var type = parameterTypes[i];

                if (IsTypeCompatible(arg, type))
                    newArgs[i] = arg;
                else {
                    var v = GetConvertedType(context, arg, type);

                    if (ReferenceEquals(v, TypeConverter.NoConversionPossible))
                        result = false;
                    else
                        newArgs[i] = v;
                }
            }
        }

        return result;
    }

    private static MethodInfo? GetConvertedMethodAndArgs(
        OgnlContext context, IList<MethodInfo> methods, object[] args,
        object[] newArgs)
    {
        MethodInfo? result = null;

        for (int i = 0, icount = methods.Count;
            result == null && i < icount; i++) {
            var m = methods[i];
            var parameterTypes = GetParameterTypes(m);

            if (GetConvertedTypes(context,
                    parameterTypes, args, newArgs))
                result = m;
        }

        return result;
    }

    private static ConstructorInfo? GetConvertedConstructorAndArgs(
        OgnlContext context, IList<ConstructorInfo> constructors,
        object[] args, object?[] newArgs)
    {
        ConstructorInfo? result = null;

        for (int i = 0, icount = constructors.Count;
            result == null && i < icount; i++) {
            var ctor = constructors[i];
            var parameterTypes = GetParameterTypes(ctor);

            if (GetConvertedTypes(context,
                    parameterTypes, args, newArgs))
                result = ctor;
        }

        return result;
    }

    private static MethodInfo GetAppropriateMethod(OgnlContext context,
        IList<MethodInfo> methods, object?[] args, object?[] actualArgs)
    {
        MethodInfo result = null;
        Type[] resultParameterTypes = null;

        for (int i = 0, icount = methods.Count; i < icount; i++) {
            var m = methods[i];
            var mParameterTypes = GetParameterTypes(m);

            if (!AreArgsCompatible(args, mParameterTypes) ||
                (result != null &&
                    !IsMoreSpecific(mParameterTypes, resultParameterTypes)))
                continue;

            result = m;
            resultParameterTypes = mParameterTypes;
            Array.Copy(args, 0, actualArgs, 0, args.Length);

            for (var j = 0; j < mParameterTypes.Length; j++) {
                var type = mParameterTypes[j];

                if (type.IsPrimitive && actualArgs[j] == null)
                    actualArgs[j] = GetConvertedType(context, null, type);
            }
        }

        if (result == null)
            result = GetConvertedMethodAndArgs(context, methods, args, actualArgs);

        return result;
    }

    public static object? CallAppropriateMethod(OgnlContext context,
        object source, object? target, string? methodName,
        string? propertyName, IList<MethodInfo> methods, object?[] args)
    {
        var actualArgs = new object[args.Length];
        Exception? reason;

        try {
            var method = GetAppropriateMethod(context, methods, args, actualArgs);

            if (method == null || !IsMethodAccessible(context, source, method, propertyName)) {
                var buffer = new StringBuilder();

                if (args != null) {
                    for (int i = 0, ilast = args.Length - 1; i <= ilast; i++) {
                        var arg = args[i];

                        buffer.Append(arg == null ? NullString : arg.GetType().Name);

                        if (i < ilast)
                            buffer.Append(", ");
                    }
                }

                throw new MissingMethodException(methodName + "(" + buffer + ")");
            }

            return method.Invoke(target, actualArgs);
        } catch (TargetInvocationException e) {
            reason = e.InnerException;
        } catch (Exception e) {
            reason = e;
        }

        throw new MethodFailedException(source, methodName, reason);
    }

    public static object? CallStaticMethod(OgnlContext context,
        string className, string methodName, object?[] args)
    {
        try {
            var targetClass = TypeForName(context, className);

            return GetMethodAccessor(targetClass)
                .callStaticMethod(context, targetClass, methodName, args);
        } catch (TypeLoadException ex) {
            throw new MethodFailedException(className, methodName, ex);
        }
    }

    public static object? CallMethod(OgnlContext context, object target,
        string methodName, string? propertyName, object?[] args)
    {
        return GetMethodAccessor(target.GetType())
            .callMethod(context, target, methodName, args);
    }

    public static object CallConstructor(OgnlContext context,
        string typeName, object?[] args)
    {
        Exception? reason;
        var actualArgs = args;

        try {
            var target = TypeForName(context, typeName);
            var constructors = target.GetConstructors();
            ConstructorInfo? ctor = null;
            Type[]? ctorParameterTypes = null;

            for (int i = 0, icount = constructors.Length; i < icount; i++) {
                var c = constructors[i];
                var cParameterTypes = GetParameterTypes(c);

                if (AreArgsCompatible(args, cParameterTypes) &&
                    (ctor == null || IsMoreSpecific(cParameterTypes, ctorParameterTypes))) {
                    ctor = c;
                    ctorParameterTypes = cParameterTypes;
                }
            }

            if (ctor == null) {
                actualArgs = new object[args.Length];

                if ((ctor = GetConvertedConstructorAndArgs(context, constructors, args, actualArgs)) == null)
                    throw new MissingMethodException();
            }

            return ctor.Invoke(actualArgs);
        } catch (TypeLoadException e) {
            reason = e;
        } catch (MissingMethodException e) {
            reason = e;
        } catch (MethodAccessException e) {
            reason = e;
        } catch (TargetInvocationException e) {
            reason = e.InnerException;
        } catch (TypeInitializationException e) {
            reason = e;
        }

        throw new MethodFailedException(typeName, "new", reason);
    }

    /**
        If the checkAccessAndExistence flag is true this method will check to see if the
        method exists and if it is accessible according to the context's MemberAccess.
        If neither test passes this will return NotFound.
     */
    public static object? GetMethodValue(OgnlContext context,
        object target, string propertyName,
        bool checkAccessAndExistence = false)
    {
        object? result = null;
        var method = GetGetMethod(target.GetType(), propertyName);

        if (checkAccessAndExistence && method == null)
            result = NotFound;

        if (result == null)
            if (method != null)
                try {
                    result = method.Invoke(target, NoArguments);
                } catch (TargetInvocationException ex) {
                    throw new OgnlException(propertyName, ex.InnerException);
                }
            else
                throw new MissingMethodException(propertyName);

        return result;
    }

    public static bool SetMethodValue(OgnlContext context, object target,
        string propertyName, object? value, bool checkAccessAndExistence)
    {
        var result = true;
        var m = GetSetMethod(target == null ? null : target.GetType(), propertyName);

        if (result)
            if (m != null)
                CallAppropriateMethod(context, target, target, m.Name,
                    propertyName, Util.NCopies(1, m), [value]);
            else
                result = false;

        return result;
    }

    private static MethodMap GetMethods(Type targetClass, bool staticMethods)
    {
        var cache = staticMethods ? StaticMethodCache : InstanceMethodCache;

        lock (cache) {
            cache.TryGetValue(targetClass, out var cached);

            if (cached != null)
                return cached;

            var methods = targetClass.GetMethods();
            var result = new Dictionary<string, IList<MethodInfo>>();

            foreach (var method in methods)
                if (method.IsStatic == staticMethods)
                    result.Append(method.Name, method);

            cache[targetClass] = result;

            return result;
        }
    }

    public static IList<MethodInfo> GetMethods(Type targetClass,
        string name, bool staticMethods)
    {
        var methodMap = GetMethods(targetClass, staticMethods);

        return methodMap.TryGetValue(name, out var methods) ? methods :
            new List<MethodInfo>();
    }

    private static FieldMap GetFields(Type targetClass)
    {
        lock (FieldCache) {
            FieldCache.TryGetValue(targetClass, out var cached);

            if (cached != null)
                return cached;

            var fields = targetClass.GetFields();
            var result = new Dictionary<string, FieldInfo>();

            foreach (var field in fields)
                result[field.Name] = field;

            FieldCache[targetClass] = result;

            return result;
        }
    }

    private static FieldInfo? GetField(Type inClass, string name)
    {
        GetFields(inClass).TryGetValue(name, out var value);

        return value;
    }

    public static object? GetFieldValue(OgnlContext context,
        object target, string name, bool checkAccessAndExistence)
    {
        var field = GetField(target.GetType(), name);

        if (field == null || field.IsStatic)
            throw new MissingFieldException(name);

        try {
            var state = context.MemberAccess
                .Setup(context, target, field, name);

            try {
                return field.GetValue(target);
            } finally {
                context.MemberAccess
                    .Restore(context, target, field, name, state);
            }
        } catch (MemberAccessException ex) {
            throw new MissingFieldException(name, ex);
        }
    }

    public static bool SetFieldValue(OgnlContext context, object target,
        string name, object? value)
    {
        var field = GetField(target.GetType(), name);

        if (field == null || field.IsStatic)
            throw new MissingFieldException(name);

        try {
            var state = context.MemberAccess
                .Setup(context, target, field, name);

            try {
                if (!IsTypeCompatible(value, field.FieldType) &&
                    (value = GetConvertedType(context, value, field.FieldType)) == null)
                    return false;

                field.SetValue(target, value);

                return true;
            } finally {
                context.MemberAccess
                    .Restore(context, target, field, name, state);
            }
        } catch (MemberAccessException ex) {
            throw new NoSuchPropertyException(target, name, ex);
        }
    }

    public static object GetStaticField(OgnlContext context,
        string className, string fieldName)
    {
        Exception? reason;

        try {
            var c = TypeForName(context, className);

            /*
                Check for virtual static field "class"; this cannot interfere with
                normal static fields because it is a reserved word.
             */
            if (fieldName.Equals("class"))
                return c;

            var f = c.GetField(fieldName);

            if (f == null) {
                // try to load Property
                var p = c.GetProperty(fieldName);

                if (p == null)
                    throw new MissingFieldException("Field or Property " + fieldName + " of class " + className +
                        " is not found.");

                if (!p.GetAccessors()[0].IsStatic)
                    throw new MissingFieldException("Property " + fieldName + " of class " + className +
                        " is not static.");

                if (!p.CanRead)
                    throw new MissingFieldException("Property " + fieldName + " of class " + className +
                        " is write-only.");

                return p.GetValue(null, []);
            }

            if (!f.IsStatic)
                throw new OgnlException("Field " + fieldName + " of class " + className + " is not static");

            return f.GetValue(null);
        } catch (TypeLoadException e) {
            reason = e;
        } catch (MissingFieldException e) {
            reason = e;
        } catch (MemberAccessException e) {
            reason = e;
        }

        throw new OgnlException("Could not get static field " + fieldName + " from class " + className, reason);
    }

    private static MethodInfo? GetGetMethod(Type targetClass,
        string propertyName)
    {
        return GetPropertyDescriptor(targetClass, propertyName)?.ReadMethod;
    }

    private static bool IsMethodAccessible(OgnlContext context,
        object target, MethodInfo? method, string? propertyName)
    {
        return method != null && context.MemberAccess.IsAccessible(context,
            target, method, propertyName);
    }

    private static MethodInfo? GetSetMethod(Type targetClass,
        string propertyName)
    {
        return GetPropertyDescriptor(targetClass, propertyName)?.WriteMethod;
    }

    private static IDictionary GetPropertyDescriptors(Type targetClass)
    {
        // TODO: This is the main method about PropertyDescriptor.
        IDictionary result;

        lock (PropertyDescriptorCache) {
            if ((result = (IDictionary)PropertyDescriptorCache.Get(targetClass)) == null) {
                // TODO: Introspector
                // No Setter or Getter, Use property.
                var pda = Introspector.GetPropertyDescriptors(targetClass);

                result = new Hashtable(101);

                for (int i = 0, icount = pda.Length; i < icount; i++)
                    result[pda[i].Name] = pda[i];

                PropertyDescriptorCache.Put(targetClass, result);
            }
        }

        return result;
    }

    /**
     * TODO: About PropertyDescriptor
        This method returns a PropertyDescriptor for the given class and property name using
        a IDictionary lookup (using getPropertyDescriptorsMap()).
     */
    private static PropertyDescriptor? GetPropertyDescriptor(
        Type targetClass, string propertyName)
    {
        return targetClass == null ? null :
            (PropertyDescriptor)GetPropertyDescriptors(targetClass)[propertyName];
    }

    private static void SetMethodAccessor(Type cls, MethodAccessor accessor)
    {
        lock (MethodAccessors) {
            MethodAccessors.Put(cls, accessor);
        }
    }

    private static MethodAccessor GetMethodAccessor(Type cls)
    {
        lock (MethodAccessors) {
            return (MethodAccessor)GetHandler(cls, MethodAccessors) ??
                throw new OgnlException($"No method accessor for {cls}");
        }
    }

    public static void SetPropertyAccessor(Type cls, PropertyAccessor accessor)
    {
        lock (PropertyAccessors) {
            PropertyAccessors.Put(cls, accessor);
        }
    }

    private static PropertyAccessor GetPropertyAccessor(Type cls)
    {
        lock (PropertyAccessors) {
            return (PropertyAccessor)GetHandler(cls, PropertyAccessors) ??
                throw new OgnlException($"No property accessor for class {cls}");
        }
    }

    public static ElementsAccessor GetElementsAccessor(Type cls)
    {
        lock (ElementsAccessors) {
            return (ElementsAccessor)GetHandler(cls, ElementsAccessors) ??
                throw new OgnlException($"No elements accessor for class {cls}");
        }
    }

    private static void SetElementsAccessor(Type cls, ElementsAccessor accessor)
    {
        lock (ElementsAccessors) {
            ElementsAccessors.Put(cls, accessor);
        }
    }

    public static NullHandler GetNullHandler(Type cls)
    {
        lock (NullHandlers) {
            return (NullHandler)GetHandler(cls, NullHandlers) ??
                throw new OgnlException($"No null handler for class {cls}");
        }
    }

    public static void SetNullHandler(Type cls, NullHandler handler)
    {
        lock (NullHandlers) {
            NullHandlers.Put(cls, handler);
        }
    }

    private static object GetHandler(Type forClass, ClassCache handlers)
    {
        object? answer;

        lock (handlers) {
            if ((answer = handlers.Get(forClass)) == null) {
                Type? keyFound;

                if (forClass.IsArray) {
                    answer = handlers.Get(typeof(object[]));
                    keyFound = null;
                } else {
                    keyFound = forClass;

                    // outer:
                    for (var c = forClass; c != null; c = c.BaseType) {
                        answer = handlers.Get(c);

                        if (answer == null) {
                            var interfaces = c.GetInterfaces();

                            for (int index = 0, count = interfaces.Length; index < count; ++index) {
                                var iface = interfaces[index];

                                /* Try base-interfaces */
                                answer = handlers.Get(iface) ??
                                    GetHandler(iface, handlers);

                                if (answer != null) {
                                    keyFound = iface;

                                    // TODO: Break to label.
                                    goto outer;
                                }
                            }
                        } else {
                            keyFound = c;

                            break;
                        }
                    }

                    outer: ;
                }

                if (answer != null && keyFound != forClass)
                    handlers.Put(forClass, answer);
            }
        }

        return answer;
    }

    public static object? GetProperty(OgnlContext context, object source,
        object name)
    {
        PropertyAccessor accessor;

        if (source == null)
            throw new OgnlException($"source is null for getProperty(null, \"{name}\")");

        if ((accessor = GetPropertyAccessor(GetTargetType(source))) == null)
            throw new OgnlException($"No property accessor for {GetTargetType(source).Name}");

        return accessor.GetProperty(context, source, name);
    }

    public static void SetProperty(OgnlContext context, object target,
        object name, object value)
    {
        PropertyAccessor accessor;

        if (target == null)
            throw new OgnlException($"target is null for setProperty(null, \"{name}\", {value})");

        if ((accessor = GetPropertyAccessor(GetTargetType(target))) == null)
            throw new OgnlException($"No property accessor for {GetTargetType(target).Name}");

        accessor.SetProperty(context, target, name, value);
    }

    /**
        Determines the index property type, if any.  Returns <code>INDEXED_PROPERTY_NONE</code> if the
        property is not index-accessible as determined by OGNL or JavaBeans.  If it is indexable
        then this will return whether it is a JavaBeans indexed property, conforming to the
        indexed property patterns (returns <code>INDEXED_PROPERTY_INT</code>) or if it conforms
        to the OGNL arbitrary object indexable (returns <code>INDEXED_PROPERTY_OBJECT</code>).
     */
    public static int GetIndexedPropertyType(OgnlContext context,
        Type sourceClass, string name)
    {
        var result = IndexedPropertyNone;

        try {
            var pd = GetPropertyDescriptor(sourceClass, name);

            if (pd != null) {
                if (pd is IndexedPropertyDescriptor)
                    result = IndexedPropertyInt;
                else if (pd is ObjectIndexedPropertyDescriptor)
                    result = IndexedPropertyObject;
            }
        } catch (Exception ex) {
            throw new OgnlException($"problem determining if '{name}' is an indexed property", ex);
        }

        return result;
    }

    public static object GetIndexedProperty(OgnlContext context,
        object source, string name, object index)
    {
        try {
            var pd = GetPropertyDescriptor(source == null ? null : source.GetType(), name);
            MethodInfo m;

            if (pd is IndexedPropertyDescriptor indexedPropertyDescriptor)
                m = indexedPropertyDescriptor.GetIndexedReadMethod();
            else if (pd is ObjectIndexedPropertyDescriptor objectIndexedPropertyDescriptor)
                m = objectIndexedPropertyDescriptor.GetIndexedReadMethod();
            else
                throw new OgnlException($"property '{name}' is not an indexed property");

            return CallMethod(context, source, m.Name, name, [index]);
        } catch (OgnlException) {
            throw;
        } catch (Exception ex) {
            throw new OgnlException($"getting indexed property descriptor for '{name}'", ex);
        }
    }

    public static void SetIndexedProperty(OgnlContext context,
        object source, string name, object index, object value)
    {
        Exception? reason = null;
        var args = new[] { index, value };

        try {
            var pd = GetPropertyDescriptor(source == null ? null : source.GetType(), name);
            MethodInfo m;

            if (pd is IndexedPropertyDescriptor indexedPropertyDescriptor)
                m = indexedPropertyDescriptor.GetIndexedWriteMethod();
            else if (pd is ObjectIndexedPropertyDescriptor objectIndexedPropertyDescriptor)
                m = objectIndexedPropertyDescriptor.GetIndexedWriteMethod();
            else
                throw new OgnlException($"property '{name}' is not an indexed property");

            CallMethod(context, source, m.Name, name, args);
        } catch (OgnlException) {
            throw;
        } catch (Exception ex) {
            throw new OgnlException($"getting indexed property descriptor for '{name}'", ex);
        }
    }

    private static List<MethodInfo> GetIndexerSetMethods(Type source)
    {
        return (
            from property in source.GetProperties()
            where property.CanWrite && property.GetIndexParameters().Length > 0
            select property.GetSetMethod()
        )
            .Where(method => method != null)
            .ToList();
    }

    private static List<MethodInfo> GetIndexerGetMethods(Type source)
    {
        return (
                from property in source.GetProperties()
                where property.CanRead && property.GetIndexParameters().Length > 0
                select property.GetGetMethod()
            )
            .Where(method => method != null)
            .ToList();
    }

    public static bool SetIndexerValue(OgnlContext context, object target,
        object name, object? value, object[] args)
    {
        var methods = GetIndexerSetMethods(target.GetType());

        if (methods.Count == 0)
            return false;

        var actualArgs = new object?[args.Length + 1];

        Array.Copy(args, 0, actualArgs, 0, args.Length);
        actualArgs[args.Length] = value;

        CallAppropriateMethod(context, target, target, null, null,
            methods, actualArgs);

        return true;
    }

    public static object? GetIndexerValue(OgnlContext context,
        object target, object name, object[] args)
    {
        var methods = GetIndexerGetMethods(target.GetType());

        if (methods.Count == 0)
            throw new NoSuchPropertyException(target, GetIndexerName(args));

        return CallAppropriateMethod(context, target, target,
            GetIndexerName(args), "Indexer", methods, args);
    }

    private static string GetIndexerName(object[] ts)
    {
        var sb = new StringBuilder();
        sb.Append("this [");

        for (var i = 0; i < ts.Length; i++) {
            if (i > 0)
                sb.Append(", ");

            sb.Append(ts[i].GetType().Name);
        }

        sb.Append(']');

        return sb.ToString();
    }

    public static bool HasSetIndexer(OgnlContext context, object target,
        Type targetClass, int paramCount)
    {
        return GetIndexerSetMethods(targetClass)
            .Any(method => method.GetParameters().Length == paramCount + 1);
    }

    public static bool HasGetIndexer(OgnlContext context, object target,
        Type targetClass, int paramCount)
    {
        return GetIndexerGetMethods(targetClass)
            .Any(method => method.GetParameters().Length == paramCount);
    }
}
