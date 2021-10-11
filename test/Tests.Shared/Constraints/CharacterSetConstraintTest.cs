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
