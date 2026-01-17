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

namespace Concordion.Internal;

public class TableSupport {
    public long ColumnCount => GetLastHeaderRow().GetCells().Count;

    private CommandCall TableCommandCall { get; }

    private Table Table { get; }

    private IDictionary<int, CommandCall> CommandCallByColumn { get; }

    public TableSupport(CommandCall tableCommandCall)
    {
        if (!tableCommandCall.Element.IsNamed("table"))
            throw new ArgumentException(
                @"This strategy can only work on table elements",
                nameof(tableCommandCall));

        TableCommandCall = tableCommandCall;
        Table = new Table(tableCommandCall.Element);

        CommandCallByColumn = new Dictionary<int, CommandCall>();
        PopulateCommandCallByColumnMap();
    }

    private void PopulateCommandCallByColumnMap()
    {
        var headerRow = GetLastHeaderRow();
        var children = TableCommandCall.Children;

        foreach (var childCall in children) {
            var columnIndex = headerRow.GetIndexOfCell(childCall.Element);

            if (columnIndex == -1)
                throw new Exception(
                    "Commands must be placed on <th> elements when using 'execute' or 'verifyRows' commands on a <table>.");

            CommandCallByColumn.Add(columnIndex, childCall);
        }
    }

    public void CopyCommandCallsTo(Row detailRow)
    {
        var columnIndex = 0;

        foreach (var cell in detailRow.GetCells()) {
            if (CommandCallByColumn.TryGetValue(columnIndex, out var cellCall))
                cellCall.Element = cell;

            columnIndex++;
        }
    }

    public IList<Row> GetDetailRows()
    {
        return Table.GetDetailRows();
    }

    public Row AddDetailRow()
    {
        return Table.AddDetailRow();
    }

    public Row GetLastHeaderRow()
    {
        return Table.GetLastHeaderRow();
    }
}
