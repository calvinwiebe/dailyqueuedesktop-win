using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web;

namespace DailyQueue
{
    class WebServiceSender
    {
        public SendResult send(string link, DailyQueueConfiguration config)
        {
            if (config.User == null || config.Password == null)
            {
                return SendResult.NOT_CONFIGURED;
            }

            bool trying = true;
            CookieContainer cookieJar = new CookieContainer();

            while (trying)
            {

                //the request
                string sRequest = Constants.getDailyQueueAddRequest();
                string content = "{ url : '" + link + "', authenticity_token : '" + config.AuthenticityToken + "' }";
                byte[] contentBytes = Encoding.ASCII.GetBytes(content);

                // prepare the web page we will be asking for
                HttpWebRequest request = createHttpRequest(sRequest, "POST", "application/json", contentBytes, cookieJar, config); 

                // execute the request
                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();

                }
                catch (WebException e)
                {
                    string message = e.Message;
                    return SendResult.FAILURE;
                }

                string json = parseHttpResponse(response);
                JObject o = JObject.Parse(json);
                JObject oLink = (JObject)o["link"];
                if (oLink != null)
                {
                    if (oLink["id"].ToString() != "")
                    {
                        return SendResult.SUCCESS;
                    }
                    else
                    {
                        return SendResult.FAILURE;
                    }
                }
                else
                {
                    if (authenticate(ref config, cookieJar) == SendResult.FAILURE)
                    {
                        trying = false;
                    }
                }
            }

            return SendResult.FAILURE;
            
        }

        private SendResult authenticate(ref DailyQueueConfiguration config, CookieContainer cookieJar)
        {

            //the request 
            string sRequest = Constants.getDailyQueueSignInRequest();
            string content = "{remote: true, commit: 'Sign in', utf8: '✓', user: {remember_me: 1, password: '" + config.Password + "', email: '" + config.User + "'}}";
            byte[] contentBytes = Encoding.ASCII.GetBytes(content);

            // prepare the web page we will be asking for
            HttpWebRequest request = createHttpRequest(sRequest, "POST", "application/json", contentBytes, cookieJar, config); 

            // execute the request
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                config.Cookies = cookieJar.GetCookies(request.RequestUri);
            }
            catch (WebException e)
            {
                string message = e.Message;
                return SendResult.FAILURE;
            }

            string json = parseHttpResponse(response);
            try
            {
                JObject o = JObject.Parse(json);
                JValue oSuccess = (JValue)o["success"];
                JValue oAuthenticityToken = (JValue)o["authenticity_token"];
                if ((bool)oSuccess.Value)
                {
                    config.AuthenticityToken = oAuthenticityToken.Value.ToString();
                    return SendResult.SUCCESS;
                }
                else
                {
                    return SendResult.FAILURE;
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                return SendResult.FAILURE;
            }

        }

        private HttpWebRequest createHttpRequest(string sRequest, string method, string contentType, byte[] contentBytes, CookieContainer cookieJar, DailyQueueConfiguration config)
        {
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(sRequest);
            request.Method = method;
            cookieJar.Add(config.Cookies);
            request.CookieContainer = cookieJar;
            request.ContentLength = contentBytes.Length;
            request.ContentType = contentType;
            if (contentBytes != null)
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(contentBytes, 0, contentBytes.Length);
                requestStream.Close();
            }
            return request;
        }

        private string parseHttpResponse(HttpWebResponse response)
        {

            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            return sb.ToString();
        }
    }
}
