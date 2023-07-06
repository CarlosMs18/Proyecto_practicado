using Dapper;
using Microsoft.Data.SqlClient;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{
    public interface IRepositorioTiposCuentas
    {

    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaulConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuenta(Nombre, UsuarioId, Orden) 
                                                            VALUES(@Nombre, @UsuarioId, 0);", tipoCuenta);
        }
    }
}
