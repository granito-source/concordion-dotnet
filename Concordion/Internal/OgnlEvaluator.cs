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

using System.Data;
using Concordion.Api;
using Concordion.Internal.Util;
using OGNL;

namespace Concordion.Internal;

public class OgnlEvaluator(object fixture) : Evaluator {
    private static void AssertStartsWithHash(string expression)
    {
        if (!expression.StartsWith('#'))
            throw new InvalidExpressionException(
                $"""
                Variable for concordion:set must start with '#'
                 (i.e. change concordion:set="{expression}" to concordion:set="#{expression}".
                """);
    }

    public object Fixture { get; } = fixture;

    private readonly OgnlContext ognlContext = new();

    public object? GetVariable(string expression)
    {
        AssertStartsWithHash(expression);

        return ognlContext[expression[1..]];
    }

    public virtual void SetVariable(string expression, object? value)
    {
        AssertStartsWithHash(expression);

        if (expression.Contains('='))
            Evaluate(expression);
        else
            PutVariable(expression[1..], value);
    }

    public virtual object? Evaluate(string expression)
    {
        return Ognl.GetValue(expression, ognlContext, Fixture);
    }

    private void PutVariable(string variable, object? value)
    {
        Check.IsFalse(variable.StartsWith('#'),
            "Variable name passed to evaluator should not start with #");
        Check.IsTrue(!variable.Equals("in"),
            "'%s' is a reserved word and cannot be used for variables names",
            variable);

        ognlContext[variable] = value;
    }
}
