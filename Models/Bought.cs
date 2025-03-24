using System;
using System.Collections.Generic;

namespace ProjectPRNLamnthe180410.Models;

public partial class Bought
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TitleId { get; set; }

    public virtual LightNovel Title { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
