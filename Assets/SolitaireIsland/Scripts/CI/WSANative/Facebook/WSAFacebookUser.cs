using System;

namespace CI.WSANative.Facebook
{
	public class WSAFacebookUser
	{
		public string Id
		{
			get;
			set;
		}

		public WSAFacebookAgeRange AgeRange
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public string Link
		{
			get;
			set;
		}

		public string Gender
		{
			get;
			set;
		}

		public string Locale
		{
			get;
			set;
		}

		public WSAFacebookPicture Picture
		{
			get;
			set;
		}

		public int TimeZone
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public DateTime? Birthday
		{
			get;
			set;
		}
	}
}
