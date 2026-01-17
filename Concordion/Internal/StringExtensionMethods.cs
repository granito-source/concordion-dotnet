// Copyright 2009 Jeffrey Cameron
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;

namespace Concordion.Internal;

public static class StringExtensionMethods {
    public static string RemoveFirst(this string str, string toRemove)
    {
        if (string.IsNullOrEmpty(toRemove))
            return string.Empty;

        var index = str.IndexOf(toRemove, StringComparison.Ordinal);
        var builder = new StringBuilder();

        if (index == -1)
            return str;

        builder.Append(str.AsSpan(0, index));
        builder.Append(str.AsSpan(index + toRemove.Length));

        return builder.ToString();
    }
}
