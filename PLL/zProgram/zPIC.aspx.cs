using BLL.Services;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PLL
{
    public partial class zPIC : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string USERNAME = Request.QueryString["USERNAME"];
            string ZONE = Request.QueryString["ZONE"];
            string SOLDTO = Request.QueryString["SOLDTO"];
            string SHIPTO = Request.QueryString["SHIPTO"];
            string COUNTRY = Request.QueryString["COUNTRY"];

            if (!string.IsNullOrEmpty(USERNAME)
                && !string.IsNullOrEmpty(ZONE)
                && !string.IsNullOrEmpty(SOLDTO)
                && !string.IsNullOrEmpty(SHIPTO))
            {
                string dupResult = ""; dupResult = checkDuplicate(USERNAME, ZONE, SOLDTO, SHIPTO, COUNTRY);
                string insertResult = "";
                if (dupResult != "Dup")
                {
                    insertResult = insertPIC(USERNAME, ZONE, SOLDTO, SHIPTO, COUNTRY);
                }

            }

            else
            {
                lblStatus.Text = "Missing Parameter";
            }

            //  getPIC();

//            gtest();
        }

        public string checkDuplicate(string username, string zone, string soldto, string shipto, string country)
        {
            string isDup = "NO";
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    List<ART_M_PIC> ART_M_USER = context.ART_M_PIC.Where(t => t.COUNTRY == country && t.SHIP_TO_CODE == shipto && t.SOLD_TO_CODE == soldto && t.ZONE == zone).Select(t => t).ToList();
                    if (ART_M_USER.Count > 0)
                    {
                        foreach (var item in ART_M_USER)
                        {
                            lblStatus.Text = "Duplicate with " + item.PIC_ID;
                            isDup = "Dup";
                        }
                        // break
                    }
                }
            }
            return isDup;
        }

        public string insertPIC(string username, string zone, string soldto, string shipto, string country)
        {
            string insertResult = "";


            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {

                    try
                    {

                        {

                            //   string user_id = context.ART_M_USER.Where(t.user)
                            username = username.TrimEnd(' ');
                            ART_M_USER user_id = context.ART_M_USER.Where(t => t.USERNAME == username).Select(t => t).FirstOrDefault();

                            if (user_id != null)
                            {
                                int currentUSERID = 0; currentUSERID = user_id.USER_ID;
                                if (country == null) { country = ""; }

                                if (currentUSERID != 0)
                                {
                                    ART_M_PIC newPIC = new ART_M_PIC();

                                    newPIC.USER_ID = currentUSERID;
                                    newPIC.ZONE = zone.Trim();
                                    newPIC.SOLD_TO_CODE = soldto.Trim();
                                    newPIC.SHIP_TO_CODE = shipto.Trim();
                                    newPIC.COUNTRY = country.Trim();
                                    newPIC.IS_ACTIVE = "X";
                                    newPIC.CREATE_BY = -1;
                                    newPIC.UPDATE_BY = -1;
                                    newPIC.UPDATE_DATE = DateTime.Now;
                                    newPIC.CREATE_DATE = DateTime.Now;

                                    ART_M_PIC_SERVICE.SaveOrUpdate(newPIC, context);
                                    dbContextTransaction.Commit();
                                    lblStatus.Text = "Success";
                                    txtUsername.Text = username; txtUsername.Enabled = false;
                                    txtZone.Text = zone; txtZone.Enabled = false;
                                    txtShipTo.Text = shipto; txtShipTo.Enabled = false;
                                    txtSoldTo.Text = soldto; txtSoldTo.Enabled = false;
                                    txtCountry.Text = country; txtCountry.Enabled = false;

                                    insertResult = "Completed";                  

                                }
                            }




                        }

                    }

                    catch (Exception ex)
                    {
                        lblStatus.Text = "Failed";
                        dbContextTransaction.Rollback();
                        insertResult = "Failed : " + ex.Message;
                        //throw ;
                    }
                }
            }

            return insertResult;
        }

        protected void btnCommit_Click(object sender, EventArgs e)
        {
            string USERNAME = Request.QueryString["USERNAME"];
            string ZONE = Request.QueryString["ZONE"];
            string SOLDTO = Request.QueryString["SOLDTO"];
            string SHIPTO = Request.QueryString["SHIPTO"];
            string COUNTRY = Request.QueryString["COUNTRY"];

            insertPIC(USERNAME, ZONE, SOLDTO, SHIPTO, COUNTRY);
        }

        public void getPIC()
        {
            //using (var context = new ARTWORKEntities())
            //{


            //    DataTable dt = new DataTable();
            //    dt.Columns.Add("ID");
            //    dt.Columns.Add("Username");
            //    dt.Columns.Add("FullName");
            //    dt.Columns.Add("Zone");
            //    dt.Columns.Add("SoldTo");
            //    dt.Columns.Add("ShipTo");
            //    dt.Columns.Add("Country");
            //    dt.Columns.Add("Active");

            //    List<ART_M_PIC> ART_M_PIC = context.ART_M_PIC.Where(t => t.PIC_ID != 0).ToList();
            //    if (ART_M_PIC.Count > 0)
            //    {
            //        foreach (var item in ART_M_PIC)
            //        {
            //            DataRow dr = dt.NewRow();

            //            ART_M_USER getUser = new ART_M_USER();
            //            getUser = context.ART_M_USER.Where(t => t.USER_ID == item.USER_ID).Select(t => t).FirstOrDefault();

            //            dr["ID"] = item.PIC_ID;
            //            dr["Username"] = getUser.USERNAME;
            //            dr["FullName"] = getUser.FIRST_NAME + ' ' + getUser.LAST_NAME;
            //            dr["Zone"] = item.ZONE;
            //            dr["SoldTo"] = item.SOLD_TO_CODE;
            //            dr["ShipTo"] = item.SHIP_TO_CODE;
            //            dr["Country"] = item.COUNTRY;
            //            dr["Active"] = item.IS_ACTIVE;

            //            dt.Rows.Add(dr);
            //        }

            //        gvPIC.DataSource = dt;
            //        gvPIC.DataBind();

            //    }
            //}
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            //        insertPIC(txtUsername.Text, txtZone.Text, txtSoldTo.Text, txtShipTo.Text, txtCountry.Text);

        }

        protected void linkEdit_Click(object sender, EventArgs e)
        {
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            LinkButton lblPic = grdrow.Cells[0].FindControl("linkEdit") as LinkButton;

            using (var context = new ARTWORKEntities())
            {
                string picNO = lblPic.Text;
                lblStatus.Text = "Edit" + picNO;

                //  int picNumber = context.ART_M_USER.Where(t => t.USERNAME == picNO).Select(t => t.USER_ID).FirstOrDefault();
                int picNumber = Convert.ToInt32(picNO);

                //ART_M_PIC editPIC = new ART_M_PIC();
                int? UserName = context.ART_M_PIC.Where(t => t.PIC_ID.Equals(picNumber)).Select(t => t.USER_ID).FirstOrDefault();
                string Zone = context.ART_M_PIC.Where(t => t.PIC_ID.Equals(picNumber)).Select(t => t.ZONE).FirstOrDefault();
                string Ship = context.ART_M_PIC.Where(t => t.PIC_ID.Equals(picNumber)).Select(t => t.SHIP_TO_CODE).FirstOrDefault();
                string Sold = context.ART_M_PIC.Where(t => t.PIC_ID.Equals(picNumber)).Select(t => t.SOLD_TO_CODE).FirstOrDefault();
                string County = context.ART_M_PIC.Where(t => t.PIC_ID.Equals(picNumber)).Select(t => t.COUNTRY).FirstOrDefault();

                lblStatus.Text = "Edit Mode";
                txtUsername.Text = UserName.ToString();
                txtZone.Text = Zone;
                txtShipTo.Text = Ship;
                txtSoldTo.Text = Sold;
                txtCountry.Text = County;
                lblID.Text = picNumber.ToString();

            }
        }

        public string upDatePIC(string user, string zone, string soldto, string shipto, string country, string ID)
        {
            string updateResult = "";
            int iID = Convert.ToInt32(ID);

            return updateResult;
        }

        public void gtest()
        {
            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {



                    try
                    {
                        List<int> xxx = new List<int>();
                        List<int> final = new List<int>();
                        string leadder = string.Empty;

                        xxx = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(982)).Select(t => t.USER_ID).ToList();

                        foreach (var item in xxx)
                        {
                            List<int> lItem1 = new List<int>();
                            List<int> lItem2 = new List<int>();
                            List<int> lItem3 = new List<int>();
                            List<int> lItem4 = new List<int>();
                            List<int> lItem5 = new List<int>();

                            final.Add(item);
                            lItem1 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item)).Select(t => t.USER_ID).ToList();
                            if (lItem1.Count > 0)
                            {
                                foreach (var item1 in lItem1)
                                {
                                    final.Add(item1);
                                    lItem2 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item1)).Select(t => t.USER_ID).ToList();
                                    if (lItem2.Count > 0)
                                    {
                                        foreach (var item2 in lItem2)
                                        {
                                            final.Add(item2);
                                            lItem3 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item2)).Select(t => t.USER_ID).ToList();
                                            if (lItem3.Count > 0)
                                            {
                                                foreach (var item3 in lItem3)
                                                {
                                                    final.Add(item3);

                                                    lItem4 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item3)).Select(t => t.USER_ID).ToList();

                                                    if (lItem4.Count > 0)
                                                    {
                                                        foreach (var item4 in lItem4)
                                                        {
                                                            final.Add(item4);
                                                            lItem5 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item4)).Select(t => t.USER_ID).ToList();

                                                            if (lItem5.Count > 0)
                                                            {
                                                                foreach (var item5 in lItem5)
                                                                {
                                                                    final.Add(item5);
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }

                            }


                        }

                        List<int> xxxxxx = new List<int>();
                       
                        xxxxxx = (final.Distinct().ToList());

                        txtG.Text = xxxxxx.Count.ToString();
                    }
                    catch
                    {


                    }

                 
                }
            }
        }

    }
}



