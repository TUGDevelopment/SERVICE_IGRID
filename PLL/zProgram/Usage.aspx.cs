using BLL.Services;
using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace PLL.zProgram
{
    public partial class Usage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // generateUsage();
            wfQuery(DateTime.Now, DateTime.Now);
            getCustomer();
            getTerminateReason();
        }


        public void generateUsage(DateTime dtFrom, DateTime dtTo)
        {
            using (var context = new ARTWORKEntities())
            {



                int iPkg = 0;
                int iMK = 0;
                int iRD = 0;
                int iPN = 0;
                int iWH = 0;
                int iTH = 0;
                int iCus = 0;
                int iVen = 0;
                int iQC = 0;
                int iCusDummy = 0;
                int iETC = 0;
                //      DateTime xxx = DateTime.Now;
                //  string xx = "/";

                //  List<ART_SYS_LOG> ARTx_SYS_LOG = context.ART_SYS_LOG.Where(t => t.ACTION.Equals("Open Dashboard") && t.CREATE_DATE > xxx).Select(t => t).ToList();


                List<ART_SYS_LOG> ART_SYS_LOG = context.ART_SYS_LOG.Where(t => t.ACTION.Equals("Open Dashboard") && t.CREATE_DATE >= dtFrom && t.CREATE_DATE <= dtTo).Select(t => t).ToList();
                List<string> totalEmp = context.ART_SYS_LOG.Where(t => t.ACTION.Equals("Open Dashboard") && t.CREATE_DATE >= dtFrom && t.CREATE_DATE <= dtTo).Select(t => t.NEW_VALUE).ToList();
                string totalDis = totalEmp.Distinct().ToList().Count().ToString();
                List<string> lsTotalDis = totalEmp.Distinct().ToList();
                string currentDepartment = string.Empty;
                List<string> lssEtc = new List<string>();
                int? iDepartment = 0;
                //  char x = @"/";
                foreach (var item in lsTotalDis)
                {
                    string currentUser = item;
                    if (currentUser.Contains("THAIUNION"))
                    {
                        currentUser = currentUser.Split('\\')[1];



                        //  iDepartment = context.ART_M_USER.Where(t => t.USERNAME.Equals(currentUser)).Select(t => t.POSITION_ID).FirstOrDefault();
                        // currentDepartment = context.ART_M_POSITION.Where(t => t.ART_M_POSITION_ID.Equals(iDepartment)).Select(t => t.ART_M_POSITION_CODE).FirstOrDefault();
                    }

                    iDepartment = context.ART_M_USER.Where(t => t.USERNAME.Equals(currentUser)).Select(t => t.POSITION_ID).FirstOrDefault();
                    currentDepartment = context.ART_M_POSITION.Where(t => t.ART_M_POSITION_ID == iDepartment).Select(t => t.ART_M_POSITION_CODE).FirstOrDefault();

                    if (currentDepartment == "PK")
                    {
                        iPkg++;
                    }
                    else if (currentDepartment == "MK")
                    {
                        iMK++;
                    }
                    else if (currentDepartment == "QC")
                    {
                        iQC++;
                    }
                    else if (currentDepartment == "RD")
                    {
                        iRD++;
                    }
                    else if (currentDepartment == "WH")
                    {
                        iWH++;
                    }
                    else if (currentDepartment == "PN")
                    {
                        iPN++;
                    }

                    else if (currentDepartment == "T-Holding")
                    {
                        iTH++;
                    }
                    else if (currentDepartment == "CUSTOMER")
                    {
                        if (currentUser.Contains("CD"))
                        {
                            iCusDummy++;
                        }
                        else
                        {
                            iCus++;
                        }

                    }
                    else if (currentDepartment == "VENDOR")
                    {
                        iVen++;
                    }
                    else
                    {
                        lssEtc.Add(currentDepartment);
                        iETC++;
                    }


                }

                //lsEtc.DataSource = lssEtc;
                //  lsEtc.DataBind();

                //bind
                lblTotal.Text = ART_SYS_LOG.Count().ToString();

                lblTotaltb.Text = totalDis;

                //Dearp

                lblPackaging.Text = iPkg.ToString();
                lblMarketing.Text = iMK.ToString();
                lblPN.Text = iPN.ToString();
                lblQC.Text = iQC.ToString();
                lblRD.Text = iRD.ToString();
                lblTH.Text = iTH.ToString();
                lblWH.Text = iWH.ToString();
                lblCus.Text = iCus.ToString();
                lblVen.Text = iVen.ToString();
                LblCusDummy.Text = iCusDummy.ToString();
                lblEtc.Text = iETC.ToString();

                //DataTable dt = new DataTable();
                //dt.Columns.Add("ID");
                //dt.Columns.Add("Username");
                ////dt.Columns.Add("FullName");
                //dt.Columns.Add("Browser");

                //  foreach (var item in ART_SYS_LOG)
                //  {
                //      DataRow dr = dt.NewRow();
                //      dr["ID"] = item.LOG_ID;
                //      string sUser = item.NEW_VALUE;
                //      if (sUser.Contains("Thainuion"))
                //      {
                //          sUser = sUser.Split('/')[1];
                //      }

                //      dr["Username"] = sUser;
                //      dr["Browser"] = item.OLD_VALUE;

                //      dt.Rows.Add(dr);
                //  }

                //      tbDeparment. = dt;
                //    tbDeparment.DataBind();

            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime dtFrom = DateTime.Parse(txtDateStart.Text);
            DateTime dtTo = DateTime.Parse(txtDateTo.Text);

            // TimeSpan tsFrom = new TimeSpan(0, 30, 0);
            TimeSpan tsTo = new TimeSpan(23, 59, 0);

            //  dtFrom.TimeOfDay()
            generateUsage(dtFrom, dtTo + tsTo);
            wfQuery(dtFrom, dtTo + tsTo);


        }


        public void wfQuery(DateTime dtFrom, DateTime dtTo)
        {
            using (var context = new ARTWORKEntities())
            {


                //MO

                int iMONn = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-N-%' and(IS_END is null or IS_END = '') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iMONc = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-N-%' and(IS_END = 'X') and (IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iMONt = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-N-%' and (IS_TERMINATE = 'X')").FirstOrDefault();
                int iMONa = context.Database.SqlQuery<int>("  select Count(mockup_no) MO_N_Toltal from ART_WF_MOCKUP_CHECK_LIST_ITEM  where mockup_no like 'MO-N-%'").FirstOrDefault();

                int iMODn = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-D-%' and(IS_END is null or IS_END = '') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iMODc = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-D-%' and(IS_END = 'X') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iMODt = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-D-%' and(IS_TERMINATE = 'X')").FirstOrDefault();
                int iMODa = context.Database.SqlQuery<int>("  select Count(mockup_no) MO_N_Toltal from ART_WF_MOCKUP_CHECK_LIST_ITEM  where mockup_no like 'MO-D-%'").FirstOrDefault();

                int iMOpn = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-P-%' and(IS_END is null or IS_END = '') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iMOpc = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-P-%' and(IS_END = 'X') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iMOpt = context.Database.SqlQuery<int>(" select Count(R.MOCKUP_ID)AW_N_Count_Inprogress from ART_WF_MOCKUP_CHECK_LIST_ITEM R left join ART_WF_MOCKUP_PROCESS P on R.MOCKUP_ID = p.MOCKUP_ID and  CURRENT_STEP_ID = 1 where R.Mockup_NO like 'MO-P-%' and(IS_TERMINATE = 'X')").FirstOrDefault();
                int iMOpa = context.Database.SqlQuery<int>("  select Count(mockup_no) MO_N_Toltal from ART_WF_MOCKUP_CHECK_LIST_ITEM  where mockup_no like 'MO-P-%'").FirstOrDefault();


                //AW










                int iAWNn = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_N_Count_Inprogress from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-N-%' and(IS_END is null or IS_END = '') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iAWNc = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_N_Count_End from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-N-%' and  IS_END = 'X' and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iAWNt = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_N_Count_Inprogress from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-N-%' and(IS_TERMINATE = 'X')").FirstOrDefault();
                int iAWNa = context.Database.SqlQuery<int>(" select Count(REQUEST_ITEM_NO) AW_N_Toltal from ART_WF_ARTWORK_REQUEST_ITEM where REQUEST_ITEM_NO like 'AW-N-%'").FirstOrDefault();

                int iAWRn = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_N_Count_Inprogress from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-R-%' and(IS_END is null or IS_END = '') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iAWRc = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_R_Count_End from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-R-%' and(IS_END = 'X') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iAWRt = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_N_Count_Inprogress from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-R-%' and(IS_TERMINATE = 'X')").FirstOrDefault();
                int iAWRa = context.Database.SqlQuery<int>(" select Count(REQUEST_ITEM_NO) AW_N_Toltal from ART_WF_ARTWORK_REQUEST_ITEM where REQUEST_ITEM_NO like 'AW-R-%'").FirstOrDefault();

                int iAWR6n = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_N_Count_Inprogress from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-R6-%' and(IS_END is null or IS_END = '') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iAWR6c = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_R_Count_End from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-R6-%' and(IS_END = 'X') and(IS_TERMINATE is null or IS_TERMINATE = '')").FirstOrDefault();
                int iAWR6t = context.Database.SqlQuery<int>("select Count(R.REQUEST_ITEM_NO)AW_N_Count_Inprogress from ART_WF_ARTWORK_REQUEST_ITEM R left join ART_WF_ARTWORK_PROCESS P on R.ARTWORK_ITEM_ID = p.ARTWORK_ITEM_ID and  CURRENT_STEP_ID = 2 where R.REQUEST_ITEM_NO like 'AW-R6-%' and(IS_TERMINATE = 'X')").FirstOrDefault();
                int iAWR6a = context.Database.SqlQuery<int>(" select Count(REQUEST_ITEM_NO) AW_N_Toltal from ART_WF_ARTWORK_REQUEST_ITEM where REQUEST_ITEM_NO like 'AW-R6-%'").FirstOrDefault();






                lblMNp.Text = iMONn.ToString();
                lblMNt.Text = iMONt.ToString();
                lblMNc.Text = iMONc.ToString();
                lblMNa.Text = iMONa.ToString();


                lblMDp.Text = iMODn.ToString();
                lblMDt.Text = iMODt.ToString();
                lblMDc.Text = iMODc.ToString();
                lblMDa.Text = iMODa.ToString();


                lblMPp.Text = iMOpn.ToString();
                lblMPt.Text = iMOpt.ToString();
                lblMPc.Text = iMOpc.ToString();
                lblMPa.Text = iMOpa.ToString();

                lblAWNp.Text = iAWNn.ToString();
                lblAWNt.Text = iAWNt.ToString();
                lblAWNc.Text = iAWNc.ToString();
                lblAWNa.Text = iAWNa.ToString();

                lblAWRp.Text = iAWRn.ToString();
                lblAWRt.Text = iAWRt.ToString();
                lblAWRc.Text = iAWRc.ToString();
                lblAWRa.Text = iAWRa.ToString();


                lblAWR6p.Text = iAWR6n.ToString();
                lblAWR6t.Text = iAWR6t.ToString();
                lblAWR6c.Text = iAWR6c.ToString();
                lblAWR6a.Text = iAWR6a.ToString();


                int n = iMONn + iMODn + iMOpn + iAWNn + iAWRn + iAWR6n;
                int c = iMONc + iMODc + iMOpc + iAWNc + iAWRc + iAWR6c;
                int t = iMONt + iMODt + iMOpt + iAWNt + iAWRt + iAWR6t;


                lblGrandTotal.Text = (n + c + t).ToString();
                lblInTotal.Text = (iMONn + iMODn + iMOpn + iAWNn + iAWRn + iAWR6n).ToString();
                lblComTotal.Text = (iMONc + iMODc + iMOpc + iAWNc + iAWRc + iAWR6c).ToString();
                lblTerTotal.Text = (iMONt + iMODt + iMOpt + iAWNt + iAWRt + iAWR6t).ToString();



                //string iAWNn = "select Count(REQUEST_ITEM_NO) AW_N_Toltal from ART_WF_ARTWORK_REQUEST_ITEM where REQUEST_ITEM_NO like 'AW-N-%'";


                //    int count = context.Database.SqlQuery<int>(sqlCommand).FirstOrDefault();





            }
        }

        public void getCustomer()
        {

            lblCuslogin.Text = "";

            lblVenlogin.Text = "";

            lblCustomerList.Text = "";
            using (var context = new ARTWORKEntities())
            {

                List<int> lsCusUpload = context.Database.SqlQuery<int>("SELECT UPDATE_BY FROM dbo.ART_WF_ARTWORK_ATTACHMENT where STEP_ARTWORK_ID = 1").ToList();

                List<int> lsCusfinal = lsCusUpload.Distinct().ToList();

                List<string> lsCusLogin = context.ART_SYS_LOG.Where(t => t.ACTION == "Open Dashboard").Select(t => t.NEW_VALUE).ToList();

                List<string> cusId = new List<string>();

                List<string> venId = new List<string>();

                //int totalEmp = 0;

                List<string> lsTotalCus = context.Database.SqlQuery<string>("SELECT CUSTOMER_CODE FROM[XECM_M_CUSTOMER]").ToList();
                List<string> lsTotalVen = context.Database.SqlQuery<string>("SELECT VENDOR_CODE FROM[XECM_M_VENDOR]").ToList();

                int totalCus = lsTotalCus.Count();
                int totalVen = lsTotalVen.Count();

                //   List<string> cusCount = new List<string>();
                int totalCusLogon = 0;
                int totalVenLogon = 0;

                foreach (var item in lsCusLogin)
                {
                    if (item != null && item.Contains("C") && !item.Contains("CD") && !item.Contains("CUS"))
                    {
                        string isCus = context.ART_M_USER.Where(t => t.USERNAME == item && t.POSITION_ID == 12).Select(t => t.USERNAME).FirstOrDefault();



                        string currentUser = isCus;


                        if (currentUser != null)
                        {
                            if (currentUser.Contains("THAIUNION"))
                            {
                                currentUser = currentUser.Split('\\')[1];

                            }

                        }

                        if (currentUser != null)
                        {
                            string sCus = "";
                            if (currentUser.Contains('_'))
                            {
                                sCus = item.Split('_')[0];
                            }
                            if (sCus != "")
                            {
                                cusId.Add(sCus);
                            }

                        }


                    }

                    else if (item.Contains("V"))
                    {

                        string isVen = context.ART_M_USER.Where(t => t.USERNAME == item && t.POSITION_ID == 11).Select(t => t.USERNAME).FirstOrDefault();
                        if (isVen != null)
                        {
                            string sVendor = "";
                            if (item.Contains('_'))
                            {
                                sVendor = item.Split('_')[0];
                            }
                            if (sVendor != "")
                            {
                                venId.Add(sVendor);
                            }

                        }
                    }


                }

                List<int> lsCusinArt = context.ART_M_USER_CUSTOMER.Where(t => t.CUSTOMER_ID != 0).Select(t => t.CUSTOMER_ID).ToList();
                List<int> lsVeninArt = context.ART_M_USER_VENDOR.Where(t => t.VENDOR_ID != 0).Select(t => t.VENDOR_ID).ToList();

                int totalCusinArt = lsCusinArt.Distinct().Count();
                int totalVeninArt = lsVeninArt.Distinct().Count();



                totalCusLogon = cusId.Distinct().Count();
                totalVenLogon = venId.Distinct().Count();

                lblCuslogin.Text += "<TR><TD>Customer</TD><TD>" + totalCus.ToString() + "</TD><TD>" + totalCusinArt.ToString() + "</TD><TD>" + totalCusLogon.ToString() + "</TD></TR>";

                lblVenlogin.Text += "<TR><TD>Vendor</TD><TD>" + totalVen.ToString() + "</TD><TD>" + totalVeninArt.ToString() + "</TD><TD>" + totalVenLogon.ToString() + "</TD></TR>";


                List<string> fCus = new List<string>();


                int iNo = 0;
                foreach (var item in lsCusfinal)
                {

                    if (item != 0)
                    {


                        string lscusUser = context.ART_M_USER.Where(t => t.USER_ID.Equals(item) && (t.POSITION_ID == 12)).Select(t => t.USERNAME).FirstOrDefault();
                        try
                        {

                            if (lscusUser != null && lscusUser.Contains("C") && !lscusUser.Contains("CD") && !lscusUser.Contains("CUS"))
                            {
                                string cus1 = lscusUser.Split('C')[1];
                                string cusF = cus1.Split('_')[0];

                                // int cusNo = Convert.ToInt32(cusF);
                                string lsCusotmerDesc = context.XECM_M_CUSTOMER.Where(t => t.CUSTOMER_CODE == cusF).Select(t => t.CUSTOMER_NAME).FirstOrDefault();
                                if (lsCusotmerDesc != null)
                                {
                                    fCus.Add(lsCusotmerDesc);

                                }

                            }


                        }
                        catch 
                        {

                        }




                    }
                }

                foreach (var item in fCus.Distinct())
                {
                    iNo++;

                    string lsCusotmerCode = context.XECM_M_CUSTOMER.Where(t => t.CUSTOMER_NAME == item).Select(t => t.CUSTOMER_CODE).FirstOrDefault();
                    lblCustomerList.Text += "<TR><TD>" + iNo + "</TD><TD>" + lsCusotmerCode + "</TD><TD>" + item + "</TD></TR>";
                    lblCusTotal.Text = iNo.ToString();

                }

            }
        }

        public void getTerminateReason()
        {

            //bind terminate reason
            // var sTerminateReason = new List<var>();
            lblMoterminateReason.Text = "";
            lblTerminateReason.Text = "";
            int iNo = 0;
            int imNo = 0;

            using (var context = new ARTWORKEntities())
            {
                var AWvTerminateReason = context.ART_M_DECISION_REASON.Where(t => t.STEP_CODE == "ARTWORK_TERMINATE_REASON").Select(t => new { t.DESCRIPTION, t.ART_M_DECISION_REASON_ID }).ToList();
                var MOvTerminateReason = context.ART_M_DECISION_REASON.Where(t => t.STEP_CODE == "MOCK_UP_TERMINATE_REASON").Select(t => new { t.DESCRIPTION, t.ART_M_DECISION_REASON_ID }).ToList();

                foreach (var item in AWvTerminateReason)
                {
                    int total = 0;
                    total = context.Database.SqlQuery<int>("SELECT count (TERMINATE_REASON_CODE) UPDATE_BY FROM dbo.ART_WF_ARTWORK_PROCESS where TERMINATE_REASON_CODE =" + item.ART_M_DECISION_REASON_ID).FirstOrDefault();
                    iNo++;
                    lblTerminateReason.Text += "<TR><TD>" + iNo + "</TD><TD>" + item.DESCRIPTION + "</TD><TD>" + total + "</TD></TR>";


                }


                foreach (var item in MOvTerminateReason)
                {
                    int total = 0;
                    total = context.Database.SqlQuery<int>("SELECT count (TERMINATE_REASON_CODE) UPDATE_BY FROM dbo.ART_WF_MOCKUP_PROCESS where TERMINATE_REASON_CODE =" + item.ART_M_DECISION_REASON_ID).FirstOrDefault();

                    imNo++;
                    lblMoterminateReason.Text += "<TR><TD>" + imNo + "</TD><TD>" + item.DESCRIPTION + "</TD><TD>" + total + "</TD></TR>";


                }



                //  List<int?> iTerminateReasonID = iTerminateReasonID = context.ART_WF_ARTWORK_PROCESS.Where(t => t.CURRENT_STEP_ID == 2 && t.IS_TERMINATE == "X" && t.TERMINATE_REASON_CODE != null).Select(t => t.TERMINATE_REASON_CODE).ToList();


            }



        }

        protected void btnDecrypt_Click(object sender, EventArgs e)
        {
            string msg_error = "";
            //string status = "1";
            string sUsername = txtUsername.Text;
            using (var context = new ARTWORKEntities())
            {

                if (txtUsername.Text != null || txtUsername.Text != "")
                {
                    string getUser = context.ART_M_USER.Where(t => t.USERNAME == sUsername).Select(t => t.PASSWORD).FirstOrDefault();

                    if (getUser != null && getUser != "")
                    {
                        if (CNService.IsEncryptJson())
                        {
                            lblPassword.Text = EncryptionService.Decrypt(getUser);
                        }


                        int iCurrent = CNService.getCurrentUser(context);
                        string Body = "User" + iCurrent.ToString() + "  has decrypt password of" + sUsername + " on" + DateTime.Now.ToString();
                        string From = "adminartworksystem@thaiunion.com";


                        MailMessage msg = new MailMessage(From, "chanvit@iamconsulting.co.th", "TU xECM Password Decrypt", Body);
                        msg.SubjectEncoding = Encoding.UTF8;
                        msg.BodyEncoding = Encoding.UTF8;
                        msg.IsBodyHtml = true;
                        int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                        SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                        sc.UseDefaultCredentials = false;

                        bool IsUseSSL = false;
                        var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                        if (SMTPSSL.ToUpper().Trim() == "TRUE")
                        {
                            IsUseSSL = true;
                        }

                        try
                        {
                            sc.EnableSsl = IsUseSSL;
                            sc.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            msg_error = CNService.GetErrorMessage(ex);
                            //status = "0";
                        }
                    }
                    else
                    {
                        lblPassword.Text = "Invalid username";
                    }
                }
            }

        }

        protected void btnSOcomplete_Click(object sender, EventArgs e)
        {

            string sTargetSO = txtSO.Text;
               string msg_error = "";
            //string status = "1";
            using (var context = new ARTWORKEntities())
            {

                try
                {

                    var target = context.SAP_M_PO_COMPLETE_SO_HEADER.Where(t => t.SALES_ORDER_NO == sTargetSO).Select(t => t.PO_COMPLETE_SO_HEADER_ID).FirstOrDefault();
                    if (target == 0)
                    {
                        lblSOresult.Text = "Failed"; 
                    }
                    else
                    {
                        context.Database.ExecuteSqlCommand("update SAP_M_PO_COMPLETE_SO_HEADER set IS_MIGRATION = 'X' where SALES_ORDER_NO = " + sTargetSO);
                        lblSOresult.Text = "Completed";
                    }

                }
                catch 
                {
                    lblSOresult.Text = "Failed";
                }

                int iCurrent = CNService.getCurrentUser(context);
                string Body = "User" + iCurrent.ToString() + "  has mark SO " + sTargetSO + " to completed on" + DateTime.Now.ToString();
                string From = "adminartworksystem@thaiunion.com";


                MailMessage msg = new MailMessage(From, "chanvit@iamconsulting.co.th", "TU xECM SO complete " + sTargetSO + "  " + lblSOresult.Text, Body);
                msg.SubjectEncoding = Encoding.UTF8;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                sc.UseDefaultCredentials = false;

                bool IsUseSSL = false;
                var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                if (SMTPSSL.ToUpper().Trim() == "TRUE")
                {
                    IsUseSSL = true;
                }

                try
                {
                    sc.EnableSsl = IsUseSSL;
                    sc.Send(msg);
                }
                catch (Exception ex)
                {
                    msg_error = CNService.GetErrorMessage(ex);
                    //status = "0";
                }
            }
        }
    }
}