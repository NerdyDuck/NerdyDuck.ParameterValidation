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
using NerdyDuck.CodedExceptions;
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.AllowedSchemeConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class AllowedSchemeConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint();
			Assert.AreEqual(Constraint.AllowedSchemeConstraintName, c.Name);
			Assert.IsNull(c.AllowedSchemes);
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint(Constants.SchemeName);
			Assert.AreEqual(Constraint.AllowedSchemeConstraintName, c.Name);
			Assert.IsNotNull(c.AllowedSchemes);
			List<string> TempSchemas = new List<string>(c.AllowedSchemes);
			Assert.AreEqual(Constants.SchemeName, TempSchemas[0]);
		}

		[TestMethod]
		public void Ctor_StringEmpty_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				AllowedSchemeConstraint c = new AllowedSchemeConstraint(string.Empty);
			});
		}

		[TestMethod]
		public void Ctor_IEnumerableString_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint(Constants.SchemeNames);
			Assert.AreEqual(Constraint.AllowedSchemeConstraintName, c.Name);
			Assert.IsNotNull(c.AllowedSchemes);
			List<string> TempSchemas = new List<string>(c.AllowedSchemes);
			Assert.AreEqual(3, TempSchemas.Count);
		}

		[TestMethod]
		public void Ctor_IEnumerableNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				AllowedSchemeConstraint c = new AllowedSchemeConstraint((IEnumerable<string>)null);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint(Constants.SchemeNames);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			AllowedSchemeConstraint c2 = SerializationHelper.Deserialize<AllowedSchemeConstraint>(Buffer);

			Assert.AreEqual(Constraint.AllowedSchemeConstraintName, c2.Name);
			Assert.IsNotNull(c2.AllowedSchemes);
			List<string> TempSchemas = new List<string>(c2.AllowedSchemes);
			Assert.AreEqual(3, TempSchemas.Count);
		}
#endif
		#endregion

		#region Public methods

		[TestMethod]
		public void ToString_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint(Constants.SchemeNames);
			Assert.AreEqual(string.Format("[AllowedScheme({0},{1},{2})]", Constants.SchemeNames[0], Constants.SchemeNames[1], Constants.SchemeNames[2]), c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint();
			c.SetParametersInternal(new string[] { Constants.SchemeNames[0], Constants.SchemeNames[1], Constants.SchemeNames[2] }, ParameterDataType.Uri);
			Assert.IsNotNull(c.AllowedSchemes);
			List<string> TempSchemas = new List<string>(c.AllowedSchemes);
			Assert.AreEqual(3, TempSchemas.Count);

		}

		[TestMethod]
		public void SetParameters_NoParamsOrNull_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				AllowedSchemeConstraint c = new AllowedSchemeConstraint();
				c.SetParametersInternal(new string[0], ParameterDataType.Uri);
			});
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				AllowedSchemeConstraint c = new AllowedSchemeConstraint();
				c.SetParametersInternal(new string[] { "1", "2", null, "4" }, ParameterDataType.Uri);
			});
		}

		[TestMethod]
		public void Validate_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint(Constants.SchemeNames);
			IEnumerable<ParameterValidationResult> res = c.Validate(new Uri("http://www.contoso.com"), ParameterDataType.Uri, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsFalse(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_UnknownSchema_Success()
		{
			AllowedSchemeConstraint c = new AllowedSchemeConstraint(Constants.SchemeNames);
			IEnumerable<ParameterValidationResult> res = c.Validate(new Uri("ftp://ftp.contoso.com"), ParameterDataType.Uri, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_SchemasNull_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				AllowedSchemeConstraint c = new AllowedSchemeConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate(new Uri("http://www.contoso.com"), ParameterDataType.Uri, Constants.MemberName);
			});
		}
		#endregion
	}
}
