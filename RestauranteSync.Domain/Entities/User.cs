using System.ComponentModel.DataAnnotations;

namespace RestauranteSync.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Usuario { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;
}
