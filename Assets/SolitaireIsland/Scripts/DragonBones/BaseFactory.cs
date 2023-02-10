using System;
using System.Collections.Generic;

namespace DragonBones
{
	public abstract class BaseFactory
	{
		protected static ObjectDataParser _objectParser;

		protected static BinaryDataParser _binaryParser;

		public bool autoSearch;

		protected readonly Dictionary<string, DragonBonesData> _dragonBonesDataMap = new Dictionary<string, DragonBonesData>();

		protected readonly Dictionary<string, List<TextureAtlasData>> _textureAtlasDataMap = new Dictionary<string, List<TextureAtlasData>>();

		public DragonBones _dragonBones;

		protected DataParser _dataParser;

		public WorldClock clock => _dragonBones.clock;

		public BaseFactory(DataParser dataParser = null)
		{
			if (_objectParser == null)
			{
				_objectParser = new ObjectDataParser();
			}
			if (_binaryParser == null)
			{
				_binaryParser = new BinaryDataParser();
			}
			_dataParser = ((dataParser == null) ? _objectParser : dataParser);
		}

		protected bool _IsSupportMesh()
		{
			return true;
		}

		protected TextureData _GetTextureData(string textureAtlasName, string textureName)
		{
			if (_textureAtlasDataMap.ContainsKey(textureAtlasName))
			{
				foreach (TextureAtlasData item in _textureAtlasDataMap[textureAtlasName])
				{
					TextureData texture = item.GetTexture(textureName);
					if (texture != null)
					{
						return texture;
					}
				}
			}
			if (autoSearch)
			{
				foreach (List<TextureAtlasData> value in _textureAtlasDataMap.Values)
				{
					foreach (TextureAtlasData item2 in value)
					{
						if (item2.autoSearch)
						{
							TextureData texture2 = item2.GetTexture(textureName);
							if (texture2 != null)
							{
								return texture2;
							}
						}
					}
				}
			}
			return null;
		}

		protected bool _FillBuildArmaturePackage(BuildArmaturePackage dataPackage, string dragonBonesName, string armatureName, string skinName, string textureAtlasName)
		{
			DragonBonesData dragonBonesData = null;
			ArmatureData armatureData = null;
			bool flag = !string.IsNullOrEmpty(dragonBonesName);
			if (flag && _dragonBonesDataMap.ContainsKey(dragonBonesName))
			{
				dragonBonesData = _dragonBonesDataMap[dragonBonesName];
				armatureData = dragonBonesData.GetArmature(armatureName);
			}
			if (armatureData == null && (!flag || autoSearch))
			{
				foreach (string key in _dragonBonesDataMap.Keys)
				{
					dragonBonesData = _dragonBonesDataMap[key];
					if (!flag || dragonBonesData.autoSearch)
					{
						armatureData = dragonBonesData.GetArmature(armatureName);
						if (armatureData != null)
						{
							dragonBonesName = key;
							break;
						}
					}
				}
			}
			if (armatureData != null)
			{
				dataPackage.dataName = dragonBonesName;
				dataPackage.textureAtlasName = textureAtlasName;
				dataPackage.data = dragonBonesData;
				dataPackage.armature = armatureData;
				dataPackage.skin = null;
				if (!string.IsNullOrEmpty(skinName))
				{
					dataPackage.skin = armatureData.GetSkin(skinName);
					if (dataPackage.skin == null && autoSearch)
					{
						foreach (string key2 in _dragonBonesDataMap.Keys)
						{
							DragonBonesData dragonBonesData2 = _dragonBonesDataMap[key2];
							ArmatureData armature = dragonBonesData2.GetArmature(skinName);
							if (armature != null)
							{
								dataPackage.skin = armature.defaultSkin;
								break;
							}
						}
					}
				}
				if (dataPackage.skin == null)
				{
					dataPackage.skin = armatureData.defaultSkin;
				}
				return true;
			}
			return false;
		}

		protected void _BuildBones(BuildArmaturePackage dataPackage, Armature armature)
		{
			List<BoneData> sortedBones = dataPackage.armature.sortedBones;
			int i = 0;
			for (int count = sortedBones.Count; i < count; i++)
			{
				BoneData boneData = sortedBones[i];
				Bone bone = BaseObject.BorrowObject<Bone>();
				bone.Init(boneData, armature);
			}
		}

		protected void _BuildSlots(BuildArmaturePackage dataPackage, Armature armature)
		{
			SkinData skin = dataPackage.skin;
			SkinData defaultSkin = dataPackage.armature.defaultSkin;
			if (skin != null && defaultSkin != null)
			{
				Dictionary<string, List<DisplayData>> dictionary = new Dictionary<string, List<DisplayData>>();
				foreach (string key in defaultSkin.displays.Keys)
				{
					List<DisplayData> list = dictionary[key] = defaultSkin.GetDisplays(key);
				}
				if (skin != defaultSkin)
				{
					foreach (string key2 in skin.displays.Keys)
					{
						List<DisplayData> list2 = dictionary[key2] = skin.GetDisplays(key2);
					}
				}
				foreach (SlotData sortedSlot in dataPackage.armature.sortedSlots)
				{
					List<DisplayData> list3 = (!dictionary.ContainsKey(sortedSlot.name)) ? null : dictionary[sortedSlot.name];
					Slot slot = _BuildSlot(dataPackage, sortedSlot, armature);
					slot.rawDisplayDatas = list3;
					if (list3 != null)
					{
						List<object> list4 = new List<object>();
						int i = 0;
						for (int count = list3.Count; i < count; i++)
						{
							DisplayData displayData = list3[i];
							if (displayData != null)
							{
								list4.Add(_GetSlotDisplay(dataPackage, displayData, null, slot));
							}
							else
							{
								list4.Add(null);
							}
						}
						slot._SetDisplayList(list4);
					}
					slot._SetDisplayIndex(sortedSlot.displayIndex, isAnimation: true);
				}
			}
		}

		protected void _BuildConstraints(BuildArmaturePackage dataPackage, Armature armature)
		{
			Dictionary<string, ConstraintData> constraints = dataPackage.armature.constraints;
			foreach (ConstraintData value in constraints.Values)
			{
				IKConstraint iKConstraint = BaseObject.BorrowObject<IKConstraint>();
				iKConstraint.Init(value, armature);
				armature._AddConstraint(iKConstraint);
			}
		}

		protected virtual Armature _BuildChildArmature(BuildArmaturePackage dataPackage, Slot slot, DisplayData displayData)
		{
			return BuildArmature(displayData.path, (dataPackage == null) ? string.Empty : dataPackage.dataName, string.Empty, (dataPackage == null) ? string.Empty : dataPackage.textureAtlasName);
		}

		protected object _GetSlotDisplay(BuildArmaturePackage dataPackage, DisplayData displayData, DisplayData rawDisplayData, Slot slot)
		{
			string textureAtlasName = (dataPackage == null) ? displayData.parent.parent.parent.name : dataPackage.dataName;
			object result = null;
			switch (displayData.type)
			{
			case DisplayType.Image:
			{
				ImageDisplayData imageDisplayData = displayData as ImageDisplayData;
				if (imageDisplayData.texture == null)
				{
					imageDisplayData.texture = _GetTextureData(textureAtlasName, displayData.path);
				}
				else if (dataPackage != null && !string.IsNullOrEmpty(dataPackage.textureAtlasName))
				{
					imageDisplayData.texture = _GetTextureData(dataPackage.textureAtlasName, displayData.path);
				}
				result = ((rawDisplayData == null || rawDisplayData.type != DisplayType.Mesh || !_IsSupportMesh()) ? slot.rawDisplay : slot.meshDisplay);
				break;
			}
			case DisplayType.Mesh:
			{
				MeshDisplayData meshDisplayData = displayData as MeshDisplayData;
				if (meshDisplayData.texture == null)
				{
					meshDisplayData.texture = _GetTextureData(textureAtlasName, meshDisplayData.path);
				}
				else if (dataPackage != null && !string.IsNullOrEmpty(dataPackage.textureAtlasName))
				{
					meshDisplayData.texture = _GetTextureData(dataPackage.textureAtlasName, meshDisplayData.path);
				}
				result = ((!_IsSupportMesh()) ? slot.rawDisplay : slot.meshDisplay);
				break;
			}
			case DisplayType.Armature:
			{
				ArmatureDisplayData armatureDisplayData = displayData as ArmatureDisplayData;
				Armature armature = _BuildChildArmature(dataPackage, slot, displayData);
				if (armature != null)
				{
					armature.inheritAnimation = armatureDisplayData.inheritAnimation;
					if (!armature.inheritAnimation)
					{
						List<ActionData> list = (armatureDisplayData.actions.Count <= 0) ? armature.armatureData.defaultActions : armatureDisplayData.actions;
						if (list.Count > 0)
						{
							foreach (ActionData item in list)
							{
								EventObject eventObject = BaseObject.BorrowObject<EventObject>();
								EventObject.ActionDataToInstance(item, eventObject, slot.armature);
								eventObject.slot = slot;
								slot.armature._BufferAction(eventObject, append: false);
							}
						}
						else
						{
							armature.animation.Play();
						}
					}
					armatureDisplayData.armature = armature.armatureData;
				}
				result = armature;
				break;
			}
			}
			return result;
		}

		protected abstract TextureAtlasData _BuildTextureAtlasData(TextureAtlasData textureAtlasData, object textureAtlas);

		protected abstract Armature _BuildArmature(BuildArmaturePackage dataPackage);

		protected abstract Slot _BuildSlot(BuildArmaturePackage dataPackage, SlotData slotData, Armature armature);

		public DragonBonesData ParseDragonBonesData(object rawData, string name = null, float scale = 1f)
		{
			DataParser dataParser = (!(rawData is byte[])) ? _dataParser : _binaryParser;
			DragonBonesData dragonBonesData = dataParser.ParseDragonBonesData(rawData, scale);
			TextureAtlasData textureAtlasData;
			while (true)
			{
				textureAtlasData = _BuildTextureAtlasData(null, null);
				if (!dataParser.ParseTextureAtlasData(null, textureAtlasData, scale))
				{
					break;
				}
				AddTextureAtlasData(textureAtlasData, name);
			}
			textureAtlasData.ReturnToPool();
			if (dragonBonesData != null)
			{
				AddDragonBonesData(dragonBonesData, name);
			}
			return dragonBonesData;
		}

		public TextureAtlasData ParseTextureAtlasData(Dictionary<string, object> rawData, object textureAtlas, string name = null, float scale = 1f)
		{
			TextureAtlasData textureAtlasData = _BuildTextureAtlasData(null, null);
			_dataParser.ParseTextureAtlasData(rawData, textureAtlasData, scale);
			_BuildTextureAtlasData(textureAtlasData, textureAtlas);
			AddTextureAtlasData(textureAtlasData, name);
			return textureAtlasData;
		}

		public void UpdateTextureAtlasData(string name, List<object> textureAtlases)
		{
			List<TextureAtlasData> textureAtlasData = GetTextureAtlasData(name);
			if (textureAtlasData == null)
			{
				return;
			}
			int i = 0;
			for (int count = textureAtlasData.Count; i < count; i++)
			{
				if (i < textureAtlases.Count)
				{
					_BuildTextureAtlasData(textureAtlasData[i], textureAtlases[i]);
				}
			}
		}

		public DragonBonesData GetDragonBonesData(string name)
		{
			return (!_dragonBonesDataMap.ContainsKey(name)) ? null : _dragonBonesDataMap[name];
		}

		public void AddDragonBonesData(DragonBonesData data, string name = null)
		{
			name = (string.IsNullOrEmpty(name) ? data.name : name);
			if (_dragonBonesDataMap.ContainsKey(name))
			{
				if (_dragonBonesDataMap[name] != data)
				{
					Helper.Assert(condition: false, "Can not add same name data: " + name);
				}
			}
			else
			{
				_dragonBonesDataMap[name] = data;
			}
		}

		public virtual void RemoveDragonBonesData(string name, bool disposeData = true)
		{
			if (_dragonBonesDataMap.ContainsKey(name))
			{
				if (disposeData)
				{
					_dragonBones.BufferObject(_dragonBonesDataMap[name]);
				}
				_dragonBonesDataMap.Remove(name);
			}
		}

		public List<TextureAtlasData> GetTextureAtlasData(string name)
		{
			return (!_textureAtlasDataMap.ContainsKey(name)) ? null : _textureAtlasDataMap[name];
		}

		public void AddTextureAtlasData(TextureAtlasData data, string name = null)
		{
			name = (string.IsNullOrEmpty(name) ? data.name : name);
			object list;
			if (_textureAtlasDataMap.ContainsKey(name))
			{
				list = _textureAtlasDataMap[name];
			}
			else
			{
				List<TextureAtlasData> list2 = new List<TextureAtlasData>();
				_textureAtlasDataMap[name] = list2;
				list = list2;
			}
			List<TextureAtlasData> list3 = (List<TextureAtlasData>)list;
			if (!list3.Contains(data))
			{
				list3.Add(data);
			}
		}

		public virtual void RemoveTextureAtlasData(string name, bool disposeData = true)
		{
			if (_textureAtlasDataMap.ContainsKey(name))
			{
				List<TextureAtlasData> list = _textureAtlasDataMap[name];
				if (disposeData)
				{
					foreach (TextureAtlasData item in list)
					{
						_dragonBones.BufferObject(item);
					}
				}
				_textureAtlasDataMap.Remove(name);
			}
		}

		public virtual ArmatureData GetArmatureData(string name, string dragonBonesName = "")
		{
			BuildArmaturePackage buildArmaturePackage = new BuildArmaturePackage();
			if (!_FillBuildArmaturePackage(buildArmaturePackage, dragonBonesName, name, string.Empty, string.Empty))
			{
				return null;
			}
			return buildArmaturePackage.armature;
		}

		public virtual void Clear(bool disposeData = true)
		{
			if (disposeData)
			{
				foreach (DragonBonesData value in _dragonBonesDataMap.Values)
				{
					_dragonBones.BufferObject(value);
				}
				foreach (List<TextureAtlasData> value2 in _textureAtlasDataMap.Values)
				{
					foreach (TextureAtlasData item in value2)
					{
						_dragonBones.BufferObject(item);
					}
				}
			}
			_dragonBonesDataMap.Clear();
			_textureAtlasDataMap.Clear();
		}

		public virtual Armature BuildArmature(string armatureName, string dragonBonesName = "", string skinName = null, string textureAtlasName = null)
		{
			BuildArmaturePackage dataPackage = new BuildArmaturePackage();
			if (!_FillBuildArmaturePackage(dataPackage, dragonBonesName, armatureName, skinName, textureAtlasName))
			{
				Helper.Assert(condition: false, "No armature data: " + armatureName + ", " + ((!(dragonBonesName != string.Empty)) ? string.Empty : dragonBonesName));
				return null;
			}
			Armature armature = _BuildArmature(dataPackage);
			_BuildBones(dataPackage, armature);
			_BuildSlots(dataPackage, armature);
			_BuildConstraints(dataPackage, armature);
			armature.InvalidUpdate(null, updateSlot: true);
			armature.AdvanceTime(0f);
			return armature;
		}

		public virtual void ReplaceDisplay(Slot slot, DisplayData displayData, int displayIndex = -1)
		{
			if (displayIndex < 0)
			{
				displayIndex = slot.displayIndex;
			}
			if (displayIndex < 0)
			{
				displayIndex = 0;
			}
			slot.ReplaceDisplayData(displayData, displayIndex);
			List<object> displayList = slot.displayList;
			if (displayList.Count <= displayIndex)
			{
				displayList.ResizeList(displayIndex + 1);
				int i = 0;
				for (int count = displayList.Count; i < count; i++)
				{
					displayList[i] = null;
				}
			}
			if (displayData != null)
			{
				List<DisplayData> rawDisplayDatas = slot.rawDisplayDatas;
				DisplayData rawDisplayData = null;
				if (rawDisplayDatas != null && displayIndex < rawDisplayDatas.Count)
				{
					rawDisplayData = rawDisplayDatas[displayIndex];
				}
				displayList[displayIndex] = _GetSlotDisplay(null, displayData, rawDisplayData, slot);
			}
			else
			{
				displayList[displayIndex] = null;
			}
			slot.displayList = displayList;
		}

		public bool ReplaceSlotDisplay(string dragonBonesName, string armatureName, string slotName, string displayName, Slot slot, int displayIndex = -1)
		{
			ArmatureData armatureData = GetArmatureData(armatureName, dragonBonesName);
			if (armatureData == null || armatureData.defaultSkin == null)
			{
				return false;
			}
			DisplayData display = armatureData.defaultSkin.GetDisplay(slotName, displayName);
			if (display == null)
			{
				return false;
			}
			ReplaceDisplay(slot, display, displayIndex);
			return true;
		}

		public bool ReplaceSlotDisplayList(string dragonBonesName, string armatureName, string slotName, Slot slot)
		{
			ArmatureData armatureData = GetArmatureData(armatureName, dragonBonesName);
			if (armatureData == null || armatureData.defaultSkin == null)
			{
				return false;
			}
			List<DisplayData> displays = armatureData.defaultSkin.GetDisplays(slotName);
			if (displays == null)
			{
				return false;
			}
			int num = 0;
			int i = 0;
			for (int count = displays.Count; i < count; i++)
			{
				DisplayData displayData = displays[i];
				ReplaceDisplay(slot, displayData, num++);
			}
			return true;
		}

		public bool ReplaceSkin(Armature armature, SkinData skin, bool isOverride = false, List<string> exclude = null)
		{
			bool result = false;
			SkinData defaultSkin = skin.parent.defaultSkin;
			foreach (Slot slot in armature.GetSlots())
			{
				if (exclude == null || !exclude.Contains(slot.name))
				{
					List<DisplayData> displays = skin.GetDisplays(slot.name);
					if (displays == null)
					{
						if (defaultSkin != null && skin != defaultSkin)
						{
							displays = defaultSkin.GetDisplays(slot.name);
						}
						if (displays == null)
						{
							if (isOverride)
							{
								slot.rawDisplayDatas = null;
								slot.displayList.Clear();
							}
							continue;
						}
					}
					int count = displays.Count;
					List<object> displayList = slot.displayList;
					displayList.ResizeList(count);
					int i = 0;
					for (int num = count; i < num; i++)
					{
						DisplayData displayData = displays[i];
						if (displayData != null)
						{
							displayList[i] = _GetSlotDisplay(null, displayData, null, slot);
						}
						else
						{
							displayList[i] = null;
						}
					}
					result = true;
					slot.rawDisplayDatas = displays;
					slot.displayList = displayList;
				}
			}
			return result;
		}

		public bool ReplaceAnimation(Armature armature, ArmatureData armatureData, bool isOverride = true)
		{
			SkinData defaultSkin = armatureData.defaultSkin;
			if (defaultSkin == null)
			{
				return false;
			}
			if (isOverride)
			{
				armature.animation.animations = armatureData.animations;
			}
			else
			{
				Dictionary<string, AnimationData> animations = armature.animation.animations;
				Dictionary<string, AnimationData> dictionary = new Dictionary<string, AnimationData>();
				foreach (string key in animations.Keys)
				{
					dictionary[key] = animations[key];
				}
				foreach (string key2 in armatureData.animations.Keys)
				{
					dictionary[key2] = armatureData.animations[key2];
				}
				armature.animation.animations = dictionary;
			}
			foreach (Slot slot in armature.GetSlots())
			{
				int num = 0;
				foreach (object display in slot.displayList)
				{
					if (display is Armature)
					{
						List<DisplayData> displays = defaultSkin.GetDisplays(slot.name);
						if (displays != null && num < displays.Count)
						{
							DisplayData displayData = displays[num];
							if (displayData != null && displayData.type == DisplayType.Armature)
							{
								ArmatureData armatureData2 = GetArmatureData(displayData.path, displayData.parent.parent.parent.name);
								if (armatureData2 != null)
								{
									ReplaceAnimation(display as Armature, armatureData2, isOverride);
								}
							}
						}
					}
				}
			}
			return true;
		}

		public Dictionary<string, DragonBonesData> GetAllDragonBonesData()
		{
			return _dragonBonesDataMap;
		}

		public Dictionary<string, List<TextureAtlasData>> GetAllTextureAtlasData()
		{
			return _textureAtlasDataMap;
		}

		[Obsolete("")]
		public bool ChangeSkin(Armature armature, SkinData skin, List<string> exclude = null)
		{
			return ReplaceSkin(armature, skin, isOverride: false, exclude);
		}
	}
}
