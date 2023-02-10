namespace UnityEngine.UI
{
	public class LoopScrollArraySource<T> : LoopScrollDataSource
	{
		private T[] objectsToFill;

		public LoopScrollArraySource(T[] objectsToFill)
		{
			this.objectsToFill = objectsToFill;
		}

		public override void ProvideData(Transform transform, int idx)
		{
			transform.SendMessage("ScrollCellContent", objectsToFill[idx]);
		}
	}
}
