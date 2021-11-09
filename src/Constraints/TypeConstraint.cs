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

namespace NerdyDuck.ParameterValidation.Constraints;

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
[Serializable]
public class TypeConstraint : Constraint
{
	private string _typeName;
	private Type? _resolvedType;
	private bool _isTypeResolved;
	private readonly object _lock = new();

	/// <summary>
	/// Gets the assembly-qualified name of the type.
	/// </summary>
	/// <value>A string containing the full name of a type and the assembly that defines it.</value>
	public string TypeName
	{
		get => _typeName;
		protected set
		{
			if (_typeName != value)
			{
				_typeName = value;
				_resolvedType = null;
				_isTypeResolved = false;
			}
		}
	}

	/// <summary>
	/// Gets the type resolved from <see cref="TypeName"/>.
	/// </summary>
	/// <value>The resolved type; if no type could be resolved from <see cref="TypeName"/>, <see langword="null"/> is returned.</value>
	public Type? ResolvedType
	{
		get
		{
			ResolveType();
			return _resolvedType;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TypeConstraint"/> class.
	/// </summary>
	public TypeConstraint()
		: base(TypeConstraintName)
	{
		_typeName = string.Empty;
		_resolvedType = null;
		_isTypeResolved = false;
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
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.TypeConstraint_ctor_TypeNullEmpty), nameof(typeName));
		}
		_typeName = typeName;
		_resolvedType = null;
		_isTypeResolved = false;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TypeConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected TypeConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		_typeName = info.GetString(nameof(TypeName)) ?? throw new CodedSerializationException(HResult.Create(ErrorCodes.TypeConstraint_ctor_TypeName), TextResources.TypeConstraint_ctor_TypeName);
		_resolvedType = null;
		_isTypeResolved = false;
	}

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
#if NETSTD20
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
#else
	public override void GetObjectData([System.Diagnostics.CodeAnalysis.NotNull] SerializationInfo info, StreamingContext context)
#endif
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(TypeName), _typeName);
	}

	/// <summary>
	/// Adds the parameters of the constraint to a list of strings.
	/// </summary>
	/// <param name="parameters">A list of strings to add the parameters to.</param>
	/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
#if NETSTD20
	protected override void GetParameters(IList<string> parameters)
#else
	protected override void GetParameters([System.Diagnostics.CodeAnalysis.NotNull] IList<string> parameters)
#endif
	{
		base.GetParameters(parameters);
		parameters.Add(_typeName);
	}

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
		base.SetParameters(parameters, dataType);
		AssertDataType(dataType, ParameterDataType.Xml);
		if (parameters.Count != 1)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.TypeConstraint_SetParameters_OnlyOneParam), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidCount, Name, 1), this);
		}

		if (string.IsNullOrWhiteSpace(parameters[0]))
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.TypeConstraint_SetParameters_TypeNullEmpty), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_Invalid, Name), this);
		}
		_typeName = parameters[0];
		_resolvedType = null;
		_isTypeResolved = false;
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
		OnBaseValidation(results, value, dataType, memberName, displayName);
		AssertDataType(dataType, ParameterDataType.Xml);
	}

	/// <summary>
	/// Calls <see cref="Constraint.OnValidation(IList{ParameterValidationResult}, object, ParameterDataType, string, string)"/> of the base class.
	/// </summary>
	/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
	/// <remarks>This method is a small hack to be used by the derived <see cref="EnumTypeConstraint"/> class.</remarks>
#if NETSTD20
	protected void OnBaseValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
#else
	protected void OnBaseValidation([System.Diagnostics.CodeAnalysis.NotNull] IList<ParameterValidationResult> results, [System.Diagnostics.CodeAnalysis.NotNull] object value, ParameterDataType dataType, [System.Diagnostics.CodeAnalysis.NotNull] string memberName, string? displayName)
#endif
		=> base.OnValidation(results, value, dataType, memberName, displayName);

	/// <summary>
	/// Resolves the <see cref="TypeName"/>, if possible, and stores it in <see cref="ResolvedType"/>.
	/// </summary>
	private void ResolveType()
	{
		lock (_lock)
		{
			if (_isTypeResolved)
			{
				return;
			}
			_resolvedType = Type.GetType(_typeName, false);
			_isTypeResolved = true;
		}
	}
}
