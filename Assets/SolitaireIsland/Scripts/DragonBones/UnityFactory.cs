using MiniJSON;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DragonBones
{
	public class UnityFactory : BaseFactory
	{
		internal const string defaultShaderName = "Sprites/Default";

		internal const string defaultUIShaderName = "UI/Default";

		internal static DragonBones _dragonBonesInstance;

		private static UnityFactory _factory;

		private static GameObject _gameObject;

		private GameObject _armatureGameObject;

		private bool _isUGUI;

		private readonly List<UnityDragonBonesData> _cacheUnityDragonBonesData = new List<UnityDragonBonesData>();

		[CompilerGenerated]
		private static BinaryDataParser.JsonParseDelegate _003C_003Ef__mg_0024cache0;

		public static UnityFactory factory
		{
			get
			{
				if (_factory == null)
				{
					_factory = new UnityFactory();
				}
				return _factory;
			}
		}

		public IEventDispatcher<EventObject> soundEventManager => _dragonBonesInstance.eventManager;

		public UnityFactory(DataParser dataParser = null)
			: base(dataParser)
		{
			Init();
		}

		private void Init()
		{
			if (Application.isPlaying)
			{
				if (_gameObject == null)
				{
					_gameObject = GameObject.Find("DragonBones Object");
					if (_gameObject == null)
					{
						_gameObject = new GameObject("DragonBones Object", typeof(ClockHandler));
						_gameObject.isStatic = true;
						_gameObject.hideFlags = HideFlags.HideInHierarchy;
					}
				}
				Object.DontDestroyOnLoad(_gameObject);
				ClockHandler component = _gameObject.GetComponent<ClockHandler>();
				if (component == null)
				{
					_gameObject.AddComponent<ClockHandler>();
				}
				DragonBoneEventDispatcher dragonBoneEventDispatcher = _gameObject.GetComponent<DragonBoneEventDispatcher>();
				if (dragonBoneEventDispatcher == null)
				{
					dragonBoneEventDispatcher = _gameObject.AddComponent<DragonBoneEventDispatcher>();
				}
				if (_dragonBonesInstance == null)
				{
					_dragonBonesInstance = new DragonBones(dragonBoneEventDispatcher);
					DragonBones.yDown = false;
				}
			}
			else if (_dragonBonesInstance == null)
			{
				_dragonBonesInstance = new DragonBones(null);
				DragonBones.yDown = false;
			}
			_dragonBones = _dragonBonesInstance;
		}

		protected override TextureAtlasData _BuildTextureAtlasData(TextureAtlasData textureAtlasData, object textureAtlas)
		{
			if (textureAtlasData != null)
			{
				if (textureAtlas != null)
				{
					(textureAtlasData as UnityTextureAtlasData).uiTexture = (textureAtlas as UnityDragonBonesData.TextureAtlas).uiMaterial;
					(textureAtlasData as UnityTextureAtlasData).texture = (textureAtlas as UnityDragonBonesData.TextureAtlas).material;
				}
			}
			else
			{
				textureAtlasData = BaseObject.BorrowObject<UnityTextureAtlasData>();
			}
			return textureAtlasData;
		}

		protected override Armature _BuildArmature(BuildArmaturePackage dataPackage)
		{
			Armature armature = BaseObject.BorrowObject<Armature>();
			GameObject gameObject = (!(_armatureGameObject == null)) ? _armatureGameObject : new GameObject(dataPackage.armature.name);
			UnityArmatureComponent unityArmatureComponent = gameObject.GetComponent<UnityArmatureComponent>();
			if (unityArmatureComponent == null)
			{
				unityArmatureComponent = gameObject.AddComponent<UnityArmatureComponent>();
				unityArmatureComponent.isUGUI = _isUGUI;
				if (unityArmatureComponent.isUGUI)
				{
					unityArmatureComponent.transform.localScale = Vector2.one * (1f / dataPackage.armature.scale);
				}
			}
			else
			{
				UnityEngine.Transform transform = gameObject.transform.Find("Slots");
				if (transform != null)
				{
					for (int num = transform.transform.childCount; num > 0; num--)
					{
						UnityEngine.Transform child = transform.transform.GetChild(num - 1);
						child.transform.SetParent(gameObject.transform, worldPositionStays: false);
					}
					UnityFactoryHelper.DestroyUnityObject(transform.gameObject);
				}
			}
			unityArmatureComponent._armature = armature;
			armature.Init(dataPackage.armature, unityArmatureComponent, gameObject, _dragonBones);
			_armatureGameObject = null;
			return armature;
		}

		protected override Armature _BuildChildArmature(BuildArmaturePackage dataPackage, Slot slot, DisplayData displayData)
		{
			string name = slot.slotData.name + " (" + displayData.path + ")";
			UnityArmatureComponent unityArmatureComponent = slot.armature.proxy as UnityArmatureComponent;
			UnityEngine.Transform transform = unityArmatureComponent.transform.Find(name);
			Armature armature = null;
			armature = ((transform == null) ? ((dataPackage == null) ? BuildArmature(displayData.path, displayData.parent.parent.parent.name) : BuildArmature(displayData.path, dataPackage.dataName)) : ((dataPackage == null) ? BuildArmatureComponent(displayData.path, null, null, null, transform.gameObject).armature : BuildArmatureComponent(displayData.path, (dataPackage == null) ? string.Empty : dataPackage.dataName, null, dataPackage.textureAtlasName, transform.gameObject).armature));
			if (armature == null)
			{
				return null;
			}
			GameObject gameObject = armature.display as GameObject;
			gameObject.GetComponent<UnityArmatureComponent>().isUGUI = unityArmatureComponent.GetComponent<UnityArmatureComponent>().isUGUI;
			gameObject.name = name;
			gameObject.transform.SetParent(unityArmatureComponent.transform, worldPositionStays: false);
			gameObject.gameObject.hideFlags = HideFlags.HideInHierarchy;
			gameObject.SetActive(value: false);
			return armature;
		}

		protected override Slot _BuildSlot(BuildArmaturePackage dataPackage, SlotData slotData, Armature armature)
		{
			UnitySlot unitySlot = BaseObject.BorrowObject<UnitySlot>();
			GameObject gameObject = armature.display as GameObject;
			UnityEngine.Transform transform = gameObject.transform.Find(slotData.name);
			GameObject gameObject2 = (!(transform == null)) ? transform.gameObject : null;
			bool flag = false;
			if (gameObject2 == null)
			{
				gameObject2 = new GameObject(slotData.name);
			}
			else if (gameObject2.hideFlags == HideFlags.None)
			{
				UnityCombineMeshs component = (armature.proxy as UnityArmatureComponent).GetComponent<UnityCombineMeshs>();
				if (component != null)
				{
					flag = !component.slotNames.Contains(slotData.name);
				}
			}
			unitySlot.Init(slotData, armature, gameObject2, gameObject2);
			if (flag)
			{
				unitySlot.DisallowCombineMesh();
			}
			return unitySlot;
		}

		public UnityArmatureComponent BuildArmatureComponent(string armatureName, string dragonBonesName = "", string skinName = "", string textureAtlasName = "", GameObject gameObject = null, bool isUGUI = false)
		{
			_armatureGameObject = gameObject;
			_isUGUI = isUGUI;
			Armature armature = BuildArmature(armatureName, dragonBonesName, skinName, textureAtlasName);
			if (armature != null)
			{
				_dragonBones.clock.Add(armature);
				GameObject gameObject2 = armature.display as GameObject;
				return gameObject2.GetComponent<UnityArmatureComponent>();
			}
			return null;
		}

		public GameObject GetTextureDisplay(string textureName, string textureAtlasName = null)
		{
			return null;
		}

		protected void _RefreshTextureAtlas(UnityTextureAtlasData textureAtlasData, bool isUGUI, bool isEditor = false)
		{
			Material material = null;
			if (isUGUI && textureAtlasData.uiTexture == null)
			{
				if (!isEditor)
				{
					material = Resources.Load<Material>(textureAtlasData.imagePath + "_UI_Mat");
				}
				if (material == null)
				{
					Texture2D texture2D = null;
					if (!isEditor)
					{
						texture2D = Resources.Load<Texture2D>(textureAtlasData.imagePath);
					}
					material = UnityFactoryHelper.GenerateMaterial("UI/Default", texture2D.name + "_UI_Mat", texture2D);
					if (textureAtlasData.width < 2)
					{
						textureAtlasData.width = (uint)texture2D.width;
					}
					if (textureAtlasData.height < 2)
					{
						textureAtlasData.height = (uint)texture2D.height;
					}
					textureAtlasData._disposeEnabled = true;
				}
				textureAtlasData.uiTexture = material;
			}
			else
			{
				if (isUGUI || !(textureAtlasData.texture == null))
				{
					return;
				}
				if (!isEditor)
				{
					material = Resources.Load<Material>(textureAtlasData.imagePath + "_Mat");
				}
				if (material == null)
				{
					Texture2D texture2D2 = null;
					if (!isEditor)
					{
						texture2D2 = Resources.Load<Texture2D>(textureAtlasData.imagePath);
					}
					material = UnityFactoryHelper.GenerateMaterial("Sprites/Default", texture2D2.name + "_Mat", texture2D2);
					if (textureAtlasData.width < 2)
					{
						textureAtlasData.width = (uint)texture2D2.width;
					}
					if (textureAtlasData.height < 2)
					{
						textureAtlasData.height = (uint)texture2D2.height;
					}
					textureAtlasData._disposeEnabled = true;
				}
				textureAtlasData.texture = material;
			}
		}

		public override void Clear(bool disposeData = true)
		{
			base.Clear(disposeData);
			_armatureGameObject = null;
			_isUGUI = false;
			_cacheUnityDragonBonesData.Clear();
		}

		public DragonBonesData LoadData(UnityDragonBonesData data, bool isUGUI = false, float armatureScale = 0.01f, float texScale = 1f)
		{
			DragonBonesData dragonBonesData = null;
			if (data.dragonBonesJSON != null)
			{
				dragonBonesData = LoadDragonBonesData(data.dragonBonesJSON, data.dataName, armatureScale);
				if (!string.IsNullOrEmpty(data.dataName) && dragonBonesData != null && data.textureAtlas != null)
				{
					List<TextureAtlasData> textureAtlasData = GetTextureAtlasData(data.dataName);
					if (textureAtlasData != null)
					{
						int i = 0;
						for (int count = textureAtlasData.Count; i < count; i++)
						{
							if (i < data.textureAtlas.Length)
							{
								UnityTextureAtlasData unityTextureAtlasData = textureAtlasData[i] as UnityTextureAtlasData;
								UnityDragonBonesData.TextureAtlas textureAtlas = data.textureAtlas[i];
								unityTextureAtlasData.uiTexture = textureAtlas.uiMaterial;
								unityTextureAtlasData.texture = textureAtlas.material;
							}
						}
					}
					else
					{
						for (int j = 0; j < data.textureAtlas.Length; j++)
						{
							LoadTextureAtlasData(data.textureAtlas[j], data.dataName, texScale, isUGUI);
						}
					}
				}
			}
			return dragonBonesData;
		}

		public DragonBonesData LoadDragonBonesData(string dragonBonesJSONPath, string name = "", float scale = 0.01f)
		{
			dragonBonesJSONPath = UnityFactoryHelper.CheckResourecdPath(dragonBonesJSONPath);
			TextAsset dragonBonesJSON = Resources.Load<TextAsset>(dragonBonesJSONPath);
			return LoadDragonBonesData(dragonBonesJSON, name);
		}

		public DragonBonesData LoadDragonBonesData(TextAsset dragonBonesJSON, string name = "", float scale = 0.01f)
		{
			if (dragonBonesJSON == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(name))
			{
				DragonBonesData dragonBonesData = GetDragonBonesData(name);
				if (dragonBonesData != null)
				{
					return dragonBonesData;
				}
			}
			DragonBonesData dragonBonesData2 = null;
			if (dragonBonesJSON.text == "DBDT")
			{
				BinaryDataParser.jsonParseDelegate = Json.Deserialize;
				dragonBonesData2 = ParseDragonBonesData(dragonBonesJSON.bytes, name, scale);
			}
			else
			{
				dragonBonesData2 = ParseDragonBonesData((Dictionary<string, object>)Json.Deserialize(dragonBonesJSON.text), name, scale);
			}
			name = (string.IsNullOrEmpty(name) ? dragonBonesData2.name : name);
			_dragonBonesDataMap[name] = dragonBonesData2;
			return dragonBonesData2;
		}

		public UnityTextureAtlasData LoadTextureAtlasData(string textureAtlasJSONPath, string name = "", float scale = 1f, bool isUGUI = false)
		{
			textureAtlasJSONPath = UnityFactoryHelper.CheckResourecdPath(textureAtlasJSONPath);
			TextAsset textAsset = Resources.Load<TextAsset>(textureAtlasJSONPath);
			if (textAsset != null)
			{
				Dictionary<string, object> rawData = (Dictionary<string, object>)Json.Deserialize(textAsset.text);
				UnityTextureAtlasData unityTextureAtlasData = ParseTextureAtlasData(rawData, null, name, scale) as UnityTextureAtlasData;
				if (unityTextureAtlasData != null)
				{
					unityTextureAtlasData.imagePath = UnityFactoryHelper.GetTextureAtlasImagePath(textureAtlasJSONPath, unityTextureAtlasData.imagePath);
					_RefreshTextureAtlas(unityTextureAtlasData, isUGUI);
				}
				return unityTextureAtlasData;
			}
			return null;
		}

		public UnityTextureAtlasData LoadTextureAtlasData(UnityDragonBonesData.TextureAtlas textureAtlas, string name, float scale = 1f, bool isUGUI = false)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(textureAtlas.textureAtlasJSON.text);
			UnityTextureAtlasData unityTextureAtlasData = ParseTextureAtlasData(dictionary, null, name, scale) as UnityTextureAtlasData;
			if (dictionary.ContainsKey("width"))
			{
				unityTextureAtlasData.width = uint.Parse(dictionary["width"].ToString());
			}
			if (dictionary.ContainsKey("height"))
			{
				unityTextureAtlasData.height = uint.Parse(dictionary["height"].ToString());
			}
			if (unityTextureAtlasData != null)
			{
				unityTextureAtlasData.uiTexture = textureAtlas.uiMaterial;
				unityTextureAtlasData.texture = textureAtlas.material;
			}
			return unityTextureAtlasData;
		}

		public void RefreshAllTextureAtlas(UnityArmatureComponent unityArmature)
		{
			foreach (List<TextureAtlasData> value in _textureAtlasDataMap.Values)
			{
				foreach (UnityTextureAtlasData item in value)
				{
					_RefreshTextureAtlas(item, unityArmature.isUGUI);
				}
			}
		}

		public override void ReplaceDisplay(Slot slot, DisplayData displayData, int displayIndex = -1)
		{
			if (displayData.type == DisplayType.Image || displayData.type == DisplayType.Mesh)
			{
				string name = displayData.parent.parent.parent.name;
				TextureData textureData = _GetTextureData(name, displayData.path);
				if (textureData != null)
				{
					UnityTextureAtlasData unityTextureAtlasData = textureData.parent as UnityTextureAtlasData;
					bool isUGUI = (slot._armature.proxy as UnityArmatureComponent).isUGUI;
					if ((isUGUI && unityTextureAtlasData.uiTexture == null) || (!isUGUI && unityTextureAtlasData.texture == null))
					{
						LogHelper.LogWarning("ugui display object and normal display object cannot be replaced with each other");
						return;
					}
				}
			}
			base.ReplaceDisplay(slot, displayData, displayIndex);
		}

		public void ReplaceSlotDisplay(string dragonBonesName, string armatureName, string slotName, string displayName, Slot slot, Texture2D texture, Material material = null, bool isUGUI = false, int displayIndex = -1)
		{
			ArmatureData armatureData = GetArmatureData(armatureName, dragonBonesName);
			if (armatureData == null || armatureData.defaultSkin == null)
			{
				return;
			}
			List<DisplayData> displays = armatureData.defaultSkin.GetDisplays(slotName);
			if (displays == null)
			{
				return;
			}
			DisplayData displayData = null;
			foreach (DisplayData item in displays)
			{
				if (item.name == displayName)
				{
					displayData = item;
					break;
				}
			}
			if (displayData != null && (displayData is ImageDisplayData || displayData is MeshDisplayData))
			{
				TextureData textureData = null;
				textureData = ((!(displayData is ImageDisplayData)) ? (displayData as MeshDisplayData).texture : (displayData as ImageDisplayData).texture);
				UnityTextureData unityTextureData = new UnityTextureData();
				unityTextureData.CopyFrom(textureData);
				unityTextureData.rotated = false;
				unityTextureData.region.x = 0f;
				unityTextureData.region.y = 0f;
				unityTextureData.region.width = texture.width;
				unityTextureData.region.height = texture.height;
				unityTextureData.frame = unityTextureData.region;
				unityTextureData.name = textureData.name;
				unityTextureData.parent = new UnityTextureAtlasData();
				unityTextureData.parent.width = (uint)texture.width;
				unityTextureData.parent.height = (uint)texture.height;
				unityTextureData.parent.scale = textureData.parent.scale;
				if (material == null)
				{
					material = ((!isUGUI) ? UnityFactoryHelper.GenerateMaterial("Sprites/Default", texture.name + "_Mat", texture) : UnityFactoryHelper.GenerateMaterial("UI/Default", texture.name + "_UI_Mat", texture));
				}
				if (isUGUI)
				{
					(unityTextureData.parent as UnityTextureAtlasData).uiTexture = material;
				}
				else
				{
					(unityTextureData.parent as UnityTextureAtlasData).texture = material;
				}
				material.mainTexture = texture;
				DisplayData displayData2 = null;
				if (displayData is ImageDisplayData)
				{
					displayData2 = new ImageDisplayData();
					displayData2.type = displayData.type;
					displayData2.name = displayData.name;
					displayData2.path = displayData.path;
					displayData2.transform.CopyFrom(displayData.transform);
					displayData2.parent = displayData.parent;
					(displayData2 as ImageDisplayData).pivot.CopyFrom((displayData as ImageDisplayData).pivot);
					(displayData2 as ImageDisplayData).texture = unityTextureData;
				}
				else if (displayData is MeshDisplayData)
				{
					displayData2 = new MeshDisplayData();
					displayData2.type = displayData.type;
					displayData2.name = displayData.name;
					displayData2.path = displayData.path;
					displayData2.transform.CopyFrom(displayData.transform);
					displayData2.parent = displayData.parent;
					(displayData2 as MeshDisplayData).texture = unityTextureData;
					(displayData2 as MeshDisplayData).vertices.inheritDeform = (displayData as MeshDisplayData).vertices.inheritDeform;
					(displayData2 as MeshDisplayData).vertices.offset = (displayData as MeshDisplayData).vertices.offset;
					(displayData2 as MeshDisplayData).vertices.data = (displayData as MeshDisplayData).vertices.data;
					(displayData2 as MeshDisplayData).vertices.weight = (displayData as MeshDisplayData).vertices.weight;
				}
				ReplaceDisplay(slot, displayData2, displayIndex);
			}
		}

		public UnityDragonBonesData GetCacheUnityDragonBonesData(string draonBonesName)
		{
			if (string.IsNullOrEmpty(draonBonesName))
			{
				return null;
			}
			for (int i = 0; i < _cacheUnityDragonBonesData.Count; i++)
			{
				if (_cacheUnityDragonBonesData[i].dataName == draonBonesName)
				{
					return _cacheUnityDragonBonesData[i];
				}
			}
			return null;
		}

		public void AddCacheUnityDragonBonesData(UnityDragonBonesData unityData)
		{
			for (int i = 0; i < _cacheUnityDragonBonesData.Count; i++)
			{
				if (_cacheUnityDragonBonesData[i].dataName == unityData.dataName)
				{
					_cacheUnityDragonBonesData[i] = unityData;
					return;
				}
			}
			_cacheUnityDragonBonesData.Add(unityData);
		}
	}
}
