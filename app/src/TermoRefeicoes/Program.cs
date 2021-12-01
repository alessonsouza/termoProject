using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TermoRefeicoes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // .SetBasePath("C:/www/termoRefeicoes/app/src/TermoRefeicoes/")
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();


            //  var configuration = builder.Build();

            // DateTime dataHoraExtracao = DateTime.Now;
            // var excelConfigurations = new ExcelConfigurations();
            // new ConfigureFromConfigurationOptions<ExcelConfigurations>(
            //     configuration.GetSection("ExcelConfigurations"))
            //         .Configure(excelConfigurations);

            // Console.WriteLine("Gerando o arquivo .xlsx (Excel) com as cotações...");



            try
            {
                Log.Information("Iniciando a aplicação");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Aplicação encerrada devido a um erro");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseSerilog(); ;
                });
    }
}
