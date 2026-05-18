using Eventra.Services.Interfaces;
using QRCoder;

namespace Eventra.Services
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] GenerateBytes(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(10);
        }
    }
}
