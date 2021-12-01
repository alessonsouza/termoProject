using System;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace termoRefeicoes.Controllers
{
    [ApiController]
    [Route("/termo")]
    [Authorize]
    public class TermoController : ControllerBase
    {

        public readonly ITermo _termoService;

        public TermoController(ITermo termoService)
        {
            _termoService = termoService;
        }

        [HttpPut("{competencia}/{matricula}/{hora}", Name = "Submit")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        public object Submit([FromRoute] string competencia, int matricula, int hora)
        {
            Response response = new Response();

            var resp = _termoService.Submit(competencia, matricula, hora);
            response.Success = true;
            response.Data = resp;
            return Ok(response);
        }

        [HttpGet("get-termo", Name = "GeTermo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Termo))]

        public object GetTermo()
        {
            Response response = new Response();
            try
            {

                var resp = _termoService.GetTermo();
                response.Success = true;
                response.Data = resp.Result;
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Error = e.Message;
                response.Success = false;
                return BadRequest(response);

            }
        }

    }
}