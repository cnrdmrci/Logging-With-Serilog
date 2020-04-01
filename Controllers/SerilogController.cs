using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

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

        [HttpGet("DependencyTest")]
        public IActionResult DependencyTest()
        {
            // StartUp ve Program classlarındaki tanımlamayı baz alarak log ataması yapıldı.
            var person = new { Name = "Caner", City = "İstanbul" };
            _logger.LogWarning("Test Dependency, {@Person}", person);
            _logger.LogTrace("Verbose log");
            Log.Logger.ForContext("Tip","Genel").Information("Info log");
            return Ok();
        }

        [HttpGet("ConsoleTest")]
        public IActionResult WriteToConsoleTest()
        {
            //Local olarak tanımladık. Konsol uygulamalarında kullanılabilir.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            var person = new { Name = "Caner", City = "İstanbul" };
            Log.Logger.ForContext("Tip", "Console").Warning("Test Console, {@Person}", person);
            
            return Ok();
        }

        [HttpGet("SqlServerTest")]
        public IActionResult WriteToSqlServerTest()
        {
            //Local olarak tanımladık. Konsol uygulamalarında kullanılabilir.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer("Server=localhost;Database=SeriLog;User Id=userName;Password=password;", "Logs")
                .CreateLogger();
            var person = new { Name = "Caner", City = "İstanbul" };
            Log.Logger.ForContext("Tip", "SqlServer").Warning("Test SqlServer {@Person}", person);
            return Ok();
        }

        [HttpGet("Seq")]
        public IActionResult TestSeq()
        {
            //Local olarak tanımladık. Konsol uygulamalarında kullanılabilir.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.Console()
                .CreateLogger();
            var person = new { Name = "Caner", City = "İstanbul" };
            Log.Logger.ForContext("Tip", "Seq").Warning("Test Seq {@Person}", person);

            return Ok();
        }
    }
}
