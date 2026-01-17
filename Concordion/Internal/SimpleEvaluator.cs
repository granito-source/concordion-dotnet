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

namespace Concordion.Internal;

public class SimpleEvaluator(object fixture) : OgnlEvaluator(fixture) {
    private const string MethodNamePattern = "[a-z][a-zA-Z0-9_]*";

    private const string PropertyNamePattern = "[a-z][a-zA-Z0-9_]*";

    private const string StringPattern = "'[^']+'";

    private const string LhsVariablePattern = "#" + MethodNamePattern;

    private const string RhsVariablePattern = "(" + LhsVariablePattern +
        "|#TEXT|#HREF|#LEVEL)";

    private static void ValidateEvaluationExpression(string expression)
    {
        const string methodCallParams = MethodNamePattern +
            " *\\( *" + RhsVariablePattern +
            "(, *" + RhsVariablePattern + " *)*\\)";
        const string methodCallNoParams = MethodNamePattern + " *\\( *\\)";
        const string ternaryStringResult = " \\? " + StringPattern +
            " : " + StringPattern;
        var regexPatterns = new List<string> {
            PropertyNamePattern,
            methodCallNoParams,
            methodCallParams,
            RhsVariablePattern,
            LhsVariablePattern + "(\\." + PropertyNamePattern + ")+",
            LhsVariablePattern + " *= *" + PropertyNamePattern,
            LhsVariablePattern + " *= *" + methodCallNoParams,
            LhsVariablePattern + " *= *" + methodCallParams,
            LhsVariablePattern + ternaryStringResult,
            PropertyNamePattern + ternaryStringResult,
            methodCallNoParams + ternaryStringResult,
            methodCallParams + ternaryStringResult,
            LhsVariablePattern + "\\." + methodCallNoParams,
            LhsVariablePattern + "\\." + methodCallParams
        };

        expression = expression.Trim();

        if (regexPatterns.Any(regex => Regex.IsMatch(expression, regex)))
            return;

        throw new InvalidOperationException($"Invalid expression [{expression}]");
    }

    private static void ValidateSetVariableExpression(string expression)
    {
        var regexPatterns = new List<string> {
            RhsVariablePattern,
            LhsVariablePattern + "\\." + PropertyNamePattern,
            LhsVariablePattern + " *= *" + PropertyNamePattern,
            LhsVariablePattern + " *= *" + MethodNamePattern + " *\\( *\\)",
            LhsVariablePattern + " *= *" + MethodNamePattern + " *\\( *" +
                RhsVariablePattern + "(, *" + RhsVariablePattern + " *)*\\)"
        };

        expression = expression.Trim();

        if (regexPatterns.Any(regex => Regex.IsMatch(expression, regex)))
            return;

        throw new InvalidOperationException($"Invalid expression [{expression}]");
    }

    public override object? Evaluate(string expression)
    {
        ValidateEvaluationExpression(expression);

        return base.Evaluate(expression);
    }

    public override void SetVariable(string expression, object? value)
    {
        ValidateSetVariableExpression(expression);
        base.SetVariable(expression, value);
    }
}
