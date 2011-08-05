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

            while (trying)
            {

                // used to build entire input
                StringBuilder sb = new StringBuilder();

                // used on each read operation
                byte[] buf = new byte[8192];

                //the request
                string sRequest = Constants.getDailyQueueAddRequest(); 
                string content = "{ url : '" + link + "' }";
                byte[] contentBytes = Encoding.ASCII.GetBytes(content);

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)
                    WebRequest.Create(sRequest);
                request.Method = "POST";
                CookieContainer cookieJar = new CookieContainer();
                cookieJar.Add(config.Cookies);
                request.CookieContainer = cookieJar;
                request.ContentLength = contentBytes.Length;
                request.ContentType = "application/json";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(contentBytes, 0, contentBytes.Length);
                requestStream.Close();


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

                string json = sb.ToString();
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
                    if (authenticate(config) == SendResult.FAILURE)
                    {
                        trying = false;
                    }
                }
            }

            return SendResult.FAILURE;
            
        }

        private SendResult authenticate(DailyQueueConfiguration config)
        {

            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            //the request
            //string sRequest = Constants.getDailyQueueAddRequest(user, link); 
            string sRequest = Constants.getDailyQueueSignInRequest();
            string content = "{remote: true, commit: 'Sign in', utf8: '✓', user: {remember_me: 1, password: '" + config.Password + "', email: '" + config.User + "'}}";
            byte[] contentBytes = Encoding.ASCII.GetBytes(content);

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(sRequest);
            request.Method = "POST";
            CookieContainer cookieJar = new CookieContainer();
            request.CookieContainer = cookieJar;
            request.ContentLength = contentBytes.Length;
            request.ContentType = "application/json";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(contentBytes, 0, contentBytes.Length);
            requestStream.Close();

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

            string json = sb.ToString();
            JObject o = JObject.Parse(json);
            JValue oSuccess = (JValue)o["success"];
            if ((bool)oSuccess.Value)
            {
                return SendResult.SUCCESS;
            }
            else
            {
                return SendResult.FAILURE;
            }

        }
    }
}
