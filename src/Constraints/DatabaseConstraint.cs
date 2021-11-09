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
/// A constraint specifying that the parameter value is a key of a database entity.
/// </summary>
/// <remarks>
/// <para>
/// <list type="bullet">
/// <item>The textual representation of the constraint is <c>[Database(Entity,KeyProperty[,DisplayProperty])]</c>.</item>
/// <item>The constraint is applicable to all data types.</item>
/// <item>If a string parameter with that constraint uses characters not defined in the character set, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
/// <item>The constraint is not used during validation or serialization, but is solely thought as a hint for display purposes, e.g. to get a list of valid choice from a database.</item>
/// </list>
/// </para>
/// </remarks>
[Serializable]
public class DatabaseConstraint : Constraint
{
	/// <summary>
	/// Gets the name of the database entity (e.g. table, view, or domain model entity) that stores the value of the parameter.
	/// </summary>
	/// <value>A table, view or entity name.</value>
	public string Entity { get; private set; }

	/// <summary>
	/// Gets the name of the property (e.g. field name) that contains the value of the parameter.
	/// </summary>
	/// <value>A column or property name.</value>
	public string KeyProperty { get; private set; }

	/// <summary>
	/// Gets the name of the property (e.g. field name) that should be displayed instead of <see cref="KeyProperty"/>. May be empty.
	/// </summary>
	/// <value>A column or property name.</value>
	public string DisplayProperty { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseConstraint"/> class.
	/// </summary>
	public DatabaseConstraint()
		: this(string.Empty, string.Empty, string.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseConstraint"/> class with the specified <see cref="Uri"/> scheme.
	/// </summary>
	/// <param name="entity">The name of the database entity (e.g. table, view, or domain model entity) that stores the value of the parameter.</param>
	/// <param name="keyProperty">The name of the property (e.g. field name) that contains the value of the parameter.</param>
	/// <param name="displayProperty">The name of the property (e.g. field name) that should be displayed instead of <paramref name="keyProperty"/>. May be empty.</param>
	public DatabaseConstraint(string entity, string keyProperty, string displayProperty)
		: base(DatabaseConstraintName)
	{
		Entity = entity ?? string.Empty;
		KeyProperty = keyProperty ?? string.Empty;
		DisplayProperty = displayProperty ?? string.Empty;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DatabaseConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be serialized correctly.</exception>
	protected DatabaseConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Entity = info.GetString(nameof(Entity)) ?? string.Empty;
		KeyProperty = info.GetString(nameof(KeyProperty)) ?? string.Empty;
		DisplayProperty = info.GetString(nameof(DisplayProperty)) ?? string.Empty;
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
		info.AddValue(nameof(Entity), Entity);
		info.AddValue(nameof(KeyProperty), KeyProperty);
		info.AddValue(nameof(DisplayProperty), DisplayProperty);
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
		parameters.Add(Entity);
		parameters.Add(KeyProperty);
		parameters.Add(DisplayProperty);
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
		if (parameters.Count is < 1 or > 3)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.DatabaseConstraint_SetParameters_ParamCountInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidCountVariable, Name, 2, 3), this);
		}

		Entity = parameters[0] ?? string.Empty;
		if (parameters.Count > 1)
		{
			KeyProperty = parameters[1] ?? string.Empty;
		}
		if (parameters.Count > 2)
		{
			DisplayProperty = parameters[2] ?? string.Empty;
		}
	}
}
