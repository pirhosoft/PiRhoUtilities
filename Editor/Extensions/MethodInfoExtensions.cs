using System;
using System.Reflection;

namespace PiRhoSoft.Utilities.Editor
{
	public static class MethodInfoExtensions
	{
		public static bool HasSignature(this MethodInfo method, Type returnType, params Type[] parameterTypes)
		{
			if (returnType != null && method.ReturnType != returnType)
				return false;

			return method.HasParameters(parameterTypes);
		}

		public static bool HasParameters(this MethodInfo method, params Type[] parameterTypes)
		{
			var parameters = method.GetParameters();
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