#define SQLCLIENTEX_REV_1 // 11/27/15

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace FoundationProject.Database
{

    public abstract class SqlClient 
        : Connection
    {

        public SqlClient(String connectionString)
            : base(connectionString)
        {
            TestConnection(connectionString);
        }

        #region Internal or non-transaction supporting methods.

        protected T ExecuteScalar<T>(
            SqlConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            params SqlParameter[] parameters)
        {
            SqlCommand cmd = null;
            Object output = null;

            try
            {
                if (!IsValidReturnType<T>()) // validate the return type
                    throw new SqlClientExException("Invalid data type T for function ExecuteScalar<T>.");

                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteScalar<T>.");

                cmd = connection.CreateCommand();
                cmd.CommandType = procedureCommandType;
                cmd.CommandText = procedureText;
                cmd.Parameters.AddRange(parameters);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                output = cmd.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }

            return ReturnTyped<T>(ref output);
            //return (T)output;
        }

        protected Int32 ExecuteNonQuery(
            SqlConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            params SqlParameter[] parameters)
        {
            SqlCommand cmd = null;
            Int32 output = 0;

            try
            {
                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteNonQuery.");

                cmd = new SqlCommand(String.Empty, connection);
                cmd.CommandType = procedureCommandType;
                cmd.CommandText = procedureText;
                cmd.Parameters.AddRange(parameters);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                output = cmd.ExecuteNonQuery(); // count of records effected
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }
            return output;

        }

        protected DataTable ExecuteQuery(
            SqlConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            params SqlParameter[] parameters)
        {
            SqlDataAdapter da = null;
            DataTable output = null;

            try
            {
                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteQuery.");

                da = new SqlDataAdapter();
                da.SelectCommand = connection.CreateCommand();
                da.SelectCommand.CommandType = procedureCommandType;
                da.SelectCommand.CommandText = procedureText;
                da.SelectCommand.Parameters.AddRange(parameters);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                output = new DataTable();
                output.Locale = System.Globalization.CultureInfo.CurrentCulture;

                da.Fill(output);
            }
            catch
            {
                if (output != null)
                    output.Dispose();
                output = null;

                throw;
            }
            finally
            {
                if (da != null)
                    da.Dispose();
                da = null;
            }
            return output;
        }

        protected T ExecuteReturnQuery<T>(
            SqlConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            SqlParameter returnParameter,
            params SqlParameter[] parameters)
        {
            SqlCommand cmd = null;
            Object output = null;

            try
            {
                #region Validate Arguments

                if (returnParameter == null)
                    throw new ArgumentNullException("returnParameter");

                #endregion

                if (!IsValidReturnType<T>()) // validate the return type
                    throw new SqlClientExException("Invalid data type T for function ExecuteReturnQuery<T>.");

                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteReturnQuery<T>.");

                cmd = connection.CreateCommand();
                cmd.CommandType = procedureCommandType;
                cmd.CommandText = procedureText;
                cmd.Parameters.Add(returnParameter);
                cmd.Parameters.AddRange(parameters);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                cmd.ExecuteNonQuery();
                output = returnParameter.Value;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }
            return (T)output;
        }

        #endregion

        #region External transaction supported methods.

        protected T ExecuteScalar<T>(
            SqlConnection connection,
            SqlTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            params SqlParameter[] parameters)
        {
            SqlCommand cmd = null;
            object output = null;

            try
            {
                if (!IsValidReturnType<T>()) // validate the return type
                    throw new SqlClientExException("Invalid data type T for function ExecuteScalar<T>.");

                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteScalar<T>.");

                if (transaction == null)
                    throw new SqlClientExException("Transaction object can not be null for function ExecuteScalar<T> supporting transaction.");

                cmd = connection.CreateCommand();
                cmd.CommandType = procedureCommandType;
                cmd.CommandText = procedureText;
                cmd.Parameters.AddRange(parameters);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                cmd.Transaction = transaction;
                output = cmd.ExecuteScalar();
                output = ReturnTyped<T>(ref output);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }
            return (T)output;
        }

        protected T ExecuteReturnQuery<T>(
            SqlConnection connection,
            SqlTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            SqlParameter returnParameter,
            params SqlParameter[] parameters)
        {
            SqlCommand cmd = null;
            Object output = null;

            try
            {
                #region Validate Arguments

                if (returnParameter == null)
                    throw new ArgumentNullException("returnParameter");

                #endregion

                if (!IsValidReturnType<T>()) // validate the return type
                    throw new SqlClientExException("Invalid data type T for function ExecuteReturnQuery<T>.");

                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteReturnQuery<T>.");

                if (transaction == null)
                    throw new SqlClientExException("Transaction object can not be null for function ExecuteReturnQuery<T> supporting transaction.");

                cmd = connection.CreateCommand();
                cmd.CommandType = procedureCommandType;
                cmd.CommandText = procedureText;
                cmd.Parameters.Add(returnParameter);
                cmd.Parameters.AddRange(parameters);

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                output = returnParameter.Value;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }
            return (T)output;
        }

        protected Int32 ExecuteNonQuery(
            SqlConnection connection,
            SqlTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            params SqlParameter[] parameters)
        {
            SqlCommand cmd = null;
            Int32 output = 0;

            try
            {
                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteNonQuery.");

                if (transaction == null)
                    throw new SqlClientExException("Transaction object can not be null for function ExecuteReturnQuery<T> supporting transaction.");

                cmd = new SqlCommand(String.Empty, connection);
                cmd.CommandType = procedureCommandType;
                cmd.CommandText = procedureText;
                cmd.Parameters.AddRange(parameters);
                cmd.Transaction = transaction;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                output = cmd.ExecuteNonQuery(); // count of records effected
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }
            return output;

        }

        protected DataTable ExecuteQuery(
            SqlConnection connection,
            SqlTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            params SqlParameter[] parameters)
        {
            SqlDataAdapter da = null;
            DataTable output = null;

            try
            {
                if (connection == null)
                    throw new SqlClientExException("Connection object can not be null for function ExecuteQuery.");

                da = new SqlDataAdapter();
                da.SelectCommand = connection.CreateCommand();
                da.SelectCommand.CommandType = procedureCommandType;
                da.SelectCommand.CommandText = procedureText;
                da.SelectCommand.Parameters.AddRange(parameters);
                da.SelectCommand.Transaction = transaction;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                output = new DataTable();
                output.Locale = System.Globalization.CultureInfo.CurrentCulture;

                da.Fill(output);
            }
            catch
            {
                if (output != null)
                    output.Dispose();
                output = null;

                throw;
            }
            finally
            {
                if (da != null)
                    da.Dispose();
                da = null;
            }
            return output;
        }

        #endregion

        #region Create Parameter +3 overload(s)

        protected SqlParameter CreateParameter(
            String parameterName,
            SqlDbType dataType, 
            Object value, 
            Int32 size,
            ParameterDirection direction)
        {
            try
            {
                return new SqlParameter(
                    parameterName, 
                    dataType, 
                    size, 
                    direction,
                    false, 
                    0, 
                    0, 
                    "", 
                    DataRowVersion.Current, 
                    value);
            }
            catch
            {
                throw;
            }
        }

        protected SqlParameter CreateParameter(
            String parameterName,
            SqlDbType dataType, 
            Object value)
        {
            try
            {
                return new SqlParameter(
                    parameterName, 
                    dataType, 
                    0, 
                    ParameterDirection.Input,
                    false, 
                    0, 
                    0, 
                    "", 
                    DataRowVersion.Current, 
                    value);
            }
            catch
            {
                throw;
            }
        }

        protected SqlParameter CreateParameter(
            String parameterName,
            SqlDbType dataType, 
            Object value, 
            Int32 size)
        {
            try
            {
                return new SqlParameter(
                    parameterName, 
                    dataType, 
                    size, 
                    ParameterDirection.Input,
                    false, 
                    0, 
                    0, 
                    "", 
                    DataRowVersion.Current, 
                    value);
            }
            catch
            {
                throw;
            }
        }

        protected SqlParameter CreateParameter(
            String parameterName, 
            Object value)
        {
            try
            {
                return new SqlParameter(
                    parameterName, value);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        public static void TestConnection(string connectionString)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                connection.Close();
                connection.Dispose();
                connection = null;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (SqlException)
            {
                throw;
            }
        }

    }

}
