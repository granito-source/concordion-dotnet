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

using System.Text.RegularExpressions;
using Concordion.Api;

namespace Concordion.Internal;

public class ClassNameBasedSpecificationLocator(string suffix) :
    ISpecificationLocator {
    public ClassNameBasedSpecificationLocator() : this("html")
    {
    }

    public Resource LocateSpecification(object? fixture)
    {
        var fixtureName = fixture
            .GetType()
            .ToString()
            .Replace('.', Path.DirectorySeparatorChar);

        // Add Test und Fixture -> Case Sensitive
        fixtureName = Regex.Replace(fixtureName, "(Fixture|Test)$", "");

        //Suffix from Concordion.Specification.config
        var path = fixtureName + "." + suffix;

        return new Resource(path, fixture.GetType().Assembly.GetName().Name);
    }
}
