using WebApplication50.Data;

namespace WebApplication50.Web.Models
{
    public class HomeViewModel
    {
        public List<Listing> Listings { get; set; }
        public User User {get; set;}
        //public List<int> UserListingIds { get; set; }
    }
}
