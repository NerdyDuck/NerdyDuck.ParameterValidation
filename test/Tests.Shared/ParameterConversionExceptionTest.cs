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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.ParameterConversionException class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class ParameterConversionExceptionTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				try
				{
					throw new ParameterConversionException();
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.None, ex.DataType);
					Assert.IsNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void Ctor_String_Success()
			{
				try
				{
					throw new ParameterConversionException(Constants.TestMessage);
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.None, ex.DataType);
					Assert.IsNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void Ctor_StringParameterDataTypeObject_Success()
			{
				try
				{
					throw new ParameterConversionException(Constants.TestMessage, ParameterDataType.Int32, 42);
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.Int32, ex.DataType);
					Assert.IsNotNull(ex.ActualValue);
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
						throw new ParameterConversionException(Constants.TestMessage, ex);
					}
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.None, ex.DataType);
					Assert.IsNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void Ctor_StringParameterDataTypeObjectException_Success()
			{
				try
				{
					try
					{
						throw new FormatException();
					}
					catch (Exception ex)
					{
						throw new ParameterConversionException(Constants.TestMessage, ParameterDataType.Int32, 42, ex);
					}
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.Int32, ex.DataType);
					Assert.IsNotNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void Ctor_Int32_Success()
			{
				try
				{
					throw new ParameterConversionException(Constants.CustomHResult);
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.None, ex.DataType);
					Assert.IsNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void Ctor_IntStringParameterDataTypeObject_Success()
			{
				try
				{
					throw new ParameterConversionException(Constants.CustomHResult, Constants.TestMessage, ParameterDataType.Int32, 42);
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.Int32, ex.DataType);
					Assert.IsNotNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void Ctor_IntString_Success()
			{
				try
				{
					throw new ParameterConversionException(Constants.CustomHResult, Constants.TestMessage);
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.None, ex.DataType);
					Assert.IsNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void Ctor_IntStringParameterDataTypeObjectException_Success()
			{
				try
				{
					try
					{
						throw new FormatException();
					}
					catch (Exception ex)
					{
						throw new ParameterConversionException(Constants.CustomHResult, Constants.TestMessage, ParameterDataType.Int32, 42, ex);
					}
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.Int32, ex.DataType);
					Assert.IsNotNull(ex.ActualValue);
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
						throw new ParameterConversionException(Constants.CustomHResult, Constants.TestMessage, ex);
					}
				}
				catch (ParameterConversionException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.None, ex.DataType);
					Assert.IsNull(ex.ActualValue);
				}
			}

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
						throw new ParameterConversionException(Constants.CustomHResult, Constants.TestMessage,	ParameterDataType.Int32, 42, ex);
					}
				}
				catch (ParameterConversionException ex)
				{
					System.IO.MemoryStream Buffer = SerializationHelper.Serialize(ex);
					ParameterConversionException ex2 = SerializationHelper.Deserialize<ParameterConversionException>(Buffer);

					Assert.AreEqual(Constants.CustomHResult, ex2.HResult);
					Assert.IsNotNull(ex2.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex2.Message);
					Assert.AreEqual<ParameterDataType>(ParameterDataType.Int32, ex.DataType);
					Assert.IsNotNull(ex.ActualValue);
				}
			}

			[TestMethod]
			public void ToString_Success()
			{
				try
				{
					throw new ParameterConversionException(Constants.CustomHResult, Constants.TestMessage);
				}
				catch (Exception ex)
				{
					string str = ex.ToString();
					StringAssert.StartsWith(str, string.Format("{0}: ({1}) {2}", typeof(ParameterConversionException).FullName, Constants.CustomHResultString, Constants.TestMessage));
					StringAssert.Contains(str, "ToString_Success");
				}
			}
		}
	}
}
