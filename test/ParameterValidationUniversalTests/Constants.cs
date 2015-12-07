#region Copyright
/*******************************************************************************
 * <copyright file="Constants.cs" owner="Daniel Kopp">
 * Copyright 2015 Daniel Kopp
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * </copyright>
 * <author name="Daniel Kopp" email="dak@nerdyduck.de" />
 * <assembly name="NerdyDuck.Tests.ParameterValidation">
 * Unit tests for NerdyDuck.ParameterValidation assembly.
 * </assembly>
 * <file name="Constants.cs" date="2015-11-09">
 * Constant values for recurring test scenarios.
 * </file>
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
