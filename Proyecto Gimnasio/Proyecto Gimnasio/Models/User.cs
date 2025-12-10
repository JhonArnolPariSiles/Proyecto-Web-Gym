using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Gimnasio.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        // --- VALIDACIÓN PERSONALIZADA ---
        [Required(ErrorMessage = "Password is required")]
        // 1. Mínimo 5 caracteres
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters")]
        [DataType(DataType.Password)]
        // 2. Expresión Regular: Al menos una letra y un número
        // (?=.*[0-9]) -> Busca al menos un número
        // (?=.*[a-zA-Z]) -> Busca al menos una letra
        // [a-zA-Z0-9]+$ -> Solo permite letras y números (sin símbolos)
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-zA-Z])[a-zA-Z0-9]+$",
         ErrorMessage = "The password must have a minimum of 5 letters.")]
        public string Password { get; set; }
        // -------------------------------

        public bool primarySession { get; set; }

        [Required(ErrorMessage = "Role is required")]
        // Importante: Mantén 'Customer' aquí para que tu registro funcione
        [RegularExpression(@"^(Admin|User|Employee|Customer)$", ErrorMessage = "Invalid Role")]
        public string Rol { get; set; }

        public Person Person { get; set; }
    }
}