using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using DAL;
using System.Web.Script.Serialization;
using System;
using System.IO;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using BLL.Helpers;

namespace BLL.Services
{
    public class ADService
    {
        public static void SycnADUser()
        {
            PrincipalContext principalContext = new PrincipalContext(
                                         ContextType.Domain,
                                         "thaiunion.co.th",
                                         "DC=thaiunion,DC=co,DC=th",
                                         "service.xecm",
                                         "AAaa123*");

            GroupPrincipal group = GroupPrincipal.FindByIdentity(principalContext, "Artwork-Users");
            var devOrQas = CNService.IsDevOrQAS();
          
            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = CNService.IsolationLevel(context))
                {

                    var POSITION_ID_NOT_FOUND = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "NOT_FOUND" }, context).FirstOrDefault().ART_M_POSITION_ID;

                    var allADUser = (from m in context.ART_M_USER where m.IS_ADUSER == "X" select m).ToList();

                    //by aof 20220111--------------start
                    //foreach (var item in allADUser)
                    //{
                    //    item.IS_ACTIVE = null;
                    //    ART_M_USER_SERVICE.SaveOrUpdate(item, context);
                    //}
                    Console.WriteLine("ADService Latest version 11/01/2022 by TU team.");
                    Console.WriteLine("sv:" + context.Database.Connection.DataSource + "  db:" + context.Database.Connection.Database);
                    List<string> listUserUpdate = new List<string>();
                    //by aof 20220111--------------end
                    Console.WriteLine("----------------start update users are active.--------------------");
                    foreach (UserPrincipal principal in group.Members)
                    {
                        string email = principal.EmailAddress;
                        string userName = principal.SamAccountName;
                        string firstName = principal.GivenName;
                        string lastName = principal.Surname;
                        string password = "init1234";
                        string desc = principal.Description;

                        if (!string.IsNullOrEmpty(userName))
                        {
                            if (!string.IsNullOrEmpty(userName.Trim()))
                            {
                                ART_M_USER user = new ART_M_USER();
                                user.USERNAME = userName.Trim().ToUpper();
                                user.TITLE = "";

                                if (!string.IsNullOrEmpty(firstName)) user.FIRST_NAME = firstName.Trim();
                                else user.FIRST_NAME = "";

                                if (!string.IsNullOrEmpty(lastName)) user.LAST_NAME = lastName.Trim();
                                else user.LAST_NAME = "";

                                user.PASSWORD = EncryptionService.Encrypt(password);

                                if (!string.IsNullOrEmpty(email)) user.EMAIL = email.Trim();
                                else user.EMAIL = "";

                                if (devOrQas)
                                    user.EMAIL = "artwork.thaiunion@gmail.com";

                                user.POSITION_ID = POSITION_ID_NOT_FOUND;
                                user.IS_ACTIVE = "X";
                                user.IS_ADUSER = "X";
                                user.CREATE_BY = -2;
                                user.UPDATE_BY = -2;

                                var checkUser = (from m in context.ART_M_USER where m.USERNAME.ToUpper() == user.USERNAME.ToUpper() select m).ToList();
                                if (checkUser.Count > 0)
                                {
                                    user.USER_ID = checkUser.FirstOrDefault().USER_ID;
                                    if (!string.IsNullOrEmpty(checkUser.FirstOrDefault().PASSWORD))
                                        user.PASSWORD = checkUser.FirstOrDefault().PASSWORD;
                                    user.TITLE = checkUser.FirstOrDefault().TITLE;
                                    user.POSITION_ID = checkUser.FirstOrDefault().POSITION_ID;
                                }

                                ART_M_USER_SERVICE.SaveOrUpdate(user, context);

                                listUserUpdate.Add(user.USERNAME.ToUpper()); //by aof 20220111
                            }
                        }

                        Console.WriteLine(userName + " " + firstName + " " + lastName + " " + email + " " + desc);
                    }
                    Console.WriteLine("----------------finish update users are active.--------------------");

                    //by aof 20220111--------------start
                    if (listUserUpdate != null && allADUser != null )
                    {
                        var listUser = allADUser.Where(w => !listUserUpdate.Contains(w.USERNAME.ToUpper()) && !string.IsNullOrEmpty(w.IS_ACTIVE)).ToList();
                        if (listUser != null)
                        {
                            Console.WriteLine("----------------start update users are not active.--------------------");
                            foreach (var item in listUser)
                            {
                                item.IS_ACTIVE = null;
                                ART_M_USER_SERVICE.SaveOrUpdate(item, context);
                                Console.WriteLine(item.USERNAME + " " + item.FIRST_NAME + " " + item.LAST_NAME + " " + item.EMAIL );
                            }
                            Console.WriteLine("----------------finish update users are not active..--------------------");
                        }
                    }
                    //by aof 20220111--------------end


                    dbContextTransaction.Commit();
                }
            }

            Console.WriteLine("Completed");
        }
    }
}