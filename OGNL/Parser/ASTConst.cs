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

using System.Text;

namespace OGNL.Parser;

/**
 * @author Luke Blanshard (blanshlu@netscape.net)
 * @author Drew Davidson (drew@ognl.org)
 */
internal class ASTConst(int id) : SimpleNode(id) {
    public object? Value { get; set; }

    protected override object? GetValueBody(OgnlContext context,
        object source)
    {
        return Value;
    }

    protected override bool IsNodeConstant(OgnlContext context)
    {
        return true;
    }

    private string GetEscapedChar(char ch)
    {
        string result;

        switch (ch) {
            case '\b':
                result = "\b";

                break;
            case '\t':
                result = "\\t";

                break;
            case '\n':
                result = "\\n";

                break;
            case '\f':
                result = "\\f";

                break;
            case '\r':
                result = "\\r";

                break;
            case '\"':
                result = "\\\"";

                break;
            case '\'':
                result = "\\\'";

                break;
            case '\\':
                result = "\\\\";

                break;
            default:
                // TODO: What's ISO Control.
                if (Util.IsIsoControl(ch) || ch > 255) {
                    var hc = ((int)ch).ToString("X");
                    var hcl = hc.Length;

                    result = "\\u";

                    if (hcl < 4) {
                        if (hcl == 3) {
                            result += "0";
                        } else {
                            if (hcl == 2) {
                                result += "00";
                            } else {
                                result += "000";
                            }
                        }
                    }

                    result += hc;
                } else
                    result = new string(new[] { ch });

                break;
        }

        return result;
    }

    private string GetEscapedString(string value)
    {
        var result = new StringBuilder();

        for (int i = 0, icount = value.Length; i < icount; i++)
            result.Append(GetEscapedChar(value[i]));

        return result.ToString();
    }

    public override string ToString()
    {
        if (Value == null)
            return "null";

        if (Value is string)
            return '\"' + GetEscapedString(Value.ToString()) + '\"';

        if (Value is char)
            return '\'' + GetEscapedChar((char)Value) + '\'';

        var result = Value.ToString() ?? "";

        if (Value is long)
            return result + "L";

        if (Value is decimal)
            return result + "B";

        if (Value is Node)
            return ":[ " + result + " ]";

        return result;
    }
}
