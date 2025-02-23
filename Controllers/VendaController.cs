using GerenciamentoVendas.Models;
using GerenciamentoVendas.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Runtime.InteropServices;

namespace GerenciamentoVendas.Controllers
{
    [Route("api/vendas")]
    [ApiController]
    public class VendaController : ControllerBase
    {
        private readonly VendaService _vendaService;

        public VendaController()
        {
            _vendaService = new VendaService();
        }

        [HttpPost("registrar")]
        public IActionResult RegistrarVenda([FromForm] string tipoVenda, [FromForm] int quantidade)
        {
            _vendaService.AdicionarVenda(tipoVenda, quantidade);
            return Ok(new { message = "Venda registrada com sucesso!" });
        }

        [HttpGet("listar")]
        public IActionResult ListarVendas()
        {
            return Ok(_vendaService.ObterVendas());
        }

        [HttpGet("gerar-excel")]
        public IActionResult GerarRelatorioExcel()
        {
            string caminhoArquivo = _vendaService.GerarRelatorioExcel();
            if (!System.IO.File.Exists(caminhoArquivo))
                return NotFound("Erro ao gerar relatório.");

            return PhysicalFile(caminhoArquivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Relatorio_Vendas.xlsx");
        }
    }
}
