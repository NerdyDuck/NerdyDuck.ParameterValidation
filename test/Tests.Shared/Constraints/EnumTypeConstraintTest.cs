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

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
#if WINDOWS_DESKTOP
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
#endif
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NerdyDuck.CodedExceptions;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.EnumTypeConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class EnumTypeConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			EnumTypeConstraint c = new EnumTypeConstraint();
			Assert.AreEqual(Constraint.TypeConstraintName, c.Name);
			Assert.AreEqual(string.Empty, c.TypeName);
			Assert.IsNull(c.ResolvedType);
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			Assert.AreEqual(Constraint.TypeConstraintName, c.Name);
			Assert.AreEqual(type, c.TypeName);
			Assert.IsNotNull(c.ResolvedType);
			Assert.IsFalse(c.HasFlags);
			Assert.AreEqual(3, c.EnumValues.Count);
			Assert.IsNotNull(c.ResolvedType); // To trigger retrieval of already resolved type
		}

		[TestMethod]
		public void Ctor_StringUnknownType_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName.Replace("UriKind", "MyClass");
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			Assert.AreEqual(Constraint.TypeConstraintName, c.Name);
			Assert.AreEqual(type, c.TypeName);
			Assert.IsNull(c.ResolvedType);
		}

		[TestMethod]
		public void Ctor_StringNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				EnumTypeConstraint c = new EnumTypeConstraint(null);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			EnumTypeConstraint c2 = SerializationHelper.Deserialize<EnumTypeConstraint>(Buffer);

			Assert.AreEqual(Constraint.TypeConstraintName, c2.Name);
			Assert.AreEqual(type, c2.TypeName);
			Assert.IsNotNull(c2.ResolvedType);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void ToString_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			Assert.AreEqual(string.Format("[Type('{0}')]", type), c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint();
			c.SetParametersInternal(new string[] { type }, ParameterDataType.Enum);
			Assert.AreEqual(type, c.TypeName);
			Assert.IsNotNull(c.ResolvedType);
		}
		[TestMethod]
		public void SetParameters_ParamsNullOrNoDataType_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				EnumTypeConstraint c = new EnumTypeConstraint();
				c.SetParametersInternal(new string[0], ParameterDataType.None);
			});
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				EnumTypeConstraint c = new EnumTypeConstraint();
				c.SetParametersInternal(null, ParameterDataType.Enum);
			});
		}

		[TestMethod]
		public void SetParameters_TooManyParams_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				EnumTypeConstraint c = new EnumTypeConstraint();
				c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.Enum);
			});
		}

		[TestMethod]
		public void SetParameters_ParamWhitespace_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				EnumTypeConstraint c = new EnumTypeConstraint();
				c.SetParametersInternal(new string[] { "    " }, ParameterDataType.Enum);
			});
		}

		[TestMethod]
		public void Validate_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(System.UriKind.Absolute, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_FlagsEnum_Success()
		{
			string type = typeof(System.Text.RegularExpressions.RegexOptions).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvEnum_Success()
		{
			string type = typeof(System.DayOfWeek).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate((System.DayOfWeek)0x80, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvFlagsEnum_Success()
		{
			string type = typeof(System.Text.RegularExpressions.RegexOptions).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate((System.Text.RegularExpressions.RegexOptions)0x80, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_ValueNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				string type = typeof(System.UriKind).AssemblyQualifiedName;
				EnumTypeConstraint c = new EnumTypeConstraint(type);
				IEnumerable<ParameterValidationResult> result = c.Validate(null, ParameterDataType.Enum, Constants.MemberName);
			});
		}

		[TestMethod]
		public void Validate_UnknownType_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName.Replace("UriKind", "MyClass");
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(System.UriKind.Absolute, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_NotEnum_Success()
		{
			string type = typeof(System.Uri).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(System.UriKind.Absolute, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_WrongEnum_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(System.StringComparison.CurrentCulture, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvValue_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(10, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_ValueFlagsInt_Success()
		{
			string type = typeof(System.Text.RegularExpressions.RegexOptions).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvValueFlagInt_Success()
		{
			string type = typeof(System.Text.RegularExpressions.RegexOptions).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(128, ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvValueType_Success()
		{
			string type = typeof(System.Text.RegularExpressions.RegexOptions).AssemblyQualifiedName;
			EnumTypeConstraint c = new EnumTypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate("narf", ParameterDataType.Enum, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}
		#endregion
	}
}
