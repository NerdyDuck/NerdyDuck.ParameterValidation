#region Copyright
/*******************************************************************************
 * NerdyDuck.Collections - Validation and serialization of parameter values
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

using System.Reflection;

namespace NerdyDuck.ParameterValidation.Constraints;

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
[Serializable]
public class EnumTypeConstraint : TypeConstraint
{
	private Type? _underlyingType;
	private bool _hasFlags;
	private Dictionary<string, object>? _enumValues;
	private long _flagMask;
	private bool _isTypeResolved;
	private readonly object _lock = new();

	/// <summary>
	/// Gets the underlying type of the type resolved from <see cref="TypeConstraint.TypeName"/>.
	/// </summary>
	/// <value>The integer type the enumeration is based on; if no type could be resolved from <see cref="TypeConstraint.TypeName"/>, <see langword="null"/> is returned.</value>
	public Type? UnderlyingType
	{
		get
		{
			ResolveType();
			return _underlyingType;
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
			return _hasFlags;
		}
	}

	/// <summary>
	/// Gets a read-only dictionary of enumeration names and values derived from the type resolved from <see cref="TypeConstraint.TypeName"/>.
	/// </summary>
	/// <value>A read-only dictionary, or if no type could be resolved from <see cref="TypeConstraint.TypeName"/>, <see langword="null"/> is returned.</value>
	public IReadOnlyDictionary<string, object>? EnumValues
	{
		get
		{
			ResolveType();
			return _enumValues;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EnumTypeConstraint"/> class.
	/// </summary>
	public EnumTypeConstraint()
		: base() => ResetFields();

	/// <summary>
	/// Initializes a new instance of the <see cref="EnumTypeConstraint"/> class with the specified type name.
	/// </summary>
	/// <param name="typeName">The assembly-qualified name of the type.</param>
	public EnumTypeConstraint(string typeName)
		: base(typeName) => ResetFields();

	/// <summary>
	/// Initializes a new instance of the <see cref="EnumTypeConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected EnumTypeConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context) => ResetFields();

	/// <summary>
	/// Sets the parameters that the <see cref="Constraint"/> requires to work.
	/// </summary>
	/// <param name="parameters">A enumeration of string parameters.</param>
	/// <param name="dataType">The data type that the constraint needs to restrict.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	/// <exception cref="ConstraintConfigurationException"><paramref name="parameters"/> contains no elements, or an invalid element.</exception>
#if NETSTD20
	protected override void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
#else
	protected override void SetParameters([System.Diagnostics.CodeAnalysis.NotNull] IReadOnlyList<string> parameters, ParameterDataType dataType)
#endif
	{
		if (parameters == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_SetParameters_ParametersNull), nameof(parameters));
		}

		if (dataType == ParameterDataType.None)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.Constraint_SetParameters_TypeNone), nameof(dataType), TextResources.Global_ParameterDataType_None);
		}

		AssertDataType(dataType, ParameterDataType.Enum);
		if (parameters.Count != 1)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumTypeConstraint_SetParameters_OnlyOneParam), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidCount, Name, 1), this);
		}

		if (string.IsNullOrWhiteSpace(parameters[0]))
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumTypeConstraint_SetParameters_ParamNullEmpty), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_Invalid, Name), this);
		}
		TypeName = parameters[0];
		ResetFields();
	}

	/// <summary>
	/// When implemented by a deriving class, checks that the provided value is within the bounds of the constraint.
	/// </summary>
	/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
#if NETSTD20
	protected override void OnValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
#else
	protected override void OnValidation([System.Diagnostics.CodeAnalysis.NotNull] IList<ParameterValidationResult> results, [System.Diagnostics.CodeAnalysis.NotNull] object value, ParameterDataType dataType, [System.Diagnostics.CodeAnalysis.NotNull] string memberName, string? displayName)
#endif
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
				results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumTypeConstraint_Validate_WrongEnum), string.Format(CultureInfo.CurrentCulture, TextResources.EnumTypeConstraint_Validate_WrongEnum, displayName, ValType.FullName, ResolvedType!.FullName), memberName, this));
				return;
			}

			if (_hasFlags)
			{
				long value2 = Convert.ToInt64(value, CultureInfo.InvariantCulture);
				if (((value2 ^ _flagMask) & value2) != 0)
				{
					results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumTypeConstraint_Validate_InvalidFlag), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_InvalidFlag, displayName), memberName, this));
				}
			}
			else
			{
				if (!_enumValues!.ContainsValue(value))
				{
					results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumTypeConstraint_Validate_NotInEnum), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_NotDefined, displayName, value), memberName, this));
				}
			}
		}
		else if (ValType == typeof(int) || ValType == typeof(long) || ValType == typeof(short) || ValType == typeof(byte) ||
			ValType == typeof(uint) || ValType == typeof(ulong) || ValType == typeof(ushort) || ValType == typeof(sbyte))
		{
			if (_hasFlags)
			{
				long value2 = Convert.ToInt64(value, CultureInfo.InvariantCulture);
				if (((value2 ^ _flagMask) & value2) != 0)
				{
					results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumTypeConstraint_Validate_InvalidFlag), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_InvalidFlag, displayName), memberName, this));
				}
			}
			else
			{
				if (!_enumValues!.ContainsValue(Convert.ChangeType(value, ValType, CultureInfo.InvariantCulture)))
				{
					results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumTypeConstraint_Validate_NotInEnum), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_NotDefined, displayName, value), memberName, this));
				}
			}
		}
		else
		{
			results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumTypeConstraint_Validate_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_NotSupported, displayName, ValType.Name), memberName, this));
		}
	}

	/// <summary>
	/// Resolves the <see cref="TypeConstraint.TypeName"/>, if possible, and dissects <see cref="TypeConstraint.ResolvedType"/> to gets its enumeration values, underlying type and Flags attribute.
	/// </summary>
	private void ResolveType()
	{
		lock (_lock)
		{
			if (_isTypeResolved)
			{
				return;
			}
			_isTypeResolved = true;

			Type? enumType = ResolvedType;
			if (enumType == null)
				return;

			try
			{
				_enumValues = ParameterConvert.ExamineEnumeration(enumType, false, out _underlyingType, out _hasFlags);
			}
			catch (CodedArgumentException)
			{
				return;
			}

			if (_hasFlags)
			{
				foreach (object value in _enumValues.Values)
				{
					_flagMask |= Convert.ToInt64(value, CultureInfo.InvariantCulture);
				}
			}
		}
	}

	/// <summary>
	/// Resets all fields set by ResolveType.
	/// </summary>
	private void ResetFields()
	{
		_underlyingType = null;
		_hasFlags = false;
		_enumValues = null;
		_flagMask = 0;
		_isTypeResolved = false;
	}
}
