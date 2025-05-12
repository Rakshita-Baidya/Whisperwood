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
            XFont headerFont = new XFont("Times New Roman", 18, XFontStyle.Bold);
            XFont subHeaderFont = new XFont("Times New Roman", 14, XFontStyle.Bold);
            XFont regularFont = new XFont("Times New Roman", 12);
            XFont boldFont = new XFont("Times New Roman", 12, XFontStyle.Bold);
            XFont smallFont = new XFont("Times New Roman", 10);

            // Define layout constants
            double margin = 40;
            double yPosition = margin;
            double tableStartY = 0;
            double[] columnWidths = new[] { 250.0, 80.0, 90.0, 90.0 };
            double rowHeight = 25;
            double cellPadding = 5;
            double pageWidth = page.Width - 2 * margin;
            double tableWidth = columnWidths.Sum();
            double tableX = margin;

            // Store Header
            gfx.DrawString("Whisperwood Bookstore", headerFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopCenter);
            yPosition += 25;
            gfx.DrawString("Kamalpokhari, Kathmandu", smallFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopCenter);
            yPosition += 15;
            gfx.DrawString("Phone: +977 9841257741 | Email: whisperwood@gmail.com", smallFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopCenter);
            yPosition += 20;
            gfx.DrawLine(XPens.Black, margin, yPosition, page.Width - margin, yPosition);
            yPosition += 20;

            // Order Details Section
            gfx.DrawString("Order Details:", boldFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 25;

            gfx.DrawString($"Order ID: {order.Id}", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Ordered By: {(order.User?.Name ?? "Unknown")}", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Order Date: {order.Date:yyyy-MM-dd}", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Claim Code: {order.OrderBill.ClaimCode}", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Pickup Date: {order.OrderBill.PickUpDate:yyyy-MM-dd}", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 45;

            // Pickup Instructions
            gfx.DrawString($"Please come pick up your books at the bookstore on or after the pickup date shown above.", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 20;
            gfx.DrawString($"Bring your claim code for verification.", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 45;

            // Items Table
            gfx.DrawString("Order Items", boldFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 20;

            tableStartY = yPosition;
            gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, tableX, yPosition, tableWidth, rowHeight);
            gfx.DrawString("Book Title", boldFont, XBrushes.Black, new XRect(tableX + cellPadding, yPosition + cellPadding, columnWidths[0], rowHeight), XStringFormats.TopLeft);
            gfx.DrawString("Quantity", boldFont, XBrushes.Black, new XRect(tableX + columnWidths[0] + cellPadding, yPosition + cellPadding, columnWidths[1], rowHeight), XStringFormats.TopCenter);
            gfx.DrawString("Unit Price", boldFont, XBrushes.Black, new XRect(tableX + columnWidths.Take(2).Sum() + cellPadding, yPosition + cellPadding, columnWidths[2], rowHeight), XStringFormats.TopCenter);
            gfx.DrawString("Subtotal", boldFont, XBrushes.Black, new XRect(tableX + columnWidths.Take(3).Sum() + cellPadding, yPosition + cellPadding, columnWidths[3], rowHeight), XStringFormats.TopCenter);
            yPosition += rowHeight;

            foreach (var item in order.OrderItems)
            {
                gfx.DrawRectangle(XPens.Black, tableX, yPosition, tableWidth, rowHeight);
                gfx.DrawString(item.Book.Title, regularFont, XBrushes.Black, new XRect(tableX + cellPadding, yPosition + cellPadding, columnWidths[0], rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(item.Quantity.ToString(), regularFont, XBrushes.Black, new XRect(tableX + columnWidths[0] + cellPadding, yPosition + cellPadding, columnWidths[1], rowHeight), XStringFormats.TopCenter);
                gfx.DrawString($"Rs. {item.UnitPrice:F2}", regularFont, XBrushes.Black, new XRect(tableX + columnWidths.Take(2).Sum() + cellPadding, yPosition + cellPadding, columnWidths[2], rowHeight), XStringFormats.TopCenter);
                gfx.DrawString($"Rs. {item.SubTotal:F2}", regularFont, XBrushes.Black, new XRect(tableX + columnWidths.Take(3).Sum() + cellPadding, yPosition + cellPadding, columnWidths[3], rowHeight), XStringFormats.TopCenter);
                yPosition += rowHeight;
            }

            double tableHeight = yPosition - tableStartY;
            for (int i = 0; i <= columnWidths.Length; i++)
            {
                double x = tableX + columnWidths.Take(i).Sum();
                gfx.DrawLine(XPens.Black, x, tableStartY, x, tableStartY + tableHeight);
            }

            yPosition += 25;

            // Financial Summary
            gfx.DrawString("Payment Summary", boldFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            yPosition += 25;

            double rightColumn = margin + pageWidth - 100;
            gfx.DrawString($"Subtotal:", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            gfx.DrawString($"Rs. {order.SubTotal:F2}", regularFont, XBrushes.Black, new XRect(rightColumn, yPosition, 100, 20), XStringFormats.TopRight);
            yPosition += 20;

            if (order.OrderBill.BookDiscount > 0)
            {
                gfx.DrawString($"Book Discount:", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Rs. {order.OrderBill.BookDiscount:F2}", regularFont, XBrushes.Black, new XRect(rightColumn, yPosition, 100, 20), XStringFormats.TopRight);
                yPosition += 20;
            }
            if (order.OrderBill.PromoDiscount > 0)
            {
                gfx.DrawString($"Promo Discount:", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Rs. {order.OrderBill.PromoDiscount:F2}", regularFont, XBrushes.Black, new XRect(rightColumn, yPosition, 100, 20), XStringFormats.TopRight);
                yPosition += 20;
            }
            if (order.OrderBill.BulkDiscount > 0)
            {
                gfx.DrawString($"Bulk Discount (5%):", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Rs. {order.OrderBill.BulkDiscount:F2}", regularFont, XBrushes.Black, new XRect(rightColumn, yPosition, 100, 20), XStringFormats.TopRight);
                yPosition += 20;
            }
            if (order.OrderBill.LoyalDiscount > 0)
            {
                gfx.DrawString($"Loyal Customer Discount (10%):", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Rs. {order.OrderBill.LoyalDiscount:F2}", regularFont, XBrushes.Black, new XRect(rightColumn, yPosition, 100, 20), XStringFormats.TopRight);
                yPosition += 20;
            }
            gfx.DrawString($"Total Discount:", regularFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            gfx.DrawString($"Rs. {order.Discount:F2}", regularFont, XBrushes.Black, new XRect(rightColumn, yPosition, 100, 20), XStringFormats.TopRight);
            yPosition += 25;

            gfx.DrawLine(XPens.Black, margin, yPosition, page.Width - margin, yPosition);
            yPosition += 15;
            gfx.DrawString($"Total:", boldFont, XBrushes.Black, new XRect(margin, yPosition, pageWidth, 20), XStringFormats.TopLeft);
            gfx.DrawString($"Rs. {order.TotalAmount:F2}", boldFont, XBrushes.Black, new XRect(rightColumn, yPosition, 100, 20), XStringFormats.TopRight);

            // Footer at bottom
            double footerY = page.Height - margin - 40;
            gfx.DrawLine(XPens.Black, margin, footerY, page.Width - margin, footerY);
            footerY += 15;
            gfx.DrawString("Thank you for shopping at Whisperwood Bookstore!", smallFont, XBrushes.Black, new XRect(margin, footerY, pageWidth, 20), XStringFormats.TopCenter);

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
