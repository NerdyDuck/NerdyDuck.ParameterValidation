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

using System.Text;
using NerdyDuck.ParameterValidation.Constraints;

namespace NerdyDuck.ParameterValidation;

/// <summary>
/// Parses a string containing one or more constraints.
/// </summary>
public class ConstraintParser
{
	/// <summary>
	/// Specifies the logical region of a position in a constraint string while being parsed.
	/// </summary>
	private enum ConstraintPosition
	{
		/// <summary>
		/// Outside of a constraint, or at string start.
		/// </summary>
		OutsideConstraint,

		/// <summary>
		/// Within a constraint, but not within parameters.
		/// </summary>
		InConstraint,

		/// <summary>
		/// In a constraint, within the parameters, but not within a parameter
		/// </summary>
		InParameters,

		/// <summary>
		/// In a constraint, within a parameter value
		/// </summary>
		InParameter,

		/// <summary>
		/// In a constraint, after a parameter value
		/// </summary>
		AfterParameter,

		/// <summary>
		/// At end of constraint, only ] is expected
		/// </summary>
		AfterParameters
	}

	/// <summary>
	/// The event that is raised when an unknown is encountered during parsing.
	/// </summary>
	public event EventHandler<UnknownConstraintEventArgs>? UnknownConstraint;

	private static readonly Lazy<ConstraintParser> s_parser = new(() => new ConstraintParser());

	/// <summary>
	/// Gets a global instance of the <see cref="ConstraintParser"/>.
	/// </summary>
	/// <value>A static instance of a <see cref="ConstraintParser"/>.</value>
	public static ConstraintParser Parser => s_parser.Value;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintParser"/> class.
	/// </summary>
	public ConstraintParser()
	{
	}

	/// <summary>
	/// Parses a string containing the textual representation of one or more <see cref="Constraints"/>.
	/// </summary>
	/// <param name="constraints">A string containing one or more constraints. May be null or empty.</param>
	/// <param name="type">The data type to create specific constraints for.</param>
	/// <returns>A collection of <see cref="Constraint"/> instances. If <paramref name="constraints"/> is <see langword="null"/> or empty, an empty list is returned.</returns>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="type"/> is <see cref="ParameterDataType.None"/>.</exception>
	/// <exception cref="ConstraintParserException"><paramref name="constraints"/> is not a valid constraint string.</exception>
	public IReadOnlyList<Constraint> Parse(string? constraints, ParameterDataType type)
	{
		if (type == ParameterDataType.None)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.ConstraintParser_Parse_DataTypeNone), TextResources.Global_ParameterDataType_None);
		}

		if (string.IsNullOrWhiteSpace(constraints))
			return new List<Constraint>();

		ParserContext context = new(constraints, type);
		while (context.MoveNext())
		{
			switch (context.LogicalPosition)
			{
				case ConstraintPosition.OutsideConstraint:
					HandleOutsideConstraint(context);
					break;
				case ConstraintPosition.InConstraint:
					HandleInConstraint(context);
					break;
				case ConstraintPosition.InParameters:
					HandleInParameters(context);
					break;
				case ConstraintPosition.InParameter:
					HandleInParameter(context);
					break;
				case ConstraintPosition.AfterParameter:
					HandleAfterParameter(context);
					break;
				case ConstraintPosition.AfterParameters:
					HandleAfterParameters(context);
					break;
			}
		}

		return context.LogicalPosition != ConstraintPosition.OutsideConstraint
			? throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_Parse_IncompleteConstraint), TextResources.ConstraintParser_Parse_ConstraintIncomplete, context.CurrentConstraintStart)
			: context.Constraints;
	}

	/// <summary>
	/// Creates a string containing one or more string representations of <see cref="Constraint"/>s from the specified list.
	/// </summary>
	/// <param name="constraints">A list of <see cref="Constraints"/>s to concatenate into a string.</param>
	/// <returns>A string containing the string representations of all <see cref="Constraint"/>s in <paramref name="constraints"/>, or <see langword="null"/>, if <paramref name="constraints"/> is <see langword="null"/>.</returns>
	/// <remarks><see cref="NullConstraint"/>s and <see cref="EncryptedConstraint"/>s are moved to the beginning of the string to improve search speed. All other constraints are written in the provided order.</remarks>
	public static string? ConcatConstraints(IReadOnlyList<Constraint>? constraints)
	{
		if (constraints == null)
			return null;

		StringBuilder result = new();
		string nullConstraints = string.Empty;
		string encryptedConstraints = string.Empty;

		foreach (Constraint c in constraints)
		{
			if (c.Name == Constraint.NullConstraintName)
			{
				nullConstraints += c.ToString();
			}
			else if (c.Name == Constraint.EncryptedConstraintName)
			{
				encryptedConstraints += c.ToString();
			}
			else
			{
				_ = result.Append(c.ToString());
			}
		}

		if (!string.IsNullOrEmpty(encryptedConstraints))
		{
			_ = result.Insert(0, encryptedConstraints);
		}

		if (!string.IsNullOrEmpty(nullConstraints))
		{
			_ = result.Insert(0, nullConstraints);
		}

		return result.ToString();
	}

	/// <summary>
	/// Raises the <see cref="UnknownConstraint"/> event, and returns the first constraint resolved by an event handler.
	/// </summary>
	/// <param name="constraintName">The name of the constraint.</param>
	/// <param name="dataType">The data type that the constraint is used for.</param>
	/// <returns>The <see cref="Constraint"/> created by an event handler, based on the <paramref name="constraintName"/>, or <see langword="null"/>, if no event handler was present, or the <paramref name="constraintName"/> is unknown.</returns>
	protected virtual Constraint? OnUnknownConstraint(string constraintName, ParameterDataType dataType)
	{
		EventHandler<UnknownConstraintEventArgs>? ucHandler = UnknownConstraint;
		if (ucHandler != null)
		{
			UnknownConstraintEventArgs e = new(constraintName, dataType);
			foreach (EventHandler<UnknownConstraintEventArgs> handler in ucHandler.GetInvocationList())
			{
				handler.Invoke(this, e);
				if (e.Constraint != null)
				{
					return e.Constraint;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Creates a <see cref="Constraint"/> from the values extracted from a constraint string.
	/// </summary>
	/// <param name="context">The current parsing context.</param>
	/// <returns>A <see cref="Constraint"/>.</returns>

	private Constraint CreateConstraint(ParserContext context)
	{
		Constraint result = FindConstraint(context.CurrentConstraintName, context.DataType);
		try
		{
			result.SetParametersInternal(context.CurrentParameters, context.DataType);
		}
		catch (Exception ex) when (ex is ArgumentException or FormatException)
		{
			throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_CreateConstraint_ParamInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_Parse_ParametersInvalid, result.Name), ex);
		}

		return result;
	}

	/// <summary>
	/// Identifies the correct constraint class to use, according to the constraint name and parameter data type.
	/// </summary>
	/// <param name="name">The name of the constraint.</param>
	/// <param name="type">The type of the parameter that the constraint needs to validate.</param>
	/// <returns>A <see cref="Constraint"/>.</returns>
	/// <remarks>Queries <see cref="UnknownConstraint"/>, if the constraint (or constraint/type combination) is not known.</remarks>
	private Constraint FindConstraint(string name, ParameterDataType type)
	{
		Constraint? result = null;
		bool isConstraintNameUnknown = false;

		switch (name)
		{
			case Constraint.AllowedSchemeConstraintName:
				result = CreateAllowedSchemeConstraint(type);
				break;
			case Constraint.CharacterSetConstraintName:
				result = CreateCharacterSetConstraint(type);
				break;
			case Constraint.DatabaseConstraintName:
				result = new DatabaseConstraint();
				break;
			case Constraint.DecimalPlacesConstraintName:
				result = CreateDecimalPlacesConstraint(type);
				break;
			case Constraint.EncryptedConstraintName:
				result = new EncryptedConstraint();
				break;
			case Constraint.EndpointConstraintName:
				result = CreateEndpointConstraint(type);
				break;
			case Constraint.EnumValuesConstraintName:
				result = CreateEnumValuesConstraint(type);
				break;
			case Constraint.FileNameConstraintName:
				result = CreateFileNameConstraint(type);
				break;
			case Constraint.HostNameConstraintName:
				result = CreateHostnameConstraint(type);
				break;
			case Constraint.LengthConstraintName:
				result = CreateLengthConstraint(type);
				break;
			case Constraint.LowercaseConstraintName:
				result = CreateLowerCaseConstraint(type);
				break;
			case Constraint.MaximumLengthConstraintName:
				result = CreateMaximumLengthConstraint(type);
				break;
			case Constraint.MaximumValueConstraintName:
				result = CreateMaximumValueConstraint(type);
				break;
			case Constraint.MinimumLengthConstraintName:
				result = CreateMinimumLengthConstraint(type);
				break;
			case Constraint.MinimumValueConstraintName:
				result = CreateMinimumValueConstraint(type);
				break;
			case Constraint.NullConstraintName:
				result = new NullConstraint();
				break;
			case Constraint.PasswordConstraintName:
				result = CreatePasswordConstraint(type);
				break;
			case Constraint.PathConstraintName:
				result = CreatePathConstraint(type);
				break;
			case Constraint.RegexConstraintName:
				result = CreateRegexConstraint(type);
				break;
			case Constraint.StepConstraintName:
				result = CreateStepConstraint(type);
				break;
			case Constraint.TypeConstraintName:
				result = CreateTypeConstraint(type);
				break;
			case Constraint.UppercaseConstraintName:
				result = CreateUpperCaseConstraint(type);
				break;
			default:
				// Constraint name is unknown
				isConstraintNameUnknown = true;
				break;
		}

		if (result == null)
		{
			result = HandleUnknownConstraint(name, type, isConstraintNameUnknown);
		}

		return result;
	}

	/// <summary>
	/// Creates a new instance of the <see cref="AllowedSchemeConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="AllowedSchemeConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateAllowedSchemeConstraint(ParameterDataType type) => type == ParameterDataType.Uri ? new AllowedSchemeConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="CharacterSetConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="CharacterSetConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateCharacterSetConstraint(ParameterDataType type) => type == ParameterDataType.String ? new CharacterSetConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="DecimalPlacesConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="DecimalPlacesConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateDecimalPlacesConstraint(ParameterDataType type) => type == ParameterDataType.Decimal ? new DecimalPlacesConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="EndpointConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="EndpointConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateEndpointConstraint(ParameterDataType type) => type == ParameterDataType.String ? new EndpointConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="FileNameConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="FileNameConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateFileNameConstraint(ParameterDataType type) => type == ParameterDataType.String ? new FileNameConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="HostNameConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="HostNameConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateHostnameConstraint(ParameterDataType type) => type == ParameterDataType.String ? new HostNameConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="LengthConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="LengthConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateLengthConstraint(ParameterDataType type) => type is ParameterDataType.String or ParameterDataType.Bytes ? new LengthConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="LowercaseConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="LowercaseConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateLowerCaseConstraint(ParameterDataType type) => type == ParameterDataType.String ? new LowercaseConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="MaximumLengthConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="MaximumLengthConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateMaximumLengthConstraint(ParameterDataType type) => type switch
	{
		ParameterDataType.Bytes or ParameterDataType.String or ParameterDataType.Uri => new MaximumLengthConstraint(),
		_ => null,
	};

	/// <summary>
	/// Creates a new instance of the <see cref="MaximumValueConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="MaximumValueConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateMaximumValueConstraint(ParameterDataType type) => type switch
	{
		ParameterDataType.Byte or ParameterDataType.DateTimeOffset or ParameterDataType.Decimal or ParameterDataType.Int16 or ParameterDataType.Int32 or ParameterDataType.Int64 or ParameterDataType.SignedByte or ParameterDataType.TimeSpan or ParameterDataType.UInt16 or ParameterDataType.UInt32 or ParameterDataType.UInt64 or ParameterDataType.Version => new MaximumValueConstraint(type),
		_ => null,
	};

	/// <summary>
	/// Creates a new instance of the <see cref="MinimumLengthConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="MinimumLengthConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateMinimumLengthConstraint(ParameterDataType type) => type switch
	{
		ParameterDataType.Bytes or ParameterDataType.String or ParameterDataType.Uri => new MinimumLengthConstraint(),
		_ => null,
	};

	/// <summary>
	/// Creates a new instance of the <see cref="MinimumValueConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="MinimumValueConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateMinimumValueConstraint(ParameterDataType type) => type switch
	{
		ParameterDataType.Byte or ParameterDataType.DateTimeOffset or ParameterDataType.Decimal or ParameterDataType.Int16 or ParameterDataType.Int32 or ParameterDataType.Int64 or ParameterDataType.SignedByte or ParameterDataType.TimeSpan or ParameterDataType.UInt16 or ParameterDataType.UInt32 or ParameterDataType.UInt64 or ParameterDataType.Version => new MinimumValueConstraint(type),
		_ => null,
	};

	/// <summary>
	/// Creates a new instance of the <see cref="PasswordConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="PasswordConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreatePasswordConstraint(ParameterDataType type) => type == ParameterDataType.String ? new PasswordConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="PathConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="PathConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreatePathConstraint(ParameterDataType type) => type == ParameterDataType.String ? new PathConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="RegexConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="RegexConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateRegexConstraint(ParameterDataType type) => type == ParameterDataType.String ? new RegexConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="StepConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="StepConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateStepConstraint(ParameterDataType type) => type switch
	{
		ParameterDataType.Byte or ParameterDataType.Decimal or ParameterDataType.Int16 or ParameterDataType.Int32 or ParameterDataType.Int64 or ParameterDataType.SignedByte or ParameterDataType.UInt16 or ParameterDataType.UInt32 or ParameterDataType.UInt64 => new StepConstraint(type),
		_ => null,
	};

	/// <summary>
	/// Creates a new instance of the <see cref="TypeConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="TypeConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateTypeConstraint(ParameterDataType type) => type switch
	{
		ParameterDataType.Enum => new EnumTypeConstraint(),
		ParameterDataType.Xml => new TypeConstraint(),
		_ => null,
	};

	/// <summary>
	/// Creates a new instance of the <see cref="UppercaseConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="UppercaseConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateUpperCaseConstraint(ParameterDataType type) => type == ParameterDataType.String ? new UppercaseConstraint() : null;

	/// <summary>
	/// Creates a new instance of the <see cref="EnumValuesConstraint"/>, if the data type matches
	/// </summary>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <returns>A <see cref="EnumValuesConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
	private static Constraint? CreateEnumValuesConstraint(ParameterDataType type) => type == ParameterDataType.Enum ? new EnumValuesConstraint() : null;

	/// <summary>
	/// Handles an unknown constraint, or when a constraint is used for a data type that is originally not supported.
	/// </summary>
	/// <param name="name">The name of the constraint.</param>
	/// <param name="type">The data type to create the constraint for.</param>
	/// <param name="isConstraintNameUnknown">A value indicating if the constraint name is well-known.</param>
	/// <returns>A <see cref="Constraint"/>.</returns>
	private Constraint HandleUnknownConstraint(string name, ParameterDataType type, bool isConstraintNameUnknown)
	{
		Constraint? result = OnUnknownConstraint(name, type);

		if (result == null)
		{
			if (isConstraintNameUnknown)
				// Constraint name is unknown
				throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleUnknownConstraint_UnknownConstraint), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_HandleUnknownConstraint_UnknownName, name));
			else
				// Constraint is known, but not for specified data type
				throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleUnknownConstraint_NotDefinedForType), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_HandleUnknownConstraint_NotDefined, name, type));
		}

		return result;
	}

	/// <summary>
	/// Handles the next character while outside of a constraint.
	/// </summary>
	/// <param name="context">The current parsing context.</param>
	private static void HandleOutsideConstraint(ParserContext context)
	{
		switch (context.CurrentCharacter)
		{
			case ' ': // Space between constraints is allowed
				break;
			case '[': // Constraint start
				context.LogicalPosition = ConstraintPosition.InConstraint;
				context.CurrentConstraintStart = context.StringPosition;
				break;
			default: // Invalid char outside of constraint
				throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleOutsideConstraint_DataOutside), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_Parse_InvalidContent, context.StringPosition), context.StringPosition);
		}

	}

	/// <summary>
	/// Handles the next character while inside of a constraint.
	/// </summary>
	/// <param name="context">The current parsing context.</param>
	private void HandleInConstraint(ParserContext context)
	{
		switch (context.CurrentCharacter)
		{
			case ' ': // No spaces in names
			case '\'': // No quote in names
			case '[': // Invalid constraint start in constraint
			case ')': // Invalid parameter end in constraint
				throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleInConstraint_InvalidChar), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_Parse_InvalidCharInName, context.CurrentCharacter), context.StringPosition);
			case ']': // Constraint without parameters
				context.CurrentConstraintStart++;
				if (context.StringPosition == context.CurrentConstraintStart)
				{
					// No constraint name
					throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleInConstraint_EmptyConstraint), TextResources.ConstraintParser_Parse_EmptyConstraint, context.StringPosition);
				}
				context.CurrentConstraintName = context.GetSubstring(context.CurrentConstraintStart, context.StringPosition - context.CurrentConstraintStart);
				context.Constraints.Add(CreateConstraint(context));
				context.ResetConstraintData();
				break;
			case '(': // Start of parameters
				context.CurrentConstraintStart++;
				if (context.StringPosition == context.CurrentConstraintStart)
				{
					// No constraint name
					throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleInConstraint_EmptyConstraint), TextResources.ConstraintParser_Parse_EmptyConstraint, context.StringPosition);
				}
				context.CurrentConstraintName = context.GetSubstring(context.CurrentConstraintStart, context.StringPosition - context.CurrentConstraintStart);
				context.LogicalPosition = ConstraintPosition.InParameters;
				break;
		}
	}

	/// <summary>
	/// Handles the next character while inside the parameters.
	/// </summary>
	/// <param name="context">The current parsing context.</param>
	private static void HandleInParameters(ParserContext context)
	{
		switch (context.CurrentCharacter)
		{
			case ' ': // Space between parameters is allowed
				break;
			case ',': // Comma must be set after first constraint
			case '[':
			case ']':
			case '(': // Invalid characters within parameters if unmasked
				throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleInParameters_ParamCharInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_Parse_InvalidCharInParameters, context.CurrentCharacter), context.StringPosition);
			case ')': // End of parameters
				context.LogicalPosition = ConstraintPosition.AfterParameters;
				break;
			case '\'': // Parameter is masked
				context.IsMasked = true;
				context.LogicalPosition = ConstraintPosition.InParameter;
				break;
			default: // First character of parameter
				context.LogicalPosition = ConstraintPosition.InParameter;
				context.AppendParameter();
				break;
		}
	}

	/// <summary>
	/// Handles the next character while inside a parameter.
	/// </summary>
	/// <param name="context">The current parsing context.</param>
	private static void HandleInParameter(ParserContext context)
	{
		if (context.IsMasked)
		{
			// All characters allowed
			if (context.CurrentCharacter == '\'')
			{
				if (context.IsNextCharQuote())
				{
					// Masked ', so skip next character and continue
					context.AppendParameter();
					context.SkipOne();
				}
				else
				{
					// End of masked parameter
					context.LogicalPosition = ConstraintPosition.AfterParameter;
					context.AddParameter();
				}
			}
			else
			{
				// All characters allowed
				context.AppendParameter();
			}
		}
		else
		{
			switch (context.CurrentCharacter)
			{
				case ' ': // Space after parameter is allowed
					context.LogicalPosition = ConstraintPosition.AfterParameter;
					if (context.CurrentParameter.Length > 0)
						context.AddParameter();
					break;
				case ',': // End of parameter
					context.LogicalPosition = ConstraintPosition.InParameters;
					if (context.CurrentParameter.Length > 0)
						context.AddParameter();
					break;
				case ')': // At end of parameters
					context.LogicalPosition = ConstraintPosition.AfterParameters;
					if (context.CurrentParameter.Length > 0)
						context.AddParameter();
					break;
				case '[':
				case ']':
				case '(':
				case '\'': // Invalid delimiters without masking
					throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleInParameter_ParamUnmaskedChar), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_Parse_UnmaskedChar, context.CurrentCharacter), context.StringPosition);
				default: // All other characters allowed in parameter
					context.AppendParameter();
					break;

			}
		}

	}

	/// <summary>
	/// Handles the next character while after a parameter.
	/// </summary>
	/// <param name="context">The current parsing context.</param>
	private static void HandleAfterParameter(ParserContext context)
	{
		switch (context.CurrentCharacter)
		{
			case ' ': // Space after parameter is allowed
				break;
			case ',': // Next parameter
				context.LogicalPosition = ConstraintPosition.InParameters;
				break;
			case ')': // At end of parameters
				context.LogicalPosition = ConstraintPosition.AfterParameters;
				break;
			default: // Invalid character
				throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleAfterParameter_CharAfterParam), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_Parse_InvalidCharAfterParam, context.CurrentCharacter), context.StringPosition);
		}

	}

	/// <summary>
	/// Handles the next character while after parameters.
	/// </summary>
	/// <param name="context">The current parsing context.</param>
	private void HandleAfterParameters(ParserContext context)
	{
		switch (context.CurrentCharacter)
		{
			case ']': // Constraint end
				context.Constraints.Add(CreateConstraint(context));
				context.ResetConstraintData();
				break;
			default: // Invalid char after parameters
				throw new ConstraintParserException(HResult.Create(ErrorCodes.ConstraintParser_HandleAfterParameters_CharAfterParams), string.Format(CultureInfo.CurrentCulture, TextResources.ConstraintParser_Parse_InvalidCharAfterParams, context.CurrentCharacter), context.StringPosition);
		}

	}

	/// <summary>
	/// Represents the context data while parsing a constraint string.
	/// </summary>
	private class ParserContext
	{
		private readonly int _stringLength;
		private readonly string _constraintString;

		/// <summary>
		/// A list of Constraints extracted from ConstraintString.
		/// </summary>
		public List<Constraint> Constraints;

		/// <summary>
		/// The data type of the parameter that the constraint string handles.
		/// </summary>
		public ParameterDataType DataType;

		/// <summary>
		/// The logical area in ConstraintString that StringPosition points to.
		/// </summary>
		public ConstraintPosition LogicalPosition;

		/// <summary>
		/// The position in ConstraintString that is currently handled.
		/// </summary>
		public int StringPosition;

		/// <summary>
		/// The starting position in ConstraintString of the Constraint currently being parsed.
		/// </summary>
		public int CurrentConstraintStart;

		/// <summary>
		/// A list of parameters of the Constraint currently being parsed.
		/// </summary>
		public List<string> CurrentParameters;

		/// <summary>
		/// The name of the Constraint currently being parsed.
		/// </summary>
		public string CurrentConstraintName;

		/// <summary>
		/// Indicates if the current position in ConstraintString is masked.
		/// </summary>
		public bool IsMasked;

		/// <summary>
		/// The characters of the Constraint parameter currently being parsed.
		/// </summary>
		public StringBuilder CurrentParameter;

		/// <summary>
		/// The character currently being handled.
		/// </summary>
		public char CurrentCharacter;

		/// <summary>
		/// Initializes a new instance of the ParserContext class with the specified constraint string.
		/// </summary>
		/// <param name="constraintString">The constraint string to parse.</param>
		/// <param name="dataType">The data type of the parameter.</param>
		internal ParserContext(string? constraintString, ParameterDataType dataType)
		{
			_constraintString = constraintString ?? string.Empty;
			DataType = dataType;
			Constraints = new List<Constraint>();
			LogicalPosition = ConstraintPosition.OutsideConstraint;
			CurrentConstraintStart = -1;
			StringPosition = -1;
			CurrentParameters = new List<string>();
			CurrentConstraintName = string.Empty;
			IsMasked = false;
			CurrentParameter = new StringBuilder();
			CurrentCharacter = char.MinValue;
			_stringLength = _constraintString.Length;
		}

		/// <summary>
		/// Advances the current position in ConstraintString and sets CurrentCharacter.
		/// </summary>
		/// <returns>true, if the position was advanced to the next character; false, if the end of the string is reached.</returns>
		public bool MoveNext()
		{
			StringPosition++;
			if (StringPosition < _stringLength)
			{
				CurrentCharacter = _constraintString[StringPosition];
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks if the next character is a quote
		/// </summary>
		/// <returns>true, if the next character is a quote; false, if it is not, or the end of the string is reached.</returns>
		public bool IsNextCharQuote()
		{
			int i = StringPosition + 1;
			return i < _stringLength && _constraintString[i] == '\'';
		}

		/// <summary>
		/// Gets a substring from the constraint string.
		/// </summary>
		/// <param name="startIndex">The start of the substring.</param>
		/// <param name="length">The length of the substring.</param>
		/// <returns>The substring to retrieve.</returns>
		public string GetSubstring(int startIndex, int length) => _constraintString.Substring(startIndex, length);

		/// <summary>
		/// Resets the field storing constraint information and the logical position to OutsideConstraint.
		/// </summary>
		public void ResetConstraintData()
		{
			CurrentConstraintStart = -1;
			CurrentParameters.Clear();
			CurrentConstraintName = string.Empty;
			IsMasked = false;
			LogicalPosition = ConstraintPosition.OutsideConstraint;
		}

		/// <summary>
		/// Adds the content of CurrentParameter to CurrentParameters, and resets it.
		/// </summary>
		public void AddParameter()
		{
			CurrentParameters.Add(CurrentParameter.ToString());
			_ = CurrentParameter.Clear();
			IsMasked = false;
		}

		/// <summary>
		/// Appends the current character to CurrentParameter.
		/// </summary>
		public void AppendParameter() => _ = CurrentParameter.Append(CurrentCharacter);

		/// <summary>
		/// Skips the next character.
		/// </summary>
		public void SkipOne() => StringPosition++;
	}
}
