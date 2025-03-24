using System;
using System.Collections.Generic;

namespace ProjectPRNLamnthe180410.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LightNovelId { get; set; }

    public bool Status { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual LightNovel LightNovel { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
