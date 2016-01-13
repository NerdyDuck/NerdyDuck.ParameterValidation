#region Copyright
/*******************************************************************************
 * <copyright file="ConstraintParser.cs" owner="Daniel Kopp">
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
 * <file name="ConstraintParser.cs" date="2015-10-19">
 * Parses a string containing one or more constraints.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Parses a string containing one or more constraints.
	/// </summary>
	public class ConstraintParser
	{
		#region Private enumeration
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
		#endregion

		#region Events
		/// <summary>
		/// The event that is raised when an unknown is encountered during parsing.
		/// </summary>
		public event EventHandler<UnknownConstraintEventArgs> UnknownConstraint;
		#endregion

		#region Private fields
		// Singleton with on-demand instantiation, .NET 4 style
		private static readonly Lazy<ConstraintParser> mParser = new Lazy<ConstraintParser>(() =>
		{
			return new ConstraintParser();
		});
		#endregion

		#region Properties
		/// <summary>
		/// Gets a global instance of the <see cref="ConstraintParser"/>.
		/// </summary>
		/// <value>A static instance of a <see cref="ConstraintParser"/>.</value>
		public static ConstraintParser Parser
		{
			get { return mParser.Value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParser"/> class.
		/// </summary>
		public ConstraintParser()
		{
		}
		#endregion

		#region Public methods
		#region Parse
		/// <summary>
		/// Parses a string containing the textual representation of one or more <see cref="Constraints"/>.
		/// </summary>
		/// <param name="constraints">A string containing one or more constraints. May be null or empty.</param>
		/// <param name="type">The data type to create specific constraints for.</param>
		/// <returns>A collection of <see cref="Constraint"/> instances. If <paramref name="constraints"/> is <see langword="null"/> or empty, an empty list is returned.</returns>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="type"/> is <see cref="ParameterDataType.None"/>.</exception>
		/// <exception cref="ConstraintParserException"><paramref name="constraints"/> is not a valid constraint string.</exception>
		public IReadOnlyList<Constraint> Parse(string constraints, ParameterDataType type)
		{
			if (type == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x8c), Properties.Resources.Global_ParameterDataType_None);
			}

			if (string.IsNullOrWhiteSpace(constraints))
				return new List<Constraint>();

			ParserContext context = new ParserContext(constraints, type);
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

			if (context.LogicalPosition != ConstraintPosition.OutsideConstraint)
			{
				throw new ConstraintParserException(Errors.CreateHResult(0xa1), Properties.Resources.ConstraintParser_Parse_ConstraintIncomplete, context.CurrentConstraintStart);
			}

			return context.Constraints;
		}
		#endregion

		#region ConcatConstraints
		/// <summary>
		/// Creates a string containing one or more string representations of <see cref="Constraint"/>s from the specified list.
		/// </summary>
		/// <param name="constraints">A list of <see cref="Constraints"/>s to concatenate into a string.</param>
		/// <returns>A string containing the string representations of all <see cref="Constraint"/>s in <paramref name="constraints"/>, or <see langword="null"/>, if <paramref name="constraints"/> is <see langword="null"/>.</returns>
		/// <remarks><see cref="NullConstraint"/>s and <see cref="EncryptedConstraint"/>s are moved to the beginning of the string to improve search speed. All other constraints are written in the provided order.</remarks>
		public static string ConcatConstraints(IList<Constraint> constraints)
		{
			if (constraints == null)
				return null;

			StringBuilder ReturnValue = new StringBuilder();
			string NullConstraints = string.Empty;
			string EncryptedConstraints = string.Empty;

			foreach (Constraint c in constraints)
			{
				if (c.Name == Constraint.NullConstraintName)
				{
					NullConstraints += c.ToString();
				}
				else if (c.Name == Constraint.EncryptedConstraintName)
				{
					EncryptedConstraints += c.ToString();
				}
				else
				{
					ReturnValue.Append(c.ToString());
				}
			}

			if (!string.IsNullOrEmpty(EncryptedConstraints))
			{
				ReturnValue.Insert(0, EncryptedConstraints);
			}

			if (!string.IsNullOrEmpty(NullConstraints))
			{
				ReturnValue.Insert(0, NullConstraints);
			}

			return ReturnValue.ToString();
		}
		#endregion
		#endregion

		#region Protected methods
		/// <summary>
		/// Raises the <see cref="UnknownConstraint"/> event, and returns the first constraint resolved by an event handler.
		/// </summary>
		/// <param name="constraintName">The name of the constraint.</param>
		/// <param name="dataType">The data type that the constraint is used for.</param>
		/// <returns>The <see cref="Constraint"/> created by an event handler, based on the <paramref name="constraintName"/>, or <see langword="null"/>, if no event handler was present, or the <paramref name="constraintName"/> is unknown.</returns>
		protected virtual Constraint OnUnknownConstraint(string constraintName, ParameterDataType dataType)
		{
			EventHandler<UnknownConstraintEventArgs> ucHandler = UnknownConstraint;
			if (ucHandler != null)
			{
				UnknownConstraintEventArgs e = new UnknownConstraintEventArgs(constraintName, dataType);
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
		#endregion

		#region Private methods
		#region CreateConstraint
		/// <summary>
		/// Creates a <see cref="Constraint"/> from the values extracted from a constraint string.
		/// </summary>
		/// <param name="context">The current parsing context.</param>
		/// <returns>A <see cref="Constraint"/>.</returns>

		private Constraint CreateConstraint(ParserContext context)
		{
			Constraint ReturnValue = null;

			ReturnValue = FindConstraint(context.CurrentConstraintName, context.DataType);
			try
			{
				ReturnValue.SetParametersInternal(context.CurrentParameters, context.DataType);
			}
			catch (Exception ex) when (ex is ArgumentException || ex is FormatException)
			{
				throw new ConstraintParserException(Errors.CreateHResult(0x8b), string.Format(Properties.Resources.ConstraintParser_Parse_ParametersInvalid, ReturnValue.Name), ex);
			}

			return ReturnValue;
		}
		#endregion

		#region FindConstraint
		/// <summary>
		/// Identifies the correct constraint class to use, according to the constraint name and parameter data type.
		/// </summary>
		/// <param name="name">The name of the constraint.</param>
		/// <param name="type">The type of the parameter that the constraint needs to validate.</param>
		/// <returns>A <see cref="Constraint"/>.</returns>
		/// <remarks>Queries <see cref="UnknownConstraint"/>, if the constraint (or constraint/type combination) is not known.</remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private Constraint FindConstraint(string name, ParameterDataType type)
		{
			Constraint ReturnValue = null;
			bool IsConstraintNameUnknown = false;

			switch (name)
			{
				case Constraint.AllowedSchemeConstraintName:
					ReturnValue = CreateAllowedSchemeConstraint(type);
					break;
				case Constraint.CharacterSetConstraintName:
					ReturnValue = CreateCharacterSetConstraint(type);
					break;
				case Constraint.DatabaseConstraintName:
					ReturnValue = new DatabaseConstraint();
					break;
				case Constraint.DecimalPlacesConstraintName:
					ReturnValue = CreateDecimalPlacesConstraint(type);
					break;
				case Constraint.EncryptedConstraintName:
					ReturnValue = new EncryptedConstraint();
					break;
				case Constraint.EndpointConstraintName:
					ReturnValue = CreateEndpointConstraint(type);
					break;
				case Constraint.EnumValuesConstraintName:
					ReturnValue = CreateEnumValuesConstraint(type);
					break;
				case Constraint.FileNameConstraintName:
					ReturnValue = CreateFileNameConstraint(type);
					break;
				case Constraint.HostNameConstraintName:
					ReturnValue = CreateHostnameConstraint(type);
					break;
				case Constraint.LengthConstraintName:
					ReturnValue = CreateLengthConstraint(type);
					break;
				case Constraint.LowercaseConstraintName:
					ReturnValue = CreateLowerCaseConstraint(type);
					break;
				case Constraint.MaximumLengthConstraintName:
					ReturnValue = CreateMaximumLengthConstraint(type);
					break;
				case Constraint.MaximumValueConstraintName:
					ReturnValue = CreateMaximumValueConstraint(type);
					break;
				case Constraint.MinimumLengthConstraintName:
					ReturnValue = CreateMinimumLengthConstraint(type);
					break;
				case Constraint.MinimumValueConstraintName:
					ReturnValue = CreateMinimumValueConstraint(type);
					break;
				case Constraint.NullConstraintName:
					ReturnValue = new NullConstraint();
					break;
				case Constraint.PasswordConstraintName:
					ReturnValue = CreatePasswordConstraint(type);
					break;
				case Constraint.PathConstraintName:
					ReturnValue = CreatePathConstraint(type);
					break;
				case Constraint.RegexConstraintName:
					ReturnValue = CreateRegexConstraint(type);
					break;
				case Constraint.StepConstraintName:
					ReturnValue = CreateStepConstraint(type);
					break;
				case Constraint.TypeConstraintName:
					ReturnValue = CreateTypeConstraint(type);
					break;
				case Constraint.UppercaseConstraintName:
					ReturnValue = CreateUpperCaseConstraint(type);
					break;
				default:
					// Constraint name is unknown
					IsConstraintNameUnknown = true;
					break;
			}

			if (ReturnValue == null)
			{
				ReturnValue = HandleUnknownConstraint(name, type, IsConstraintNameUnknown);
			}

			return ReturnValue;
		}
		#endregion

		#region CreateAllowedSchemeConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="AllowedSchemeConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="AllowedSchemeConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateAllowedSchemeConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.Uri)
			{
				return new AllowedSchemeConstraint();
			}

			return null;
		}
		#endregion

		#region CreateCharacterSetConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="CharacterSetConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="CharacterSetConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateCharacterSetConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new CharacterSetConstraint();
			}

			return null;
		}
		#endregion

		#region CreateDecimalPlacesConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="DecimalPlacesConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="DecimalPlacesConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateDecimalPlacesConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.Decimal)
			{
				return new DecimalPlacesConstraint();
			}

			return null;
		}
		#endregion

		#region CreateEndpointConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="EndpointConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="EndpointConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateEndpointConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new EndpointConstraint();
			}

			return null;
		}
		#endregion

		#region CreateFileNameConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="FileNameConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="FileNameConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateFileNameConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new FileNameConstraint();
			}

			return null;
		}
		#endregion

		#region CreateHostnameConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="HostNameConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="HostNameConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateHostnameConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new HostNameConstraint();
			}

			return null;
		}
		#endregion

		#region CreateLengthConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="LengthConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="LengthConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateLengthConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String || type == ParameterDataType.Bytes)
			{
				return new LengthConstraint();
			}

			return null;
		}
		#endregion

		#region CreateLowerCaseConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="LowercaseConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="LowercaseConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateLowerCaseConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new LowercaseConstraint();
			}

			return null;
		}
		#endregion

		#region CreateMaximumLengthConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="MaximumLengthConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="MaximumLengthConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateMaximumLengthConstraint(ParameterDataType type)
		{
			switch (type)
			{
				case ParameterDataType.Bytes:
				case ParameterDataType.String:
				case ParameterDataType.Uri:
					return new MaximumLengthConstraint();
			}

			return null;
		}
		#endregion

		#region CreateMaximumValueConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="MaximumValueConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="MaximumValueConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateMaximumValueConstraint(ParameterDataType type)
		{
			switch (type)
			{
				case ParameterDataType.Byte:
				case ParameterDataType.DateTimeOffset:
				case ParameterDataType.Decimal:
				case ParameterDataType.Int16:
				case ParameterDataType.Int32:
				case ParameterDataType.Int64:
				case ParameterDataType.SignedByte:
				case ParameterDataType.TimeSpan:
				case ParameterDataType.UInt16:
				case ParameterDataType.UInt32:
				case ParameterDataType.UInt64:
				case ParameterDataType.Version:
					return new MaximumValueConstraint(type);
			}

			return null;
		}
		#endregion

		#region CreateMinimumLengthConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="MinimumLengthConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="MinimumLengthConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateMinimumLengthConstraint(ParameterDataType type)
		{
			switch (type)
			{
				case ParameterDataType.Bytes:
				case ParameterDataType.String:
				case ParameterDataType.Uri:
					return new MinimumLengthConstraint();
			}

			return null;
		}
		#endregion

		#region CreateMinimumValueConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="MinimumValueConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="MinimumValueConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateMinimumValueConstraint(ParameterDataType type)
		{
			switch (type)
			{
				case ParameterDataType.Byte:
				case ParameterDataType.DateTimeOffset:
				case ParameterDataType.Decimal:
				case ParameterDataType.Int16:
				case ParameterDataType.Int32:
				case ParameterDataType.Int64:
				case ParameterDataType.SignedByte:
				case ParameterDataType.TimeSpan:
				case ParameterDataType.UInt16:
				case ParameterDataType.UInt32:
				case ParameterDataType.UInt64:
				case ParameterDataType.Version:
					return new MinimumValueConstraint(type);
			}

			return null;
		}
		#endregion

		#region CreatePasswordConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="PasswordConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="PasswordConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreatePasswordConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new PasswordConstraint();
			}

			return null;
		}
		#endregion

		#region CreatePathConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="PathConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="PathConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreatePathConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new PathConstraint();
			}

			return null;
		}
		#endregion

		#region CreateRegexConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="RegexConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="RegexConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateRegexConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new RegexConstraint();
			}

			return null;
		}
		#endregion

		#region CreateStepConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="StepConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="StepConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateStepConstraint(ParameterDataType type)
		{
			switch (type)
			{
				case ParameterDataType.Byte:
				case ParameterDataType.Decimal:
				case ParameterDataType.Int16:
				case ParameterDataType.Int32:
				case ParameterDataType.Int64:
				case ParameterDataType.SignedByte:
				case ParameterDataType.UInt16:
				case ParameterDataType.UInt32:
				case ParameterDataType.UInt64:
					return new StepConstraint(type);
			}

			return null;
		}
		#endregion

		#region CreateTypeConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="TypeConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="TypeConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateTypeConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.Enum)
			{
				return new EnumTypeConstraint();
			}
			else if (type == ParameterDataType.Xml)
			{
				return new TypeConstraint();
			}

			return null;
		}
		#endregion

		#region CreateUpperCaseConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="UppercaseConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="UppercaseConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateUpperCaseConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new UppercaseConstraint();
			}

			return null;
		}
		#endregion

		#region CreateEnumValuesConstraint
		/// <summary>
		/// Creates a new instance of the <see cref="EnumValuesConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="EnumValuesConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateEnumValuesConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.Enum)
			{
				return new EnumValuesConstraint();
			}

			return null;
		}
		#endregion

		#region HandleUnknownConstraint
		/// <summary>
		/// Handles an unknown constraint, or when a constraint is used for a data type that is originally not supported.
		/// </summary>
		/// <param name="name">The name of the constraint.</param>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <param name="isConstraintNameUnknown">A value indicating if the constraint name is well-known.</param>
		/// <returns>A <see cref="Constraint"/>.</returns>
		private Constraint HandleUnknownConstraint(string name, ParameterDataType type, bool isConstraintNameUnknown)
		{
			Constraint ReturnValue = OnUnknownConstraint(name, type);

			if (ReturnValue == null)
			{
				if (isConstraintNameUnknown)
					// Constraint name is unknown
					throw new ConstraintParserException(Errors.CreateHResult(0x92), string.Format(Properties.Resources.ConstraintParser_HandleUnknownConstraint_UnknownName, name));
				else
					// Constraint is known, but not for specified data type
					throw new ConstraintParserException(Errors.CreateHResult(0x93), string.Format(Properties.Resources.ConstraintParser_HandleUnknownConstraint_NotDefined, name, type));
			}

			return ReturnValue;
		}
		#endregion

		#region HandleOutsideConstraint
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
					throw new ConstraintParserException(Errors.CreateHResult(0x8e), string.Format(Properties.Resources.ConstraintParser_Parse_InvalidContent, context.StringPosition), context.StringPosition);
			}

		}
		#endregion

		#region HandleInConstraint
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
					throw new ConstraintParserException(Errors.CreateHResult(0x8d), string.Format(Properties.Resources.ConstraintParser_Parse_InvalidCharInName, context.CurrentCharacter), context.StringPosition);
				case ']': // Constraint without parameters
					context.CurrentConstraintStart++;
					if (context.StringPosition == context.CurrentConstraintStart)
					{
						// No constraint name
						throw new ConstraintParserException(Errors.CreateHResult(0x8a), Properties.Resources.ConstraintParser_Parse_EmptyConstraint, context.StringPosition);
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
						throw new ConstraintParserException(Errors.CreateHResult(0x8a), Properties.Resources.ConstraintParser_Parse_EmptyConstraint, context.StringPosition);
					}
					context.CurrentConstraintName = context.GetSubstring(context.CurrentConstraintStart, context.StringPosition - context.CurrentConstraintStart);
					context.LogicalPosition = ConstraintPosition.InParameters;
					break;
			}
		}
		#endregion

		#region HandleInParameters
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
					throw new ConstraintParserException(Errors.CreateHResult(0x8f), string.Format(Properties.Resources.ConstraintParser_Parse_InvalidCharInParameters, context.CurrentCharacter), context.StringPosition);
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
		#endregion

		#region HandleInParameter
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
						throw new ConstraintParserException(Errors.CreateHResult(0x90), string.Format(Properties.Resources.ConstraintParser_Parse_UnmaskedChar, context.CurrentCharacter), context.StringPosition);
					default: // All other characters allowed in parameter
						context.AppendParameter();
						break;

				}
			}

		}
		#endregion

		#region HandleAfterParameter
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
					throw new ConstraintParserException(Errors.CreateHResult(0x91), string.Format(Properties.Resources.ConstraintParser_Parse_InvalidCharAfterParam, context.CurrentCharacter), context.StringPosition);
			}

		}
		#endregion

		#region HandleAfterParameters
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
					throw new ConstraintParserException(Errors.CreateHResult(0x9d), string.Format(Properties.Resources.ConstraintParser_Parse_InvalidCharAfterParams, context.CurrentCharacter), context.StringPosition);
			}

		}
		#endregion
		#endregion

		#region Private classes
		/// <summary>
		/// Represents the context data while parsing a constraint string.
		/// </summary>
		private class ParserContext
		{
			#region Private fields
			private int StringLength;
			private string ConstraintString;
			#endregion

			#region Public fields
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
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the ParserContext class with the specified constraint string.
			/// </summary>
			/// <param name="constraintString">The constraint string to parse.</param>
			/// <param name="dataType">The data type of the parameter.</param>
			internal ParserContext(string constraintString, ParameterDataType dataType)
			{
				ConstraintString = constraintString;
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
				StringLength = constraintString.Length;
			}
			#endregion

			#region Public methods
			/// <summary>
			/// Advances the current position in ConstraintString and sets CurrentCharacter.
			/// </summary>
			/// <returns>true, if the position was advanced to the next character; false, if the end of the string is reached.</returns>
			public bool MoveNext()
			{
				StringPosition++;
				if (StringPosition < StringLength)
				{
					CurrentCharacter = ConstraintString[StringPosition];
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
				if (i < StringLength)
				{
					return (ConstraintString[i] == '\'');
				}
				return false;
			}

			/// <summary>
			/// Gets a substring from the constraint string.
			/// </summary>
			/// <param name="startIndex">The start of the substring.</param>
			/// <param name="length">The length of the substring.</param>
			/// <returns>The substring to retrieve.</returns>
			public string GetSubstring(int startIndex, int length)
			{
				return ConstraintString.Substring(startIndex, length);
			}

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
				CurrentParameter.Clear();
				IsMasked = false;
			}

			/// <summary>
			/// Appends the current character to CurrentParameter.
			/// </summary>
			public void AppendParameter()
			{
				CurrentParameter.Append(CurrentCharacter);
			}

			/// <summary>
			/// Skips the next character.
			/// </summary>
			public void SkipOne()
			{
				StringPosition++;
			}
			#endregion
		}
		#endregion
	}
}
