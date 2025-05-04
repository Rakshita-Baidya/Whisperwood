namespace Whisperwood.Interfaces
{
    public interface IBillService
    {
        Task<byte[]> GenerateBillPdfAsync(Guid orderId);
        Task SendBillEmailAsync(Guid orderId, string userEmail, byte[] pdfBytes);
    }
}
