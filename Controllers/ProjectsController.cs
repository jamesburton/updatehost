using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Updatehost.Controllers
{
    public class ProjectsController : Controller
    {
        public class Project { 
            public string name { get; set; } 
            public long id { get; set; } 
            public Project(long _id, string _name) {
                id = _id;
                name = _name;
            }
        }

        public IActionResult Index()
        {
            return Json(new [] { new Project(-1, "Example - 1"), new Project(-2, "Another test project") });
        }
    }
}
