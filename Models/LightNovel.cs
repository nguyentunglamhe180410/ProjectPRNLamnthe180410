using System;
using System.Collections.Generic;

namespace ProjectPRNLamnthe180410.Models;

public partial class LightNovel
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? Aired { get; set; }

    public int? Episodes { get; set; }

    public int? Read { get; set; }

    public int? Ranked { get; set; }

    public double? Score { get; set; }

    public string? ImgUrl { get; set; }

    public string? Link { get; set; }

    public bool Status { get; set; }

    public int GenreId { get; set; }

    public int? Cost { get; set; }

    public virtual ICollection<Bought> Boughts { get; set; } = new List<Bought>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Genre Genre { get; set; } = null!;
}
