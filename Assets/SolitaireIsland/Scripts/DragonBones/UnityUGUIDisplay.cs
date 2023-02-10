using UnityEngine;
using UnityEngine.UI;

namespace DragonBones
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
	public class UnityUGUIDisplay : MaskableGraphic
	{
		[HideInInspector]
		public Mesh sharedMesh;

		private Texture _texture;

		public override Texture mainTexture => (!(_texture == null)) ? _texture : material.mainTexture;

		public Texture texture
		{
			get
			{
				return _texture;
			}
			set
			{
				if (!(_texture == value))
				{
					_texture = value;
					SetMaterialDirty();
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}

		public override void Rebuild(CanvasUpdate update)
		{
			base.Rebuild(update);
			if (!base.canvasRenderer.cull && update == CanvasUpdate.PreRender)
			{
				base.canvasRenderer.SetMesh(sharedMesh);
			}
		}

		private void Update()
		{
			base.canvasRenderer.SetMesh(sharedMesh);
		}
	}
}
