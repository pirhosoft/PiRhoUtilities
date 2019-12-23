using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(DelayAttribute))]
	class DelayDrawer : PropertyDrawer
	{
		private const string _invalidDrawerWarning = "(PUDDID) invalid drawer for DelayedAttribute on field {0}: the element does not have a TextInputBaseField";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			if (property.propertyType == SerializedPropertyType.Integer ||
				property.propertyType == SerializedPropertyType.Float ||
				property.propertyType == SerializedPropertyType.String ||
				property.propertyType == SerializedPropertyType.Vector2 ||
				property.propertyType == SerializedPropertyType.Vector2Int ||
				property.propertyType == SerializedPropertyType.Vector3 ||
				property.propertyType == SerializedPropertyType.Vector3Int ||
				property.propertyType == SerializedPropertyType.Vector4 ||
				property.propertyType == SerializedPropertyType.Rect ||
				property.propertyType == SerializedPropertyType.RectInt ||
				property.propertyType == SerializedPropertyType.Bounds ||
				property.propertyType == SerializedPropertyType.BoundsInt ||
				property.propertyType == SerializedPropertyType.Quaternion ||
				property.propertyType == SerializedPropertyType.Character)
			{
				var inputs = element.Query(className: TextInputBaseField<string>.ussClassName);
				inputs.ForEach(field =>
				{
					if (field is TextInputBaseField<int> i)
						i.isDelayed = true;
					else if (field is TextInputBaseField<float> f)
						f.isDelayed = true;
					else if (field is TextInputBaseField<string> s)
						s.isDelayed = true;
					else if (field is TextInputBaseField<long> l)
						l.isDelayed = true;
					else if (field is TextInputBaseField<double> d)
						d.isDelayed = true;
				});
			}
			else
			{
				Debug.LogWarningFormat(_invalidDrawerWarning, property.propertyPath);
			}

			return element;
		}
	}
}