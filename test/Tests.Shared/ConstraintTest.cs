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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdyDuck.CodedExceptions;
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NerdyDuck.Tests.ParameterValidation
{
#if NET60
	namespace Net60
#elif NET50
	namespace Net50
#elif NETCORE31
	namespace NetCore31
#elif NET48
	namespace Net48
#endif
	{
		/// <summary>
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.Constraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class ConstraintTest
		{
			[TestMethod]
			public void Ctor_NameNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
				{
					DummyConstraint c = new DummyConstraint(null);

				});
			}

			[TestMethod]
			public void Ctor_InfoNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					DummyConstraint c = new DummyConstraint(null, new System.Runtime.Serialization.StreamingContext());
				});
			}

			[TestMethod]
			public void GetObjectData_InfoNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					LengthConstraint c = new LengthConstraint(5);
					c.GetObjectData(null, new System.Runtime.Serialization.StreamingContext());
				});
			}


			[TestMethod]
			public void AssertDataType_DataType_Error()
			{
				CustomAssert.ThrowsException<InvalidDataTypeException>(() =>
				{
					AllowedSchemeConstraint c = new AllowedSchemeConstraint(Constants.SchemeName);
					c.Validate(42, ParameterDataType.Int32, Constants.MemberName);

				});
			}

			[TestMethod]
			public void AssertDataType_DataTypeArray_Error()
			{
				CustomAssert.ThrowsException<InvalidDataTypeException>(() =>
				{
					LengthConstraint c = new LengthConstraint(5);
					c.Validate(42, ParameterDataType.Int32, Constants.MemberName);
				});
			}

			[TestMethod]
			public void AssertDataType_ExpectedTypesNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					DummyConstraint c = new DummyConstraint();
					c.AssertDataTypeTest(ParameterDataType.Bool, null);
				});
			}

			[TestMethod]
			public void GetParameters_ParamsNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					DummyConstraint c = new DummyConstraint();
					c.GetParametersTest(null);
				});
			}

			[TestMethod]
			public void SetParameters_ParamsNullOrNoDataType_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
				{
					Constraint c = new PasswordConstraint();
					c.SetParametersInternal(new string[0], ParameterDataType.None);
				});
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					Constraint c = new PasswordConstraint();
					c.SetParametersInternal(null, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void Validate_DataTypeNone_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
				{
					Constraint c = new PasswordConstraint();
					c.Validate(42, ParameterDataType.None, Constants.MemberName);
				});
			}

			[TestMethod]
			public void Validate_MemberNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
				{
					Constraint c = new PasswordConstraint();
					c.Validate(42, ParameterDataType.Int32, null, null);
				});
			}
			[TestMethod]
			public void Validate_DisplayNull_Success()
			{
				Constraint c = new PasswordConstraint();
				c.Validate(42, ParameterDataType.Int32, Constants.MemberName, null);
			}

			[TestMethod]
			public void Validate_ResultsNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					DummyConstraint c = new DummyConstraint();
					c.OnValidationTest(null, 1, ParameterDataType.Int32, Constants.MemberName, Constants.MemberName);
				});
			}

			[TestMethod]
			public void Validate_ValueNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					DummyConstraint c = new DummyConstraint();
					c.OnValidationTest(new List<ParameterValidationResult>(), null, ParameterDataType.Int32, Constants.MemberName, Constants.MemberName);
				});
			}

			[TestMethod]
			public void CheckErrorCodes()
			{
				Assert.AreEqual(0xb4, (int)ErrorCodes.LastErrorCode);
			}
		}
	}
}
