#region Copyright
/*******************************************************************************
 * <copyright file="TypeConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="TypeConstraintTest.cs" date="2015-11-16">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.TypeConstraint class.
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
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NerdyDuck.CodedExceptions;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.TypeConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class TypeConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			TypeConstraint c = new TypeConstraint();
			Assert.AreEqual(Constraint.TypeConstraintName, c.Name);
			Assert.AreEqual(string.Empty, c.TypeName);
			Assert.IsNull(c.ResolvedType);
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			TypeConstraint c = new TypeConstraint(type);
			Assert.AreEqual(Constraint.TypeConstraintName, c.Name);
			Assert.AreEqual(type, c.TypeName);
			Assert.IsNotNull(c.ResolvedType);
			Assert.IsNotNull(c.ResolvedType); // To trigger retrieval of already resolved type
		}

		[TestMethod]
		public void Ctor_StringUnknownType_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName.Replace("UriKind", "MyClass");
			TypeConstraint c = new TypeConstraint(type);
			Assert.AreEqual(Constraint.TypeConstraintName, c.Name);
			Assert.AreEqual(type, c.TypeName);
			Assert.IsNull(c.ResolvedType);
		}

		[TestMethod]
		public void Ctor_StringNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				TypeConstraint c = new TypeConstraint(null);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			TypeConstraint c = new TypeConstraint(type);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			TypeConstraint c2 = SerializationHelper.Deserialize<TypeConstraint>(Buffer);

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
			TypeConstraint c = new TypeConstraint(type);
			Assert.AreEqual(string.Format("[Type('{0}')]", type), c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			TypeConstraint c = new TypeConstraint();
			c.SetParametersInternal(new string[] { type }, ParameterDataType.Xml);
			Assert.AreEqual(type, c.TypeName);
			Assert.IsNotNull(c.ResolvedType);
		}

		[TestMethod]
		public void SetParameters_TooManyParams_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				TypeConstraint c = new TypeConstraint();
				c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.Xml);
			});
		}

		[TestMethod]
		public void SetParameters_ParamWhitespace_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				TypeConstraint c = new TypeConstraint();
				c.SetParametersInternal(new string[] { "    " }, ParameterDataType.Xml);
			});
		}

		[TestMethod]
		public void Validate_Success()
		{
			string type = typeof(System.UriKind).AssemblyQualifiedName;
			TypeConstraint c = new TypeConstraint(type);
			IEnumerable<ParameterValidationResult> result = c.Validate(System.UriKind.Absolute, ParameterDataType.Xml, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}
		#endregion
	}
}
