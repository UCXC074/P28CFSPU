using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;



namespace P28CFSPU
{
    class ExportDataCall
    {

        // Database Layer DLL for interaction with the DB
        DataLayerBase dlb = new DataLayerBase();

        // Connection string declaration
        private string _connectionString = string.Empty;

        public string ConnectionString
        {
            set { _connectionString = value; }
            get { return _connectionString; }
        }

        public DataTable getTravelerData(int personidn)
        {


            SqlParameter[] parameter = new SqlParameter[1];

            DataTable ProjectNumVal;


            parameter[0] = new SqlParameter("@p_Person_IDN", SqlDbType.Int);
            parameter[0].Value = personidn;

            dlb.ConnectionString = _connectionString;

            try
            {
                dlb.OpenConnection();
                ProjectNumVal = dlb.ExecuteDataTable("up_SOPO_Load_s", parameter);
            }
            finally
            {
                dlb.CloseConnection();
            }

            return ProjectNumVal;


        }



        public System.Xml.XmlReader getProfileData(int personidn)
        {

   
            SqlParameter[] parameter = new SqlParameter[1];

            System.Xml.XmlReader Profile;
            parameter[0] = new SqlParameter("@p_Person_IDN", SqlDbType.Int);
            parameter[0].Value = personidn;

            dlb.ConnectionString = _connectionString;

            try
            {
                //dlb.OpenConnection();
                Profile = dlb.ExecuteXML("up_SOPO_Load_s", parameter);

            }
            finally
            {
                //dlb = null;
            }

            return Profile;
        }



        public DataTable getSabreIndexOrgs()
        {

            SqlParameter[] parameter = new SqlParameter[1];
            DataTable OrgIdex;



            dlb.ConnectionString = _connectionString;

            try
            {
                dlb.OpenConnection();
                OrgIdex = dlb.ExecuteDataTable("up_get_org_index_s", parameter);
            }
            finally
            {
                dlb.CloseConnection();
            }

            return OrgIdex;
        }


        public DataTable getTravelerIndexInfo(String strPCC, String strCRSID)
        {

            //   @p_pcc      char(4),
            //   @p_crsid    char(12)

            //dlb = new CWTDataLayerBase05.DataLayerBase();

            SqlParameter[] parameter = new SqlParameter[2];

            DataTable Trav;
            parameter[0] = new SqlParameter("@p_pcc", SqlDbType.VarChar);
            parameter[0].Value = strPCC;

            parameter[0] = new SqlParameter("@p_crsid", SqlDbType.VarChar);
            parameter[0].Value = strCRSID;


            dlb.ConnectionString = _connectionString;

            try
            {
                //dlb.OpenConnection();
                Trav = dlb.ExecuteDataTable("up_get_travlndx_s", parameter);
            }
            finally
            {
                //dlb = null;
            }

            return Trav;
        }




        public DataTable getLevelOneGECreates(string subOrg)
        {

            String mySQL;

           
            //--need traveler type
            //mySQL = "select 'Create' as Action, 'General Electric' as Company, 'US-General Electric' as Subunit, p.person_idn, ";
            //mySQL += "p.national_person_id as External_System_ID, p.first_nm as First_Name, p.last_nm as Last_Name ";
            //mySQL += "p.email_address_txt as Work_Email_address, a.address_line2_txt as Address_work, ";
            //mySQL += "a.city_nm as Address_Work_City,cc.country_cd as Assress_Work_Country, a.postal_code_txt as Address_Work_Postal_Code,";
            //mySQL += "spc.state_province_cd As Address_Work_state,ov.org_dvsn_cd,od.org_dept_cd  ";

            //mySQL += "from person as p ";
            //mySQL += "JOIN person_org_unit_xref AS pox ON p.person_idn = pox.person_idn ";
            //mySQL += "JOIN org_unit AS ou ON pox.org_unit_idn = ou.org_unit_idn ";
            //mySQL += "JOIN org_group AS og ON ou.org_grp_idn = og.org_grp_idn ";
            //mySQL += "JOIN org_department AS od ON og.org_dept_idn = od.org_dept_idn ";
            //mySQL += "JOIN org_division AS ov ON od.org_dvsn_idn = ov.org_dvsn_idn ";
            //mySQL += "JOIN org_sub AS os ON ov.org_sub_idn = os.org_sub_idn ";
            //mySQL += "LEFT OUTER JOIN dbo.person_address_xref pax on p.person_idn = pax.person_idn and pax.address_type_idn = 4 ";
            //mySQL += "LEFT OUTER JOIN dbo.address a on pax.address_idn = a.address_idn  ";
            //mySQL += "LEFT OUTER JOIN dbo.state_province_code spc on a.state_province_idn = spc.state_province_idn ";
            //mySQL += "LEFT OUTER JOIN dbo.country_code cc on a.country_idn = cc.country_idn ";
            //mySQL += "where ";
            //mySQL += "p.prs_cntr_status_idn in (1,3) ";
            //mySQL += "AND   pox.primary_org_unit_fg = 'Y' ";
            //mySQL += "AND national_person_id in ('500300386','500299676','500896680','500299405', '500833069','500299987','501711575','501493748','501591224') ";
            //mySQL += "AND   os.org_sub_idn = " + subOrg ;
            //mySQL += "Order by national_person_id asc ";

            //mySQL = "select 'Create' as Action, ps.person_idn from person p join dbo.portrait_profile_sync ps on p.person_idn = ps.person_idn where ps.modified_dt  > '6-27-2013' and ps.process_flag = 'N' ";


            mySQL = "select 'Create' as Action, person_idn from person where national_person_id in ";
            mySQL += "('212348062',	'212348073',	'212348224',	'212348233',	'212348147',	'212348072',	'212348240',	'212348004',	'212356150',	'212348226',	'212348011',	'212348060',	'212348028',	'212348048',	'212348014',	'212348100',	'212348019',	'212348238',	'212348031',	'212348006',	'212348013',	'212348051',	'212348236',	'212348053',	'212348016',	'212348079',	'212348165',	'212348015',	'212347979',	'212347990',	'212347967',	'212347957',	'212347923',	'212347925',	'212347930',	'212348252',	'212347963',	'212347933',	'212347919',	'212348250',	'212347952',	'212347966',	'212347929',	'212347940',	'212347920',	'212347931',	'212347964',	'212347962',	'212348247',	'212347965',	'212347947',	'212348010',	'212347978',	'212348217',	'212348020',	'212348070',	'212348073',	'212348039',	'212348198',	'212347998',	'212348022',	'212348029',	'212348021',	'212348243',	'212348192',	'212348111',	'212348062',	'212348032',	'212348061',	'212348055',	'212348223',	'212348080',	'212347944',	'212347942',	'212347959',	'212348248',	'212347928',	'212348245',	'212347973',	'212347937',	'212347971',	'125004669',	'212348251',	'212347961',	'212347927',	'212347955',	'212348246',	'212347921',	'212347946',	'212347948',	'212347939',	'212347950',	'212347968',	'212347969',	'212347958',	'212347975',	'212347926',	'212347956',	'212347938',	'212347949',	'212347945',	'212347936',	'212347918',	'212347934',	'212347953',	'212347922',	'212347972',	'212347954',	'212347924',	'212302801',	'123060192',	'203010324',	'212348417',	'103003604',	'212313501',	'123060071',	'105001782',	'212314703',	'100042387',	'204059273',	'105067147',	'212348057',	'212347989',	'212347995',	'212347991',	'212348242',	'212348078',	'212348195',	'212348076',	'212348050',	'212348068',	'212348018',	'212348033',	'212348001',	'212348244',	'212348177',	'212347987',	'212348239',	'212348007',	'212348041',	'212348196',	'212348063',	'100045700',	'212321636',	'212322297',	'212071934',	'502220415',	'501988637',	'501480776',	'502235112',	'212347983',	'212348237',	'212348026',	'212347994',	'212348043',	'212347981',	'212348038',	'212348074',	'212348056',	'212348024',	'212347999',	'212348240',	'212348004',	'212348067',	'212348235',	'212348035',	'212348224',	'212348042',	'212348059',	'212348115',	'212348233',	'212348147',	'212348072',	'212348171',	'212348005',	'212348232',	'212348241',	'212348071',	'212348066',	'212347980',	'212348023',	'212348012',	'212347982',	'212348052',	'212348002',	'501981559',	'113009447',	'113004324',	'113011440',	'105052616',	'120012820',	'113010369',	'105049656',	'100008765',	'105006923',	'113011221',	'108014647',	'113008300',	'113009702',	'105045012',	'212307130',	'105062763',	'212357050',	'212357050',	'113010432',	'105041051',	'312001097',	'105048343',	'105060668',	'105045674',	'113010845',	'120010906',	'212314151',	'105064469',	'204011233',	'105000479',	'113009566',	'105046031',	'210022674',	'105035605',	'113010123',	'113011314',	'105050391',	'100045700',	'409006044',	'108014042',	'105033041',	'105050826',	'105050550',	'204069928',	'105034249',	'303024392',	'113003542',	'113009863',	'302003570',	'105067302',	'105062753',	'105012421',	'105051195',	'105050389',	'220043299',	'410001313',	'113010680',	'105039129',	'312001000',	'106003435',	'105052379',	'212342333',	'113007505',	'105040069',	'105031078',	'105711474',	'113007529',	'113010422',	'106003792',	'106001035',	'123060192',	'105033063',	'113006993',	'100008894',	'105050713',	'307007827',	'212321636',	'105001780',	'113007726',	'105056193',	'110010080',	'212310879',	'113009478',	'212357052',	'105031749',	'105006350',	'502222310',	'204049452',	'105012848',	'105035498',	'212326463',	'103003604',	'409009100',	'414003345',	'502223604',	'414004451',	'105012795',	'105052763',	'105007992',	'105056718',	'105062691',	'113010288',	'103022049',	'105027456',	'106000305',	'212302801',	'105042493',	'420000567',	'212357111',	'120012328',	'502235166',	'212308188',	'103013334',	'105027755',	'502180887',	'105061908',	'106002360',	'105048733',	'105059498',	'105033295',	'108002009',	'100000822',	'210042837',	'200006647',	'502180886',	'105061967',	'105065804',	'105062682',	'502235163',	'113009818',	'502235313',	'113003695',	'105053453',	'120007306',	'113010290',	'105063950',	'105056031',	'108003190',	'113001922',	'105049413',	'307008568',	'301001355',	'420000972',	'105051933',	'103018921',	'108014767',	'113005065',	'105049690',	'502235157',	'502235157',	'108003424',	'105052464',	'106003714',	'212322297',	'105053043',	'113004639',	'113004917',	'120009313',	'105040381',	'105046808',	'105030174',	'113009582',	'502227030',	'106001961',	'123060071',	'502180888',	'502226218',	'120009197',	'204059273',	'105065814',	'105063711',	'105058200',	'108010011',	'108004678',	'414003372',	'105030825',	'105053035',	'212357127',	'113007230',	'105055377',	'105025797',	'105052922',	'105049219',	'113010472',	'105030751',	'105049080',	'409007951',	'113007564',	'105050381',	'120011223',	'108015184',	'113009685',	'113004886',	'105014286',	'212342392',	'120009327')";
            //mySQL += "'212348239','212347973','212347961','212348238','212341643','106004760','212348049','212348074','212348039','212356769','212347939','212348252','212347991','212347971','212348192','212348246','212348012','212348171','105067480','105047564','212348245','212348059','212348010','212356885','212356981', ";
            //mySQL += "'212347922','212348042','123009063','212356739','212348402','212347968','212347955','212348217','212348060','212347987','212348061','212347952','212348236','212348055','212338368','212348020','212347967','212347947','212347965','212348029','212348066','212348223','212347995','212347930','212347918', ";
            //mySQL += "'212348070','212348014','212321636','212322297','212348235','212348111','212348048','212348232','212348024','123060071','212348071','123065697','212347936','212348241','212313435','212347946','212348005','212347963','212347966','212347948','212348237','212348078','212347923','212347964','212348052', ";
            //mySQL += "'212348413','212348031','212347989','212313451','212348250','212347981','212348019','212348018','212348196','212348115','212348007','212347983','212348165','212330013','212348016','212348032','212347990','212348022','212348041','212347951','212347926','501913386','212347980','212347921','212348067', ";
            //mySQL += "'212348226','212347927','212347928','212348011') ";

            DataTable LevelOne;

            dlb.ConnectionString = _connectionString;

            try
            {
                dlb.OpenConnection();
                LevelOne = dlb.ExecuteDataTableEmbedded(mySQL);
            }
            finally
            {
                //dlb = null;

            }

            return LevelOne;
        }



        public DataTable getLastRunDate(string _jobtype)
        {

            String mySQL;

            if (_jobtype == "REMOVE")
            {
                mySQL = "select convert(datetime, default_value_txt, 0)  from system_default where default_nm = 'PROFILEEXTRACT_PORTAIT_SYNC_DELETE'";
            }
            else
            {
                mySQL = "select convert(datetime, default_value_txt, 0)  from system_default where default_nm = 'PROFILEEXTRACT_PORTAIT_SYNC_CREATE_UPDATE'";
            }
            DataTable RunDate;

            dlb.ConnectionString = _connectionString;

            try
            {
                dlb.OpenConnection();
                RunDate = dlb.ExecuteDataTableEmbedded(mySQL);
            }
            finally
            {
                //dlb = null;

            }

            return RunDate;
        }

        public DataTable getPersonNoteData(int personidn)
        {

            //
            //@p_Person_IDN 	VARCHAR(4)  = NULL,


            //dlb = new CWTDataLayerBase05.DataLayerBase();

            SqlParameter[] parameter = new SqlParameter[1];

            DataTable Notes;
            parameter[0] = new SqlParameter("@p_Person_IDN", SqlDbType.Int);
            parameter[0].Value = personidn;

            dlb.ConnectionString = _connectionString;

            try
            {
                //dlb.OpenConnection();
                Notes = dlb.ExecuteDataTable("up_sabre_star_person_notes_s", parameter);
            }
            finally
            {
                //dlb = null;
            }

            return Notes;
        }



        public DataTable getProlileUpdates(DateTime _extractDate )
        {


            DataTable pu = null;

            SqlParameter[] parameter = new SqlParameter[1];

            string xx;
 
            parameter[0] = new SqlParameter("@p_profileextract_date", SqlDbType.DateTime);
            parameter[0].Value = _extractDate;

            dlb.ConnectionString = _connectionString;

            try
            {
                //dlb.OpenConnection();
                pu = dlb.ExecuteDataTable("up_ge_profile_extract_create_update", parameter);
            }
            catch (Exception ex)
            {
                //throw;
                xx = ex.Message;

            }
            finally
            {
                //dlb = null;
            }

            return pu;
        }


        public DataTable getProlileDeletes(DateTime _extractDate)
        {


            DataTable pu;

            SqlParameter[] parameter = new SqlParameter[1];


            parameter[0] = new SqlParameter("@p_profileextract_date", SqlDbType.DateTime);
            parameter[0].Value = _extractDate;

            dlb.ConnectionString = _connectionString;

            try
            {
                //dlb.OpenConnection();
                pu = dlb.ExecuteDataTable("up_ge_profile_extract_delete", parameter);
            }
            finally
            {
                //dlb = null;
            }

            return pu;
        }



        public int ProfileProcessUpdate(int personIDN)
        {

            int rtn;

            dlb.ConnectionString = _connectionString;
            
            SqlParameter[] parameter = new SqlParameter[1];


            parameter[0] = new SqlParameter("@p_person_idn", SqlDbType.Int);
            parameter[0].Value = personIDN;


            try
            {
                //dlb.OpenConnection();
                rtn = dlb.ExecuteNonQuery("up_sync_profile_u", parameter);
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                //dlb = null;
            }

            return rtn;
        }


        public int deletePersonfromBatch(int personidn)
        {

            //
            //@p_Person_IDN 	VARCHAR(4)  = NULL,


            SqlParameter[] parameter = new SqlParameter[1];

            int result;
            parameter[0] = new SqlParameter("@p_Person_IDN", SqlDbType.Int);
            parameter[0].Value = personidn;

            dlb.ConnectionString = _connectionString;

            try
            {
                //dlb.OpenConnection();
                result = dlb.ExecuteNonQuery("up_sabre_star_d", parameter);
            }
            finally
            {
                //dlb = null;
            }

            return result;
        }


        public int Sabre_Star_Error(int personidn, int errorcode, String msg)
        {

            //
            //@p_Person_IDN 	VARCHAR(4)  = NULL,


            SqlParameter[] parameter = new SqlParameter[3];

            int result;
            parameter[0] = new SqlParameter("@p_Person_IDN", SqlDbType.Int);
            parameter[0].Value = personidn;

            parameter[1] = new SqlParameter("@p_ErrorCode", SqlDbType.Int);
            parameter[1].Value = errorcode;

            parameter[2] = new SqlParameter("@p_ErrorMsg", SqlDbType.VarChar);
            parameter[2].Value = msg;

            dlb.ConnectionString = _connectionString;

            try
            {
                //dlb.OpenConnection();
                result = dlb.ExecuteNonQuery("up_sabre_star_error_u", parameter);
            }
            finally
            {
                //dlb = null;
            }

            return result;
        }







        public DataTable getSabreStarPersonIDN(int AppNum)
        {

            //up_sabre_star_person_idn_s
            //@p_AppNum 	Int


            dlb = new DataLayerBase();

            SqlParameter[] parameter = new SqlParameter[1];

            DataTable dtPersonIDN;

            parameter[0] = new SqlParameter("@p_AppNum", SqlDbType.Int);
            parameter[0].Value = AppNum;

            dlb.ConnectionString = _connectionString;

            try
            {
                dlb.OpenConnection();
                dtPersonIDN = dlb.ExecuteDataTable("up_sabre_star_person_idn_s", parameter);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                dlb.CloseConnection();
            }

            return dtPersonIDN;
        }




    }

}
