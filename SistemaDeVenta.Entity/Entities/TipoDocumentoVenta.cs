﻿using System;
using System.Collections.Generic;

namespace SistemaDeVenta.Entity.Entities;

public partial class TipoDocumentoVenta
{
    public int IdTipoDocumentoVenta { get; set; }

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Venta> Venta { get; } = new List<Venta>();
}
