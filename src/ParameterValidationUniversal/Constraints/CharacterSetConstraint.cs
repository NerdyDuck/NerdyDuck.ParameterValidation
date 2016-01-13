#region Copyright
/*******************************************************************************
 * <copyright file="CharacterSetConstraint.cs" owner="Daniel Kopp">
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
 * <file name="CharacterSetConstraint.cs" date="2015-10-01">
 * A constraint specifying the allowed characters set for a string.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// A constraint specifying the allowed characters set for a string.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[CharSet(charSet)]</c>.</item>
	/// <item>The constraint is only applicable to the <see cref="ParameterDataType.String"/> data type.</item>
	/// <item>If a string parameter with that constraint uses characters not defined in the character set, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// <item>Valid character set names are defined in the <see cref="CharSet"/> enumeration.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class CharacterSetConstraint : Constraint
	{
		#region Constants
		// Extra characters defined in Windows-1252, but not defined in ISO-8859-1.
		private static readonly int[] Windows1252Extras = new int[]
		{   0x20ac, 0x20a1, 0x0192, 0x20e1, 0x2026, 0x2020, 0x2021, 0x02c6, 0x2030, 0x0160, 0x2039, 0x0152, 0x017d,
			0x2018, 0x2019, 0x201c, 0x201d, 0x2022, 0x2013, 0x2014, 0x02dc, 0x2122, 0x0161, 0x203a, 0x0153, 0x017e, 0x0178
		};
		#endregion

		#region Enumerations
		/// <summary>
		/// Specifies the character set to use in a <see cref="CharacterSetConstraint"/>.
		/// </summary>
		public enum CharSet
		{
			/// <summary>
			/// The 7-bit ASCII character set (ISO 646).
			/// </summary>
			Ascii = 0,

			/// <summary>
			/// The ISO 8859-1 (Unicode section Latin-1) character set.
			/// </summary>
			Iso8859 = 1,

			/// <summary>
			/// The Windows-1252 (CP-1252) character set, a superset of ISO 8859-1
			/// </summary>
			Windows1252 = 2

#if ODETTE
			,
			/// <summary>
			/// The subset of ISO 646-IRV (ASCII) used for strings in OFTP, e.g. virtual file names.
			/// </summary>
			Iso646Odette = 3
#endif
		}
		#endregion

		#region Private fields
		private CharSet mCharacterSet;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the character set to use as a constraint.
		/// </summary>
		/// <value>One of the <see cref="CharSet"/> values.</value>
		public CharSet CharacterSet
		{
			get { return mCharacterSet; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CharacterSetConstraint"/> class.
		/// </summary>
		public CharacterSetConstraint()
			: this(CharSet.Windows1252)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CharacterSetConstraint"/> class with the specified <see cref="Uri"/> scheme.
		/// </summary>
		/// <param name="characterSet">The character set to use as a constraint.</param>
		public CharacterSetConstraint(CharSet characterSet)
			: base(CharacterSetConstraintName)
		{
			mCharacterSet = characterSet;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="CharacterSetConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected CharacterSetConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mCharacterSet = (CharSet)info.GetValue(nameof(CharacterSet), typeof(CharSet));
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
			info.AddValue(nameof(CharacterSet), mCharacterSet);
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
			parameters.Add(Enum.GetName(typeof(CharSet), mCharacterSet));
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
			AssertDataType(dataType, ParameterDataType.String);
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x12), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			if (!Enum.TryParse<CharSet>(parameters[0], out mCharacterSet))
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x13), string.Format(Properties.Resources.CharacterSetConstraint_SetParameters_InvalidValue, parameters[0]), this);
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
			AssertDataType(dataType, ParameterDataType.String);

			if (!CheckString((string)value))
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x13), string.Format(Properties.Resources.CharacterSetConstraint_Validate_Failed, displayName, GetCharSetName()), memberName, this));
			}

		}
		#endregion
		#endregion

		#region Private methods
		/// <summary>
		/// Checks a string against the current character set.
		/// </summary>
		/// <param name="value">The string to check.</param>
		/// <returns><see langword="true"/> if all characters of <paramref name="value"/> are part of <see cref="CharacterSet"/>; otherwise, <see langword="false"/>.</returns>
		private bool CheckString(string value)
		{
			foreach (int c in value)
			{
				if (mCharacterSet == CharSet.Ascii)
				{
					if (!IsAsciiCharacter(c))
					{
						return false;
					}
				}
#if ODETTE
				else if (mCharacterSet == CharSet.Iso646Odette)
				{
					if (!IsIso646Character(c))
					{
						return false;
					}
				}
#endif
				else // Windows-1252 & ISO-8859-1
				{
					if (!IsIso8859Character(c))
					{
						if (mCharacterSet == CharSet.Windows1252)
						{
							if (!Windows1252Extras.Contains(c))
							{
								return false;
							}
						}
						else
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Checks if the character is a valid ASCII character.
		/// </summary>
		/// <param name="c">The numeric value of the character.</param>
		/// <returns>true, if the character is an ASCII character; otherwise, false.</returns>
		private static bool IsAsciiCharacter(int c)
		{
			if (c > 0x1f && c < 0x7f)
				return true;
			if (c == 0x09 || c == 0x0a || c == 0x0d)
				return true;

			return false;
		}
#if ODETTE

		/// <summary>
		/// Checks if the character is a valid ISO 646 (OFTP subset) character.
		/// </summary>
		/// <param name="c">The numeric value of the character.</param>
		/// <returns>true, if the character is an ISO 646 character; otherwise, false.</returns>
		private static bool IsIso646Character(int c)
		{
			if (c > 0x40 && c < 0x5b)
				return true;
			if (c > 0x2c && c < 0x3a)
				return true;
			if (c == 0x20 || c == 0x26 || c == 0x28 || c == 0x29 || c == 0x5f)
				return true;

			return false;
		}
#endif

		/// <summary>
		/// Checks if the character is a valid ISO 8859-1 character.
		/// </summary>
		/// <param name="c">The numeric value of the character.</param>
		/// <returns>true, if the character is an ISO 8859-1 character; otherwise, false.</returns>
		private static bool IsIso8859Character(int c)
		{
			if (c > 0x1f && c < 0x7f)
				return true;
			if (c > 0x9f && c < 0x100)
				return true;
			if (c == 0x09 || c == 0x0a || c == 0x0d)
				return true;

			return false;
		}
		/// <summary>
		/// Gets the display name of the current character set.
		/// </summary>
		/// <returns>A string.</returns>
		private string GetCharSetName()
		{
			return Properties.Resources.GetResource("CharacterSetConstraint_CharSet_" + Enum.GetName(typeof(CharSet), mCharacterSet));
		}
		#endregion
	}
}
