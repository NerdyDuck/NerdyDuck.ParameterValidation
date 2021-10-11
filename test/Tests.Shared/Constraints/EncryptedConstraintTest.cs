﻿#region Copyright
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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.NullConstraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class EncryptedConstraintTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				EncryptedConstraint c = new EncryptedConstraint();
				Assert.AreEqual(Constraint.EncryptedConstraintName, c.Name);
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				EncryptedConstraint c = new EncryptedConstraint();
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
				EncryptedConstraint c2 = SerializationHelper.Deserialize<EncryptedConstraint>(Buffer);

				Assert.AreEqual(Constraint.EncryptedConstraintName, c2.Name);
			}

			[TestMethod]
			public void Validate_Success()
			{
				EncryptedConstraint c = new EncryptedConstraint();
				IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Int32, Constants.MemberName);
				Assert.IsNotNull(result);
				List<ParameterValidationResult> Temp = new List<ParameterValidationResult>(result);
				Assert.AreEqual(0, Temp.Count);
			}
		}
	}
}
