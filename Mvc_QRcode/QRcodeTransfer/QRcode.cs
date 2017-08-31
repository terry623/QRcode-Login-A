using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using QRcodeTransfer.Repository;

namespace QRcodeTransfer
{
    public class QRcode
    {

        public static IHtmlString GenerateQRcode(string encryptedText, string targetUrl)
        {
            var qrRepository = new qrRepository();        
            var url = _GenerateBUrl(encryptedText, targetUrl);
            var re = qrRepository.GenerateRelayQrCode(url);        
            return re;
        }

        private static string _GenerateBUrl(string encode, string targetUrl = null)
        {
            return targetUrl + "?encode=" + encode;
        }
    }
}
