using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace DragonBones
{
	public class ObjectDataParser : DataParser
	{
		protected int _rawTextureAtlasIndex;

		protected readonly List<BoneData> _rawBones = new List<BoneData>();

		protected DragonBonesData _data;

		protected ArmatureData _armature;

		protected BoneData _bone;

		protected SlotData _slot;

		protected SkinData _skin;

		protected MeshDisplayData _mesh;

		protected AnimationData _animation;

		protected TimelineData _timeline;

		protected List<object> _rawTextureAtlases;

		private int _defaultColorOffset = -1;

		private int _prevClockwise;

		private float _prevRotation;

		private readonly Matrix _helpMatrixA = new Matrix();

		private readonly Matrix _helpMatrixB = new Matrix();

		private readonly Transform _helpTransform = new Transform();

		private readonly ColorTransform _helpColorTransform = new ColorTransform();

		private readonly Point _helpPoint = new Point();

		private readonly List<float> _helpArray = new List<float>();

		private readonly List<short> _intArray = new List<short>();

		private readonly List<float> _floatArray = new List<float>();

		private readonly List<short> _frameIntArray = new List<short>();

		private readonly List<float> _frameFloatArray = new List<float>();

		private readonly List<short> _frameArray = new List<short>();

		private readonly List<ushort> _timelineArray = new List<ushort>();

		private readonly List<object> _cacheRawMeshes = new List<object>();

		private readonly List<MeshDisplayData> _cacheMeshes = new List<MeshDisplayData>();

		private readonly List<ActionFrame> _actionFrames = new List<ActionFrame>();

		private readonly Dictionary<string, List<float>> _weightSlotPose = new Dictionary<string, List<float>>();

		private readonly Dictionary<string, List<float>> _weightBonePoses = new Dictionary<string, List<float>>();

		private readonly Dictionary<string, List<uint>> _weightBoneIndices = new Dictionary<string, List<uint>>();

		private readonly Dictionary<string, List<BoneData>> _cacheBones = new Dictionary<string, List<BoneData>>();

		private readonly Dictionary<string, List<ActionData>> _slotChildActions = new Dictionary<string, List<ActionData>>();

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Func<object, short> _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cache5;

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cache6;

		[CompilerGenerated]
		private static Func<object, int> _003C_003Ef__mg_0024cache7;

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cache8;

		[CompilerGenerated]
		private static Func<object, int> _003C_003Ef__mg_0024cache9;

		[CompilerGenerated]
		private static Func<object, float> _003C_003Ef__mg_0024cacheA;

		[CompilerGenerated]
		private static Func<object, string> _003C_003Ef__mg_0024cacheB;

		protected static bool _GetBoolean(Dictionary<string, object> rawData, string key, bool defaultValue)
		{
			if (rawData.ContainsKey(key))
			{
				object obj = rawData[key];
				if (obj is bool)
				{
					return (bool)obj;
				}
				if (obj is string)
				{
					string text = obj as string;
					switch (text)
					{
					default:
						if (!(text == string.Empty) && !(text == "false") && !(text == "null") && !(text == "undefined"))
						{
							break;
						}
						goto case "0";
					case "0":
					case "NaN":
						return false;
					case null:
						break;
					}
					return true;
				}
				return Convert.ToBoolean(obj);
			}
			return defaultValue;
		}

		protected static uint _GetNumber(Dictionary<string, object> rawData, string key, uint defaultValue)
		{
			if (rawData.ContainsKey(key))
			{
				object obj = rawData[key];
				if (obj == null)
				{
					return defaultValue;
				}
				if (obj is uint)
				{
					return (uint)obj;
				}
				return Convert.ToUInt32(obj);
			}
			return defaultValue;
		}

		protected static int _GetNumber(Dictionary<string, object> rawData, string key, int defaultValue)
		{
			if (rawData.ContainsKey(key))
			{
				object obj = rawData[key];
				if (obj == null)
				{
					return defaultValue;
				}
				if (obj is int)
				{
					return (int)obj;
				}
				return Convert.ToInt32(obj);
			}
			return defaultValue;
		}

		protected static float _GetNumber(Dictionary<string, object> rawData, string key, float defaultValue)
		{
			if (rawData.ContainsKey(key))
			{
				object obj = rawData[key];
				if (obj == null)
				{
					return defaultValue;
				}
				if (obj is float)
				{
					return (float)obj;
				}
				return Convert.ToSingle(obj);
			}
			return defaultValue;
		}

		protected static string _GetString(Dictionary<string, object> rawData, string key, string defaultValue)
		{
			if (rawData.ContainsKey(key))
			{
				object obj = rawData[key];
				string text = Convert.ToString(obj);
				if (obj is string)
				{
					text = (string)obj;
				}
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return defaultValue;
		}

		private void _GetCurvePoint(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float t, Point result)
		{
			float num = 1f - t;
			float num2 = num * num;
			float num3 = t * t;
			float num4 = num * num2;
			float num5 = 3f * t * num2;
			float num6 = 3f * num * num3;
			float num7 = t * num3;
			result.x = num4 * x1 + num5 * x2 + num6 * x3 + num7 * x4;
			result.y = num4 * y1 + num5 * y2 + num6 * y3 + num7 * y4;
		}

		private void _SamplingEasingCurve(float[] curve, List<float> samples)
		{
			int num = curve.Length;
			int i = -2;
			int j = 0;
			for (int count = samples.Count; j < count; j++)
			{
				float num2;
				for (num2 = ((float)j + 1f) / ((float)count + 1f); ((i + 6 >= num) ? 1f : curve[i + 6]) < num2; i += 6)
				{
				}
				bool flag = i >= 0 && i + 6 < num;
				float x = (!flag) ? 0f : curve[i];
				float y = (!flag) ? 0f : curve[i + 1];
				float x2 = curve[i + 2];
				float y2 = curve[i + 3];
				float x3 = curve[i + 4];
				float y3 = curve[i + 5];
				float x4 = (!flag) ? 1f : curve[i + 6];
				float y4 = (!flag) ? 1f : curve[i + 7];
				float num3 = 0f;
				float num4 = 1f;
				while (num4 - num3 > 0.0001f)
				{
					float num5 = (num4 + num3) * 0.5f;
					_GetCurvePoint(x, y, x2, y2, x3, y3, x4, y4, num5, _helpPoint);
					if ((double)(num2 - _helpPoint.x) > 0.0)
					{
						num3 = num5;
					}
					else
					{
						num4 = num5;
					}
				}
				samples[j] = _helpPoint.y;
			}
		}

		private void _ParseActionDataInFrame(object rawData, int frameStart, BoneData bone = null, SlotData slot = null)
		{
			Dictionary<string, object> dictionary = rawData as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("event"))
				{
					_MergeActionFrame(dictionary["event"], frameStart, ActionType.Frame, bone, slot);
				}
				if (dictionary.ContainsKey("sound"))
				{
					_MergeActionFrame(dictionary["sound"], frameStart, ActionType.Sound, bone, slot);
				}
				if (dictionary.ContainsKey("action"))
				{
					_MergeActionFrame(dictionary["action"], frameStart, ActionType.Play, bone, slot);
				}
				if (dictionary.ContainsKey("events"))
				{
					_MergeActionFrame(dictionary["events"], frameStart, ActionType.Frame, bone, slot);
				}
				if (dictionary.ContainsKey("actions"))
				{
					_MergeActionFrame(dictionary["actions"], frameStart, ActionType.Play, bone, slot);
				}
			}
		}

		private void _MergeActionFrame(object rawData, int frameStart, ActionType type, BoneData bone = null, SlotData slot = null)
		{
			int count = _armature.actions.Count;
			List<ActionData> list = _ParseActionData(rawData, type, bone, slot);
			int num = 0;
			ActionFrame actionFrame = null;
			foreach (ActionData item in list)
			{
				_armature.AddAction(item, isDefault: false);
			}
			if (_actionFrames.Count == 0)
			{
				actionFrame = new ActionFrame();
				actionFrame.frameStart = 0;
				_actionFrames.Add(actionFrame);
				actionFrame = null;
			}
			foreach (ActionFrame actionFrame2 in _actionFrames)
			{
				if (actionFrame2.frameStart == frameStart)
				{
					actionFrame = actionFrame2;
					break;
				}
				if (actionFrame2.frameStart > frameStart)
				{
					break;
				}
				num++;
			}
			if (actionFrame == null)
			{
				actionFrame = new ActionFrame();
				actionFrame.frameStart = frameStart;
				if (num + 1 < _actionFrames.Count)
				{
					_actionFrames.Insert(num + 1, actionFrame);
				}
				else
				{
					_actionFrames.Add(actionFrame);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				actionFrame.actions.Add(count + i);
			}
		}

		private int _ParseCacheActionFrame(ActionFrame frame)
		{
			int count = _frameArray.Count;
			int count2 = frame.actions.Count;
			_frameArray.ResizeList<short>(_frameArray.Count + 1 + 1 + count2, 0);
			_frameArray[count] = (short)frame.frameStart;
			_frameArray[count + 1] = (short)count2;
			for (int i = 0; i < count2; i++)
			{
				_frameArray[count + 2 + i] = (short)frame.actions[i];
			}
			return count;
		}

		private ArmatureData _ParseArmature(Dictionary<string, object> rawData, float scale)
		{
			ArmatureData armatureData = BaseObject.BorrowObject<ArmatureData>();
			armatureData.name = _GetString(rawData, "name", string.Empty);
			armatureData.frameRate = _GetNumber(rawData, "frameRate", _data.frameRate);
			armatureData.scale = scale;
			if (rawData.ContainsKey("type") && rawData["type"] is string)
			{
				armatureData.type = DataParser._GetArmatureType((string)rawData["type"]);
			}
			else
			{
				armatureData.type = (ArmatureType)_GetNumber(rawData, "type".ToString(), 0);
			}
			if (armatureData.frameRate == 0)
			{
				armatureData.frameRate = 24u;
			}
			_armature = armatureData;
			if (rawData != null && rawData.ContainsKey("canvas"))
			{
				Dictionary<string, object> rawData2 = rawData["canvas"] as Dictionary<string, object>;
				CanvasData canvasData = BaseObject.BorrowObject<CanvasData>();
				if (rawData.ContainsKey("color"))
				{
					canvasData.hasBackground = true;
				}
				else
				{
					canvasData.hasBackground = false;
				}
				canvasData.color = _GetNumber(rawData2, "color", 0);
				canvasData.x = (float)_GetNumber(rawData2, "x", 0) * armatureData.scale;
				canvasData.y = (float)_GetNumber(rawData2, "y", 0) * armatureData.scale;
				canvasData.width = (float)_GetNumber(rawData2, "width", 0) * armatureData.scale;
				canvasData.height = (float)_GetNumber(rawData2, "height", 0) * armatureData.scale;
				armatureData.canvas = canvasData;
			}
			if (rawData.ContainsKey("aabb"))
			{
				Dictionary<string, object> rawData3 = rawData["aabb"] as Dictionary<string, object>;
				armatureData.aabb.x = _GetNumber(rawData3, "x", 0f) * armatureData.scale;
				armatureData.aabb.y = _GetNumber(rawData3, "y", 0f) * armatureData.scale;
				armatureData.aabb.width = _GetNumber(rawData3, "width", 0f) * armatureData.scale;
				armatureData.aabb.height = _GetNumber(rawData3, "height", 0f) * armatureData.scale;
			}
			if (rawData.ContainsKey("bone"))
			{
				List<object> list = rawData["bone"] as List<object>;
				foreach (Dictionary<string, object> item in list)
				{
					string text = _GetString(item, "parent", string.Empty);
					BoneData boneData = _ParseBone(item);
					if (text.Length > 0)
					{
						BoneData bone = armatureData.GetBone(text);
						if (bone != null)
						{
							boneData.parent = bone;
						}
						else
						{
							if (!_cacheBones.ContainsKey(text))
							{
								_cacheBones[text] = new List<BoneData>();
							}
							_cacheBones[text].Add(boneData);
						}
					}
					if (_cacheBones.ContainsKey(boneData.name))
					{
						foreach (BoneData item2 in _cacheBones[boneData.name])
						{
							item2.parent = boneData;
						}
						_cacheBones[boneData.name].Clear();
					}
					armatureData.AddBone(boneData);
					_rawBones.Add(boneData);
				}
			}
			if (rawData.ContainsKey("ik"))
			{
				List<object> list2 = rawData["ik"] as List<object>;
				foreach (Dictionary<string, object> item3 in list2)
				{
					ConstraintData constraintData = _ParseIKConstraint(item3);
					if (constraintData != null)
					{
						armatureData.AddConstraint(constraintData);
					}
				}
			}
			armatureData.SortBones();
			if (rawData.ContainsKey("slot"))
			{
				int num = 0;
				List<object> list3 = rawData["slot"] as List<object>;
				foreach (Dictionary<string, object> item4 in list3)
				{
					armatureData.AddSlot(_ParseSlot(item4, num++));
				}
			}
			if (rawData.ContainsKey("skin"))
			{
				List<object> list4 = rawData["skin"] as List<object>;
				foreach (Dictionary<string, object> item5 in list4)
				{
					armatureData.AddSkin(_ParseSkin(item5));
				}
			}
			int i = 0;
			for (int count = _cacheRawMeshes.Count; i < count; i++)
			{
				string text2 = _GetString(rawData, "share", string.Empty);
				if (text2.Length != 0)
				{
					string text3 = _GetString(rawData, "skin", "default");
					if (text3.Length == 0)
					{
						text3 = "default";
					}
					MeshDisplayData mesh = armatureData.GetMesh(text3, string.Empty, text2);
					if (mesh != null)
					{
						MeshDisplayData meshDisplayData = _cacheMeshes[i];
						meshDisplayData.vertices.ShareFrom(mesh.vertices);
					}
				}
			}
			if (rawData.ContainsKey("animation"))
			{
				List<object> list5 = rawData["animation"] as List<object>;
				foreach (Dictionary<string, object> item6 in list5)
				{
					AnimationData value = _ParseAnimation(item6);
					armatureData.AddAnimation(value);
				}
			}
			if (rawData.ContainsKey("defaultActions"))
			{
				List<ActionData> list6 = _ParseActionData(rawData["defaultActions"], ActionType.Play);
				foreach (ActionData item7 in list6)
				{
					armatureData.AddAction(item7, isDefault: true);
					if (item7.type == ActionType.Play)
					{
						AnimationData animation = armatureData.GetAnimation(item7.name);
						if (animation != null)
						{
							armatureData.defaultAnimation = animation;
						}
					}
				}
			}
			if (rawData.ContainsKey("actions"))
			{
				List<ActionData> list7 = _ParseActionData(rawData["actions"], ActionType.Play);
				foreach (ActionData item8 in list7)
				{
					armatureData.AddAction(item8, isDefault: false);
				}
			}
			_rawBones.Clear();
			_cacheRawMeshes.Clear();
			_cacheMeshes.Clear();
			_armature = null;
			_cacheBones.Clear();
			_slotChildActions.Clear();
			_weightSlotPose.Clear();
			_weightBonePoses.Clear();
			_weightBoneIndices.Clear();
			return armatureData;
		}

		protected BoneData _ParseBone(Dictionary<string, object> rawData)
		{
			float scale = _armature.scale;
			BoneData boneData = BaseObject.BorrowObject<BoneData>();
			boneData.inheritTranslation = _GetBoolean(rawData, "inheritTranslation", defaultValue: true);
			boneData.inheritRotation = _GetBoolean(rawData, "inheritRotation", defaultValue: true);
			boneData.inheritScale = _GetBoolean(rawData, "inheritScale", defaultValue: true);
			boneData.inheritReflection = _GetBoolean(rawData, "inheritReflection", defaultValue: true);
			boneData.length = (float)_GetNumber(rawData, "length", 0) * scale;
			boneData.name = _GetString(rawData, "name", string.Empty);
			if (rawData.ContainsKey("transform"))
			{
				_ParseTransform(rawData["transform"] as Dictionary<string, object>, boneData.transform, scale);
			}
			return boneData;
		}

		protected ConstraintData _ParseIKConstraint(Dictionary<string, object> rawData)
		{
			BoneData bone = _armature.GetBone(_GetString(rawData, "bone", string.Empty));
			if (bone == null)
			{
				return null;
			}
			BoneData bone2 = _armature.GetBone(_GetString(rawData, "target", string.Empty));
			if (bone2 == null)
			{
				return null;
			}
			IKConstraintData iKConstraintData = BaseObject.BorrowObject<IKConstraintData>();
			iKConstraintData.scaleEnabled = _GetBoolean(rawData, "scale", defaultValue: false);
			iKConstraintData.bendPositive = _GetBoolean(rawData, "bendPositive", defaultValue: true);
			iKConstraintData.weight = _GetNumber(rawData, "weight", 1f);
			iKConstraintData.name = _GetString(rawData, "name", string.Empty);
			iKConstraintData.bone = bone;
			iKConstraintData.target = bone2;
			int num = _GetNumber(rawData, "chain", 0);
			if (num > 0 && bone.parent != null)
			{
				iKConstraintData.root = bone.parent;
				iKConstraintData.bone = bone;
			}
			else
			{
				iKConstraintData.root = bone;
				iKConstraintData.bone = null;
			}
			return iKConstraintData;
		}

		private SlotData _ParseSlot(Dictionary<string, object> rawData, int zOrder)
		{
			SlotData slotData = BaseObject.BorrowObject<SlotData>();
			slotData.displayIndex = _GetNumber(rawData, "displayIndex", 0);
			slotData.zOrder = zOrder;
			slotData.name = _GetString(rawData, "name", string.Empty);
			slotData.parent = _armature.GetBone(_GetString(rawData, "parent", string.Empty));
			if (rawData.ContainsKey("blendMode") && rawData["blendMode"] is string)
			{
				slotData.blendMode = DataParser._GetBlendMode((string)rawData["blendMode"]);
			}
			else
			{
				slotData.blendMode = (BlendMode)_GetNumber(rawData, "blendMode", 0);
			}
			if (rawData.ContainsKey("color"))
			{
				slotData.color = SlotData.CreateColor();
				_ParseColorTransform(rawData["color"] as Dictionary<string, object>, slotData.color);
			}
			else
			{
				slotData.color = SlotData.DEFAULT_COLOR;
			}
			if (rawData.ContainsKey("actions"))
			{
				_slotChildActions[slotData.name] = _ParseActionData(rawData["actions"], ActionType.Play);
			}
			return slotData;
		}

		protected SkinData _ParseSkin(Dictionary<string, object> rawData)
		{
			SkinData skinData = BaseObject.BorrowObject<SkinData>();
			skinData.name = _GetString(rawData, "name", "default");
			if (rawData.ContainsKey("slot"))
			{
				List<object> list = rawData["slot"] as List<object>;
				_skin = skinData;
				foreach (Dictionary<string, object> item in list)
				{
					string slotName = _GetString(item, "name", string.Empty);
					SlotData slot = _armature.GetSlot(slotName);
					if (slot != null)
					{
						_slot = slot;
						if (item.ContainsKey("display"))
						{
							List<object> list2 = item["display"] as List<object>;
							foreach (Dictionary<string, object> item2 in list2)
							{
								skinData.AddDisplay(slotName, _ParseDisplay(item2));
							}
						}
						_slot = null;
					}
				}
				_skin = null;
			}
			return skinData;
		}

		protected DisplayData _ParseDisplay(Dictionary<string, object> rawData)
		{
			string text = _GetString(rawData, "name", string.Empty);
			string text2 = _GetString(rawData, "path", string.Empty);
			DisplayType defaultValue = DisplayType.Image;
			DisplayData displayData = null;
			switch ((!rawData.ContainsKey("type") || !(rawData["type"] is string)) ? _GetNumber(rawData, "type", (int)defaultValue) : ((int)DataParser._GetDisplayType((string)rawData["type"])))
			{
			case 0:
			{
				ImageDisplayData imageDisplayData = BaseObject.BorrowObject<ImageDisplayData>();
				displayData = imageDisplayData;
				imageDisplayData.name = text;
				imageDisplayData.path = ((text2.Length <= 0) ? text : text2);
				_ParsePivot(rawData, imageDisplayData);
				break;
			}
			case 1:
			{
				ArmatureDisplayData armatureDisplayData = BaseObject.BorrowObject<ArmatureDisplayData>();
				displayData = armatureDisplayData;
				armatureDisplayData.name = text;
				armatureDisplayData.path = ((text2.Length <= 0) ? text : text2);
				armatureDisplayData.inheritAnimation = true;
				if (rawData.ContainsKey("actions"))
				{
					List<ActionData> list = _ParseActionData(rawData["actions"], ActionType.Play);
					foreach (ActionData item in list)
					{
						armatureDisplayData.AddAction(item);
					}
				}
				else if (_slotChildActions.ContainsKey(_slot.name))
				{
					List<DisplayData> displays = _skin.GetDisplays(_slot.name);
					if ((displays != null) ? (_slot.displayIndex == displays.Count) : (_slot.displayIndex == 0))
					{
						foreach (ActionData item2 in _slotChildActions[_slot.name])
						{
							armatureDisplayData.AddAction(item2);
						}
						_slotChildActions[_slot.name].Clear();
					}
				}
				break;
			}
			case 2:
			{
				MeshDisplayData meshDisplayData = BaseObject.BorrowObject<MeshDisplayData>();
				displayData = meshDisplayData;
				meshDisplayData.vertices.inheritDeform = _GetBoolean(rawData, "inheritDeform", defaultValue: true);
				meshDisplayData.name = text;
				meshDisplayData.path = ((text2.Length <= 0) ? text : text2);
				meshDisplayData.vertices.data = _data;
				if (rawData.ContainsKey("share"))
				{
					_cacheRawMeshes.Add(rawData);
					_cacheMeshes.Add(meshDisplayData);
				}
				else
				{
					_ParseMesh(rawData, meshDisplayData);
				}
				break;
			}
			case 3:
			{
				BoundingBoxData boundingBoxData = _ParseBoundingBox(rawData);
				if (boundingBoxData != null)
				{
					BoundingBoxDisplayData boundingBoxDisplayData = BaseObject.BorrowObject<BoundingBoxDisplayData>();
					displayData = boundingBoxDisplayData;
					boundingBoxDisplayData.name = text;
					boundingBoxDisplayData.path = ((text2.Length <= 0) ? text : text2);
					boundingBoxDisplayData.boundingBox = boundingBoxData;
				}
				break;
			}
			}
			if (displayData != null && rawData.ContainsKey("transform"))
			{
				_ParseTransform(rawData["transform"] as Dictionary<string, object>, displayData.transform, _armature.scale);
			}
			return displayData;
		}

		protected void _ParsePivot(Dictionary<string, object> rawData, ImageDisplayData display)
		{
			if (rawData.ContainsKey("pivot"))
			{
				Dictionary<string, object> rawData2 = rawData["pivot"] as Dictionary<string, object>;
				display.pivot.x = _GetNumber(rawData2, "x", 0f);
				display.pivot.y = _GetNumber(rawData2, "y", 0f);
			}
			else
			{
				display.pivot.x = 0.5f;
				display.pivot.y = 0.5f;
			}
		}

		protected virtual void _ParseMesh(Dictionary<string, object> rawData, MeshDisplayData mesh)
		{
			List<float> list = (rawData["vertices"] as List<object>).ConvertAll(Convert.ToSingle);
			List<float> list2 = (rawData["uvs"] as List<object>).ConvertAll(Convert.ToSingle);
			List<short> list3 = (rawData["triangles"] as List<object>).ConvertAll(Convert.ToInt16);
			int num = list.Count / 2;
			int num2 = list3.Count / 3;
			int count = _floatArray.Count;
			int num3 = count + num * 2;
			int count2 = _intArray.Count;
			string key = _skin.name + "_" + _slot.name + "_" + mesh.name;
			mesh.vertices.offset = count2;
			_intArray.ResizeList<short>(_intArray.Count + 1 + 1 + 1 + 1 + num2 * 3, 0);
			_intArray[count2] = (short)num;
			_intArray[count2 + 1] = (short)num2;
			_intArray[count2 + 2] = (short)count;
			int i = 0;
			for (int num4 = num2 * 3; i < num4; i++)
			{
				_intArray[count2 + 4 + i] = list3[i];
			}
			_floatArray.ResizeList(_floatArray.Count + num * 2 + num * 2, 0f);
			int j = 0;
			for (int num5 = num * 2; j < num5; j++)
			{
				_floatArray[count + j] = list[j];
				_floatArray[num3 + j] = list2[j];
			}
			if (!rawData.ContainsKey("weights"))
			{
				return;
			}
			List<float> list4 = (rawData["weights"] as List<object>).ConvertAll(Convert.ToSingle);
			List<float> value = (rawData["slotPose"] as List<object>).ConvertAll(Convert.ToSingle);
			List<float> list5 = (rawData["bonePose"] as List<object>).ConvertAll(Convert.ToSingle);
			List<uint> list6 = new List<uint>();
			int num6 = list5.Count / 7;
			int count3 = _floatArray.Count;
			int num7 = (int)Math.Floor((double)list4.Count - (double)num) / 2;
			int count4 = _intArray.Count;
			WeightData weightData = BaseObject.BorrowObject<WeightData>();
			weightData.count = num7;
			weightData.offset = count4;
			list6.ResizeList(num6, 0u);
			_intArray.ResizeList<short>(_intArray.Count + 1 + 1 + num6 + num + weightData.count, 0);
			_intArray[count4 + 1] = (short)count3;
			for (int k = 0; k < num6; k++)
			{
				int num8 = (int)list5[k * 7];
				BoneData boneData = _rawBones[num8];
				weightData.AddBone(boneData);
				list6[k] = (uint)num8;
				_intArray[count4 + 2 + k] = (short)_armature.sortedBones.IndexOf(boneData);
			}
			_floatArray.ResizeList(_floatArray.Count + num7 * 3, 0f);
			_helpMatrixA.CopyFromArray(value);
			int l = 0;
			int num9 = 0;
			int num10 = count4 + 2 + num6;
			int num11 = count3;
			for (; l < num; l++)
			{
				int num12 = l * 2;
				short num14 = short.Parse(list4[num9++].ToString());
				_intArray[num10++] = num14;
				short num16 = num14;
				float x = _floatArray[count + num12];
				float y = _floatArray[count + num12 + 1];
				_helpMatrixA.TransformPoint(x, y, _helpPoint);
				x = _helpPoint.x;
				y = _helpPoint.y;
				for (int m = 0; m < num16; m++)
				{
					uint item = (uint)list4[num9++];
					int num18 = list6.IndexOf(item);
					_helpMatrixB.CopyFromArray(list5, list6.IndexOf(item) * 7 + 1);
					_helpMatrixB.Invert();
					_helpMatrixB.TransformPoint(x, y, _helpPoint);
					_intArray[num10++] = (short)num18;
					_floatArray[num11++] = list4[num9++];
					_floatArray[num11++] = _helpPoint.x;
					_floatArray[num11++] = _helpPoint.y;
				}
			}
			mesh.vertices.weight = weightData;
			_weightSlotPose[key] = value;
			_weightBonePoses[key] = list5;
		}

		protected BoundingBoxData _ParseBoundingBox(Dictionary<string, object> rawData)
		{
			BoundingBoxData boundingBoxData = null;
			BoundingBoxType defaultValue = BoundingBoxType.Rectangle;
			switch ((!rawData.ContainsKey("subType") || !(rawData["subType"] is string)) ? ((int)_GetNumber(rawData, "subType", (uint)defaultValue)) : ((int)DataParser._GetBoundingBoxType((string)rawData["subType"])))
			{
			case 0:
				boundingBoxData = BaseObject.BorrowObject<RectangleBoundingBoxData>();
				break;
			case 1:
				boundingBoxData = BaseObject.BorrowObject<EllipseBoundingBoxData>();
				break;
			case 2:
				boundingBoxData = _ParsePolygonBoundingBox(rawData);
				break;
			}
			if (boundingBoxData != null)
			{
				boundingBoxData.color = _GetNumber(rawData, "color", 0u);
				if (boundingBoxData.type == BoundingBoxType.Rectangle || boundingBoxData.type == BoundingBoxType.Ellipse)
				{
					boundingBoxData.width = _GetNumber(rawData, "width", 0f);
					boundingBoxData.height = _GetNumber(rawData, "height", 0f);
				}
			}
			return boundingBoxData;
		}

		protected PolygonBoundingBoxData _ParsePolygonBoundingBox(Dictionary<string, object> rawData)
		{
			PolygonBoundingBoxData polygonBoundingBoxData = BaseObject.BorrowObject<PolygonBoundingBoxData>();
			if (rawData.ContainsKey("vertices"))
			{
				float scale = _armature.scale;
				List<float> list = (rawData["vertices"] as List<object>).ConvertAll(Convert.ToSingle);
				List<float> vertices = polygonBoundingBoxData.vertices;
				vertices.ResizeList(list.Count, 0f);
				int i = 0;
				for (int count = list.Count; i < count; i += 2)
				{
					float num = list[i] * scale;
					float num2 = list[i + 1] * scale;
					vertices[i] = num;
					vertices[i + 1] = num2;
					if (i == 0)
					{
						polygonBoundingBoxData.x = num;
						polygonBoundingBoxData.y = num2;
						polygonBoundingBoxData.width = num;
						polygonBoundingBoxData.height = num2;
						continue;
					}
					if (num < polygonBoundingBoxData.x)
					{
						polygonBoundingBoxData.x = num;
					}
					else if (num > polygonBoundingBoxData.width)
					{
						polygonBoundingBoxData.width = num;
					}
					if (num2 < polygonBoundingBoxData.y)
					{
						polygonBoundingBoxData.y = num2;
					}
					else if (num2 > polygonBoundingBoxData.height)
					{
						polygonBoundingBoxData.height = num2;
					}
				}
				polygonBoundingBoxData.width -= polygonBoundingBoxData.x;
				polygonBoundingBoxData.height -= polygonBoundingBoxData.y;
			}
			else
			{
				Helper.Assert(condition: false, "Data error.\n Please reexport DragonBones Data to fixed the bug.");
			}
			return polygonBoundingBoxData;
		}

		protected virtual AnimationData _ParseAnimation(Dictionary<string, object> rawData)
		{
			AnimationData animationData = BaseObject.BorrowObject<AnimationData>();
			animationData.frameCount = (uint)Math.Max(_GetNumber(rawData, "duration", 1), 1);
			animationData.playTimes = (uint)_GetNumber(rawData, "playTimes", 1);
			animationData.duration = (float)(double)animationData.frameCount / (float)(double)_armature.frameRate;
			animationData.fadeInTime = _GetNumber(rawData, "fadeInTime", 0f);
			animationData.scale = _GetNumber(rawData, "scale", 1f);
			animationData.name = _GetString(rawData, "name", "default");
			if (animationData.name.Length == 0)
			{
				animationData.name = "default";
			}
			animationData.frameIntOffset = (uint)_frameIntArray.Count;
			animationData.frameFloatOffset = (uint)_frameFloatArray.Count;
			animationData.frameOffset = (uint)_frameArray.Count;
			_animation = animationData;
			if (rawData.ContainsKey("frame"))
			{
				List<object> list = rawData["frame"] as List<object>;
				int count = list.Count;
				if (count > 0)
				{
					int i = 0;
					int num = 0;
					for (; i < count; i++)
					{
						Dictionary<string, object> rawData2 = list[i] as Dictionary<string, object>;
						_ParseActionDataInFrame(rawData2, num);
						num += _GetNumber(rawData2, "duration", 1);
					}
				}
			}
			if (rawData.ContainsKey("zOrder"))
			{
				_animation.zOrderTimeline = _ParseTimeline(rawData["zOrder"] as Dictionary<string, object>, null, "frame", TimelineType.ZOrder, addIntOffset: false, addFloatOffset: false, 0u, _ParseZOrderFrame);
			}
			if (rawData.ContainsKey("bone"))
			{
				List<object> list2 = rawData["bone"] as List<object>;
				foreach (Dictionary<string, object> item in list2)
				{
					_ParseBoneTimeline(item);
				}
			}
			if (rawData.ContainsKey("slot"))
			{
				List<object> list3 = rawData["slot"] as List<object>;
				foreach (Dictionary<string, object> item2 in list3)
				{
					_ParseSlotTimeline(item2);
				}
			}
			if (rawData.ContainsKey("ffd"))
			{
				List<object> list4 = rawData["ffd"] as List<object>;
				foreach (Dictionary<string, object> item3 in list4)
				{
					string text = _GetString(item3, "skin", "default");
					string slotName = _GetString(item3, "slot", string.Empty);
					string meshName = _GetString(item3, "name", string.Empty);
					if (text.Length == 0)
					{
						text = "default";
					}
					_slot = _armature.GetSlot(slotName);
					_mesh = _armature.GetMesh(text, slotName, meshName);
					if (_slot != null && _mesh != null)
					{
						TimelineData timelineData = _ParseTimeline(item3, null, "frame", TimelineType.SlotDeform, addIntOffset: false, addFloatOffset: true, 0u, _ParseSlotFFDFrame);
						if (timelineData != null)
						{
							_animation.AddSlotTimeline(_slot, timelineData);
						}
						_slot = null;
						_mesh = null;
					}
				}
			}
			if (rawData.ContainsKey("ik"))
			{
				List<object> list5 = rawData["ik"] as List<object>;
				foreach (Dictionary<string, object> item4 in list5)
				{
					string constraintName = _GetString(item4, "name", string.Empty);
					ConstraintData constraint = _armature.GetConstraint(constraintName);
					if (constraint != null)
					{
						TimelineData timelineData2 = _ParseTimeline(item4, null, "frame", TimelineType.IKConstraint, addIntOffset: true, addFloatOffset: false, 2u, _ParseIKConstraintFrame);
						if (timelineData2 != null)
						{
							_animation.AddConstraintTimeline(constraint, timelineData2);
						}
					}
				}
			}
			if (_actionFrames.Count > 0)
			{
				TimelineData timelineData3 = _animation.actionTimeline = BaseObject.BorrowObject<TimelineData>();
				int count2 = _actionFrames.Count;
				timelineData3.type = TimelineType.Action;
				timelineData3.offset = (uint)_timelineArray.Count;
				_timelineArray.ResizeList<ushort>(_timelineArray.Count + 1 + 1 + 1 + 1 + 1 + count2, 0);
				_timelineArray[(int)timelineData3.offset] = 100;
				_timelineArray[(int)(timelineData3.offset + 1)] = 0;
				_timelineArray[(int)(timelineData3.offset + 2)] = (ushort)count2;
				_timelineArray[(int)(timelineData3.offset + 3)] = 0;
				_timelineArray[(int)(timelineData3.offset + 4)] = 0;
				_timeline = timelineData3;
				if (count2 == 1)
				{
					timelineData3.frameIndicesOffset = -1;
					_timelineArray[(int)(timelineData3.offset + 5)] = (ushort)(_ParseCacheActionFrame(_actionFrames[0]) - _animation.frameOffset);
				}
				else
				{
					uint num2 = _animation.frameCount + 1;
					List<uint> frameIndices = _data.frameIndices;
					timelineData3.frameIndicesOffset = frameIndices.Count;
					frameIndices.ResizeList(frameIndices.Count + (int)num2, 0u);
					int j = 0;
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					for (; j < num2; j++)
					{
						if (num4 + num5 <= j && num3 < count2)
						{
							ActionFrame actionFrame = _actionFrames[num3];
							num4 = actionFrame.frameStart;
							num5 = ((num3 != count2 - 1) ? (_actionFrames[num3 + 1].frameStart - num4) : ((int)_animation.frameCount - num4));
							_timelineArray[(int)(timelineData3.offset + 5) + num3] = (ushort)(_ParseCacheActionFrame(actionFrame) - (int)_animation.frameOffset);
							num3++;
						}
						frameIndices[timelineData3.frameIndicesOffset + j] = (uint)(num3 - 1);
					}
				}
				_timeline = null;
				_actionFrames.Clear();
			}
			_animation = null;
			return animationData;
		}

		protected TimelineData _ParseTimeline(Dictionary<string, object> rawData, List<object> rawFrames, string framesKey, TimelineType type, bool addIntOffset, bool addFloatOffset, uint frameValueCount, Func<Dictionary<string, object>, int, int, int> frameParser)
		{
			if (rawData != null && framesKey.Length > 0 && rawData.ContainsKey(framesKey))
			{
				rawFrames = (rawData[framesKey] as List<object>);
			}
			if (rawFrames == null)
			{
				return null;
			}
			int count = rawFrames.Count;
			if (count == 0)
			{
				return null;
			}
			int count2 = _frameIntArray.Count;
			int count3 = _frameFloatArray.Count;
			TimelineData timelineData = BaseObject.BorrowObject<TimelineData>();
			int count4 = _timelineArray.Count;
			_timelineArray.ResizeList<ushort>(_timelineArray.Count + 1 + 1 + 1 + 1 + 1 + count, 0);
			if (rawData != null)
			{
				_timelineArray[count4] = (ushort)Math.Round(_GetNumber(rawData, "scale", 1f) * 100f);
				_timelineArray[count4 + 1] = (ushort)Math.Round(_GetNumber(rawData, "offset", 0f) * 100f);
			}
			else
			{
				_timelineArray[count4] = 100;
				_timelineArray[count4 + 1] = 0;
			}
			_timelineArray[count4 + 2] = (ushort)count;
			_timelineArray[count4 + 3] = (ushort)frameValueCount;
			if (addIntOffset)
			{
				_timelineArray[count4 + 4] = (ushort)(count2 - _animation.frameIntOffset);
			}
			else if (addFloatOffset)
			{
				_timelineArray[count4 + 4] = (ushort)(count3 - (int)_animation.frameFloatOffset);
			}
			else
			{
				_timelineArray[count4 + 4] = 0;
			}
			_timeline = timelineData;
			_timeline.type = type;
			_timeline.offset = (uint)count4;
			if (count == 1)
			{
				timelineData.frameIndicesOffset = -1;
				int num = frameParser(rawFrames[0] as Dictionary<string, object>, 0, 0);
				_timelineArray[(int)(timelineData.offset + 5)] = (ushort)(num - _animation.frameOffset);
			}
			else
			{
				List<uint> frameIndices = _data.frameIndices;
				uint num2 = _animation.frameCount + 1;
				timelineData.frameIndicesOffset = frameIndices.Count;
				frameIndices.ResizeList(frameIndices.Count + (int)num2, 0u);
				int i = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (; i < num2; i++)
				{
					if (num4 + num5 <= i && num3 < count)
					{
						Dictionary<string, object> dictionary = rawFrames[num3] as Dictionary<string, object>;
						num4 = i;
						num5 = _GetNumber(dictionary, "duration", 1);
						if (num3 == count - 1)
						{
							num5 = (int)_animation.frameCount - num4;
						}
						int num6 = frameParser(dictionary, num4, num5);
						_timelineArray[(int)(timelineData.offset + 5) + num3] = (ushort)(num6 - _animation.frameOffset);
						num3++;
					}
					frameIndices[timelineData.frameIndicesOffset + i] = (uint)(num3 - 1);
				}
			}
			_timeline = null;
			return timelineData;
		}

		protected void _ParseBoneTimeline(Dictionary<string, object> rawData)
		{
			BoneData bone = _armature.GetBone(_GetString(rawData, "name", string.Empty));
			if (bone == null)
			{
				return;
			}
			_bone = bone;
			_slot = _armature.GetSlot(_bone.name);
			if (rawData.ContainsKey("translateFrame"))
			{
				TimelineData timelineData = _ParseTimeline(rawData, null, "translateFrame", TimelineType.BoneTranslate, addIntOffset: false, addFloatOffset: true, 2u, _ParseBoneTranslateFrame);
				if (timelineData != null)
				{
					_animation.AddBoneTimeline(bone, timelineData);
				}
			}
			if (rawData.ContainsKey("rotateFrame"))
			{
				TimelineData timelineData2 = _ParseTimeline(rawData, null, "rotateFrame", TimelineType.BoneRotate, addIntOffset: false, addFloatOffset: true, 2u, _ParseBoneRotateFrame);
				if (timelineData2 != null)
				{
					_animation.AddBoneTimeline(bone, timelineData2);
				}
			}
			if (rawData.ContainsKey("scaleFrame"))
			{
				TimelineData timelineData3 = _ParseTimeline(rawData, null, "scaleFrame", TimelineType.BoneScale, addIntOffset: false, addFloatOffset: true, 2u, _ParseBoneScaleFrame);
				if (timelineData3 != null)
				{
					_animation.AddBoneTimeline(bone, timelineData3);
				}
			}
			if (rawData.ContainsKey("frame"))
			{
				TimelineData timelineData4 = _ParseTimeline(rawData, null, "frame", TimelineType.BoneAll, addIntOffset: false, addFloatOffset: true, 6u, _ParseBoneAllFrame);
				if (timelineData4 != null)
				{
					_animation.AddBoneTimeline(bone, timelineData4);
				}
			}
			_bone = null;
			_slot = null;
		}

		protected void _ParseSlotTimeline(Dictionary<string, object> rawData)
		{
			SlotData slot = _armature.GetSlot(_GetString(rawData, "name", string.Empty));
			if (slot != null)
			{
				_slot = slot;
				TimelineData timelineData = null;
				timelineData = ((!rawData.ContainsKey("displayFrame")) ? _ParseTimeline(rawData, null, "frame", TimelineType.SlotDisplay, addIntOffset: false, addFloatOffset: false, 0u, _ParseSlotDisplayFrame) : _ParseTimeline(rawData, null, "displayFrame", TimelineType.SlotDisplay, addIntOffset: false, addFloatOffset: false, 0u, _ParseSlotDisplayFrame));
				if (timelineData != null)
				{
					_animation.AddSlotTimeline(slot, timelineData);
				}
				TimelineData timelineData2 = null;
				timelineData2 = ((!rawData.ContainsKey("colorFrame")) ? _ParseTimeline(rawData, null, "frame", TimelineType.SlotColor, addIntOffset: true, addFloatOffset: false, 1u, _ParseSlotColorFrame) : _ParseTimeline(rawData, null, "colorFrame", TimelineType.SlotColor, addIntOffset: true, addFloatOffset: false, 1u, _ParseSlotColorFrame));
				if (timelineData2 != null)
				{
					_animation.AddSlotTimeline(slot, timelineData2);
				}
				_slot = null;
			}
		}

		protected int _ParseFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int count = _frameArray.Count;
			_frameArray.ResizeList<short>(_frameArray.Count + 1, 0);
			_frameArray[count] = (short)frameStart;
			return count;
		}

		protected int _ParseTweenFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int num = _ParseFrame(rawData, frameStart, frameCount);
			if (frameCount > 0)
			{
				if (rawData.ContainsKey("curve"))
				{
					int num2 = frameCount + 1;
					_helpArray.ResizeList(num2, 0f);
					List<object> list = rawData["curve"] as List<object>;
					float[] array = new float[list.Count];
					int i = 0;
					for (int count = list.Count; i < count; i++)
					{
						array[i] = Convert.ToSingle(list[i]);
					}
					_SamplingEasingCurve(array, _helpArray);
					_frameArray.ResizeList<short>(_frameArray.Count + 1 + 1 + _helpArray.Count, 0);
					_frameArray[num + 1] = 2;
					_frameArray[num + 2] = (short)num2;
					for (int j = 0; j < num2; j++)
					{
						_frameArray[num + 3 + j] = (short)Math.Round(_helpArray[j] * 10000f);
					}
				}
				else
				{
					float num3 = -2f;
					float num4 = num3;
					if (rawData.ContainsKey("tweenEasing"))
					{
						num4 = _GetNumber(rawData, "tweenEasing", num3);
					}
					if (num4 == num3)
					{
						_frameArray.ResizeList<short>(_frameArray.Count + 1, 0);
						_frameArray[num + 1] = 0;
					}
					else if (num4 == 0f)
					{
						_frameArray.ResizeList<short>(_frameArray.Count + 1, 0);
						_frameArray[num + 1] = 1;
					}
					else if (num4 < 0f)
					{
						_frameArray.ResizeList<short>(_frameArray.Count + 1 + 1, 0);
						_frameArray[num + 1] = 3;
						_frameArray[num + 2] = (short)Math.Round((0f - num4) * 100f);
					}
					else if (num4 <= 1f)
					{
						_frameArray.ResizeList<short>(_frameArray.Count + 1 + 1, 0);
						_frameArray[num + 1] = 4;
						_frameArray[num + 2] = (short)Math.Round(num4 * 100f);
					}
					else
					{
						_frameArray.ResizeList<short>(_frameArray.Count + 1 + 1, 0);
						_frameArray[num + 1] = 5;
						_frameArray[num + 2] = (short)Math.Round(num4 * 100f - 100f);
					}
				}
			}
			else
			{
				_frameArray.ResizeList<short>(_frameArray.Count + 1, 0);
				_frameArray[num + 1] = 0;
			}
			return num;
		}

		private int _ParseZOrderFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int num = _ParseFrame(rawData, frameStart, frameCount);
			if (rawData.ContainsKey("zOrder"))
			{
				List<int> list = (rawData["zOrder"] as List<object>).ConvertAll(Convert.ToInt32);
				if (list.Count > 0)
				{
					int count = _armature.sortedSlots.Count;
					int[] array = new int[count - list.Count / 2];
					int[] array2 = new int[count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = 0;
					}
					for (int j = 0; j < count; j++)
					{
						array2[j] = -1;
					}
					int num2 = 0;
					int num3 = 0;
					int k = 0;
					for (int count2 = list.Count; k < count2; k += 2)
					{
						int num4 = list[k];
						int num5 = list[k + 1];
						while (num2 != num4)
						{
							array[num3++] = num2++;
						}
						if (num2 + num5 >= 0)
						{
							int num8 = num2 + num5;
							array2[num8] = num2++;
						}
						else
						{
							num2++;
						}
					}
					while (num2 < count)
					{
						array[num3++] = num2++;
					}
					_frameArray.ResizeList<short>(_frameArray.Count + 1 + count, 0);
					_frameArray[num + 1] = (short)count;
					int num12 = count;
					while (num12-- > 0)
					{
						int num14 = 0;
						if (array2[num12] == -1)
						{
							if (num3 > 0)
							{
								num14 = array[--num3];
							}
							_frameArray[num + 2 + num12] = (short)((num14 > 0) ? num14 : 0);
						}
						else
						{
							num14 = array2[num12];
							_frameArray[num + 2 + num12] = (short)((num14 > 0) ? num14 : 0);
						}
					}
					return num;
				}
			}
			_frameArray.ResizeList<short>(_frameArray.Count + 1, 0);
			_frameArray[num + 1] = 0;
			return num;
		}

		protected int _ParseBoneAllFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			_helpTransform.Identity();
			if (rawData.ContainsKey("transform"))
			{
				_ParseTransform(rawData["transform"] as Dictionary<string, object>, _helpTransform, 1f);
			}
			float num = _helpTransform.rotation;
			if (frameStart != 0)
			{
				if (_prevClockwise == 0)
				{
					num = _prevRotation + Transform.NormalizeRadian(num - _prevRotation);
				}
				else
				{
					if ((_prevClockwise <= 0) ? (num <= _prevRotation) : (num >= _prevRotation))
					{
						_prevClockwise = ((_prevClockwise <= 0) ? (_prevClockwise + 1) : (_prevClockwise - 1));
					}
					num = _prevRotation + num - _prevRotation + Transform.PI_D * (float)_prevClockwise;
				}
			}
			_prevClockwise = _GetNumber(rawData, "tweenRotate", 0);
			_prevRotation = num;
			int result = _ParseTweenFrame(rawData, frameStart, frameCount);
			int num2 = _frameFloatArray.Count;
			_frameFloatArray.ResizeList(_frameFloatArray.Count + 6, 0f);
			_frameFloatArray[num2++] = _helpTransform.x;
			_frameFloatArray[num2++] = _helpTransform.y;
			_frameFloatArray[num2++] = num;
			_frameFloatArray[num2++] = _helpTransform.skew;
			_frameFloatArray[num2++] = _helpTransform.scaleX;
			_frameFloatArray[num2++] = _helpTransform.scaleY;
			_ParseActionDataInFrame(rawData, frameStart, _bone, _slot);
			return result;
		}

		protected int _ParseBoneTranslateFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int result = _ParseTweenFrame(rawData, frameStart, frameCount);
			int num = _frameFloatArray.Count;
			_frameFloatArray.ResizeList(_frameFloatArray.Count + 2, 0f);
			_frameFloatArray[num++] = _GetNumber(rawData, "x", 0f);
			_frameFloatArray[num++] = _GetNumber(rawData, "y", 0f);
			return result;
		}

		protected int _ParseBoneRotateFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			float num = _GetNumber(rawData, "rotate", 0f) * Transform.DEG_RAD;
			if (frameStart != 0)
			{
				if (_prevClockwise == 0)
				{
					num = _prevRotation + Transform.NormalizeRadian(num - _prevRotation);
				}
				else
				{
					if ((_prevClockwise <= 0) ? (num <= _prevRotation) : (num >= _prevRotation))
					{
						_prevClockwise = ((_prevClockwise <= 0) ? (_prevClockwise + 1) : (_prevClockwise - 1));
					}
					num = _prevRotation + num - _prevRotation + Transform.PI_D * (float)_prevClockwise;
				}
			}
			_prevClockwise = _GetNumber(rawData, "clockwise", 0);
			_prevRotation = num;
			int result = _ParseTweenFrame(rawData, frameStart, frameCount);
			int num2 = _frameFloatArray.Count;
			_frameFloatArray.ResizeList(_frameFloatArray.Count + 2, 0f);
			_frameFloatArray[num2++] = num;
			_frameFloatArray[num2++] = _GetNumber(rawData, "skew", 0f) * Transform.DEG_RAD;
			return result;
		}

		protected int _ParseBoneScaleFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int result = _ParseTweenFrame(rawData, frameStart, frameCount);
			int num = _frameFloatArray.Count;
			_frameFloatArray.ResizeList(_frameFloatArray.Count + 2, 0f);
			_frameFloatArray[num++] = _GetNumber(rawData, "x", 1f);
			_frameFloatArray[num++] = _GetNumber(rawData, "y", 1f);
			return result;
		}

		protected int _ParseSlotDisplayFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int num = _ParseFrame(rawData, frameStart, frameCount);
			_frameArray.ResizeList<short>(_frameArray.Count + 1, 0);
			if (rawData.ContainsKey("value"))
			{
				_frameArray[num + 1] = (short)_GetNumber(rawData, "value", 0);
			}
			else
			{
				_frameArray[num + 1] = (short)_GetNumber(rawData, "displayIndex", 0);
			}
			_ParseActionDataInFrame(rawData, frameStart, _slot.parent, _slot);
			return num;
		}

		protected int _ParseSlotColorFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int result = _ParseTweenFrame(rawData, frameStart, frameCount);
			int num = -1;
			if (rawData.ContainsKey("value") || rawData.ContainsKey("color"))
			{
				Dictionary<string, object> dictionary = (!rawData.ContainsKey("value")) ? (rawData["color"] as Dictionary<string, object>) : (rawData["value"] as Dictionary<string, object>);
				using (Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, object> current = enumerator.Current;
						_ParseColorTransform(dictionary, _helpColorTransform);
						num = _intArray.Count;
						_intArray.ResizeList<short>(_intArray.Count + 8, 0);
						_intArray[num++] = (short)Math.Round(_helpColorTransform.alphaMultiplier * 100f);
						_intArray[num++] = (short)Math.Round(_helpColorTransform.redMultiplier * 100f);
						_intArray[num++] = (short)Math.Round(_helpColorTransform.greenMultiplier * 100f);
						_intArray[num++] = (short)Math.Round(_helpColorTransform.blueMultiplier * 100f);
						_intArray[num++] = (short)Math.Round((float)_helpColorTransform.alphaOffset);
						_intArray[num++] = (short)Math.Round((float)_helpColorTransform.redOffset);
						_intArray[num++] = (short)Math.Round((float)_helpColorTransform.greenOffset);
						_intArray[num++] = (short)Math.Round((float)_helpColorTransform.blueOffset);
						num -= 8;
					}
				}
			}
			if (num < 0)
			{
				if (_defaultColorOffset < 0)
				{
					num = (_defaultColorOffset = _intArray.Count);
					_intArray.ResizeList<short>(_intArray.Count + 8, 0);
					_intArray[num++] = 100;
					_intArray[num++] = 100;
					_intArray[num++] = 100;
					_intArray[num++] = 100;
					_intArray[num++] = 0;
					_intArray[num++] = 0;
					_intArray[num++] = 0;
					_intArray[num++] = 0;
				}
				num = _defaultColorOffset;
			}
			int count = _frameIntArray.Count;
			_frameIntArray.ResizeList<short>(_frameIntArray.Count + 1, 0);
			_frameIntArray[count] = (short)num;
			return result;
		}

		protected int _ParseSlotFFDFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int count = _frameFloatArray.Count;
			int result = _ParseTweenFrame(rawData, frameStart, frameCount);
			List<float> list = (!rawData.ContainsKey("vertices")) ? null : (rawData["vertices"] as List<object>).ConvertAll(Convert.ToSingle);
			int num = _GetNumber(rawData, "offset", 0);
			short num2 = _intArray[_mesh.vertices.offset];
			string key = _mesh.parent.name + "_" + _slot.name + "_" + _mesh.name;
			WeightData weight = _mesh.vertices.weight;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			int num6 = 0;
			if (weight != null)
			{
				List<float> value = _weightSlotPose[key];
				_helpMatrixA.CopyFromArray(value);
				_frameFloatArray.ResizeList(_frameFloatArray.Count + weight.count * 2, 0f);
				num5 = weight.offset + 2 + weight.bones.Count;
			}
			else
			{
				_frameFloatArray.ResizeList(_frameFloatArray.Count + num2 * 2, 0f);
			}
			for (int i = 0; i < num2 * 2; i += 2)
			{
				if (list != null)
				{
					num3 = ((i >= num && i - num < list.Count) ? list[i - num] : 0f);
					num4 = ((i + 1 >= num && i + 1 - num < list.Count) ? list[i + 1 - num] : 0f);
				}
				else
				{
					num3 = 0f;
					num4 = 0f;
				}
				if (weight != null)
				{
					List<float> value2 = _weightBonePoses[key];
					short num8 = _intArray[num5++];
					_helpMatrixA.TransformPoint(num3, num4, _helpPoint, delta: true);
					num3 = _helpPoint.x;
					num4 = _helpPoint.y;
					for (int j = 0; j < num8; j++)
					{
						short num10 = _intArray[num5++];
						_helpMatrixB.CopyFromArray(value2, num10 * 7 + 1);
						_helpMatrixB.Invert();
						_helpMatrixB.TransformPoint(num3, num4, _helpPoint, delta: true);
						_frameFloatArray[count + num6++] = _helpPoint.x;
						_frameFloatArray[count + num6++] = _helpPoint.y;
					}
				}
				else
				{
					_frameFloatArray[count + i] = num3;
					_frameFloatArray[count + i + 1] = num4;
				}
			}
			if (frameStart == 0)
			{
				int count2 = _frameIntArray.Count;
				_frameIntArray.ResizeList<short>(_frameIntArray.Count + 1 + 1 + 1 + 1 + 1, 0);
				_frameIntArray[count2] = (short)_mesh.vertices.offset;
				_frameIntArray[count2 + 1] = (short)(_frameFloatArray.Count - count);
				_frameIntArray[count2 + 2] = (short)(_frameFloatArray.Count - count);
				_frameIntArray[count2 + 3] = 0;
				_frameIntArray[count2 + 4] = (short)(count - _animation.frameFloatOffset);
				_timelineArray[(int)(_timeline.offset + 3)] = (ushort)(count2 - _animation.frameIntOffset);
			}
			return result;
		}

		protected int _ParseIKConstraintFrame(Dictionary<string, object> rawData, int frameStart, int frameCount)
		{
			int result = _ParseTweenFrame(rawData, frameStart, frameCount);
			int num = _frameIntArray.Count;
			_frameIntArray.ResizeList<short>(_frameIntArray.Count + 2, 0);
			_frameIntArray[num++] = (short)(_GetBoolean(rawData, "bendPositive", defaultValue: true) ? 1 : 0);
			_frameIntArray[num++] = (short)Math.Round((double)_GetNumber(rawData, "weight", 1f) * 100.0);
			return result;
		}

		protected List<ActionData> _ParseActionData(object rawData, ActionType type, BoneData bone = null, SlotData slot = null)
		{
			List<ActionData> list = new List<ActionData>();
			if (rawData is string)
			{
				ActionData actionData = BaseObject.BorrowObject<ActionData>();
				actionData.type = type;
				actionData.name = (string)rawData;
				actionData.bone = bone;
				actionData.slot = slot;
				list.Add(actionData);
			}
			else if (rawData is IList)
			{
				List<object> list2 = rawData as List<object>;
				{
					foreach (Dictionary<string, object> item in list2)
					{
						ActionData actionData2 = BaseObject.BorrowObject<ActionData>();
						if (item.ContainsKey("gotoAndPlay"))
						{
							actionData2.type = ActionType.Play;
							actionData2.name = _GetString(item, "gotoAndPlay", string.Empty);
						}
						else
						{
							if (item.ContainsKey("type") && item["type"] is string)
							{
								actionData2.type = DataParser._GetActionType((string)item["type"]);
							}
							else
							{
								actionData2.type = (ActionType)_GetNumber(item, "type", (uint)type);
							}
							actionData2.name = _GetString(item, "name", string.Empty);
						}
						if (item.ContainsKey("bone"))
						{
							string boneName = _GetString(item, "bone", string.Empty);
							actionData2.bone = _armature.GetBone(boneName);
						}
						else
						{
							actionData2.bone = bone;
						}
						if (item.ContainsKey("slot"))
						{
							string slotName = _GetString(item, "slot", string.Empty);
							actionData2.slot = _armature.GetSlot(slotName);
						}
						else
						{
							actionData2.slot = slot;
						}
						UserData userData = null;
						if (item.ContainsKey("ints"))
						{
							if (userData == null)
							{
								userData = BaseObject.BorrowObject<UserData>();
							}
							List<int> list3 = (item["ints"] as List<object>).ConvertAll(Convert.ToInt32);
							foreach (int item2 in list3)
							{
								userData.AddInt(item2);
							}
						}
						if (item.ContainsKey("floats"))
						{
							if (userData == null)
							{
								userData = BaseObject.BorrowObject<UserData>();
							}
							List<float> list4 = (item["floats"] as List<object>).ConvertAll(Convert.ToSingle);
							foreach (float item3 in list4)
							{
								userData.AddFloat(item3);
							}
						}
						if (item.ContainsKey("strings"))
						{
							if (userData == null)
							{
								userData = BaseObject.BorrowObject<UserData>();
							}
							List<string> list5 = (item["strings"] as List<object>).ConvertAll(Convert.ToString);
							foreach (string item4 in list5)
							{
								userData.AddString(item4);
							}
						}
						actionData2.data = userData;
						list.Add(actionData2);
					}
					return list;
				}
			}
			return list;
		}

		protected void _ParseTransform(Dictionary<string, object> rawData, Transform transform, float scale)
		{
			transform.x = _GetNumber(rawData, "x", 0f) * scale;
			transform.y = _GetNumber(rawData, "y", 0f) * scale;
			if (rawData.ContainsKey("rotate") || rawData.ContainsKey("skew"))
			{
				transform.rotation = Transform.NormalizeRadian(_GetNumber(rawData, "rotate", 0f) * Transform.DEG_RAD);
				transform.skew = Transform.NormalizeRadian(_GetNumber(rawData, "skew", 0f) * Transform.DEG_RAD);
			}
			else if (rawData.ContainsKey("skX") || rawData.ContainsKey("skY"))
			{
				transform.rotation = Transform.NormalizeRadian(_GetNumber(rawData, "skY", 0f) * Transform.DEG_RAD);
				transform.skew = Transform.NormalizeRadian(_GetNumber(rawData, "skX", 0f) * Transform.DEG_RAD) - transform.rotation;
			}
			transform.scaleX = _GetNumber(rawData, "scX", 1f);
			transform.scaleY = _GetNumber(rawData, "scY", 1f);
		}

		protected void _ParseColorTransform(Dictionary<string, object> rawData, ColorTransform color)
		{
			color.alphaMultiplier = (float)_GetNumber(rawData, "aM", 100) * 0.01f;
			color.redMultiplier = (float)_GetNumber(rawData, "rM", 100) * 0.01f;
			color.greenMultiplier = (float)_GetNumber(rawData, "gM", 100) * 0.01f;
			color.blueMultiplier = (float)_GetNumber(rawData, "bM", 100) * 0.01f;
			color.alphaOffset = _GetNumber(rawData, "aO", 0);
			color.redOffset = _GetNumber(rawData, "rO", 0);
			color.greenOffset = _GetNumber(rawData, "gO", 0);
			color.blueOffset = _GetNumber(rawData, "bO", 0);
		}

		protected virtual void _ParseArray(Dictionary<string, object> rawData)
		{
			_intArray.Clear();
			_floatArray.Clear();
			_frameIntArray.Clear();
			_frameFloatArray.Clear();
			_frameArray.Clear();
			_timelineArray.Clear();
		}

		protected void _ModifyArray()
		{
			if (_intArray.Count % Helper.INT16_SIZE != 0)
			{
				_intArray.Add(0);
			}
			if (_frameIntArray.Count % Helper.INT16_SIZE != 0)
			{
				_frameIntArray.Add(0);
			}
			if (_frameArray.Count % Helper.INT16_SIZE != 0)
			{
				_frameArray.Add(0);
			}
			if (_timelineArray.Count % Helper.UINT16_SIZE != 0)
			{
				_timelineArray.Add(0);
			}
			int num = _intArray.Count * Helper.INT16_SIZE;
			int num2 = _floatArray.Count * Helper.FLOAT_SIZE;
			int num3 = _frameIntArray.Count * Helper.INT16_SIZE;
			int num4 = _frameFloatArray.Count * Helper.FLOAT_SIZE;
			int num5 = _frameArray.Count * Helper.INT16_SIZE;
			int num6 = _timelineArray.Count * Helper.UINT16_SIZE;
			int capacity = num + num2 + num3 + num4 + num5 + num6;
			using (MemoryStream memoryStream = new MemoryStream(capacity))
			{
				using (BinaryDataWriter binaryDataWriter = new BinaryDataWriter(memoryStream))
				{
					using (BinaryDataReader binaryDataReader = new BinaryDataReader(memoryStream))
					{
						binaryDataWriter.Write(_intArray.ToArray());
						binaryDataWriter.Write(_floatArray.ToArray());
						binaryDataWriter.Write(_frameIntArray.ToArray());
						binaryDataWriter.Write(_frameFloatArray.ToArray());
						binaryDataWriter.Write(_frameArray.ToArray());
						binaryDataWriter.Write(_timelineArray.ToArray());
						memoryStream.Position = 0L;
						_data.binary = memoryStream.GetBuffer();
						_data.intArray = binaryDataReader.ReadInt16s(0, _intArray.Count);
						_data.floatArray = binaryDataReader.ReadSingles(0, _floatArray.Count);
						_data.frameIntArray = binaryDataReader.ReadInt16s(0, _frameIntArray.Count);
						_data.frameFloatArray = binaryDataReader.ReadSingles(0, _frameFloatArray.Count);
						_data.frameArray = binaryDataReader.ReadInt16s(0, _frameArray.Count);
						_data.timelineArray = binaryDataReader.ReadUInt16s(0, _timelineArray.Count);
						memoryStream.Close();
					}
				}
			}
			_defaultColorOffset = -1;
		}

		public override DragonBonesData ParseDragonBonesData(object rawObj, float scale = 1f)
		{
			Dictionary<string, object> dictionary = rawObj as Dictionary<string, object>;
			Helper.Assert(dictionary != null, "Data error.");
			string text = _GetString(dictionary, "version", string.Empty);
			string item = _GetString(dictionary, "compatibleVersion", string.Empty);
			if (DataParser.DATA_VERSIONS.IndexOf(text) >= 0 || DataParser.DATA_VERSIONS.IndexOf(item) >= 0)
			{
				DragonBonesData dragonBonesData = BaseObject.BorrowObject<DragonBonesData>();
				dragonBonesData.version = text;
				dragonBonesData.name = _GetString(dictionary, "name", string.Empty);
				dragonBonesData.frameRate = _GetNumber(dictionary, "frameRate", 24u);
				if (dragonBonesData.frameRate == 0)
				{
					dragonBonesData.frameRate = 24u;
				}
				if (dictionary.ContainsKey("armature"))
				{
					_data = dragonBonesData;
					_ParseArray(dictionary);
					List<object> list = dictionary["armature"] as List<object>;
					foreach (Dictionary<string, object> item2 in list)
					{
						dragonBonesData.AddArmature(_ParseArmature(item2, scale));
					}
					if (_data.binary == null)
					{
						_ModifyArray();
					}
					if (dictionary.ContainsKey("stage"))
					{
						dragonBonesData.stage = dragonBonesData.GetArmature(_GetString(dictionary, "stage", string.Empty));
					}
					else if (dragonBonesData.armatureNames.Count > 0)
					{
						dragonBonesData.stage = dragonBonesData.GetArmature(dragonBonesData.armatureNames[0]);
					}
					_data = null;
				}
				if (dictionary.ContainsKey("textureAtlas"))
				{
					_rawTextureAtlases = (dictionary["textureAtlas"] as List<object>);
				}
				return dragonBonesData;
			}
			Helper.Assert(condition: false, "Nonsupport data version: " + text + "\nPlease convert DragonBones data to support version.\nRead more: https://github.com/DragonBones/Tools/");
			return null;
		}

		public override bool ParseTextureAtlasData(object rawObj, TextureAtlasData textureAtlasData, float scale = 1f)
		{
			Dictionary<string, object> dictionary = rawObj as Dictionary<string, object>;
			if (dictionary == null)
			{
				if (_rawTextureAtlases == null || _rawTextureAtlases.Count == 0)
				{
					return false;
				}
				object rawData = _rawTextureAtlases[_rawTextureAtlasIndex++];
				ParseTextureAtlasData(rawData, textureAtlasData, scale);
				if (_rawTextureAtlasIndex >= _rawTextureAtlases.Count)
				{
					_rawTextureAtlasIndex = 0;
					_rawTextureAtlases = null;
				}
				return true;
			}
			textureAtlasData.width = _GetNumber(dictionary, "width", 0u);
			textureAtlasData.height = _GetNumber(dictionary, "height", 0u);
			textureAtlasData.scale = ((scale != 1f) ? scale : (1f / _GetNumber(dictionary, "scale", 1f)));
			textureAtlasData.name = _GetString(dictionary, "name", string.Empty);
			textureAtlasData.imagePath = _GetString(dictionary, "imagePath", string.Empty);
			if (dictionary.ContainsKey("SubTexture"))
			{
				List<object> list = dictionary["SubTexture"] as List<object>;
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					Dictionary<string, object> rawData2 = list[i] as Dictionary<string, object>;
					TextureData textureData = textureAtlasData.CreateTexture();
					textureData.rotated = _GetBoolean(rawData2, "rotated", defaultValue: false);
					textureData.name = _GetString(rawData2, "name", string.Empty);
					textureData.region.x = _GetNumber(rawData2, "x", 0f);
					textureData.region.y = _GetNumber(rawData2, "y", 0f);
					textureData.region.width = _GetNumber(rawData2, "width", 0f);
					textureData.region.height = _GetNumber(rawData2, "height", 0f);
					float num = _GetNumber(rawData2, "frameWidth", -1f);
					float num2 = _GetNumber(rawData2, "frameHeight", -1f);
					if ((double)num > 0.0 && (double)num2 > 0.0)
					{
						textureData.frame = TextureData.CreateRectangle();
						textureData.frame.x = _GetNumber(rawData2, "frameX", 0f);
						textureData.frame.y = _GetNumber(rawData2, "frameY", 0f);
						textureData.frame.width = num;
						textureData.frame.height = num2;
					}
					textureAtlasData.AddTexture(textureData);
				}
			}
			return true;
		}
	}
}
