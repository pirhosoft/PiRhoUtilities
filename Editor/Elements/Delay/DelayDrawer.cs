using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(DelayAttribute))]
	class DelayDrawer : PropertyDrawer
	{
		private const string _invalidDrawerWarning = "(PUMDID) invalid drawer for DelayedAttribute on field {0}: the element does not have a TextInputBaseField";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var input = element.Query(className: TextInputBaseField<string>.ussClassName).First();

			if (input is TextInputBaseField<string> s)
				s.isDelayed = true;
			else if (input is TextInputBaseField<int> i)
				i.isDelayed = true;
			else if (input is TextInputBaseField<long> l)
				l.isDelayed = true;
			else if (input is TextInputBaseField<float> f)
				f.isDelayed = true;
			else if (input is TextInputBaseField<double> d)
				d.isDelayed = true;
			else
				Debug.LogWarningFormat(_invalidDrawerWarning, property.propertyPath);

			return element;
		}
	}
}