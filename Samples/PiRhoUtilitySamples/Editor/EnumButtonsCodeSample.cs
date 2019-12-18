using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class EnumButtonsCodeSample : CodeSample
	{
		// These enum values are written as flags so they will work properly when Use Flags is true. This is not
		// necessary when Use Flags is false. The [Flags] attribute itself isn't required - it's only purpose is to
		// make an enum value with multiple flags selected print as a comma separated list rather than a number.

		[Flags]
		public enum Games
		{
			None = 0,
			AdventRising = 1 << 0,
			TheOuterWilds = 1 << 1,
			BeyondGoodAndEvil = 1 << 2,
			EternalDarkness = 1 << 3,
			OcarinaOfTime = 1 << 4,
			HorizonZeroDawn = 1 << 5,
			All = ~0
		}

		[Flags]
		public enum Books
		{
			None = 0,
			ThePillarsOfTheEarth = 1 << 0,
			HardBoiledWonderland = 1 << 1,
			RedRising = 1 << 2,
			ThePhantomTollboot = 1 << 3,
			EndersGame = 1 << 4,
			HarryPotter = 1 << 5,
			All = ~0
		}

		[Flags]
		public enum Movies
		{
			None = 0,
			TheMatrix = 1 << 0,
			TheDarkKnight = 1 << 1,
			All = ~0
		}

		private List<Type> _types = new List<Type> { typeof(Games), typeof(Books), typeof(Movies) };

		public override void Create(VisualElement root)
		{
			var enumButtons = new EnumButtonsField("Enum Buttons");
			enumButtons.Type = _types[0];
			enumButtons.UseFlags = false;
			enumButtons.RegisterValueChangedCallback(ValueChanged);
			root.Add(enumButtons);

			var typePopup = new UnityEditor.UIElements.PopupField<Type>("Type", _types, 0, type => type.Name, type => type.Name);
			typePopup.RegisterValueChangedCallback(e => enumButtons.Type = e.newValue);
			root.Add(typePopup);

			var flagsToggle = new Toggle("Use Flags");
			flagsToggle.RegisterValueChangedCallback(e => enumButtons.UseFlags = e.newValue);
			root.Add(flagsToggle);
		}

		private void ValueChanged(ChangeEvent<Enum> e)
		{
			Debug.Log($"Value changed from {e.previousValue} to {e.newValue}");
		}
	}
}
