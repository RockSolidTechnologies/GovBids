using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;
using GovBids.DAL;
using PushSharp;
using PushSharp.Core;
using PushSharp.Apple;
using PushSharp.WindowsPhone;

namespace GovBids
{
    public partial class PushBid : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //loading the dropdown list
                var list = GovBidsDAL.FetchDataBids(String.Empty, String.Empty, String.Empty, String.Empty);
                list.Insert(0, new AvailBidResponseAvailBidsBid { ID = "0", Agency = "", Category = "", DateOpened = "", DatePosted = "", ItemID = "0", Location = "", PreBidDate = "", SpecificationsDate = "", Title = "" });
                ddlBids.DataSource = list;
                ddlBids.DataValueField = "ID";
                ddlBids.DataTextField = "Title";
                ddlBids.DataBind();
            }
        }

        protected void btnPush_Click(object sender, EventArgs e)
        {
            //Create our push services broker
            var push = new PushBroker();
            try
            {
                if (ddlBids.SelectedItem.Value != null && Convert.ToInt32(ddlBids.SelectedItem.Value) != 0)
                {
                    //Wire up the events for all the services that the broker registers
                    push.OnNotificationSent += NotificationSent;
                    push.OnNotificationFailed += NotificationFailed;

                    //Apple settings for push
                    string appdatafolder = Server.MapPath("~\\App_Data\\");
                    var appleCert = File.ReadAllBytes(Path.Combine(appdatafolder, ConfigurationManager.AppSettings["AppleCert"]));


                    //Development = false; Production = True
                    push.RegisterAppleService(new ApplePushChannelSettings(Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]), appleCert, ConfigurationManager.AppSettings["AppleCertPWD"]));

                    push.RegisterWindowsPhoneService();


                    //fetch all devices for push
                    foreach (var item in GovBidsDAL.FetchDataDeviceTokens())
                    {

                        switch (item.DeviceType)
                        {
                            case "iOS":
                                //Queue notification
                                push.QueueNotification(new AppleNotification()
                                                     .ForDeviceToken(item.DeviceToken)
                                                     .WithAlert(ConfigurationManager.AppSettings["NotificationLabel"] + " : " + GovBidsDAL.FetchDataBidByID(Convert.ToInt32(ddlBids.SelectedItem.Value)).Title)
                                                     .WithBadge(1)
                                                     .WithSound(ConfigurationManager.AppSettings["AppleSoundName"]));
                                break;
                            case "WP8":
                                push.QueueNotification(new WindowsPhoneToastNotification()
                                                     .ForEndpointUri(new Uri(item.DeviceToken))
                                                     .ForOSVersion(WindowsPhoneDeviceOSVersion.Eight)
                                                     .WithBatchingInterval(BatchingInterval.Immediate)
                                                     .WithNavigatePath("/MainPage.xaml")
                                                     .WithText1(ConfigurationManager.AppSettings["NotificationLabel"])
                                                     .WithText2(GovBidsDAL.FetchDataBidByID(Convert.ToInt32(ddlBids.SelectedItem.Value)).Title));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //Stop and wait for the queues to drains
                try
                {
                    push.StopAllServices();
                }
                catch (Exception) { }

                push = null;
            }
        }

        static void NotificationSent(object sender, INotification notification)
        {
            //Your notification sent here
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            //Your notification sent here
        }

        /// <summary>
        /// Handles the change to enable the push button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlBids_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btnPush.Enabled = Convert.ToInt32(ddlBids.SelectedItem.Value) != 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //protected void lnkTest_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string appdatafolder = Server.MapPath("~\\App_Data\\");
        //        string certPath = Path.Combine(appdatafolder, ConfigurationManager.AppSettings["AppleCert"]);

        //        ScriptManager.RegisterStartupScript(Page, GetType(), "disp_alert1", "<script>alert('" + appdatafolder.Replace("\\", "\\\\") + "');alert('" + certPath.Replace("\\", "\\\\") + "');</script>", false);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}