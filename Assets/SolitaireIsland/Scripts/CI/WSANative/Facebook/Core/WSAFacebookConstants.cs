namespace CI.WSANative.Facebook.Core
{
	public static class WSAFacebookConstants
	{
		public const string ApiVersionNumber = "v2.7";

		public const string WebRedirectUri = "http://www.facebook.com/connect/login_success.html";

		public const string FeedDialogResponseUri = "/dialog/return/close";

		public const string RequestDialogResponseUri = "/connect/login_success.html";

		public const string SendDialogResponseUri = "/connect/login_success.html";

		public const string LoginDialogResponseUri = "/connect/login_success.html";

		public static string GraphApiUri => string.Format("https://graph.facebook.com/{0}/", "v2.7");

		public static string FeedDialogUri => string.Format("https://www.facebook.com/{0}/dialog/feed", "v2.7");

		public static string RequestDialogUri => string.Format("https://www.facebook.com/{0}/dialog/apprequests", "v2.7");

		public static string SendDialogUri => string.Format("https://www.facebook.com/{0}/dialog/send", "v2.7");
	}
}
