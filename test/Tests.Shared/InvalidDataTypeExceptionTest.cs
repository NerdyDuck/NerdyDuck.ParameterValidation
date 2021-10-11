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
using System;
using NerdyDuck.ParameterValidation.Constraints;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.InvalidDataTypeException class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class InvalidDataTypeExceptionTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			try
			{
				throw new InvalidDataTypeException();
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.IsNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.None, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			try
			{
				throw new InvalidDataTypeException(Constants.TestMessage);
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.None, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_StringException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new InvalidDataTypeException(Constants.TestMessage, ex);
				}
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.None, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_Int32_Success()
		{
			try
			{
				throw new InvalidDataTypeException(Constants.CustomHResult);
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.IsNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.None, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_IntString_Success()
		{
			try
			{
				throw new InvalidDataTypeException(Constants.CustomHResult, Constants.TestMessage);
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.None, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_IntStringException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new InvalidDataTypeException(Constants.CustomHResult, Constants.TestMessage, ex);
				}
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.None, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_Int32Constraint_Success()
		{
			try
			{
				throw new InvalidDataTypeException(Constants.CustomHResult, new NullConstraint(), ParameterDataType.Int32);
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.IsNotNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.Int32, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_Int32ConstraintNull_Success()
		{
			try
			{
				throw new InvalidDataTypeException(Constants.CustomHResult, null, ParameterDataType.Int32);
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.IsNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.Int32, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_IntStringConstraint_Success()
		{
			try
			{
				throw new InvalidDataTypeException(Constants.CustomHResult, Constants.TestMessage, new NullConstraint(), ParameterDataType.Int32);
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNotNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.Int32, ex.DataType);
			}
		}

		[TestMethod]
		public void Ctor_IntStringConstraintException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new InvalidDataTypeException(Constants.CustomHResult, Constants.TestMessage, new NullConstraint(), ParameterDataType.Int32, ex);
				}
			}
			catch (InvalidDataTypeException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNotNull(ex.Constraint);
				Assert.AreEqual(ParameterDataType.Int32, ex.DataType);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new InvalidDataTypeException(Constants.CustomHResult, Constants.TestMessage, new NullConstraint(), ParameterDataType.Int32, ex);
				}
			}
			catch (InvalidDataTypeException ex)
			{
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(ex);
				InvalidDataTypeException ex2 = SerializationHelper.Deserialize<InvalidDataTypeException>(Buffer);

				Assert.AreEqual(Constants.CustomHResult, ex2.HResult);
				Assert.IsNotNull(ex2.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex2.Message);
				Assert.IsNotNull(ex2.Constraint);
				Assert.AreEqual(ParameterDataType.Int32, ex2.DataType);
			}
		}
#endif
		#endregion

		#region ToString
		[TestMethod]
		public void ToString_Success()
		{
			try
			{
				throw new InvalidDataTypeException(Constants.CustomHResult, Constants.TestMessage);
			}
			catch (Exception ex)
			{
				string str = ex.ToString();
				StringAssert.StartsWith(str, string.Format("{0}: ({1}) {2}", typeof(InvalidDataTypeException).FullName, Constants.CustomHResultString, Constants.TestMessage));
				StringAssert.Contains(str, "ToString_Success");
			}
		}
		#endregion
	}
}
