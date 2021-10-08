#region Copyright
/*******************************************************************************
 * <copyright file="CharacterSetConstraintTest.cs" owner="Daniel Kopp">
 * Copyright 2015-2016 Daniel Kopp
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
 * <file name="CharacterSetConstraintTest.cs" date="2015-11-11">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.CharacterSetConstraint class.
 * </file>
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.NullConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class CharacterSetConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint();
			Assert.AreEqual(Constraint.CharacterSetConstraintName, c.Name);
			Assert.AreEqual(CharacterSetConstraint.CharSet.Windows1252, c.CharacterSet);
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint(CharacterSetConstraint.CharSet.Ascii);
			Assert.AreEqual(Constraint.CharacterSetConstraintName, c.Name);
			Assert.AreEqual(CharacterSetConstraint.CharSet.Ascii, c.CharacterSet);
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint(CharacterSetConstraint.CharSet.Ascii);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			CharacterSetConstraint c2 = SerializationHelper.Deserialize<CharacterSetConstraint>(Buffer);

			Assert.AreEqual(Constraint.CharacterSetConstraintName, c2.Name);
			Assert.AreEqual(CharacterSetConstraint.CharSet.Ascii, c2.CharacterSet);
		}
#endif
		#endregion

		#region Public methods

		[TestMethod]
		public void ToString_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint(CharacterSetConstraint.CharSet.Ascii);
			Assert.AreEqual("[CharSet(Ascii)]", c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint();
			c.SetParametersInternal(new string[] { "Ascii" }, ParameterDataType.String);
			Assert.AreEqual(CharacterSetConstraint.CharSet.Ascii, c.CharacterSet);

		}

		[TestMethod]
		public void SetParameters_NoParamsOrInvalid_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				CharacterSetConstraint c = new CharacterSetConstraint();
				c.SetParametersInternal(new string[0], ParameterDataType.String);
			});
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				CharacterSetConstraint c = new CharacterSetConstraint();
				c.SetParametersInternal(new string[] { "EBCDIC" }, ParameterDataType.String);
			});
		}

		[TestMethod]
		public void Validate_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint(CharacterSetConstraint.CharSet.Ascii);
			IEnumerable<ParameterValidationResult> res = c.Validate("Hello World!\n", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsFalse(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_NoAscii_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint(CharacterSetConstraint.CharSet.Ascii);
			IEnumerable<ParameterValidationResult> res = c.Validate("Holà!", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_NoIso_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint(CharacterSetConstraint.CharSet.Iso8859);
			IEnumerable<ParameterValidationResult> res = c.Validate("Hello World\näφ", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_NoWin_Success()
		{
			CharacterSetConstraint c = new CharacterSetConstraint(CharacterSetConstraint.CharSet.Windows1252);
			IEnumerable<ParameterValidationResult> res = c.Validate("Hello World\nä€φ", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}
		#endregion
	}
}
