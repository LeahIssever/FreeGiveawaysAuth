using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication50.Web.Models;
using WebApplication50.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication50.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=FreeGiveaways;Integrated Security=True";
        //private static List<int> _userListings;

        public IActionResult Index()
        {
            //_userListings = HttpContext.Session.Get<List<int>>("listingIds");
            var repo = new FreeGiveawaysRepository(_connectionString);
            var vm = new HomeViewModel();
            vm.Listings = repo.GetListings();
            if (User.Identity.IsAuthenticated)
            {
                vm.User = repo.GetByEmail(User.Identity.Name);
            }
            //if (_userListings == null)
            //{
            //    _userListings = new List<int>();
            //}
            return View(vm);
        }

        public IActionResult NewListing()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/account/login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddListing(Listing listing)
        {
            var repo = new FreeGiveawaysRepository(_connectionString);
            listing.UserId = repo.GetByEmail(User.Identity.Name).Id;
            repo.AddListing(listing);
            //_userListings.Add(id);
            //HttpContext.Session.Set("listingIds", _userListings);
            return Redirect("/home/index");
        }

        [Authorize]
        public IActionResult Account()
        {
            var repo = new FreeGiveawaysRepository(_connectionString);
            return View(new AccountViewModel
            {
                Listings = repo.GetListingsForUser(User.Identity.Name)
            });
        }

        [HttpPost]
        public IActionResult DeleteListing(int id)
        {
            var repo = new FreeGiveawaysRepository(_connectionString);
            repo.DeleteListing(id);
            //_userListings.Remove(id);
            return Redirect("/home/index");
        }
    }

    //public static class SessionExtensions
    //{
    //    public static void Set<T>(this ISession session, string key, T value)
    //    {
    //        session.SetString(key, JsonSerializer.Serialize(value));
    //    }

    //    public static T Get<T>(this ISession session, string key)
    //    {
    //        string value = session.GetString(key);

    //        return value == null ? default(T) :
    //            JsonSerializer.Deserialize<T>(value);
    //    }
    //}
}
