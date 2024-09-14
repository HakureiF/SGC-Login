using System.Security.Cryptography;
using System.Text;

namespace Seer.Utils;

public static class AesUtil
{
    private const string Key = "test";

    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="text">加密字符</param>
    /// <returns></returns>
    public static string AesEncrypt(string text)
    {
        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.KeySize = 128;
        aes.BlockSize = 128;
        var pwdBytes = Encoding.UTF8.GetBytes(Key);
        var keyBytes = new byte[16];
        var len = pwdBytes.Length;
        if (len > keyBytes.Length) len = keyBytes.Length;
        Array.Copy(pwdBytes, keyBytes, len);
        aes.Key = keyBytes;
        var ivBytes = Encoding.UTF8.GetBytes(Key);
        aes.IV = ivBytes;
        var transform = aes.CreateEncryptor();
        var plainText = Encoding.UTF8.GetBytes(text);
        var cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
        return Convert.ToBase64String(cipherBytes);
    }
    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string AesDecrypt(string text)
    {
        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.KeySize = 128;
        aes.BlockSize = 128;
        var encryptedData = Convert.FromBase64String(text);
        var pwdBytes = Encoding.UTF8.GetBytes(Key);
        var keyBytes = new byte[16];
        var len = pwdBytes.Length;
        if (len > keyBytes.Length) len = keyBytes.Length;
        Array.Copy(pwdBytes, keyBytes, len);
        aes.Key = keyBytes;
        var ivBytes = Encoding.UTF8.GetBytes(Key);
        aes.IV = ivBytes;
        var transform = aes.CreateDecryptor();
        var plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        return Encoding.UTF8.GetString(plainText);
    }
}