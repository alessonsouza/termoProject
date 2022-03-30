using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;
using System.IO;

namespace termoRefeicoes.Controllers
{
    [ApiController]
    [Route("/consumos-aceitos")]
    [Authorize]
    public class ConsumosAceitosController : ControllerBase
    {

        public readonly IConsumosAceitosReport _consumosAceitosService;
        public readonly ISetor _setorService;
        public readonly IConfiguration _configuration;

        public ConsumosAceitosController(IConsumosAceitosReport termosAceitosService, ISetor setorService, IConfiguration configuration)
        {
            _consumosAceitosService = termosAceitosService;
            _setorService = setorService;
            _configuration = configuration;
        }

        [HttpPost("consumption", Name = "GetConsumption")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Termo))]
        public object GetConsumption([FromBody] ReportFilters filtros)
        {
            Response response = new Response();

            var resp = _consumosAceitosService.Consultar(filtros);
            response.Success = true;
            response.Data = resp;
            return Ok(response);
        }


        public string FormataHora(int horas)
        {
            double valor = horas / 60;
            var hora = Math.Truncate(valor);
            var min = Math.Abs(hora * 60 - horas);
            var dh = hora.ToString().PadLeft(2, '0');
            dh += ':';
            dh += min.ToString().PadLeft(2, '0');
            return dh;

        }


        [HttpPost("documento-excel")]
        [AllowAnonymous]
        public async Task<ActionResult> DocumentoExcel([FromBody] ReportFilters filtros)
        {
            // query data from database   
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            List<Termo> users = _consumosAceitosService.DadosExcel(filtros);
            var TemplateArqConsumos = _configuration.GetSection("ExcelConfigurations:TemplateArqConsumos").Value;

            try
            {
                using (var workbook = new XLWorkbook(TemplateArqConsumos))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Valores");

                    for (int i = 0; i < users.Count; i++)
                    {
                        var cotacao = users[i];
                        worksheet.Cell("A" + (4 + i)).Value =
                            cotacao.NUMCAD;
                        worksheet.Cell("B" + (4 + i)).Value =
                            cotacao.NOMCCU;
                        worksheet.Cell("C" + (4 + i)).Value =
                            cotacao.NOMFUN;
                        if (filtros.JAHACEITOU == "aceitos")
                        {
                            worksheet.Cell("D" + (4 + i)).Value =
                                cotacao.DATA_ACEITE.ToString("dd/MM/yyyy") + " " + FormataHora(cotacao.HORA_ACEITE);
                        }
                        else
                        {
                            worksheet.Cell("D" + (4 + i)).Value =
                                cotacao.TERMO_DESCRICAO;
                        }
                    }

                    var tipo = "";
                    if (filtros.JAHACEITOU == "aceitos")
                    {
                        tipo = "Aceitos";
                        worksheet.Cell("D3").Value = "Data confirmação";
                    }
                    else
                    {
                        tipo = "Não Aceitos";
                        worksheet.Cell("D3").Value = "Competência";
                    }
                    worksheet.Cell("C1").Value = "(" + tipo + ")";
                    worksheet.Cell("C2").Value = "Data/Horário da Geração: " + DateTime.Now.ToLocalTime();


                    // workbook.Save();
                    //required using System.IO;
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, excelName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            /*
            List<Termo> users = _termosAceitosService.DadosExcel(filtros);

            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheets.Worksheet("Valores");

                for (int i = 0; i < users.Count; i++)
                {
                    var cotacao = users[i];
                    worksheet.Cell("A" + (4 + i)).Value =
                        cotacao.NUMCAD;
                    worksheet.Cell("B" + (4 + i)).Value =
                        cotacao.NOMCCU;
                    worksheet.Cell("C" + (4 + i)).Value =
                        cotacao.NOMFUN;
                }
                worksheet.Cell("C2").Value = DateTime.Now;

                workbook.Save();
            }
            */

            // workbook.SaveAs(stream);
            // var fileName = System.IO.Path.GetFileName(caminhoArqCotacoes);
            // var content = System.IO.File.ReadAllBytes(caminhoArqCotacoes);
            // new FileExtensionContentTypeProvider()
            //     .TryGetContentType(fileName, out string contentType);

            // return File(content, contentType, fileName);
            // // string wwwRootPath = _webHostEnvironment.WebRootPath + @"/Image/";
            // // byte[] resp = System.IO.File.ReadAllBytes(wwwRootPath + fileName);
            // byte[] resp = System.IO.File.ReadAllBytes(caminhoArqCotacoes);
            // return File(resp, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            // var stream = System.IO.File.OpenRead(caminhoArqCotacoes);
            // return new FileStreamResult(stream, "application/vnd.ms-excel");


            //     using (var stream = new MemoryStream())
            //     {
            //         var content = stream.ToArray();
            //         // stream.Seek(0, SeekOrigin.Begin);

            //         var fileName = System.IO.Path.GetFileName(caminhoArqCotacoes);

            //         // var content = await System.IO.File.ReadAllBytesAsync(caminhoArqCotacoes);
            //         new FileExtensionContentTypeProvider()
            //             .TryGetContentType(fileName, out string contentType);

            //         // var message = new HttpResponseMessage(HttpStatusCode.OK)
            //         // {
            //         //     Content = new StreamContent(stream)
            //         // };
            //         // message.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            //         // message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //         // {
            //         //     FileName = fileName
            //         // };

            //         // return message;
            //         Byte[] bytes = System.IO.File.ReadAllBytes(caminhoArqCotacoes);
            //         string base64 = Convert.ToBase64String(bytes);
            //         response.Data = base64;

            //         return File(bytes, contentType, fileName);
            //         // return Ok(response);
            //         // Ok(response);
            //     }
            // //required using OfficeOpenXml;
            // // If you use EPPlus in a noncommercial context
            // // according to the Polyform Noncommercial license:
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // using (var package = new ExcelPackage(stream))
            // {
            //     var workSheet = package.Workbook.Worksheets.Add("Sheet1");
            //     workSheet.Cells.LoadFromCollection(list, true);
            //     package.Save();
            // }
            // stream.Position = 0;
            // string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            // return Ok(excelName);
            // return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}