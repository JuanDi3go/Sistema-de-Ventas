﻿namespace SistemaDeVenta.WebApplication.Utilities.Response
{
    public class GenericResponse<TObject>
    {
        public bool Estado { get; set; }
        public string? Message { get; set; }
        public TObject? Object { get; set; }
        public List<TObject>? ListObject { get; set; }
    }
}
