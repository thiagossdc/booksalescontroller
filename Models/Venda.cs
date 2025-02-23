using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciamentoVendas.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Faturamento { get; set; }
    }
}

