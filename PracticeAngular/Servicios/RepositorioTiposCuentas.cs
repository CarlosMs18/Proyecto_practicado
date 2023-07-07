using Dapper;
using Microsoft.Data.SqlClient;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> ExisteTipoCuenta(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> ObtenerTiposCuentas(int usuarioId);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<IEnumerable<TipoCuenta>> ObtenerTiposCuentas(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                        FROM TiposCuenta
                                                        WHERE UsuarioId = @UsuarioId
                                                        ORDER BY Orden", new { usuarioId });
        }
            public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuenta(Nombre, UsuarioId, Orden) 
                                                            VALUES(@Nombre, @UsuarioId, 0);
                                                            SELECT SCOPE_IDENTITY()", tipoCuenta);

            tipoCuenta.Id = id;
        }


        public async Task<bool> ExisteTipoCuenta(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"
                                                SELECT 1 FROM TiposCuenta 
                                                WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;", new { nombre, usuarioId });
            return existe == 1;
        }
    }
}
