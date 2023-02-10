using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.IO;
using System.Text;

namespace Nightingale.Utilitys
{
	public class CompressUtility
	{
		public static string CompressString(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return string.Empty;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(content);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BZip2OutputStream bZip2OutputStream = new BZip2OutputStream(memoryStream))
				{
					bZip2OutputStream.Write(bytes, 0, bytes.Length);
					bZip2OutputStream.Close();
				}
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}

		public static string DecompressString(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return string.Empty;
			}
			string empty = string.Empty;
			byte[] buffer = Convert.FromBase64String(content);
			using (Stream stream = new MemoryStream(buffer))
			{
				BZip2InputStream stream2 = new BZip2InputStream(stream);
				using (StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8))
				{
					return streamReader.ReadToEnd();
				}
			}
		}
	}
}
