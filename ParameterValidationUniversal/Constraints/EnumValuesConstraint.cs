#region Copyright
/*******************************************************************************
 * <copyright file="EnumValuesConstraint.cs" owner="Daniel Kopp">
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
 * <file name="EnumValuesConstraint.cs" date="2015-10-01">
 * Specifies valid values of an enumeration parameter.
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
	/// Specifies valid values of an enumeration parameter.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[Values([underlyingType,][Flags,]enum1=value1[,enum2=value2,...)]</c>. <c>underlyingType</c> must one of the <see cref="ParameterDataType"/> values specifying an integer type. If <c>Flags</c> is specified, then the values of the enumeration may be combined. <c>enum1=value1</c> defines a key/value pair, where <c>enum1</c> is the textual representation of the value, and <c>value1</c> is the underlying integer value, either in decimal or in hexadecimal representation.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Enum"/> data type only.</item>
	/// <item>If an eumeration parameter with that constraint contains a value that is not defined in the list of enumeration values, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// <item>If the <c>Flags</c> parameter is set, the validated value may also be a combination of enumeration values.</item>
	/// <item>This <see cref="Constraint"/> can work on its own, especially in scenarios where actual enumeration type is not available on all systems that need to handle the parameter value. But it can also work in combination with the <see cref="EnumTypeConstraint"/> to further limit the number of valid enumeration values.</item>
	/// <item>The <see cref="EnumValuesConstraint"/> can also be used to populate a combobox or radio buttons when displaying the parameter.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class EnumValuesConstraint : Constraint
	{
		#region Constants
		private const string FlagsParameter = "Flags";
		private const string CountName = "ValueCount";
		private const string NamePrefix = "Name_";
		private const string ValuePrefix = "Value_";
		#endregion

		#region Private fields
		private Type mUnderlyingType;
		private bool mHasFlags;
		private Dictionary<string, object> mEnumValues;
		private long FlagMask;
		private ParameterDataType mUnderlyingDataType;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the underlying type of the enumeration.
		/// </summary>
		/// <value>The integer type the enumeration is based on.</value>
		public Type UnderlyingType
		{
			get { return mUnderlyingType; }
		}

		/// <summary>
		/// Gets the underlying data type of the enumeration.
		/// </summary>
		/// <value>The integer type the enumeration is based on.</value>
		public ParameterDataType UnderlyingDataType
		{
			get { return mUnderlyingDataType; }
		}

		/// <summary>
		/// Gets a value indicating if the enumeration type represented by the <see cref="EnumValuesConstraint"/> has the <see cref="FlagsAttribute"/>.
		/// </summary>
		/// <value><see langword="true"/>, if the type has the <see cref="FlagsAttribute"/>; otherwise, <see langword="false"/>.</value>
		public bool HasFlags
		{
			get { return mHasFlags; }
		}

		/// <summary>
		/// Gets a read-only dictionary of enumeration names and values.
		/// </summary>
		/// <value>A read-only dictionary of string keys and integer values.</value>
		public IReadOnlyDictionary<string, object> EnumValues
		{
			get { return mEnumValues; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumValuesConstraint"/> class.
		/// </summary>
		public EnumValuesConstraint()
			: base(EnumValuesConstraintName)
		{
			ResetFields();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumValuesConstraint"/> class with the specified type name.
		/// </summary>
		/// <param name="type">The underlying type of the enumeration.</param>
		/// <param name="flags">A value indicating if the enumeration type represented by the <see cref="EnumValuesConstraint"/> has the <see cref="FlagsAttribute"/>.</param>
		/// <param name="values">A dictionary of enumeration names and values.</param>
		public EnumValuesConstraint(ParameterDataType type, bool flags, IDictionary<string, object> values)
			: base(EnumValuesConstraintName)
		{
			ResetFields();
			if (!ParameterConvert.IsIntegerType(type))
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x44), nameof(type), type, Properties.Resources.EnumValuesConstraint_NotInteger);
			}
			mUnderlyingDataType = type;
			mUnderlyingType = ParameterConvert.ParameterToNetDataType(type);
			mHasFlags = flags;
			if (values == null || values.Count == 0)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x45), nameof(values));
			}
			mEnumValues = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> pair in values)
			{
				if (pair.Value.GetType() == mUnderlyingType)
				{
					mEnumValues.Add(pair.Key, pair.Value);
				}
				else
				{
					try
					{
						object ChangedType = Convert.ChangeType(pair.Value, mUnderlyingType);
						mEnumValues.Add(pair.Key, ChangedType);
					}
					catch (InvalidCastException ex)
					{
						throw new CodedArgumentException(Errors.CreateHResult(0x46), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, pair.Value.GetType().Name, mUnderlyingDataType), nameof(values), ex);
					}
					catch (FormatException ex)
					{
						throw new CodedArgumentException(Errors.CreateHResult(0x46), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, pair.Value.GetType().Name, mUnderlyingDataType), nameof(values), ex);
					}
					catch (OverflowException ex)
					{
						throw new CodedArgumentException(Errors.CreateHResult(0x46), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, pair.Value.GetType().Name, mUnderlyingDataType), nameof(values), ex);
					}
				}
			}

			CreateMask();
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumValuesConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected EnumValuesConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			ResetFields();
			mUnderlyingDataType = (ParameterDataType)info.GetValue(nameof(UnderlyingDataType), typeof(ParameterDataType));
			mUnderlyingType = ParameterConvert.ParameterToNetDataType(mUnderlyingDataType);
			mHasFlags = info.GetBoolean(nameof(HasFlags));
			int ValueCount = info.GetInt32(CountName);
			mEnumValues = new Dictionary<string, object>();
			for (int i = 0; i < ValueCount; i++)
			{
				mEnumValues.Add(info.GetString(NamePrefix + i.ToString()), info.GetValue(ValuePrefix + i.ToString(), mUnderlyingType));
			}
			CreateMask();
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
			info.AddValue(nameof(UnderlyingDataType), mUnderlyingDataType);
			info.AddValue(nameof(HasFlags), mHasFlags);
			info.AddValue(CountName, mEnumValues.Count);
			int i = 0;
			foreach (KeyValuePair<string, object> pair in mEnumValues)
			{
				info.AddValue(NamePrefix + i.ToString(), pair.Key);
				info.AddValue(ValuePrefix + i.ToString(), pair.Value);
				i++;
			}
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
			parameters.Add(Enum.GetName(typeof(ParameterDataType), mUnderlyingDataType));
			if (mHasFlags)
				parameters.Add(FlagsParameter);
			foreach (KeyValuePair<string, object> pair in mEnumValues)
			{
				parameters.Add(string.Format("{0}={1}", pair.Key, ParameterConvert.ToString(pair.Value, mUnderlyingDataType, null)));
			}
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
			AssertDataType(dataType, ParameterDataType.Enum);
			ResetFields();
			if (parameters.Count < 2)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x14), string.Format(Properties.Resources.Global_SetParameters_InvalidMinCount, this.Name, 2), this);
			}

			if (!Enum.TryParse<ParameterDataType>(parameters[0], out mUnderlyingDataType))
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x47), Properties.Resources.EnumValuesConstraint_SetParameters_NotDataType, this);
			}
			mUnderlyingType = ParameterConvert.ParameterToNetDataType(mUnderlyingDataType);

			int EnumStart = 1;
			if (parameters[1] == FlagsParameter)
			{
				mHasFlags = true;
				EnumStart = 2;
			}

			if (parameters.Count <= EnumStart)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x48), string.Format(Properties.Resources.EnumValuesConstraint_SetParameters_NoValues, this.Name), this);
			}

			string Name, Value;
			mEnumValues = new Dictionary<string, object>();
			for (int i = EnumStart; i < parameters.Count; i++)
			{
				string[] tokens = parameters[i].Trim().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length != 2)
				{
					throw new ConstraintConfigurationException(Errors.CreateHResult(0x49), string.Format(Properties.Resources.EnumValuesConstraint_SetParameters_InvalidValue, parameters[i]), this);
				}
				Name = tokens[0].Trim();
				Value = tokens[1].Trim();
				if (Value.StartsWith("0x"))
				{
					ulong HexValue;
					if (!ulong.TryParse(Value.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out HexValue))
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x4a), string.Format(Properties.Resources.EnumValuesConstraint_SetParameters_InvalidHex, Value), this);
					}

					try
					{
						mEnumValues.Add(Name, Convert.ChangeType(HexValue, mUnderlyingType));
					}
					catch (InvalidCastException ex)
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x4b), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, Value, mUnderlyingType), this, ex);
					}
					catch (FormatException ex)
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x4b), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, Value, mUnderlyingType), this, ex);
					}
					catch (OverflowException ex)
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x4b), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, Value, mUnderlyingType), this, ex);
					}
				}
				else
				{
					try
					{
						mEnumValues.Add(Name, Convert.ChangeType(Value, mUnderlyingType));
					}
					catch (InvalidCastException ex)
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x4b), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, Value, mUnderlyingType), this, ex);
					}
					catch (FormatException ex)
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x4b), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, Value, mUnderlyingType), this, ex);
					}
					catch (OverflowException ex)
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x4b), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, Value, mUnderlyingType), this, ex);
					}
				}
			}

			CreateMask();
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
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x4c), Properties.Resources.EnumValuesConstraint_Validate_NotConfigured, this);
			}

			Type ValType = value.GetType();
			TypeInfo ValTypeInfo = ValType.GetTypeInfo();
			if (ValTypeInfo.IsEnum)
			{
				ValType = ParameterConvert.GetEnumUnderlyingType(ValType);
				value = Convert.ChangeType(value, ValType);
			}

			if (ParameterConvert.IsIntegerType(ValType))
			{
				if (mHasFlags)
				{
					long value2 = Convert.ToInt64(value);
					if (((value2 ^ FlagMask) & value2) != 0)
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(0x4d), Properties.Resources.EnumConstraint_Validate_InvalidFlag, this));
					}
				}
				else
				{
					if (ValType != mUnderlyingType)
					{
						try
						{
							value = Convert.ChangeType(value, mUnderlyingType);
						}
						catch (InvalidCastException ex)
						{
							throw new CodedArgumentException(Errors.CreateHResult(0x53), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, value, mUnderlyingType), ex);
						}
						catch (FormatException ex)
						{
							throw new CodedArgumentException(Errors.CreateHResult(0x53), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, value, mUnderlyingType), ex);
						}
						catch (OverflowException ex)
						{
							throw new CodedArgumentException(Errors.CreateHResult(0x53), string.Format(Properties.Resources.EnumValuesConstraint_InvalidValueType, value, mUnderlyingType), ex);
						}

					}
					if (!mEnumValues.ContainsValue(value))
					{
						results.Add(new ParameterValidationResult(Errors.CreateHResult(0x4f), string.Format(Properties.Resources.EnumConstraint_Validate_NotDefined, value), this));
					}
				}
			}
			else
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x51), string.Format(Properties.Resources.EnumConstraint_Validate_NotSupported, ValType.Name), this));
			}
		}
		#endregion
		#endregion

		#region Private methods
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
			mUnderlyingDataType = ParameterDataType.Int32;
		}
		#endregion

		#region CreateMask
		/// <summary>
		/// If the enumeration has the Flags attribute, a mask containing all flags is stored in FlagMask.
		/// </summary>
		private void CreateMask()
		{
			if (mHasFlags)
			{
				foreach (object value in mEnumValues.Values)
				{
					FlagMask |= Convert.ToInt64(value);
				}
			}
		}
		#endregion
		#endregion
	}
}
