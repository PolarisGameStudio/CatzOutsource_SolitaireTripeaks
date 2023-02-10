namespace DragonBones
{
	internal class BlendState
	{
		public bool dirty;

		public int layer;

		public float leftWeight;

		public float layerWeight;

		public float blendWeight;

		public int Update(float weight, int p_layer)
		{
			if (dirty)
			{
				if (leftWeight > 0f)
				{
					if (layer != p_layer)
					{
						if (layerWeight >= leftWeight)
						{
							leftWeight = 0f;
							return 0;
						}
						layer = p_layer;
						leftWeight -= layerWeight;
						layerWeight = 0f;
					}
					weight *= leftWeight;
					layerWeight += weight;
					blendWeight = weight;
					return 2;
				}
				return 0;
			}
			dirty = true;
			layer = p_layer;
			layerWeight = weight;
			leftWeight = 1f;
			blendWeight = weight;
			return 1;
		}

		public void Clear()
		{
			dirty = false;
			layer = 0;
			leftWeight = 0f;
			layerWeight = 0f;
			blendWeight = 0f;
		}
	}
}
