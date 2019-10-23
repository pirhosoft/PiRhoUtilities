namespace PiRhoSoft.Utilities.Editor
{
	public static class Configuration
	{
		private const string _packagePath = "Packages/com.pirho.utilities/Editor/";
		private static string _elementsPath = null;
		internal static string ElementsPath
		{
			get
			{
				if (_elementsPath != null)
					return _elementsPath;

				_elementsPath = AssetHelper.FindEditorPath(nameof(Configuration), "PiRhoUtilities/Editor/", _packagePath) + "Elements/";
				return _elementsPath;
			}
		}
	}
}