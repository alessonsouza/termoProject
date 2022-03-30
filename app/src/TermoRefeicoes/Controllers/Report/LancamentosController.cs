using System;
using System.Threading.Tasks;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.StaticFiles;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace termoRefeicoes.Controllers
{
    [ApiController]
    [Route("/lancamentos")]
    [Authorize]
    public class LancamentosController : ControllerBase
    {

        public readonly ILancamentosReport _lancamentosService;
        public readonly ISetor _setorService;
        public readonly IConfiguration _configuration;

        public LancamentosController(ILancamentosReport lancamentosService, ISetor setorService, IConfiguration configuration)
        {
            _lancamentosService = lancamentosService;
            _setorService = setorService;
            _configuration = configuration;
        }

        [HttpPost("releases", Name = "GetReleases")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Lancamentos))]
        public object GetReleases([FromBody] ReportFilters filtros)
        {
            Response response = new Response();

            var resp = _lancamentosService.Consultar(filtros);
            response.Success = true;
            response.Data = resp.Result;
            return Ok(response);
        }

        // [HttpGet("setores", Name = "GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Setor))]
        public object GetAll()
        {
            Response response = new Response();
            try
            {

                var resp = _setorService.GetAll();
                response.Success = true;
                response.Data = resp.Result;
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Error = e.Message;
                return BadRequest(response);
            }
        }

        // [HttpPost("documento", Name = "Documento")]
        public async Task<ActionResult> Documento([FromBody] ReportFilters filtros)
        {

            List<Lancamentos> users = _lancamentosService.DadosExcel(filtros);
            // using (var workbook = new XLWorkbook())
            // {
            Response response = new Response();
            //     var worksheet = workbook.Worksheets.Add("Termo");
            //     var currentRow = 1;
            //     // worksheet.Cell(currentRow, 1).Value = "NUMCAD";
            //     // worksheet.Cell(currentRow, 2).Value = "NUMCAD";
            //     // foreach (var user in users)
            //     // {
            //     //     currentRow++;
            //     worksheet.Cell(1, 1).Value = "alesson";
            //     worksheet.Cell(2, 2).Value = "Souzaz";
            //     // }


            //     using (var stream = new MemoryStream())
            //     {
            //         workbook.SaveAs(stream);
            //         var content = stream.ToArray();

            //         response.Data = content;
            //         // File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "excel.xlsx");
            //         return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "excel.xlsx");
            //         // Ok(response);
            //     }

            //     public static string GerarArquivo(
            //      DateTime dataHoraExtracao,
            //     List<Cotacao> cotacoes ExcelConfigurations configurations,
            //   )
            // {
            // Response response = new Response();
            // ExcelConfigurations configurations;
            // List<Termo> users = _lancamentosService.DadosExcel(filtros);
            // string caminhoArqCotacoes =
            //     configurations.TemplateArqCotacoes +
            //     $"Cotacoes {DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss")}.xlsx";
            // var res = File.Copy(configurations.TemplateArqCotacoes, caminhoArqCotacoes);

            // using (var workbook = new XLWorkbook(caminhoArqCotacoes))
            // {
            //     var worksheet = workbook.Worksheets.Worksheet("Valores");

            //     for (int i = 0; i < users.Count; i++)
            //     {
            //         var cotacao = users[i];
            //         worksheet.Cell("A" + (4 + i)).Value =
            //             "dfsdffsdf";
            //         worksheet.Cell("B" + (4 + i)).Value =
            //             "cotacao.ValorReais";
            //     }
            //     // worksheet.Cell("B9").Value = dataHoraExtracao;

            //     workbook.Save();
            // }
            // response.Data = caminhoArqCotacoes;
            // var configuration = builder.Build();
            var DiretorioGeracaoArqCotacoes = Directory.GetCurrentDirectory() + _configuration.GetSection("ExcelConfigurations:DiretorioGeracaoArqCotacoes").Value;
            var TemplateArqCotacoes = _configuration.GetSection("ExcelConfigurations:TemplateArqCotacoes").Value;


            // var temp = System.IO.File.OpenRead("template");
            // new ConfigureFromConfigurationOptions<ExcelConfigurations>(
            //     configuration.GetSection("ExcelConfigurations"))
            //         .Configure(excelConfigurations);

            // var excelConfigurations =  _configuration.GetSection("ExcelConfigurations").Value;
            string caminhoArqCotacoes =
                DiretorioGeracaoArqCotacoes +
                $"Termo {DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss")}.xlsx";
            System.IO.File.Copy(TemplateArqCotacoes, caminhoArqCotacoes);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            using (var workbook = new XLWorkbook(caminhoArqCotacoes))
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

                // workbook.Save();

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


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    var content = stream.ToArray();
                    // stream.Seek(0, SeekOrigin.Begin);

                    var fileName = System.IO.Path.GetFileName(caminhoArqCotacoes);

                    // var content = await System.IO.File.ReadAllBytesAsync(caminhoArqCotacoes);
                    new FileExtensionContentTypeProvider()
                        .TryGetContentType(fileName, out string contentType);

                    // var message = new HttpResponseMessage(HttpStatusCode.OK)
                    // {
                    //     Content = new StreamContent(stream)
                    // };
                    // message.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    // message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    // {
                    //     FileName = fileName
                    // };

                    // return message;
                    Byte[] bytes = System.IO.File.ReadAllBytes(caminhoArqCotacoes);
                    string base64 = Convert.ToBase64String(bytes);
                    response.Data = base64;

                    return File(content, contentType, fileName);
                    // return Ok(response);
                    // Ok(response);
                }
            }

            // return caminhoArqCotacoes;

            Console.WriteLine("Gerando o arquivo .xlsx (Excel) com as cotações...");

            // string arquivoXlsx = ArquivoExcelCotacoes.GerarArquivo(
            //     excelConfigurations, DateTime.Now,
            //     users);
            response.Data = caminhoArqCotacoes;
            Console.WriteLine($"O arquivo {caminhoArqCotacoes} foi gerado com sucesso!");
            // return Ok(response);
            // }
        }

        //     public static string GerarArquivo(
        //        string configurations,
        //        DateTime dataHoraExtracao,
        //        List<Termo> cotacoes)
        //     {

        //         var excelConfigurations =  _configuration.GetSection("ExcelConfigurations").Value;
        //         string caminhoArqCotacoes =
        //             configurations.DiretorioGeracaoArqCotacoes +
        //             $"Cotacoes {DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss")}.xlsx";
        //         File.Copy(configurations.TemplateArqCotacoes, caminhoArqCotacoes);

        //         using (var workbook = new XLWorkbook(caminhoArqCotacoes))
        //         {
        //             var worksheet = workbook.Worksheets.Worksheet("Valores");

        //             for (int i = 0; i < cotacoes.Count; i++)
        //             {
        //                 var cotacao = cotacoes[i];
        //                 worksheet.Cell("A" + (4 + i)).Value =
        //                     "cotacao.NomeMoeda";
        //                 worksheet.Cell("B" + (4 + i)).Value =
        //                     "cotacao.ValorReais";
        //             }
        //             worksheet.Cell("B9").Value = dataHoraExtracao;

        //             workbook.Save();
        //         }

        //         return caminhoArqCotacoes;
        //     }
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
            List<Lancamentos> users = _lancamentosService.DadosExcel(filtros);
            var TemplateArqLancamentos = _configuration.GetSection("ExcelConfigurations:TemplateArqLancamentos").Value;

            try
            {
                using (var workbook = new XLWorkbook(TemplateArqLancamentos))
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
                        worksheet.Cell("D" + (4 + i)).Value =
                            cotacao.DESREF;
                        worksheet.Cell("E" + (4 + i)).Value =
                            cotacao.USU_DATCON.ToString("dd/MM/yyyy") + " " + FormataHora(cotacao.USU_HORCON);
                        worksheet.Cell("F" + (4 + i)).Value =
                                                cotacao.USU_TPCAPT;
                    }

                    var tipo = "";
                    if (filtros.JAHACEITOU == "0")
                    {
                        tipo = "Refeições";

                    }
                    else
                    {
                        tipo = "Frigobar";
                    }
                    worksheet.Cell("D1").Value = "(" + tipo + ")";
                    worksheet.Cell("E2").Value = "Data/Horário da Geração: " + DateTime.Now.ToLocalTime();


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
            List<Termo> users = _lancamentosService.DadosExcel(filtros);

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