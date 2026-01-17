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
using Concordion.Api.Listener;

namespace Concordion.Internal.Commands;

public class SpecificationCommand : AbstractCommand {
    private readonly List<SpecificationProcessingListener> listeners = [];

    public void AddSpecificationListener(
        SpecificationProcessingListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveSpecificationListener(
        SpecificationProcessingListener listener)
    {
        listeners.Remove(listener);
    }

    private void AnnounceAfterProcessingEvent(Resource resource,
        Element element)
    {
        foreach (var listener in listeners)
            listener.AfterProcessingSpecification(
                new SpecificationProcessingEvent(resource, element));
    }

    private void AnnounceBeforeProcessingEvent(Resource resource,
        Element element)
    {
        foreach (var listener in listeners)
            listener.BeforeProcessingSpecification(
                new SpecificationProcessingEvent(resource, element));
    }

    public override void Setup(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
        throw new InvalidOperationException(
            "Unexpected call to SpecificationCommand's SetUp() method. Only the Execute() method should be called.");
    }

    public override void Execute(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
        AnnounceBeforeProcessingEvent(commandCall.Resource,
            commandCall.Element);
        commandCall.Children.ProcessSequentially(evaluator, resultRecorder);
        AnnounceAfterProcessingEvent(commandCall.Resource,
            commandCall.Element);
    }

    public override void Verify(CommandCall commandCall,
        Evaluator evaluator, ResultRecorder resultRecorder)
    {
        throw new InvalidOperationException(
            "Unexpected call to SpecificationCommand's Verify() method. Only the Execute() method should be called.");
    }
}
