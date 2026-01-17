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

using Concordion.Api;

namespace Concordion.Internal;

public class Row(Element element) {
    public Element RowElement { get; } = element;

    public bool IsHeaderRow =>
        RowElement.GetChildElements().All(cell => !cell.IsNamed("td"));

    public IList<Element> GetCells()
    {
        return RowElement
            .GetChildElements()
            .Where(child => child.IsNamed("td") || child.IsNamed("th"))
            .ToList();
    }

    public int GetIndexOfCell(Element element)
    {
        var index = 0;

        foreach (var cell in RowElement.GetChildElements()) {
            if (element.Text == cell.Text)
                return index;

            index++;
        }

        return -1;
    }
}
