using System.Collections.Generic;
using UnityEngine;

namespace DragonBones
{
	public class UnitySlot : Slot
	{
		internal const float Z_OFFSET = 0.001f;

		private static readonly int[] TRIANGLES = new int[6]
		{
			0,
			1,
			2,
			0,
			2,
			3
		};

		private static Vector3 _helpVector3 = default(Vector3);

		internal GameObject _renderDisplay;

		internal UnityUGUIDisplay _uiDisplay;

		internal MeshBuffer _meshBuffer;

		internal MeshRenderer _meshRenderer;

		internal MeshFilter _meshFilter;

		internal bool _isIgnoreCombineMesh;

		internal bool _isCombineMesh;

		internal int _sumMeshIndex = -1;

		internal int _verticeOrder = -1;

		internal int _verticeOffset = -1;

		internal UnityCombineMeshs _combineMesh;

		internal bool _isActive;

		private bool _skewed;

		private UnityArmatureComponent _proxy;

		private BlendMode _currentBlendMode;

		public Mesh mesh
		{
			get
			{
				if (_meshBuffer == null)
				{
					return null;
				}
				return _meshBuffer.sharedMesh;
			}
		}

		public MeshRenderer meshRenderer => _meshRenderer;

		public UnityTextureAtlasData currentTextureAtlasData
		{
			get
			{
				if (_textureData == null || _textureData.parent == null)
				{
					return null;
				}
				return _textureData.parent as UnityTextureAtlasData;
			}
		}

		public GameObject renderDisplay => _renderDisplay;

		public UnityArmatureComponent proxy => _proxy;

		public bool isIgnoreCombineMesh => _isIgnoreCombineMesh;

		protected override void _OnClear()
		{
			base._OnClear();
			if (_meshBuffer != null)
			{
				_meshBuffer.Dispose();
			}
			_skewed = false;
			_proxy = null;
			_renderDisplay = null;
			_uiDisplay = null;
			_meshBuffer = null;
			_meshRenderer = null;
			_meshFilter = null;
			_isIgnoreCombineMesh = false;
			_isCombineMesh = false;
			_sumMeshIndex = -1;
			_verticeOrder = -1;
			_verticeOffset = -1;
			_combineMesh = null;
			_currentBlendMode = BlendMode.Normal;
			_isActive = false;
		}

		protected override void _InitDisplay(object value, bool isRetain)
		{
		}

		protected override void _DisposeDisplay(object value, bool isRelease)
		{
			if (!isRelease)
			{
				UnityFactoryHelper.DestroyUnityObject(value as GameObject);
			}
		}

		protected override void _OnUpdateDisplay()
		{
			_renderDisplay = (((_display == null) ? _rawDisplay : _display) as GameObject);
			_proxy = (_armature.proxy as UnityArmatureComponent);
			if (_proxy.isUGUI)
			{
				_uiDisplay = _renderDisplay.GetComponent<UnityUGUIDisplay>();
				if (_uiDisplay == null)
				{
					_uiDisplay = _renderDisplay.AddComponent<UnityUGUIDisplay>();
					_uiDisplay.raycastTarget = false;
				}
			}
			else
			{
				_meshRenderer = _renderDisplay.GetComponent<MeshRenderer>();
				if (_meshRenderer == null)
				{
					_meshRenderer = _renderDisplay.AddComponent<MeshRenderer>();
				}
				_meshFilter = _renderDisplay.GetComponent<MeshFilter>();
				if (_meshFilter == null && _renderDisplay.GetComponent<TextMesh>() == null)
				{
					_meshFilter = _renderDisplay.AddComponent<MeshFilter>();
				}
			}
			if (_meshBuffer == null)
			{
				_meshBuffer = new MeshBuffer();
				_meshBuffer.sharedMesh = MeshBuffer.GenerateMesh();
				_meshBuffer.sharedMesh.name = base.name;
			}
		}

		protected override void _AddDisplay()
		{
			_proxy = (_armature.proxy as UnityArmatureComponent);
			UnityArmatureComponent proxy = _proxy;
			if (_renderDisplay.transform.parent != proxy.transform)
			{
				_renderDisplay.transform.SetParent(proxy.transform);
				_helpVector3.Set(0f, 0f, 0f);
				_SetZorder(_helpVector3);
			}
		}

		protected override void _ReplaceDisplay(object value)
		{
			UnityArmatureComponent proxy = _proxy;
			GameObject gameObject = value as GameObject;
			int siblingIndex = gameObject.transform.GetSiblingIndex();
			gameObject.SetActive(value: false);
			_renderDisplay.hideFlags = HideFlags.None;
			_renderDisplay.transform.SetParent(proxy.transform);
			_renderDisplay.SetActive(value: true);
			_renderDisplay.transform.SetSiblingIndex(siblingIndex);
			_SetZorder(gameObject.transform.localPosition);
		}

		protected override void _RemoveDisplay()
		{
			_renderDisplay.transform.parent = null;
		}

		protected override void _UpdateZOrder()
		{
			_SetZorder(_renderDisplay.transform.localPosition);
			if (_childArmature != null || !_isActive)
			{
				_CombineMesh();
			}
		}

		internal void _SetZorder(Vector3 zorderPos)
		{
			if (_isCombineMesh)
			{
				MeshBuffer meshBuffer = _combineMesh.meshBuffers[_sumMeshIndex];
				meshBuffer.zorderDirty = true;
			}
			zorderPos.z = (float)(-_zOrder) * (_proxy._zSpace + 0.001f);
			if (!(_renderDisplay != null))
			{
				return;
			}
			_renderDisplay.transform.localPosition = zorderPos;
			_renderDisplay.transform.SetSiblingIndex(_zOrder);
			if (_proxy.isUGUI)
			{
				return;
			}
			if (_childArmature == null)
			{
				_meshRenderer.sortingLayerName = _proxy.sortingLayerName;
				if (_proxy.sortingMode == SortingMode.SortByOrder)
				{
					_meshRenderer.sortingOrder = _zOrder * 10;
				}
				else
				{
					_meshRenderer.sortingOrder = _proxy.sortingOrder;
				}
				return;
			}
			UnityArmatureComponent unityArmatureComponent = base.childArmature.proxy as UnityArmatureComponent;
			unityArmatureComponent._sortingMode = _proxy._sortingMode;
			unityArmatureComponent._sortingLayerName = _proxy._sortingLayerName;
			if (_proxy._sortingMode == SortingMode.SortByOrder)
			{
				unityArmatureComponent.sortingOrder = _zOrder * 10;
			}
			else
			{
				unityArmatureComponent.sortingOrder = _proxy._sortingOrder;
			}
		}

		public void DisallowCombineMesh()
		{
			CancelCombineMesh();
			_isIgnoreCombineMesh = true;
		}

		internal void CancelCombineMesh()
		{
			if (_isCombineMesh)
			{
				_isCombineMesh = false;
				if (_meshFilter != null)
				{
					_meshFilter.sharedMesh = _meshBuffer.sharedMesh;
					bool flag = _deformVertices != null && _deformVertices.verticesData != null && _deformVertices.verticesData.weight != null;
					if (!flag)
					{
						_meshBuffer.rawVertextBuffers.CopyTo(_meshBuffer.vertexBuffers, 0);
					}
					_meshBuffer.UpdateVertices();
					_meshBuffer.UpdateColors();
					if (flag)
					{
						_UpdateMesh();
						_IdentityTransform();
					}
					else
					{
						_UpdateTransform();
					}
				}
				_meshBuffer.enabled = true;
			}
			if (_renderDisplay != null)
			{
				if (_childArmature != null)
				{
					_renderDisplay.SetActive(value: true);
				}
				else
				{
					_renderDisplay.SetActive(_isActive);
				}
				_renderDisplay.hideFlags = HideFlags.None;
			}
			_isCombineMesh = false;
			_sumMeshIndex = -1;
			_verticeOrder = -1;
			_verticeOffset = -1;
		}

		private void _CombineMesh()
		{
			if (!_isIgnoreCombineMesh && !_proxy.isUGUI)
			{
				if (_isCombineMesh)
				{
					CancelCombineMesh();
					_isIgnoreCombineMesh = true;
				}
				UnityCombineMeshs component = _proxy.GetComponent<UnityCombineMeshs>();
				if (component != null)
				{
					component.dirty = true;
				}
			}
		}

		internal override void _UpdateVisible()
		{
			_renderDisplay.SetActive(_parent.visible);
			if (_isCombineMesh && !_parent.visible)
			{
				_CombineMesh();
			}
		}

		internal override void _UpdateBlendMode()
		{
			if (_currentBlendMode == _blendMode)
			{
				return;
			}
			if (_childArmature == null)
			{
				if (_uiDisplay != null)
				{
					_uiDisplay.material = (_textureData as UnityTextureData).GetMaterial(_blendMode, isUGUI: true);
				}
				else
				{
					_meshRenderer.sharedMaterial = (_textureData as UnityTextureData).GetMaterial(_blendMode);
				}
				_meshBuffer.name = ((!(_uiDisplay != null)) ? _meshRenderer.sharedMaterial.name : _uiDisplay.material.name);
			}
			else
			{
				foreach (Slot slot in _childArmature.GetSlots())
				{
					slot._blendMode = _blendMode;
					slot._UpdateBlendMode();
				}
			}
			_currentBlendMode = _blendMode;
			_CombineMesh();
		}

		protected override void _UpdateColor()
		{
			if (_childArmature == null)
			{
				ColorTransform colorTransform = _proxy._colorTransform;
				if (_isCombineMesh)
				{
					MeshBuffer meshBuffer = _combineMesh.meshBuffers[_sumMeshIndex];
					for (int i = 0; i < _meshBuffer.vertexBuffers.Length; i++)
					{
						int num = _verticeOffset + i;
						_meshBuffer.color32Buffers[i].r = (byte)(_colorTransform.redMultiplier * colorTransform.redMultiplier * 255f);
						_meshBuffer.color32Buffers[i].g = (byte)(_colorTransform.greenMultiplier * colorTransform.greenMultiplier * 255f);
						_meshBuffer.color32Buffers[i].b = (byte)(_colorTransform.blueMultiplier * colorTransform.blueMultiplier * 255f);
						_meshBuffer.color32Buffers[i].a = (byte)(_colorTransform.alphaMultiplier * colorTransform.alphaMultiplier * 255f);
						meshBuffer.color32Buffers[num] = _meshBuffer.color32Buffers[i];
					}
					meshBuffer.UpdateColors();
				}
				else if (_meshBuffer.sharedMesh != null)
				{
					int j = 0;
					for (int vertexCount = _meshBuffer.sharedMesh.vertexCount; j < vertexCount; j++)
					{
						_meshBuffer.color32Buffers[j].r = (byte)(_colorTransform.redMultiplier * colorTransform.redMultiplier * 255f);
						_meshBuffer.color32Buffers[j].g = (byte)(_colorTransform.greenMultiplier * colorTransform.greenMultiplier * 255f);
						_meshBuffer.color32Buffers[j].b = (byte)(_colorTransform.blueMultiplier * colorTransform.blueMultiplier * 255f);
						_meshBuffer.color32Buffers[j].a = (byte)(_colorTransform.alphaMultiplier * colorTransform.alphaMultiplier * 255f);
					}
					_meshBuffer.UpdateColors();
				}
			}
			else
			{
				(_childArmature.proxy as UnityArmatureComponent).color = _colorTransform;
			}
		}

		protected override void _UpdateFrame()
		{
			VerticesData verticesData = (_deformVertices == null || _display != _meshDisplay) ? null : _deformVertices.verticesData;
			UnityTextureData unityTextureData = _textureData as UnityTextureData;
			_meshBuffer.Clear();
			_isActive = false;
			if (_displayIndex >= 0 && _display != null && unityTextureData != null)
			{
				Material material = (!_proxy.isUGUI) ? currentTextureAtlasData.texture : currentTextureAtlasData.uiTexture;
				if (material != null)
				{
					_isActive = true;
					int num = (!((float)(double)currentTextureAtlasData.width > 0f)) ? material.mainTexture.width : ((int)currentTextureAtlasData.width);
					int num2 = (!((float)(double)currentTextureAtlasData.height > 0f)) ? material.mainTexture.height : ((int)currentTextureAtlasData.height);
					float num3 = _armature.armatureData.scale * unityTextureData.parent.scale;
					float x = unityTextureData.region.x;
					float y = unityTextureData.region.y;
					float width = unityTextureData.region.width;
					float height = unityTextureData.region.height;
					if (verticesData != null)
					{
						DragonBonesData data = verticesData.data;
						int offset = verticesData.offset;
						short[] intArray = data.intArray;
						float[] floatArray = data.floatArray;
						short num4 = intArray[offset];
						short num5 = intArray[offset + 1];
						int num6 = intArray[offset + 2];
						if (num6 < 0)
						{
							num6 += 65536;
						}
						int num7 = num6 + num4 * 2;
						if (_meshBuffer.uvBuffers == null || _meshBuffer.uvBuffers.Length != num4)
						{
							_meshBuffer.uvBuffers = new Vector2[num4];
						}
						if (_meshBuffer.rawVertextBuffers == null || _meshBuffer.rawVertextBuffers.Length != num4)
						{
							_meshBuffer.rawVertextBuffers = new Vector3[num4];
							_meshBuffer.vertexBuffers = new Vector3[num4];
						}
						_meshBuffer.triangleBuffers = new int[num5 * 3];
						int i = 0;
						int num8 = num6;
						int num9 = num7;
						for (int num10 = num4; i < num10; i++)
						{
							_meshBuffer.uvBuffers[i].x = (x + floatArray[num9++] * width) / (float)num;
							_meshBuffer.uvBuffers[i].y = 1f - (y + floatArray[num9++] * height) / (float)num2;
							_meshBuffer.rawVertextBuffers[i].x = floatArray[num8++] * num3;
							_meshBuffer.rawVertextBuffers[i].y = floatArray[num8++] * num3;
							_meshBuffer.vertexBuffers[i].x = _meshBuffer.rawVertextBuffers[i].x;
							_meshBuffer.vertexBuffers[i].y = _meshBuffer.rawVertextBuffers[i].y;
						}
						for (int j = 0; j < num5 * 3; j++)
						{
							_meshBuffer.triangleBuffers[j] = intArray[offset + 4 + j];
						}
						if (verticesData.weight != null)
						{
							_IdentityTransform();
						}
					}
					else
					{
						if (_meshBuffer.rawVertextBuffers == null || _meshBuffer.rawVertextBuffers.Length != 4)
						{
							_meshBuffer.rawVertextBuffers = new Vector3[4];
							_meshBuffer.vertexBuffers = new Vector3[4];
						}
						if (_meshBuffer.uvBuffers == null || _meshBuffer.uvBuffers.Length != _meshBuffer.rawVertextBuffers.Length)
						{
							_meshBuffer.uvBuffers = new Vector2[_meshBuffer.rawVertextBuffers.Length];
						}
						int k = 0;
						for (int num15 = 4; k < num15; k++)
						{
							float num16 = 0f;
							float num17 = 0f;
							switch (k)
							{
							case 1:
								num16 = 1f;
								break;
							case 2:
								num16 = 1f;
								num17 = 1f;
								break;
							case 3:
								num17 = 1f;
								break;
							}
							float num18 = width * num3;
							float num19 = height * num3;
							float num20 = _pivotX;
							float num21 = _pivotY;
							if (unityTextureData.rotated)
							{
								float num22 = num18;
								num18 = num19;
								num19 = num22;
								num20 = num18 - _pivotX;
								num21 = num19 - _pivotY;
								_meshBuffer.uvBuffers[k].x = (x + (1f - num17) * width) / (float)num;
								_meshBuffer.uvBuffers[k].y = 1f - (y + num16 * height) / (float)num2;
							}
							else
							{
								_meshBuffer.uvBuffers[k].x = (x + num16 * width) / (float)num;
								_meshBuffer.uvBuffers[k].y = 1f - (y + num17 * height) / (float)num2;
							}
							_meshBuffer.rawVertextBuffers[k].x = num16 * num18 - num20;
							_meshBuffer.rawVertextBuffers[k].y = (1f - num17) * num19 - num21;
							_meshBuffer.vertexBuffers[k].x = _meshBuffer.rawVertextBuffers[k].x;
							_meshBuffer.vertexBuffers[k].y = _meshBuffer.rawVertextBuffers[k].y;
						}
						_meshBuffer.triangleBuffers = TRIANGLES;
					}
					if (_proxy.isUGUI)
					{
						_uiDisplay.material = material;
						_uiDisplay.texture = material.mainTexture;
						_uiDisplay.sharedMesh = _meshBuffer.sharedMesh;
					}
					else
					{
						_meshFilter.sharedMesh = _meshBuffer.sharedMesh;
						_meshRenderer.sharedMaterial = material;
					}
					_meshBuffer.name = material.name;
					_meshBuffer.InitMesh();
					_currentBlendMode = BlendMode.Normal;
					_blendModeDirty = true;
					_colorDirty = true;
					_visibleDirty = true;
					_CombineMesh();
					return;
				}
			}
			_renderDisplay.SetActive(_isActive);
			if (_proxy.isUGUI)
			{
				_uiDisplay.material = null;
				_uiDisplay.texture = null;
				_uiDisplay.sharedMesh = null;
			}
			else
			{
				_meshFilter.sharedMesh = null;
				_meshRenderer.sharedMaterial = null;
			}
			_helpVector3.x = 0f;
			_helpVector3.y = 0f;
			Vector3 localPosition = _renderDisplay.transform.localPosition;
			_helpVector3.z = localPosition.z;
			_renderDisplay.transform.localPosition = _helpVector3;
			if (_isCombineMesh)
			{
				_CombineMesh();
			}
		}

		protected override void _IdentityTransform()
		{
			UnityEngine.Transform transform = _renderDisplay.transform;
			UnityEngine.Transform transform2 = transform;
			Vector3 localPosition = transform.localPosition;
			transform2.localPosition = new Vector3(0f, 0f, localPosition.z);
			transform.localEulerAngles = Vector3.zero;
			transform.localScale = Vector3.one;
		}

		protected override void _UpdateMesh()
		{
			if (_meshBuffer.sharedMesh == null || _deformVertices == null)
			{
				return;
			}
			float scale = _armature.armatureData.scale;
			List<float> vertices = _deformVertices.vertices;
			List<Bone> bones = _deformVertices.bones;
			bool flag = vertices.Count > 0;
			VerticesData verticesData = _deformVertices.verticesData;
			WeightData weight = verticesData.weight;
			DragonBonesData data = verticesData.data;
			short[] intArray = data.intArray;
			float[] floatArray = data.floatArray;
			short num = intArray[verticesData.offset];
			if (weight != null)
			{
				int num2 = intArray[weight.offset + 1];
				if (num2 < 0)
				{
					num2 += 65536;
				}
				MeshBuffer meshBuffer = null;
				if (_isCombineMesh)
				{
					meshBuffer = _combineMesh.meshBuffers[_sumMeshIndex];
				}
				int num3 = weight.offset + 2 + weight.bones.Count;
				int num4 = num2;
				int num5 = 0;
				for (int i = 0; i < num; i++)
				{
					short num7 = intArray[num3++];
					float num8 = 0f;
					float num9 = 0f;
					for (int j = 0; j < num7; j++)
					{
						short index = intArray[num3++];
						Bone bone = bones[index];
						if (bone != null)
						{
							Matrix globalTransformMatrix = bone.globalTransformMatrix;
							float num12 = floatArray[num4++];
							float num14 = floatArray[num4++] * scale;
							float num16 = floatArray[num4++] * scale;
							if (flag)
							{
								num14 += vertices[num5++];
								num16 += vertices[num5++];
							}
							num8 += (globalTransformMatrix.a * num14 + globalTransformMatrix.c * num16 + globalTransformMatrix.tx) * num12;
							num9 += (globalTransformMatrix.b * num14 + globalTransformMatrix.d * num16 + globalTransformMatrix.ty) * num12;
						}
					}
					_meshBuffer.vertexBuffers[i].x = num8;
					_meshBuffer.vertexBuffers[i].y = num9;
					if (meshBuffer != null)
					{
						meshBuffer.vertexBuffers[i + _verticeOffset].x = num8;
						meshBuffer.vertexBuffers[i + _verticeOffset].y = num9;
					}
				}
				if (meshBuffer != null)
				{
					meshBuffer.vertexDirty = true;
				}
				else
				{
					_meshBuffer.UpdateVertices();
				}
			}
			else
			{
				if (vertices.Count <= 0)
				{
					return;
				}
				int num19 = data.intArray[verticesData.offset + 2];
				if (num19 < 0)
				{
					num19 += 65536;
				}
				float a = base.globalTransformMatrix.a;
				float b = base.globalTransformMatrix.b;
				float c = base.globalTransformMatrix.c;
				float d = base.globalTransformMatrix.d;
				float tx = base.globalTransformMatrix.tx;
				float ty = base.globalTransformMatrix.ty;
				int num20 = 0;
				float num21 = 0f;
				float num22 = 0f;
				float num23 = 0f;
				float num24 = 0f;
				MeshBuffer meshBuffer2 = null;
				if (_isCombineMesh)
				{
					meshBuffer2 = _combineMesh.meshBuffers[_sumMeshIndex];
				}
				int k = 0;
				int num25 = 0;
				int num26 = 0;
				for (int num27 = num; k < num27; k++)
				{
					num21 = data.floatArray[num19 + num25++] * scale + vertices[num26++];
					num22 = data.floatArray[num19 + num25++] * scale + vertices[num26++];
					_meshBuffer.rawVertextBuffers[k].x = num21;
					_meshBuffer.rawVertextBuffers[k].y = 0f - num22;
					_meshBuffer.vertexBuffers[k].x = num21;
					_meshBuffer.vertexBuffers[k].y = 0f - num22;
					if (meshBuffer2 != null)
					{
						num20 = k + _verticeOffset;
						num23 = num21 * a + num22 * c + tx;
						num24 = num21 * b + num22 * d + ty;
						meshBuffer2.vertexBuffers[num20].x = num23;
						meshBuffer2.vertexBuffers[num20].y = num24;
					}
				}
				if (meshBuffer2 != null)
				{
					meshBuffer2.vertexDirty = true;
				}
				else
				{
					_meshBuffer.UpdateVertices();
				}
			}
		}

		protected override void _UpdateTransform()
		{
			if (_isCombineMesh)
			{
				float a = globalTransformMatrix.a;
				float b = globalTransformMatrix.b;
				float c = globalTransformMatrix.c;
				float d = globalTransformMatrix.d;
				float tx = globalTransformMatrix.tx;
				float ty = globalTransformMatrix.ty;
				int num = 0;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				MeshBuffer meshBuffer = _combineMesh.meshBuffers[_sumMeshIndex];
				int i = 0;
				for (int num6 = _meshBuffer.vertexBuffers.Length; i < num6; i++)
				{
					num = i + _verticeOffset;
					num2 = _meshBuffer.rawVertextBuffers[i].x;
					num3 = 0f - _meshBuffer.rawVertextBuffers[i].y;
					num4 = num2 * a + num3 * c + tx;
					num5 = num2 * b + num3 * d + ty;
					_meshBuffer.vertexBuffers[i].x = num4;
					_meshBuffer.vertexBuffers[i].y = num5;
					meshBuffer.vertexBuffers[num].x = num4;
					meshBuffer.vertexBuffers[num].y = num5;
				}
				meshBuffer.vertexDirty = true;
			}
			else
			{
				UpdateGlobalTransform();
				bool flipX = _armature.flipX;
				bool flipY = _armature.flipY;
				UnityEngine.Transform transform = _renderDisplay.transform;
				_helpVector3.x = global.x;
				_helpVector3.y = global.y;
				Vector3 localPosition = transform.localPosition;
				_helpVector3.z = localPosition.z;
				transform.localPosition = _helpVector3;
				if (_childArmature == null)
				{
					_helpVector3.x = ((!flipY) ? 0f : 180f);
					_helpVector3.y = ((!flipX) ? 0f : 180f);
					_helpVector3.z = global.rotation * Transform.RAD_DEG;
				}
				else
				{
					_helpVector3.x = 0f;
					_helpVector3.y = 0f;
					_helpVector3.z = global.rotation * Transform.RAD_DEG;
					if (flipX != flipY)
					{
						_helpVector3.z = 0f - _helpVector3.z;
					}
				}
				if (flipX || flipY)
				{
					if (flipX && flipY)
					{
						_helpVector3.z += 180f;
					}
					else if (flipX)
					{
						_helpVector3.z = 180f - _helpVector3.z;
					}
					else
					{
						_helpVector3.z = 0f - _helpVector3.z;
					}
				}
				transform.localEulerAngles = _helpVector3;
				if ((_display == _rawDisplay || _display == _meshDisplay) && _meshBuffer.sharedMesh != null)
				{
					float skew = global.skew;
					float num7 = skew;
					if (flipX && flipY)
					{
						num7 = 0f - skew + Transform.PI;
					}
					else if (!flipX && !flipY)
					{
						num7 = 0f - skew - Transform.PI;
					}
					bool flag = num7 < -0.01f || 0.01f < num7;
					if (_skewed || flag)
					{
						_skewed = flag;
						bool flag2 = global.scaleX >= 0f;
						float num8 = Mathf.Cos(num7);
						float num9 = Mathf.Sin(num7);
						float num10 = 0f;
						float num11 = 0f;
						int j = 0;
						for (int num12 = _meshBuffer.vertexBuffers.Length; j < num12; j++)
						{
							num10 = _meshBuffer.rawVertextBuffers[j].x;
							num11 = _meshBuffer.rawVertextBuffers[j].y;
							if (flag2)
							{
								_meshBuffer.vertexBuffers[j].x = num10 + num11 * num9;
							}
							else
							{
								_meshBuffer.vertexBuffers[j].x = 0f - num10 + num11 * num9;
							}
							_meshBuffer.vertexBuffers[j].y = num11 * num8;
						}
						_meshBuffer.UpdateVertices();
					}
				}
				_helpVector3.x = global.scaleX;
				_helpVector3.y = global.scaleY;
				_helpVector3.z = 1f;
				transform.localScale = _helpVector3;
			}
			if (_childArmature != null)
			{
				_childArmature.flipX = _armature.flipX;
				_childArmature.flipY = _armature.flipY;
			}
		}
	}
}
