#define OLEDBEX_REV_1 // 11/27/15
#define OLEDBEX_REV_2 // 12/01/15
/* Added GetOleDbSchema() */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace FoundationProject.Database
{

    public abstract class OleDb
        : Connection
    {

        protected OleDb(String connectionString)
            : base(connectionString)
        {
        }

        #region Internal or non-transaction supporting methods.

        protected T ExecuteScalar<T>(
            OleDbConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            params OleDbParameter[] parameters)
        {
            OleDbCommand cmd = null;
            Object output = null;

            try
            {
                if (!IsValidReturnType<T>()) // validate the return type
                    throw new OleDbExException("Invalid data type T for function ExecuteScalar<T>.");

                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteScalar<T>.");

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
            OleDbConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            params OleDbParameter[] parameters)
        {
            System.Data.OleDb.OleDbCommand cmd = null;
            Int32 output = 0;

            try
            {
                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteNonQuery.");

                cmd = new OleDbCommand(String.Empty, connection);
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
            OleDbConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            params OleDbParameter[] parameters)
        {
            OleDbDataAdapter da = null;
            DataTable output = null;

            try
            {
                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteQuery.");

                da = new OleDbDataAdapter();
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
            OleDbConnection connection, 
            String procedureText,
            CommandType procedureCommandType,
            OleDbParameter returnParameter,
            params OleDbParameter[] parameters)
        {
            OleDbCommand cmd = null;
            Object output = null;

            try
            {
                #region Validate Arguments

                if (returnParameter == null)
                    throw new ArgumentNullException("returnParameter");

                #endregion

                if (!IsValidReturnType<T>()) // validate the return type
                    throw new OleDbExException("Invalid data type T for function ExecuteReturnQuery<T>.");

                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteReturnQuery<T>.");

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
            OleDbConnection connection,
            OleDbTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            params OleDbParameter[] parameters)
        {
            OleDbCommand cmd = null;
            object output = null;

            try
            {
                if (!IsValidReturnType<T>()) // validate the return type
                    throw new OleDbExException("Invalid data type T for function ExecuteScalar<T>.");

                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteScalar<T>.");

                if (transaction == null)
                    throw new OleDbExException("Transaction object can not be null for function ExecuteScalar<T> supporting transaction.");

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
            OleDbConnection connection,
            OleDbTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            OleDbParameter returnParameter,
            params OleDbParameter[] parameters)
        {
            OleDbCommand cmd = null;
            Object output = null;

            try
            {
                #region Validate Arguments

                if (returnParameter == null)
                    throw new ArgumentNullException("returnParameter");

                #endregion

                if (!IsValidReturnType<T>()) // validate the return type
                    throw new OleDbExException("Invalid data type T for function ExecuteReturnQuery<T>.");

                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteReturnQuery<T>.");

                if (transaction == null)
                    throw new OleDbExException("Transaction object can not be null for function ExecuteReturnQuery<T> supporting transaction.");

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
            OleDbConnection connection,
            OleDbTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            params OleDbParameter[] parameters)
        {
            OleDbCommand cmd = null;
            Int32 output = 0;

            try
            {
                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteNonQuery.");

                if (transaction == null)
                    throw new OleDbExException("Transaction object can not be null for function ExecuteReturnQuery<T> supporting transaction.");

                cmd = new OleDbCommand(String.Empty, connection);
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

        protected System.Data.DataTable ExecuteQuery(
            OleDbConnection connection,
            OleDbTransaction transaction, 
            String procedureText,
            CommandType procedureCommandType,
            params OleDbParameter[] parameters)
        {
            OleDbDataAdapter da = null;
            DataTable output = null;

            try
            {
                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteQuery.");

                da = new OleDbDataAdapter();
                da.SelectCommand = connection.CreateCommand();
                da.SelectCommand.CommandType = procedureCommandType;
                da.SelectCommand.CommandText = procedureText;
                da.SelectCommand.Parameters.AddRange(parameters);
                da.SelectCommand.Transaction = transaction;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                output = new System.Data.DataTable();
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

        protected OleDbParameter CreateParameter(
            String parameterName,
            OleDbType dataType, 
            Object value, 
            Int32 size,
            ParameterDirection direction)
        {
            try
            {
                return new OleDbParameter(
                    parameterName, dataType, size, direction,
                    false, 0, 0, "", System.Data.DataRowVersion.Current, value);
            }
            catch
            {
                throw;
            }
        }

        protected OleDbParameter CreateParameter(
            String parameterName,
            OleDbType dataType, 
            Object value)
        {
            try
            {
                return new OleDbParameter(
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

        protected OleDbParameter CreateParameter(
            String parameterName,
            OleDbType dataType, 
            Object value, 
            Int32 size)
        {
            try
            {
                return new OleDbParameter(
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

        protected OleDbParameter CreateParameter(
            String parameterName, 
            Object value)
        {
            try
            {
                return new OleDbParameter(
                    parameterName, 
                    value);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        protected DataTable GetOleDbSchema(
            OleDbConnection connection)
        {
            DataTable output = null;

            try
            {
                if (connection == null)
                    throw new OleDbExException("Connection object can not be null for function ExecuteQuery.");

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                output = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (output == null)
                    output = new DataTable();

                output.Locale = System.Globalization.CultureInfo.CurrentCulture;
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
            }
            return output;
        }

    }

}
