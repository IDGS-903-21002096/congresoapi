using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace congresoAPI.Models
{
    public class clsParticipante
    {
        public int IdParticipante { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        public string Email { get; set; }

        public string? UsuarioTwitter { get; set; }

        [Required]
        public string Ocupacion { get; set; }

        [Required]
        public string AvatarSeleccionado { get; set; }

        public bool AceptaTerminos { get; set; }

        [JsonIgnore]
        public string? CodigoQR { get; set; }
    }
}
