using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bff.Application.Backsup.Command;
using Bff.Domain.Constants;
using Bff.Identity;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bff.Endpoints
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackUpData : ControllerBase
    {
        private readonly ISender sender;
        public BackUpData(ISender sender)
        {
            this.sender = sender;
        }

        [HttpPost("back-up-data")]
        // [Authorize(Roles = "Administrator")]
        public async Task<IResult> BackUpDatas([FromBody] BacksUpDatas requestTenantId)
        {
            if (requestTenantId.TenantId == Guid.Empty)
            {
                return Results.BadRequest(new { Message = "400 | error back up data !" });
            }
            try
            {
                var result = await sender.Send(new ResetDataCommand() { TenantId = requestTenantId.TenantId });
                return Results.Ok(new { Message = "200 | success back up data !" });
            }
            catch (System.Exception)
            {
                return Results.BadRequest(new { Message = "500 | error back up data !" });

            }
        }
    }
}