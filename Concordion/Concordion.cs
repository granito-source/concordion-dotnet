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
using Concordion.Internal;

namespace Concordion;

public class Concordion(SpecificationLocator specificationLocator,
    SpecificationReader specificationReader,
    EvaluatorFactory evaluatorFactory) {
    public ResultSummary Process(object? fixture)
    {
        return Process(specificationLocator.LocateSpecification(fixture),
            fixture);
    }

    public ResultSummary Process(Resource resource, object? fixture)
    {
        var specification = specificationReader.ReadSpecification(resource);
        var resultRecorder = new SummarizingResultRecorder();

        specification.Process(evaluatorFactory.CreateEvaluator(fixture),
            resultRecorder);

        return resultRecorder;
    }
}
