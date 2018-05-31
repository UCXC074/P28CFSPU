using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml;



namespace P28CFSPU
{
    class DataLayerBase
    {

        private SqlConnection _connection = null;
        private string _connectionString = string.Empty;


        public string ConnectionString
        {
            set { _connectionString = value; }
            get { return _connectionString; }
        }

        public DataLayerBase()
        {
            //
            // TODO: Add constructor logic here
            //
        }



        public bool OpenConnection()
        {
            ValidateConnectionString();
            _connection = new SqlConnection();
            _connection.ConnectionString = _connectionString;
            string exx;
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                exx = ex.Message;
                _connection = null;
                return false;
            }

            return true;
        }

        public bool CloseConnection()
        {

            bool result = true;
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                _connection = null;
            }

            return result;

        }


        public void ValidateConnectionString()
        {
            if (0 == _connectionString.Length)
            {
                throw new Exception("No connection string has been supplied");
            }
        }

        public int ExecuteNonQuery(string storedProcedure, SqlParameter[] parameterArray)
        {
            int result;
            string msg;

            SqlCommand comm = new SqlCommand();

            OpenConnection();

            result = 0;

            try
            {
                comm = _connection.CreateCommand();
                comm.CommandText = storedProcedure;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = 60;

                foreach (SqlParameter param in parameterArray)
                {
                    comm.Parameters.Add(param);
                }

                result = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                comm = null;
            }

            return result;
        }

        public object ExecuteScalar(string storedProcedure, SqlParameter[] parameterArray)
        {
            object result;
            SqlCommand comm = null;

            try
            {
                comm = _connection.CreateCommand();

                comm.CommandText = storedProcedure;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = 60;

                foreach (SqlParameter param in parameterArray)
                {
                    comm.Parameters.Add(param);
                }

                result = comm.ExecuteScalar();
            }

            finally
            {
                comm = null;
            }

            return result;
        }

        public DataSet ExecuteDataSet(string storedProcedure, SqlParameter[] parameterArray)
        {
            DataSet result = new DataSet();

            SqlDataAdapter dataAdapter = null;
            dataAdapter = new SqlDataAdapter();
            SqlCommand comm = null;

            try
            {
                comm = _connection.CreateCommand();

                comm.CommandText = storedProcedure;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = 60;

                foreach (SqlParameter param in parameterArray)
                {
                    comm.Parameters.Add(param);
                }
                dataAdapter.SelectCommand = comm;
                dataAdapter.Fill(result, "data");
            }

            finally
            {
                dataAdapter = null;
                comm = null;
            }


            return result;
        }

        public SqlDataReader ExecuteReader(string storedProcedure, SqlParameter[] parameterArray)
        {
            SqlDataReader result = null;
            SqlCommand comm = null;
            comm = _connection.CreateCommand();
            string msg;

            comm.CommandText = storedProcedure;
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 60;

            foreach (SqlParameter param in parameterArray)
            {
                comm.Parameters.Add(param);
            }

            try
            {
                result = comm.ExecuteReader();
            }
            catch (Exception ex)
            {
                msg = ex
                    .Message;
            }

            finally
            {
                comm = null;
            }

            return result;
        }

        public DataTable ExecuteDataTable(string storedProcedure, SqlParameter[] parameterArray)
        {

            DataTable result = new DataTable();
            SqlDataAdapter dataAdapter = null;
            dataAdapter = new SqlDataAdapter();
            SqlCommand comm = new SqlCommand();

            OpenConnection();

            comm = _connection.CreateCommand();
            comm.CommandText = storedProcedure;
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 30;

            foreach (SqlParameter param in parameterArray)
            {
                comm.Parameters.Add(param);
            }

            try
            {

                dataAdapter.SelectCommand = comm;
                dataAdapter.Fill(result);
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                throw (se);
            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                CloseConnection();
                dataAdapter = null;
                comm = null;
            }

            return result;


        }


        public DataTable ExecuteDataTable(string storedProcedure)
        {

            DataTable result = new DataTable();
            SqlDataAdapter dataAdapter = null;
            dataAdapter = new SqlDataAdapter();
            SqlCommand comm = new SqlCommand();

            OpenConnection();

            comm = _connection.CreateCommand();
            comm.CommandText = storedProcedure;
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 30;

            try
            {

                dataAdapter.SelectCommand = comm;
                dataAdapter.Fill(result);
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                throw (se);
            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                CloseConnection();
                dataAdapter = null;
                comm = null;
            }

            return result;


        }

        public DataTable ExecuteDataTableEmbedded(string sql)
        {

            DataTable result = new DataTable();
            SqlDataAdapter dataAdapter = null;
            dataAdapter = new SqlDataAdapter();
            SqlCommand comm = new SqlCommand();

            OpenConnection();

            comm = _connection.CreateCommand();
            comm.CommandText =  sql;
            comm.CommandType = CommandType.Text ;
            comm.CommandTimeout = 30;

            try
            {
                dataAdapter.SelectCommand = comm;
                dataAdapter.Fill(result);
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                throw (se);
            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                CloseConnection();
                dataAdapter = null;
                comm = null;
            }

            return result;


        }


        
        public SqlDataReader ExecuteReader(string storedProcedure)
        {
            SqlDataReader result;
            SqlCommand comm = null;
            comm = _connection.CreateCommand();
            comm.CommandText = storedProcedure;
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 60;

            try
            {
                result = comm.ExecuteReader();
            }

            finally
            {
                comm = null;
            }

            return result;
        }



        public System.Xml.XmlReader ExecuteXMLReader(string storedProcedure)
        {
            

            System.Xml.XmlReader Result;


            SqlCommand comm = null;
            comm = _connection.CreateCommand();
            comm.CommandText = storedProcedure;
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 60;

            try
            {
                Result = comm.ExecuteXmlReader();

            }

            finally
            {
                comm = null;
            }

            return Result;
        }




        public System.Xml.XmlReader ExecuteXML(string storedProcedure, SqlParameter[] parameterArray)
        {

            System.Xml.XmlReader result;

            SqlCommand comm = new SqlCommand();

            OpenConnection();

            comm = _connection.CreateCommand();
            comm.CommandText = storedProcedure;
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 30;

            foreach (SqlParameter param in parameterArray)
            {
                comm.Parameters.Add(param);
            }

            try
            {

                result = comm.ExecuteXmlReader();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                throw (se);
            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                CloseConnection();
                comm = null;
            }

            return result;


        }




//SQLXMLReader = SQLCmd.ExecuteXmlReader()
//While (SQLXMLReader.Read)
//    MsgBox(SQLXMLReader.ReadOuterXml)
//End While






    }

}
