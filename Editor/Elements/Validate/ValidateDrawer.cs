using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ValidateAttribute))]
	class ValidateDrawer : PropertyDrawer
	{
		public const string Stylesheet = "ValidateStyle.uss";
		public const string UssClassName = "pirho-validate";
		public const string SideUssClassName = UssClassName + "--side";
		public const string MessageUssClassName = UssClassName + "__message";
		public const string AboveUssClassName = MessageUssClassName + "--above";
		public const string BelowUssClassName = MessageUssClassName + "--below";
		public const string LeftUssClassName = MessageUssClassName + "--left";
		public const string RightUssClassName = MessageUssClassName + "--right";

		private const string _invalidTypeWarning = "(PUVDIT) invalid type for ValidateAttribute on field '{0}': Validate can only be applied to serializable fields";
		private const string _invalidMethodWarning = "(PUVDIM) invalid method for ValidateAttribute on field '{0}': the method '{1}' should return a bool and take 0, or 1 parameter of type '{2}'";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);
			var validateAttribute = attribute as ValidateAttribute;

			var message = new MessageBox(validateAttribute.Type, validateAttribute.Message);
			message.AddToClassList(MessageUssClassName);

			var change = CreateControl(message, property, fieldInfo.DeclaringType, validateAttribute.Method);
			if (change != null)
			{
				var container = new VisualElement();
				container.AddToClassList(UssClassName);
				container.AddStyleSheet(Stylesheet);
				container.Add(element);
				container.Add(change);

				if (validateAttribute.Location == TraitLocation.Above)
				{
					message.AddToClassList(BelowUssClassName);
					container.Insert(0, message);
				}
				if (validateAttribute.Location == TraitLocation.Below)
				{
					message.AddToClassList(BelowUssClassName);
					container.Add(message);
				}
				else if (validateAttribute.Location == TraitLocation.Left)
				{
					message.AddToClassList(LeftUssClassName);
					container.Insert(0, message);
					container.AddToClassList(SideUssClassName);
				}
				else if (validateAttribute.Location == TraitLocation.Right)
				{
					message.AddToClassList(RightUssClassName);
					container.Add(message);
					container.AddToClassList(SideUssClassName);
				}

				return container;
			}

			return element;
		}
		
		private PropertyWatcher CreateControl(MessageBox message, SerializedProperty property, Type declaringType, string method)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer: return CreateControl<int>(message, property, declaringType, method);
				case SerializedPropertyType.Boolean: return CreateControl<bool>(message, property, declaringType, method);
				case SerializedPropertyType.Float: return CreateControl<float>(message, property, declaringType, method);
				case SerializedPropertyType.String: return CreateControl<string>(message, property, declaringType, method);
				case SerializedPropertyType.Color: return CreateControl<Color>(message, property, declaringType, method);
				case SerializedPropertyType.ObjectReference: return CreateControl<Object>(message, property, declaringType, method);
				case SerializedPropertyType.LayerMask: return CreateControl<int>(message, property, declaringType, method);
				case SerializedPropertyType.Enum: return CreateControl<Enum>(message, property, declaringType, method);
				case SerializedPropertyType.Vector2: return CreateControl<Vector2>(message, property, declaringType, method);
				case SerializedPropertyType.Vector2Int: return CreateControl<Vector2Int>(message, property, declaringType, method);
				case SerializedPropertyType.Vector3: return CreateControl<Vector3>(message, property, declaringType, method);
				case SerializedPropertyType.Vector3Int: return CreateControl<Vector3Int>(message, property, declaringType, method);
				case SerializedPropertyType.Vector4: return CreateControl<Vector4>(message, property, declaringType, method);
				case SerializedPropertyType.Rect: return CreateControl<Rect>(message, property, declaringType, method);
				case SerializedPropertyType.RectInt: return CreateControl<RectInt>(message, property, declaringType, method);
				case SerializedPropertyType.Bounds: return CreateControl<Bounds>(message, property, declaringType, method);
				case SerializedPropertyType.BoundsInt: return CreateControl<BoundsInt>(message, property, declaringType, method);
				case SerializedPropertyType.Character: return CreateControl<char>(message, property, declaringType, method);
				case SerializedPropertyType.AnimationCurve: return CreateControl<AnimationCurve>(message, property, declaringType, method);
				case SerializedPropertyType.Gradient: return CreateControl<Gradient>(message, property, declaringType, method);
				case SerializedPropertyType.Quaternion: return CreateControl<Quaternion>(message, property, declaringType, method);
				case SerializedPropertyType.ExposedReference: return CreateControl<Object>(message, property, declaringType, method);
				case SerializedPropertyType.FixedBufferSize: return CreateControl<int>(message, property, declaringType, method);
				case SerializedPropertyType.ManagedReference: return CreateControl<object>(message, property, declaringType, method);
			}

			Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath, this.GetFieldType().Name);
			return null;
		}

		private PropertyWatcher CreateControl<T>(MessageBox message, SerializedProperty property, Type declaringType, string method)
		{
			var none = ReflectionHelper.CreateFunctionCallback<bool>(method, declaringType, property);
			if (none != null)
			{
				Validated(message, none());
				return new ChangeTrigger<T>(property, (_, oldValue, newValue) => Validated(message, none()));
			}
			else
			{
				var one = ReflectionHelper.CreateFunctionCallback<T, bool>(method, declaringType, property);
				if (one != null)
				{
					var change = new ChangeTrigger<T>(property, (_, oldValue, newValue) => Validated(message, one(newValue)));
					Validated(message, one(change.value));
					return change;
				}
			}

			Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, method, typeof(T).Name);
			return null;
		}

		private void Validated(MessageBox message, bool valid)
		{
			if (!EditorApplication.isPlaying)
				message.SetDisplayed(!valid);
		}
	}
}