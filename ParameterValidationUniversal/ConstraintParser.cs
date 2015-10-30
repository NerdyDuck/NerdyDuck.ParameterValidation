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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Parses a string containing one or more constraints.
	/// </summary>
	public class ConstraintParser
	{
		#region Events
		/// <summary>
		/// The event that is raised when an unknown is encountered during parsing.
		/// </summary>
		public event EventHandler<UnknownConstraintEventArgs> UnknownConstraint;
		#endregion

		#region Private fields
		private static readonly Lazy<ConstraintParser> mParser = new Lazy<ConstraintParser>(() =>
		{
			return new ConstraintParser();
		});
		#endregion

		#region Properties
		/// <summary>
		/// Gets a global instance of the <see cref="ConstraintParser"/>.
		/// </summary>
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
		/// <exception cref="CodedArgumentException"><paramref name="type"/> is <see cref="ParameterDataType.None"/>.</exception>
		public IReadOnlyList<Constraint> Parse(string constraints, ParameterDataType type)
		{
			if (type == ParameterDataType.None)
			{
				throw new CodedArgumentException(Errors.CreateHResult(0x8c), Properties.Resources.Global_ParameterDataType_None);
			}

			List<Constraint> ReturnValue = new List<Constraint>();

			if (string.IsNullOrWhiteSpace(constraints))
				return ReturnValue;

			List<string> ConstraintTokens = SplitConstraints(constraints);

			string ConstraintString, ConstraintName, ConstraintParameterString;
			Constraint CurrentConstraint;
			foreach (string constraint in ConstraintTokens)
			{
				ConstraintString = constraint.Trim();
				if (string.IsNullOrWhiteSpace(ConstraintString))
				{
					throw new ConstraintParserException(Errors.CreateHResult(0x8a), Properties.Resources.ConstraintParser_Parse_EmptyConstraint);
				}

				SplitConstraint(ConstraintString, out ConstraintName, out ConstraintParameterString);

				CurrentConstraint = FindConstraint(ConstraintName, type);

				if (!string.IsNullOrEmpty(ConstraintParameterString))
				{
					try
					{
						CurrentConstraint.SetParametersInternal(SplitConstraintParameters(ConstraintParameterString), type);
					}
					catch (ArgumentException ex)
					{
						throw new ConstraintParserException(Errors.CreateHResult(0x8b), string.Format(Properties.Resources.ConstraintParser_Parse_ParametersInvalid, CurrentConstraint.Name), ex);
					}
					catch (FormatException ex)
					{
						throw new ConstraintParserException(Errors.CreateHResult(0x8b), string.Format(Properties.Resources.ConstraintParser_Parse_ParametersInvalid, CurrentConstraint.Name), ex);
					}
				}

				ReturnValue.Add(CurrentConstraint);
			}

			return ReturnValue;
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
		/// <returns></returns>
		protected virtual Constraint OnUnknownConstraint(string constraintName, ParameterDataType dataType)
		{
			UnknownConstraintEventArgs e = new UnknownConstraintEventArgs(constraintName, dataType);

			EventHandler<UnknownConstraintEventArgs> ucHandler = UnknownConstraint;
			if (ucHandler != null)
			{
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
		#region SplitConstraints
		/// <summary>
		/// Splits a string containing one or more constraints.
		/// </summary>
		/// <param name="constraints">A string of constraints.</param>
		/// <returns>A list of constraints including parameters, without [ ].</returns>
		private static List<string> SplitConstraints(string constraints)
		{
			List<string> ConstraintTokens = new List<string>();
			int StartPos = -1;
			bool IsMasked = false;

			for (int i = 0; i < constraints.Length; i++)
			{
				switch (constraints[i])
				{
					case '[':
						if (!IsMasked)
						{
							if (StartPos != -1)
							{
								throw new ConstraintParserException(Errors.CreateHResult(0x8d), string.Format(Properties.Resources.ConstraintParser_SplitConstraints_InvalidLBracket, i));
							}
							if (i + 1 >= constraints.Length)
							{
								throw new ConstraintParserException(Errors.CreateHResult(0x8e), string.Format(Properties.Resources.ConstraintParser_SplitConstraints_InvalidContent, i));
							}
							StartPos = i;
						}
						break;
					case ']':
						if (!IsMasked)
						{
							if (StartPos == -1)
							{
								throw new ConstraintParserException(Errors.CreateHResult(0x8f), string.Format(Properties.Resources.ConstraintParser_SplitConstraints_InvalidRBracket, i));
							}
							ConstraintTokens.Add(constraints.Substring(StartPos + 1, i - StartPos - 1));
							StartPos = -1;
						}
						break;
					case '\'':
						if (IsMasked)
						{
							if (i + 1 < constraints.Length)
							{
								if (constraints[i + 1] == '\'')
								{
									i++;
								}
								else
								{
									IsMasked = false;
								}
							}
						}
						else
						{
							IsMasked = true;
						}
						break;
				}
			}
			return ConstraintTokens;
		}
		#endregion

		#region SplitConstraint
		/// <summary>
		/// Splits a constraint into its name and its parameters.
		/// </summary>
		/// <param name="constraint">The constraint string to split.</param>
		/// <param name="constraintName">When the method returns, the contains the name of the constraint.</param>
		/// <param name="constraintParameters">When the method returns, contains the parametrs of the constraint as a single string, or an empty string, if the constraint has no parameters.</param>
		private static void SplitConstraint(string constraint, out string constraintName, out string constraintParameters)
		{
			constraintParameters = string.Empty;
			int StartPos = constraint.IndexOf('(');
			if (StartPos > -1)
			{
				if (constraint.LastIndexOf(')') != constraint.Length - 1)
				{
					throw new ConstraintParserException(Errors.CreateHResult(0x90), string.Format(Properties.Resources.ConstraintParser_SplitConstraint_MissingRBracket, constraint));
				}
				constraintName = constraint.Substring(0, StartPos).Trim();
				if (string.IsNullOrWhiteSpace(constraintName))
				{
					throw new ConstraintParserException(Errors.CreateHResult(0x91), string.Format(Properties.Resources.ConstraintParser_SplitConstraint_NoName, constraint));
				}
				constraintParameters = constraint.Substring(StartPos + 1, constraint.Length - StartPos - 2).Trim();
			}
			else
			{
				constraintName = constraint;
			}
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
				case Constraint.HostnameConstraintName:
					ReturnValue = CreateHostnameConstraint(type);
					break;
				case Constraint.LengthConstraintName:
					ReturnValue = CreateLengthConstraint(type);
					break;
				case Constraint.LowerCaseConstraintName:
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
				case Constraint.UpperCaseConstraintName:
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
		/// Creates a new instance of the <see cref="HostnameConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="HostnameConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateHostnameConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new HostnameConstraint();
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
		/// Creates a new instance of the <see cref="LowerCaseConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="LowerCaseConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateLowerCaseConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new LowerCaseConstraint();
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
		/// Creates a new instance of the <see cref="UpperCaseConstraint"/>, if the data type matches
		/// </summary>
		/// <param name="type">The data type to create the constraint for.</param>
		/// <returns>A <see cref="UpperCaseConstraint"/>, or <see langword="null"/>, if <paramref name="type"/> is not appropriate for the constraint.</returns>
		private static Constraint CreateUpperCaseConstraint(ParameterDataType type)
		{
			if (type == ParameterDataType.String)
			{
				return new UpperCaseConstraint();
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

		#region SplitConstraintParameters
		/// <summary>
		/// Splits a string containing one or more constraint parameters.
		/// </summary>
		/// <param name="parameters">The string to split.</param>
		/// <returns>An array of constraint parameters.</returns>
		private static string[] SplitConstraintParameters(string parameters)
		{
			List<string> ReturnValues = new List<string>();
			int StartPos = 0;
			bool IsMasked = false;

			for (int i = 0; i < parameters.Length; i++)
			{
				switch (parameters[i])
				{
					case ',':
						if (!IsMasked)
						{
							ReturnValues.Add(parameters.Substring(StartPos, i - StartPos).Trim());
							StartPos = i + 1;
						}
						break;
					case '\'':
						if (IsMasked)
						{
							if (i + 1 < parameters.Length)
							{
								if (parameters[i + 1] == '\'')
								{
									i++;
								}
								else
								{
									IsMasked = false;
								}
							}
						}
						else
						{
							IsMasked = true;
						}
						break;
				}
			}
			if (StartPos == 0) // Only one parameter
			{
				ReturnValues.Add(parameters.Trim());
			}

			// Unquote and unescape
			for (int i = 0; i < ReturnValues.Count; i++)
			{
				string Temp = ReturnValues[i];
				if (Temp.StartsWith("'") && Temp.Length > 1)
				{
					Temp = Temp.Substring(1, Temp.Length - 2).Replace("''", "'");
					ReturnValues[i] = Temp;
				}
			}

			return ReturnValues.ToArray();
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
		#endregion
	}
}
