using HWBlogPosts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HWBlogPosts.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
         @"Data Source=.\sqlexpress; Initial Catalog=BlogPosts;Integrated Security=true;";
        public IActionResult Index(int page)
        {
            BlogPostDB db = new(_connectionString);
            HomeViewModel vm = new()
            {
                BlogPosts = db.GetPostsPerPage(page),
                Page = page,
                AmountOfPages = db.AmountOfPages()
            };
            return View(vm);
        }
        public IActionResult IndividualBlog(int id = -1)
        {
            BlogPostDB db = new(_connectionString);
            if (id == -1)
            {
                id = db.MostRecentBPId();
            }
            IndividualPostViewModel vm = new()
            {
                BlogPost = db.GetBlogPost(id),
                Comments = db.GetCommentsForPost(id),
                Name = Request.Cookies["name"]
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult IndividualBlog(Comment c)
        {
            BlogPostDB db = new(_connectionString);
            db.AddComment(c);
            Response.Cookies.Append("name", c.Name);
            return Redirect($"/Home/IndividualBlog?id={c.BlogPostId}");
        }
    }
}

