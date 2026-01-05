using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMedia.Shared.DTOs
{
    public class VendaDto
    {
        public int EncomendaId { get; set; }
        public DateTime Data { get; set; }
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Total => Quantidade * PrecoUnitario;
        public string NomeCliente { get; set; }
        public string EstadoEntrega { get; set; }
    }
}