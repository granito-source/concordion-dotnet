//--------------------------------------------------------------------------
//	Copyright (c) 1998-2004, Drew Davidson and Luke Blanshard
//  All rights reserved.
//
//	Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are
//  met:
//
//	Redistributions of source code must retain the above copyright notice,
//  this list of conditions and the following disclaimer.
//	Redistributions in binary form must reproduce the above copyright
//  notice, this list of conditions and the following disclaimer in the
//  documentation and/or other materials provided with the distribution.
//	Neither the name of the Drew Davidson nor the names of its contributors
//  may be used to endorse or promote products derived from this software
//  without specific prior written permission.
//
//	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
//  FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
//  COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
//  BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
//  OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
//  AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//  OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF
//  THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
//  DAMAGE.
//--------------------------------------------------------------------------

using OGNL.Parser;

namespace OGNL;

/// <summary>
/// An <b>Evaluation</b> is and object that holds a node being evaluated
/// and the source from which that node will take extract its
/// value.
/// </summary>
/// <remarks>It refers to child evaluations that occur as
/// a result of the nodes' evaluation.
/// </remarks>
public class Evaluation(Node node, object source,
    bool setOperation = false) {
    /** The result of the evaluation. */
    public object? Result { private get; set; }

    /** An exception captured while evaluating. */
    public Exception? Exception { private get; set; }

    /** The parent of the evaluation. */
    public Evaluation? Parent { get; private set; }

    private Evaluation? next;

    private Evaluation? firstChild;

    private Evaluation? lastChild;

    /// <summary>
    /// Adds a child to the list of children of this evaluation.  The
    /// parent of the child is set to the receiver and the children
    /// references are modified in the receiver to reflect the new child.
    /// The lastChild of the receiver is set to the child, and the
    /// firstChild is set also if child is the first (or only) child.
    /// </summary>
    public void AddChild(Evaluation child)
    {
        if (firstChild == null)
            firstChild = lastChild = child;
        else {
            if (firstChild == lastChild) {
                firstChild.next = child;
                lastChild = child;
            } else {
                lastChild!.next = child;
                lastChild = child;
            }
        }

        child.Parent = this;
    }

    private string ToString(bool compact, bool showChildren, string depth)
    {
        string stringResult;

        if (compact)
            stringResult = $"{depth}<{node.GetType().Name} {GetHashCode()}>";
        else {
            var ss = source.GetType().Name;
            var rs = Result == null ? "null" : Result.GetType().Name;

            stringResult = $"{depth}<{node.GetType().Name}: [{(setOperation ? "set" : "get")}] source = {ss}, result = {Result} [{rs}]>";
        }

        if (!showChildren)
            return stringResult;

        var child = firstChild;

        stringResult += "\n";

        while (child != null) {
            stringResult += child.ToString(compact, depth + "  ");
            child = child.next;
        }

        return stringResult;
    }

    private string ToString(bool compact, string depth)
    {
        return ToString(compact, true, depth);
    }

    /// <summary>
    /// Returns a string description of the Evaluation.
    /// </summary>
    public override string ToString()
    {
        return ToString(false, "");
    }
}
