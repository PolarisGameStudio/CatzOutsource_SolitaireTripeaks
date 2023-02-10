using UnityEngine;

namespace DragonBones
{
	public class UnityTextureAtlasData : TextureAtlasData
	{
		internal bool _disposeEnabled;

		public Material texture;

		public Material uiTexture;

		protected override void _OnClear()
		{
			base._OnClear();
			if (_disposeEnabled && texture != null)
			{
				UnityFactoryHelper.DestroyUnityObject(texture);
			}
			if (_disposeEnabled && uiTexture != null)
			{
				UnityFactoryHelper.DestroyUnityObject(uiTexture);
			}
			_disposeEnabled = false;
			texture = null;
			uiTexture = null;
		}

		public override TextureData CreateTexture()
		{
			return BaseObject.BorrowObject<UnityTextureData>();
		}
	}
}
