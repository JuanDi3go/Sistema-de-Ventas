﻿namespace SistemaDeVenta.WebApplication.Models.ViewModels
{
    public class VMDashBoard
    {
        public int TotalVentas { get; set; }
        public string? TotalIngresos { get; set; }
        public int TotalProductos { get; set; }
        public int TotalCategorias { get; set; }
    
        public List<VMVentasSemana> VentasSemanaList { get;set; }
        public List<VMProductosPorSemana> ProductosTopUltimaSemana { get; set; }    
    }
}
