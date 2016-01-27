#region Copyright
/*******************************************************************************
 * <copyright file="TypeConstraint.cs" owner="Daniel Kopp">
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
 * <file name="TypeConstraint.cs" date="2015-10-01">
 * Specifies a .NET type as the underlying type for an Xml parameter.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// Specifies a .NET type as the underlying type for an <see cref="ParameterDataType.Xml"/> parameter.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[Type(type)]</c>. <c>type</c> must be an assembly-qualified type name.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Xml"/> data type only.</item>
	/// <item>The constraint is not used during validation, but is used as a hint when attempting to serialize or deserialize a <see cref="ParameterDataType.Xml"/> parameter.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class TypeConstraint : Constraint
	{
		#region Private fields
		private string mTypeName;
		private Type mResolvedType;
		private bool IsTypeResolved;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the assembly-qualified name of the type.
		/// </summary>
		/// <value>A string containing the full name of a type and the assembly that defines it.</value>
		public string TypeName
		{
			get { return mTypeName; }
			protected set
			{
				if (mTypeName != value)
				{
					mTypeName = value;
					mResolvedType = null;
					IsTypeResolved = false;
				}
			}
		}

		/// <summary>
		/// Gets the type resolved from <see cref="TypeName"/>.
		/// </summary>
		/// <value>The resolved type; if no type could be resolved from <see cref="TypeName"/>, <see langword="null"/> is returned.</value>
		public Type ResolvedType
		{
			get
			{
				ResolveType();
				return mResolvedType;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeConstraint"/> class.
		/// </summary>
		public TypeConstraint()
			: base(TypeConstraintName)
		{
			mTypeName = string.Empty;
			mResolvedType = null;
			IsTypeResolved = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeConstraint"/> class with the specified type name.
		/// </summary>
		/// <param name="typeName">The assembly-qualified name of the type.</param>
		public TypeConstraint(string typeName)
			: base(TypeConstraintName)
		{
			if (string.IsNullOrWhiteSpace(typeName))
			{
				throw new CodedArgumentNullOrWhiteSpaceException(Errors.CreateHResult(0x41), nameof(typeName));
			}
			mTypeName = typeName;
			mResolvedType = null;
			IsTypeResolved = false;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected TypeConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mTypeName = info.GetString(nameof(TypeName));
			mResolvedType = null;
			IsTypeResolved = false;
		}
#endif
		#endregion

		#region Public methods
#if WINDOWS_DESKTOP
		#region GetObjectData
		/// <summary>
		/// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> with information about the <see cref="Constraint"/>.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
		/// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(nameof(TypeName), mTypeName);
		}
		#endregion
#endif
		#endregion

		#region Protected methods
		#region GetParameters
		/// <summary>
		/// Adds the parameters of the constraint to a list of strings.
		/// </summary>
		/// <param name="parameters">A list of strings to add the parameters to.</param>
		/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
		protected override void GetParameters(IList<string> parameters)
		{
			base.GetParameters(parameters);
			parameters.Add(mTypeName);
		}
		#endregion

		#region SetParameters
		/// <summary>
		/// Sets the parameters that the <see cref="Constraint"/> requires to work.
		/// </summary>
		/// <param name="parameters">A enumeration of string parameters.</param>
		/// <param name="dataType">The data type that the constraint needs to restrict.</param>
		/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
		/// <exception cref="ConstraintConfigurationException"><paramref name="parameters"/> contains no elements, or an invalid element.</exception>
		protected override void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
		{
			base.SetParameters(parameters, dataType);
			AssertDataType(dataType, ParameterDataType.Xml);
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x2a), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			if (string.IsNullOrWhiteSpace(parameters[0]))
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x42), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this);
			}
			mTypeName = parameters[0];
			mResolvedType = null;
			IsTypeResolved = false;
		}
		#endregion

		#region OnValidation
		/// <summary>
		/// When implemented by a deriving class, checks that the provided value is within the bounds of the constraint.
		/// </summary>
		/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
		/// <param name="value">The value to check.</param>
		/// <param name="dataType">The data type of the value.</param>
		/// <param name="memberName">The name of the property or field that is validated.</param>
		/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
		protected override void OnValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
		{
			OnBaseValidation(results, value, dataType, memberName, displayName);
			AssertDataType(dataType, ParameterDataType.Xml);
		}
		#endregion

		#region OnBaseValidation
		/// <summary>
		/// Calls <see cref="Constraint.OnValidation(IList{ParameterValidationResult}, object, ParameterDataType, string, string)"/> of the base class.
		/// </summary>
		/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
		/// <param name="value">The value to check.</param>
		/// <param name="dataType">The data type of the value.</param>
		/// <param name="memberName">The name of the property or field that is validated.</param>
		/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
		/// <remarks>This method is a small hack to be used by the derived <see cref="EnumTypeConstraint"/> class.</remarks>
		protected void OnBaseValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
		{
			base.OnValidation(results, value, dataType, memberName, displayName);
		}
		#endregion
		#endregion

		#region Private methods
		/// <summary>
		/// Resolves the <see cref="TypeName"/>, if possible, and stores it in <see cref="ResolvedType"/>.
		/// </summary>
		private void ResolveType()
		{
			lock (this)
			{
				if (IsTypeResolved)
				{
					return;
				}
				mResolvedType = Type.GetType(mTypeName, false);
				IsTypeResolved = true;
			}
		}
		#endregion
	}
}
