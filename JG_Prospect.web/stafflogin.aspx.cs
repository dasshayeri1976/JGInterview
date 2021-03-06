﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using JG_Prospect.Common.Logger;
using JG_Prospect.BLL;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using ASPSnippets.FaceBookAPI;
using System.Web.Script.Serialization;
using ASPSnippets.GoogleAPI;
using ASPSnippets.TwitterAPI;
using DotNetOpenAuth.AspNet.Clients;
using JG_Prospect.Common;
using System.Web.Services;
using JG_Prospect.Common.modal;

namespace JG_Prospect
{
    public partial class stafflogin : System.Web.UI.Page
    {
        #region '--Members--'

        static int c = 0;
        ErrorLog logErr = new ErrorLog();

        #endregion

        #region '-- Page methods --'

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["PopUpOnSessionExpire"] == null)
            //{
            //    if (c != 0 && Session["LogOut"]!=null)
            //    {
            //        ScriptManager.RegisterStartupScript(this, GetType(), "overlay", "SessionExpire();", true);                    
            //    }
            //    c++;
            //}
            //facebook login
            FaceBookConnect.API_Key = "1617979618482118";
            FaceBookConnect.API_Secret = "1b8ede82b0adbebb2282934247773490";
            //google plus login
            //GoogleConnect.ClientId = "356184594367-5iu5qlbe4ddgtst0p6teae8r2s0b5a6n.apps.googleusercontent.com";
            //GoogleConnect.ClientSecret = "rVkwAed1NzC_-F3Z6yUFiFQ_";
            //GoogleConnect.RedirectUri = Request.Url.AbsoluteUri.Split('?')[0];
            GoogleConnect.ClientId = "230635153352-67pgqgc8n4ao9dhnnr3plb1sbnvga1tu.apps.googleusercontent.com";
            GoogleConnect.ClientSecret = "4t6zZfPMfgLVxSRSItsWeOGo";
            GoogleConnect.RedirectUri = Request.Url.AbsoluteUri.Split('?')[0];


            //Login with Twitter
            // TwitterConnect.API_Key = " hlFND0IQOjA7hMPVmVvKKVlzI";
            // TwitterConnect.API_Secret = "NNL9H5GCNSvNH0XJv4ax2wh9iWbqmqTxO9ydR7ewcX1l7XMY5o";
            TwitterConnect.API_Key = "SWPrFVQ6o5q2f2Zjo5R4iNeFv";
            TwitterConnect.API_Secret = "sHRhjjETwXOF5LwxYvK7yk5jz81OchC7IFSyQGWTKzpVeoWOkd";
            //  CalendarExtender2.EndDate = DateTime.Today;
            //txtDateOfBith.Attributes.Add("readonly", "readonly");
            if (!IsPostBack)
            {
                rdSalesIns.Checked = true;
                JGSession.Designation = "";
                JGSession.DesignationId = 0;
                if (Request.Cookies["UserName"] != null && Request.Cookies["Password"] != null)
                {
                    txtloginid.Text = Request.Cookies["UserName"].Value;
                    txtpassword.Attributes["value"] = Request.Cookies["Password"].Value;
                    chkRememberMe.Checked = true;
                }

                #region Twitter  login
                if (TwitterConnect.IsAuthorized)
                {
                    TwitterConnect twitter = new TwitterConnect();
                    DataTable dt = twitter.FetchProfile();
                    string email = dt.Rows[0]["screen_name"].ToString();
                    string name = dt.Rows[0]["name"].ToString();
                    //pradip sir code

                    //Procedure prObj = new Procedure();
                    //Generic gnObj = new Generic();
                    //gnObj.Username1 = dt.Rows[0]["Id"].ToString();                    
                    //gnObj.StatementName = "Student";
                    //int count = prObj.InsertFacebookUser(gnObj);
                    //if (count == 1)
                    //{
                    //    ScriptManager.RegisterStartupScript(this.Page, GetType(), "duplicatI", "Student();", true);
                    //    return;
                    //}
                    //else if (count == 2)
                    //{
                    //    ScriptManager.RegisterStartupScript(this.Page, GetType(), "register", "Register();", true);
                    //    return;
                    //}
                    try
                    {
                        int isvaliduser = 0;
                        DataSet ds = new DataSet();
                        ds = UserBLL.Instance.getUser(email);
                        string AdminId = string.Empty;
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                JGSession.Username = ds.Tables[0].Rows[0]["Username"].ToString().Trim();
                                JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                AdminId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                isvaliduser = UserBLL.Instance.chklogin(email, txtpassword.Text);
                            }

                            if (isvaliduser > 0)
                            {
                                JGSession.UserLoginId = email;
                                JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track
                                //JGSession.UserPassword = txtpassword.Text.Trim();
                                RememberMe();
                                if (txtloginid.Text.Trim() == AdminId)
                                {
                                    JGSession.AdminUserId = AdminId;
                                    JGSession.UserType = "Admin";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }

                                if (isvaliduser == 1)
                                {
                                    JGSession.UserType = "Admin";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }
                                else if (isvaliduser == 2)
                                {
                                    JGSession.UserType = "JSE";
                                    Response.Redirect("~/home.aspx", true);
                                }
                                else if (isvaliduser == 3)
                                {
                                    JGSession.UserType = "SSE";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }
                                else if (isvaliduser == 4)
                                {
                                    JGSession.UserType = "MM";
                                    Response.Redirect("~/home.aspx", true);
                                }
                                else if (isvaliduser == 5)
                                {
                                    JGSession.UserType = "SM";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }
                                else if (isvaliduser == 6)
                                {
                                    JGSession.UserType = "AdminSec";
                                    Response.Redirect("~/home.aspx", true);
                                }
                                else if (isvaliduser == 7)
                                {
                                    JGSession.UserType = "Employee";
                                    Response.Redirect("~/home.aspx", true);
                                }
                            }
                            else  // if installer
                            {
                                ds = null;
                                ds = InstallUserBLL.Instance.getInstallerUserDetailsByLoginId(txtloginid.Text.Trim());
                                JGSession.Username = ds.Tables[0].Rows[0]["FristName"].ToString().Trim();
                                JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                string AdminInstallerId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                int IsValidInstallerUser = InstallUserBLL.Instance.IsValidInstallerUser(txtloginid.Text.Trim(), txtpassword.Text);
                                if (IsValidInstallerUser > 0)
                                {
                                    JGSession.UserLoginId = txtloginid.Text.Trim();
                                    JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track

                                    //JGSession.UserPassword = txtpassword.Text.Trim();
                                    if (txtloginid.Text.Trim() == AdminInstallerId)
                                    {
                                        JGSession.AdminUserId = AdminInstallerId;
                                    }
                                    JGSession.UserType = "Installer";
                                    RememberMe();
                                    Response.Redirect("~/Installer/InstallerHome.aspx", true);
                                }
                                //else
                                //{
                                //    JGSession.UserLoginId = null;
                                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                                //}
                            }
                        }
                        else
                        {
                            string username = email;
                            //string password = txtSignupPassword.Text;
                            string password = "";
                            try
                            {
                                InstallUserBLL.Instance.AddUserFB(username);
                                Response.Redirect("~/home.aspx", true);
                                //SendActivationLink(username);
                                //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your account is successfully created')", true);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //logErr.writeToLog(ex, this.Page.ToString(), Request.ServerVariables["remote_addr"].ToString());
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                        //  Response.Redirect("ErrorPage.aspx");
                    }

                }
                if (TwitterConnect.IsDenied)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "key", "alert('User has denied access.')", true);
                }
                #endregion

                #region google + login

                if (!string.IsNullOrEmpty(Request.QueryString["code"]) && JGSession.IsGooglePlus == true)
                {
                    string code = Request.QueryString["code"];
                    string json = GoogleConnect.Fetch("me", code);
                    string name, emailGPlus;
                    GoogleProfile profile = new JavaScriptSerializer().Deserialize<GoogleProfile>(json);
                    //lblId.Text = profile.Id;
                    name = profile.DisplayName;
                    emailGPlus = profile.Emails.Find(email => email.Type == "account").Value;
                    try
                    {
                        int isvaliduser = 0;
                        DataSet ds = new DataSet();
                        ds = UserBLL.Instance.getUser(emailGPlus);
                        string AdminId = string.Empty;
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                JGSession.Username = ds.Tables[0].Rows[0]["Username"].ToString().Trim();
                                JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                AdminId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                isvaliduser = UserBLL.Instance.chklogin(emailGPlus, txtpassword.Text);
                            }

                            if (isvaliduser > 0)
                            {
                                JGSession.UserLoginId = emailGPlus;
                                JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track
                                //JGSession.UserPassword = txtpassword.Text.Trim();
                                RememberMe();
                                if (txtloginid.Text.Trim() == AdminId)
                                {
                                    JGSession.AdminUserId = AdminId;
                                    JGSession.UserType = "Admin";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }

                                if (isvaliduser == 1)
                                {
                                    JGSession.UserType = "Admin";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }
                                else if (isvaliduser == 2)
                                {
                                    JGSession.UserType = "JSE";
                                    Response.Redirect("~/home.aspx", true);
                                }
                                else if (isvaliduser == 3)
                                {
                                    JGSession.UserType = "SSE";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }
                                else if (isvaliduser == 4)
                                {
                                    JGSession.UserType = "MM";
                                    Response.Redirect("~/home.aspx", true);
                                }
                                else if (isvaliduser == 5)
                                {
                                    JGSession.UserType = "SM";
                                    Response.Redirect("~/Sr_App/home.aspx", true);
                                }
                                else if (isvaliduser == 6)
                                {
                                    JGSession.UserType = "AdminSec";
                                    Response.Redirect("~/home.aspx", true);
                                }
                                else if (isvaliduser == 7)
                                {
                                    JGSession.UserType = "Employee";
                                    Response.Redirect("~/home.aspx", true);
                                }
                            }
                            else  // if installer
                            {
                                ds = null;
                                ds = InstallUserBLL.Instance.getInstallerUserDetailsByLoginId(txtloginid.Text.Trim());
                                JGSession.Username = ds.Tables[0].Rows[0]["FristName"].ToString().Trim();
                                JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                string AdminInstallerId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                int IsValidInstallerUser = InstallUserBLL.Instance.IsValidInstallerUser(txtloginid.Text.Trim(), txtpassword.Text);
                                if (IsValidInstallerUser > 0)
                                {
                                    JGSession.UserLoginId = txtloginid.Text.Trim();
                                    JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track

                                    //JGSession.UserPassword = txtpassword.Text.Trim();
                                    if (txtloginid.Text.Trim() == AdminInstallerId)
                                    {
                                        JGSession.AdminUserId = AdminInstallerId;
                                    }
                                    JGSession.UserType = "Installer";
                                    RememberMe();
                                    Response.Redirect("~/Installer/InstallerHome.aspx", true);
                                }
                                //else
                                //{
                                //    JGSession.UserLoginId = null;
                                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                                //}
                            }
                        }
                        else
                        {
                            string username = emailGPlus;
                            //string password = txtSignupPassword.Text;
                            string password = "";
                            try
                            {
                                InstallUserBLL.Instance.AddUserFB(username);
                                Response.Redirect("~/home.aspx", true);
                                //SendActivationLink(username);
                                //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your account is successfully created')", true);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //logErr.writeToLog(ex, this.Page.ToString(), Request.ServerVariables["remote_addr"].ToString());
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                        //  Response.Redirect("ErrorPage.aspx");
                    }
                }
                #endregion

                #region Facebook login

                if (JGSession.IsFacebook == true)
                {

                    if (Request.QueryString["error"] == "access_denied")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User has denied access.')", true);
                        return;
                    }

                    string code = Request.QueryString["code"];
                    if (!string.IsNullOrEmpty(code))
                    {
                        string email, name, user, firstName, lastName;
                        string data = FaceBookConnect.Fetch(code, "me");
                        FaceBookUser faceBookUser = new JavaScriptSerializer().Deserialize<FaceBookUser>(data);
                        faceBookUser.PictureUrl = string.Format("https://graph.facebook.com/{0}/picture", faceBookUser.Id);
                        //pnlFaceBookUser.Visible = true;
                        //save data in database
                        user = faceBookUser.Name;
                        email = faceBookUser.Email;
                        try
                        {
                            int isvaliduser = 0;
                            DataSet ds = new DataSet();
                            ds = UserBLL.Instance.getUser(email);
                            string AdminId = string.Empty;
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    JGSession.Username = ds.Tables[0].Rows[0]["Username"].ToString().Trim();
                                    JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                    Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                    AdminId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                    isvaliduser = UserBLL.Instance.chklogin(email, txtpassword.Text);
                                }

                                if (isvaliduser > 0)
                                {
                                    JGSession.UserLoginId = email;
                                    JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track
                                    //JGSession.UserPassword = txtpassword.Text.Trim();
                                    RememberMe();
                                    if (txtloginid.Text.Trim() == AdminId)
                                    {
                                        JGSession.AdminUserId = AdminId;
                                        JGSession.UserType = "Admin";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }

                                    if (isvaliduser == 1)
                                    {
                                        JGSession.UserType = "Admin";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 2)
                                    {
                                        JGSession.UserType = "JSE";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 3)
                                    {
                                        JGSession.UserType = "SSE";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 4)
                                    {
                                        JGSession.UserType = "MM";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 5)
                                    {
                                        JGSession.UserType = "SM";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 6)
                                    {
                                        JGSession.UserType = "AdminSec";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 7)
                                    {
                                        JGSession.UserType = "Employee";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                }
                                else  // if installer
                                {
                                    ds = null;
                                    ds = InstallUserBLL.Instance.getInstallerUserDetailsByLoginId(txtloginid.Text.Trim());
                                    JGSession.Username = ds.Tables[0].Rows[0]["FristName"].ToString().Trim();
                                    JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                    Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                    string AdminInstallerId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                    int IsValidInstallerUser = InstallUserBLL.Instance.IsValidInstallerUser(txtloginid.Text.Trim(), txtpassword.Text);
                                    if (IsValidInstallerUser > 0)
                                    {
                                        JGSession.UserLoginId = txtloginid.Text.Trim();
                                        JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track

                                        //JGSession.UserPassword = txtpassword.Text.Trim();
                                        if (txtloginid.Text.Trim() == AdminInstallerId)
                                        {
                                            JGSession.AdminUserId = AdminInstallerId;
                                        }
                                        JGSession.UserType = "Installer";
                                        RememberMe();
                                        Response.Redirect("~/Installer/InstallerHome.aspx", true);
                                    }
                                    //else
                                    //{
                                    //    JGSession.UserLoginId = null;
                                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                                    //}
                                }
                            }
                            else
                            {
                                string username = email;
                                //string password = txtSignupPassword.Text;
                                string password = "";
                                try
                                {
                                    InstallUserBLL.Instance.AddUserFB(username);
                                    Response.Redirect("~/home.aspx", true);
                                    //SendActivationLink(username);
                                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your account is successfully created')", true);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //logErr.writeToLog(ex, this.Page.ToString(), Request.ServerVariables["remote_addr"].ToString());
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                            //  Response.Redirect("ErrorPage.aspx");
                        }
                    }
                }
                #endregion

                #region Yahoo login
                if (JGSession.IsYahoo == true)
                {
                    try
                    {
                        YahooOpenIdClient yahoo = new DotNetOpenAuth.AspNet.Clients.YahooOpenIdClient();
                        var httpContextBase = new HttpContextWrapper(HttpContext.Current);
                        var res = yahoo.VerifyAuthentication(httpContextBase);
                        if (res.IsSuccessful)
                        {
                            string name, emailyahoo;
                            name = res.ExtraData["fullName"];
                            emailyahoo = res.ExtraData["email"];

                            int isvaliduser = 0;
                            DataSet ds = new DataSet();
                            ds = UserBLL.Instance.getUser(emailyahoo);
                            string AdminId = string.Empty;
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    JGSession.Username = ds.Tables[0].Rows[0]["Username"].ToString().Trim();
                                    JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                    Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                    AdminId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                    isvaliduser = UserBLL.Instance.chklogin(emailyahoo, txtpassword.Text);
                                }

                                if (isvaliduser > 0)
                                {
                                    JGSession.UserLoginId = emailyahoo;
                                    JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track
                                    //JGSession.UserPassword = txtpassword.Text.Trim();
                                    RememberMe();
                                    if (txtloginid.Text.Trim() == AdminId)
                                    {
                                        JGSession.AdminUserId = AdminId;
                                        JGSession.UserType = "Admin";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }

                                    if (isvaliduser == 1)
                                    {
                                        JGSession.UserType = "Admin";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 2)
                                    {
                                        JGSession.UserType = "JSE";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 3)
                                    {
                                        JGSession.UserType = "SSE";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 4)
                                    {
                                        JGSession.UserType = "MM";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 5)
                                    {
                                        JGSession.UserType = "SM";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 6)
                                    {
                                        JGSession.UserType = "AdminSec";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 7)
                                    {
                                        JGSession.UserType = "Employee";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                }
                                else  // if installer
                                {
                                    ds = null;
                                    ds = InstallUserBLL.Instance.getInstallerUserDetailsByLoginId(txtloginid.Text.Trim());
                                    JGSession.Username = ds.Tables[0].Rows[0]["FristName"].ToString().Trim();
                                    JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                    Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                    string AdminInstallerId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                    int IsValidInstallerUser = InstallUserBLL.Instance.IsValidInstallerUser(txtloginid.Text.Trim(), txtpassword.Text);
                                    if (IsValidInstallerUser > 0)
                                    {
                                        JGSession.UserLoginId = txtloginid.Text.Trim();
                                        JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track

                                        //JGSession.UserPassword = txtpassword.Text.Trim();
                                        if (txtloginid.Text.Trim() == AdminInstallerId)
                                        {
                                            JGSession.AdminUserId = AdminInstallerId;
                                        }
                                        JGSession.UserType = "Installer";
                                        RememberMe();
                                        Response.Redirect("~/Installer/InstallerHome.aspx", true);
                                    }
                                    //else
                                    //{
                                    //    JGSession.UserLoginId = null;
                                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                                    //}
                                }
                            }
                            else
                            {
                                string username = emailyahoo;
                                //string password = txtSignupPassword.Text;
                                string password = "";
                                try
                                {
                                    InstallUserBLL.Instance.AddUserFB(username);
                                    Response.Redirect("~/home.aspx", true);
                                    //SendActivationLink(username);
                                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your account is successfully created')", true);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        JGSession.IsYahoo = false;
                    }
                    catch (Exception ex)
                    {
                        //logErr.writeToLog(ex, this.Page.ToString(), Request.ServerVariables["remote_addr"].ToString());
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                        //  Response.Redirect("ErrorPage.aspx");
                        JGSession.IsYahoo = false;
                    }
                }
                #endregion

                #region Microsoft login

                if (JGSession.IsMicrosoft == true)
                {
                    try
                    {
                        if (Request.QueryString["error"] == "unauthorized_client")
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + Request.QueryString["error_description"] + "')", true);
                            return;
                        }

                        string appId = "00000000481C1797";
                        string appSecrets = "cec1ShT5FFjexbtm08qv0w8";

                        MicrosoftClient ms = new MicrosoftClient(appId, appSecrets);
                        var httpContextBase = new HttpContextWrapper(HttpContext.Current);
                        string returnUrl = Request.Url.AbsoluteUri.Split('?')[0];
                        returnUrl = "http://jaylem.localtest.me/login.aspx";
                        Uri authUrl = new Uri(returnUrl);

                        var res = ms.VerifyAuthentication(httpContextBase, authUrl);

                        if (res.IsSuccessful)
                        {
                            string name, emailyahoo, token;
                            token = res.ExtraData["accesstoken"];
                            //userData = GetUserData(token);

                            name = res.ExtraData["name"];
                            emailyahoo = res.ExtraData["username"];

                            int isvaliduser = 0;
                            DataSet ds = new DataSet();
                            ds = UserBLL.Instance.getUser(emailyahoo);
                            string AdminId = string.Empty;
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    JGSession.Username = ds.Tables[0].Rows[0]["Username"].ToString().Trim();
                                    JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                    Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                    AdminId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                    isvaliduser = UserBLL.Instance.chklogin(emailyahoo, txtpassword.Text);
                                }

                                if (isvaliduser > 0)
                                {
                                    JGSession.UserLoginId = emailyahoo;
                                    JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track
                                    //JGSession.UserPassword = txtpassword.Text.Trim();
                                    RememberMe();
                                    if (txtloginid.Text.Trim() == AdminId)
                                    {
                                        JGSession.AdminUserId = AdminId;
                                        JGSession.UserType = "Admin";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }

                                    if (isvaliduser == 1)
                                    {
                                        JGSession.UserType = "Admin";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 2)
                                    {
                                        JGSession.UserType = "JSE";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 3)
                                    {
                                        JGSession.UserType = "SSE";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 4)
                                    {
                                        JGSession.UserType = "MM";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 5)
                                    {
                                        JGSession.UserType = "SM";
                                        Response.Redirect("~/Sr_App/home.aspx", true);
                                    }
                                    else if (isvaliduser == 6)
                                    {
                                        JGSession.UserType = "AdminSec";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                    else if (isvaliduser == 7)
                                    {
                                        JGSession.UserType = "Employee";
                                        Response.Redirect("~/home.aspx", true);
                                    }
                                }
                                else  // if installer
                                {
                                    ds = null;
                                    ds = InstallUserBLL.Instance.getInstallerUserDetailsByLoginId(txtloginid.Text.Trim());
                                    JGSession.Username = ds.Tables[0].Rows[0]["FristName"].ToString().Trim();
                                    JGSession.UserPassword = ds.Tables[0].Rows[0]["Password"].ToString().Trim();
                                    Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();
                                    string AdminInstallerId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                                    int IsValidInstallerUser = InstallUserBLL.Instance.IsValidInstallerUser(txtloginid.Text.Trim(), txtpassword.Text);
                                    if (IsValidInstallerUser > 0)
                                    {
                                        JGSession.UserLoginId = txtloginid.Text.Trim();
                                        JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track

                                        //JGSession.UserPassword = txtpassword.Text.Trim();
                                        if (txtloginid.Text.Trim() == AdminInstallerId)
                                        {
                                            JGSession.AdminUserId = AdminInstallerId;
                                        }
                                        JGSession.UserType = "Installer";
                                        RememberMe();
                                        Response.Redirect("~/Installer/InstallerHome.aspx", true);
                                    }
                                    //else
                                    //{
                                    //    JGSession.UserLoginId = null;
                                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                                    //}
                                }
                            }
                            else
                            {
                                string username = emailyahoo;
                                //string password = txtSignupPassword.Text;
                                string password = "";
                                try
                                {
                                    InstallUserBLL.Instance.AddUserFB(username);
                                    Response.Redirect("~/home.aspx", true);
                                    //SendActivationLink(username);
                                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your account is successfully created')", true);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        JGSession.IsMicrosoft = false;
                    }
                    catch (Exception ex)
                    {
                        //logErr.writeToLog(ex, this.Page.ToString(), Request.ServerVariables["remote_addr"].ToString());
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('Please enter a valid Loginid and password!');", true);
                        //  Response.Redirect("ErrorPage.aspx");
                        JGSession.IsMicrosoft = false;
                    }
                }
                #endregion

                //if (!JGSession.IsActive)
                {
                    if (JGSession.IsCustomer)
                    {
                        rdCustomer.Checked = true;
                        rdSalesIns.Checked = false;
                        rdUserType_CheckedChanged(rdCustomer, e);
                    }
                    else
                    {
                        rdCustomer.Checked = false;
                        rdSalesIns.Checked = true;
                        rdUserType_CheckedChanged(rdSalesIns, e);
                    }

                    txtloginid.Text = JGSession.UserLoginId;
                    txtpassword.Text = JGSession.UserPassword;
                    txtpassword.Attributes.Add("value", JGSession.UserPassword);

                    if (!string.IsNullOrEmpty(txtloginid.Text) && !string.IsNullOrEmpty(txtpassword.Text))
                    {
                        btnsubmit_Click(sender, e);
                    }
                }

                // Check if request is coming from www.jmgroveconstruction.com's Employment page, for login
                Uri UrlReferer = Request.UrlReferrer;
                if (UrlReferer != null && UrlReferer.Host.Contains("jmgroveconstruction.com") && UrlReferer.AbsolutePath.Contains("employment.php"))
                {
                    // Login Request.
                    if (Request.Form.Count > 0 && !String.IsNullOrEmpty(Request.Form["txtloginid"]) && !String.IsNullOrEmpty(Request.Form["txtpassword"]))
                    {
                        String strUserName = Request.Form["txtloginid"];
                        String strUserPassword = Request.Form["txtpassword"];

                        if (!String.IsNullOrEmpty(strUserName) && !String.IsNullOrEmpty(strUserPassword))
                        {
                            txtloginid.Text = strUserName;
                            txtpassword.Text = strUserPassword;
                            btnsubmit_Click(sender, e);
                        }
                    }
                }

                // Check if request is coming from www.jmgroveconstruction.com's Employment page, for new applicant user.
                if (UrlReferer != null && UrlReferer.Host.Contains("jmgroveconstruction.com") && UrlReferer.AbsolutePath.Contains("sendemployee.php"))
                {
                    // New user request : Welcome email and Login
                    if (!string.IsNullOrEmpty(Request.QueryString["ID"]) && !string.IsNullOrEmpty(Request.QueryString["Email"]))
                    {
                        CheckForNewUserFromOtherSite(Convert.ToString(Request.QueryString["Email"]), Convert.ToInt32(Request.QueryString["ID"]));
                    }
                }
            }
        }

        #endregion

        #region '-- Control Events --'

        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            try
            {
                JGSession.DesignationId = 0;
                string strRedirectUrl = string.Empty;
                DataSet ds = new DataSet();
                if (rdSalesIns.Checked)
                {
                    JGSession.IsInstallUser = true;

                    #region 'Install User'

                    ds = InstallUserBLL.Instance.getInstallerUserDetailsByLoginId(txtloginid.Text.Trim());
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            Session[JG_Prospect.Common.SessionKey.Key.UserId.ToString()] = ds.Tables[0].Rows[0]["Id"].ToString().Trim();

                            JGSession.Username = ds.Tables[0].Rows[0]["FristName"].ToString().Trim();
                            JGSession.UserProfileImg = ds.Tables[0].Rows[0]["Picture"].ToString();
                            JGSession.LoginUserID = ds.Tables[0].Rows[0]["Id"].ToString();
                            JGSession.Designation = ds.Tables[0].Rows[0]["Designation"].ToString().Trim();
                            JGSession.UserStatus = (JGConstant.InstallUserStatus)Convert.ToInt32(ds.Tables[0].Rows[0]["Status"]);
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DesignationId"].ToString()))
                            {
                                JGSession.DesignationId = Convert.ToInt32(ds.Tables[0].Rows[0]["DesignationId"].ToString().Trim());
                            }
                            if (ds.Tables[0].Rows[0]["IsFirstTime"] != null && ds.Tables[0].Rows[0]["IsFirstTime"].ToString().ToLower() == "true")
                            {
                                JGSession.IsFirstTime = true;
                            }
                        }

                        string AdminInstallerId = ConfigurationManager.AppSettings["AdminUserId"].ToString();
                        int IsValidInstallerUser = InstallUserBLL.Instance.IsValidInstallerUser(txtloginid.Text.Trim(), txtpassword.Text);
                        if (IsValidInstallerUser > 0)
                        {
                            JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track
                            JGSession.UserLoginId = txtloginid.Text.Trim();
                            JGSession.UserPassword = txtpassword.Text.Trim();

                            if (txtloginid.Text.Trim() == AdminInstallerId)
                            {
                                JGSession.AdminUserId = AdminInstallerId;
                            }

                            JGSession.UserType = "Installer";

                            RememberMe();

                            if (JGSession.UserStatus.HasValue && JGSession.UserStatus.Value == JGConstant.InstallUserStatus.Applicant)
                            {
                                strRedirectUrl = "~/ViewApplicantUser.aspx?Id=" + JGSession.LoginUserID;
                            }
                            else
                            {
                                if (JGSession.IsFirstTime == true)
                                {
                                    strRedirectUrl = "~/changepassword.aspx";
                                }

                                else if (Convert.ToString(JGSession.Designation) != "" && JGSession.IsFirstTime == false && (Convert.ToString(JGSession.Designation) == "Sr. Sales" || Convert.ToString(JGSession.Designation) == "Admin" || Convert.ToString(JGSession.Designation) == "Office Manager" || Convert.ToString(JGSession.Designation) == "Recruiter" || Convert.ToString(JGSession.Designation) == "Sales Manager") || Convert.ToString(JGSession.Designation) == "Admin Recruiter")
                                {
                                    #region Redirect to home Or Sr_App/home Or Installer/InstallerHome

                                    if (Convert.ToString(JGSession.Designation) == "Jr. Sales" || Convert.ToString(JGSession.Designation) == "Jr Project Manager")
                                    {
                                        strRedirectUrl = "~/home.aspx";
                                    }
                                    else if (Convert.ToString(JGSession.Designation) == "sales" || Convert.ToString(JGSession.Designation).Trim() == "Admin Recruiter" || Convert.ToString(JGSession.Designation) == "SalesUser" || Convert.ToString(JGSession.Designation) == "SSE")
                                    {
                                        strRedirectUrl = "~/Sr_App/home.aspx";
                                    }
                                    else if (Convert.ToString(JGSession.Designation) == "Sr. Sales" || Convert.ToString(JGSession.Designation) == "A" || Convert.ToString(JGSession.Designation) == "Admin" || Convert.ToString(JGSession.Designation) == "Office Manager" || Convert.ToString(JGSession.Designation) == "Recruiter" || Convert.ToString(JGSession.Designation) == "Sales Manager" || Convert.ToString(JGSession.Designation).Contains("IT"))
                                    {
                                        if (Convert.ToString(JGSession.Designation) == "Admin" || Convert.ToString(JGSession.Designation) == "Recruiter" || Convert.ToString(JGSession.Designation) == "Office Manager")
                                        {
                                            strRedirectUrl = "~/Sr_App/GoogleCalendarView.aspx?lastpage=login";
                                        }
                                        else
                                        {
                                            strRedirectUrl = "~/Sr_App/home.aspx";
                                        }

                                    }
                                    else if (Convert.ToString(JGSession.Designation).StartsWith("Installer"))
                                    {
                                        Response.Redirect("~/Installer/InstallerHome.aspx", false);
                                    }
                                    else if (Convert.ToString(JGSession.Designation) == "SSE")
                                    {
                                        strRedirectUrl = "~/Sr_App/home.aspx";
                                    }
                                    else if (Convert.ToString(JGSession.Designation) == "Forman" || Convert.ToString(JGSession.Designation) == "ForeMan")
                                    {
                                        strRedirectUrl = "~/Installer/InstallerHome.aspx";
                                    }
                                    else if (Convert.ToString(JGSession.Designation) == "SubContractor")
                                    {
                                        strRedirectUrl = "~/Installer/InstallerHome.aspx";
                                    }
                                    else
                                    {
                                        strRedirectUrl = "~/Installer/InstallerHome.aspx";
                                    }

                                    #endregion
                                }

                                else if (Convert.ToString(JGSession.Designation).Contains("IT") || Convert.ToString(JGSession.Designation).ToLower().Contains("developer"))
                                {
                                    strRedirectUrl = "~/Sr_App/ITDashboard.aspx";
                                }

                                else if (Convert.ToString(JGSession.Designation).StartsWith("Installer") && JGSession.IsFirstTime == false)
                                {
                                    strRedirectUrl = "~/Installer/InstallerHome.aspx";
                                }
                                else if (Convert.ToString(JGSession.Designation) == "Jr. Sales" && JGSession.IsFirstTime == false)
                                {
                                    strRedirectUrl = "~/home.aspx";
                                }
                                else if (Convert.ToString(JGSession.Designation) == "SSE" && JGSession.IsFirstTime == false)
                                {
                                    strRedirectUrl = "~/Sr_App/home.aspx";
                                }
                                else if ((Convert.ToString(JGSession.Designation) == "Forman" || Convert.ToString(JGSession.Designation) == "ForeMan") && JGSession.IsFirstTime == false)
                                {
                                    strRedirectUrl = "~/Installer/InstallerHome.aspx";
                                }
                                else
                                {
                                    // Response.Redirect("~/Installer/InstallerHome.aspx");//
                                }

                            }
                        }
                        else
                        {
                            JGSession.UserLoginId = null;
                            JGSession.GuIdAtLogin = null;
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "loginFailMessage();", true);
                        }
                    }
                    #endregion

                    // redirects user to the last accessed page.
                    if (!string.IsNullOrEmpty(strRedirectUrl))
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["returnurl"]))
                        {
                            if (strRedirectUrl.ToLower().Contains("sr_app") && Request.QueryString["returnurl"].ToLower().Contains("sr_app"))
                            {
                                strRedirectUrl = HttpUtility.UrlDecode(Request.Url.Query.Replace("?returnurl=", ""));
                            }
                            else if (!strRedirectUrl.ToLower().Contains("sr_app") && !Request.QueryString["returnurl"].ToLower().Contains("sr_app"))
                            {
                                strRedirectUrl = HttpUtility.UrlDecode(Request.Url.Query.Replace("?returnurl=", ""));
                            }
                        }
                        Response.Redirect(strRedirectUrl);
                    }
                    else
                    {
                        JGSession.UserLoginId = null;
                        JGSession.GuIdAtLogin = null;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "loginFailMessage();", true);
                    }
                }
                else if (rdCustomer.Checked)
                {
                    #region Customer User

                    ds = null;
                    ds = InstallUserBLL.Instance.getCustomerUserLogin(txtloginid.Text.Trim(), txtpassword.Text.Trim());
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToString(ds.Tables[0].Rows[0][0]) != "")
                        {
                            JGSession.UserLoginId = txtloginid.Text.Trim();
                            JGSession.GuIdAtLogin = Guid.NewGuid().ToString(); // Adding GUID for Audit Track
                            JGSession.UserPassword = txtpassword.Text.Trim();
                            JGSession.Username = ds.Tables[0].Rows[0]["CustomerName"].ToString();
                            // Response.Redirect("~/Customer_Panel.php?Cust_Id=" + Convert.ToString(ds.Tables[0].Rows[0][0]), false);
                            // Response.Redirect("50.191.13.206/JGP/Customer_Panel.php?Cust_Id=" + Convert.ToString(ds.Tables[0].Rows[0][0]), false);
                            // Uri url = new Uri("http://50.191.13.206:82/JGP/Customer_Panel.php");                          
                            Uri uri = Context.Request.Url;
                            string host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":82";
                            //  Response.Redirect(host + "/JGP/Customer_Panel.php?Cust_Id=" + Convert.ToString(ds.Tables[0].Rows[0][0]), false);

                            //Response.Redirect("~/Customer_Panel.php?Cust_Id=" + Convert.ToString(ds.Tables[0].Rows[0][0]), false);
                            Response.Redirect("~/Sr_App/Customer_Profile.aspx?CustomerId=" + Convert.ToString(ds.Tables[0].Rows[0][0]), false);
                        }
                    }
                    else
                    {
                        JGSession.UserLoginId = null;
                        JGSession.GuIdAtLogin = null;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "alert('User Name or Password is incorrect');", true);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                //logErr.writeToLog(ex, this.Page.ToString(), Request.ServerVariables["remote_addr"].ToString());
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertBox", "loginFailMessage();", true);
                //  Response.Redirect("ErrorPage.aspx");
            }
        }

        protected void lblForgotUserId_Click(object sender, EventArgs e)
        {
            Response.Redirect("ForgotuserId.aspx");
        }

        protected void lnkForgotPassword_Click(object sender, EventArgs e)
        {
            //forgotpassword.Show();
            Response.Redirect("ForgotPassword.aspx");
        }

        protected void btnSubForgotPassword_Click(object sender, EventArgs e)
        {

        }

        protected void btnForgotEmail_Click(object sender, EventArgs e)
        {
            //string password = "";
            //password = InstallUserBLL.Instance.GetPassword(txtFPEmail.Text);
            //if (password != "")
            //{
            //    string str_Body = "<table><tr><td>Hello,</td></tr><tr><td>your password for the GM Grove Construction is:" + password;
            //    str_Body = str_Body + "</td></tr>";
            //    str_Body = str_Body + "<tr><td></td></tr>";
            //    str_Body = str_Body + "<tr><td>Thanks & Regards.</td></tr>";
            //    str_Body = str_Body + "<tr><td>JM Grove Constructions</td></tr></table>";
            //    using (MailMessage mm = new MailMessage("hr@jmgrove.com", txtFPEmail.Text))
            //    {
            //        mm.Subject = "Foreman Job Acceptance";
            //        mm.Body = str_Body;
            //        mm.IsBodyHtml = false;
            //        SmtpClient smtp = new SmtpClient();
            //        smtp.Host = "smtp.gmail.com";
            //        smtp.EnableSsl = true;
            //        NetworkCredential NetworkCred = new NetworkCredential("Customsofttest@gmail.com", "customsoft567");
            //        smtp.UseDefaultCredentials = true;
            //        smtp.Credentials = NetworkCred;
            //        smtp.Port = 587;
            //        smtp.Send(mm);
            //        //ClientScript.RegisterStartupScript(GetType(), "alert", "alert('Email sent.');", true);
            //    }
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Password send to your registered email id.')", true);
            //    forgotpassword.Hide();
            //    return;
            //}
            //else
            //{
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Id does not exists.')", true);
            //    forgotpassword.Hide();
            //    return;
            //}
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            JGSession.IsFacebook = true;
            FaceBookConnect.Authorize("user_photos,email", Request.Url.AbsoluteUri.Split('?')[0]);
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            JGSession.IsGooglePlus = true;
            GoogleConnect.Authorize("profile", "email");
        }

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            if (!TwitterConnect.IsAuthorized)
            {
                TwitterConnect twitter = new TwitterConnect();
                twitter.Authorize(Request.Url.AbsoluteUri.Split('?')[0]);
            }
        }

        protected void ImageButton5_Click(object sender, ImageClickEventArgs e)
        {
            JGSession.IsMicrosoft = true;
            string appId = "00000000481C1797";
            string appSecrets = "cec1ShT5FFjexbtm08qv0w8";
            string returnUrl = Request.Url.AbsoluteUri.Split('?')[0];
            returnUrl = "http://jaylem.localtest.me/login.aspx";

            MicrosoftClient ms = new DotNetOpenAuth.AspNet.Clients.MicrosoftClient(appId, appSecrets);
            Uri authUrl = new Uri(returnUrl);
            var httpContextBase = new HttpContextWrapper(HttpContext.Current);
            ms.RequestAuthentication(httpContextBase, authUrl);
        }

        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            JGSession.IsYahoo = true;
            string consumerKey = ConfigurationManager.AppSettings["YahooConsumerKey"];
            string consumerSecret = ConfigurationManager.AppSettings["YahooConsumerSecret"];
            string returnUrl = Request.Url.AbsoluteUri.Split('?')[0];

            YahooOpenIdClient yahoo = new DotNetOpenAuth.AspNet.Clients.YahooOpenIdClient();
            Uri authUrl = new Uri(returnUrl);
            var httpContextBase = new HttpContextWrapper(HttpContext.Current);
            yahoo.RequestAuthentication(httpContextBase, authUrl);
        }

        protected void rdUserType_CheckedChanged(object sender, EventArgs e)
        {
            string returnUrl = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["returnurl"]))
            {
                returnUrl = HttpUtility.UrlDecode(Request.Url.Query);
            }

            RadioButton rb = (RadioButton)sender;
            string pageName = Path.GetFileName(Request.Url.AbsolutePath);
            if (rb.Text == "Customer" && pageName == "stafflogin.aspx")
            {
                Response.Redirect("login.aspx" + returnUrl, false);
            }
            if (rb.Text == "Staff" && pageName == "login.aspx")
            {
                Response.Redirect("stafflogin.aspx" + returnUrl, false);
            }
        }

        #endregion

        #region '-- Methods --'

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public void RememberMe()
        {
            if (chkRememberMe.Checked)
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);

            }
            Response.Cookies["UserName"].Value = txtloginid.Text.Trim();
            Response.Cookies["Password"].Value = txtpassword.Text.Trim();
        }

        public void CheckForNewUserFromOtherSite(string UserEmail, int UserID)
        {
            string strReturnValue = InstallUserBLL.Instance.CheckForNewUserByEmaiID(UserEmail, UserID, JG_Prospect.Common.JGConstant.Default_PassWord);

            if (strReturnValue == "YES")
            {
                DataSet ds = InstallUserBLL.Instance.getuserdetails(UserID);
                DataRow drUser = null;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    drUser = ds.Tables[0].Rows[0];

                    string strName = Convert.ToString(drUser["FristName"]) + " " + Convert.ToString(drUser["Lastname"]);
                    string strUserDesignation = Convert.ToString(drUser["Designation"]);
                    string strUserDesignationId = Convert.ToString(drUser["DesignationId"]);
                    string strUserEmail = Convert.ToString(drUser["Email"]);
                    string strUserPhone = Convert.ToString(drUser["Phone"]);

                    string strDesignationCode = JG_Prospect.App_Code.CommonFunction.GetDesignationCode((JGConstant.DesignationType)Convert.ToByte(strUserDesignationId));

                    InstallUserBLL.Instance.SetUserDisplayID(UserID, strDesignationCode, "YES");

                    SendWelcomeEmail(strUserEmail, UserID, strName, strUserDesignation, strUserDesignationId, strUserPhone);

                    string strPopupText = "";
                    strPopupText += "You have successfully been entered into JMGrove Human Resource Database and should have also received an email with login details. ";
                    strPopupText += "Your username is the phone #: {0} or Email: {1} you have entered, your default password is \"jmgrove\". ";
                    strPopupText += "Select continue to proceed with application process or cancel. Save your username and password for future use. ";
                    strPopupText = string.Format(strPopupText, strUserPhone, strUserEmail);

                    Page.ClientScript.RegisterStartupScript
                            (
                                Page.GetType(),
                                "TheConfirm_Ok_Cancel",
                                string.Format(
                                                "TheConfirm_Ok_Cancel('{0}',{1},{2},'{3}');",
                                                strPopupText,
                                                String.Concat("\"", strUserEmail, "^", JG_Prospect.Common.JGConstant.Default_PassWord, "\""),
                                                "function () {}",
                                                "Login Guidance"
                                             ),
                                true
                            );
                }
            }
        }

        public void SendWelcomeEmail(string strUserEmail, int UserID, string strName, string strUserDesignation, string strUserDesignationId, string strUserPhone)
        {
            DesignationHTMLTemplate objHTMLTemplate = HTMLTemplateBLL.Instance.GetDesignationHTMLTemplate(HTMLTemplates.PHP_HR_Welcome_Auto_Email, strUserDesignationId);

            string strHeader = objHTMLTemplate.Header;
            string strSubject = objHTMLTemplate.Subject;
            string strBody = objHTMLTemplate.Body.Replace("#name#", strName).Replace("#Designation#", strUserDesignation);
            string strFooter = objHTMLTemplate.Footer;

            JG_Prospect.App_Code.CommonFunction.SendEmail(strUserDesignationId, strUserEmail, strSubject, strBody, objHTMLTemplate.Attachments);
        }

        #endregion

        #region '-- Classes --'
        public class GoogleProfile
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public Image Image { get; set; }
            public List<Email> Emails { get; set; }
            public string Gender { get; set; }
            public string ObjectType { get; set; }
        }

        public class Email
        {
            public string Value { get; set; }
            public string Type { get; set; }
        }

        public class Image
        {
            public string Url { get; set; }
        }
        #endregion
    }
}
