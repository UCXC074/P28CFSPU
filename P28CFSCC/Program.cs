using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;



namespace P28CFSPU
{
    class Program
    {


        static ExportDataCall myDataCalls = new ExportDataCall();
        private static string _connectionString;
        private static string _connectionAsyncString;
        const string pi = "|";
        private static System.IO.TextWriter SOPOWriter;
        private static string fileName;
        private static string fileNameMoveTo;
        private static string _errMsg;
        private string pathName;
        private static string pathNameMoveTo;
        private static string moveFile;
        private static StringBuilder sb;
        private static DataTable myTable;
        private static DataTable runDateTB;
        private XmlDataDocument xmlCntry;

        //up_ge_find_cc_changes_s

        static void Main(string[] args)
        {
            int _rtn;
            DateTime _dt;

            try
            {
                _rtn = getConnectionString();
                myDataCalls.ConnectionString = _connectionString;
            }
            catch (Exception ex)
            {
                
                // Writte error to error file
            }

            myTable = new DataTable();
            runDateTB = new DataTable();

            try
            {
                runDateTB = myDataCalls.getLastRunDate("");
                _dt = (DateTime) runDateTB.Rows[0].ItemArray[0];

                myTable = myDataCalls.getProlileUpdates(_dt);
                //myTable = myDataCalls.getLevelOneGECreates("");  //only for testing
                ProcessUpdates("");

                runDateTB = myDataCalls.getLastRunDate("REMOVE");
                _dt = (DateTime)runDateTB.Rows[0].ItemArray[0];


                // get the deletes
                myTable = myDataCalls.getProlileDeletes(_dt);
                ProcessUpdates("DELETE");

                
            }
            catch (Exception ex)
            {
                
                // write to error log
                _errMsg = ex.Message;
                WriteErrorsToLog(_errMsg, "Error in Main module");
            }

        }



        private static XmlDocument RunCommandAsynchronously(string connectionString, int intIDN)
        {

            XmlDocument ProfileDOM = new XmlDocument();

            SqlParameter SqlParm1 = new SqlParameter("@p_Person_IDN", SqlDbType.Int);
            
            SqlParm1.Value = intIDN;
            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {


                SqlCommand command = new SqlCommand();

                command = connection.CreateCommand();
                command.CommandText = "up_SOPO_Profile_Load_s";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(SqlParm1);

                try
                {
                    connection.Open();
                    IAsyncResult result = command.BeginExecuteXmlReader();


                    int count = 0;
                    while (!result.IsCompleted)
                    {
                        //Console.WriteLine("Waiting ({0})", count++);
                        System.Threading.Thread.Sleep(100);
                    }

                    XmlReader reader = command.EndExecuteXmlReader(result);

                    StringBuilder XmlStringB = new StringBuilder();

                    reader.MoveToContent();
                    XmlStringB.Append(reader.ReadOuterXml());


                    while (reader.Read())
                    {
                        XmlStringB.Append(reader.ReadOuterXml());
                    }

                    ProfileDOM.LoadXml(XmlStringB.ToString());
                }
                catch (Exception ex)
                {
                    WriteErrorsToLog(_errMsg, "Error in RunCommandAsynchronously Function");
                }


                connection.Close();
                command.Dispose();

                return ProfileDOM;

            }
        }



        private static XmlDocument RunCommandXML(string connectionString, int intIDN)
        {

            XmlDocument ProfileDOM = new XmlDocument();

            SqlParameter SqlParm1 = new SqlParameter("@p_Person_IDN", SqlDbType.Int);

            SqlParm1.Value = intIDN;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {


                SqlCommand command = new SqlCommand();

                command = connection.CreateCommand();
                command.CommandText = "up_SOPO_Profile_Load_s";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(SqlParm1);

                try
                {
                    connection.Open();
                    //IAsyncResult result = command.BeginExecuteXmlReader();
                    XmlReader reader = command.ExecuteXmlReader();


                    StringBuilder XmlStringB = new StringBuilder();

                    reader.MoveToContent();
                    XmlStringB.Append(reader.ReadOuterXml());

                    while (reader.Read())
                    {
                        XmlStringB.Append(reader.ReadOuterXml());
                    }

                    ProfileDOM.LoadXml(XmlStringB.ToString());
                }
                catch (Exception ex)
                {
                    WriteErrorsToLog(_errMsg, "Error in RunCommandXML Function");
                }


                connection.Close();
                command.Dispose();
                

                return ProfileDOM;

            }
        }


        private static void ProcessUpdates(string _type)
        {

            
            String PersIDN;
            string _action;
            int _ccidn;
            Int32 idn;
            Int32 cntr, cntr2 = 1, cntr3 = 2, cntr4 = 0;
            Int32 loopcnt;
            bool geus = false;
            string pathName;
            string nationalPersonID = "";
            string lastName = "";
            string firstName = "";
            bool setProcessFlag = true;

            XmlDocument ProfileDOM = new XmlDocument();

            cntr = myTable.Rows.Count;
            pathName = ConfigurationManager.AppSettings["PathName"];

            fileName = CreateFileName(_type) + ".txt";
            fileName = pathName + "\\" + fileName;

            //fileNameMoveTo = CreateFileName(1) + ".txt";
            //fileNameMoveTo = pathNameMoveTo + "\\" + fileName;


            SOPOWriter = new System.IO.StreamWriter(fileName);

            sb = new StringBuilder();



            if (_type.ToUpper() == "DELETE")
            {
                BuildHeader();                
            }
            else
            {
                BuildHeaderImplied();
            }


            foreach (DataRow row in myTable.Rows)
            {
                _action = row.ItemArray[0].ToString();
                PersIDN = row.ItemArray[1].ToString();

                idn = Convert.ToInt32(PersIDN);
                //ProfileDOM = RunCommandAsynchronously(_connectionAsyncString, idn);
                


                if (_action.ToUpper() == "DELETE")
                {
                    _action = "Delete";
                }
                else
                {
                    _action = "";
                }


                try
                {

                    if (_action == "Delete")
                    {
                        nationalPersonID = row.ItemArray[2].ToString();
                        lastName = row.ItemArray[3].ToString();
                        firstName = row.ItemArray[4].ToString();
                        nationalPersonID = InsertRemoveprofile(nationalPersonID, lastName, firstName, true, _action);
                    }
                    else
                    {

                        ProfileDOM = RunCommandXML(_connectionString, idn);
                        nationalPersonID = Insertprofile(ProfileDOM, true, _action);
                    }
                }
                catch (Exception ex)
                {
                    WriteErrorsToLog( ex.Message, idn.ToString());
                    setProcessFlag = false;
                }
                

                cntr2++;
                
                Console.WriteLine("Processing Profile: ({0})", cntr4++);


                if (setProcessFlag)
                {
                    try
                    {
                        int rtn;
                        rtn = myDataCalls.ProfileProcessUpdate(idn);
                    }
                    catch (Exception ex)
                    {

                        WriteErrorsToLog("Process Flag was not set for: ", idn.ToString());
                    }
                }

                setProcessFlag = true;
            }

            SOPOWriter.Close();

        }


        private static void ProcessDeletes()
        {
            String PersIDN;
            string _action;
            int _ccidn;
            Int32 idn;
            Int32 cntr, cntr2 = 1, cntr3 = 2, cntr4 = 0;
            Int32 loopcnt;
            bool geus = false;
            string pathName;
            string nationalPersonID = "";
            bool setProcessFlag = true;

            XmlDocument ProfileDOM = new XmlDocument();

            cntr = myTable.Rows.Count;
            pathName = ConfigurationManager.AppSettings["PathName"];

            fileName = CreateFileName("") + ".txt";
            fileName = pathName + "\\" + fileName;


            SOPOWriter = new System.IO.StreamWriter(fileName);

            sb = new StringBuilder();

            BuildHeader();

            foreach (DataRow row in myTable.Rows)
            {
                _action = row.ItemArray[0].ToString();
                PersIDN = row.ItemArray[1].ToString();
                idn = Convert.ToInt32(PersIDN);
                //ProfileDOM = RunCommandAsynchronously(_connectionAsyncString, idn);
                ProfileDOM = RunCommandXML(_connectionString, idn);


                if (_action == "Remove")
                {
                    _action = "Delete";
                }


                try
                {
                    nationalPersonID = Insertprofile(ProfileDOM, true, _action);
                }
                catch (Exception ex)
                {
                    WriteErrorsToLog(ex.Message, idn.ToString());
                    setProcessFlag = false;
                }

                cntr2++;

                Console.WriteLine("Processing Profile: ({0})", cntr4++);


                if (setProcessFlag)
                {
                    try
                    {
                        int rtn;
                        rtn = myDataCalls.ProfileProcessUpdate(idn);
                    }
                    catch (Exception ex)
                    {

                        WriteErrorsToLog("Process Flag was not set for: ", idn.ToString());
                    }
                }

                setProcessFlag = true;
            }

            SOPOWriter.Close();

        }



        private static string Insertprofile(XmlDocument xdPortrait, Boolean topheader, string action)
        {

            //DOM Element Vars

            string strID = "";

            string strTrlType = "";
            //string strEmpID = "";
            string strMstrEmpID = "";
            //string strMstrPin = "";

            // Org and Sub must be converted to Soujourn Org and Sub ID's
            string strOrgID = "General Electric";
            string strSubOrgID = "";
            string strSubOrgName = "";

            string strLName = "";
            string strFName = "";
            string strMName = "";
            string strPName = "";
            string strSName = "";

            string strCRScd = "";
            string strCVW = "";

            string strWEmail = "";
            string _ccNumber;

            //Credit Cards
            //string StrCC2(0, 0) ;
            string[,] StrCC = new string[3, 4];
            string[] strTA = new string[1];


            string strOrgDvsn = "";
            string strOrgDept = "";
            string strOrgUnit = "";
            string strOrgGrp = "";
            string strOrgDvsnName = "";
            string strOrgDeptName = "";
            string strOrgUnitName = "";
            string strOrgGrpName = "";
            string strWACntryID = "";
            string strWABusID = "";

            string strDK = "";
            string strLegalEntity = "";
            string strLegalEntityName = "";
            string strCostCntr = "";
            string strCostCntrName = "";
            string str_tl_parent_bus_id = "";
            string str_tl_parent_bus_name = "";
            string str_tl_business_id = "";
            string str_tl_business_name = "";
            string str_dk_anc_1 = "";
            string str_dk_anc_2 = "";
            string str_dk_anc_3 = "";
            string str_dk_anc_4 = "";
            string _suborg = "";

            string str_gstin = "";
            string str_gstin_LegalEntity = "";
            string str_gstin_phone = "";
            string str_gstin_email = "";
            string str_gstin_add1 = "";


            XmlNode Person = xdPortrait.SelectSingleNode("Profile/Person");


            foreach (XmlNode elem in Person.ChildNodes)
            {


                switch (elem.Name)
                {
                    case "national_person_id":
                        strMstrEmpID = elem.InnerXml;
                        break;
                    case "emp_id":
                        strMstrEmpID = elem.InnerXml;
                        break;
                    case "CRS_Client_cd":
                        strCRScd = elem.InnerXml;
                        break;
                    case "person_nm_sfx_cd":
                        strSName = elem.InnerXml;
                        break;
                    case "person_nm_pfx_cd":
                        strPName = elem.InnerXml;
                        break;
                    case "last_nm":
                        strLName = elem.InnerXml;
                        break;
                    case "first_nm":
                        strFName = elem.InnerXml;
                        break;
                    case "middle_nm":
                        strMName = elem.InnerXml;
                        break;
                    case "email_address_txt":
                        ////////////////////////////////////////////////////////////////////////
                        //
                        //  Temp Code
                        //
                        ////////////////////////////////////////////////////////////////////////
                        //strWEmail = "globalsupport@carlsonwagonlit.com"; //elem.InnerXml;
                        strWEmail = elem.InnerXml;
                        break;
                    case "vip":
                        strTrlType = elem.InnerXml;
                        break;
                    
                    case "cvw":
                        strCVW = elem.InnerXml.ToUpper();
                        break;
                    

                    default:

                        break;
                }


            }


            try
            {


                //strID = xdPortrait.SelectSingleNode("Type").Value ?? String.Empty; 
                //strID = xdPortrait.SelectSingleNode("somepath") != null ? xdPortrait.SelectSingleNode("somepath").Value : "";



                strID = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("person_idn").Value;
                strOrgID = "General Electric";
                strSubOrgID = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_sub_cd").Value;
                strSubOrgName = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_sub_name").Value;
                //strTrlType = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("group_lvl").Value;

                strOrgDvsn = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_dvsn_cd").Value;
                strOrgDvsnName = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_dvsn_name").Value;

                strOrgDept = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_dept_cd").Value;
                strOrgDeptName = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_dept_name").Value;

                strOrgUnit = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_unit_cd").Value;
                strOrgUnitName = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_unit_name").Value;

                strOrgGrp = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_grp_cd").Value;
                strOrgGrpName = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("org_grp_name").Value;

                strWACntryID = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("webaccess_cntry_id") != null ? xdPortrait.SelectSingleNode("//Profile/Person/OrgDept").Attributes.GetNamedItem("webaccess_cntry_id").Value : "";
                strWABusID = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("webaccess_bus_id") != null ? xdPortrait.SelectSingleNode("//Profile/Person/OrgDept").Attributes.GetNamedItem("webaccess_bus_id").Value : "";

                //strTrlType += "-" + strSubOrgID + "-S";

                if (strSubOrgID == "GB")
                {
                    strSubOrgID = "UK";
                }

                if ( strTrlType == "Officer")
                {
                    strTrlType = "Officers";
                }

            }
            catch (XmlException)
            {

                throw;
            }

            catch (Exception)
            {

            }


            ////////////////////////////////////////////////////////////////////////////
            try
            {
                XmlNode anc = xdPortrait.SelectSingleNode("Profile/Person/Anc_Data");

                foreach (XmlNode elem in anc.ChildNodes)
                {
                    switch (elem.Name)
                    {
                        case "dk_number":
                            strDK = elem.InnerXml;
                            break;
                        case "legal_entity_id":
                            strLegalEntity = elem.InnerXml;
                            break;
                        case "legal_entity_name":
                            strLegalEntityName = CleanString(elem.InnerXml);
                            strLegalEntityName.Replace("&amp", "&");
                            break;
                        case "tl_cost_center_id":
                            strCostCntr = elem.InnerXml;
                            break;
                        case "tl_cost_center_name":
                            strCostCntrName = CleanString(elem.InnerXml);
                            break;
                        case "tl_parent_bus_id":
                            str_tl_parent_bus_id = elem.InnerXml;
                            break;
                        case "tl_parent_bus_name":
                            str_tl_parent_bus_name = CleanString(elem.InnerXml);
                            break;
                        case "tl_business_id":
                            str_tl_business_id = elem.InnerXml;
                            break;
                        case "tl_business_name":
                            str_tl_business_name = CleanString(elem.InnerXml);
                            break;
                        case "dk_info_1":
                            str_dk_anc_1 = CleanString(elem.InnerXml);
                            break;
                        case "dk_info_2":
                            str_dk_anc_2 = CleanString(elem.InnerXml);
                            break;
                        case "dk_info_3":
                            str_dk_anc_3 = CleanString(elem.InnerXml);
                            break;
                        case "dk_info_4":
                            str_dk_anc_4 = CleanString(elem.InnerXml);
                            break;
                        default:

                            break;
                    }


                }
            }
            catch (Exception ex)
            {

            }



            try
            {
                XmlNode anc = xdPortrait.SelectSingleNode("Profile/Person/GSTIN_Data");

                foreach (XmlNode elem in anc.ChildNodes)
                {
                    switch (elem.Name)
                    {
                        case "gstin":
                            str_gstin = elem.InnerXml;
                            break;
                        case "legalentityname":
                            str_gstin_LegalEntity = elem.InnerXml;
                            break;
                        case "busphone":
                            str_gstin_phone = elem.InnerXml;
                            break;
                        case "busemail":
                            str_gstin_email = elem.InnerXml;
                            break;
                        case "busaddress":
                            str_gstin_add1 = elem.InnerXml;
                            break;
                        default:

                            break;
                    }


                }
            }
            catch (Exception ex)
            {

            }

            ////////////////////////////////////////////////////////////////////////////




            // Build the file using a StringBuilder 
            try
            {

                ////////////////////////////////////////////////////////////////////////////
                //   The is the Base file or miniumal file requirements to create a Profile
                ///////////////////////////////////////////////////////////////////////////

                _suborg = strSubOrgID + "-" + CleanString(strOrgDvsnName, "", true);

                if (_suborg == "-")
                {
                    _suborg = "";
                }

                if (action == "Delete")
                {
                    sb.Append(action);
                    sb.Append(pi);
                }


                sb.Append(strOrgID);
                sb.Append(pi);
                //sb.Append(strOrgDvsnName + "-"+strSubOrgID);


                sb.Append(_suborg);
                sb.Append(pi);
                sb.Append(strTrlType);
                sb.Append(pi);
                sb.Append(strMstrEmpID);
                sb.Append(pi);
                sb.Append(strFName);
                sb.Append(pi);
                sb.Append(strLName);
                sb.Append(pi);
                sb.Append(strWEmail);
                sb.Append(pi);


                sb.Append(strDK);
                sb.Append(pi);
                sb.Append(CleanReportingFields(maxStringLen(strLegalEntity, 50)));
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strLegalEntityName, 50)));
                sb.Append(pi);
                sb.Append(strCostCntr);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strCostCntrName, 50)));
                sb.Append(pi);
                sb.Append(str_tl_parent_bus_id);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(str_tl_parent_bus_name, 50)));
                sb.Append(pi);
                sb.Append(str_tl_business_id);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(str_tl_business_name, 50)));
                sb.Append(pi);
                sb.Append(strSubOrgID);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strSubOrgName, 50)));
                sb.Append(pi);
                sb.Append(strOrgDept);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strOrgDeptName, 50)));
                sb.Append(pi);
                sb.Append(strOrgDvsn);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strOrgDvsnName, 50)));
                sb.Append(pi);
                sb.Append(strOrgUnit);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strOrgUnitName, 50)));
                sb.Append(pi);
                sb.Append(strOrgGrp);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strOrgGrpName, 50)));
                sb.Append(pi);
                sb.Append(strWACntryID);
                sb.Append(pi);
                sb.Append(maxStringLen(strWABusID, 50));
                sb.Append(pi);
                sb.Append(maxStringLen(str_dk_anc_1, 50));
                sb.Append(pi);
                sb.Append(maxStringLen(str_dk_anc_2, 50));
                sb.Append(pi);
                sb.Append(maxStringLen(str_dk_anc_3, 50));
                sb.Append(pi);
                sb.Append(maxStringLen(str_dk_anc_4, 50));
                sb.Append(pi);
                sb.Append(pi);     // Place holders for DK info 5
                sb.Append(pi);     // Place holders for DK info 6
                sb.Append(pi);     // Place holders for DK info 7
                sb.Append(strMstrEmpID);
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strOrgDeptName, 50)));
                sb.Append(pi);
                sb.Append(CleanString(maxStringLen(strOrgDvsnName, 50)));
                sb.Append(pi);
                sb.Append(strCVW);
                sb.Append(pi);
                // Business Segment Code 
                sb.Append(strOrgDept);
                sb.Append(pi);
                //Industry Focus Code 
                sb.AppendLine(strOrgDvsn);


                sb.Append(pi);
                //Industry Focus Code 
                sb.AppendLine(strOrgDvsn);

                //sb.Append(pi);
                //sb.AppendLine(str_gstin_email);
                //sb.Append(pi);
                //sb.AppendLine(str_gstin_phone);
                //sb.Append(pi);
                //sb.AppendLine(str_gstin_add1);
                //sb.Append(pi);
                //sb.AppendLine(str_gstin_LegalEntity);
                //sb.Append(pi);
                //sb.AppendLine(str_gstin);
                
                //sb.Append(pi);
                //sb.Append("\n");

                SaveToFile(fileName);
                

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                throw ex;

            }

            return strMstrEmpID;


        }



        private static string InsertRemoveprofile(string _id, string _ln, string _fn, Boolean topheader, string action)
        //private static string InsertRemoveprofile(XmlDocument xdPortrait, Boolean topheader, string action)
        {

            //DOM Element Vars

            string strID = "";

            string strTrlType = "";
            //string strEmpID = "";
            string strMstrEmpID = "";
            //string strMstrPin = "";

            // Org and Sub must be converted to Soujourn Org and Sub ID's
            string strOrgID = "General Electric";
            string strSubOrgID = "";

            string strLName = "";
            string strFName = "";
            string strMName = "";

            string strWEmail = "";
            string _suborg = "";

            strMstrEmpID = _id;
            strMstrEmpID = _id;
            strLName = _ln;
            strFName = _fn;



            try
            {


                //strID = xdPortrait.SelectSingleNode("Type").Value ?? String.Empty; 
                //strID = xdPortrait.SelectSingleNode("somepath") != null ? xdPortrait.SelectSingleNode("somepath").Value : "";


                //strID = xdPortrait.SelectSingleNode("//Profile/Person").Attributes.GetNamedItem("person_idn").Value;
                strOrgID = "General Electric";
                strSubOrgID = "";
                strTrlType = "";
              

            }
            catch (XmlException)
            {

                throw;
            }

            catch (Exception)
            {

            }


            ////////////////////////////////////////////////////////////////////////////


            // Build the file using a StringBuilder 
            try
            {

                ////////////////////////////////////////////////////////////////////////////
                //   The is the Base file or miniumal file requirements to create a Profile
                ///////////////////////////////////////////////////////////////////////////

                _suborg = "";

                sb.Append(action);
                sb.Append(pi);
                sb.Append(strOrgID);
                sb.Append(pi);
                sb.Append(_suborg);
                sb.Append(pi);
                sb.Append(strTrlType);
                sb.Append(pi);
                sb.Append(strMstrEmpID);
                sb.Append(pi);
                sb.Append(strFName);
                sb.Append(pi);
                sb.AppendLine(strLName);

                SaveToFile(fileName);


            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                throw ex;

            }

            return strMstrEmpID;


        }



        private static string CreateFileName(string _type)
        { 
            string outstring;

            
            if (_type == "DELETE")
            {
                outstring = "REMOVE_GE_PU_" + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString();
            }
            else
            {
                outstring = "GE_PU_" + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString();
            }

            

            return outstring;
        }

        private static void SaveToFile(string fileName)
        {

            SOPOWriter.Write(sb.ToString());
            SOPOWriter.Flush();
            sb.Clear();
            //w.Close(); 
        }


        private static string maxStringLen(string input, int maxlen)
        {
            int ilen;

            if (input.Length < 1)
            {
                return "";
            }

            ilen = (input.Length < maxlen) ? input.Length : maxlen;

            input = input.Substring(0, ilen);

            return input;
        }

        private static string CleanString(string strInputString, string replacement, bool removenextspace)
        {

            string strChar;
            string strHoldString = "";
            int intX;
            int ilen;
            bool hitit = false;



            if (strInputString == null)
            {
                strInputString = "";
            }


            ilen = strInputString.Length;



            if (strInputString.Length > 0)
            {
                for (intX = 0; intX < ilen; intX++)
                {
                    strChar = strInputString.Substring(intX, 1);
                    switch (strChar)
                    {
                        
                        case "(":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case ")":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "!":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "#":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "$":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "%":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "^":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case ",":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "?":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "[":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "]":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "{":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "}":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case "\\":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case ":":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        case ";":
                            strHoldString = strHoldString + replacement;
                            hitit = true;
                            break;
                        default:
                            if (hitit)
                            {
                                if (strChar == " ")
                                {
                                    //strHoldString = strHoldString + ;
                                }
                            }
                            else
                            {
                                strHoldString = strHoldString + strChar;
                            }
                            hitit = false;
                            break;
                    }

                    hitit = false;

                }
            }

            return strHoldString;


        }

        private static string CleanString(string strInputString, string replacement)
        {
            return CleanString(strInputString, " ", false);
        }


        private static string CleanString(string strInputString)
        {

            return CleanString(strInputString, " ");
        }

        private static void BuildHeader()
        {
            // This header is only for removes
            //This first header line is all the manditory columns to creat a shell
            //sb.Append("Action|Company|Sub Unit|Traveler Type|Traveler Unique ID            |First Name                                     |Last Name                                    |Email - Work                           |");

            sb.Append("Action|Company|Sub Unit|Traveler Type|Traveler Unique ID            |First Name                                     |Last Name");

            //Reporting Fields
            //sb.Append("DK Number|Legal Entity|Legal Entity Name|TL Cost Center ID|TL Cost Center Name|TL Parent Business Id|TL Parent Business Name|TL Business Id|TL Business Name|");
            //sb.Append("Org Sub Code|Org Sub Name|Org Department Code|Org Department Name|Org Division Code|Org Division Name|Org Unit Code|Org Unit Name|Org Group Code|Org Group Name|WA Country Id|WA Business Id|");
            //sb.Append("DK Info 1|DK Info 2|DK Info 3|DK Info 4|DK Info 5|DK Info 6|DK Info 7|SSO|Business Segment Name|Industry Focus Name|CVW|Business Segment Code|Industry Focus Code|");

            // Example sent by Diane F.
            //Action |Company Name |Subunit Name |Traveler Type |External System ID |First Name |Last Name
            //Remove |General Electric |CL-GE Aviation |General |T500299676|Susie |Hulen




            sb.Append("\n");

            SaveToFile(fileName);

        }


        private static void BuildHeaderImplied()
        {

            //This first header line is all the manditory columns to creat a shell
            sb.Append("Company|Sub Unit|Traveler Type|Traveler Unique ID            |First Name                                     |Last Name                                    |Email - Work                           |");


            //Reporting Fields
            sb.Append("DK Number|Legal Entity|Legal Entity Name|TL Cost Center ID|TL Cost Center Name|TL Parent Business Id|TL Parent Business Name|TL Business Id|TL Business Name|");
            sb.Append("Org Sub Code|Org Sub Name|Org Department Code|Org Department Name|Org Division Code|Org Division Name|Org Unit Code|Org Unit Name|Org Group Code|Org Group Name|WA Country Id|WA Business Id|");
            sb.Append("DK Info 1|DK Info 2|DK Info 3|DK Info 4|DK Info 5|DK Info 6|DK Info 7|SSO|Business Segment Name|Industry Focus Name|CVW|Business Segment Code|Industry Focus Code|");
            //sb.Append("GGST Contact email|GST Contact Phone|GST Entity address|GST Entity name|GSTIN|");

            sb.Append("\n");

            SaveToFile(fileName);

        }


        private static string CleanReportingFields(string strInputString, string replacement)
        {
            return CleanString(strInputString, " ", false);
        }


        private static string CleanReportingFields(string strInputString)
        {

            return CleanString(strInputString, " ");
        }


        public static void WriteErrorsToLog(string strParm1, string strParm2)
        {
            string dt = System.DateTime.Today.ToShortDateString();
            string dtt = System.DateTime.Today.ToShortTimeString();
            string strMessage = dt + " " + dtt + "  Error: " + strParm1 + " at: " + strParm2 + "\n";
            string logFile = ConfigurationManager.AppSettings["ErrorFile"];
            StreamWriter swMissing;

            try
            {
                swMissing = new StreamWriter(logFile);
                swMissing.Write(strMessage);
                swMissing.WriteLine();
                swMissing.Flush();
                swMissing.Close();
            }
            catch (Exception ex)
            {
                WriteErrorsToLog(ex.Message, "OnMessageReceived:WriteErrorsToLog");
            }
            finally
            {

            }

        }



        private static int getConnectionString()
        {

            PSDEncrypt psde = new PSDEncrypt();
           
            try
            {
                string db;
                string passwrd;
                db = ConfigurationManager.AppSettings["Database"];
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[db].ConnectionString);
                // this is the key being used to dyencrypt the password is the db password ever changes a new password encryption string will 
                // need to be built using this same key.
                String key = "c3RLeTFSeEM0TlR4N2theVlra3NqQT09LHFaRkJSY3EvY1N3ZUZCRW5aSTl6dHk2eWVHUFhuQXpoQ3o4dWlLOHZVdEU9";
                passwrd = con.Password;
                con.Password = psde.Decrypt(passwrd, key, 256);
                _connectionString = con.ConnectionString;
                


                
                //db = ConfigurationManager.AppSettings["Database"];
                db = ConfigurationManager.AppSettings["DatabaseAsync"];
                con = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[db].ConnectionString);
                // will change when encryption is is place for the camaro
                //_connectionAsyncString = con.ConnectionString;

                passwrd = con.Password;
                con.Password = psde.Decrypt(passwrd, key, 256);
                _connectionAsyncString = con.ConnectionString;

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return 1;
        }



    }
}
