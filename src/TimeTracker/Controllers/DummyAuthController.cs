﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSwag.Annotations;

namespace TimeTracker.Controllers
{
    // WARNING: For demo only!!
    [OpenApiIgnore]
    public class DummyAuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public DummyAuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // NOT FOR PRODUCTION USE!!!
        // you will need a robust auth implementation for production
        // i.e. try IdentityServer4
        //[AllowAnonymous]
        [Route("/get-token")]
        public IActionResult GetToken(string name = "etf-workshop", bool admin = false)
        {
            var jwt = JwtTokenGenerator
                .Generate(
                name, admin, 
                _configuration["Tokens:Issuer"], 
                _configuration["Tokens:Key"]);

            return Ok(jwt);
        }
    }
}