using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using System.Data.SqlClient;

namespace Registration.Data.Helpers
{
    public class DapperHelper
    {
        public string ConnectionString { get; set; }
        public DapperHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string query, int commandTimeout = 6000)
        {

            using IDbConnection connection = GetConnection();
            connection.Open();

            var resultado = await connection.QueryAsync<T>(query, null, commandType: null, commandTimeout: commandTimeout);
            connection.Close();
            return resultado.ToList();
        }

        public List<T> Execute<T>(string query, int commandTimeout = 6000)
        {
            List<T> resultado;

            using IDbConnection connection = GetConnection();
            connection.Open();

            resultado = connection.Query<T>(query, null, commandType: null, commandTimeout: commandTimeout).ToList();
            connection.Close();

            return resultado;
        }

        public async Task<int> ExecuteAsync(string query, int commandTimeout = 6000)
        {
            int resultado = 0;

            try
            {
                using IDbConnection connection = GetConnection();
                connection.Open();
                resultado = await connection.ExecuteAsync(query, null, commandType: null, commandTimeout: commandTimeout);
                connection.Close();
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
                resultado = -1;
            }




            return resultado;
        }

        public List<T> Execute<T>(string query, DynamicParameters parameters, CommandType cmdType = CommandType.StoredProcedure, int commandTimeout = 6000)
        {
            using IDbConnection connection = GetConnection();
            connection.Open();

            var resultado = connection.Query<T>(query, parameters, commandType: cmdType, commandTimeout: commandTimeout).ToList();
            connection.Close();
            return resultado;
        }

        public async Task<IEnumerable<T>> ExecuteAsync<T>(string query, DynamicParameters parameters, CommandType cmdType = CommandType.StoredProcedure, int commandTimeout = 6000)
        {
            using IDbConnection connection = GetConnection();
            connection.Open();

            var resultado = await connection.QueryAsync<T>(query, parameters, commandType: cmdType, commandTimeout: commandTimeout);
            connection.Close();
            return resultado.ToList();
        }

        public IEnumerable<T> YieldExecute<T>(string query, DynamicParameters parameters, CommandType cmdType = CommandType.StoredProcedure, int commandTimeout = 6000)
        {
            using IDbConnection connection = GetConnection();
            connection.Open();

            var resultado = connection.Query<T>(query, parameters, commandType: cmdType, commandTimeout: commandTimeout);
            connection.Close();

            foreach (var item in resultado)
            {
                yield return item;
            }
        }

        public int ExecuteScalar(string query, DynamicParameters parameters, CommandType cmdType = CommandType.StoredProcedure, int commandTimeout = 6000)
        {
            int resultado = 0;
            try
            {
                using IDbConnection connection = GetConnection();
                connection.Open();

                resultado = connection.Execute(query, parameters, commandType: cmdType, commandTimeout: commandTimeout);
                connection.Close();
            }
            catch (Exception)
            {

                resultado = -1;
            }

            return resultado;
        }

        public async Task<int> ExecuteScalarAsync(string query, DynamicParameters parameters, CommandType cmdType = CommandType.StoredProcedure, int commandTimeout = 6000)
        {
            int resultado = 0;
            try
            {
                using IDbConnection connection = GetConnection();
                connection.Open();

                resultado = await connection.ExecuteAsync(query, parameters, commandType: cmdType, commandTimeout: commandTimeout);
                connection.Close();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                resultado = -1;
            }

            return resultado;
        }

        public int ExecuteScalar(string query)
        {
            using IDbConnection connection = GetConnection();
            connection.Open();

            var resultado = connection.ExecuteScalar<int>(query);
            connection.Close();
            return resultado;
        }

        public async Task<int> ExecuteScalarAsync(string query)
        {
            using IDbConnection connection = GetConnection();
            connection.Open();

            var resultado = await connection.ExecuteScalarAsync<int>(query);
            connection.Close();
            return resultado;
        }

        public T ExecuteSingleRow<T>(string query, DynamicParameters parameters, CommandType cmdType = CommandType.StoredProcedure, int commandTimeout = 6000)
        {
            T? resultado;
            try
            {
                using IDbConnection connection = GetConnection();
                connection.Open();

                resultado = connection.Query<T>(query, parameters, commandType: cmdType, commandTimeout: commandTimeout).FirstOrDefault();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex.Message" + ex.Message);
                resultado = default(T);
            }

            return resultado;
        }

        public async Task<T> ExecuteSingleRowAsync<T>(string query, DynamicParameters parameters, CommandType cmdType = CommandType.StoredProcedure, int commandTimeout = 6000)
        {
            using IDbConnection connection = GetConnection();
            connection.Open();

            var resultado = await connection.QueryAsync<T>(query, parameters, commandType: cmdType, commandTimeout: commandTimeout);
            connection.Close();
            return resultado.FirstOrDefault();
        }
    }
}
