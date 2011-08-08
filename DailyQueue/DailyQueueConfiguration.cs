using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace DailyQueue
{
    class DailyQueueConfiguration
    {
        private string user;
        private string password;
        private string authenticityToken = "";

        private CookieCollection cookies;

        private const string CONFIG_XML = "config.xml";
        private const string USER_NODE_NAME = "user";
        private const string PASSWORD_NODE_NAME = "password";

        public DailyQueueConfiguration()
        {
            cookies = new CookieCollection();
            try
            {
                //grab the user and password values from the config file
                using (XmlTextReader reader = new XmlTextReader(CONFIG_XML))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == USER_NODE_NAME)
                                {
                                    user = reader.ReadInnerXml();
                                }
                                else if (reader.Name == PASSWORD_NODE_NAME)
                                {
                                    password = reader.ReadInnerXml();
                                }
                                break;
                        }
                    }
                    reader.Close();
                }
            }
            catch (FileNotFoundException e)
            {
                string msg = e.Message;
                //create the configuration file
                using (XmlWriter writer = XmlWriter.Create(CONFIG_XML))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("config");
                    writer.WriteElementString("user", "");
                    writer.WriteElementString("password", "");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                
            }
        }

        #region Properties

        public string User
        {
            get { return user; }
            set
            {
                user = value;
                saveProperty(USER_NODE_NAME, value);
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                saveProperty(PASSWORD_NODE_NAME, value);
            }
        }

        public CookieCollection Cookies
        {
            get { return cookies; }
            set { cookies = value; }
        }

        public string AuthenticityToken
        {
            get { return authenticityToken; }
            set { authenticityToken = value; }
        }

        #endregion

        private void saveProperty(string nodeName, string value)
        {
            //save to xml
            XmlDocument doc = new XmlDocument();
            doc.Load(CONFIG_XML);
            XmlNode root = doc.DocumentElement;
            foreach (XmlNode node in root)
            {
                if (node.Name == nodeName)
                {
                    node.InnerText = value;
                }
            
            }
            try
            {
                doc.Save(CONFIG_XML);
            }
            catch (IOException e)
            {
                string message = e.Message;
            }
        }

        public void resetCookies()
        {
            cookies = new CookieCollection();
        }



    }
}
