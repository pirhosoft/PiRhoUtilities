# PiRhoUtilities
A suite of [UIElements](https://docs.unity3d.com/Manual/UIElements.html) based attributes, drawers, controls, and other helpers to expedite the creation of inspectors without the need of additional custom editors.

## How To Use

- In your Unity project open the Package Manager ('Window -> Package Manager')
- Click on the 'Add' (+) button in the top left and choose "Add package from git URL..."
- Enter the URL, https://github.com/pirhosoft/PiRhoUtilities.git#upm in the popup box and click 'Add'
- To import samples into your projeect, find and select the installed package in the Package and click on 'Import into Project' next to the PiRho Utilities Samples listing

## Property Drawer Attributes

These attributes are added to serializable fields in editable classes (*MonoBehaviours* or *ScriptableObjects*) to enable custom drawing for the field. Only a single one of these can be applied to a field as it handles the final control creation. All attributes are in the ```PiRhoSoft.Utilities``` namespace.

**List**

Add this to a *SerializedList* or *SerializedArray* derived type to show a much more user friendly list editor than Unity's default with custom callbacks and constraints for adding, removing, and reordering. Any additional PropertyTraits will be applied to each item in the list individually.

```c#
[Serializable] public class TestList : SerializedList<int> { }
[Serializable] public class TestArray : SerializedArray<float> { public TestArray(int count) : base(count) { } }

[List(AllowAdd = nameof(ListCanAdd))]
public TestList List;

[List(AllowAdd = ListAttribute.Never, AllowRemove = ListAttribute.Never, AllowReorder = ListAttribute.Never)]
public TestArray Array = new TestArray(4);

private bool ListCanAdd()
{
	return List.Count < 5;
}
```

| Property			| Description	| Default	|
| ----------------- | ------------- | --------- |
| EmptyLabel		| When the list is empty display this text - use an empty string to hide the label - `null` will show the default text, "The list is empty"	| `null`	|
| AllowAdd			| The string name of a bool returning method (parameterless) that enables/disables adding items to the list. If adding is not conditional, use `ListAttribute.Always` or `ListAttribute.Never`	| `Always`	|
| AllowRemove		| The string name of a bool returning method (parameterless) or (int index) that enables/disables removing items from the list. If removing is not conditional, use `ListAttribute.Always` or `ListAttribute.Never` | `Always`	|
| AllowReorder		| The string name of a bool returning method (parameterless) or (int to) or (int from, int to) that enables/disables reording items in the list. If reordering is not conditional, use `ListAttribute.Always` or `ListAttribute.Never`	| `Always`	|
| AddCallback		| The string name of a method (parameterless) to call when an item has been added to the list	| `null`	|
| RemoveCallback	| The string name of a method (parameterless) or (int index) to call when an item is about to be removed from the list	| `null`	|
| ReorderCallback	| The string name of a method (parameterless) or (int to) or (int from, int to) to call when an item has been reordered in the list	| `null`	|

**Dictionary**

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
| AllowAdd			| The string name of a bool returning method (parameterless) or (string key) that enables/disables adding items to the dictionary. If adding is not conditional, use `DictionaryAttribute.Always` or `DictionaryAttribute.Never`	| `Always`	|
| AllowRemove		| The string name of a bool returning method (parameterless) or (string key) that enables/disables removing items from the dictionary. If removing is not conditional, use `DictionaryAttribute.Always` or `DictionaryAttribute.Never` | `Always`	|
| AllowReorder		| The string name of a bool returning method (parameterless) or (string key) that enables/disables reording items in the dictionary. If reordering is not conditional, use `DictionaryAttribute.Always` or `DictionaryAttribute.Never`	| `Never`	|
| AddCallback		| The string name of a method (parameterless) or (string key) to call when an item has been added to the dictionary	| `null`	|
| RemoveCallback	| The string name of a method (parameterless) or (string key) to call when an item is about to be removed from the dictionary	| `null`	|
| ReorderCallback	| The string name of a method (parameterless) or (string key) to call when an item has been reordered in the dictionary	| `null`	|

> **NOTE:** Normally dictionaries don't have defined ordering, however, serializing dictionaries requires the key/value pairs to be stored in lists and are thus ordered.

**ObjectPicker**

Add this to a UnityEngine.Object derived field to show a searchable popup listing (like the *AddComponent*window') of the available objects of the correct type instead of the default object picker This will list assets based on their structure in the project folder, and Components/GameObjects by their hierarchy in the scene.

```c#
[ObjectPicker]
public ScriptableObject Asset;
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| BaseType		| The type to use as the BaseType of valid types - if `null` then the field's type will be used | `null`	|

**ScenePicker**

Similar to the ObjectPicker in that it shows a searchable popup of Scenes to assign. This attribute utilizes Unity's new [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@0.4/manual/index.html) package and must be applied to a field of type ```AssetReference``` contained in the ```UnityEngine.AddressableAssets``` namespace;

```c#
[ScenePicker(nameof(CreateScene)]
public AssetReference Scene;

private void CreateScene()
{
	Debug.Log("Scene Created");
}
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| CreateMethod	| The string name of a method to call when the create scene button is clicked. Useful to create any relevant defaults for a scene assigned to this field | `null`	|

**TypePicker**

Add this to a string field to show a searchable popup listing (like the 'AddComponent window') of types derived from the selected type. Since the *Type* class itself is not serializable this will store the selected type's [AssemmblyQualifiedName](https://docs.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=netframework-4.8) so that the desired type can be looked up with ```Type.GetType(assemblyQualifiedName)``` method.

```c#
[TypePicker(typeof(MonoBehaviour), false)]
public string Type;
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| BaseType		| The type to use as the BaseType to select from 	| required	|
| ShowAbstract	| Whether to display abstract types in the picker	| `false`	|

**Popup**

Add this to an int, float, or string field to show a popup instead of the default text box. The constructor takes two parameters: an array of floats/ints/strings/ that are the values to select and assign, and an optional array of strings that are shown as the labels for each value.

```c#
[Popup(new int[] { 0, 1, 2, 3, 4 }, new string[] { "Zero", "One", "Two", "Three", "Four" })]
public int IntPopup;

[Popup(new float[] { 0.0f, 0.1f, 0.2f, 0.4f, 0.8f, 1.6f })]
public float FloatPopup;

[Popup(new string[] { "", "Hello", "Hola", "Bonjour" })]
public string StringPopup;
```

**ComboBox**

Add this to a string field to display a ComboBox control in which you can select from dropdown list of values or enter your own custom value. The constructor takes an array of strings that will appear in the dropdown.

```c#
[ComboBox(new string[] { "One Fish", "Two Fish", "Red Fish", "Blue Fish" })]
public string ComboBox;
```

**Group**

Add this attribute to any number of fields to display them together as a group with the header label specified in the constructor despite being defined out of order in the class. The group can optionally be displayed as a collapsible rollout.

```c#
[Group("One", Style = GroupStyle.Rollout)] public int Int1;
[Group("One")] public float Float1;
[Group("Two")] [Maximum(100)] public float Float2;
[Group("One")] public bool Bool1;
[Group("Two")] public bool Bool2;
[Group("Two")] public int Int2;
```

**Rollout**

Add this to a class or struct type with a *Serializable* attribute to show the fields of the class in a collapsible rollout style which looks much better than Unity's default.

```c#

[Serializable]
public class Subclass
{
	public bool Bool;
	public int Int;
	public float Float;
	public string String;
}

[Rollout]
public Subclass Rollout;
```

**Inline**

Add this to a class or struct type with a *Serializable* attribute to show the fields of the class or struct inline rather than in the default foldout.

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

**Euler**

A simple attribute to apply to a *Quaternion* field to display and assign the *Quaternion* in Euler angles (like the default *Transform* inspector does).

```c#
[Euler]
public Quaternion Euler;
```

**Slider**

Apply to an *int*, *float*, or *Vector2* field to display the value as a slider with a minimum and maximum. Applying this to a *Vector2* displays a MinMaxSlider corresponding to the x and y values repectively.

```c#
[Slider(0, 10)]
public int Slider;

[Slider(0, 10)]
public Vector2 MinMaxSlider;
```

**EnumButtons**

Apply to an *Enum* field to show selectable buttons instead of a dropdown list. An optional bool can be specified to override the default behavior of the enum being selectable as flags or not.

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

## Property Trait Attributes

These attributes are added to serializable fields in editable classes (*MonoBehaviours* or *ScriptableObjects*) to enable custom validation display and execution for the field. Unlike normal property attributes in Unity, multiple of these can be applied to a single field. All attributes are in the ```PiRhoSoft.Utilities``` namespace.

**Conditional**

Add this to any field to only display the field based on the specfied rules. The constructor takes the string name of a sibling *SerializableProperty* (default), a sibling field, a sibling property, or a sibling method that returns the dependent value. The dependent value can be a string, enum, int, float, bool, or Unity Object. If it is an Object, the field is tested as a bool based on whether the Object is null or not and if it is an Enum the object is tested based on the int value of the Enum.

```c#
public bool ShowConditional = true;

[Conditional(nameof(ShowConditional), BoolValue = true)]
public string ConditionalString;
```

| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| Source	| The source to retreived the tested value from: `Sibling` retreives the value based on the *SerializedProperty* value of the field. `Field` means test a non Serializable sibling field. `Property` means test a sibling property value. `Method` means test the return value of a sibling method.	| `Sibling`	|
| Test		| The method used to test the source value against: `Equal`, `Inequal`, `LessThan`, `GreaterThan`, `LessThanOrEqual`, `GreaterThanOrEqual`	| `Equal`	|

**ChangeTrigger**

Apply this to a field and pass in the string name of a method to call when the value changes. The method may be optionally be parameterless, take the new value, or take both the old and the new value.

```c#
[ChangeTrigger(nameof(Changed))]
public bool Toggle;

private void Changed(bool oldValue, bool newValue) => Debug.Log($"Changed from {oldValue} to {newValue}", this);
```

**InspectTrigger**

Apply this to a field and pass in the string name of a method to call when the object is initially selected in the inspector. This is useful for updating and validating values prior to inspecting.

```c#
[InspectTrigger(nameof(Inspect))]
public bool Toggle;

private void Inspect() => Debug.Log("Object selected", this);
```

**Required**

Apply this to a *string* or *UnityEngine.Object* derived type to display a message box warning if they `null`.

```c#
[Required("A string must be entered")]
public string RequiredString;

[Required("An object must be selected")]
public GameObject RequiredObject;
```

**Validate**

Apply this to a field to call a bool returning method determining whether or not the specified value is valid. If not a message box with a waring will displayed.

```c#
[Validate(nameof(IsOdd), "An odd number must be entered")]
public int OddInt;
private bool IsOdd() => OddInt % 2 != 0;
```

**Button**

Apply this to a field to display a button next to this field's control. Clicking the button will call the method defined by the passed in string name.

```c#
[Button(nameof(Clicked), Label = "Click")]
public bool Toggle;

private void Clicked() => Debug.Log("Clicked", this);
```
| Parameter		| Description	| Default	|
| ------------- | ------------- | --------- |
| Label		| The label of the button	| `null`	|
| Tooltip	| The tooltip of thh button	| `null`	|
| Location	| The location of the button: `Before`, `After`, `Left`, or `Right` | `Before`	|

**Minimum**

Add this to an *int* or *float* field to disallow selecting of a value lower than a specified value. Set the minimum value in the attribute constructor.

```c#
[Minimum(0.0f)]
public float MinimumFloat;
```

**Maximum**

Add this to an *int* or *float* field to disallow selecting of a value higher than a specified value. Set the maximum value in the attribute constructor.

```c#
[Maximum(100.0f)]
public float MaximumFloat;
```

**Snap**

Add this to an *int* or *float* field to round the selected value to be a multiple of a specified value. Set the snap value in the attribute constructor.

```c#
[Snap(0.5f)]
public float SnapFloat;
```

**Placeholder**

Apply this to a *string*, field to display a placeholder label in the text field when the the string is empty.

```c#
[Placeholder("placeholder")]
public string Placeholder;
```

**Delay**

```c#
[Delay]
public string DelayValidation;
```

Apply this to a *string*, *int*, or *float* field to delay the value being applied until enter is pressed.

**Multiline**

Apply this to a *string* field to display it as a multiline text box.

```c#
[Multiline]
public string MultilineText;
```

**MaximumLength**

Apply this to a *string* field to constrain it to a maximum length of characters.

```c#
[MaximumLength(5)]
public string MaximumString;
```

**ReadOnly**

Add this to any field to disable editing of the field (while still showing it) in the inspector.

```c#
[ReadOnly]
public float Disabled;
```

**Stretch**

Apply this to any field to display its control below the label instead of next to it. This is useful for controls that can take advantage of the extra space normally taken up by the label.

```c#
[Stretch]
public string MultilineStretch;
```

**CustomLabel**

Add this to any field to change the display of the label. This is useful to give more info in the inspector without changing the name of the field to something over verbose or invalid in code.

```c#
[CustomLabel("Speed (m/s)")]
public float Speed;
```

## Wrapper Classes

**SerializedList&lt;T&gt;**

This can be used just like the built in List class but because it isn't the built in List class can be targeted by PropertyDrawers. Because Unity doesn't serialize generic classes, though, it is necessary to subclass this for each type and give the subclass the *Serializable* attribute.

```c#
[Serializable] public class IntList : SerializedList<int> {}

public class ExampleBehaviour : MonoBehaviour
{
	[List] public IntList List = new IntList();
}
```

**SerializedArray&lt;T&gt;**

This is exactly like *SerializedList* except for Arrays. The exception is *SerializedArray* must be constructed with a length.

```c#
[Serializable] public class IntArray : SerializedArray<int> { public IntArray(int count) : base(count) {} }

public class ExampleBehaviour : MonoBehaviour
{
	[List] public IntArray Array = new IntArray(5);
}
```

**SerializedDictionary&lt;KeyType, ValueType&gt;**

And again for Dictionary. KeyType must be `string` in order to be targeted by the `DictionaryAttribute`

```c#
[Serializable] public class IntDictionary : SerializedDictionary<string, int> {}

public class ExampleBehaviour : MonoBehaviour
{
	[Dictionary] public IntDictionary Dictionary = new IntDictionary();
}
```
