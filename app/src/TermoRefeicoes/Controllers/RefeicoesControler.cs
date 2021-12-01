using System;
using System.Threading.Tasks;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace termoRefeicoes.Controllers
{
    [ApiController]
    [Route("/refeicoes")]
    [Authorize]
    public class RefeicoesController : ControllerBase
    {
        public readonly IRefeicoes _refeicoesService;
        public readonly ITermo _termoService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefeicoesController(IRefeicoes refeicoesService, ITermo termoService, IHttpContextAccessor httpContextAccessor)
        {
            _refeicoesService = refeicoesService;
            _termoService = termoService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{competencia}", Name = "GetRefeicoes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Refeicoes))]
        public async Task<IActionResult> GetRefeicoes([FromRoute] string competencia)
        {
            Response response = new Response();
            try
            {
                var resp = await _refeicoesService.Consultar(competencia);
                response.Success = true;
                response.Data = resp;
                return Ok(response);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Error = e.Message;

                return BadRequest(response);
            }
        }

        [HttpGet("get-count/{competencia}", Name = "GetCountAccept")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Refeicoes))]
        public int GetCountAccept([FromRoute] string competencia)
        {
            try
            {
                var resp = _refeicoesService.GetCountAccept(competencia);
                return resp;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        [HttpGet("get-termo/{matricula}", Name = "GetTermo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Termo))]
        public object GetTermo([FromRoute] int matricula)
        {
            Response response = new Response();
            try
            {
                var resp = _refeicoesService.GetTermoAceite(matricula);
                response.Success = true;
                response.Data = resp;
                response.lastMonth = _refeicoesService.GetFirstMonth(matricula);
                return response;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Error = e.Message;

                return response;
                //  throw new ApplicationException(e.Message);
            }
        }

        [HttpPost("submit-termo", Name = "SubmitTermo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        public object SubmitTermo([FromBody] Termo obj)
        {
            Response response = new Response();
            try
            {

                var termo = _refeicoesService.GetTermoAceite(obj.NUMCAD);
                if (termo > 0)
                {
                    response.Success = false;
                    response.Data = termo;
                    response.Error = "Termo já foi aceito. Recarregue a página!";
                    return Ok(response);
                }
                var resp = _refeicoesService.SaveTerm(obj);
                response.Success = true;
                response.Data = resp;
                response.Error = "Termo aceito com sucesso!";
                return Ok(response);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Error = e.Message;

                return BadRequest(response);
            }
        }
    }
}