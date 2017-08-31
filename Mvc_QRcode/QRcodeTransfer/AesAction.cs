using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRcodeTransfer.Repository;

namespace QRcodeTransfer
{
    public class AesAction
    {
        public static string Encryption(string text, string key)
        {
            var aesRepository = new aesRepository();
            var afterencode = aesRepository.EncryptStringToBytes_Aes(text, Convert.FromBase64String(key));
            var token = Convert.ToBase64String(afterencode);
            return token;
        }
        public static string Decryption(string encryptedText, string key)
        {
            if (encryptedText == null) return null;
            else
            {
                var aesRepository = new aesRepository();
                var decryptionResult = aesRepository.DecryptStringFromBytes_Aes(Convert.FromBase64String(encryptedText.Replace(" ", "+")), Convert.FromBase64String(key));
                return decryptionResult;
            }
        }
    }
}
