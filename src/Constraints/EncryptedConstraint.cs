#region Copyright
/*******************************************************************************
 * <copyright file="EncryptedConstraint.cs" owner="Daniel Kopp">
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
 * <assembly name="NerdyDuck.ParameterValidation">
 * Validation and serialization of parameter values for .NET
 * </assembly>
 * <file name="EncryptedConstraint.cs" date="2015-09-29">
 * A constraint specifying that a parameter value needs to be encrypted before
 * its value is stored.
 * </file>
 ******************************************************************************/
#endregion

using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// A constraint specifying that a parameter value needs to be encrypted before its value is stored.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[Encrypted]</c>.</item>
	/// <item>The constraint is applicable to all data types in <see cref="ParameterDataType"/>.</item>
	/// <item>The constraint is not used during validation, but is considered during serialization of values.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class EncryptedConstraint : Constraint
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EncryptedConstraint"/> class.
		/// </summary>
		public EncryptedConstraint()
			: base(EncryptedConstraintName)
		{
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="EncryptedConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected EncryptedConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif
		#endregion
	}
}
