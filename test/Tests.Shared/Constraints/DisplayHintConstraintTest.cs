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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.DisplayHintConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class DisplayHintConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint();
			Assert.AreEqual(Constraint.DisplayHintConstraintName, c.Name);
			Assert.IsNull(c.Hints);
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeName);
			Assert.AreEqual(Constraint.DisplayHintConstraintName, c.Name);
			Assert.IsNotNull(c.Hints);
			List<string> TempSchemas = new List<string>(c.Hints);
			Assert.AreEqual(Constants.SchemeName, TempSchemas[0]);
		}

		[TestMethod]
		public void Ctor_StringEmpty_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint(string.Empty);
			});
		}

		[TestMethod]
		public void Ctor_IEnumerableString_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			Assert.AreEqual(Constraint.DisplayHintConstraintName, c.Name);
			Assert.IsNotNull(c.Hints);
			List<string> TempSchemas = new List<string>(c.Hints);
			Assert.AreEqual(3, TempSchemas.Count);
		}

		[TestMethod]
		public void Ctor_IEnumerableNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint((IEnumerable<string>)null);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			DisplayHintConstraint c2 = SerializationHelper.Deserialize<DisplayHintConstraint>(Buffer);

			Assert.AreEqual(Constraint.DisplayHintConstraintName, c2.Name);
			Assert.IsNotNull(c2.Hints);
			List<string> TempSchemas = new List<string>(c2.Hints);
			Assert.AreEqual(3, TempSchemas.Count);
		}
#endif
		#endregion

		#region Public methods

		[TestMethod]
		public void ToString_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			Assert.AreEqual(string.Format("[DisplayHint({0},{1},{2})]", Constants.SchemeNames[0], Constants.SchemeNames[1], Constants.SchemeNames[2]), c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint();
			c.SetParametersInternal(new string[] { Constants.SchemeNames[0], Constants.SchemeNames[1], Constants.SchemeNames[2] }, ParameterDataType.String);
			Assert.IsNotNull(c.Hints);
			List<string> TempSchemas = new List<string>(c.Hints);
			Assert.AreEqual(3, TempSchemas.Count);

		}

		[TestMethod]
		public void SetParameters_NoParamsOrNull_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint();
				c.SetParametersInternal(new string[0], ParameterDataType.String);
			});
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint();
				c.SetParametersInternal(new string[] { "1", "2", null, "4" }, ParameterDataType.String);
			});
		}

		[TestMethod]
		public void Validate_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Int32, Constants.MemberName);
			Assert.IsNotNull(result);
			List<ParameterValidationResult> Temp = new List<ParameterValidationResult>(result);
			Assert.AreEqual(0, Temp.Count);
		}
		#endregion
	}
}
