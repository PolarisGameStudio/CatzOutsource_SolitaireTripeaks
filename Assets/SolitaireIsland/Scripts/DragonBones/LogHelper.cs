using UnityEngine;

namespace DragonBones
{
	internal static class LogHelper
	{
		internal static void LogWarning(object message)
		{
			UnityEngine.Debug.LogWarning("[DragonBones]" + message);
		}
	}
}
