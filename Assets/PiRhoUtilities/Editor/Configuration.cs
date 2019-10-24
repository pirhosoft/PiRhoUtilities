namespace PiRhoSoft.Utilities.Editor
{
	public static class Configuration
	{
		private static string _elementsPath = null;
		internal static string ElementsPath
		{
			get
			{
				if (_elementsPath == null)
					_elementsPath = AssetHelper.FindEditorPath(nameof(Configuration), "PiRhoUtilities/Editor/", "Packages/com.pirho.utilities/Editor/") + "Elements/";

				return _elementsPath;
			}
		}
	}
}