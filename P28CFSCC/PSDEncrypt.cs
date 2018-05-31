using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;


/// <summary>
/// Summary description for PSDEncrypt
/// </summary>
public class PSDEncrypt
{
	public PSDEncrypt()
	{
		//
		// TODO: Add constructor logic here
		//
	}


    public string GenerateKey(int keySize)         
    {                 
        RijndaelManaged aesEncryption = new RijndaelManaged();                 
        aesEncryption.KeySize = keySize;                 
        aesEncryption.BlockSize = 128;                 
        aesEncryption.Mode = CipherMode.CBC;                 
        aesEncryption.Padding = PaddingMode.PKCS7;                 
        aesEncryption.GenerateIV();                 
        string ivStr = Convert.ToBase64String(aesEncryption.IV);                 
        aesEncryption.GenerateKey();                 
        string keyStr = Convert.ToBase64String(aesEncryption.Key);                       
        string completeKey = ivStr + "," + keyStr;                       
        return Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(completeKey));         
    }
    
    
    public string Encrypt(string plainStr, string completeEncodedKey, int keySize)         
    {                 
        RijndaelManaged aesEncryption = new RijndaelManaged();                 
        aesEncryption.KeySize = keySize;                 
        aesEncryption.BlockSize = 128;                 
        aesEncryption.Mode = CipherMode.CBC;                
        aesEncryption.Padding = PaddingMode.PKCS7;                 
        aesEncryption.IV = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[0]);                 
        aesEncryption.Key = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[1]);                 
        byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);                 
        ICryptoTransform crypto = aesEncryption.CreateEncryptor();                             
        // The result of the encryption and decryption                 
        byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);                 
        return Convert.ToBase64String(cipherText);         
    }


    public string Decrypt(string encryptedText, string completeEncodedKey, int keySize) 
    { 
        RijndaelManaged aesEncryption = new RijndaelManaged(); 
        aesEncryption.KeySize = keySize; aesEncryption.BlockSize = 128; 
        aesEncryption.Mode = CipherMode.CBC; aesEncryption.Padding = PaddingMode.PKCS7; 
        aesEncryption.IV = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[0]); 
        aesEncryption.Key = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[1]); 
        ICryptoTransform decrypto = aesEncryption.CreateDecryptor(); 
        byte[] encryptedBytes = Convert.FromBase64CharArray(encryptedText.ToCharArray(), 0, encryptedText.Length); 
        return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length)); 
    }
    
}