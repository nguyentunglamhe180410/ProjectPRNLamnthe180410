using System;
using System.Collections.Generic;

namespace ProjectPRNLamnthe180410.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? Coins { get; set; }

    public bool Status { get; set; }

    public string? ProfilePicture { get; set; }

    public string? Description { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Bought> Boughts { get; set; } = new List<Bought>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<History> Histories { get; set; } = new List<History>();
}
