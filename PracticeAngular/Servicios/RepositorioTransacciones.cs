﻿using Dapper;
using Microsoft.Data.SqlClient;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }

    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType : System.Data.CommandType.StoredProcedure
                );
        }


        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                    new
                    {
                        transaccion.Id,
                        transaccion.FechaTransaccion,
                        transaccion.Monto,
                        transaccion.CategoriaId,
                        transaccion.CuentaId,
                        transaccion.Nota,
                        montoAnterior,
                        cuentaAnteriorId
                    }, commandType: System.Data.CommandType.StoredProcedure);
            //luego de hacer esto ncesitamos un metodo apra obtener una transaccion en base a su Id,
            //Otro cosa que necesitaremos sera el tipoOperacion porque podremos mostrar el listado de categorias adecuado en formaulrio
        }


        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT Transacciones.* , cat.TipoOperacionId
                FROM Transacciones
                INNER JOIN Categorias cat
                ON cat.Id = Transacciones.CategoryId
                WHERE Transacciones.Id =@Id AND Transacciones.UsuarioId = @UsuarioId;", new { id, usuarioId });

        }


        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(
                @"SELECT t.Id, t.Monto, T.FechaTransaccion, c.Nombre as Categoria, 
                cu.Nombre as Cuenta, c.TipoOperacionId
                FROM Transacciones t   
                JOIN Categorias c
                ON c.Id = T.CategoryId
                JOIN Cuentas cu
                ON cu.Id = T.CuentaId
                WHERE t.CuentaId = @CuentaId AND T.UsuarioId = @UsuarioId AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin;",
                modelo );
        }


        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(
                @"SELECT t.Id, t.Monto, T.FechaTransaccion, c.Nombre as Categoria, 
                cu.Nombre as Cuenta, c.TipoOperacionId
                FROM Transacciones t   
                JOIN Categorias c
                ON c.Id = T.CategoryId
                JOIN Cuentas cu
                ON cu.Id = T.CuentaId
                WHERE  T.UsuarioId = @UsuarioId AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                ORDER BY T.FechaTransaccion DESC;",
                modelo);
        }


        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"
                                SELECT datediff (d, @fechaInicio, FechaTransaccion) / 7+1 as Semana, 
                                SUM(Monto) as Monto, cat.TipoOperacionId
                                FROM Transacciones
                                JOIN Categorias cat
                                ON cat.Id = Transacciones.CategoryId
                                WHERE  Transacciones.UsuarioId  = @usuarioId AND
                                FechaTransaccion BETWEEN @fechaInicio AND @fechaFin
                                GROUP BY datediff (d, @fechaInicio, FechaTransaccion) / 7, cat.TipoOperacionId;",modelo);
        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteScalarAsync("Transacciones_Borrar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
