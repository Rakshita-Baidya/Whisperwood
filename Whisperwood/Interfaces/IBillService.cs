using Microsoft.AspNetCore.Mvc;

namespace Whisperwood.Interfaces
{
    public interface IBillService
    {
        Task<byte[]> GenerateBillPdfAsync(Guid orderId);
        Task SendBillEmailAsync(Guid orderId, string userEmail, byte[] pdfBytes);

        Task<IActionResult> GetBillByOrderIdAsync(Guid orderId);
    }
}
