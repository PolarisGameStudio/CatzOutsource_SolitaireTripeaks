namespace CI.WSANative.Facebook
{
	public class WSAFacebookResponse<T>
	{
		public bool Success
		{
			get;
			set;
		}

		public T Data
		{
			get;
			set;
		}

		public WSAFacebookError Error
		{
			get;
			set;
		}
	}
}
