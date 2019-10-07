namespace PiRhoSoft.Utilities.Editor
{
	public static class Configuration
	{
		private static string _elementsPath = null;
		private const string _elementsFolder = "Elements/";
		private const string _editorFolder = "PiRhoUtilities/Scripts/Editor/";

		internal static string ElementsPath
		{
			get
			{
				if (_elementsPath == null)
					_elementsPath = AssetHelper.FindEditorPath(nameof(Configuration), _editorFolder) + _elementsFolder;

				return _elementsPath;
			}
			set
			{
				_elementsPath = value; // settable so PiRho Utilities can be moved or renamed by end users
			}
		}
	}
}