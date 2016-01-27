﻿#region Copyright
/*******************************************************************************
 * <copyright file="Gender.cs" owner="Daniel Kopp">
 * Copyright 2015-2016 Daniel Kopp
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
 * <assembly name="ExampleCsharpDesktop">
 * Example application for NerdyDuck.ParameterValidation library.
 * </assembly>
 * <file name="Gender.cs" date="2016-01-19">
 * Specifies the gender of a person.
 * </file>
 ******************************************************************************/
#endregion

using System;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// Specifies the gender of a person.
	/// </summary>
	[Serializable]
	public enum Gender
	{
		Female,
		Male,
		NotSpecified
	}
}
