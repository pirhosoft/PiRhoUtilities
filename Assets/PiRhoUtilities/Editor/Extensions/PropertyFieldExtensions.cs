using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class PropertyFieldExtensions
	{
		public static void SetLabel(this PropertyField field, string label)
		{
			// This must be scheduled because the field won't be created until the next frame after the Property Field has been set up
			field.schedule.Execute(() =>
			{
				var baseField = field.Q(className: BaseFieldExtensions.UssClassName);
				BaseFieldExtensions.SetLabel(baseField, label);
			}).StartingIn(0);
		}
	}
}