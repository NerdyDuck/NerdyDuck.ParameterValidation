#region Copyright
/*******************************************************************************
 * <copyright file="ConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="ConstraintTest.cs" date="2015-11-11">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.Constraint class.
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

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.Constraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_NameNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				DummyConstraint c = new DummyConstraint(null);

			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_InfoNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				DummyConstraint c = new DummyConstraint(null, new System.Runtime.Serialization.StreamingContext());
			});
		}
#endif
		#endregion

		#region Public methods
#if WINDOWS_DESKTOP
		[TestMethod]
		public void GetObjectData_InfoNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				LengthConstraint c = new LengthConstraint(5);
				c.GetObjectData(null, new System.Runtime.Serialization.StreamingContext());
			});
		}
#endif


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
		#endregion
	}
}
