# PiRhoUtilities
A suite of [UIElements](https://docs.unity3d.com/Manual/UIElements.html) based attributes, drawers, controls, and other helpers to expedite the creation of inspectors without the need of additional custom editors.

## Installation

- In your Unity project open the Package Manager ('Window -> Package Manager')
- Click on the 'Add' (+) button in the top left and choose "Add package from git URL..."
- Enter the URL, https://github.com/pirhosoft/PiRhoUtilities.git#upm in the popup box and click 'Add'
- To import samples into your project, find and select the installed package in the Package and click on 'Import into Project' next to the PiRho Utilities Samples listing. The samples project comes with a scene containing objects with an example of each attribute as well as a sample window (Window > PiRhoUtiilities > UI Samples) showing how to create controls through C# and UXML

## Updating

- Once installed, open the Packages/manifest.json file in a text editor
- At the bottom in a property named "lock", remove the object entry titled "com.pirho.utilties"
- Save and return to Unity and it will automatically reimport the updated version from the repository

## Known Issues

- https://github.com/pirhosoft/PiRhoUtilities/issues
- Working with [SerializedReference] and the [Reference] attribute is still experimental

## Usage

### Property Drawer Attributes

These attributes are added to serializable fields in editable classes (*MonoBehaviours* or *ScriptableObjects*) to enable custom drawing for the field. Most attributes can have their properties retrieved from another field, property, or method through reflection by specifying the associated string name. All attributes are in the ```PiRhoSoft.Utilities``` namespace.

#### List

Add this to a *SerializedList* or *SerializedArray* derived type to show a much more user friendly list editor than Unity's default with custom callbacks and constraints for adding, removing, and reordering. Any additional PropertyTraitAttributes will be applied to each item in the list individually.

```c#
[Serializable] public class TestList : SerializedList<int> { }
[Serializable] public class TestArray : SerializedArray<float> { public TestArray(int count) : base(count) { } }

[List(AllowAdd = nameof(ListCanAdd))]
public TestList List;

[List(AllowAdd = ListAttribute.Never, AllowRemove = ListAttribute.Never, AllowReorder = false)]
public TestArray Array = new TestArray(4);

private bool ListCanAdd()
{
	return List.Count < 5;
}
```

| Property			| Description	| Default	|
| ----------------- | ------------- | --------- |
| EmptyLabel		| When the list is empty display this text - use an empty string to hide the label - `null` will show the default text, "The list is empty"	| `null`	|
| AllowAdd			| The string name of a bool returning method (parameterless), field, or property, that enables/disables adding items to the list. If adding is not conditional, use `ListAttribute.Always` or `ListAttribute.Never`	| `Always`	|
| AllowRemove		| The string name of a bool returning method (parameterless) or (int index), field, or property, that enables/disables removing items from the list. If removing is not conditional, use `ListAttribute.Always` or `ListAttribute.Never` | `Always`	|
| AllowReorder		| Enables/disables reording items in the list.	| `true`	|
| IsCollapsable		| Whether or not the List is collapsible to save space | `true`	|
| AddCallback		| The string name of a method (parameterless) to call when an item has been added to the list	| `null`	|
| RemoveCallback	| The string name of a method (parameterless) or (int index) to call when an item is about to be removed from the list	| `null`	|
| ReorderCallback	| The string name of a method (parameterless) or (int to) or (int from, int to) to call when an item has been reordered in the list	| `null`	|
| ChangeCallback	| The string name of a method (parameterless) to call when the list changes	| `null`	|

![List](/Documentation~/images/list.png)

#### Dictionary

Add this to a SerializedDictionary derived type to show a dictionary editor (by default dictionary editing is unsupported by Unity). This has similar features/functionality as ListAttribute.

```c#
[Serializable] public class TestDictionary : SerializedDictionary<string, string> { }

[Dictionary(AddCallback = nameof(DictionaryItemAdded), RemoveCallback = nameof(DictionaryItemRemoved))]
public TestDictionary Dictionary;

private void DictionaryItemAdded(string key)
{
	Debug.Log($"'{key}' added", this);
}

private void DictionaryItemRemoved(string key)
{
	Debug.Log($"'{key}' removed", this);
}
```

| Property			| Description	| Default	|
| ----------------- | ------------- | --------- |
| EmptyLabel		| When the dictionary is empty display this text - use an empty string to hide the label - `null` will show the default text, "The dictionary is empty"	| `null`	|
| AddPlaceholder	| The string to display in the add area	| `null`	|
| AllowAdd			| The string name of a bool returning method (parameterless) or (string key), field, or property, that enables/disables adding items to the dictionary. If adding is not conditional, use `DictionaryAttribute.Always` or `DictionaryAttribute.Never`	| `Always`	|
| AllowRemove		| The string name of a bool returning method (parameterless) or (string key), field, or property, that enables/disables removing items from the dictionary. If removing is not conditional, use `DictionaryAttribute.Always` or `DictionaryAttribute.Never` | `Always`	|
| AllowReorder		| Enables/disables reording items in the list.	| `true`	|
| IsCollapsable		| Whether or not the List is collapsible to save space | `true`	|
| AddCallback		| The string name of a method (parameterless) or (string key) to call when an item has been added to the dictionary	| `null`	|
| RemoveCallback	| The string name of a method (parameterless) or (string key) to call when an item is about to be removed from the dictionary	| `null`	|
| ReorderCallback	| The string name of a method (parameterless) or (string key) to call when an item has been reordered in the dictionary	| `null`	|

> **NOTE:** Normally dictionaries don't have defined ordering, however, serializing dictionaries requires the key/value pairs to be stored in lists and are thus ordered.

![Dictionary](/Documentation~/images/dictionary.png)

#### ComboBox

Add this to a string field to display a ComboBox control in which you can select from dropdown list of values or enter your own custom value.

```c#
[ComboBox(new string[] { "One Fish", "Two Fish", "Red Fish", "Blue Fish" })]
public string ComboBox;
```

| Property			| Description	| Default	|
| ----------------- | ------------- | --------- |
| Options		| A predefined list of options to show in the dropdown	| `null`	|
| OptionsSource	| The string name of a List<string> returning method (parameterless), field, or property, that defines the list of options	| `null`	|
| AutoUpdate	| Whether the options should automatically update if the values change (may affect performance)	| `true`	|

![Combo Box](/Documentation~/images/combo-box.png)

#### EnumButtons

Apply to an *Enum* field to show selectable buttons instead of a dropdown list. An optional bool can be specified to override the default behavior of the enum being selectable as flags or not (the default is based on whether the _[Flags]_ attribute is set on the declared enum type).

```c#
public enum TestEnum
{
	Zero = 0x0,
	One = 0x1,
	Two = 0x2,
	Four = 0x4,
	Eight = 0x8
}

[EnumButtons]
public TestEnum Buttons;

[EnumButtons(true)]
public TestEnum Flags;
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| Flags			| Whether multiple options can be selected as a flags enum	| `true` if enum class has the *Flags* attribute, `false` otherwise	|

![Enum Buttons](/Documentation~/images/enum-buttons.png)

#### Euler

A simple attribute to apply to a *Quaternion* field to display and assign the *Quaternion* in Euler angles (like the default *Transform* inspector does).

```c#
[Euler]
public Quaternion Euler;
```

#### Frame

Add this to a class or struct type with a *Serializable* attribute to show the fields of the class in a collapsible frame style which looks much better than Unity's default.

```c#
[Serializable]
public class Subclass
{
	public bool Bool;
	public int Int;
	public float Float;
	public string String;
}

[Frame]
public Subclass Frame;
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| IsCollapsable	| Whether the frame can be collapsed or not	| `true`	|

![Frame](/Documentation~/images/frame.png)

#### Group

Add this attribute to any number of fields to display them together as a group with the header label specified in the constructor despite being defined out of order in the class.

```c#
[Group("Group One")] public int Int1;
[Group("Group One")] public float Float1;
[Group("Group Two")] [Maximum(100)] public float Float2;
[Group("Group One")] public bool Bool1;
[Group("Group Two")] public bool Bool2;
[Group("Group Two")] public int Int2;
```

![Group](/Documentation~/images/group.png)

#### Inline

Add this to a class or struct type with a *Serializable* attribute to show the fields of the class or struct inline rather than in the default foldout. Member labels can be optionally shown which is useful if the class has one value field.

```c#
[Serializable]
public class WrapperClass
{
	public int Value;
}

[Inline]
public WrapperClass Wrapper;
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| ShowMemberLabels	| Whether the label of the owning field should be used rather than the label for each of the fields in the class or struct (useful for wrapper classes with a single field)	| `true`	|

#### ObjectPicker

Add this to a UnityEngine.Object derived field to show a searchable popup listing (like the *AddComponent* window) of the available objects of the correct type instead of the default object picker. This will list assets based on their structure in the project folder, and Components/GameObjects by their hierarchy in the scene.

```c#
[ObjectPicker]
public ScriptableObject Asset;
```

![Object Picker](/Documentation~/images/object-picker.png)

#### TypePicker

Add this to a string field to show a searchable popup listing (like the 'AddComponent window') of types derived from the selected type. Since the *Type* class itself is not serializable this will store the selected type's [AssemmblyQualifiedName](https://docs.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=netframework-4.8) so that the desired type can be looked up with ```Type.GetType(assemblyQualifiedName)``` method.

```c#
[TypePicker(typeof(MonoBehaviour), false)]
public string Type;
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| BaseType		| The type to use as the base type to select from 	| required	|
| ShowAbstract	| Whether to display abstract types in the picker	| `false`	|

![Type Picker](/Documentation~/images/type-picker.png)

#### Popup

Add this to an int, float, or string field to show a popup instead of the default text box. The constructor takes two parameters: an array of floats/ints/strings that are the values to select and assign, and an optional array of strings that are shown as the labels for each value. These can also be retreived from a method, property, or field returning a List&lt;T&gt; or a PopupValues&lt;T&gt; if string option names are desired.

```c#
[Popup(new int[] { 0, 1, 2, 3, 4 }, new string[] { "Zero", "One", "Two", "Three", "Four" })]
public int IntPopup;

[Popup(new float[] { 0.0f, 0.1f, 0.2f, 0.4f, 0.8f, 1.6f })]
public float FloatPopup;

[Popup(new string[] { "", "Hello", "Hola", "Bonjour" })]
public string StringPopup;
```

![Popup](/Documentation~/images/popup.png)

#### Reference

Add this attribte to a field in addition to Unity's *[SerializeReference]* attribute to enable creating and editing of any valid subtype for the field.

```c#
public interface IReferenceType
{
}

[Serializable]
public class ReferenceTypeOne : IReferenceType
{
	public bool Value;
}

[Serializable]
public class ReferenceTypeTwo : IReferenceType
{
	public int Value;
}

[SerializeReference] [Reference]
public IReferenceType Reference;
```

![Reference](/Documentation~/images/reference.png)

#### Tabs

Add this attribute to any number of fields to display them together in a tab like view. The first parameter specifies the group of fields to display together and the second is the title of the tab to display the field on.

```c#
[Tabs("Tabs", "One")] public int Int1;
[Tabs("Tabs", "One")] public float Float1;
[Tabs("Tabs", "Two")] [Maximum(100)] public float Float2;
[Tabs("Tabs", "One")] public bool Bool1;
[Tabs("Tabs", "Two")] public bool Bool2;
[Tabs("Tabs", "Two")] public int Int2;
```

![Tabs](/Documentation~/images/tabs.png)

#### Slider

Apply to an *int*, *float*, or *Vector2* field to display the value as a slider with a minimum and maximum. Applying this to a *Vector2* displays a MinMaxSlider corresponding to the x and y values repectively. The minimum and maximum values can also be retreived from a method, property, or field of the corresponding type.

```c#
[Slider(0, 10)]
public int Slider;

[Slider(0, 10)]
public float SliderFloat

[Slider(0, 10)]
public Vector2 MinMaxSlider;
```

![Slider](/Documentation~/images/slider.png)
![Min Max Slider](/Documentation~/images/min-max-slider.png)

### Property Trait Attributes

These attributes are added to serializable fields in editable classes (*MonoBehaviours* or *ScriptableObjects*) to enable custom validation display and execution for the field. Unlike normal property attributes in Unity, multiple of these can be applied to a single field. Most attributes can have their properties retrieved from another field, property, or method through reflection by specifying the associated string name. All attributes are in the ```PiRhoSoft.Utilities``` namespace.

#### Button

Apply this to a field to display a button next to this field's control. Clicking the button will call the method defined by the passed in string name. Either a label or icon can be specified to show.

```c#
[Button(nameof(Clicked), "Click")]
public bool Toggle;

[Button(nameof(Clicked), ButtonIcon.Inspect, Location = TraitLocation.Right)]
public GameObject Inspect;

private void Clicked() => Debug.Log("Clicked");
```
| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| Label		| The label of the button	| `null`	|
| Icon		| The icon of the button	| `Add`		|
| Tooltip	| The tooltip of the button	| `null`	|
| Location	| The location of the button: `Above`, `Below`, `Left`, or `Right` | `Above`	|

#### ChangeTrigger

Apply this to a field and pass in the string name of a method to call when the value changes. The method may be optionally be parameterless, take the new value, or take both the old and the new value.

```c#
[ChangeTrigger(nameof(Changed))]
public bool Toggle;

private void Changed(bool oldValue, bool newValue) => Debug.Log($"Changed from {oldValue} to {newValue}");
```

#### Conditional

Add this to a field to only display the field based on the specfied rules. The constructor takes the string name of a sibling method, field, or property that returns the dependent value and how to test agaist that value. The dependent value can be a string, enum, int, float, bool, or UnityObject.

```c#
public bool ShowConditional = true;

[Conditional(nameof(ShowConditional), BoolTest.ShowIfTrue)]
public string ConditionalString;
```

#### CustomLabel

Add this to any field to change the display of the label. This is useful to give more info in the inspector without changing the name of the field to something over verbose or invalid in code. The label can also be retreived from a string method, field, or property.

```c#
[CustomLabel("Speed (m/s)")]
public float Speed;
```

#### Delay

```c#
[Delay]
public string DelayValidation;
```

Apply this to a any field that uses text input to edit its value to delay the value being applied until enter is pressed.

#### InspectTrigger

Apply this to a field and pass in the string name of a method to call when the object is initially selected in the inspector. This is useful for updating and validating values prior to inspecting.

```c#
[InspectTrigger(nameof(Inspect))]
public bool Toggle;

private void Inspect() => Debug.Log("Object selected");
```

#### Maximum

Add this to an *int* or *float* field to disallow selecting of a value higher than a specified value. The maximum can also be retreived from a method, field, or property of the corresponding type.

```c#
[Maximum(100.0f)]
public float MaximumFloat;
```

#### Minimum

Add this to an *int* or *float* field to disallow selecting of a value lower than a specified value. The minimum can also be retreived from a method, field, or property of the corresponding type.

```c#
[Minimum(0.0f)]
public float MinimumFloat;
```

#### MaximumLength

Apply this to a *string* field to constrain it to a maximum length of characters. The maximum can also be retreived from a method, field, or property.

```c#
[MaximumLength(5)]
public string MaximumString;
```

#### Multiline

Apply this to a *string* field to display it as a multiline text box.

```c#
[Multiline]
public string MultilineText;
```

#### MessageBox

Apply this to any field to display a MessageBox that can provide useful warnings or info.

```c#
[MessageBox("ALERT: ALL OUR BASE ARE BELONG TO US", MesasgeBoxType.Info)]
public string Message = "A message is above me";
```

#### NoLabel

Add this to any field to make it display in the inspector without a label.

```c#
[NoLabel]
public string NoLabel = "I don't have a label";
```

#### Placeholder

Apply this to a *string*, field to display a placeholder label in the text field when the the string is empty. The placeholder can also be retreived from a string method, field, or property.

```c#
[Placeholder("placeholder")]
public string Placeholder;
```

#### ReadOnly

Add this to any field to disable editing of the field (while still showing it) in the inspector.

```c#
[ReadOnly]
public float Disabled;
```

#### Required

Apply this to a *string*, *UnityEngine.Object* derived type, or *[SerializeReference]* field to display a message box warning if they are empty or `null`.

```c#
[Required("A string must be entered")]
public string RequiredString;

[Required("An object must be selected")]
public GameObject RequiredObject;
```

#### Snap

Add this to any number field (*int*, *float*, *Vector2*, etc) to round the selected value to be a multiple of a specified value. The snapped value can also be retreived from a method, field, or property that returns the corresponding type.

```c#
[Snap(0.5f)]
public float SnapFloat;
```

#### Stretch

Apply this to any field to display its control below the label instead of next to it. This is useful for controls that can take advantage of the extra space normally taken up by the label.

```c#
[Stretch]
[Multiline]
public string MultilineStretch;
```

#### Validate

Apply this to a field to call a bool returning method determining whether or not the specified value is valid. If not a MessageBox will be displayed.

```c#
[Validate(nameof(IsOdd), "An odd number must be entered")]
public int OddInt;
private bool IsOdd() => OddInt % 2 != 0;
```

### Wrapper Classes

#### SerializedList&lt;T&gt;

This can be used just like the built in List class but because it isn't the built in List class can be targeted by PropertyDrawers. Because Unity doesn't serialize generic classes, though, it is necessary to subclass this for each type and give the subclass the *[Serializable]* attribute.

```c#
[Serializable] public class IntList : SerializedList<int> {}

public class ExampleBehaviour : MonoBehaviour
{
	[List] public IntList List = new IntList();
}
```

#### SerializedArray&lt;T&gt;

This is exactly like *SerializedList* except for Arrays. The exception is *SerializedArray* must be constructed with a length.

```c#
[Serializable] public class IntArray : SerializedArray<int> { public IntArray(int count) : base(count) {} }

public class ExampleBehaviour : MonoBehaviour
{
	[List] public IntArray Array = new IntArray(5);
}
```

#### SerializedDictionary&lt;KeyType, ValueType&gt;

And again for Dictionary. KeyType must be `string` in order to be targeted by the `DictionaryAttribute`

```c#
[Serializable] public class IntDictionary : SerializedDictionary<string, int> {}

public class ExampleBehaviour : MonoBehaviour
{
	[Dictionary] public IntDictionary Dictionary = new IntDictionary();
}
```
