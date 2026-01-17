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

public class CommandRegistry : CommandFactory {
    private static string MakeKey(string namespaceUri, string commandName)
    {
        return namespaceUri + " " + commandName.ToLower();
    }

    private readonly Dictionary<string, Command> commandMap = new();

    public CommandRegistry Register(string namespaceUri,
        string commandName, Command command)
    {
        commandMap.Add(MakeKey(namespaceUri, commandName), command);

        return this;
    }

    public Command CreateCommand(string namespaceUri, string commandName)
    {
        return commandMap[MakeKey(namespaceUri, commandName)];
    }
}
