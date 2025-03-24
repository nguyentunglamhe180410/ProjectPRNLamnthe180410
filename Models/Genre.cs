using System;
using System.Collections.Generic;

namespace ProjectPRNLamnthe180410.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<LightNovel> LightNovels { get; set; } = new List<LightNovel>();
}
