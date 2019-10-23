using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(SnapAttribute))]
	class SnapDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUSDIT) invalid type for SnapAttribute on field {0}: Snap can only be applied to int, float, vector, rect, or bounds fields";
		private const string _missingCompareWarning = "(PUSDMC) invalid method, field, or property for SnapAttribute on field '{0}': '{1}' could not be found on type '{2}'";
		private const string _invalidMethodReturnWarning = "(PUSDIMR) invalid method for SnapAttribute on field '{0}': the method '{1}' should return a '{2}'";
		private const string _invalidMethodParametersWarning = "(PUSDIMP) invalid method for SnapAttribute on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidFieldReturnWarning = "(PUSDIFR) invalid field for SnapAttribute on field '{0}': the method '{1}' should return an {2}";
		private const string _invalidPropertyReturnWarning = "(PUSDIPR) invalid property for SnapAttribute on field '{0}': the method '{1}' should return an {2}";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var snapAttribute = attribute as SnapAttribute;
			var element = this.CreateNextElement(property);

			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
				{
					var snap = GetSnap(property, snapAttribute, Mathf.RoundToInt(snapAttribute.Number));

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Float:
				{
					var snap = GetSnap(property, snapAttribute, snapAttribute.Number);

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Vector2:
				{
					var snap = GetSnap(property, snapAttribute, (Vector2)snapAttribute.Vector);

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Vector2Int:
				{
					var snap = GetSnap(property, snapAttribute, Vector2Int.RoundToInt(snapAttribute.Vector));

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Vector3:
				{
					var snap = GetSnap(property, snapAttribute, (Vector3)snapAttribute.Vector);

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Vector3Int:
				{
					var snap = GetSnap(property, snapAttribute, Vector3Int.RoundToInt(snapAttribute.Vector));

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Vector4:
				{
					var snap = GetSnap(property, snapAttribute, snapAttribute.Vector);

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Rect:
				{
					var snap = GetSnap(property, snapAttribute, new Rect(snapAttribute.Vector.x, snapAttribute.Vector.y, snapAttribute.Vector.y, snapAttribute.Vector.w));

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.RectInt:
				{
					var snap = GetSnap(property, snapAttribute, new RectInt(Mathf.RoundToInt(snapAttribute.Vector.x), Mathf.RoundToInt(snapAttribute.Vector.y), Mathf.RoundToInt(snapAttribute.Vector.y), Mathf.RoundToInt(snapAttribute.Vector.w)));

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.Bounds:
				{
					var snap = GetSnap(property, snapAttribute, snapAttribute.Bounds);

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				case SerializedPropertyType.BoundsInt:
				{
					var snap = GetSnap(property, snapAttribute, new BoundsInt(Vector3Int.RoundToInt(snapAttribute.Bounds.center), Vector3Int.RoundToInt(snapAttribute.Bounds.extents)));

					Snap(property, snap());
					element.RegisterCallback<FocusOutEvent>(e => Snap(property, snap()));
					break;
				}
				default:
				{
					Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
					break;
				}
			}

			return element;
		}

		private void Snap(SerializedProperty property, int snap)
		{
			property.intValue = Snap(property.intValue, snap);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, float snap)
		{
			property.floatValue = Snap(property.floatValue, snap);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector2 snap)
		{
			var x = Snap(property.vector2Value.x, snap.x);
			var y = Snap(property.vector2Value.y, snap.y);

			property.vector2Value = new Vector2(x, y);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector2Int snap)
		{
			var x = Snap(property.vector2IntValue.x, snap.x);
			var y = Snap(property.vector2IntValue.y, snap.y);

			property.vector2IntValue = new Vector2Int(x, y);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector3 snap)
		{
			var x = Snap(property.vector3Value.x, snap.x);
			var y = Snap(property.vector3Value.y, snap.y);
			var z = Snap(property.vector3Value.z, snap.z);

			property.vector3Value = new Vector3(x, y, z);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector3Int snap)
		{
			var x = Snap(property.vector3IntValue.x, snap.x);
			var y = Snap(property.vector3IntValue.y, snap.y);
			var z = Snap(property.vector3IntValue.z, snap.z);

			property.vector3IntValue = new Vector3Int(x, y, z);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Vector4 snap)
		{
			var x = Snap(property.vector4Value.x, snap.x);
			var y = Snap(property.vector4Value.y, snap.y);
			var z = Snap(property.vector4Value.z, snap.z);
			var w = Snap(property.vector4Value.w, snap.w);

			property.vector4Value = new Vector4(x, y, z, w);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Rect snap)
		{
			var x = Snap(property.rectValue.x, snap.x);
			var y = Snap(property.rectValue.y, snap.y);
			var width = Snap(property.rectValue.width, snap.width);
			var height = Snap(property.rectValue.height, snap.height);

			property.rectValue = new Rect(x, y, width, height);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, RectInt snap)
		{
			var x = Snap(property.rectIntValue.x, snap.x);
			var y = Snap(property.rectIntValue.y, snap.y);
			var width = Snap(property.rectIntValue.width, snap.width);
			var height = Snap(property.rectIntValue.height, snap.height);

			property.rectIntValue = new RectInt(x, y, width, height);
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, Bounds snap)
		{
			var x = Snap(property.boundsValue.center.x, snap.center.x);
			var y = Snap(property.boundsValue.center.y, snap.center.y);
			var z = Snap(property.boundsValue.center.z, snap.center.z);
			var width = Snap(property.boundsValue.extents.x, snap.extents.x);
			var height = Snap(property.boundsValue.extents.y, snap.extents.y);
			var depth = Snap(property.boundsValue.extents.z, snap.extents.z);

			property.boundsValue = new Bounds(new Vector3(x, y, z), new Vector3(width, height, depth));
			property.serializedObject.ApplyModifiedProperties();
		}

		private void Snap(SerializedProperty property, BoundsInt snap)
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

		private Func<FieldType> GetSnap<FieldType>(SerializedProperty property, SnapAttribute snapAttribute, FieldType defaultValue)
		{
			if (!string.IsNullOrEmpty(snapAttribute.Snap))
			{
				var method = fieldInfo.DeclaringType.GetMethod(snapAttribute.Snap, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var field = fieldInfo.DeclaringType.GetField(snapAttribute.Snap, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var prop = fieldInfo.DeclaringType.GetProperty(snapAttribute.Snap, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					if (method.ReturnType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidMethodReturnWarning, property.propertyPath, snapAttribute.Snap, property.type);
					else if (!method.HasSignature(null))
						Debug.LogWarningFormat(_invalidMethodParametersWarning, property.propertyPath, snapAttribute.Snap);
					else
						return () => (FieldType)method.Invoke(method.IsStatic ? null : property.GetOwner<object>(), null);
				}
				else if (field != null)
				{
					if (field.FieldType != typeof(FieldType))
						Debug.LogWarningFormat(_invalidFieldReturnWarning, property.propertyPath, snapAttribute.Snap, property.type);
					else
						return () => (FieldType)field.GetValue(field.IsStatic ? null : property.GetOwner<object>());
				}
				else if (prop != null)
				{
					if (prop.PropertyType != typeof(FieldType) || !prop.CanRead)
						Debug.LogWarningFormat(_invalidPropertyReturnWarning, property.propertyPath, snapAttribute.Snap, property.type);
					else
						return () => (FieldType)prop.GetValue(prop.GetGetMethod().IsStatic ? null : property.GetOwner<object>());
				}
				else
				{
					Debug.LogWarningFormat(_missingCompareWarning, property.propertyPath, snapAttribute.Snap, fieldInfo.DeclaringType.Name);
				}
			}

			return () => defaultValue;
		}
	}
}