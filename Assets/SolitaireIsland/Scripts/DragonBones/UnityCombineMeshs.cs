using System.Collections.Generic;
using UnityEngine;

namespace DragonBones
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(UnityArmatureComponent))]
	public class UnityCombineMeshs : MonoBehaviour
	{
		[HideInInspector]
		public List<string> slotNames = new List<string>();

		[HideInInspector]
		public MeshBuffer[] meshBuffers;

		[HideInInspector]
		public bool dirty;

		private UnityArmatureComponent _unityArmature;

		private int _subSlotCount;

		private int _verticeOffset;

		private bool _isCanCombineMesh;

		private void Start()
		{
			_unityArmature = GetComponent<UnityArmatureComponent>();
			_isCanCombineMesh = true;
			dirty = true;
		}

		private void OnDestroy()
		{
			if (_unityArmature != null)
			{
				RestoreArmature(_unityArmature._armature);
			}
			if (meshBuffers != null)
			{
				for (int i = 0; i < meshBuffers.Length; i++)
				{
					MeshBuffer meshBuffer = meshBuffers[i];
					meshBuffer.Dispose();
				}
			}
			meshBuffers = null;
			dirty = false;
			_unityArmature = null;
			_subSlotCount = 0;
			_verticeOffset = -1;
			_isCanCombineMesh = false;
		}

		private void RestoreArmature(Armature armature)
		{
			if (armature != null)
			{
				foreach (UnitySlot slot in armature.GetSlots())
				{
					if (slot.childArmature == null)
					{
						slot.CancelCombineMesh();
					}
				}
			}
		}

		private void LateUpdate()
		{
			if (dirty)
			{
				BeginCombineMesh();
				dirty = false;
			}
			if (meshBuffers == null)
			{
				return;
			}
			for (int i = 0; i < meshBuffers.Length; i++)
			{
				MeshBuffer meshBuffer = meshBuffers[i];
				if (meshBuffer.zorderDirty)
				{
					meshBuffer.UpdateOrder();
					meshBuffer.zorderDirty = false;
				}
				else if (meshBuffer.vertexDirty)
				{
					meshBuffer.UpdateVertices();
					meshBuffer.vertexDirty = false;
				}
			}
		}

		public void BeginCombineMesh()
		{
			if (!_isCanCombineMesh || _unityArmature.isUGUI)
			{
				return;
			}
			_verticeOffset = 0;
			_subSlotCount = 0;
			slotNames.Clear();
			if (meshBuffers != null)
			{
				for (int i = 0; i < meshBuffers.Length; i++)
				{
					MeshBuffer meshBuffer = meshBuffers[i];
					meshBuffer.Dispose();
				}
				meshBuffers = null;
			}
			List<CombineMeshInfo> list = new List<CombineMeshInfo>();
			CollectMesh(_unityArmature.armature, list);
			meshBuffers = new MeshBuffer[list.Count];
			for (int j = 0; j < list.Count; j++)
			{
				CombineMeshInfo combineMeshInfo = list[j];
				UnitySlot proxySlot = combineMeshInfo.proxySlot;
				MeshBuffer meshBuffer2 = new MeshBuffer();
				meshBuffer2.name = proxySlot._meshBuffer.name;
				meshBuffer2.sharedMesh = MeshBuffer.GenerateMesh();
				meshBuffer2.sharedMesh.Clear();
				meshBuffer2.CombineMeshes(combineMeshInfo.combines.ToArray());
				meshBuffer2.vertexDirty = true;
				proxySlot._meshFilter.sharedMesh = meshBuffer2.sharedMesh;
				meshBuffers[j] = meshBuffer2;
				_verticeOffset = 0;
				for (int k = 0; k < combineMeshInfo.slots.Count; k++)
				{
					UnitySlot unitySlot = combineMeshInfo.slots[k];
					unitySlot._isCombineMesh = true;
					unitySlot._sumMeshIndex = j;
					unitySlot._verticeOrder = k;
					unitySlot._verticeOffset = _verticeOffset;
					unitySlot._combineMesh = this;
					unitySlot._meshBuffer.enabled = false;
					if (unitySlot._renderDisplay != null)
					{
						unitySlot._renderDisplay.SetActive(value: false);
						unitySlot._renderDisplay.hideFlags = HideFlags.HideInHierarchy;
						UnityEngine.Transform transform = unitySlot._renderDisplay.transform;
						UnityEngine.Transform transform2 = transform;
						Vector3 localPosition = transform.localPosition;
						transform2.localPosition = new Vector3(0f, 0f, localPosition.z);
						transform.localEulerAngles = Vector3.zero;
						transform.localScale = Vector3.one;
					}
					if (unitySlot._deformVertices != null)
					{
						unitySlot._deformVertices.verticesDirty = true;
					}
					unitySlot._transformDirty = true;
					unitySlot.Update(-1);
					meshBuffer2.combineSlots.Add(unitySlot);
					slotNames.Add(unitySlot.name);
					_verticeOffset += unitySlot._meshBuffer.vertexBuffers.Length;
					_subSlotCount++;
				}
				if (proxySlot._renderDisplay != null)
				{
					proxySlot._renderDisplay.SetActive(value: true);
					proxySlot._renderDisplay.hideFlags = HideFlags.None;
				}
			}
		}

		public void CollectMesh(Armature armature, List<CombineMeshInfo> combineSlots)
		{
			if (armature == null)
			{
				return;
			}
			List<Slot> list = new List<Slot>(armature.GetSlots());
			if (list.Count == 0)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			UnitySlot unitySlot = null;
			GameObject gameObject = null;
			for (int i = 0; i < list.Count; i++)
			{
				UnitySlot unitySlot2 = list[i] as UnitySlot;
				unitySlot2.CancelCombineMesh();
				flag3 = (unitySlot2.childArmature != null);
				gameObject = unitySlot2.renderDisplay;
				flag2 = ((unitySlot == null) ? (unitySlot == null) : (unitySlot2._meshBuffer.name == string.Empty || unitySlot._meshBuffer.name == unitySlot2._meshBuffer.name));
				flag = (flag3 || unitySlot2._isIgnoreCombineMesh || unitySlot2._blendMode != 0 || !flag2);
				if (flag)
				{
					if (combineSlots.Count > 0)
					{
						CombineMeshInfo combineMeshInfo = combineSlots[combineSlots.Count - 1];
						if (combineMeshInfo.combines.Count == 1)
						{
							combineSlots.RemoveAt(combineSlots.Count - 1);
						}
					}
					unitySlot = null;
				}
				if (unitySlot == null && !flag && gameObject != null && gameObject.activeSelf)
				{
					CombineMeshInfo item = default(CombineMeshInfo);
					item.proxySlot = unitySlot2;
					item.combines = new List<CombineInstance>();
					item.slots = new List<UnitySlot>();
					combineSlots.Add(item);
					unitySlot = unitySlot2;
				}
				if (flag3)
				{
					continue;
				}
				if (unitySlot != null && gameObject != null && gameObject.activeSelf && !unitySlot2._isIgnoreCombineMesh)
				{
					UnityEngine.Transform transform = (unitySlot2._armature.proxy as UnityArmatureComponent).transform;
					CombineInstance item2 = default(CombineInstance);
					item2.mesh = unitySlot2._meshBuffer.sharedMesh;
					item2.transform = unitySlot._renderDisplay.transform.worldToLocalMatrix * gameObject.transform.localToWorldMatrix;
					CombineMeshInfo combineMeshInfo2 = combineSlots[combineSlots.Count - 1];
					combineMeshInfo2.combines.Add(item2);
					CombineMeshInfo combineMeshInfo3 = combineSlots[combineSlots.Count - 1];
					combineMeshInfo3.slots.Add(unitySlot2);
				}
				if (i != list.Count - 1)
				{
					continue;
				}
				if (combineSlots.Count > 0)
				{
					CombineMeshInfo combineMeshInfo4 = combineSlots[combineSlots.Count - 1];
					if (combineMeshInfo4.combines.Count == 1)
					{
						combineSlots.RemoveAt(combineSlots.Count - 1);
					}
				}
				unitySlot = null;
			}
		}
	}
}
