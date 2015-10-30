#region Copyright
/*******************************************************************************
 * <copyright file="EnumTypeConstraint.cs" owner="Daniel Kopp">
 * Copyright 2015 Daniel Kopp
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
	/// <item>If an eumeration parameter with that constraint contains a value that is not a part of the enumeration type, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// <item>If the enumeration type has the <see cref="FlagsAttribute"/>, the validated value may also be a combination of enumeration values.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
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
		/// <value>The integer type the enum is based on; if no type could be resolved from <see cref="TypeConstraint.TypeName"/>, <see langword="null"/> is returned.</value>
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
			ResetFields();
			base.SetParameters(parameters, dataType);
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
					results.Add(new ParameterValidationResult(Errors.CreateHResult(0x43), string.Format(Properties.Resources.EnumTypeConstraint_Validate_WrongEnum, ValType.FullName, ResolvedType.FullName), this));
					return;
				}

				ValType = ParameterConvert.GetEnumUnderlyingType(ValType);
			}

			if (ValType == typeof(int) || ValType == typeof(long) || ValType == typeof(short) || ValType == typeof(byte) ||
				ValType == typeof(uint) || ValType == typeof(ulong) || ValType == typeof(ushort) || ValType == typeof(sbyte))
			{
				if (mHasFlags)
				{
					long value2 = Convert.ToInt64(value);
					if (((value2 ^ FlagMask) & value2) != 0)
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(0x4e), Properties.Resources.EnumConstraint_Validate_InvalidFlag, this));
					}
				}
				else
				{
					if (!mEnumValues.ContainsValue(Convert.ChangeType(value, ValType)))
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(0x50), string.Format(Properties.Resources.EnumConstraint_Validate_NotDefined, value), this));
					}
				}
			}
			else
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x52), string.Format(Properties.Resources.EnumConstraint_Validate_NotSupported, ValType.Name), this));
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

				TypeInfo EnumInfo = EnumType.GetTypeInfo();
				if (!EnumInfo.IsEnum)
					return;


				mUnderlyingType = ParameterConvert.GetEnumUnderlyingType(EnumType);
				if (EnumInfo.GetCustomAttribute(typeof(FlagsAttribute)) != null)
				{
					mHasFlags = true;
				}

				mEnumValues = new Dictionary<string, object>();
				foreach (FieldInfo field in EnumInfo.DeclaredFields)
				{
					if (field.IsLiteral)
					{
#if WINDOWS_UWP
						mEnumValues.Add(field.Name, Convert.ChangeType(field.GetValue(null), mUnderlyingType));
#endif
#if WINDOWS_DESKTOP
						mEnumValues.Add(field.Name, field.GetRawConstantValue());
#endif
					}
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
