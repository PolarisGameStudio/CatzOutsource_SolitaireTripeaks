using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DragonBones
{
	public class MeshBuffer : IDisposable
	{
		public readonly List<UnitySlot> combineSlots = new List<UnitySlot>();

		public string name;

		public Mesh sharedMesh;

		public int vertexCount;

		public Vector3[] rawVertextBuffers;

		public Vector2[] uvBuffers;

		public Vector3[] vertexBuffers;

		public Color32[] color32Buffers;

		public int[] triangleBuffers;

		public bool vertexDirty;

		public bool zorderDirty;

		public bool enabled;

		[CompilerGenerated]
		private static Comparison<UnitySlot> _003C_003Ef__mg_0024cache0;

		public static Mesh GenerateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
			mesh.MarkDynamic();
			return mesh;
		}

		private static int _OnSortSlots(Slot a, Slot b)
		{
			if (a._zOrder > b._zOrder)
			{
				return 1;
			}
			if (a._zOrder < b._zOrder)
			{
				return -1;
			}
			return 0;
		}

		public void Dispose()
		{
			if (sharedMesh != null)
			{
				UnityFactoryHelper.DestroyUnityObject(sharedMesh);
			}
			combineSlots.Clear();
			name = string.Empty;
			sharedMesh = null;
			vertexCount = 0;
			rawVertextBuffers = null;
			uvBuffers = null;
			vertexBuffers = null;
			color32Buffers = null;
			vertexDirty = false;
			enabled = false;
		}

		public void Clear()
		{
			if (sharedMesh != null)
			{
				sharedMesh.Clear();
				sharedMesh.uv = null;
				sharedMesh.vertices = null;
				sharedMesh.normals = null;
				sharedMesh.triangles = null;
				sharedMesh.colors32 = null;
			}
			name = string.Empty;
		}

		public void CombineMeshes(CombineInstance[] combines)
		{
			if (sharedMesh == null)
			{
				sharedMesh = GenerateMesh();
			}
			sharedMesh.CombineMeshes(combines);
			uvBuffers = sharedMesh.uv;
			rawVertextBuffers = sharedMesh.vertices;
			vertexBuffers = sharedMesh.vertices;
			color32Buffers = sharedMesh.colors32;
			triangleBuffers = sharedMesh.triangles;
			vertexCount = vertexBuffers.Length;
			if (color32Buffers == null || color32Buffers.Length != vertexCount)
			{
				color32Buffers = new Color32[vertexCount];
			}
		}

		public void InitMesh()
		{
			if (vertexBuffers != null)
			{
				vertexCount = vertexBuffers.Length;
			}
			else
			{
				vertexCount = 0;
			}
			if (color32Buffers == null || color32Buffers.Length != vertexCount)
			{
				color32Buffers = new Color32[vertexCount];
			}
			sharedMesh.vertices = vertexBuffers;
			sharedMesh.uv = uvBuffers;
			sharedMesh.colors32 = color32Buffers;
			sharedMesh.triangles = triangleBuffers;
			sharedMesh.RecalculateBounds();
			enabled = true;
		}

		public void UpdateVertices()
		{
			sharedMesh.vertices = vertexBuffers;
			sharedMesh.RecalculateBounds();
		}

		public void UpdateColors()
		{
			sharedMesh.colors32 = color32Buffers;
		}

		public void UpdateOrder()
		{
			combineSlots.Sort(_OnSortSlots);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Vector2[] array = new Vector2[vertexCount];
			Vector3[] array2 = new Vector3[vertexCount];
			Color32[] array3 = new Color32[vertexCount];
			CombineInstance[] array4 = new CombineInstance[combineSlots.Count];
			for (int i = 0; i < combineSlots.Count; i++)
			{
				UnitySlot unitySlot = combineSlots[i];
				num3 = unitySlot._verticeOffset;
				unitySlot._verticeOrder = i;
				unitySlot._verticeOffset = num2;
				CombineInstance combineInstance = default(CombineInstance);
				unitySlot._meshBuffer.InitMesh();
				combineInstance.mesh = unitySlot._meshBuffer.sharedMesh;
				array4[i] = combineInstance;
				float zSpace = (unitySlot._armature.proxy as UnityArmatureComponent).zSpace;
				for (int j = 0; j < unitySlot._meshBuffer.vertexCount; j++)
				{
					num = num3 + j;
					array[num2] = uvBuffers[num];
					array2[num2] = vertexBuffers[num];
					array3[num2] = color32Buffers[num];
					array2[num2].z = (float)(-unitySlot._verticeOrder) * (zSpace + 0.001f);
					num2++;
				}
			}
			sharedMesh.Clear();
			sharedMesh.CombineMeshes(array4);
			uvBuffers = array;
			vertexBuffers = array2;
			color32Buffers = array3;
			triangleBuffers = sharedMesh.triangles;
			InitMesh();
		}
	}
}
