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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.ConstraintParserException class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class ConstraintParserExceptionTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				try
				{
					throw new ConstraintParserException();
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.AreEqual(-1, ex.Position);
					Assert.IsNull(ex.InnerException);
				}
			}

			[TestMethod]
			public void Ctor_String_Success()
			{
				try
				{
					throw new ConstraintParserException(Constants.TestMessage);
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.AreEqual(-1, ex.Position);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
				}
			}

			[TestMethod]
			public void Ctor_StringInt_Success()
			{
				try
				{
					throw new ConstraintParserException(Constants.TestMessage, 42);
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.AreEqual(42, ex.Position);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
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
						throw new ConstraintParserException(Constants.TestMessage, ex);
					}
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.AreEqual(-1, ex.Position);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
				}
			}

			[TestMethod]
			public void Ctor_StringIntException_Success()
			{
				try
				{
					try
					{
						throw new FormatException();
					}
					catch (Exception ex)
					{
						throw new ConstraintParserException(Constants.TestMessage, 42, ex);
					}
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
					Assert.AreEqual(42, ex.Position);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
				}
			}

			[TestMethod]
			public void Ctor_Int32_Success()
			{
				try
				{
					throw new ConstraintParserException(Constants.CustomHResult);
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.AreEqual(-1, ex.Position);
					Assert.IsNull(ex.InnerException);
				}
			}

			[TestMethod]
			public void Ctor_IntString_Success()
			{
				try
				{
					throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage);
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.AreEqual(-1, ex.Position);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
				}
			}

			[TestMethod]
			public void Ctor_IntStringInt_Success()
			{
				try
				{
					throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, 42);
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.AreEqual(42, ex.Position);
					Assert.IsNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
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
						throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, ex);
					}
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.AreEqual(-1, ex.Position);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
				}
			}

			[TestMethod]
			public void Ctor_IntStringIntException_Success()
			{
				try
				{
					try
					{
						throw new FormatException();
					}
					catch (Exception ex)
					{
						throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, 42, ex);
					}
				}
				catch (ConstraintParserException ex)
				{
					Assert.AreEqual(Constants.CustomHResult, ex.HResult);
					Assert.AreEqual(42, ex.Position);
					Assert.IsNotNull(ex.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex.Message);
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
						throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, 42, ex);
					}
				}
				catch (ConstraintParserException ex)
				{
					System.IO.MemoryStream Buffer = SerializationHelper.Serialize(ex);
					ConstraintParserException ex2 = SerializationHelper.Deserialize<ConstraintParserException>(Buffer);

					Assert.AreEqual(Constants.CustomHResult, ex2.HResult);
					Assert.AreEqual(42, ex.Position);
					Assert.IsNotNull(ex2.InnerException);
					Assert.AreEqual(Constants.TestMessage, ex2.Message);
				}
			}

			[TestMethod]
			public void ToString_Success()
			{
				try
				{
					throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage);
				}
				catch (Exception ex)
				{
					string str = ex.ToString();
					StringAssert.StartsWith(str, string.Format("{0}: ({1}) {2}", typeof(ConstraintParserException).FullName, Constants.CustomHResultString, Constants.TestMessage));
					StringAssert.Contains(str, "ToString_Success");
				}
			}
		}
	}
}
