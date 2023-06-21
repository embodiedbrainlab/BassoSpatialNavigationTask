using UnityEngine;
using System.Collections;
using System.Text;

public class CryptoSample : MonoBehaviour
{
	// 32byte length.(Insert your encryption key)
	public static string encryptKey = "12345678901234567890123456789012";

	// Use this for initialization
	void Start()
	{
		string data = "EasyCrypto is very simple and easy.";
		// 1. Encrypt.
		byte[] encrypted = easy.Crypto.encrypt(data, encryptKey);
		Debug.Log("Encrypted : " + Encoding.UTF8.GetString(encrypted));

		// 2. Decrypt.
		string decrypted = easy.Crypto.decrypt(encrypted, encryptKey);
		Debug.Log("Decrypted : " + decrypted);

	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
