namespace Eventra.Services.Interfaces
{
    public interface IQrCodeService
    {
        byte[] GenerateBytes(string content);
    }
}
