using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CafeProject.MobileWebApplication.Models
{
    public class DetailsModel
    {
        public int ObjectID { get; set; }
        public string ObjectCaption { get; set; }
        public string ObjectType { get; set; }
        public string ObjectIcon { get; set; }
        public ICollection<string> ObjectAddresses { get; set; }
        public string ObjectPhoneNumber { get; set; }
        public ICollection<ObjectOption> ObjectOptions { get; set; }
        public Double[] Statistics { get; set; }
    }
}