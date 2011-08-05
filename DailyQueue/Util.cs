using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailyQueue
{
    class Util
    {
    }

    class Constants
    {
        public const string SUCCESS_MSG = "link successfully uploaded";
        public const string FAILED_MSG = "an error occured during upload. Check user credentials.";
        public const string NOT_CONFIGURED_MSG = "DQ is not configured";
        public const string DAILYQUEUE_DOMAIN = "http://localhost:3000";
        public const string DAILYQUEUE_ADD_URL = "/add.json";
        public const string DAILYQUEUE_SIGN_IN_URL = "/users/sign_in.json";

        public const Int32 BALLOON_TIME = 30000;

        public static string getDailyQueueAddRequest()
        {
            return DAILYQUEUE_DOMAIN + DAILYQUEUE_ADD_URL;
        }

        public static string getDailyQueueSignInRequest()
        {
            return DAILYQUEUE_DOMAIN + DAILYQUEUE_SIGN_IN_URL;
        }
    }

    public enum SendResult
    {
        SUCCESS = 1,
        FAILURE = 2,
        NOT_CONFIGURED = 3
    }

}
