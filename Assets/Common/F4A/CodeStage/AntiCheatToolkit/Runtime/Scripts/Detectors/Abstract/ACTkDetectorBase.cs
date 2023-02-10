#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Detectors
{
	using Common;

	using System;
	using UnityEngine;
	using UnityEngine.Events;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	/// <summary>
	/// Base class for all detectors.
	/// </summary>
	[AddComponentMenu("")]
	public abstract class ACTkDetectorBase : MonoBehaviour
	{
		protected const string ContainerName = "Anti-Cheat Toolkit Detectors";
		protected const string MenuPath = "Code Stage/Anti-Cheat Toolkit/";
		protected const string GameObjectMenuPath = "GameObject/Create Other/" + MenuPath;

		protected static GameObject detectorsContainer;

		/// <summary>
		/// Allows to start detector automatically.
		/// Otherwise, you'll need to call StartDetection() method to start it.
		/// </summary>
		/// Useful in conjunction with proper Detection Event configuration in the inspector.
		/// Allows to use detector without writing any code except the actual reaction on cheating.
		[Tooltip("Automatically start detector. Detection Event will be called on detection.")]
		public bool autoStart = true;

		/// <summary>
		/// Detector will survive new level (scene) load if checked. Otherwise it will be destroyed.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other %Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		[Tooltip("Detector will survive new level (scene) load if checked.")]
		public bool keepAlive = true;

		/// <summary>
		/// Detector component will be automatically disposed after firing callback if enabled.
		/// Otherwise, it will just stop internal processes.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other %Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		[Tooltip("Automatically dispose Detector after firing callback.")]
		public bool autoDispose = true;

		/// <summary>
		/// Subscribe to this event to get notified when cheat will be detected.
		/// </summary>
		public event Action CheatDetected;

		[SerializeField]
		protected UnityEvent detectionEvent;

		[SerializeField]
		protected bool detectionEventHasListener;

		protected bool started;
		protected bool isRunning;

		/// <summary>
		/// Allows to check if detection is currently running.
		/// </summary>
		public bool IsRunning
		{
			get { return isRunning; }
		}

		#region detectors placement
#if UNITY_EDITOR
		[MenuItem(GameObjectMenuPath + "All detectors", false, 0)]
		private static void AddAllDetectorsToScene()
		{
			AddInjectionDetectorToScene();
			AddObscuredCheatingDetectorToScene();
			AddSpeedHackDetectorToScene();
			AddWallHackDetectorToScene();
			AddTimeCheatingDetectorToScene();
		}

		[MenuItem(GameObjectMenuPath + InjectionDetector.ComponentName, false, 1)]
		private static void AddInjectionDetectorToScene()
		{
			SetupDetectorInScene<InjectionDetector>();
		}

		[MenuItem(GameObjectMenuPath + ObscuredCheatingDetector.ComponentName, false, 1)]
		private static void AddObscuredCheatingDetectorToScene()
		{
			SetupDetectorInScene<ObscuredCheatingDetector>();
		}

		[MenuItem(GameObjectMenuPath + SpeedHackDetector.ComponentName, false, 1)]
		private static void AddSpeedHackDetectorToScene()
		{
			SetupDetectorInScene<SpeedHackDetector>();
		}

		[MenuItem(GameObjectMenuPath + WallHackDetector.ComponentName, false, 1)]
		private static void AddWallHackDetectorToScene()
		{
			SetupDetectorInScene<WallHackDetector>();
		}

		[MenuItem(GameObjectMenuPath + TimeCheatingDetector.ComponentName, false, 1)]
		private static void AddTimeCheatingDetectorToScene()
		{
			SetupDetectorInScene<TimeCheatingDetector>();
		}

		private static void SetupDetectorInScene<T>() where T : ACTkDetectorBase
		{
			var component = FindObjectOfType<T>();
			var detectorName = typeof(T).Name;

			if (component != null)
			{
				if (component.gameObject.name == ContainerName)
				{
					EditorUtility.DisplayDialog(detectorName + " already exists!", detectorName + " already exists in scene and correctly placed on object \"" + ContainerName + "\"", "OK");
				}
				else
				{
					var dialogResult = EditorUtility.DisplayDialogComplex(detectorName + " already exists!", detectorName + " already exists in scene and placed on object \"" + component.gameObject.name + "\". Do you wish to move it to the Game Object \"" + ContainerName + "\" or delete it from scene at all?", "Move", "Delete", "Cancel");
					switch (dialogResult)
					{
						case 0:
							var container = GameObject.Find(ContainerName);
							if (container == null)
							{
								container = new GameObject(ContainerName);
							}
							var newComponent = container.AddComponent<T>();
							EditorUtility.CopySerialized(component, newComponent);
							DestroyDetectorImmediate(component);
							break;
						case 1:
							DestroyDetectorImmediate(component);
							break;
						default:
							Debug.LogError("Unknown result from the EditorUtility.DisplayDialogComplex API!");
							break;
					}
				}
			}
			else
			{
				var container = GameObject.Find(ContainerName);
				if (container == null)
				{
					container = new GameObject(ContainerName);

					Undo.RegisterCreatedObjectUndo(container, "Create " + ContainerName);
				}
				Undo.AddComponent<T>(container);

				EditorUtility.DisplayDialog(detectorName + " added!", detectorName + " successfully added to the object \"" + ContainerName + "\"", "OK");
				Selection.activeGameObject = container;
			}
		}

		private static void DestroyDetectorImmediate(ACTkDetectorBase component)
		{
			if (component.transform.childCount == 0 && component.GetComponentsInChildren<Component>(true).Length <= 2)
			{
				DestroyImmediate(component.gameObject);
			}
			else
			{
				DestroyImmediate(component);
			}
		}
#endif
		#endregion

		#region unity messages
#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void Start()
		{
			if (detectorsContainer == null && gameObject.name == ContainerName)
			{
				detectorsContainer = gameObject;
			}

			if (autoStart && !started)
			{
				StartDetectionAutomatically();
			}
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnEnable()
		{
			ResumeDetector();
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnDisable()
		{
			PauseDetector();
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnApplicationQuit()
		{
			DisposeInternal();
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		protected virtual void OnDestroy()
		{
			StopDetectionInternal();

			if (transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
			{
				Destroy(gameObject);
			}
			else if (name == ContainerName && GetComponentsInChildren<ACTkDetectorBase>().Length <= 1)
			{
				Destroy(gameObject);
			}
		}
		#endregion

		internal virtual void OnCheatingDetected()
		{
			if (CheatDetected != null)
				CheatDetected.Invoke();

			if (detectionEventHasListener)
				detectionEvent.Invoke();

			if (autoDispose)
			{
				DisposeInternal();
			}
			else
			{
				StopDetectionInternal();
			}
		}

		protected virtual bool Init(ACTkDetectorBase instance, string detectorName)
		{
			if (instance != null && instance != this && instance.keepAlive)
			{
				Debug.LogWarning(ACTkConstants.LogPrefix + name + 
					": self-destroying, other instance already exists & only one instance allowed!", gameObject);
				Destroy(this);
				return false;
			}

			DontDestroyOnLoad(transform.parent != null ? transform.root.gameObject : gameObject);

			return true;
		}

		protected virtual void DisposeInternal()
		{
			Destroy(this);
		}

		protected virtual bool DetectorHasCallbacks()
		{
			return CheatDetected != null || detectionEventHasListener;
		}

		protected virtual void StopDetectionInternal()
		{
			CheatDetected = null;
			started = false;
			isRunning = false;
		}

		protected virtual void PauseDetector()
		{
			if (!started)
				return;

			isRunning = false;
		}

		protected virtual bool ResumeDetector()
		{
			if (!started || !DetectorHasCallbacks())
				return false;

			isRunning = true;
			return true;
		}

		protected abstract void StartDetectionAutomatically();
	}
}