using System.Collections;

namespace OGNL;

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

    public static IList<T> NCopies<T>(int n, T o)
    {
        return new List<T>(Enumerable.Repeat(o, n));
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
