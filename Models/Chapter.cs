using System;
using System.Collections.Generic;

namespace ProjectPRNLamnthe180410.Models;

public partial class Chapter
{
    public int Id { get; set; }

    public int LightNovelId { get; set; }

    public int Chapter1 { get; set; }

    public string? Description { get; set; }

    public virtual LightNovel LightNovel { get; set; } = null!;
}
