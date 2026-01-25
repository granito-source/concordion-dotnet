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

namespace OGNL;

/// <summary>
/// This interface defines some useful constants for describing the various possible
/// numeric types of OGNL.
/// </summary>
internal static class NumericTypes {
    // Order does matter here... see the getNumericType methods in ognl.g.

    /// <summary>Type tag meaning bool.</summary>
    public const int Bool = 0;

    /// <summary>Type tag meaning byte.</summary>
    public const int Byte = 1;

    /// <summary>Type tag meaning char.</summary>
    public const int Char = 2;

    /// <summary>Type tag meaning short.</summary>
    public const int Short = 3;

    /// <summary>Type tag meaning int.</summary>
    public const int Int = 4;

    /// <summary>Type tag meaning long.</summary>
    public const int Long = 5;

    /// <summary>Type tag meaning java.math.BigInteger.</summary>
    public const int BigInt = 6;

    /// <summary>Type tag meaning float.</summary>
    public const int Float = 7;

    /// <summary>Type tag meaning double.</summary>
    public const int Double = 8;

    /// <summary>Type tag meaning java.math.BigDecimal.</summary>
    public const int BigDec = 9;

    /// <summary>Type tag meaning something other than a number.</summary>
    public const int NonNumeric = 10;

    /// <summary>
    /// The smallest type tag that represents reals as opposed to
    /// integers. You can see whether a type tag represents reals or
    /// integers by comparing the tag to this constant: all tags less
    /// than this constant represent integers, and all tags greater
    /// than or equal to this constant represent reals. Of course, you
    /// must also check for <c>NonNumeric</c>, which means it is not
    /// a number at all.
    /// </summary>
    public const int MinRealType = Float;
}
