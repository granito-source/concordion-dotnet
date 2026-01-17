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

namespace Concordion.Internal.Commands;

public abstract class AbstractCommandDecorator(Command command) : Command {
    protected readonly Command Command = command;

    public abstract void Setup(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder);

    public abstract void Execute(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder);

    public abstract void Verify(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder);
}
