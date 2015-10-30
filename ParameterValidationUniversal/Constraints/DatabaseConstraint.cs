#region Copyright
/*******************************************************************************
 * <copyright file="DatabaseConstraint.cs" owner="Daniel Kopp">
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
 * <file name="DatabaseConstraint.cs" date="2015-10-01">
 * A constraint specifying that the parameter value is a key of a
 * database entity.
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
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class DatabaseConstraint : Constraint
	{
		#region Private fields
		private string mEntity;
		private string mKeyProperty;
		private string mDisplayProperty;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the database entity (e.g. table, view, or domain model entity) that stores the value of the parameter.
		/// </summary>
		public string Entity
		{
			get { return mEntity; }
		}

		/// <summary>
		/// Gets the name of the property (e.g. field name) that contains the value of the parameter.
		/// </summary>
		public string KeyProperty
		{
			get { return mKeyProperty; }
		}

		/// <summary>
		/// Gets the name of the property (e.g. field name) that should be displayed instead of <see cref="KeyProperty"/>. May be empty.
		/// </summary>
		public string DisplayProperty
		{
			get { return mDisplayProperty; }
		}
		#endregion

		#region Constructors
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
			mEntity = entity ?? string.Empty;
			mKeyProperty = keyProperty ?? string.Empty;
			mDisplayProperty = displayProperty ?? string.Empty;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected DatabaseConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mEntity = info.GetString(nameof(Entity));
			mKeyProperty = info.GetString(nameof(KeyProperty));
			mDisplayProperty = info.GetString(nameof(DisplayProperty));
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
			info.AddValue(nameof(Entity), mEntity);
			info.AddValue(nameof(KeyProperty), mKeyProperty);
			info.AddValue(nameof(DisplayProperty), mDisplayProperty);
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
			parameters.Add(mEntity);
			parameters.Add(mKeyProperty);
			parameters.Add(mDisplayProperty);
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
			if (parameters.Count < 1 || parameters.Count > 3)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x14), string.Format(Properties.Resources.Global_SetParameters_InvalidCountVariable, this.Name, 2, 3), this);
			}

			mEntity = parameters[0];
			mKeyProperty = parameters[1];
			if (parameters.Count == 3)
			{
				mDisplayProperty = parameters[2];
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
	}
}
