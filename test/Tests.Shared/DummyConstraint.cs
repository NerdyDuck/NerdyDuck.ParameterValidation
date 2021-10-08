#region Copyright
/*******************************************************************************
 * <copyright file="DummyConstraint.cs" owner="Daniel Kopp">
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
 * <file name="DummyConstraint.cs" date="2015-11-19">
 * Derivate of the Constraint class to test internal behavior.
 * </file>
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
