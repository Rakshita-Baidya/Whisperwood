using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Net.Mail;
using System.Net.Mime;
using Whisperwood.DatabaseContext;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class BillService : IBillService
    {
        private readonly WhisperwoodDbContext dbContext;
        private readonly IConfiguration configuration;

        public BillService(WhisperwoodDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        public async Task<IActionResult> GetBillByOrderIdAsync(Guid orderId)
        {
            var bill = await dbContext.Bill.FirstOrDefaultAsync(b => b.OrderId == orderId);
            return new OkObjectResult(bill);
        }

        public async Task<byte[]> GenerateBillPdfAsync(Guid orderId)
        {
            // Fetch order details
            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Include(o => o.User)
                .Include(o => o.OrderBill)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.OrderBill == null)
                throw new InvalidOperationException("Order or bill not found.");

            // Create a new PDF document
            using PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Set fonts
            XFont headerFont = new XFont("Helvetica", 16, XFontStyle.Bold);
            XFont regularFont = new XFont("Helvetica", 12);
            XFont boldFont = new XFont("Helvetica", 12, XFontStyle.Bold);

            // Define layout constants
            double margin = 40;
            double yPosition = margin;
            double tableStartY = 0;
            double[] columnWidths = new[] { 200.0, 80.0, 80.0, 80.0 };
            double rowHeight = 20;
            double cellPadding = 5;

            // Header
            gfx.DrawString("Whisperwood Bookstore - Order Bill", headerFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 30;

            // Order details
            gfx.DrawString($"Order ID: {order.Id}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Ordered By: {(order.User?.Name ?? "Unknown")}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Order Date: {order.Date:yyyy-MM-dd}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Claim Code: {order.OrderBill.ClaimCode}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Pickup Date: {order.OrderBill.PickUpDate:yyyy-MM-dd}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 30;

            // Table header
            tableStartY = yPosition;
            gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, margin, yPosition, columnWidths.Sum(), rowHeight);
            gfx.DrawString("Book Title", boldFont, XBrushes.Black, new XRect(margin + cellPadding, yPosition + cellPadding, columnWidths[0], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Quantity", boldFont, XBrushes.Black, new XRect(margin + columnWidths[0] + cellPadding, yPosition + cellPadding, columnWidths[1], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Unit Price", boldFont, XBrushes.Black, new XRect(margin + columnWidths.Take(2).Sum() + cellPadding, yPosition + cellPadding, columnWidths[2], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Subtotal", boldFont, XBrushes.Black, new XRect(margin + columnWidths.Take(3).Sum() + cellPadding, yPosition + cellPadding, columnWidths[3], rowHeight), XStringFormats.TopLeft);
            yPosition += rowHeight;

            // Table rows
            foreach (var item in order.OrderItems)
            {
                gfx.DrawRectangle(XPens.Black, margin, yPosition, columnWidths.Sum(), rowHeight);
                gfx.DrawString(item.Book.Title, regularFont, XBrushes.Black, new XRect(margin + cellPadding, yPosition + cellPadding, columnWidths[0], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(item.Quantity.ToString(), regularFont, XBrushes.Black, new XRect(margin + columnWidths[0] + cellPadding, yPosition + cellPadding, columnWidths[1], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString($"Rs. {item.UnitPrice:F2}", regularFont, XBrushes.Black, new XRect(margin + columnWidths.Take(2).Sum() + cellPadding, yPosition + cellPadding, columnWidths[2], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString($"Rs. {item.SubTotal:F2}", regularFont, XBrushes.Black, new XRect(margin + columnWidths.Take(3).Sum() + cellPadding, yPosition + cellPadding, columnWidths[3], rowHeight), XStringFormats.TopLeft);
                yPosition += rowHeight;
            }

            // Table borders
            double tableHeight = yPosition - tableStartY;
            for (int i = 0; i <= columnWidths.Length; i++)
            {
                double x = margin + columnWidths.Take(i).Sum();
                gfx.DrawLine(XPens.Black, x, tableStartY, x, tableStartY + tableHeight);
            }

            yPosition += 20;

            // Financial summary with detailed discounts
            gfx.DrawString($"Subtotal: Rs. {order.SubTotal:F2}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 20;
            if (order.OrderBill.PromoDiscount > 0)
            {
                gfx.DrawString($"Promo Discount: Rs. {order.OrderBill.PromoDiscount:F2}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                yPosition += 20;
            }
            if (order.OrderBill.BulkDiscount > 0)
            {
                gfx.DrawString($"Bulk Discount (5%): Rs. {order.OrderBill.BulkDiscount:F2}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                yPosition += 20;
            }
            if (order.OrderBill.LoyalDiscount > 0)
            {
                gfx.DrawString($"Loyal Customer Discount (10%): Rs. {order.OrderBill.LoyalDiscount:F2}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                yPosition += 20;
            }
            gfx.DrawString($"Total Discount: Rs. {order.Discount:F2}", regularFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Total: Rs. {order.TotalAmount:F2}", boldFont, XBrushes.Black, new XRect(margin, yPosition, page.Width - 2 * margin, 20), XStringFormats.TopLeft);

            // Save to MemoryStream
            using MemoryStream stream = new MemoryStream();
            document.Save(stream);
            document.Close();
            return stream.ToArray();
        }

        public async Task SendBillEmailAsync(Guid orderId, string userEmail, byte[] pdfBytes)
        {
            var order = await dbContext.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            // Send email with PDF attachment
            await SendEmailAsync(userEmail, order, pdfBytes);
        }

        private async Task SendEmailAsync(string userEmail, Orders order, byte[] pdfBytes)
        {
            var smtpSettings = configuration.GetSection("SmtpSettings");
            var smtpHost = smtpSettings["Host"];
            var smtpPort = int.Parse(smtpSettings["Port"]);
            var smtpUsername = smtpSettings["Username"];
            var smtpPassword = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "Whisperwood Bookstore"),
                Subject = $"Your Order Bill - Order {order.Id}",
                Body = "Dear Customer,\n\nPlease find attached the bill for your recent order. Your claim code and pickup details are included.\n\nThank you for shopping with Whisperwood Bookstore!\n\nBest regards,\nWhisperwood Team",
                IsBodyHtml = false
            };

            message.To.Add(userEmail);

            // Attach PDF
            using var pdfStream = new MemoryStream(pdfBytes);
            var attachment = new Attachment(pdfStream, $"Bill{order.Id}.pdf", MediaTypeNames.Application.Pdf);
            message.Attachments.Add(attachment);

            await client.SendMailAsync(message);
        }
    }
}
