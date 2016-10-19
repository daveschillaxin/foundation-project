using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace FoundationProject.Database
{

    public abstract class Connection
    {

        private String _ConnectionString = "";

        protected String ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }
        }

        public Connection(String connectionString) 
            : base()
        {
            _ConnectionString = connectionString;
        }

        protected static IDbConnection GetOpenConnection<T>(String connectionString)
        {
            // Create and open a connection
            IDbConnection output = null;

            try
            {
                Type type = typeof(T);

                if (type.IsAssignableFrom(typeof(System.Data.OleDb.OleDbConnection)))
                {
                    output = new OleDbConnection(connectionString);
                }
                else if (type.IsAssignableFrom(typeof(System.Data.SqlClient.SqlConnection)))
                {
                    output = new SqlConnection(connectionString);
                }
                else
                {
                    throw new InvalidOperationException("Invalid output type. This class only supports SqlClientConnection and OleDbConnection classes.");
                }

                output.Open(); // open the connection before returning it
            }
            catch
            {
                if (output != null)
                    output.Dispose();
                output = null;

                throw;
            }
            return output;
        }

        public static IDbConnection BeginTransaction<T>(
            String connectionString,
            out IDbTransaction transaction)
        {
            #region Validate Arguments

            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            #endregion

            try
            {
                IDbConnection output = null;
                output = GetOpenConnection<T>(connectionString);
                transaction = output.BeginTransaction();
                return output;
            }
            catch (InvalidOperationException)
            {
                /* Parallel transactions are not supported.*/
                throw;
            }
            catch (Exception)
            {
                /* Parallel transactions are not allowed when using Multiple Active Result Sets (MARS). */
                throw;
            }
        }

        public static void CommitTransaction<T>(
            IDbConnection connection,
            IDbTransaction transaction)
        {
            #region Validate Arguments

            if (connection == null)
                throw new ArgumentNullException("connection");

            if (transaction == null)
                throw new ArgumentNullException("transaction");

            #endregion

            try
            {
                transaction.Commit();
                DisposeOpenConnection(connection);
                transaction.Dispose();
                transaction = null;
            }
            catch (InvalidOperationException)
            {
                /* The transaction has already been committed or rolled back. 
                 * The connection is broken.*/
                throw;
            }
            catch (Exception)
            {
                /* An error occurred while trying to commit the transaction. */
                throw;
            }
        }

        public static void RollbackTransaction(
            IDbConnection connection,
            IDbTransaction transaction)
        {
            #region Validate Arguments

            if (connection == null)
                throw new ArgumentNullException("connection");

            if (transaction == null)
                throw new ArgumentNullException("transaction");

            #endregion

            try
            {
                transaction.Rollback();
                DisposeOpenConnection(connection);
                transaction.Dispose();
                transaction = null;
            }
            catch (InvalidOperationException)
            {
                /* The transaction has already been committed or rolled back.
                 * The connection is broken. */
                throw;
            }
            catch (Exception)
            {
                /* An error occurred while trying to commit the transaction. */
                throw;
            }
        }

        public static Boolean TestConnection(
            IDbConnection connection)
        {
            #region Validate Argument(s)

            if (connection == null)
                throw new ArgumentNullException("connection");

            #endregion

            try
            {
                connection.Open();
                return true;
            }
            catch (InvalidOperationException)
            {
                /* Cannot open a connection without specifying a data source or server.
                 * The connection is already open. */
                return false;
            }
            catch (System.Data.SqlClient.SqlException)
            {
                /* A connection-level error occurred while opening the connection. If the Number property contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the ChangePassword method for more information. */
                return false;
            }
        }

        protected static void DisposeOpenConnection(
            IDbConnection connection)
        {
            // Close and dispose of an existing connection.
            try
            {
                if (connection != null)
                {
                    if (connection.State != System.Data.ConnectionState.Closed)
                        connection.Close();
                    else
                        connection.Dispose();
                }
                connection = null;
            }
            catch
            {
                throw;
            }
        }

        protected Boolean IsValidReturnType<T>()
        {
            if (Object.ReferenceEquals(typeof(T), typeof(Int64)) ||
                Object.ReferenceEquals(typeof(T), typeof(Int32)) ||
                Object.ReferenceEquals(typeof(T), typeof(Decimal)) ||
                Object.ReferenceEquals(typeof(T), typeof(Double)) ||
                Object.ReferenceEquals(typeof(T), typeof(Boolean)) ||
                Object.ReferenceEquals(typeof(T), typeof(String)) ||
                Object.ReferenceEquals(typeof(T), typeof(Byte[])))
            {
                return true;
            }
            return false;
        }

        protected T ReturnTyped<T>(
            ref Object input)
        {
            Object output = null;

            Type type = typeof(T);

            if (type.IsAssignableFrom(typeof(Int64)))
                output = ReturnInt64(ref input);

            else if (type.IsAssignableFrom(typeof(Int32)))
                output = ReturnInt32(ref input);

            else if (type.IsAssignableFrom(typeof(Decimal)))
                output = ReturnDecimal(ref input);

            else if (type.IsAssignableFrom(typeof(Double)))
                output = ReturnDouble(ref input);

            else if (type.IsAssignableFrom(typeof(Boolean)))
                output = ReturnBoolean(ref input);

            else if (type.IsAssignableFrom(typeof(String)))
                output = ReturnString(ref input);

            else if (type.IsAssignableFrom(typeof(DateTime)))
                output = ReturnDateTime(ref input);

            else if (type.IsAssignableFrom(typeof(Byte[])))
                output = ReturnByteArray(ref input);

            return (T)output;
        }

        #region Supported Return Types

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Int64 ReturnInt64(ref Object source)
        {
            try
            {
                return Convert.ToInt64(source, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return default(Int64);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Int32 ReturnInt32(ref Object source)
        {
            try
            {
                return Convert.ToInt32(source, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return default(Int32);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Decimal ReturnDecimal(ref Object source)
        {
            try
            {
                return Convert.ToDecimal(source, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return default(Decimal);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Double ReturnDouble(ref Object source)
        {
            try
            {
                return Convert.ToDouble(source, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return default(Double);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Boolean ReturnBoolean(ref Object source)
        {
            try
            {
                return Convert.ToBoolean(source, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return default(Boolean);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private String ReturnString(ref Object source)
        {
            try
            {
                return Convert.ToString(source, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return default(String);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private DateTime ReturnDateTime(ref Object source)
        {
            try
            {
                return Convert.ToDateTime(source, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Byte[] ReturnByteArray(ref Object source)
        {
            try
            {
                return (Byte[])source;
            }
            catch (Exception)
            {
                return new Byte[] { };
            }
        }

        #endregion

    }

}
