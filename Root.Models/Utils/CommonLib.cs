using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Root.Models.ViewModels;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace Root.Models.Utils
{
    public class ResJsonOutput
    {
        public ResJsonOutput()
        {
            //Header = new Header();
            Data = new object();
            Status = new ResStatus();
        }
        //public Header Header { get; set; }        
        public object Data { get; set; }
        public ResStatus Status { get; set; }

    }
   
    public class ResStatus
    {
        [JsonProperty("i")]
        [DefaultValue(false)]
        public bool IsSuccess { get; set; }
        [JsonProperty("m")]
        public string Message { get; set; }

        [DefaultValue("")]
        [JsonProperty("s")]
        public string StatusCode { get; set; }
    }
    public class JsonOutputForList
    {
        public long TotalCount { get; set; }
        public long RowsPerPage { get; set; }
        public long PageNo { get; set; }
        public object ResultList { get; set; }
    }
    public static class CommonLib
    {
        public static string ServerIP { get; set; }
        public static string LogPath { get; set; }
       
        public static string GetRedisPlainConnectionString(string connStr)
        {
            string ConnKey = connStr.Substring(0, ProgConstants.ConnKeySize);
            string ConnIV = connStr.Substring(ProgConstants.ConnKeySize, ProgConstants.ConnIVSize);
            connStr = connStr.Substring(ProgConstants.ConnKeySize + ProgConstants.ConnIVSize, connStr.Length - ProgConstants.ConnKeySize - ProgConstants.ConnIVSize);
            return TripleDES.Decrypt(connStr, ConnKey, ConnIV);
        }
        public static string GetSortingClass(string fieldname, string cols, string order)
        {
            string str = "";
            if (cols == fieldname)
            {
                str = " onclick=\"CallSort(this, '" + fieldname + "','" + ((order == "Asc") ? "Desc" : "Asc") + "')\" class=\"sort-icon sorting_" + order.ToLower() + "\" ";
            }
            else
            {
                str = " onclick=\"CallSort(this, '" + fieldname + "','Asc')\" class=\" sort-icon sorting sorting_asc\" ";
            }
            return str;
        }
        public static string GetApplicationName()
        {
            return PlatformServices.Default.Application.ApplicationName;
        }
        public static bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }
        public static List<string> UpdateFileName<T>(this T model, string temppath, string strpath)
        {
            string Location = model.GetType().Name;
            List<string> ListOfFiles = new List<string>();
            List<string> CopyFileList = new List<string>();

            PropertyInfo[] props = model.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    object[] attrs = prop.GetCustomAttributes(true);
                    if (attrs != null)
                    {
                        foreach (object attr in attrs)
                        {
                            DataTypeAttribute uploadAttr = attr as DataTypeAttribute;
                            if (uploadAttr != null)
                            {
                                if (uploadAttr.DataType == System.ComponentModel.DataAnnotations.DataType.Upload)
                                {
                                    try
                                    {
                                        string filepath = prop.GetValue(model, null).IsNullString();
                                        if (filepath != "")
                                        {
                                            if (filepath.Contains(CommonLib.DirectorySeparatorChar()))
                                            {
                                                string[] t = filepath.Split(CommonLib.DirectorySeparatorChar());
                                                if (t[0] != Location)
                                                {
                                                    CopyFileList.Add(filepath);
                                                    filepath = t.Last();
                                                }
                                            }
                                            if (filepath.Contains(Location + CommonLib.DirectorySeparatorChar()))
                                            {
                                                filepath = filepath.IsNullString().Replace(Location + CommonLib.DirectorySeparatorChar(), "");
                                            }
                                            filepath = filepath.IsNullString().Replace(Location + CommonLib.DirectorySeparatorChar(), "");
                                            if (filepath.IsNullString() != "")
                                            {
                                                filepath = Location + CommonLib.DirectorySeparatorChar() + filepath;
                                            }

                                            prop.SetValue(model, filepath);
                                            ListOfFiles.Add(filepath);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                else if (prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    IList itemList = prop.GetValue(model, null) as IList;
                    if (itemList != null)
                    {
                        foreach (var item in itemList)
                        {
                            ListOfFiles.AddRange(UpdateFileName(item, temppath, strpath));
                        }
                    }
                }
                else if (prop.PropertyType != typeof(string) && prop.PropertyType.IsClass)
                {
                    var itemList = prop.GetValue(model, null);
                    if (itemList != null)
                    {
                        ListOfFiles.AddRange(UpdateFileName(itemList, temppath, strpath));
                    }
                }
            }
            CopyFiles(CopyFileList, temppath, strpath);
            return ListOfFiles;
        }
        public static void CopyFiles(List<string> FileNames, string temppath, string strpath)
        {
            try
            {
                List<string> ToLocation = FileNames.Where(f => f.IsNullString() != "").Select(s => s.Split(CommonLib.DirectorySeparatorChar())[0]).ToList();
                if (ToLocation.Where(f => f.IsNullString() != "").Count() > 0)
                {
                    foreach (string item in ToLocation)
                    {
                        CommonLib.CheckDir(strpath + item);
                    }
                }

                if (FileNames.Where(f => f.IsNullString() != "").Count() > 0)
                {
                    foreach (string item in FileNames)
                    {
                        string ActualFileName = item.Split(CommonLib.DirectorySeparatorChar()).LastOrDefault();
                        if (System.IO.File.Exists(strpath + item))
                        {
                            try
                            {
                                System.IO.File.Copy(strpath + item, temppath + ActualFileName);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }
        public static string GetRequestIP(this HttpContext context)
        {
            IHeaderDictionary headers = context.Request.Headers;
            string ip = CommonLib.GetHeaderValue(headers, "X-Forwarded-For");
            if (ip.IsNullString() != string.Empty && context?.Connection?.RemoteIpAddress != null)
                ip = context.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullString() != string.Empty)
                ip = CommonLib.GetHeaderValue(headers, "REMOTE_ADDR");

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            if (ip.IsNullString() != string.Empty)
                throw new Exception("Unable to determine caller's IP.");

            return ip;
        }
        public static void MoveFiles(List<string> FileNames, string temppath, string strpath)
        {
            try
            {
                foreach (string folderName in FileNames.Where(x => x.Contains(Path.DirectorySeparatorChar)).Select(x => x.Split(Path.DirectorySeparatorChar)[0]).Distinct())
                {
                    CommonLib.CheckDir(strpath + Path.DirectorySeparatorChar + folderName);
                }

                if (FileNames.Where(f => f.IsNullString() != "").Count() > 0)
                {
                    foreach (string item in FileNames)
                    {
                        string ActualFileName = item.Split(CommonLib.DirectorySeparatorChar()).LastOrDefault();
                        if (System.IO.File.Exists(temppath + ActualFileName))
                        {
                            try
                            {
                                System.IO.File.Move(temppath + ActualFileName, strpath + item);
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }
        public static char DirectorySeparatorChar()
        {
            return Path.DirectorySeparatorChar;
        }
        public static void WriteToFile(string _message)
        {
            string _Location = LogPath;
            //string _Location = webRootInfo + "/Errorlogs";

            CommonLib.CheckDir(_Location);
            _Location = System.IO.Path.Combine(_Location, DateTime.Today.ToString("dd-MM-yyyy") + ".txt");

            try
            {
                StreamWriter _sw = new StreamWriter(_Location, true);
                _sw.Write(_message);
                _sw.Close();
            }
            catch
            {
                //HttpContext.Current.Application["Error"] += _message;
            }
        }
        public static string ConvertTemplateString(string TemplateFormat, string AvailableValues, Dictionary<string, string> Args)
        {
            if (TemplateFormat != null)
            {
                if (Args != null)
                {
                    foreach (KeyValuePair<string, string> argObj in Args)
                    {
                        TemplateFormat = TemplateFormat.Replace("@" + argObj.Key + "#", argObj.Value.IsNullString());
                        TemplateFormat = TemplateFormat.Replace("{#" + argObj.Key + "#}", argObj.Value.IsNullString());
                    }
                }
                foreach (string item in AvailableValues.IsNullString().Split(','))
                {
                    TemplateFormat = TemplateFormat.Replace("@" + item + "#", "");
                    TemplateFormat = TemplateFormat.Replace("{#" + item + "#}", "");
                }
            }
            return TemplateFormat;
        }

        public static string GetHeaderValue(IHeaderDictionary headers, string key)
        {
            if (headers == null) return null;
            if (!string.IsNullOrEmpty(headers[key]))
            {
                return headers[key];
            }
            return null;
        }
        public static string GetRequestJSON(HttpRequest request)
        {
            string bodyStr = null;
            if (request.Body.CanSeek)
            {
                request.Body.Position = 0;
                using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = reader.ReadToEnd();
                }
            }
            return bodyStr;
        }
        public static int GetRowsPerPage()
        {
            return 10;
        }
        public static T GetAppConfig<T>(IConfiguration configuration)
        {
            RequestByString myConfigEncrypted = configuration.GetSection("AppConfig").Get<RequestByString>();
            if (myConfigEncrypted == null || myConfigEncrypted.id == null)
            {
                return configuration.GetSection("AppConfig").Get<T>();
            }
            else
            {
                string EncryptedStr = myConfigEncrypted.id;
                string DBEntitiesKey = EncryptedStr.Substring(0, ProgConstants.ConnKeySize);
                string DBEntitiesIV = EncryptedStr.Substring(ProgConstants.ConnKeySize, ProgConstants.ConnIVSize);
                EncryptedStr = EncryptedStr.Substring(ProgConstants.ConnKeySize + ProgConstants.ConnIVSize, EncryptedStr.Length - ProgConstants.ConnKeySize - ProgConstants.ConnIVSize);
                string AppConfigStr = TripleDES.Decrypt(EncryptedStr, DBEntitiesKey, DBEntitiesIV);
                return CommonLib.ConvertJsonToObject<T>(AppConfigStr);
            }
        }
        public static T ConvertJsonToObject<T>(object obj)
        {
            return (T)JsonConvert.DeserializeObject(obj.IsNullString(), typeof(T));
        }
        public static string IsNullString(this object str)
        {
            if (str == null)
                return string.Empty;
            try
            {
                return str.ToString().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static void CheckDir(string strPath)
        {
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
        }
        public static string ConvertObjectToJson(object obj, bool isFormat = false)
        {
            if (obj != null)
            {
                if (isFormat)
                {
                    return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(obj, new DecimalFormatConverter());
                }
            }
            return string.Empty;
        }
        public static IEnumerable<TSource> FromHierarchy<TSource>(
           this TSource source,
           Func<TSource, TSource> nextItem)
           where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
        public static IEnumerable<TSource> FromHierarchy<TSource>(
  this TSource source,
  Func<TSource, TSource> nextItem,
  Func<TSource, bool> canContinue)
        {
            for (TSource current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }
        public static string GetAllMessages(this Exception exception)
        {
            IEnumerable<string> messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message);
            return string.Join(Environment.NewLine, messages);
        }
    }

    public class DecimalFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal));
        }

        public override void WriteJson(JsonWriter writer, object value,
                                       JsonSerializer serializer)
        {
            writer.WriteValue(string.Format("{0:N2}", value));
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                     object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class KeyValue
    {
        public KeyValue() { }

        public KeyValue(string _Key, string _Value)
        {
            Key = _Key;
            Value = _Value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public static class EncryptDecrypt
    {
        #region Encryption
        private static byte[] GetBytes(string keyBytes, int length)
        {
            byte[] keyBytes1 = new byte[length];
            byte[] parameterKeyBytes = System.Text.Encoding.UTF8.GetBytes(keyBytes);
            Array.Copy(parameterKeyBytes, 0, keyBytes1, 0, Math.Min(parameterKeyBytes.Length, keyBytes1.Length));
            return keyBytes1;
        }

        private static string Array2String<T>(IEnumerable<T> list)
        {
            return "[" + string.Join(",", list) + "]";
        }

        public static string Encrypt(string PlainText, string key, string iv)
        {
            byte[] keyBytes = GetBytes(key, 32);
            byte[] ivBytes = GetBytes(key, 16);
            RijndaelManaged aes = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Key = keyBytes,
                IV = ivBytes
            };

            ICryptoTransform encrypto = aes.CreateEncryptor();

            byte[] plainTextByte = Encoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return BitConverter.ToString(CipherText).Replace("-", string.Empty);
        }

        public static string Decrypt(string encryptedText, string key, string iv)
        {
            try
            {
                int length = encryptedText.Length;
                byte[] keyBytes = GetBytes(key, 32);
                byte[] ivBytes = GetBytes(key, 16);

                string encrytedTextNew = "";
                char[] encrytArray = encryptedText.ToCharArray(0, encryptedText.Length);
                for (int i = 0; i < encryptedText.Length; i++)
                {
                    if (i != 0)
                    {
                        int j = i + 1;
                        if (j % 2 == 0)
                        {
                            encrytedTextNew = encrytedTextNew + encrytArray[i] + "-";
                        }
                        else
                        {
                            encrytedTextNew = encrytedTextNew + encrytArray[i];
                        }
                    }
                    else if (i == 0)
                    {
                        encrytedTextNew = encrytedTextNew + encrytArray[i];
                    }
                }

                encrytedTextNew = encrytedTextNew.Remove(encrytedTextNew.Length - 1);

                RijndaelManaged aes = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Key = keyBytes,
                    IV = ivBytes
                };
                ICryptoTransform encrypto = aes.CreateDecryptor();

                byte[] plainTextByte = Array.ConvertAll<string, byte>(encrytedTextNew.Split('-'), s => Convert.ToByte(s, 16));
                byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
                return ASCIIEncoding.UTF8.GetString(CipherText);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string md5 = s.ToString();
            return md5;
        }

        public static bool IsValidSHA1(string s)
        {
            string regex = @"^[a-fA-F0-9]{40}$";
            Match match = Regex.Match(s, regex, RegexOptions.IgnoreCase);
            return (match.Success);
        }


        public static string GetSha256HashLower(string plainText)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X2").ToLower());
                }
                return builder.ToString();
            }
        }

    }

}
