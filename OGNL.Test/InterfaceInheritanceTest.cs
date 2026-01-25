//--------------------------------------------------------------------------
//  Copyright (c) 2004, Drew Davidson and Luke Blanshard
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

namespace OGNL.Test;

[TestFixture]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class InterfaceInheritanceTest : OgnlFixture {
    public TestMapImpl Map { get; } = new() {
        ["one"] = "uno",
        ["two"] = "dos",
        ["three"] = "tres"
    };

    private static readonly object?[][] Tests = [
        ["Map['one']", "uno"],
        ["Map.two", "dos"],
        ["Map.three.Length", 4],
        ["Map.(null, one)", "uno"],
        ["Map[0] = 25", 25]
    ];

    [Test, TestCaseSource(nameof(Tests))]
    public void Evaluates(string expression, object? expected)
    {
        Assert.That(Get(expression), Is.EqualTo(expected));
    }

    [Test]
    public void CanSetValue()
    {
        Set("Map['tres']", "trois");
        Set("Map.four", "quatre");

        using (Assert.EnterMultipleScope()) {
            Assert.That(Get("Map['tres']"), Is.EqualTo("trois"));
            Assert.That(Get("Map['four']"), Is.EqualTo("quatre"));
        }
    }

    private interface TestMap : IDictionary;

    public class TestMapImpl : TestMap {
        private readonly Hashtable map = new();

        public void Add(object key, object? value)
        {
            map.Add(key, value);
        }

        public void Clear()
        {
            map.Clear();
        }

        public bool Contains(object key)
        {
            return map.Contains(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return map.GetEnumerator();
        }

        public void Remove(object key)
        {
            map.Remove(key);
        }

        public bool IsFixedSize => map.IsFixedSize;

        public bool IsReadOnly => map.IsReadOnly;

        public object? this[object key] {
            get => map[key];

            set => map[key] = value;
        }

        public ICollection Keys => map.Keys;

        public ICollection Values => map.Values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)map).GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            map.CopyTo(array, index);
        }

        public int Count => map.Count;

        public bool IsSynchronized => map.IsSynchronized;

        public object SyncRoot => map.SyncRoot;
    }
}
