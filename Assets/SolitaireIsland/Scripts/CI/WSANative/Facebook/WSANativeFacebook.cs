using System;
using System.Collections.Generic;

namespace CI.WSANative.Facebook
{
	public static class WSANativeFacebook
	{
		public static bool IsLoggedIn => false;

		public static string AccessToken => string.Empty;

		public static void Initialise(string facebookAppId, string packageSID)
		{
		}

		public static void Login(List<string> permissions, Action<WSAFacebookLoginResult> response)
		{
		}

		public static void Logout(bool uninstall)
		{
		}

		public static void GetUserDetails(Action<WSAFacebookResponse<WSAFacebookUser>> response)
		{
		}

		public static void HasUserLikedPage(string pageId, Action<WSAFacebookResponse<bool>> response)
		{
		}

		public static void GraphApiRead<T>(string edge, Dictionary<string, string> parameters, Action<WSAFacebookResponse<T>> response)
		{
		}

		public static void ShowFeedDialog(string link, string source, Action closed)
		{
		}

		public static void ShowRequestDialog(string title, string message, List<string> filters, List<string> to, string data, Action<IEnumerable<string>> closed)
		{
		}

		public static void ShowSendDialog(string link, Action closed)
		{
		}
	}
}
