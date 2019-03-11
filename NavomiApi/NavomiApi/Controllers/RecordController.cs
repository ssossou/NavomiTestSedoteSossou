using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NavomiApi.Interfaces;
using NavomiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using NavomiApi.Services;

namespace NavomiApi.Controllers
{
    public class RecordController:ControllerBase
    {

        
        private IConfiguration _config;
        private ILogService _logger;
        private ISalesForceService _salesForceService;
        public RecordController( ILogService logger, ISalesForceService salesForceService)
        {
            _logger = logger;
            _salesForceService = salesForceService;
            
            
        }

        // GET: api/Settings
        [HttpGet]
        //[Authorize]
        //[AllowAnonymous]
        [Route("api/records")]
        [Consumes("application/json"), Produces("application/json")]
        [ProducesResponseType(typeof(HttpResponse), 200)]
        [ProducesResponseType(typeof(FailedMessage), 400)]
        [ProducesResponseType(typeof(FailedMessage), 500)]
        public async Task<IActionResult> records(string objectType)
        {
            try
            {
               
                var userName = User.Identity.Name;
                _logger.LogWarning($"Processing request for : {userName}");
                string owner = (User.FindFirst(ClaimTypes.NameIdentifier))?.Value;






                var result = _salesForceService.GetRecords(objectType);


                if (result != null && result.Exception == null)
                {
                    return Ok(result);
                }
                else
                {

                    _logger.LogWarning("Failure");
                    return new NotFoundObjectResult(new FailedMessage()
                    {
                        ErrorCode = (int)HttpStatusCode.NotFound,
                        Message = "No record found"
                    });
                }
            }
            catch (Exception e)
            {
                throw;

            }

        }


    }
}
