#region Copyright
/*******************************************************************************
 * NerdyDuck.Tests.Collections - Unit tests for the
 * NerdyDuck.ParameterValidation assembly
 * 
 * The MIT License (MIT)
 *
 * Copyright (c) Daniel Kopp, dak@nerdyduck.de
 *
 * All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 ******************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Constant values for recurring test scenarios.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
	public static class Constants
	{
		public const int COR_E_FORMAT = unchecked((int)0x80131537);
		public const int CustomHResult = unchecked((int)0xa7ff1234);
		public const string CustomHResultString = "0xa7ff1234";
		public const string TestMessage = "[TestMessage]";
		public const string MemberName = "RandomName";
		public const string EntityName = "MyEntity";
		public const string KeyName = "Id";
		public const string DisplayName = "Name";
		public const string SchemeName = "http";
		public const string RegexString = "^[a-z]*$";
		public static readonly string[] SchemeNames = new string[] { "http", "https", "unkn" };
		public const string SimpleConstraintString = "[Null]";
		public const string MinValueConstraintString = "[MinValue(40)]";
		public const string ErrorMessage = "Unlocalized error message";
		public const string TestUri = "http://www.contoso.com/";
		public const string SerializedObjectString = "<?xml version=\"1.0\" encoding=\"utf-16\"?><XmlSerializableObject MyValue=\"narf\" />";
		public const string EncryptedTestMessage = "Ibw3FKVrZyZveXz8pmHx+A==";
	}
}
