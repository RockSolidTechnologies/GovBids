using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using GovBids;
using System.Xml.Serialization;
using System.IO;
using GovBids.WebService;
using System.Xml.Linq;
using GovBids.Models;
using System.Xml;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Notification;

namespace GovBids
{
    public partial class MainPage : PhoneApplicationPage
    {
        AvailBidsSoapClient client;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //Create soap client for the GovBids service
            client = new AvailBidsSoapClient();
            
            //Connect the Get Bids Completed event (Async)
            client.GetBidsCompleted += client_GetBidsCompleted;

            var progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);

            /// Holds the push channel that is created or found.
            HttpNotificationChannel pushChannel;

            // The name of our push channel.
            string channelName = "ToastUpdateChannel";

            InitializeComponent();

            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());
                //MessageBox.Show(String.Format("Channel Uri is {0}",
                  //  pushChannel.ChannelUri.ToString()));

            }
        }

        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
               // MessageBox.Show(String.Format("Channel Uri is {0}",
                 //   e.ChannelUri.ToString()));

            });
        }

        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }


        /// <summary>
        /// Event that handles the application loaded state.
        /// Use for loading data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Retrieve all

            if ((App.Current as App).filterSettings.Update)
            {
                (App.Current as App).filterSettings.Update = false;
                updateWeb();
            }

            else
            {
                using (var db = new DatabaseManager())
                {
                    if (db.GovBidContext.Bids.Count() > 0)
                    {
                        var settings = IsolatedStorageSettings.ApplicationSettings;

                        DateTime last = new DateTime(1995, 6, 22);

                        if (!settings.TryGetValue("LastUpdated", out last))
                            settings.Add("LastUpdated", last);

                        if ((last - DateTime.Now).TotalDays > 0)
                        {
                            updateWeb();
                        }

                        else
                        {
                            updateDb();
                        }
                    }

                    else
                        updateWeb();
                }
            }
        }

        private void updateDb()
        {
            using (var db = new DatabaseManager())
            {
                List<Bid> bids = db.GovBidContext.Bids.ToList();
                var bidsV = (from b in db.GovBidContext.BidsViewed select b);
                foreach (var bV in bidsV)
                {
                    try
                    {
                        bids.First(p => p.ItemID == bV.Id).Viewed = true;
                    }

                    catch { }
                }
                listBox1.ItemsSource = bids;
            }
        }

        private void updateWeb()
        {
            var filterSettings = (App.Current as App).filterSettings;

            //Get the progress indicator from the system tray
            var proggressIndicator = SystemTray.GetProgressIndicator(this);
            proggressIndicator.IsIndeterminate = true;
            proggressIndicator.IsVisible = true;
            proggressIndicator.Text = "Cargando subastas...";

            //Update bids
            client.GetBidsAsync(filterSettings.Agency, filterSettings.Category, filterSettings.Location, null);
        }

        //Leaving page
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //Get the progress indicator from the system tray
            var proggressIndicator = SystemTray.GetProgressIndicator(this);
            proggressIndicator.IsIndeterminate = false;
            proggressIndicator.IsVisible = false;
        }


        /// <summary>
        /// Using the selection changed event as an Item clicked.
        /// The 7.1 SDK doesn't have one, small improv.
        /// </summary>
        /// <param name="sender">The list box.</param>
        /// <param name="e">The event arguments aren't used.</param>
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //During the method the selection is changed to -1.
            //This indicates that the list box item was reseted to none.
            //Do nothing with it in this case.
            if (listBox1.SelectedIndex != -1)
            {
                //Get the selected bid
                var bid = listBox1.Items[listBox1.SelectedIndex] as Bid;

                //From DB
                using (var databaseManager = new DatabaseManager())
                {
                    if (databaseManager.GovBidContext.BidsViewed.Count(p => p.Id == bid.ItemID) <= 0)
                    {
                        bid.Viewed = true;
                        databaseManager.GovBidContext.BidsViewed.InsertOnSubmit(new BidsViewed() { Date = DateTime.Now, Id = bid.ItemID });
                        databaseManager.GovBidContext.SubmitChanges();
                    }
                }

                //Navigate to the web browser bid view
                NavigationService.Navigate(new Uri("/Pages/BidWebView.xaml?Id=" + bid.ItemID, UriKind.Relative));
            }
        }

        /// <summary>
        /// Filter appbar button click event.
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e">Plain arguments</param>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/FilterSetting.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Event handler for the Soap Client when it finishes a call to GetBids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_GetBidsCompleted(object sender, GetBidsCompletedEventArgs e)
        {
            //Get the progress indicator from the system tray
            var proggressIndicator = SystemTray.GetProgressIndicator(this);
            proggressIndicator.IsIndeterminate = false;
            proggressIndicator.IsVisible = false;

            //Detect errors
            if (e.Error == null)
            {
                (IsolatedStorageSettings.ApplicationSettings)["LastUpdated"] = DateTime.Now;
                (IsolatedStorageSettings.ApplicationSettings).Save();
                var bids = getBidListFromXml(e.Result);
                
                //From DB
                using (var databaseManager = new DatabaseManager())
                {
                    var bidsV = (from b in databaseManager.GovBidContext.BidsViewed select b);
                    foreach (var bV in bidsV)
                    {
                        try
                        {
                            bids.First(p => p.ItemID == bV.Id).Viewed = true;
                        }

                        catch { }
                    }

                    databaseManager.GovBidContext.Bids.DeleteAllOnSubmit(databaseManager.GovBidContext.Bids);
                    databaseManager.GovBidContext.Bids.InsertAllOnSubmit(bids);
                    databaseManager.GovBidContext.SubmitChanges();
                }

                listBox1.ItemsSource = bids;
            }

            else
                MessageBox.Show("Error con conexión de internet.");
        }

        /// <summary>
        /// Returns all the Bids from a string containing XML.
        /// </summary>
        /// <param name="xml">String containing XML data from web service.</param>
        /// <returns></returns>
        private List<Bid> getBidListFromXml(string xml)
        {
            //Create and XML reader from the string
            XmlReader r = XmlReader.Create(new StringReader(xml));

            //Load the document
            XDocument data = XDocument.Load(r);

            //Get all the descendants from Bid
            var test = from query in data.Descendants("Bid")
                       select query;

            //Initialize a bid list (will be return)
            var list = new List<Bid>();

            #region Foreach bid...
            foreach (var query in test.ToList())
            {
                #region Variables (Empty)
                string Title = String.Empty;
                string Agency = String.Empty;
                DateTime SpecificationsDate = DateTime.Now;
                DateTime DateOpened = DateTime.Now;
                DateTime DatePosted = DateTime.Now;
                int ItemID = -1;
                DateTime PreBidDate = DateTime.Now;
                string Category = String.Empty;
                int Id = -1;
                string Location = String.Empty;
                #endregion

                #region Fill empty variable with bid info
                //First check if it is not null (else it will crash)
                if (query.Element("Title") != null)
                    Title = (string)query.Element("Title"); //Cast and assign

                if (query.Element("Agency") != null)
                    Agency = (string)query.Element("Agency");

                try
                {
                    if (query.Element("SpecificationsDate") != null)
                        SpecificationsDate = (DateTime)query.Element("SpecificationsDate");

                    if (query.Element("DateOpened") != null)
                        DateOpened = (DateTime)query.Element("DateOpened");

                    if (query.Element("DatePosted") != null)
                        DatePosted = (DateTime)query.Element("DatePosted");
                    if (query.Element("PreBidDate") != null)
                        PreBidDate = (DateTime)query.Element("PreBidDate");
                }
                catch { }

                if (query.Element("ItemID") != null)
                    ItemID = (int)query.Element("ItemID");

                if (query.Element("Category") != null)
                    Category = (string)query.Element("Category");

                if (query.Element("Id") != null)
                    Id = (int)query.Element("Id");

                if (query.Element("Location") != null)
                    Location = (string)query.Element("Location");
                #endregion

                //Create a Bid object and add it to the list
                list.Add(new Bid
                {
                    Title = Title,
                    Agency = Agency,
                    SpecificationsDate = SpecificationsDate,
                    DateOpened = DateOpened,
                    DatePosted = DatePosted,
                    ItemID = ItemID,
                    PreBidDate = PreBidDate,
                    Category = Category,
                    Id = Id,
                    Location = Location
                });
            }
            #endregion

            //return the list with all the bids
            return list;
        }

        //Mark as... click event
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Get the menu item
            var menuItem = sender as MenuItem;
            //Get the id
            string Id = menuItem.Tag.ToString();
            //Get the bid
            var bid = (from m in listBox1.Items where (m as Bid).ItemID.ToString() == Id select m).First();

            using (var databaseManager = new DatabaseManager())
            {
                if ((bid as Bid).Viewed)
                {
                    var bV = from p in databaseManager.GovBidContext.BidsViewed where p.Id.ToString() == Id select p;
                    databaseManager.GovBidContext.BidsViewed.DeleteOnSubmit(bV.First());
                }

                else
                {
                    databaseManager.GovBidContext.BidsViewed.InsertOnSubmit(new BidsViewed() { Date = DateTime.Now, Id = Convert.ToInt32(Id) });
                    databaseManager.GovBidContext.SubmitChanges();
                }

                databaseManager.GovBidContext.SubmitChanges();
                updateDb();
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            updateWeb();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //Get the menu item
            var menuItem = sender as MenuItem;
            //Get the id
            string Id = menuItem.Tag.ToString();
            //Get the bid
            var bid = (from m in listBox1.Items where (m as Bid).ItemID.ToString() == Id select m).First() as Bid;

            EmailComposeTask email = new EmailComposeTask();
            email.Subject = bid.Title;

            string url = "http://www2.pr.gov/subasta/Pages/aviso.aspx?itemID=" + Id;
            email.Body = url;

            email.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //Get the menu item
            var menuItem = sender as MenuItem;
            //Get the id
            string Id = menuItem.Tag.ToString();

            using (var db = new DatabaseManager())
            {
                //Get the bid
                var bid = db.GovBidContext.Bids.First(p => p.ItemID.ToString() == Id) as Bid;

                if (bid.Favorite)
                    bid.Favorite = false;

                else
                    bid.Favorite = true;

                db.GovBidContext.SubmitChanges();

                updateDb();
            }
        }

        //Favorites
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/FavoritesView.xaml", UriKind.Relative));
        } 
    }
}