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

#if WINDOWS_DESKTOP
using System.Diagnostics.CodeAnalysis;
#endif
using NerdyDuck.ParameterValidation;
using System.Collections.Generic;
using System;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Derivate of the Constraint class to test internal behavior.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	public class DummyConstraint : Constraint
	{
		public IReadOnlyList<string> Parameters;

		#region Constructors
		public DummyConstraint()
			: base("Dummy")
		{
		}

		public DummyConstraint(string name)
			: base(name)
		{
		}

#if WINDOWS_DESKTOP
		public DummyConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif
		#endregion

		#region Public methods
		public void AssertDataTypeTest(ParameterDataType dataType, ParameterDataType[] expectedTypes)
		{
			base.AssertDataType(dataType, expectedTypes);
		}

		public void OnValidationTest(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
		{
			base.OnValidation(results, value, dataType, memberName, displayName);
		}

		public void GetParametersTest(IList<string> parameters)
		{
			base.GetParameters(parameters);
		}

		protected override void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
		{
			base.SetParameters(parameters, dataType);
			if (parameters.Count > 0)
			{
				if (parameters[0] == "argex")
				{
					throw new ArgumentException();
				}
				else if (parameters[0] == "formex")
				{
					throw new FormatException();
				}
				else
				{
					Parameters = new List<string>(parameters);
				}
			}
		}
		#endregion
	}
}
