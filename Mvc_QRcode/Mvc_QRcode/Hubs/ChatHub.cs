using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using QRcodeTransfer.Repository;

namespace Mvc_QRcode.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string target_Id, string message)
        {

            if (message == "Finish QRscan!")
            {
                var conn = new UserRepository();
                var received_Id = conn.GetHubId(target_Id);
                Clients.Client(received_Id).addNewMessageToPage(Context.ConnectionId, message);

            }
            else if (message == "Store Hub Id!")
            {
                var conn = new UserRepository();
                conn.SetHubId(target_Id, Context.ConnectionId);
            }
            else if (message == "Wrong User!")
            {
                var conn = new UserRepository();
                var received_Id = conn.GetHubId(target_Id);
                Clients.Client(received_Id).addNewMessageToPage(Context.ConnectionId, message);

            }
            else if (message == "Button Click!")
            {
                Clients.Client(target_Id).addNewMessageToPage(Context.ConnectionId, message);

            }
            else if(message =="Please Scan Again!")
            {
                Clients.All.addNewMessageToPage(target_Id, message);
            }

        }
    }
}