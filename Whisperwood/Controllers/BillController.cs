using Microsoft.AspNetCore.Mvc;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService billService;

        public BillController(IBillService billService)
        {
            this.billService = billService;
        }

        [HttpGet("getbyorderid/{orderId}")]
        public async Task<IActionResult> GetBillByOrderId(Guid orderId)
        {
            return await billService.GetBillByOrderIdAsync(orderId);
        }
    }
}
