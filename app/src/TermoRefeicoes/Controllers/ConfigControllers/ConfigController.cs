using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;

namespace TermoRefeicoes.Controllers.ConfigControllers
{
    [ApiController]
    [Route("/config")]
    [Authorize]
    public class ConfigController : ControllerBase
    {

        public readonly IConfigService _configService;

        public ConfigController(IConfigService configService)
        {
            _configService = configService;
        }



        [HttpGet("get-config")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Config))]
        public async Task<IActionResult> getconfig()
        {

            Response response = new Response();

            try
            {
                var resp = await _configService.getConfig();

                response.Data = resp;
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {

                response.Error = ex.Message;
                response.Success = false;
                return BadRequest(response);
            }


        }


        [HttpPost("save-config")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Config))]
        public async Task<IActionResult> Saveconfig([FromBody] Config dados)
        {

            Response response = new Response();

            try
            {
                var resp = await _configService.saveConfig(dados);

                response.Data = resp;
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {

                response.Error = ex.Message;
                response.Success = false;
                return BadRequest(response);
            }


        }

    }
}