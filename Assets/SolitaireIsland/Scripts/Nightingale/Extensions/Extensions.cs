using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.Extensions
{
	public static class Extensions
	{
		public static AssetBundle ToAssetBundle(this byte[] bytes)
		{
			try
			{
				if (bytes != null && bytes.Length > 0)
				{
					return AssetBundle.LoadFromMemory(bytes);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			return null;
		}

		public static bool HasVaule<T>(this T defaultT, UnityAction<T> unityAction = null)
		{
			if (defaultT == null)
			{
				return false;
			}
			unityAction?.Invoke(defaultT);
			return true;
		}

		public static void FindType<T>(this Transform transform, UnityAction<T> unityAction) where T : Component
		{
			transform.GetComponent<T>().HasVaule(delegate(T defauleT)
			{
				if (unityAction != null)
				{
					unityAction(defauleT);
				}
			});
		}

		public static void FindType<T>(this Transform transform, string path, UnityAction<T> unityAction) where T : Component
		{
			transform.Find(path).HasVaule(delegate(Transform _transform)
			{
				_transform.FindType(unityAction);
			});
		}

		public static string TOBase64(this string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return string.Empty;
			}
			try
			{
				return Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
			}
			catch (Exception)
			{
				return content;
			}
		}

		public static string FromBase64(this string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return string.Empty;
			}
			try
			{
				return Encoding.UTF8.GetString(Convert.FromBase64String(content));
			}
			catch (Exception)
			{
				return content;
			}
		}

		public static void ForEach<T>(this T[] bytes, UnityAction<T> action)
		{
			if (action != null)
			{
				foreach (T arg in bytes)
				{
					action(arg);
				}
			}
		}

		public static List<T> Random<T>(this List<T> arrays, int number)
		{
			if (arrays.Count <= number)
			{
				return arrays;
			}
			while (arrays.Count > number)
			{
				arrays.RemoveAt(UnityEngine.Random.Range(0, arrays.Count));
			}
			return arrays;
		}

		public static T Read<T>(this AssetBundle assetBundle, string path)
		{
			if (assetBundle.Contains(path))
			{
				TextAsset textAsset = assetBundle.LoadAsset<TextAsset>(path);
				if (textAsset == null)
				{
					return default(T);
				}
				string text = textAsset.text;
				return JsonUtility.FromJson<T>(text);
			}
			return default(T);
		}

		public static List<string> Repace(this List<string> arrays, string replaceOld, string replaceNew)
		{
			return (from e in arrays
				group e by e.Replace(replaceOld, replaceNew) into e
				select e.Key).ToList();
		}

		public static string[] Repace(this string[] arrays, string replaceOld, string replaceNew)
		{
			return (from e in arrays
				group e by e.Replace(replaceOld, replaceNew) into e
				select e.Key).ToArray();
		}

		public static T[] ToArray<T>(this T[,] arrays)
		{
			T[] array = new T[arrays.GetLength(0) * arrays.GetLength(1)];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = arrays[i / arrays.GetLength(1), i % arrays.GetLength(1)];
			}
			return array;
		}

		public static T[,] ToArray<T>(this T[] arrays, int column)
		{
			T[,] array = new T[(arrays.Length - 1) / column + 1, column];
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					array[i, j] = arrays[i * column + j];
				}
			}
			return array;
		}

		public static void DelayDo(this MonoBehaviour monoBehaviour, CustomYieldInstruction yieldInstruction, UnityAction unityAction)
		{
			monoBehaviour.StartCoroutine(delayDo(yieldInstruction, unityAction));
		}

		public static void DelayDo(this GameObject gameObject, CustomYieldInstruction yieldInstruction, UnityAction unityAction)
		{
			EmptyScript emptyScript = gameObject.GetComponent<EmptyScript>();
			if (emptyScript == null)
			{
				emptyScript = gameObject.AddComponent<EmptyScript>();
			}
			emptyScript.DelayDo(yieldInstruction, unityAction);
		}

		public static void DelayDo(this MonoBehaviour monoBehaviour, YieldInstruction yieldInstruction, UnityAction unityAction)
		{
			monoBehaviour.StartCoroutine(delayDo(yieldInstruction, unityAction));
		}

		public static void DelayDo(this GameObject gameObject, YieldInstruction yieldInstruction, UnityAction unityAction)
		{
			EmptyScript emptyScript = gameObject.GetComponent<EmptyScript>();
			if (emptyScript == null)
			{
				emptyScript = gameObject.AddComponent<EmptyScript>();
			}
			emptyScript.DelayDo(yieldInstruction, unityAction);
		}

		public static void CancelDelay(this MonoBehaviour monoBehaviour)
		{
			monoBehaviour.StopAllCoroutines();
		}

		public static void CancelDelay(this GameObject gameObject)
		{
			EmptyScript component = gameObject.GetComponent<EmptyScript>();
			if (component != null)
			{
				component.CancelDelay();
			}
		}

		private static IEnumerator delayDo(IEnumerator yieldInstruction, UnityAction unityAction)
		{
			yield return yieldInstruction;
			unityAction?.Invoke();
		}

		private static IEnumerator delayDo(YieldInstruction yieldInstruction, UnityAction unityAction)
		{
			yield return yieldInstruction;
			unityAction?.Invoke();
		}
	}
}
