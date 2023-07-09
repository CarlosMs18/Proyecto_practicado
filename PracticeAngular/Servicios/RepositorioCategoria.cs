using Dapper;
using Microsoft.Data.SqlClient;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{

    public interface IRepositorioCategoria
    {
        Task ActualizarCategoria(Categoria categoria);
        Task CrearCategoria(Categoria categoria);
        Task EliminarCategoria(int id, int usuarioId);
        Task<Categoria> ObtenerCategoriaPorId(int id, int usuarioId);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, TipoOperacionEnum tipoOperacionId);
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

        public async Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(
                                                        @"SELECT * FROM
                                                        Categorias WHERE UsuarioId = @UsuarioId;",new {usuarioId});
        }


        public async Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, TipoOperacionEnum tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(
                                                        @"SELECT * FROM
                                                        Categorias WHERE UsuarioId = @UsuarioId AND TipoOperacionId = @tipoOperacionId;", new { usuarioId, tipoOperacionId});
        }


        public async Task<Categoria> ObtenerCategoriaPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(
                                                                    @"SELECT * FROM
                                                                    Categorias Where Id = @Id AND UsuarioId = @Usuarioid;",
                                                                     new {id, usuarioId});
        }


        public async Task EliminarCategoria(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"DELETE Categorias Where Id = @Id AND  UsuarioId = @UsuarioId;",
                new {id, usuarioId}
                );
        }

        public async Task ActualizarCategoria(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"UPDATE Categorias
                SET Nombre = @Nombre , TipoOperacionId = @TipoOperacionId WHERE Id = @Id", categoria);
        }
    }
}
