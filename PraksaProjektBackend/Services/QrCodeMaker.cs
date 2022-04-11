using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;

namespace PraksaProjektBackend.Services
{
    public class QrCodeMaker
    {
        public static IWebHostEnvironment _webHostEnvironment;

        public QrCodeMaker( IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public static IWebHostEnvironment WebEnv()
        {
            var _accessor = new HttpContextAccessor();
            return _accessor.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        }
        public static async Task<dynamic> MakeQrCode(string chargeid, int quantity, string usermail, int eventid)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode("Event ID: " + eventid + " Charge: " + chargeid + " User: " + usermail + " Uses: " + quantity, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.DarkRed, Color.PaleGreen, true);

            var dirpath = WebEnv().WebRootPath + "\\QRcode";
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            var fileName = "QR_" + chargeid + ".jpg";
            fileName = dirpath + "\\" + fileName;

            qrCodeImage.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            var qrpath = "\\QRcode\\" + "QR_" + chargeid + ".jpg";
            return qrpath;
        }

        public static async Task<dynamic> ReservedTicket(string eventname)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode("VIP Reserved ticket for : " + eventname, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            var dirpath = WebEnv().WebRootPath + "\\ReservedQRcode";
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            var fileName = "QR_" + eventname + ".jpg";
            fileName = dirpath + "\\" + fileName;

            qrCodeImage.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

            return "Success";
        }

    }
}

