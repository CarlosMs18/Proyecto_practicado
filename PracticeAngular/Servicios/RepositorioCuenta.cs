using Dapper;
using Microsoft.Data.SqlClient;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{
    public interface IRepositorioCuenta
    {
        Task ActualizarCuenta(Cuenta cuenta);
        Task CrearCuenta(Cuenta cuenta);
        Task EliminarCuenta(int id);
        Task<Cuenta> ObtenerCuentaPorId(int id, int usuarioId);
        Task<IEnumerable<Cuenta>> ObtenerCuentas(int usuarioId);
    }

    public class RepositorioCuenta : IRepositorioCuenta 
    {
        private readonly string connectionString;

        public RepositorioCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task CrearCuenta(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Cuentas (Nombre, TipoCuentaId, Balance, Descripcion) 
                   VALUES (@Nombre, @TipoCuentaId , @Balance, @Descripcion);
                   SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }
        
        public async Task<IEnumerable<Cuenta>> ObtenerCuentas(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECT c.Id , c.Nombre, c.Balance, tc.Nombre as TipoCuenta
                                                      FROM Cuentas c
                                                      INNER JOIN TiposCuenta tc
                                                      ON tc.Id = c.TipoCuentaId
                                                      WHERE tc.UsuarioId = @UsuarioId;",new {usuarioId});
            

        }

        public async Task<Cuenta> ObtenerCuentaPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT c.Id , c.Nombre, c.Balance, c.Descripcion, tc.Nombre as TipoCuenta 
                                                                    FROM Cuentas c
                                                                    JOIN TiposCuenta tc
                                                                    ON tc.Id = c.TipoCuentaId
                                                                    WHERE c.Id =@Id  AND tc.UsuarioId = @UsuarioId", new {id, usuarioId});
        }


        public async Task ActualizarCuenta(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas 
                                        SET Nombre = @Nombre, Balance = @Balance , Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                                        WHERE Id = @Id;", cuenta);


        }
        public async Task EliminarCuenta(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Cuentas WHERE Id =@Id",new {id});
        }

    }
}
