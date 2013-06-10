using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GovBids.Models
{
    [Table]
    [Index(Columns = "Id Ascending")]
    public class Bid
    {
        [Column(IsPrimaryKey=true, IsDbGenerated=true)]
        public int Id { get; set; }

        [Column]
        public int ItemID { get; set; }

        [Column]
        public string Title { get; set; }

        public string TitleTruncated {
            get
            {
                try
                {
                    return Title.Substring(0, Title.IndexOf(" ", 20)) + "...";
                }

                catch
                {
                    return Title;
                }
            }
        }

        public bool Viewed { get; set; }

        [Column]
        public string Category { get; set; }

        [Column]
        public bool Favorite { get; set; }

        [Column]
        public string Agency { get; set; }

        [Column]
        public string Location { get; set; }

        [Column]
        public DateTime DatePosted { get; set; }

        [Column]
        public DateTime DateOpened { get; set; }

        [Column]
        public DateTime PreBidDate { get; set; }

        [Column]
        public DateTime SpecificationsDate { get; set; }
    }
}