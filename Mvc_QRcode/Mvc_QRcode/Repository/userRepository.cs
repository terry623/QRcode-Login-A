using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mvc_QRcode.Repository
{
    public class UserRepository
    {
        private string _connection;

        public UserRepository()
        {
            var isUat = System.Configuration.ConfigurationManager.AppSettings["IsUAT"];
            if (isUat.ToLower() == "y")
            {
                _connection = System.Configuration.ConfigurationManager.AppSettings["Demo"];
            }
            else
            {
                var computerName = Environment.MachineName;

                var connectionCnofig = new Dictionary<string, string>
                {
                    {"Data Source", computerName},
                    {"Initial Catalog", "QRcode_LogIn"},
                    {"Persist Security Info", "True"},
                    {"User ID", "murri_usr"},
                    {"Password", "0000"},
                    {"Pooling", "true"},
                    {"min pool size", "4"},
                    {"max pool size", "32"},
                };
                _connection = string.Join(";", connectionCnofig.Select(x => x.Key + "=" + x.Value));
            }
        }

        public string UserValidatefromQRcode(string userName)
        {
            bool userexitresult = _Userexit(userName);
            if (userexitresult == true)
            {
                return "Login Success";
            }
            else
            {
                return "No User, please register";
            }
        }

        public string UserLogin(string userName, string password)
        {
            bool userexitresult = _Userexit(userName);
            if (userexitresult == true)
            {
                bool uservalidateresult = _UserValidate(userName, password);
                if (uservalidateresult == false) return "Wrong password";
                else return "Login Success";
            }
            else
            {
                return "No User, please register";
            }
        }

        public string UserRegister(string userName, string password)
        {
            bool userexitresult = _Userexit(userName);
            Regex regexIllegalWord = new Regex(@"^[A-Za-z0-9]+$");
            if (!regexIllegalWord.IsMatch(userName) || !regexIllegalWord.IsMatch(password)) return "Include illegal word";
            else if (userexitresult == false)
            {
                _SetUsers(userName, password);
                var usercount = _GetUsers(userName).Count;
                if (usercount == 1) return "Register Success";
                else return "Something error in DB";
            }
            else
                return "User Exist";
        }

        /*--------------------------------Hub----------------------------------------------------------------------*/
        public void SetHubId(string encode, string userHubId)
        {
            var sql = @"Update dbo.connection_list Set [HubId]=@HubId where [url] = @encode";
            DoConnection(conn => conn.Query<string>(sql, new { HubId = userHubId, encode = encode }));
        }

        public string GetHubId(string encode)
        {
            var userhubid = _GetConnection(encode)[0].HubId;
            return userhubid;
        }
        /*--------------------------------Hub----------------------------------------------------------------------*/

        public void SetURL(string userName, string URL)
        {
            var sql = @"INSERT INTO dbo.connection_list([url],[UserName]) VALUES (@url_address,@user_name)";
            DoConnection(conn => conn.Query<string>(sql, new { url_address = URL, user_name = userName }));
        }

        public bool IsUserDeviceExist(string userName)
        {
            var userdeviceStatus = _GetUsers(userName)[0].DeviceStatus;
            if (userdeviceStatus == 1) return true;
            else return false;
        }

        public void SetDeviceStatus(string userName)
        {
            var sql = @"Update dbo.Users Set [DeviceStatus]=1 where [UserName] = @UserName";
            DoConnection(conn => conn.Query<string>(sql, new { UserName = userName }));
        }

        public void SetGuid(string userName, string userGuid)
        {
            var sql = @"Update dbo.Users Set [GUID]=@GUID where [UserName] = @UserName";
            DoConnection(conn => conn.Query<string>(sql, new { GUID = userGuid, UserName = userName }));
        }

        public bool ValidateGuid(string userName, string userGuid)
        {
            if (userGuid == _GetUsers(userName)[0].GUID) return true;
            else return false;
        }

        private bool _UserValidate(string userName, string userPassword)
        {
            var userinf = _GetUsers(userName);
            if (userinf[0].UserPassword == userPassword) { return true; }
            else { return false; }
        }

        private bool _Userexit(string userName)
        {
            var usercount = _GetUsers(userName).Count;
            if (usercount == 1) return true;
            else return false;
        }

        private void _SetUsers(string account, string password)
        {
            var sqlForGetUser = @"INSERT INTO dbo.Users([UserName],[UserPassword],[DeviceStatus]) VALUES (@Account, @Password, @DeviceStatus)";
            DoConnection(conn => conn.Query<string>(sqlForGetUser, new { Account = account, Password = password, DeviceStatus = 0 }));
        }

        private List<Users> _GetUsers(string custName)
        {
            var sql = @"SELECT * FROM [Users] WHERE UserName=@userName";
            return DoConnection(conn => conn.Query<Users>(sql, new { userName = custName })).ToList();
        }

        private List<connection_list> _GetConnection(string encode)
        {
            var sql = @"SELECT * FROM [connection_list] WHERE url=@encode";
            return DoConnection(conn => conn.Query<connection_list>(sql, new { encode = encode })).ToList();
        }

        /*--------------------------------DB Connection----------------------------------------------------------------------*/
        private T DoConnection<T>(Func<SqlConnection, T> func)
        {
            var conn = new SqlConnection(_connection);
            try
            {
                conn.Open();
                var result = func.Invoke(conn);
                return result;
            }
            finally
            {
                conn.Close();
            }
        }
    }

    public class Users
    {
        public int AccountID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string GUID { get; set; }
        public int DeviceStatus { get; set; }

    }

    public class connection_list
    {
        public string UserName { get; set; }
        public string url { get; set; }
        public DateTime createdtime { get; set; }
        public int enable { get; set; }
        public string HubId { get; set; }
    }
}