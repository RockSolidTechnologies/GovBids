using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GovBids.Models;

namespace GovBids.Pages
{
    public partial class FavoritesView : PhoneApplicationPage
    {
        public FavoritesView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new DatabaseManager())
            {
                listBox1.ItemsSource = db.GovBidContext.Bids.Where(p => p.Favorite == true);
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
    }
}