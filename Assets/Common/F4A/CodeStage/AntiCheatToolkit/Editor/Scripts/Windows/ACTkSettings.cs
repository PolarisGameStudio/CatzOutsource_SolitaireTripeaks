#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if UNITY_EDITOR

namespace CodeStage.AntiCheat.EditorCode.Windows
{
	using Common;

	using System;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	internal class ACTkSettings: EditorWindow
	{
		private class SymbolsData
		{
			public bool injectionDebug;
			public bool injectionDebugVerbose;
			public bool injectionDebugParanoid;
			public bool wallhackDebug;
			public bool excludeObfuscation;
			public bool preventReadPhoneState;
			public bool preventInternetPermission;
			public bool obscuredAutoMigration;
			public bool detectionBacklogs;
			public bool exposeThirdPartyIntegrationSymbol;
		}

		private const string WireframeShaderName = "Hidden/ACTk/WallHackTexture";

		private static SerializedObject graphicsSettingsAsset;
		private static SerializedProperty includedShaders;

		private SymbolsData symbolsData;

		[MenuItem(ACTkEditorGlobalStuff.WindowsMenuPath + "Settings...", false, 100)]
		internal static void ShowWindow()
		{
			var myself = GetWindow<ACTkSettings>(true, "Anti-Cheat Toolkit v." + ACTkConstants.Version + " settings", true);
			myself.minSize = new Vector2(510, 458);
			myself.maxSize = new Vector2(510, 458);
		}

		internal static void ReadGraphicsAsset()
		{
			if (graphicsSettingsAsset != null) return;

			var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
			if (assets.Length > 0)
			{
				graphicsSettingsAsset = new SerializedObject(assets[0]);
			}

			if (graphicsSettingsAsset != null)
			{
				includedShaders = graphicsSettingsAsset.FindProperty("m_AlwaysIncludedShaders");
				if (includedShaders == null)
				{
					Debug.LogError(ACTkEditorGlobalStuff.LogPrefix + "Couldn't find m_AlwaysIncludedShaders property, please report to " +
					               ACTkEditorGlobalStuff.ReportEmail);
				}
			}
			else
			{
				Debug.LogError(ACTkEditorGlobalStuff.LogPrefix + "Couldn't find GraphicsSettings asset, please report to " + ACTkEditorGlobalStuff.ReportEmail);
			}
		}

		internal static int GetWallhackDetectorShaderIndex()
		{
			if (graphicsSettingsAsset == null || includedShaders == null) return -1;

			var result = -1;
			graphicsSettingsAsset.Update();

			var itemsCount = includedShaders.arraySize;
			for (var i = 0; i < itemsCount; i++)
			{
				var arrayItem = includedShaders.GetArrayElementAtIndex(i);
				if (arrayItem.objectReferenceValue != null)
				{
					var shader = (Shader)(arrayItem.objectReferenceValue);

					if (shader.name == WireframeShaderName)
					{
						result = i;
						break;
					}
				}
			}

			return result;
		}

		internal static bool IsWallhackDetectorShaderIncluded()
		{
			var result = false;

			ReadGraphicsAsset();
			if (GetWallhackDetectorShaderIndex() != -1)
				result = true;

			return result;
		}

		private void OnGUI()
		{	
			using (ACTkEditorGUI.Vertical(ACTkEditorGUI.PanelWithBackground))
			{ 
				ACTkEditorGUI.DrawHeader("Injection Detector settings (global)");
				//GUILayout.Label("Injection Detector settings (global)", ACTkEditorGUI.LargeBoldLabel);

				var enableInjectionDetector = EditorPrefs.GetBool(ACTkEditorGlobalStuff.PrefsInjectionEnabled);

				EditorGUI.BeginChangeCheck();
				enableInjectionDetector = GUILayout.Toggle(enableInjectionDetector, "Enable Injection Detector");
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetBool(ACTkEditorGlobalStuff.PrefsInjectionEnabled, enableInjectionDetector);
					if (enableInjectionDetector && !ACTkPostprocessor.IsInjectionDetectorTargetCompatible())
					{
						Debug.LogWarning(ACTkEditorGlobalStuff.LogPrefix + "Injection Detector is not available on selected platform (" +
						                 EditorUserBuildSettings.activeBuildTarget + ")");
					}

					if (!enableInjectionDetector)
					{
						ACTkEditorGlobalStuff.CleanInjectionDetectorData();
					}
					else if (!File.Exists(ACTkEditorGlobalStuff.injectionDataPath))
					{
						ACTkPostprocessor.InjectionAssembliesScan();
					}
				}

				EditorGUILayout.Space();

				if (GUILayout.Button("Edit Whitelist"))
				{
					ACTkAssembliesWhitelist.ShowWindow();
				}

				GUILayout.Space(3);
			}

			EditorGUILayout.Space();

			using (ACTkEditorGUI.Vertical(ACTkEditorGUI.PanelWithBackground))
			{ 
				ACTkEditorGUI.DrawHeader("WallHack Detector settings (per-project)");
				GUILayout.Label("Wireframe module uses specific shader under the hood. Thus such shader should be included into the build to exist at runtime. To make sure it's get included, you may add it to the Always Included Shaders list using buttons below. You don't need to include it if you're not going to use Wireframe module.",
					EditorStyles.wordWrappedLabel);

				ReadGraphicsAsset();

				if (graphicsSettingsAsset != null && includedShaders != null)
				{
					// outputs whole included shaders list, use for debug
					//EditorGUILayout.PropertyField(includedShaders, true);

					var shaderIndex = GetWallhackDetectorShaderIndex();

					EditorGUI.BeginChangeCheck();

					if (shaderIndex != -1)
					{
						GUILayout.Label("Shader <b>is at</b> the Always Included Shaders list, you're good to go!", ACTkEditorGUI.RichLabel);
						if (GUILayout.Button("Remove shader"))
						{
							includedShaders.DeleteArrayElementAtIndex(shaderIndex);
							includedShaders.DeleteArrayElementAtIndex(shaderIndex);
						}
						EditorGUILayout.Space();
					}
					else
					{
						GUILayout.Label("Shader <b>is not</b> at the Always Included Shaders list.", ACTkEditorGUI.RichLabel);
						using (ACTkEditorGUI.Horizontal())
						{
							if (GUILayout.Button("Include automatically", GUILayout.Width(minSize.x / 2f)))
							{
								var shader = Shader.Find(WireframeShaderName);
								if (shader != null)
								{
									includedShaders.InsertArrayElementAtIndex(includedShaders.arraySize);
									var newItem = includedShaders.GetArrayElementAtIndex(includedShaders.arraySize - 1);
									newItem.objectReferenceValue = shader;
								}
								else
								{
									Debug.LogError("Can't find " + WireframeShaderName + " shader! Please report this to the  " +
									               ACTkEditorGlobalStuff.ReportEmail + " including your Unity version number.");
								}
							}

							if (GUILayout.Button("Include manually (see readme.pdf)"))
							{
#if UNITY_2018_3_OR_NEWER
								SettingsService.OpenProjectSettings("Project/Graphics");
#else
								EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
#endif
							}
						}
						GUILayout.Space(3);
					}

					if (EditorGUI.EndChangeCheck())
					{
						graphicsSettingsAsset.ApplyModifiedProperties();
					}
				}
				else
				{
					GUILayout.Label("Can't automatically control " + WireframeShaderName +
					                " shader existence at the Always Included Shaders list. Please, manage this manually in Graphics Settings.");
					if (GUILayout.Button("Open Graphics Settings"))
					{
						EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
					}
				}
			}

			EditorGUILayout.Space();

			using (ACTkEditorGUI.Vertical(ACTkEditorGUI.PanelWithBackground))
			{ 
				ACTkEditorGUI.DrawHeader("Conditional Compilation Symbols (per-project)");
				GUILayout.Label("Here you may switch conditional compilation symbols used in ACTk.\n" +
				                "Check Readme for more details on each symbol.", EditorStyles.wordWrappedLabel);
				EditorGUILayout.Space();
				if (symbolsData == null)
				{
					symbolsData = GetSymbolsData();
				}

				/*if (GUILayout.Button("Reset"))
				{
					var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
					foreach (BuildTargetGroup buildTargetGroup in groups)
					{
						PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Empty);
					}
				}*/

				using (ACTkEditorGUI.Horizontal())
				{
					using (ACTkEditorGUI.Vertical())
					{
						GUILayout.Label("Debug Symbols", ACTkEditorGUI.LargeBoldLabel);
						ACTkEditorGUI.Separator();

						DrawSymbol(ref symbolsData.injectionDebug, ACTkEditorGlobalStuff.ConditionalSymbols.InjectionDebug, "Switches the Injection Detector debug.");
						DrawSymbol(ref symbolsData.injectionDebugVerbose, ACTkEditorGlobalStuff.ConditionalSymbols.InjectionDebugVerbose, "Switches the Injection Detector verbose debug level.");
						DrawSymbol(ref symbolsData.injectionDebugParanoid, ACTkEditorGlobalStuff.ConditionalSymbols.InjectionDebugParanoid, "Switches the Injection Detector paranoid debug level.");
						DrawSymbol(ref symbolsData.wallhackDebug, ACTkEditorGlobalStuff.ConditionalSymbols.WallhackDebug, "Switches the WallHack Detector debug - you'll see the WallHack objects in scene and get extra information in console.");
						DrawSymbol(ref symbolsData.detectionBacklogs, ACTkEditorGlobalStuff.ConditionalSymbols.DetectionBacklogs, "Enables additional logs in some detectors to make it easier to debug false positives.");
					}

					using (ACTkEditorGUI.Vertical())
					{
						GUILayout.Label("Compatibility Symbols", ACTkEditorGUI.LargeBoldLabel);
						ACTkEditorGUI.Separator();

						DrawSymbol(ref symbolsData.exposeThirdPartyIntegrationSymbol, ACTkEditorGlobalStuff.ConditionalSymbols.ThirdPartyIntegration, "Enable to let other third-party code in project know you have ACTk added.");
						DrawSymbol(ref symbolsData.excludeObfuscation, ACTkEditorGlobalStuff.ConditionalSymbols.ExcludeObfuscation, "Enable if you use Unity-unaware obfuscators which support ObfuscationAttribute to help avoid names corruption.");
						DrawSymbol(ref symbolsData.preventReadPhoneState, ACTkEditorGlobalStuff.ConditionalSymbols.PreventReadPhoneState, "Disables ObscuredPrefs Lock To Device functionality.");
						DrawSymbol(ref symbolsData.preventInternetPermission, ACTkEditorGlobalStuff.ConditionalSymbols.PreventInternetPermission, "Disables TimeCheatingDetector functionality.");
						DrawSymbol(ref symbolsData.obscuredAutoMigration, ACTkEditorGlobalStuff.ConditionalSymbols.ObscuredAutoMigration, "Enables automatic migration of ObscuredFloat and ObscuredDouble instances from the ACTk 1.5.2.0-1.5.8.0 to the 1.5.9.0+ format. Reduces these types performance a bit.");
					}
				}

				GUILayout.Space(3);
			}
		}

		private void DrawSymbol(ref bool field, string symbol, string hint)
		{
			EditorGUI.BeginChangeCheck();
			field = GUILayout.Toggle(field, new GUIContent(symbol, hint));
			if (EditorGUI.EndChangeCheck())
			{
				if (field)
				{
					SetSymbol(symbol);
				}
				else
				{
					RemoveSymbol(symbol);
				}

				symbolsData = GetSymbolsData();
			}
		}

		private static SymbolsData GetSymbolsData()
		{
			var result = new SymbolsData();

			var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
			foreach (var buildTargetGroup in groups)
			{
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

				result.injectionDebug |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.InjectionDebug);
				result.injectionDebugVerbose |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.InjectionDebugVerbose);
				result.injectionDebugParanoid |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.InjectionDebugParanoid);
				result.wallhackDebug |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.WallhackDebug);
				result.detectionBacklogs |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.DetectionBacklogs);
				result.excludeObfuscation |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.ExcludeObfuscation);
				result.preventReadPhoneState |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.PreventReadPhoneState);
				result.preventInternetPermission |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.PreventInternetPermission);
				result.obscuredAutoMigration |= GetSymbol(symbols, ACTkEditorGlobalStuff.ConditionalSymbols.ObscuredAutoMigration);
			}

			return result;
		}

		public static bool GetSymbol(string symbols, string symbol)
		{
			var result = false;

			if (symbols == symbol)
			{
				result = true;
			}
			else if (symbols.StartsWith(symbol + ';'))
			{
				result = true;
			}
			else if (symbols.EndsWith(';' + symbol))
			{
				result = true;
			}
			else if (symbols.Contains(';' + symbol + ';'))
			{
				result = true;
			}

			return result;
		}

		private static void SetSymbol(string symbol)
		{
			var names = Enum.GetNames(typeof(BuildTargetGroup));
			foreach (var n in names)
			{
				if (IsBuildTargetGroupNameObsolete(n)) continue;

				var buildTargetGroup = (BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), n);
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;
				

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
				if (symbols.Length == 0)
				{
					symbols = symbol;
				}
				else
				{
					if (symbols.EndsWith(";"))
					{
						symbols += symbol;
					}
					else
					{
						symbols += ';' + symbol;
					}
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
			}
		}

		private static void RemoveSymbol(string symbol)
		{
			var names = Enum.GetNames(typeof(BuildTargetGroup));
			foreach (var n in names)
			{
				if (IsBuildTargetGroupNameObsolete(n)) continue;
				var buildTargetGroup = (BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), n);
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

				if (symbols == symbol)
				{
					symbols = string.Empty;
				}
				else if (symbols.StartsWith(symbol + ';'))
				{
					symbols = symbols.Remove(0, symbol.Length + 1);
				}
				else if (symbols.EndsWith(';' + symbol))
				{
					symbols = symbols.Remove(symbols.LastIndexOf(';' + symbol, StringComparison.Ordinal), symbol.Length + 1);
				}
				else if (symbols.Contains(';' + symbol + ';'))
				{
					symbols = symbols.Replace(';' + symbol + ';', ";");
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
			}
		}

		private static bool IsBuildTargetGroupNameObsolete(string name)
		{
			var fi = typeof(BuildTargetGroup).GetField(name);
			var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
			return attributes.Length > 0;
		}
	}
}
#endif