using System;
using System.Reflection;

namespace PiRhoSoft.Utilities.Editor
{
	public static class MethodInfoExtensions
	{
		public static bool HasSignature(this MethodInfo method, Type returnType, params Type[] parameterTypes)
		{
			var parameters = method.GetParameters();

			if (returnType != null && method.ReturnType != returnType) return false;
			if (parameters.Length != parameterTypes.Length) return false;

			for (var i = 0; i < parameters.Length; i++)
			{
				if (parameterTypes[i] != null && parameters[i].ParameterType != parameterTypes[i])
					return false;
			}

			return true;
		}
	}
}