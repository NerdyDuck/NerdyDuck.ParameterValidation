#region Copyright
/*******************************************************************************
 * <copyright file="EnumTypeConstraint.cs" owner="Daniel Kopp">
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
 * <file name="EnumTypeConstraint.cs" date="2015-10-01">
 * Specifies a .NET type that is the underlying type for an enumeration
 * parameter.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// Specifies a .NET type that is the underlying type for an enumeration parameter.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[Type(type)]</c>. <c>type</c> must be an assembly-qualified type name of an enumeration.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Enum"/> data type only.</item>
	/// <item>If an enumeration parameter with that constraint contains a value that is not a part of the enumeration type, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// <item>If the enumeration type has the <see cref="FlagsAttribute"/>, the validated value may also be a combination of enumeration values.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "GetObjectData handled by base class")]
	[System.Serializable]
#endif
	public class EnumTypeConstraint : TypeConstraint
	{
		#region Private fields
		private Type mUnderlyingType;
		private bool mHasFlags;
		private Dictionary<string, object> mEnumValues;
		private long FlagMask;
		private bool IsTypeResolved;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the underlying type of the type resolved from <see cref="TypeConstraint.TypeName"/>.
		/// </summary>
		/// <value>The integer type the enumeration is based on; if no type could be resolved from <see cref="TypeConstraint.TypeName"/>, <see langword="null"/> is returned.</value>
		public Type UnderlyingType
		{
			get
			{
				ResolveType();
				return mUnderlyingType;
			}
		}

		/// <summary>
		/// Gets a value indicating if the type resolved from <see cref="TypeConstraint.TypeName"/> has the <see cref="FlagsAttribute"/>.
		/// </summary>
		/// <value><see langword="true"/>, if the type has the <see cref="FlagsAttribute"/>; otherwise, or if no type could be resolved from <see cref="TypeConstraint.TypeName"/>, <see langword="false"/> is returned.</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
		public bool HasFlags
		{
			get
			{
				ResolveType();
				return mHasFlags;
			}
		}

		/// <summary>
		/// Gets a read-only dictionary of enumeration names and values derived from the type resolved from <see cref="TypeConstraint.TypeName"/>.
		/// </summary>
		/// <value>A read-only dictionary, or if no type could be resolved from <see cref="TypeConstraint.TypeName"/>, <see langword="null"/> is returned.</value>
		public IReadOnlyDictionary<string, object> EnumValues
		{
			get
			{
				ResolveType();
				return mEnumValues;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumTypeConstraint"/> class.
		/// </summary>
		public EnumTypeConstraint()
			: base()
		{
			ResetFields();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumTypeConstraint"/> class with the specified type name.
		/// </summary>
		/// <param name="typeName">The assembly-qualified name of the type.</param>
		public EnumTypeConstraint(string typeName)
			: base(typeName)
		{
			ResetFields();
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumTypeConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected EnumTypeConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			ResetFields();
		}
#endif
		#endregion

		#region Protected methods
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
			if (parameters == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(ErrorCodes.Constraint_SetParameters_ParametersNull), nameof(parameters));
			}

			if (dataType == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(ErrorCodes.Constraint_SetParameters_TypeNone), nameof(dataType), Properties.Resources.Global_ParameterDataType_None);
			}

			AssertDataType(dataType, ParameterDataType.Enum);
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_SetParameters_OnlyOneParam), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			if (string.IsNullOrWhiteSpace(parameters[0]))
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_SetParameters_ParamNullEmpty), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this);
			}
			TypeName = parameters[0];
			ResetFields();
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
			base.OnBaseValidation(results, value, dataType, memberName, displayName);
			AssertDataType(dataType, ParameterDataType.Enum);

			if (UnderlyingType == null)
			{
				// Type not found, or not enum
				return;
			}

			Type ValType = value.GetType();
			TypeInfo ValTypeInfo = ValType.GetTypeInfo();
			if (ValTypeInfo.IsEnum)
			{
				if (ResolvedType != ValType)
				{
					// Value is an enumeration, but not the resolved one
					results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_Validate_WrongEnum), string.Format(Properties.Resources.EnumTypeConstraint_Validate_WrongEnum, displayName, ValType.FullName, ResolvedType.FullName), memberName, this));
					return;
				}

				if (mHasFlags)
				{
					long value2 = Convert.ToInt64(value);
					if (((value2 ^ FlagMask) & value2) != 0)
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_Validate_InvalidFlag), string.Format(Properties.Resources.EnumConstraint_Validate_InvalidFlag, displayName), memberName, this));
					}
				}
				else
				{
					if (!mEnumValues.ContainsValue(value))
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_Validate_NotInEnum), string.Format(Properties.Resources.EnumConstraint_Validate_NotDefined, displayName, value), memberName, this));
					}
				}
			}
			else if (ValType == typeof(int) || ValType == typeof(long) || ValType == typeof(short) || ValType == typeof(byte) ||
				ValType == typeof(uint) || ValType == typeof(ulong) || ValType == typeof(ushort) || ValType == typeof(sbyte))
			{
				if (mHasFlags)
				{
					long value2 = Convert.ToInt64(value);
					if (((value2 ^ FlagMask) & value2) != 0)
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_Validate_InvalidFlag), string.Format(Properties.Resources.EnumConstraint_Validate_InvalidFlag, displayName), memberName, this));
					}
				}
				else
				{
					if (!mEnumValues.ContainsValue(Convert.ChangeType(value, ValType)))
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_Validate_NotInEnum), string.Format(Properties.Resources.EnumConstraint_Validate_NotDefined, displayName, value), memberName, this));
					}
				}
			}
			else
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.EnumTypeConstraint_Validate_TypeNotSupported), string.Format(Properties.Resources.EnumConstraint_Validate_NotSupported, displayName, ValType.Name), memberName, this));
			}
		}
		#endregion
		#endregion

		#region Private methods
		#region ResolveType
		/// <summary>
		/// Resolves the <see cref="TypeConstraint.TypeName"/>, if possible, and dissects <see cref="TypeConstraint.ResolvedType"/> to gets its enumeration values, underlying type and Flags attribute.
		/// </summary>
		private void ResolveType()
		{
			lock (this)
			{
				if (IsTypeResolved)
				{
					return;
				}
				IsTypeResolved = true;

				Type EnumType = ResolvedType;
				if (EnumType == null)
					return;

				try
				{
					mEnumValues = ParameterConvert.ExamineEnumeration(EnumType, false, out mUnderlyingType, out mHasFlags);
				}
				catch (CodedArgumentException)
				{
					return;
				}

				if (mHasFlags)
				{
					foreach (object value in mEnumValues.Values)
					{
						FlagMask |= Convert.ToInt64(value);
					}
				}
			}
		}
		#endregion

		#region ResetFields
		/// <summary>
		/// Resets all fields set by ResolveType.
		/// </summary>
		private void ResetFields()
		{
			mUnderlyingType = null;
			mHasFlags = false;
			mEnumValues = null;
			FlagMask = 0;
			IsTypeResolved = false;
		}
		#endregion
		#endregion
	}
}
