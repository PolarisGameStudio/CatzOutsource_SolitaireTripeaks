using System;
using System.Collections.Generic;

namespace DragonBones
{
	internal class DeformTimelineState : SlotTimelineState
	{
		public int vertexOffset;

		private bool _dirty;

		private int _frameFloatOffset;

		private int _valueCount;

		private int _deformCount;

		private int _valueOffset;

		private readonly List<float> _current = new List<float>();

		private readonly List<float> _delta = new List<float>();

		private readonly List<float> _result = new List<float>();

		public bool test;

		protected override void _OnClear()
		{
			base._OnClear();
			vertexOffset = 0;
			_dirty = false;
			_frameFloatOffset = 0;
			_valueCount = 0;
			_deformCount = 0;
			_valueOffset = 0;
			_current.Clear();
			_delta.Clear();
			_result.Clear();
		}

		protected override void _OnArriveAtFrame()
		{
			base._OnArriveAtFrame();
			if (_timelineData != null)
			{
				long num = _animationData.frameFloatOffset + _frameValueOffset + _frameIndex * _valueCount;
				float scale = _armature._armatureData.scale;
				float[] frameFloatArray = _dragonBonesData.frameFloatArray;
				if (_tweenState == TweenState.Always)
				{
					long num2 = num + _valueCount;
					if (_frameIndex == _frameCount - 1)
					{
						num2 = _animationData.frameFloatOffset + _frameValueOffset;
					}
					for (int i = 0; i < _valueCount; i++)
					{
						List<float> delta = _delta;
						int index = i;
						float num3 = frameFloatArray[num2 + i] * scale;
						float num4 = frameFloatArray[num + i] * scale;
						_current[i] = num4;
						delta[index] = num3 - num4;
					}
				}
				else
				{
					for (int j = 0; j < _valueCount; j++)
					{
						_current[j] = frameFloatArray[num + j] * scale;
					}
				}
			}
			else
			{
				for (int k = 0; k < _valueCount; k++)
				{
					_current[k] = 0f;
				}
			}
		}

		protected override void _OnUpdateFrame()
		{
			base._OnUpdateFrame();
			_dirty = true;
			if (_tweenState != TweenState.Always)
			{
				_tweenState = TweenState.None;
			}
			for (int i = 0; i < _valueCount; i++)
			{
				_result[i] = _current[i] + _delta[i] * _tweenProgress;
			}
		}

		public override void Init(Armature armature, AnimationState animationState, TimelineData timelineData)
		{
			base.Init(armature, animationState, timelineData);
			if (_timelineData != null)
			{
				uint num = _animationData.frameIntOffset + _timelineArray[_timelineData.offset + 3];
				vertexOffset = _frameIntArray[num];
				if (vertexOffset < 0)
				{
					vertexOffset += 65536;
				}
				_deformCount = _frameIntArray[num + 1];
				_valueCount = _frameIntArray[num + 2];
				_valueOffset = _frameIntArray[num + 3];
				_frameFloatOffset = (int)_frameIntArray[num + 4] + (int)_animationData.frameFloatOffset;
			}
			else
			{
				_deformCount = ((slot._deformVertices != null) ? slot._deformVertices.vertices.Count : 0);
				_valueCount = _deformCount;
				_valueOffset = 0;
				_frameFloatOffset = 0;
			}
			_current.ResizeList(_valueCount, 0f);
			_delta.ResizeList(_valueCount, 0f);
			_result.ResizeList(_valueCount, 0f);
			for (int i = 0; i < _valueCount; i++)
			{
				_delta[i] = 0f;
			}
		}

		public override void FadeOut()
		{
			_tweenState = TweenState.None;
			_dirty = false;
		}

		public override void Update(float passedTime)
		{
			DeformVertices deformVertices = slot._deformVertices;
			if (deformVertices == null || deformVertices.verticesData == null || deformVertices.verticesData.offset != vertexOffset || (_timelineData != null && _dragonBonesData != deformVertices.verticesData.data))
			{
				return;
			}
			base.Update(passedTime);
			if (_tweenState == TweenState.None && !_dirty)
			{
				return;
			}
			List<float> vertices = deformVertices.vertices;
			if (_animationState._fadeState != 0 || _animationState._subFadeState != 0)
			{
				float num = (float)Math.Pow(_animationState._fadeProgress, 2.0);
				if (_timelineData != null)
				{
					for (int i = 0; i < _deformCount; i++)
					{
						if (i < _valueOffset)
						{
							List<float> list;
							int index;
							(list = vertices)[index = i] = list[index] + (_frameFloatArray[_frameFloatOffset + i] - vertices[i]) * num;
						}
						else if (i < _valueOffset + _valueCount)
						{
							List<float> list;
							int index2;
							(list = vertices)[index2 = i] = list[index2] + (_result[i - _valueOffset] - vertices[i]) * num;
						}
						else
						{
							List<float> list;
							int index3;
							(list = vertices)[index3 = i] = list[index3] + (_frameFloatArray[_frameFloatOffset + i - _valueCount] - vertices[i]) * num;
						}
					}
				}
				else
				{
					_deformCount = vertices.Count;
					for (int j = 0; j < _deformCount; j++)
					{
						List<float> list;
						int index4;
						(list = vertices)[index4 = j] = list[index4] + (0f - vertices[j]) * num;
					}
				}
				deformVertices.verticesDirty = true;
			}
			else
			{
				if (!_dirty)
				{
					return;
				}
				_dirty = false;
				if (_timelineData != null)
				{
					for (int k = 0; k < _deformCount; k++)
					{
						if (k < _valueOffset)
						{
							vertices[k] = _frameFloatArray[_frameFloatOffset + k];
						}
						else if (k < _valueOffset + _valueCount)
						{
							vertices[k] = _result[k - _valueOffset];
						}
						else
						{
							vertices[k] = _frameFloatArray[_frameFloatOffset + k - _valueCount];
						}
					}
				}
				else
				{
					_deformCount = vertices.Count;
					for (int l = 0; l < _deformCount; l++)
					{
						vertices[l] = 0f;
					}
				}
				deformVertices.verticesDirty = true;
			}
		}
	}
}
