using System;
using System.Security.Cryptography;
using System.Text;

namespace Nightingale.Utilitys
{
	public class EncryptionUtility
	{
		public static string Encrypt(string key, string input)
		{
			byte[] array = Encrypt(Encoding.ASCII.GetBytes(key.Substring(0, 16)), Encoding.UTF8.GetBytes(input));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.AppendFormat("{0:X2}", array[i]);
			}
			return stringBuilder.ToString();
		}

		public static byte[] Encrypt(byte[] key, byte[] input)
		{
			AesManaged aesManaged = new AesManaged();
			ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor(key, key);
			return cryptoTransform.TransformFinalBlock(input, 0, input.Length);
		}

		public static string Decrypt(string key, string input)
		{
			byte[] array = new byte[input.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				int num = Convert.ToInt32(input.Substring(i * 2, 2), 16);
				array[i] = (byte)num;
			}
			byte[] array2 = Decrypt(Encoding.ASCII.GetBytes(key.Substring(0, 16)), array);
			return Encoding.UTF8.GetString(array2, 0, array2.Length);
		}

		public static byte[] Decrypt(byte[] key, byte[] input)
		{
			AesManaged aesManaged = new AesManaged();
			ICryptoTransform cryptoTransform = aesManaged.CreateDecryptor(key, key);
			return cryptoTransform.TransformFinalBlock(input, 0, input.Length);
		}

		public static string GetMD5(string msg)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.UTF8.GetBytes(msg);
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes, 0, bytes.Length);
			mD5CryptoServiceProvider.Clear();
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}
	}
}
