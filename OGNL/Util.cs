using System.Collections;

namespace OGNL;

/// <summary>
/// Long
/// </summary>
public static class Util {
    public static long ParseLong(string s, int radix = 10)
    {
        if (s == null)
            throw new NullReferenceException("null");

        if (radix < 2)
            throw new FormatException("radix " + radix + " less than 2");

        if (radix > 16)
            throw new FormatException("radix " + radix + " greater than 16");

        var result = 0L;
        var negative = false;
        var i = 0;
        var max = s.Length;

        if (max > 0) {
            if (s[0] == '-') {
                negative = true;
                i++;
            }

            int digit;

            if (i < max) {
                digit = Digit(s[i++], radix);

                if (digit < 0)
                    throw new FormatException(s);

                result = -digit;
            }

            while (i < max) {
                // Accumulating negatively avoids surprises near MAX_VALUE
                digit = Digit(s[i++], radix);

                if (digit < 0)
                    throw new FormatException(s);

                result *= radix;
                result -= digit;
            }
        } else
            throw new FormatException(s);

        if (negative) {
            if (i > 1)
                return result;

            /* Only got "-" */
            throw new FormatException(s);
        }

        return -result;
    }

    private static int Digit(char ch, int radix)
    {
        var d = 0;

        if (radix <= 10)
            d = ch - '0';
        else {
            if (ch >= '0' && ch <= '9')
                d = ch - '0';
            else if (ch >= 'a' && ch <= 'z')
                d = ch - 'a' + 10;
            else if (ch >= 'A' && ch <= 'Z')
                d = ch - 'A' + 10;
        }

        if (d >= radix)
            throw new ArgumentOutOfRangeException(nameof(ch), ch,
                "greater than radix [" + radix + "]");

        return d;
    }

    public static bool IsISOControl(char ch)
    {
        return ch <= 0x009F && (ch <= 0x001F || ch >= 0x007F);
    }

    public static IList NCopies(int n, object o)
    {
        return new CopiesList(n, o);
    }

    public static bool Eq(object? o1, object? o2)
    {
        return o1?.Equals(o2) ?? o2 == null;
    }

    public static void putAll(IDictionary? source, IDictionary? target)
    {
        if (source == null || target == null)
            return;

        foreach (DictionaryEntry entry in source) {
            var key = entry.Key;
            var value = entry.Value;

            target[key] = value;
        }
    }
}

internal class CopiesList : IList {
    public int Count { get; }

    public object SyncRoot => this;

    public bool IsReadOnly => true;

    public bool IsFixedSize => true;

    public bool IsSynchronized => true;

    private readonly object element;

    public CopiesList(int n, object o)
    {
        if (n < 0)
            throw new ArgumentException("List length = " + n);

        Count = n;
        element = o;
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public int Add(object? value)
    {
        throw new NotImplementedException();
    }

    public bool Contains(object? value)
    {
        return Count != 0 && Util.Eq(value, element);
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public int IndexOf(object? value)
    {
        return Contains(value) ? 0 : -1;
    }

    public void Insert(int index, object? value)
    {
        throw new NotImplementedException();
    }

    public void Remove(object? value)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public object? this[int index] {
        get {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index: " + index +
                    ", Size: " + Count);

            return element;
        }

        set => throw new NotImplementedException();
    }

    public IEnumerator GetEnumerator()
    {
        return new CopiesEnumerator(Count, element);
    }

    private class CopiesEnumerator(int n, object element) : IEnumerator {
        private int index = 0;

        public object Current {
            get {
                index++;

                return element;
            }
        }

        public bool MoveNext()
        {
            return index < n;
        }

        public void Reset()
        {
            index = 0;
        }
    }
}
