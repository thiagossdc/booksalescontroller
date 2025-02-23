using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

class Program
{
    static List<(string Tipo, int Quantidade)> vendas = new();
    static decimal precoLivro = 50m;
    static decimal porcentagemEditora = 0.2m;
    static decimal descontoAutor = 0.3m;

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Gerenciamento de Vendas de Livros");
            Console.WriteLine("1. Registrar venda");
            Console.WriteLine("2. Excluir última venda");
            Console.WriteLine("3. Exibir relatório");
            Console.WriteLine("4. Gerar relatório Excel");
            Console.WriteLine("5. Sair");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": RegistrarVenda(); break;
                case "2": ExcluirUltimaVenda(); break;
                case "3": ExibirRelatorio(); break;
                case "4": GerarRelatorioExcel(); break;
                case "5": return;
                default: Console.WriteLine("Opção inválida!"); break;
            }
        }
    }

    static void RegistrarVenda()
    {
        Console.WriteLine("Tipos de Venda: 1. Amazon 2. Direta 3. Desconto Autor");
        Console.Write("Escolha o tipo de venda: ");
        string tipo = Console.ReadLine();
        Console.Write("Quantidade de livros: ");
        if (int.TryParse(Console.ReadLine(), out int quantidade) && quantidade > 0)
        {
            string tipoVenda = tipo switch
            {
                "1" => "Amazon",
                "2" => "Direta",
                "3" => "Desconto Autor",
                _ => "Inválido"
            };

            if (tipoVenda != "Inválido")
            {
                vendas.Add((tipoVenda, quantidade));
                Console.WriteLine("Venda registrada com sucesso!");
            }
            else
            {
                Console.WriteLine("Tipo de venda inválido!");
            }
        }
        else
        {
            Console.WriteLine("Quantidade inválida!");
        }

        VoltarMenu();
    }

    static void ExcluirUltimaVenda()
    {
        if (vendas.Count > 0)
        {
            var ultimaVenda = vendas[^1];
            vendas.RemoveAt(vendas.Count - 1);
            Console.WriteLine($"Última venda removida: {ultimaVenda.Tipo} - {ultimaVenda.Quantidade} livros.");
        }
        else
        {
            Console.WriteLine("Nenhuma venda para excluir!");
        }

        VoltarMenu();
    }

    static void ExibirRelatorio()
    {
        if (!vendas.Any())
        {
            Console.WriteLine("Nenhuma venda registrada!");
            VoltarMenu();
            return;
        }

        decimal totalVendas = 0;
        int totalLivros = 0;
        Dictionary<string, int> resumo = new() { { "Amazon", 0 }, { "Direta", 0 }, { "Desconto Autor", 0 } };

        foreach (var venda in vendas)
        {
            resumo[venda.Tipo] += venda.Quantidade;
            totalLivros += venda.Quantidade;

            totalVendas += venda.Tipo switch
            {
                "Amazon" => venda.Quantidade * precoLivro * (1 - porcentagemEditora),
                "Direta" => venda.Quantidade * precoLivro,
                "Desconto Autor" => venda.Quantidade * precoLivro * (1 - descontoAutor),
                _ => 0
            };
        }

        Console.WriteLine("\nResumo das Vendas:");
        foreach (var item in resumo)
        {
            Console.WriteLine($"{item.Key}: {item.Value} livros");
        }
        Console.WriteLine($"Total de Livros Vendidos: {totalLivros}");
        Console.WriteLine($"Faturamento Total: R$ {totalVendas:F2}");

        Console.WriteLine("\nGráfico de Vendas:");
        foreach (var item in resumo)
        {
            Console.WriteLine($"{item.Key}: {new string('#', item.Value)}");
        }

        VoltarMenu();
    }

    static void GerarRelatorioExcel()
    {
        if (!vendas.Any())
        {
            Console.WriteLine("Nenhuma venda registrada para gerar relatório!");
            VoltarMenu();
            return;
        }

        string pastaDownloads = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
        string arquivoExcel = Path.Combine(pastaDownloads, "Relatorio_Vendas_Livros.xlsx");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Relatório de Vendas");

            worksheet.Cells[1, 1].Value = "Tipo de Venda";
            worksheet.Cells[1, 2].Value = "Quantidade";
            worksheet.Cells[1, 3].Value = "Faturamento";

            decimal totalVendas = 0;
            int row = 2;

            foreach (var venda in vendas)
            {
                decimal faturamento = venda.Tipo switch
                {
                    "Amazon" => venda.Quantidade * precoLivro * (1 - porcentagemEditora),
                    "Direta" => venda.Quantidade * precoLivro,
                    "Desconto Autor" => venda.Quantidade * precoLivro * (1 - descontoAutor),
                    _ => 0
                };

                worksheet.Cells[row, 1].Value = venda.Tipo;
                worksheet.Cells[row, 2].Value = venda.Quantidade;
                worksheet.Cells[row, 3].Value = faturamento;

                totalVendas += faturamento;
                row++;
            }

            worksheet.Cells[row, 1].Value = "Total";
            worksheet.Cells[row, 2].Value = vendas.Sum(v => v.Quantidade);
            worksheet.Cells[row, 3].Value = totalVendas;

            package.SaveAs(new FileInfo(arquivoExcel));
        }

        Console.WriteLine($"Relatório gerado com sucesso em: {arquivoExcel}");
        VoltarMenu();
    }

    static void VoltarMenu()
    {
        Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
        Console.ReadKey();
    }
}
