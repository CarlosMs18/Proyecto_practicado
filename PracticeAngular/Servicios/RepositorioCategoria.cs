using Dapper;
using Microsoft.Data.SqlClient;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{

    public interface IRepositorioCategoria
    {
        Task CrearCategoria(Categoria categoria);
    }

    public class RepositorioCategoria : IRepositorioCategoria
    {
        private readonly string connectionString;
        public RepositorioCategoria(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CrearCategoria(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                 @"INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId) 
                 VALUES (@Nombre, @TipoOperacionId, @UsuarioId);
                 SELECT SCOPE_IDENTITY();", categoria);

            categoria.Id = id;
        }
    }
}
