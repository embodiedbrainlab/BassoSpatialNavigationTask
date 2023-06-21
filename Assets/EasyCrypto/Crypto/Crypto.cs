using System.Collections;
using System.Text;
using System.Security.Cryptography;

namespace easy
{
	public class Crypto
	{
		public static byte[] encrypt(string toEncrypt, string key)
		{
			try
			{
				// 256-AES key
				byte[] keyArray = Encoding.UTF8.GetBytes(key);
				byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
				RijndaelManaged rDel = new RijndaelManaged();
				rDel.Key = keyArray;
				rDel.Mode = CipherMode.ECB;
				rDel.Padding = PaddingMode.ISO10126;
				ICryptoTransform cTransform = rDel.CreateEncryptor();
				return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			}
			catch
			{}
			return null;
		}
		
		public static string decrypt(byte[] toEncryptArray, string key)
		{
			try
			{
				// AES-256 key
				byte[] keyArray = Encoding.UTF8.GetBytes(key);
				RijndaelManaged rDel = new RijndaelManaged();
				rDel.Key = keyArray;
				rDel.Mode = CipherMode.ECB;
				rDel.Padding = PaddingMode.ISO10126;
				ICryptoTransform cTransform = rDel.CreateDecryptor();
				byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
				//return UTF8Encoding.UTF8.GetString(resultArray);
				return Encoding.UTF8.GetString(resultArray);
			}
			catch
			{}
			return null;
		}
	}
}