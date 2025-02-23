using GerenciamentoVendas.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace GerenciamentoVendas.Services
{
    public class VendaService
    {
        private readonly List<Venda> vendas = new();
        private static readonly decimal precoLivro = 50m;
        private static readonly decimal porcentagemEditora = 0.2m;
        private static readonly decimal descontoAutor = 0.3m;

        public void AdicionarVenda(string tipo, int quantidade)
        {
            decimal faturamento = tipo switch
            {
                "Amazon" => quantidade * precoLivro * (1 - porcentagemEditora),
                "Direta" => quantidade * precoLivro,
                "Desconto Autor" => quantidade * precoLivro * (1 - descontoAutor),
                _ => 0
            };

            vendas.Add(new Venda { Id = vendas.Count + 1, Tipo = tipo, Quantidade = quantidade, Faturamento = faturamento });
        }

        public List<Venda> ObterVendas() => vendas;

        public string GerarRelatorioExcel()
        {
            string pastaDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string caminhoArquivo = Path.Combine(pastaDownloads, "Relatorio_Vendas.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Relatório");

                worksheet.Cells[1, 1].Value = "Tipo de Venda";
                worksheet.Cells[1, 2].Value = "Quantidade";
                worksheet.Cells[1, 3].Value = "Faturamento";

                int row = 2;
                foreach (var venda in vendas)
                {
                    worksheet.Cells[row, 1].Value = venda.Tipo;
                    worksheet.Cells[row, 2].Value = venda.Quantidade;
                    worksheet.Cells[row, 3].Value = venda.Faturamento;
                    row++;
                }

                package.SaveAs(new FileInfo(caminhoArquivo));
            }

            return caminhoArquivo;
        }
    }
}
