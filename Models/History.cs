using System;
using System.Collections.Generic;

namespace ProjectPRNLamnthe180410.Models;

public partial class History
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PurchasedDay { get; set; }

    public bool Status { get; set; }

    public virtual User User { get; set; } = null!;
}
