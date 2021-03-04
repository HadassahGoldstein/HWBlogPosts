using Microsoft.AspNetCore.Mvc;
using HWBlogPosts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HWBlogPosts.Controllers
{
    public class admin : Controller
    {
        private string _connectionString =
        @"Data Source=.\sqlexpress; Initial Catalog=BlogPosts;Integrated Security=true;";
        public IActionResult NewPost()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewPost(BlogPost bp )
        {
            BlogPostDB db = new(_connectionString);
            db.AddBlogPost(bp);
            return Redirect("/Home/Index");
        }
    }
}
