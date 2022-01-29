using BussPassAPI.Classes;
using BussPassAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;

namespace BussPassAPI.Controllers
{
    [RoutePrefix("User")]
    public class UsersController : ApiController
    {
        public FirebaseDB firebaseDB = new FirebaseDB("https://buspass-6d459-default-rtdb.firebaseio.com/");

        [HttpGet, Route("Check")]
        public _OutResponseData Get(string Email,string Mobile,string AadharNo)
        {
            _OutResponseData response = new _OutResponseData();

            try
            {
                if (
                    (Email == null && Mobile == null && AadharNo == null) ||
                    (Email == "" && Mobile == "" && AadharNo == "")
                    )
                {
                    response.code = -1;
                    response.message = "Invalid input parameter";
                }
                FirebaseDB firebaseDBTeams = firebaseDB.Node("Users");
                FirebaseResponse getResponse = firebaseDBTeams.Get();

                
                if (getResponse.Success)
                {
                    JObject json = JObject.Parse(getResponse.JSONContent);
                    response.code = 1;
                    
                    response.data = new List<object>();
                    JEnumerable<JToken> token = json.PropertyValues();
                    List<_User> users = new List<_User>();
                    foreach(JToken j in token)
                    {
                        users.Add(j.ToObject<_User>());  
                    }
                    bool flag = false;
                    response.data = new List<object>();
                    foreach(_User user in users)
                    {
                        if (Mobile != null && Mobile.Trim() != "" && user.MobileNo.Trim() == Mobile.Trim())
                        {
                            response.data.Add(" Mobile Already Exist " + Mobile);
                            flag = true;
                        }
                        if (AadharNo != null && AadharNo.Trim() != "" &&  user.AadharNo.Trim() == AadharNo.Trim())
                        {
                            response.data.Add(" Aadhar Already Exist "  + AadharNo);
                            flag = true;
                        }
                        if (Email != null && Email.Trim() != "" && user.Email.Trim() == Email.Trim())
                        {
                            response.data.Add(" Email Already Exist " + Email);
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        response.code = -1;
                        response.message = "fail";
                        return response;
                    }
                    else
                    {
                        response.message = "success";
                        response.code = 1;
                    }
                }
                else
                {
                    response.code = -1;
                    response.message = "No Data Found";
                }
            }
            catch (Exception ex)
            {
                response.code = -10;
                response.message = ex.Message;
            }
            return response;
        }

        [HttpPost, Route("Insert")]
        public _OutResponse Post(_User user)
        {
            _OutResponse response = new _OutResponse();
            try
            {

                if (user == null)
                {
                    response.code = -1;
                    response.message = "Invalid input parameter";
                    response.data = null;
                    return response;
                }
                if (user.Email.Trim() == ""
                    || user.MobileNo.Trim() == ""
                    || user.AadharNo.Trim() == ""
                    || user.Passcode.Trim() == ""
                    || user.FirstName.Trim() == ""
                    || user.LastName.Trim() == "")
                {
                    response.code = -1;
                    response.message = "Invalid input parameter";
                    response.data = null;
                    return response;
                }

                var data = JsonConvert.SerializeObject(user);
                FirebaseDB firebaseDBTeams = firebaseDB.Node("Users");
                FirebaseResponse postResponse = firebaseDBTeams.Post(data);
                if (postResponse.Success)
                {
                    response.code = 1;
                    response.message = "success";
                    response.data = null;

                }
                else
                {
                    response.code = -1;
                    response.message = "fail";
                    response.data = null;
                }
            }
            catch(Exception ex)
            {
                response.code = -10;
                response.message = "Exception while insert user";
                response.data = null;
            }
                
            return response;
        }
        [HttpGet, Route("Login")]
        public _OutResponse Login(string Username, string Passcode)
        {
            _OutResponse response = new _OutResponse();

            try
            {
                if (Username == null && Passcode == null && Username == "" && Passcode == "")
                {
                    response.code = -1;
                    response.message = "Invalid input parameter";
                    return response;
                }
                FirebaseDB firebaseDBTeams = firebaseDB.Node("Users");
                FirebaseResponse getResponse = firebaseDBTeams.Get();


                if (getResponse.Success)
                {
                    if (getResponse.JSONContent == null || getResponse.JSONContent == "null")
                    {
                        response.code = -1;
                        response.message = "No User Registered";
                        return response;
                    }
                    JObject json = JObject.Parse(getResponse.JSONContent);
                    
                    JEnumerable<JToken> token = json.PropertyValues();
                    List<_User> users = new List<_User>();
                    foreach (JToken j in token)
                    {
                        users.Add(j.ToObject<_User>());
                    }
                    bool flag = false;
                    
                    foreach (_User user in users)
                    {
                        if (user.MobileNo == Username || user.Email == Username)
                        {
                            if (user.Passcode == Passcode)
                            {
                                response.code = 1;
                                response.message = "success";
                                string jsonString = JsonConvert.SerializeObject(user);
                                byte[] byteArray = Encoding.Default.GetBytes(jsonString); 
                                response.data = Convert.ToBase64String(byteArray);
                            }
                            else
                            {
                                response.code = -1;
                                response.message = "Incorrect Passcode";
                            }
                            flag = true;
                            break;
                        }
                        
                    }
                    if (flag)
                    {
                        return response;
                    }
                    else
                    {
                        response.message = "User Not Found. Please register...";
                        response.code = -1;
                    }
                }
                else
                {
                    response.code = -1;
                    response.message = "Error";
                }
            }
            catch (Exception ex)
            {
                response.code = -10;
                response.message = ex.Message;
                
            }
            return response;
        }
        
        [HttpGet, Route("UpdateInfo")]
        public _OutResponseData ChangePassword(string AadharNo, string Email, string MobileNo, string Passcode)
        {
            _OutResponseData response = new _OutResponseData();
            int nRet = 0, input = 0;

            try
            {
                if (AadharNo == null && AadharNo.Trim() == "")
                {
                    response.code = -1;
                    response.message = "Invalid input parameter";
                    return response;
                }
                FirebaseDB firebaseDBTeams = firebaseDB.Node("Users");
                FirebaseResponse getResponse = firebaseDBTeams.Get();


                if (getResponse.Success)
                {
                    if (getResponse.JSONContent == null || getResponse.JSONContent == "null")
                    {
                        response.code = -1;
                        response.message = "No User Registered";
                        return response;
                    }
                    JObject json = JObject.Parse(getResponse.JSONContent);
                    //IEnumerable<JProperty> property = json.Properties();
                    //JEnumerable<JToken> token = json.PropertyValues();
                    Dictionary<string, _User> dict = json.ToObject<Dictionary<string, _User>>();
                    List<_User> users = new List<_User>();
                    bool flag = false;
                    response.data = new List<object>();
                    foreach (string p in dict.Keys)
                    {
                        _User user = dict[p];
                        if (user.AadharNo == AadharNo || user.AadharNo == AadharNo)
                        {
                            flag = true;
                            FirebaseDB db = firebaseDBTeams.NodePath(p);
                            if (Passcode != null && Passcode.Trim() != "")
                            {
                                input++;
                                FirebaseResponse patchResponse = db.Patch("{\"Passcode\":\"" + Passcode + "\"}");
                                if (patchResponse.Success)
                                {
                                    nRet++;
                                }
                                else
                                {
                                    response.data.Add("Error while updating passcode");
                                }
                            }
                            if (Email != null && Email.Trim() != "")
                            {
                                input++;
                                FirebaseResponse patchResponse = db.Patch("{\"Email\":\"" + Email + "\"}");
                                if (patchResponse.Success)
                                {
                                    nRet++;
                                }
                                else
                                {
                                    response.data.Add("Error while updating email");
                                }
                            }
                            if (MobileNo != null && MobileNo.Trim() != "")
                            {
                                input++;
                                FirebaseResponse patchResponse = db.Patch("{\"MobileNo\":\"" + MobileNo + "\"}");
                                if (patchResponse.Success)
                                {
                                    nRet++;
                                }
                                else
                                {
                                    response.data.Add("Error while updating mobile no.");
                                }
                            }
                            if (nRet == input)
                            {
                                response.code = 1;
                                response.message = "success";
                            }
                            else
                            {
                                response.code = -1;
                                response.message = "fail";
                            }

                            break;
                        }
                    }
                    if (!flag)
                    {
                        response.code = -1;
                        response.message = "User Not Found";
                        
                    }
                   
                }
                else
                {
                    response.code = -1;
                    response.message = "Error";
                }
            }
            catch (Exception ex)
            {
                response.code = -10;
                response.message = ex.Message;

            }
            return response;
        }

        [HttpGet, Route("OTP")]
        public Dictionary<string, string> GenerateOTP(string ToEmailId)
        {
            
            Dictionary<string, string> dict = new Dictionary<string, string>();
            bool flag = false;
            string pwd = "";
            string FromAddress = "";
            string otp = "";
            try
            {
                FromAddress = "devarshipimpale@gmail.com";
                pwd = "Kanhaiya@3108";




                if (ToEmailId.Trim() != string.Empty)
                {
                    MailMessage mm = new MailMessage(FromAddress, ToEmailId);

                    mm.Subject = "OTP";
                    otp = new Random().Next(1000, 9999).ToString();
                    mm.Body = "Your OTP is " + otp;
                    //if (Subject.Contains("LMS"))
                    //{
                    //    mm.IsBodyHtml = true;
                    //}
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient
                    {
                        //Host = "smtp.tajhotels.com",
                        //Port = 587,
                        //EnableSsl = false,
                        //DeliveryMethod = SmtpDeliveryMethod.Network,
                        //Credentials = new NetworkCredential(FromAddress, pwd),
                        //Timeout = 20000

                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(FromAddress, pwd),
                        Timeout = 20000
                    };

                    smtp.Send(mm);
                    flag = true;
                }
            }
            catch (SystemException ex)
            {
               
                flag = false;
            }
            finally
            {
            }
            if (!flag)
            {
                dict.Add("OTP", "fail");
            }
            else
            {
                dict.Add("OTP", otp);
            }
            return dict;
        }
    }
}
