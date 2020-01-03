using PiRhoSoft.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Samples
{
	public class TabsCodeSample : CodeSample
	{
		public override void Create(VisualElement root)
		{
			var tabs = new Tabs();

			var stats = new TabPage("Stats");
			stats.Add(new Slider("Hp"));
			stats.Add(new Slider("Attack"));
			stats.Add(new Slider("Defense"));
			stats.Add(new Slider("Speed"));
			tabs.Add(stats);

			var equipment = new TabPage("Equipment");
			equipment.Add(new TextField("Helmet"));
			equipment.Add(new TextField("Gloves"));
			equipment.Add(new TextField("Sword"));
			equipment.Add(new TextField("Shield"));
			equipment.Add(new TextField("Boots"));
			tabs.Add(equipment);

			var inventory = new TabPage("Inventory");
			inventory.Add(new IntegerField("Money"));
			inventory.Add(new IntegerField("Potions"));
			tabs.Add(inventory);

			root.Add(tabs);
		}
	}
}
