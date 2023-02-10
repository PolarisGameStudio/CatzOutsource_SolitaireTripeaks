using MiniJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DragonBones
{
	public class BinaryDataParser : ObjectDataParser
	{
		public delegate object JsonParseDelegate(string json);

		public static JsonParseDelegate jsonParseDelegate;

		private int _binaryOffset;

		private byte[] _binary;

		private short[] _intArrayBuffer;

		private float[] _floatArrayBuffer;

		private short[] _frameIntArrayBuffer;

		private float[] _frameFloatArrayBuffer;

		private short[] _frameArrayBuffer;

		private ushort[] _timelineArrayBuffer;

		private TimelineData _ParseBinaryTimeline(TimelineType type, uint offset, TimelineData timelineData = null)
		{
			TimelineData timelineData2 = (timelineData == null) ? BaseObject.BorrowObject<TimelineData>() : timelineData;
			timelineData2.type = type;
			timelineData2.offset = offset;
			_timeline = timelineData2;
			ushort num = _timelineArrayBuffer[timelineData2.offset + 2];
			if (num == 1)
			{
				timelineData2.frameIndicesOffset = -1;
			}
			else
			{
				uint num2 = _animation.frameCount + 1;
				List<uint> frameIndices = _data.frameIndices;
				timelineData2.frameIndicesOffset = frameIndices.Count;
				frameIndices.ResizeList(frameIndices.Count + (int)num2, 0u);
				int i = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (; i < num2; i++)
				{
					if (num4 + num5 <= i && num3 < num)
					{
						num4 = _frameArrayBuffer[_animation.frameOffset + _timelineArrayBuffer[timelineData2.offset + 5 + num3]];
						num5 = ((num3 != num - 1) ? (_frameArrayBuffer[_animation.frameOffset + _timelineArrayBuffer[timelineData2.offset + 5 + num3 + 1]] - num4) : ((int)_animation.frameCount - num4));
						num3++;
					}
					frameIndices[timelineData2.frameIndicesOffset + i] = (uint)(num3 - 1);
				}
			}
			_timeline = null;
			return timelineData2;
		}

		private void _ParseVertices(Dictionary<string, object> rawData, VerticesData vertices)
		{
			vertices.offset = int.Parse(rawData["offset"].ToString());
			short num = _intArrayBuffer[vertices.offset + 3];
			if (num >= 0)
			{
				WeightData weightData = BaseObject.BorrowObject<WeightData>();
				short num2 = _intArrayBuffer[vertices.offset];
				short num3 = _intArrayBuffer[num];
				weightData.offset = num;
				for (int i = 0; i < num3; i++)
				{
					short index = _intArrayBuffer[num + 2 + i];
					weightData.AddBone(_rawBones[index]);
				}
				int num4 = num + 2 + num3;
				int num5 = 0;
				int j = 0;
				for (int num6 = num2; j < num6; j++)
				{
					short num8 = _intArrayBuffer[num4++];
					num5 += num8;
					num4 += num8;
				}
				weightData.count = num5;
				vertices.weight = weightData;
			}
		}

		protected override void _ParseMesh(Dictionary<string, object> rawData, MeshDisplayData mesh)
		{
			_ParseVertices(rawData, mesh.vertices);
		}

		protected override AnimationData _ParseAnimation(Dictionary<string, object> rawData)
		{
			AnimationData animationData = BaseObject.BorrowObject<AnimationData>();
			animationData.frameCount = (uint)Math.Max(ObjectDataParser._GetNumber(rawData, "duration", 1), 1);
			animationData.playTimes = (uint)ObjectDataParser._GetNumber(rawData, "playTimes", 1);
			animationData.duration = (float)(double)animationData.frameCount / (float)(double)_armature.frameRate;
			animationData.fadeInTime = ObjectDataParser._GetNumber(rawData, "fadeInTime", 0f);
			animationData.scale = ObjectDataParser._GetNumber(rawData, "scale", 1f);
			animationData.name = ObjectDataParser._GetString(rawData, "name", "default");
			if (animationData.name.Length == 0)
			{
				animationData.name = "default";
			}
			List<object> list = rawData["offset"] as List<object>;
			animationData.frameIntOffset = uint.Parse(list[0].ToString());
			animationData.frameFloatOffset = uint.Parse(list[1].ToString());
			animationData.frameOffset = uint.Parse(list[2].ToString());
			_animation = animationData;
			if (rawData.ContainsKey("action"))
			{
				animationData.actionTimeline = _ParseBinaryTimeline(TimelineType.Action, uint.Parse(rawData["action"].ToString()));
			}
			if (rawData.ContainsKey("zOrder"))
			{
				animationData.zOrderTimeline = _ParseBinaryTimeline(TimelineType.ZOrder, uint.Parse(rawData["zOrder"].ToString()));
			}
			if (rawData.ContainsKey("bone"))
			{
				Dictionary<string, object> dictionary = rawData["bone"] as Dictionary<string, object>;
				foreach (string key in dictionary.Keys)
				{
					List<object> list2 = dictionary[key] as List<object>;
					BoneData bone = _armature.GetBone(key);
					if (bone != null)
					{
						int i = 0;
						for (int count = list2.Count; i < count; i += 2)
						{
							int type = int.Parse(list2[i].ToString());
							int offset = int.Parse(list2[i + 1].ToString());
							TimelineData tiemline = _ParseBinaryTimeline((TimelineType)type, (uint)offset);
							_animation.AddBoneTimeline(bone, tiemline);
						}
					}
				}
			}
			if (rawData.ContainsKey("slot"))
			{
				Dictionary<string, object> dictionary2 = rawData["slot"] as Dictionary<string, object>;
				foreach (string key2 in dictionary2.Keys)
				{
					List<object> list3 = dictionary2[key2] as List<object>;
					SlotData slot = _armature.GetSlot(key2);
					if (slot != null)
					{
						int j = 0;
						for (int count2 = list3.Count; j < count2; j += 2)
						{
							int type2 = int.Parse(list3[j].ToString());
							int offset2 = int.Parse(list3[j + 1].ToString());
							TimelineData timeline = _ParseBinaryTimeline((TimelineType)type2, (uint)offset2);
							_animation.AddSlotTimeline(slot, timeline);
						}
					}
				}
			}
			if (rawData.ContainsKey("constraint"))
			{
				Dictionary<string, object> dictionary3 = rawData["constraint"] as Dictionary<string, object>;
				foreach (string key3 in dictionary3.Keys)
				{
					List<object> list4 = dictionary3[key3] as List<object>;
					ConstraintData constraint = _armature.GetConstraint(key3);
					if (constraint != null)
					{
						int k = 0;
						for (int count3 = list4.Count; k < count3; k += 2)
						{
							int type3 = int.Parse(list4[k].ToString());
							int offset3 = int.Parse(list4[k + 1].ToString());
							TimelineData timeline2 = _ParseBinaryTimeline((TimelineType)type3, (uint)offset3);
							_animation.AddConstraintTimeline(constraint, timeline2);
						}
					}
				}
			}
			_animation = null;
			return animationData;
		}

		protected override void _ParseArray(Dictionary<string, object> rawData)
		{
			List<object> list = rawData["offset"] as List<object>;
			int offset = int.Parse(list[0].ToString());
			int num = int.Parse(list[1].ToString());
			int num2 = int.Parse(list[3].ToString());
			int num3 = int.Parse(list[5].ToString());
			int num4 = int.Parse(list[7].ToString());
			int num5 = int.Parse(list[9].ToString());
			int num6 = int.Parse(list[11].ToString());
			short[] intArrayBuffer = new short[0];
			float[] floatArrayBuffer = new float[0];
			short[] frameIntArrayBuffer = new short[0];
			float[] frameFloatArrayBuffer = new float[0];
			short[] frameArrayBuffer = new short[0];
			ushort[] timelineArrayBuffer = new ushort[0];
			using (MemoryStream memoryStream = new MemoryStream(_binary))
			{
				using (BinaryDataReader binaryDataReader = new BinaryDataReader(memoryStream))
				{
					binaryDataReader.Seek(_binaryOffset, SeekOrigin.Begin);
					intArrayBuffer = binaryDataReader.ReadInt16s(offset, num / Helper.INT16_SIZE);
					floatArrayBuffer = binaryDataReader.ReadSingles(0, num2 / Helper.FLOAT_SIZE);
					frameIntArrayBuffer = binaryDataReader.ReadInt16s(0, num3 / Helper.INT16_SIZE);
					frameFloatArrayBuffer = binaryDataReader.ReadSingles(0, num4 / Helper.FLOAT_SIZE);
					frameArrayBuffer = binaryDataReader.ReadInt16s(0, num5 / Helper.INT16_SIZE);
					timelineArrayBuffer = binaryDataReader.ReadUInt16s(0, num6 / Helper.UINT16_SIZE);
					binaryDataReader.Close();
					memoryStream.Close();
				}
			}
			_data.binary = _binary;
			_intArrayBuffer = intArrayBuffer;
			_floatArrayBuffer = floatArrayBuffer;
			_frameIntArrayBuffer = frameIntArrayBuffer;
			_frameFloatArrayBuffer = frameFloatArrayBuffer;
			_frameArrayBuffer = frameArrayBuffer;
			_timelineArrayBuffer = timelineArrayBuffer;
			_data.intArray = _intArrayBuffer;
			_data.floatArray = _floatArrayBuffer;
			_data.frameIntArray = _frameIntArrayBuffer;
			_data.frameFloatArray = _frameFloatArrayBuffer;
			_data.frameArray = _frameArrayBuffer;
			_data.timelineArray = _timelineArrayBuffer;
		}

		public override DragonBonesData ParseDragonBonesData(object rawObj, float scale = 1f)
		{
			Helper.Assert(rawObj != null && rawObj is byte[], "Data error.");
			byte[] array = rawObj as byte[];
			int headerLength = 0;
			object rawObj2 = DeserializeBinaryJsonData(array, out headerLength, jsonParseDelegate);
			_binary = array;
			_binaryOffset = 12 + headerLength;
			jsonParseDelegate = null;
			return base.ParseDragonBonesData(rawObj2, scale);
		}

		public static Dictionary<string, object> DeserializeBinaryJsonData(byte[] bytes, out int headerLength, JsonParseDelegate jsonParse = null)
		{
			headerLength = 0;
			Dictionary<string, object> dictionary = null;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (BinaryDataReader binaryDataReader = new BinaryDataReader(memoryStream))
				{
					memoryStream.Position = 0L;
					byte[] array = binaryDataReader.ReadBytes(8);
					byte[] bytes2 = Encoding.ASCII.GetBytes("DBDT");
					if (array[0] != bytes2[0] || array[1] != bytes2[1] || array[2] != bytes2[2] || array[3] != bytes2[3])
					{
						Helper.Assert(condition: false, "Nonsupport data.");
						return null;
					}
					headerLength = (int)binaryDataReader.ReadUInt32();
					byte[] bytes3 = binaryDataReader.ReadBytes(headerLength);
					string @string = Encoding.UTF8.GetString(bytes3);
					dictionary = ((jsonParse == null) ? (Json.Deserialize(@string) as Dictionary<string, object>) : (jsonParse(@string) as Dictionary<string, object>));
					binaryDataReader.Close();
					memoryStream.Close();
					return dictionary;
				}
			}
		}
	}
}
