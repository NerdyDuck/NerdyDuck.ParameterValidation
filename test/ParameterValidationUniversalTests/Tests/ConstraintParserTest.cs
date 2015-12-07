#region Copyright
/*******************************************************************************
 * <copyright file="ConstraintParserTest.cs" owner="Daniel Kopp">
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
 * <assembly name="NerdyDuck.Tests.ParameterValidation">
 * Unit tests for NerdyDuck.ParameterValidation assembly.
 * </assembly>
 * <file name="ConstraintParserTest.cs" date="2015-11-19">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.ConstraintParser class.
 * </file>
 ******************************************************************************/
#endregion

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
#if WINDOWS_DESKTOP
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
#endif
using NerdyDuck.CodedExceptions;
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.ConstraintParser class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ConstraintParserTest
	{
		#region Public methods
		[TestMethod]
		public void ConcatConstraints_Success()
		{
			List<Constraint> Constraints = new List<Constraint>();
			Constraints.Add(new PasswordConstraint());
			Constraints.Add(new NullConstraint());
			Constraints.Add(new LengthConstraint(2));
			Constraints.Add(new EncryptedConstraint());

			Assert.AreEqual("[Null][Encrypted][Password][Length(2)]", ConstraintParser.ConcatConstraints(Constraints));
		}

		[TestMethod]
		public void ConcatConstraints_Null_Success()
		{
			Assert.IsNull(ConstraintParser.ConcatConstraints(null));
		}

		[TestMethod]
		public void Parse_TypeInv_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				ConstraintParser parser = new ConstraintParser();
				parser.Parse(Constants.SimpleConstraintString, ParameterDataType.None);
			});
		}

		[TestMethod]
		public void Parse_StringNull_Success()
		{
			ConstraintParser parser = new ConstraintParser();
			IReadOnlyList<Constraint> constraints = parser.Parse(null, ParameterDataType.Int32);
			Assert.AreEqual(0, constraints.Count);
		}

		[TestMethod]
		public void Parse_EmptyConstraint_Error()
		{
			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				ConstraintParser parser = new ConstraintParser();
				parser.Parse("[   ]", ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void Parse_ParametersError_Error()
		{
			ConstraintParser parser = CreateParserWithDummy();

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				parser.Parse("[Dummy(argex)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				parser.Parse("[Dummy(formex)]", ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void Parse_UnknownConstraint_Error()
		{
			ConstraintParser parser = new ConstraintParser();
			parser.UnknownConstraint += (sender, e) =>
			{
			};

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				parser.Parse("[YouThinkYouKnowMe]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				parser.Parse("[Path]", ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void Parse_ParamSplit_Success()
		{
			ConstraintParser parser = CreateParserWithDummy();
			IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy(abc,'def','g,hi', jkl,'m''no',pqr,'s tu')]", ParameterDataType.Int32);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(DummyConstraint));
			DummyConstraint c = constraints[0] as DummyConstraint;
			Assert.AreEqual(7, c.Parameters.Count);
			Assert.AreEqual("abc", c.Parameters[0]);
			Assert.AreEqual("def", c.Parameters[1]);
			Assert.AreEqual("g,hi", c.Parameters[2]);
			Assert.AreEqual("jkl", c.Parameters[3]);
			Assert.AreEqual("m'no", c.Parameters[4]);
			Assert.AreEqual("pqr", c.Parameters[5]);
			Assert.AreEqual("s tu", c.Parameters[6]);
		}

		[TestMethod]
		public void Parse_Whitespace_Success()
		{
			ConstraintParser parser = CreateParserWithDummy();
			IReadOnlyList<Constraint> constraints = parser.Parse(" [Dummy( 'abc' , def , 'ghi' )] [Null] ", ParameterDataType.Int32);
			Assert.AreEqual(2, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(DummyConstraint));
			DummyConstraint c = constraints[0] as DummyConstraint;
			Assert.AreEqual(3, c.Parameters.Count);
			Assert.AreEqual("abc", c.Parameters[0]);
			Assert.AreEqual("def", c.Parameters[1]);
			Assert.AreEqual("ghi", c.Parameters[2]);
		}

		[TestMethod]
		public void Parse_NoParams_Success()
		{
			ConstraintParser parser = CreateParserWithDummy();
			IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy()]", ParameterDataType.Int32);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(DummyConstraint));
			DummyConstraint c = constraints[0] as DummyConstraint;
			Assert.IsNull(c.Parameters);
		}

		[TestMethod]
		public void Parse_EmptyParam_Success()
		{
			ConstraintParser parser = CreateParserWithDummy();
			IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy('')]", ParameterDataType.Int32);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(DummyConstraint));
			DummyConstraint c = constraints[0] as DummyConstraint;
			Assert.AreEqual(1, c.Parameters.Count);
			Assert.AreEqual("", c.Parameters[0]);
		}

		[TestMethod]
		public void Parse_Incomplete_Error()
		{
			ConstraintParser parser = CreateParserWithDummy();

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy('", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy('a'", ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void Parse_InvalidChars_Error()
		{
			ConstraintParser parser = CreateParserWithDummy();

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("n[Dummy]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[()]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy(])]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy(a'b)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy(a b)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy('a 'b)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				IReadOnlyList<Constraint> constraints = parser.Parse("[Dummy(abc)d]", ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void Parse_AllConstraints_Success()
		{
			ConstraintParser parser = new ConstraintParser();
			IReadOnlyList<Constraint> constraints;

			constraints = parser.Parse("[AllowedScheme(http)]", ParameterDataType.Uri);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(AllowedSchemeConstraint));

			constraints = parser.Parse("[CharSet(Ascii)]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(CharacterSetConstraint));

			constraints = parser.Parse("[Database(MyTable,MyValue)]", ParameterDataType.Int32);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(DatabaseConstraint));

			constraints = parser.Parse("[DecimalPlaces(2)]", ParameterDataType.Decimal);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(DecimalPlacesConstraint));

			constraints = parser.Parse("[Encrypted]", ParameterDataType.Uri);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(EncryptedConstraint));

			constraints = parser.Parse("[Endpoint]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(EndpointConstraint));

			constraints = parser.Parse("[Values(Int32,MyValue=1)]", ParameterDataType.Enum);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(EnumValuesConstraint));

			constraints = parser.Parse("[FileName]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(FileNameConstraint));

			constraints = parser.Parse("[Host]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(HostNameConstraint));

			constraints = parser.Parse("[Length(5)]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(LengthConstraint));

			constraints = parser.Parse("[Lowercase]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(LowercaseConstraint));

			constraints = parser.Parse("[MaxLength(5)]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(MaximumLengthConstraint));

			constraints = parser.Parse("[MaxValue(42)]", ParameterDataType.Int32);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(MaximumValueConstraint));

			constraints = parser.Parse("[MinLength(5)]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(MinimumLengthConstraint));

			constraints = parser.Parse("[MinValue(42)]", ParameterDataType.Int32);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(MinimumValueConstraint));

			constraints = parser.Parse("[Null]", ParameterDataType.Uri);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(NullConstraint));

			constraints = parser.Parse("[Password]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(PasswordConstraint));

			constraints = parser.Parse("[Path]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(PathConstraint));

			constraints = parser.Parse("[Regex(.*)]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(RegexConstraint));

			constraints = parser.Parse("[Step(3)]", ParameterDataType.Int32);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(StepConstraint));

			constraints = parser.Parse(string.Format("[Type('{0}')]", typeof(XmlSerializableObject).AssemblyQualifiedName), ParameterDataType.Xml);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(TypeConstraint));

			constraints = parser.Parse(string.Format("[Type('{0}')]", typeof(DayOfWeek).AssemblyQualifiedName), ParameterDataType.Enum);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(EnumTypeConstraint));

			constraints = parser.Parse("[Uppercase]", ParameterDataType.String);
			Assert.AreEqual(1, constraints.Count);
			Assert.IsInstanceOfType(constraints[0], typeof(UppercaseConstraint));
		}

		[TestMethod]
		public void Parse_InvalidDataTypes_Error()
		{
			ConstraintParser parser = new ConstraintParser();
			IReadOnlyList<Constraint> constraints;

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[AllowedScheme(http)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[CharSet(Ascii)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[DecimalPlaces(2)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Endpoint]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Values(Int32,MyValue=1)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[FileName]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Host]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Length(5)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Lowercase]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[MaxLength(5)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[MaxValue(42)]", ParameterDataType.String);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[MinLength(5)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[MinValue(42)]", ParameterDataType.String);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Password]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Regex(.*)]", ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Step(3)]", ParameterDataType.String);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse(string.Format("[Type('{0}')]", typeof(XmlSerializableObject).AssemblyQualifiedName), ParameterDataType.Int32);
			});

			CustomAssert.ThrowsException<ConstraintParserException>(() =>
			{
				constraints = parser.Parse("[Uppercase]", ParameterDataType.Int32);
			});
		}
		#endregion

		#region Private methods
		private ConstraintParser CreateParserWithDummy()
		{
			ConstraintParser parser = new ConstraintParser();
			parser.UnknownConstraint += (sender, e) =>
			{
				if (e.ConstraintName == "Dummy")
				{
					e.Constraint = new DummyConstraint();
				}
			};

			return parser;
		}
		#endregion
	}
}
