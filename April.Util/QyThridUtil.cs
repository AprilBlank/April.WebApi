using April.Util.Entitys;
using April.Util.Entitys.QyThird;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace April.Util
{
    public class QyThridUtil
    {
        #region ========基本配置========
        private static string _CorpID = string.Empty;
        private static string _Secret = string.Empty;
        private static string _AgentID = string.Empty;
        /// <summary>
        /// 企业微信id
        /// </summary>
        public static string CorpID
        {
            get
            {
                if (string.IsNullOrEmpty(_CorpID))
                {
                    _CorpID = AprilConfig.Configuration["QyThird:CorpID"];
                }
                return _CorpID;
            }
        }
        /// <summary>
        /// 企业微信应用秘钥
        /// </summary>
        public static string Secret
        {
            get
            {
                if (string.IsNullOrEmpty(_Secret))
                {
                    _Secret = AprilConfig.Configuration["QyThird:Secret"];
                }
                return _Secret;
            }
        }
        /// <summary>
        /// 企业微信应用id
        /// </summary>
        public static string AgentID
        {
            get
            {
                if (string.IsNullOrEmpty(_AgentID))
                {
                    _AgentID = AprilConfig.Configuration["QyThird:AgentID"];
                }
                return _AgentID;
            }
        }

        #endregion

        #region ========公用方法========
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            QyAccessToken accessToken = null;
            bool isGet = false;
            if (CacheUtil.Exists("QyAccessToken"))
            {
                accessToken = CacheUtil.Get<QyAccessToken>("QyAccessToken");
                if (accessToken.Expire_Time >= DateTime.Now.AddMinutes(1))
                {
                    isGet = true;
                }
            }
            if (!isGet)
            {
                string url = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={CorpID}&corpsecret={Secret}";
                //请求获取
                string res = RequestUtil.HttpGet(url);
                accessToken = JsonConvert.DeserializeObject<QyAccessToken>(res);
                if (accessToken != null && accessToken.ErrCode == 0)
                {
                    accessToken.Expire_Time = DateTime.Now.AddSeconds(accessToken.Expires_In);
                    CacheUtil.Set("QyAccessToken", accessToken, new TimeSpan(2, 0, 0));
                }
                else
                {
                    LogUtil.Error($"获取accesstoken失败——{accessToken.ErrCode},{accessToken.ErrMsg}");
                }
            }

            return accessToken.Access_Token;
        }
        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static string GetMessageType(AprilEnums.MessageType messageType)
        {
            string type = string.Empty;
            switch (messageType)
            {
                case AprilEnums.MessageType.Text:
                    type = "text";
                    break;
                case AprilEnums.MessageType.Image:
                    type = "image";
                    break;
            }
            return type;
        }
        /// <summary>
        /// 获取List<string>拼接字符串
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string GetRangeValue(List<string> lists, string split = "|")
        {
            string value = string.Empty;
            if (lists != null && lists.Count > 0)
            {
                foreach (string str in lists)
                {
                    if (string.IsNullOrEmpty(str))
                    {
                        continue;
                    }
                    if (value.Length > 0)
                    {
                        value += split;
                    }
                    value += $"\"{str}\"";
                }
            }
            return value;
        }

        #endregion

        #region========消息推送========
        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <param name="range">推送范围</param>
        /// <param name="messageType">消息类型</param>
        /// <returns></returns>
        public static bool SendMessage(string content, MessageRange range, AprilEnums.MessageType messageType)
        {
            bool isSend = false;
            if (string.IsNullOrEmpty(content) || content.Length > 2048 || range==null)
            {
                return false;
            }
            string accessToken = GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                return false;
            }
            string url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={accessToken}";
            StringBuilder data = new StringBuilder();
            bool isVaildRange = false;
            if (range.IsAll)
            {
                data.Append($"\"touser\":\"@all\"");
                isVaildRange = true;
            }
            else
            {
                if (range.Users != null && range.Users.Count > 0)
                {
                    data.AppendFormat("\"touser\" : {0}", GetRangeValue(range.Users));
                    isVaildRange = true;
                }

                if (range.Tags != null && range.Tags.Count > 0)
                {
                    if (data.Length > 0)
                    {
                        data.Append(",");
                    }
                    data.AppendFormat("\"totag\" : {0}", GetRangeValue(range.Tags));
                    isVaildRange = true;
                }

                if (range.Departments != null && range.Departments.Count > 0)
                {
                    if (data.Length > 0)
                    {
                        data.Append(",");
                    }
                    data.AppendFormat("\"totag\" : {0}", GetRangeValue(range.Departments));
                    isVaildRange = true;
                }
            }
            if (!isVaildRange)
            {
                //没有发送范围
                return false;
            }
            data.AppendFormat(",\"msgtype\":\"{0}\"", GetMessageType(messageType));
            data.AppendFormat(",\"agentid\":\"{0}\"", AgentID);
            data.Append(",\"text\": {");
            data.AppendFormat("\"content\":\"{0}\"", content);
            data.Insert(0, "{");
            data.Append("}}");
            LogUtil.Debug($"获取到发送消息请求：{data.ToString()}");
            string res = RequestUtil.HttpPost(url, data.ToString(), "application/json");
            LogUtil.Debug($"获取到发送消息回调：{res}");

            return false;
        }
        #endregion
    }
}
