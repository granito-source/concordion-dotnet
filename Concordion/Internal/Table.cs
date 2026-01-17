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

public class Table {
    private Element TableElement { get; }

    private long ColumnCount => GetFirstHeaderRow().GetCells().Count;

    public Table(Element element)
    {
        if (!element.IsNamed("table"))
            throw new ArgumentException(
                @"This strategy can only work on table elements",
                nameof(element));

        TableElement = element;
    }

    public IList<Row> GetRows()
    {
        return TableElement
            .GetDescendantElements("tr")
            .Select(rowElement => new Row(rowElement))
            .ToList();
    }

    public IList<Row> GetHeaderRows()
    {
        return GetRows()
            .Where(row => row.IsHeaderRow)
            .ToList();
    }

    public IList<Row> GetDetailRows()
    {
        return GetRows()
            .Where(row => !row.IsHeaderRow)
            .ToList();
    }

    public Row GetLastHeaderRow()
    {
        var headerRows = GetHeaderRows();

        return headerRows.Count == 0 ?
            throw new Exception("Table has no header row (i.e. no row containing only <th> elements)") :
            headerRows[^1];
    }

    public Row GetFirstHeaderRow()
    {
        var headerRows = GetHeaderRows();

        return headerRows.Count == 0 ?
            throw new Exception("Table has no header row (i.e. no row containing only <th> elements)") :
            headerRows[0];
    }

    public Row AddDetailRow()
    {
        var rowElement = new Element("tr");
        var tbody = TableElement.GetFirstChildElement("tbody");

        if (tbody != null)
            tbody.AppendChild(rowElement);
        else
            TableElement.AppendChild(rowElement);

        for (var i = 0; i < ColumnCount; i++)
            rowElement.AppendChild(new Element("td"));

        return new Row(rowElement);
    }
}
