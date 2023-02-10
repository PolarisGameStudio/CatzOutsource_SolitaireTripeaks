using System;
using System.Security.Cryptography;
using System.Text;

namespace cookapps
{
	public class DataEncryption
	{
		//private static readonly string keyM3 = "j*p6aPrcHuc2u8!k7c2pTsag3esWe4ha";
		
		private static readonly string key = "j*p!aPrcHuc2u8!k7c2pTsag3esWe3ha";

		public static string EncryptString(string toEncrypt)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			byte[] bytes2 = Encoding.UTF8.GetBytes(toEncrypt);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = bytes;
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
			return Convert.ToBase64String(array, 0, array.Length);
		}

		public static byte[] EncryptBytes(byte[] toEncryptArray)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = bytes;
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
			return cryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		}

		public static string DecryptString(string toDecrypt)
		{
			if (string.IsNullOrEmpty(toDecrypt))
			{
				return string.Empty;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			if (toDecrypt.Length % 4 != 0)
			{
				return null;
			}
			byte[] array = Convert.FromBase64String(toDecrypt);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = bytes;
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
			try
			{
				byte[] bytes2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
				return Encoding.UTF8.GetString(bytes2);
			}
			catch (Exception)
			{
			}
			return null;
		}

		public static byte[] DecryptBytes(byte[] toEncryptArray)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = bytes;
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
			try
			{
				return cryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}
