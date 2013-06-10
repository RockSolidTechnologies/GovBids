using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace GovBids.Models
{
    /// <summary>
    /// Use for accessing the music library and playlist.  Supports I/O.
    /// </summary>
    public class DatabaseManager: IDisposable
    {
        public DatabaseManager()
        {
            GovBidContext = new GovBidDataContext(connectionString);
            if (!GovBidContext.DatabaseExists())
                GovBidContext.CreateDatabase();
        }

        const string connectionString = "Data Source='isostore:GovBid.sdf';Password='saltbidsgovpr2013@123salt';";
        public GovBidDataContext GovBidContext { get; set; }

        public void Dispose()
        {
            GovBidContext.Dispose();
        }
    }


    public class GovBidDataContext : DataContext
    {
        public GovBidDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public Table<BidsViewed> BidsViewed
        {
            get
            {
                return this.GetTable<BidsViewed>();
            }
        }

        public Table<Bid> Bids
        {
            get
            {
                return this.GetTable<Bid>();
            }
        }
    }

    [Table]
    [Index(Columns = "Id Ascending")]
    public class BidsViewed
    {
        [Column(IsPrimaryKey=true)]
        public int Id { get; set; }

        [Column]
        public DateTime Date { get; set; }
    }
}
