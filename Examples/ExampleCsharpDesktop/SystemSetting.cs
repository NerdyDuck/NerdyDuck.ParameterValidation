using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NerdyDuck.ParameterValidation;

namespace ExampleCsharpDesktop
{
	[XmlRoot("Setting")]
	public class SystemSetting
	{
		private IReadOnlyList<Constraint> mConstraints = null;
		private bool AreConstraintsParsed = false;
		private string mConstraintString = null;
		private string mSerializedValue;
		private object mValue = null;
		private bool IsValueParsed = false;

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string DisplayName { get; set; }

		[XmlAttribute]
		public ParameterDataType DataType { get; set; }

		[XmlAttribute("Constraints")]
		public string ConstraintString
		{
			get { return mConstraintString; }
			set
			{
				mConstraintString = value;
				AreConstraintsParsed = false;
			}
		}

		[XmlIgnore]
		public IReadOnlyList<Constraint> Constraints
		{
			get
			{
				if (!AreConstraintsParsed)
				{
					mConstraints = ConstraintParser.Parser.Parse(ConstraintString, DataType);
					AreConstraintsParsed = true;
				}
				return mConstraints;
			}
		}

		public string SerializedValue
		{
			get { return mSerializedValue; }
			set
			{
				mSerializedValue = value;
				IsValueParsed = false;
			}
		}

		public object Value
		{
			get
			{
				if (!IsValueParsed)
				{
					mValue = ParameterConvert.ToDataType(mSerializedValue, DataType, Constraints);
				}
				return mValue;
			}
			set
			{
				mValue = value;

			}
		}
	}
}
