using PushSharp;
using PushSharp.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TripServiceApp
{
    public  class Common
    {
        public async static Task<bool> SendGms(string notifyId, string tripUserId, string tripId, string titleText, string contentText, string type, string tripCode = null, 
            string userCode = null, string tripUserName = null, string tripDisplayName = null, string extras = null)
        {
            bool success = true;
            try
            {
                var json = @"
                    {{
                        ""titleMsg"" : ""{0}"",
                        ""contentMsg"" : ""{1}"",
                        ""tripId"" : ""{2}"",
                        ""tripUserId"" : ""{3}"",
                        ""type"" : ""{4}"",
                        ""tripCode"" : ""{5}"",
                        ""tripUserCode"" : ""{6}"",
                        ""tripUserName"" : ""{7}"",
                        ""tripDisplayName"" : ""{8}""
                    }}
                ";
                json = String.Format(json, titleText, contentText, tripId, tripUserId, type, tripCode, userCode, tripUserName, tripDisplayName);

                 new Task( () =>
                    {
                        PushBroker pushBroker = new PushBroker();
                        pushBroker.RegisterGcmService(new GcmPushChannelSettings("AIzaSyAwINnHoq85XTCZBraKW4yxKC_bk4NYqw8"));
                        pushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(notifyId)
                            .WithJson(json));
                    
                        pushBroker.StopAllServices();
                    }
                    
                    ).Start();
            } 

            catch(Exception ex)
            {
                success = false;
            }

            return success;
        }
    }
}