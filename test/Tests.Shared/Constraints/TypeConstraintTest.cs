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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.TypeConstraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class TypeConstraintTest
		{
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
		}
	}
}
