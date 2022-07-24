using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTO
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
