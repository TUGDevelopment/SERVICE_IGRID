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
    public partial class encrypt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnDecrypt_Click(object sender, EventArgs e)
        {
            //string msg_error = "";
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


                        //int iCurrent = CNService.getCurrentUser(context);
                        //string Body = "User" + iCurrent.ToString() + "  has decrypt password of" + sUsername + " on" + DateTime.Now.ToString();
                        //string From = "adminartworksystem@thaiunion.com";


                        //MailMessage msg = new MailMessage(From, "chanvit@iamconsulting.co.th", "TU xECM Password Decrypt", Body);
                        //msg.SubjectEncoding = Encoding.UTF8;
                        //msg.BodyEncoding = Encoding.UTF8;
                        //msg.IsBodyHtml = true;
                        //int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                        //SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                        //sc.UseDefaultCredentials = false;

                        //bool IsUseSSL = false;
                        //var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                        //if (SMTPSSL.ToUpper().Trim() == "TRUE")
                        //{
                        //    IsUseSSL = true;
                        //}

                        //try
                        //{
                        //    sc.EnableSsl = IsUseSSL;
                        //    sc.Send(msg);
                        //}
                        //catch (Exception ex)
                        //{
                        //    msg_error = CNService.GetErrorMessage(ex);
                        //    //status = "0";
                        //}
                    }
                    else
                    {
                        lblPassword.Text = "Invalid username";
                    }
                }
            }

        }
    }
}