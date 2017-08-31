using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.Common;

namespace Mvc_QRcode.Repository
{
    public class qrRepository
    {
        //
        // GET: /qrRepository/
        public IHtmlString GenerateRelayQrCode(string url)
            {
                int height = 250;
                int width = 250;
                int margin = 0;
                var qrValue = url + "&source=qr";

                var barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = margin
                    }
                };

                using (var bitmap = barcodeWriter.Write(qrValue))
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Gif);

                    var img = new TagBuilder("img");
                    img.MergeAttribute("alt", "your alt tag");
                    img.Attributes.Add("src", String.Format("data:image/gif;base64,{0}",
                        Convert.ToBase64String(stream.ToArray())));

                    return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
                }
            }
        
    }
}
