using Dapper;
using Microsoft.Data.SqlClient;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{
    public interface IRepositorioCuenta
    {
        Task CrearCuenta(Cuenta cuenta);
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
    }
}
