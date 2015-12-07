#region Copyright
/*******************************************************************************
 * <copyright file="MaximumValueConstraint.cs" owner="Daniel Kopp">
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
 * <file name="MaximumValueConstraint.cs" date="2015-10-01">
 * A constraint specifying the maximum value for an integer, byte, decimal,
 * DateTimeOffset, TimeSpan or Version.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// A constraint specifying the maximum value for an integer, <see cref="byte"/>, <see cref="decimal"/>, <see cref="DateTimeOffset"/>, <see cref="TimeSpan"/> or <see cref="Version"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[MaxValue(value)]</c>. <c>value</c> must be a string representation of the same data type as the data type that the <see cref="MaximumValueConstraint"/> is applied to.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Byte"/>, <see cref="ParameterDataType.DateTimeOffset"/>, <see cref="ParameterDataType.Decimal"/>, <see cref="ParameterDataType.Int16"/>,
	/// <see cref="ParameterDataType.Int32"/>, <see cref="ParameterDataType.Int64"/>, <see cref="ParameterDataType.SignedByte"/>, <see cref="ParameterDataType.TimeSpan"/>, <see cref="ParameterDataType.UInt16"/>,
	/// <see cref="ParameterDataType.UInt32"/>, <see cref="ParameterDataType.UInt64"/> and <see cref="ParameterDataType.Version"/> data types.</item>
	/// <item>If a parameter with that constraint has a value larger than <see cref="MaximumValue"/>, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class MaximumValueConstraint : Constraint
	{
		#region Private fields
		private object mMaximumValue;
		private object AltMaximumValue;
		private ParameterDataType mDataType;
		private Type ExpectedNetType;
		private Type AltNetType;
		private TypeCode ExpectedTypeCode;
		private TypeCode AltTypeCode;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the maximum value to enforce.
		/// </summary>
		public object MaximumValue
		{
			get { return mMaximumValue; }
		}

		/// <summary>
		/// Gets the data type that the constraint can validate.
		/// </summary>
		public ParameterDataType DataType
		{
			get { return mDataType; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MaximumValueConstraint"/> class.
		/// </summary>
		/// <param name="dataType">The data type that the constraint can validate.</param>
		/// <exception cref="CodedArgumentException"><paramref name="dataType"/> is not supported by the constraint.</exception>
		public MaximumValueConstraint(ParameterDataType dataType)
			: base(MaximumValueConstraintName)
		{
			mDataType = dataType;
			mMaximumValue = CheckDataType();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MaximumValueConstraint"/> class with the specified maximum value.
		/// </summary>
		/// <param name="dataType">The data type that the constraint can validate.</param>
		/// <param name="maximumValue">The maximum value to enforce.</param>
		/// <exception cref="CodedArgumentNullException"><paramref name="maximumValue"/> is <see langword="null"/>.</exception>
		public MaximumValueConstraint(ParameterDataType dataType, object maximumValue)
			: base(MaximumValueConstraintName)
		{
			if (maximumValue == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x14), nameof(maximumValue));
			}

			mDataType = dataType;
			mMaximumValue = CheckDataType();
			mMaximumValue = CheckValueType(maximumValue);
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="MaximumValueConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected MaximumValueConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mDataType = (ParameterDataType)info.GetValue(nameof(DataType), typeof(ParameterDataType));
			mMaximumValue = CheckDataType();
			mMaximumValue = CheckValueType(info.GetValue(nameof(MaximumValue), ExpectedNetType));
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
			info.AddValue(nameof(DataType), mDataType);
			info.AddValue(nameof(MaximumValue), mMaximumValue);
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
			parameters.Add(ParameterConvert.ToString(mMaximumValue, mDataType, null));
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
			AssertDataType(dataType, mDataType);
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x19), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mMaximumValue = CheckValueType(ParameterConvert.ToDataType(parameters[0], dataType, null));
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x1a), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
			}
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
			base.OnValidation(results, value, dataType, memberName, displayName);
			AssertDataType(dataType, mDataType);

			int CompareResult = 0;
			Type type = value.GetType();
			if (type == ExpectedNetType)
			{
				// Type matches
				CompareResult = ((IComparable)mMaximumValue).CompareTo(value);
			}
			else
			{
				if (IsConvertibleType(mDataType))
				{
					if (type == AltNetType)
					{
						// Type is already equal to alternative data type, no conversion necessary.
						CompareResult = ((IComparable)AltMaximumValue).CompareTo(value);
					}
					else
					{
						object ConvertedValue = null;
						// Try to convert and compare to actual data type
						try
						{
							ConvertedValue = Convert.ChangeType(value, ExpectedTypeCode, System.Globalization.CultureInfo.InvariantCulture);
						}
						catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
						{
						}

						if (ConvertedValue == null)
						{
							// Conversion to actual data type failed, try to convert and compare to alternative data type.
							try
							{
								ConvertedValue = Convert.ChangeType(value, AltTypeCode, System.Globalization.CultureInfo.InvariantCulture);
							}
							catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
							{
								throw new CodedArgumentException(Errors.CreateHResult(0x1c), string.Format(Properties.Resources.Global_Validate_TypeNotConvertible, type.Name, ExpectedNetType.Name));
							}
							CompareResult = ((IComparable)AltMaximumValue).CompareTo(ConvertedValue);
						}
						else
						{
							// Converting to actual data type successful
							CompareResult = ((IComparable)mMaximumValue).CompareTo(ConvertedValue);
						}
					}
				}
				else
				{
					throw new CodedArgumentException(Errors.CreateHResult(0x1c), string.Format(Properties.Resources.Global_Validate_TypeNotConvertible, type.Name, ExpectedNetType.Name));
				}

			}

			if (CompareResult < 0)
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x18), string.Format(Properties.Resources.MaximumValueConstraint_Validate_Failed, displayName, mMaximumValue, value), memberName, this));
			}
		}
		#endregion

		#endregion

		#region Private methods
		#region IsConvertibleType
		/// <summary>
		/// Checks if the data type can be converted into another data type before comparison.
		/// </summary>
		/// <param name="dataType">The data type to check.</param>
		/// <returns><see langword="false"/>, if <paramref name="dataType"/> <see cref="ParameterDataType.DateTimeOffset"/>, <see cref="ParameterDataType.TimeSpan"/> or <see cref="ParameterDataType.Version"/>; otherwise, <see langword="true"/>.</returns>
		private static bool IsConvertibleType(ParameterDataType dataType)
		{
			return !(dataType == ParameterDataType.DateTimeOffset || dataType == ParameterDataType.TimeSpan || dataType == ParameterDataType.Version);
		}
		#endregion

		#region CheckDataType
		/// <summary>
		/// Checks that the specified data type is supported by the <see cref="MaximumValueConstraint"/>, and sets the appropriate expected and alternative data types.
		/// </summary>
		/// <returns>The maximum value supported by the data type.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private object CheckDataType()
		{
			switch (mDataType)
			{
				case ParameterDataType.Byte:
					ExpectedNetType = typeof(byte);
					AltNetType = typeof(ulong);
					ExpectedTypeCode = TypeCode.Byte;
					AltTypeCode = TypeCode.UInt64;
					return byte.MaxValue;
				case ParameterDataType.DateTimeOffset:
					ExpectedNetType = typeof(DateTimeOffset);
					AltNetType = null;
					ExpectedTypeCode = TypeCode.Empty;
					AltTypeCode = TypeCode.Empty;
					return DateTimeOffset.MaxValue;
				case ParameterDataType.Decimal:
					ExpectedNetType = typeof(decimal);
					AltNetType = typeof(decimal);
					ExpectedTypeCode = TypeCode.Decimal;
					AltTypeCode = TypeCode.Decimal;
					return decimal.MaxValue;
				case ParameterDataType.Int16:
					ExpectedNetType = typeof(short);
					AltNetType = typeof(long);
					ExpectedTypeCode = TypeCode.Int16;
					AltTypeCode = TypeCode.Int64;
					return short.MaxValue;
				case ParameterDataType.Int32:
					ExpectedNetType = typeof(int);
					AltNetType = typeof(long);
					ExpectedTypeCode = TypeCode.Int32;
					AltTypeCode = TypeCode.Int64;
					return int.MaxValue;
				case ParameterDataType.Int64:
					ExpectedNetType = typeof(long);
					AltNetType = typeof(long);
					ExpectedTypeCode = TypeCode.Int64;
					AltTypeCode = TypeCode.Int64;
					return long.MaxValue;
				case ParameterDataType.SignedByte:
					ExpectedNetType = typeof(sbyte);
					AltNetType = typeof(long);
					ExpectedTypeCode = TypeCode.SByte;
					AltTypeCode = TypeCode.Int64;
					return sbyte.MaxValue;
				case ParameterDataType.TimeSpan:
					ExpectedNetType = typeof(TimeSpan);
					AltNetType = null;
					ExpectedTypeCode = TypeCode.Empty;
					AltTypeCode = TypeCode.Empty;
					return TimeSpan.MaxValue;
				case ParameterDataType.UInt16:
					ExpectedNetType = typeof(ushort);
					AltNetType = typeof(ulong);
					ExpectedTypeCode = TypeCode.UInt16;
					AltTypeCode = TypeCode.UInt64;
					return ushort.MaxValue;
				case ParameterDataType.UInt32:
					ExpectedNetType = typeof(uint);
					AltNetType = typeof(ulong);
					ExpectedTypeCode = TypeCode.UInt32;
					AltTypeCode = TypeCode.UInt64;
					return uint.MaxValue;
				case ParameterDataType.UInt64:
					ExpectedNetType = typeof(ulong);
					AltNetType = typeof(ulong);
					ExpectedTypeCode = TypeCode.UInt64;
					AltTypeCode = TypeCode.UInt64;
					return ulong.MaxValue;
				case ParameterDataType.Version:
					ExpectedNetType = typeof(Version);
					AltNetType = null;
					ExpectedTypeCode = TypeCode.Empty;
					AltTypeCode = TypeCode.Empty;
					return new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
				default:
					throw new CodedArgumentException(Errors.CreateHResult(0x1b), string.Format(Properties.Resources.Global_CheckDataType_NotSupported, this.Name),  "dataType");
			}
		}
		#endregion

		#region CheckValueType
		/// <summary>
		/// Checks if the value is either of the expected type, or can be converted into the expected type.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>The checked value, either in original form, or converted.</returns>
		private object CheckValueType(object value)
		{
			Type type = value.GetType();
			object CheckedValue = null;

			if (type == ExpectedNetType)
			{
				CheckedValue = value;
			}
			else
			{
				if (ExpectedTypeCode == TypeCode.Empty)
				{
					// DateTimeOffset, TimeSpan, Version cannot be converted from another type
					throw new ParameterConversionException(Errors.CreateHResult(0x1d), string.Format(Properties.Resources.Global_CheckValueType_NotConvertible, type.Name, ExpectedNetType.Name), mDataType, value);
				}
				else
				{
					try
					{
						CheckedValue = Convert.ChangeType(value, ExpectedTypeCode, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
					{
						throw new ParameterConversionException(Errors.CreateHResult(0x1d), string.Format(Properties.Resources.Global_CheckValueType_NotConvertible, type.Name, ExpectedNetType.Name), mDataType, value, ex);
					}
				}
			}

			if (AltNetType != null)
			{
				if (type == AltNetType)
				{
					AltMaximumValue = value;
				}
				else
				{
					AltMaximumValue = Convert.ChangeType(value, AltTypeCode, System.Globalization.CultureInfo.InvariantCulture);
				}
			}
			return CheckedValue;
		}
		#endregion
		#endregion
	}
}
