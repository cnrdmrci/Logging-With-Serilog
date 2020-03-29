using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoggingWithSerilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SerilogController : ControllerBase
    {
        private readonly ILogger<SerilogController> _logger;

        public SerilogController(ILogger<SerilogController> logger)
        {
            _logger = logger;
        }
    }
}
