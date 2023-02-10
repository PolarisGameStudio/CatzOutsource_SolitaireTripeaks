using System.Collections.Generic;

namespace DragonBones
{
	public class DeformVertices : BaseObject
	{
		public bool verticesDirty;

		public readonly List<float> vertices = new List<float>();

		public readonly List<Bone> bones = new List<Bone>();

		public VerticesData verticesData;

		protected override void _OnClear()
		{
			verticesDirty = false;
			vertices.Clear();
			bones.Clear();
			verticesData = null;
		}

		public void init(VerticesData verticesDataValue, Armature armature)
		{
			verticesData = verticesDataValue;
			if (verticesData != null)
			{
				int num = 0;
				num = ((verticesData.weight == null) ? (verticesData.data.intArray[verticesData.offset] * 2) : (verticesData.weight.count * 2));
				verticesDirty = true;
				vertices.ResizeList(num, 0f);
				bones.Clear();
				int i = 0;
				for (int count = vertices.Count; i < count; i++)
				{
					vertices[i] = 0f;
				}
				if (verticesData.weight != null)
				{
					int j = 0;
					for (int count2 = verticesData.weight.bones.Count; j < count2; j++)
					{
						Bone bone = armature.GetBone(verticesData.weight.bones[j].name);
						bones.Add(bone);
					}
				}
			}
			else
			{
				verticesDirty = false;
				vertices.Clear();
				bones.Clear();
				verticesData = null;
			}
		}

		public bool isBonesUpdate()
		{
			foreach (Bone bone in bones)
			{
				if (bone != null && bone._childrenTransformDirty)
				{
					return true;
				}
			}
			return false;
		}
	}
}
