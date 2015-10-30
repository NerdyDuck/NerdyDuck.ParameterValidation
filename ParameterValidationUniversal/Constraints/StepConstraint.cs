#region Copyright
/*******************************************************************************
 * <copyright file="StepConstraint.cs" owner="Daniel Kopp">
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
 * <file name="StepConstraint.cs" date="2015-10-01">
 * Specifies the increment and decrement step to use when displaying an integer
 * or decimal value in a control e.g. a NumericUpDown control.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// Specifies the increment and decrement step to use when displaying an integer or <see cref="decimal"/> value in a control e.g. a NumericUpDown control.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[Step(value)]</c>. <c>value</c> must be a string representation of the same data type as the data type that the <see cref="StepConstraint"/> is applied to.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Byte"/>, <see cref="ParameterDataType.Decimal"/>, <see cref="ParameterDataType.Int16"/>,
	/// <see cref="ParameterDataType.Int32"/>, <see cref="ParameterDataType.Int64"/>, <see cref="ParameterDataType.SignedByte"/>, <see cref="ParameterDataType.UInt16"/>,
	/// <see cref="ParameterDataType.UInt32"/> and <see cref="ParameterDataType.UInt64"/> data types.</item>
	/// <item>The constraint is not used during validation or serialization, but is solely thought as a hint for display purposes.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class StepConstraint : Constraint
	{
		#region Private fields
		private object mStepSize;
		private ParameterDataType mDataType;
		private Type ExpectedNetType;
		private TypeCode ExpectedTypeCode;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the increment step.
		/// </summary>
		public object StepSize
		{
			get { return mStepSize; }
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
		/// Initializes a new instance of the <see cref="StepConstraint"/> class.
		/// </summary>
		/// <param name="dataType">The data type that the constraint can validate.</param>
		/// <exception cref="CodedArgumentException"><paramref name="dataType"/> is not supported by the constraint.</exception>
		public StepConstraint(ParameterDataType dataType)
			: base(StepConstraintName)
		{
			mDataType = dataType;
			mStepSize = CheckDataType();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StepConstraint"/> class with the specified maximum value.
		/// </summary>
		/// <param name="dataType">The data type that the constraint can validate.</param>
		/// <param name="stepSize">The increment step.</param>
		/// <exception cref="CodedArgumentNullException"><paramref name="stepSize"/> is <see langword="null"/>.</exception>
		public StepConstraint(ParameterDataType dataType, object stepSize)
			: base(StepConstraintName)
		{
			if (stepSize == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x1e), nameof(stepSize));
			}

			mDataType = dataType;
			mStepSize = CheckDataType();
			mStepSize = CheckValueType(stepSize);
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="StepConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected StepConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mDataType = (ParameterDataType)info.GetValue(nameof(DataType), typeof(ParameterDataType));
			mStepSize = CheckDataType();
			mStepSize = CheckValueType(info.GetValue(nameof(StepSize), ExpectedNetType));
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
			info.AddValue(nameof(DataType), mDataType);
			info.AddValue(nameof(StepSize), mStepSize);
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
			parameters.Add(ParameterConvert.ToString(mStepSize, mDataType, null));
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
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x26), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mStepSize = CheckValueType(ParameterConvert.ToDataType(parameters[0], dataType, null));
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x27), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
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
		}
		#endregion
		#endregion

		#region Private methods
		/// <summary>
		/// Checks that the specified data type is supported by the <see cref="StepConstraint"/>, and sets the appropriate expected and alternative data types.
		/// </summary>
		/// <returns>The default value supported by the data type.</returns>
		private object CheckDataType()
		{
			switch (mDataType)
			{
				case ParameterDataType.Byte:
					ExpectedNetType = typeof(byte);
					ExpectedTypeCode = TypeCode.Byte;
					return (byte)1;
				case ParameterDataType.Decimal:
					ExpectedNetType = typeof(decimal);
					ExpectedTypeCode = TypeCode.Decimal;
					return 1m;
				case ParameterDataType.Int16:
					ExpectedNetType = typeof(short);
					ExpectedTypeCode = TypeCode.Int16;
					return (short)1;
				case ParameterDataType.Int32:
					ExpectedNetType = typeof(int);
					ExpectedTypeCode = TypeCode.Int32;
					return 1;
				case ParameterDataType.Int64:
					ExpectedNetType = typeof(long);
					ExpectedTypeCode = TypeCode.Int64;
					return 1L;
				case ParameterDataType.SignedByte:
					ExpectedNetType = typeof(sbyte);
					ExpectedTypeCode = TypeCode.SByte;
					return (sbyte)1;
				case ParameterDataType.UInt16:
					ExpectedNetType = typeof(ushort);
					ExpectedTypeCode = TypeCode.UInt16;
					return (ushort)1;
				case ParameterDataType.UInt32:
					ExpectedNetType = typeof(uint);
					ExpectedTypeCode = TypeCode.UInt32;
					return 1u;
				case ParameterDataType.UInt64:
					ExpectedNetType = typeof(ulong);
					ExpectedTypeCode = TypeCode.UInt64;
					return (ulong)1;
				default:
					throw new CodedArgumentException(Errors.CreateHResult(0x28), string.Format(Properties.Resources.Global_CheckDataType_NotSupported, this.Name), "dataType");
			}
		}

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
				try
				{
					CheckedValue = Convert.ChangeType(value, ExpectedTypeCode, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch (InvalidCastException ex)
				{
					throw new ParameterConversionException(Errors.CreateHResult(0x29), string.Format(Properties.Resources.Global_CheckValueType_NotConvertible, type.Name, ExpectedNetType.Name), mDataType, value, ex);
				}
				catch (FormatException ex)
				{
					throw new ParameterConversionException(Errors.CreateHResult(0x29), string.Format(Properties.Resources.Global_CheckValueType_NotConvertible, type.Name, ExpectedNetType.Name), mDataType, value, ex);
				}
				catch (OverflowException ex)
				{
					throw new ParameterConversionException(Errors.CreateHResult(0x29), string.Format(Properties.Resources.Global_CheckValueType_NotConvertible, type.Name, ExpectedNetType.Name), mDataType, value, ex);
				}
			}

			return CheckedValue;
		}
		#endregion
	}
}
