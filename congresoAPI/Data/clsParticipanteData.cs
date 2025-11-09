using Microsoft.Data.SqlClient;
using congresoAPI.Models;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace congresoAPI.Data
{
    public class clsParticipanteData
    {
        private readonly string _connectionString;

        public clsParticipanteData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL");
        }

        public async Task<List<clsParticipante>> ListaParticipantes()
        {
            var lista = new List<clsParticipante>();

            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Participantes", con);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add(new clsParticipante
                {
                    IdParticipante = Convert.ToInt32(dr["IdParticipante"]),
                    Nombre = dr["Nombre"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Email = dr["Email"].ToString(),
                    UsuarioTwitter = dr["UsuarioTwitter"].ToString(),
                    Ocupacion = dr["Ocupacion"].ToString(),
                    AvatarSeleccionado = dr["AvatarSeleccionado"].ToString(),
                    AceptaTerminos = Convert.ToBoolean(dr["AceptaTerminos"]),
                    CodigoQR = dr["CodigoQR"].ToString()
                });
            }

            return lista;
        }

        public async Task<clsParticipante?> ObtenerParticipante(int id)
        {
            clsParticipante? participante = null;

            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Participantes WHERE IdParticipante = @id", con);
            cmd.Parameters.AddWithValue("@id", id);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                participante = new clsParticipante
                {
                    IdParticipante = Convert.ToInt32(dr["IdParticipante"]),
                    Nombre = dr["Nombre"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Email = dr["Email"].ToString(),
                    UsuarioTwitter = dr["UsuarioTwitter"].ToString(),
                    Ocupacion = dr["Ocupacion"].ToString(),
                    AvatarSeleccionado = dr["AvatarSeleccionado"].ToString(),
                    AceptaTerminos = Convert.ToBoolean(dr["AceptaTerminos"]),
                    CodigoQR = dr["CodigoQR"].ToString()
                };
            }

            return participante;
        }

        public async Task<bool> CrearParticipante(clsParticipante p)
        {
            try
            {
                string qrContent =
                    $"Congreso TICs - UTL\n" +
                    $"Nombre: {p.Nombre} {p.Apellidos}\n" +
                    $"Email: {p.Email}\n" +
                    $"Twitter: {p.UsuarioTwitter ?? "N/A"}\n" +
                    $"Ocupación: {p.Ocupacion}";

                using var qrGenerator = new QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(qrData);
                using var qrBitmap = qrCode.GetGraphic(20);

                using var ms = new MemoryStream();
                qrBitmap.Save(ms, ImageFormat.Png);
                string base64QR = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                p.CodigoQR = base64QR;

                using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();

                var cmd = new SqlCommand(@"
                    INSERT INTO Participantes 
                    (Nombre, Apellidos, Email, UsuarioTwitter, Ocupacion, AvatarSeleccionado, AceptaTerminos, CodigoQR)
                    VALUES 
                    (@Nombre, @Apellidos, @Email, @UsuarioTwitter, @Ocupacion, @AvatarSeleccionado, @AceptaTerminos, @CodigoQR)",
                    con);

                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Apellidos", p.Apellidos);
                cmd.Parameters.AddWithValue("@Email", p.Email);
                cmd.Parameters.AddWithValue("@UsuarioTwitter", p.UsuarioTwitter ?? "");
                cmd.Parameters.AddWithValue("@Ocupacion", p.Ocupacion);
                cmd.Parameters.AddWithValue("@AvatarSeleccionado", p.AvatarSeleccionado);
                cmd.Parameters.AddWithValue("@AceptaTerminos", p.AceptaTerminos);
                cmd.Parameters.AddWithValue("@CodigoQR", base64QR);

                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear participante: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarParticipante(int id)
        {
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var cmd = new SqlCommand("DELETE FROM Participantes WHERE IdParticipante = @id", con);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}
