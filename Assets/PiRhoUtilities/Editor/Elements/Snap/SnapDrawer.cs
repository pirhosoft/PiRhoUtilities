using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(SnapAttribute))]
	class SnapDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUSNDIT) invalid type for SnapAttribute on field {0}: Snap can only be applied to int, float, vector, rect, or bounds fields";
		private const string _invalidSourceError = "(PUSNDIS) invalid source for SnapAttribute on field '{0}': a field, method, or property of type '{1}' named '{2}' could not be found";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var snapAttribute = attribute as SnapAttribute;
			var element = this.CreateNextElement(property);

			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer: SetupSnap(snapAttribute.SnapSource, property, element, Mathf.RoundToInt(snapAttribute.Number), SnapInt); break;
				case SerializedPropertyType.Float: SetupSnap(snapAttribute.SnapSource, property, element, snapAttribute.Number, SnapFloat); break;
				case SerializedPropertyType.Vector2: SetupSnap(snapAttribute.SnapSource, property, element, (Vector2)snapAttribute.Vector, SnapVector2); break;
				case SerializedPropertyType.Vector2Int: SetupSnap(snapAttribute.SnapSource, property, element, Vector2Int.RoundToInt(snapAttribute.Vector), SnapVector2Int); break;
				case SerializedPropertyType.Vector3: SetupSnap(snapAttribute.SnapSource, property, element, (Vector3)snapAttribute.Vector, SnapVector3); break;
				case SerializedPropertyType.Vector3Int: SetupSnap(snapAttribute.SnapSource, property, element, Vector3Int.RoundToInt(snapAttribute.Vector), SnapVector3Int); break;
				case SerializedPropertyType.Vector4: SetupSnap(snapAttribute.SnapSource, property, element, snapAttribute.Vector, SnapVector4); break;
				case SerializedPropertyType.Rect: SetupSnap(snapAttribute.SnapSource, property, element, new Rect(snapAttribute.Vector.x, snapAttribute.Vector.y, snapAttribute.Vector.y, snapAttribute.Vector.w), SnapRect); break;
				case SerializedPropertyType.RectInt: SetupSnap(snapAttribute.SnapSource, property, element, new RectInt(Mathf.RoundToInt(snapAttribute.Vector.x), Mathf.RoundToInt(snapAttribute.Vector.y), Mathf.RoundToInt(snapAttribute.Vector.y), Mathf.RoundToInt(snapAttribute.Vector.w)), SnapRectInt); break;
				case SerializedPropertyType.Bounds: SetupSnap(snapAttribute.SnapSource, property, element, snapAttribute.Bounds, SnapBounds); break;
				case SerializedPropertyType.BoundsInt: SetupSnap(snapAttribute.SnapSource, property, element, new BoundsInt(Vector3Int.RoundToInt(snapAttribute.Bounds.center), Vector3Int.RoundToInt(snapAttribute.Bounds.extents)), SnapBoundsInt); break;
				default:
				{
					Debug.LogWarningFormat(property.serializedObject.targetObject, _invalidTypeWarning, property.propertyPath);
					break;
				}
			}

			return element;
		}

		private void SetupSnap<T>(string sourceName, SerializedProperty property, VisualElement element, T defaultValue, Action<SerializedProperty, T> clamp)
		{
			var snap = ReflectionHelper.CreateValueSourceFunction(sourceName, property, element, fieldInfo.DeclaringType, defaultValue);

			if (snap != null)
			{
				clamp(property, snap());
				element.RegisterCallback<FocusOutEvent>(e => clamp(property, snap()));
			}
			else
			{
				Debug.LogWarningFormat(_invalidSourceError, property.propertyPath, nameof(T), sourceName);
			}
		}

		private void SnapInt(SerializedProperty property, int snap)
		{
			property.intValue = Snap(property.intValue, snap);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapFloat(SerializedProperty property, float snap)
		{
			property.floatValue = Snap(property.floatValue, snap);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapVector2(SerializedProperty property, Vector2 snap)
		{
			var x = Snap(property.vector2Value.x, snap.x);
			var y = Snap(property.vector2Value.y, snap.y);

			property.vector2Value = new Vector2(x, y);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapVector2Int(SerializedProperty property, Vector2Int snap)
		{
			var x = Snap(property.vector2IntValue.x, snap.x);
			var y = Snap(property.vector2IntValue.y, snap.y);

			property.vector2IntValue = new Vector2Int(x, y);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapVector3(SerializedProperty property, Vector3 snap)
		{
			var x = Snap(property.vector3Value.x, snap.x);
			var y = Snap(property.vector3Value.y, snap.y);
			var z = Snap(property.vector3Value.z, snap.z);

			property.vector3Value = new Vector3(x, y, z);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapVector3Int(SerializedProperty property, Vector3Int snap)
		{
			var x = Snap(property.vector3IntValue.x, snap.x);
			var y = Snap(property.vector3IntValue.y, snap.y);
			var z = Snap(property.vector3IntValue.z, snap.z);

			property.vector3IntValue = new Vector3Int(x, y, z);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapVector4(SerializedProperty property, Vector4 snap)
		{
			var x = Snap(property.vector4Value.x, snap.x);
			var y = Snap(property.vector4Value.y, snap.y);
			var z = Snap(property.vector4Value.z, snap.z);
			var w = Snap(property.vector4Value.w, snap.w);

			property.vector4Value = new Vector4(x, y, z, w);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapRect(SerializedProperty property, Rect snap)
		{
			var x = Snap(property.rectValue.x, snap.x);
			var y = Snap(property.rectValue.y, snap.y);
			var width = Snap(property.rectValue.width, snap.width);
			var height = Snap(property.rectValue.height, snap.height);

			property.rectValue = new Rect(x, y, width, height);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapRectInt(SerializedProperty property, RectInt snap)
		{
			var x = Snap(property.rectIntValue.x, snap.x);
			var y = Snap(property.rectIntValue.y, snap.y);
			var width = Snap(property.rectIntValue.width, snap.width);
			var height = Snap(property.rectIntValue.height, snap.height);

			property.rectIntValue = new RectInt(x, y, width, height);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapBounds(SerializedProperty property, Bounds snap)
		{
			var x = Snap(property.boundsValue.center.x, snap.center.x);
			var y = Snap(property.boundsValue.center.y, snap.center.y);
			var z = Snap(property.boundsValue.center.z, snap.center.z);
			var width = Snap(property.boundsValue.extents.x, snap.extents.x);
			var height = Snap(property.boundsValue.extents.y, snap.extents.y);
			var depth = Snap(property.boundsValue.extents.z, snap.extents.z);

			// Multiply by 2 because the editor displays extents which is half of size
			property.boundsValue = new Bounds(new Vector3(x, y, z), new Vector3(width * 2, height * 2, depth * 2));
			property.serializedObject.ApplyModifiedProperties();
		}

		private void SnapBoundsInt(SerializedProperty property, BoundsInt snap)
		{
			var x = Snap(property.boundsIntValue.x, snap.x);
			var y = Snap(property.boundsIntValue.y, snap.y);
			var z = Snap(property.boundsIntValue.z, snap.z);
			var width = Snap(property.boundsIntValue.x, snap.size.x);
			var height = Snap(property.boundsIntValue.y, snap.size.y);
			var depth = Snap(property.boundsIntValue.z, snap.size.z);

			property.boundsIntValue = new BoundsInt(new Vector3Int(x, y, z), new Vector3Int(width, height, depth));
			property.serializedObject.ApplyModifiedProperties();
		}

		private int Snap(int value, int snap)
		{
			return snap > 0 ? Mathf.RoundToInt(value / (float)snap) * snap : value;
		}

		private float Snap(float value, float snap)
		{
			return snap > 0.0f ? Mathf.Round(value / snap) * snap : value;
		}
	}
}
