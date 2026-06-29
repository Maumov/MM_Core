using UnityEngine;
using System.Collections;
using System;

using Object = UnityEngine.Object;

namespace designpatterns
{
	[AddComponentMenu("")]
	public class CoroutineHolder : MonoBehaviour
	{
	}

	public static class StaticMonoBehaviour
	{

		private static CoroutineHolder _runner;

		private static CoroutineHolder runner
		{
			get
			{
				if (_runner == null)
				{
					_runner = new GameObject("Static Coroutine MonoBehaviour").AddComponent<CoroutineHolder>();
					Object.DontDestroyOnLoad(_runner);
				}

				return _runner;
			}
		}

		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			return runner.StartCoroutine(coroutine);
		}

		private static IEnumerator InvokerCoroutine(Action cb, float delay, float frequency)
		{
			yield return new WaitForSeconds(delay);
			while (true)
			{
				cb();
				yield return new WaitForSeconds(frequency);
			}
		}

		public static void InvokeRepeating(Action cb, float delay, float frequency)
		{
			runner.StartCoroutine(InvokerCoroutine(cb, delay, frequency));
		}

		public static void StopCoroutine(Coroutine coroutine)
		{
			runner.StopCoroutine(coroutine);
		}
	}
}