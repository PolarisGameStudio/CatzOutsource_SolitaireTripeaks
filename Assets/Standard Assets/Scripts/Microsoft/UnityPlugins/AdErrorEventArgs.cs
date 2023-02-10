namespace Microsoft.UnityPlugins
{
	public class AdErrorEventArgs
	{
		public ErrorCode ErrorCode
		{
			get;
			private set;
		}

		public string ErrorMessage
		{
			get;
			private set;
		}

		public AdErrorEventArgs(ErrorCode code, string message)
		{
			ErrorCode = code;
			ErrorMessage = message;
		}
	}
}
