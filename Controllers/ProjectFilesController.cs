using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Updatehost.Controllers
{
    public class ProjectFilesController : Controller
    {
        private IHostingEnvironment _env;
        public ProjectFilesController(IHostingEnvironment env) {
            _env = env;
        }

        public IActionResult Index(string project, string filename = null, long? since = null, bool list = false)
        {
            var webRoot = _env.WebRootPath;

            var path = Path.Combine(webRoot, "App_Data\\projects");
            var projectPath = Path.Combine(path, project);
            var filePath = string.IsNullOrEmpty(filename) ? null : Path.Combine(projectPath, filename);

            if(filePath == null) {
                // Return file listing
                var directories = Directory.EnumerateDirectories(projectPath, "*");
                return Json(directories.Select(directory => Path.GetFileName(directory)).ToArray());
            } else if(list) {
                var files = Directory.EnumerateFiles(filePath, "*");
                return Json(files.Select(file => Path.GetFileName(file)).ToArray());
            } else {
                var files = Directory.EnumerateFiles(filePath);
                var fileNames = files.Select(file => Path.GetFileName(file));
                var latest = fileNames.OrderByDescending(file => long.Parse(file)).FirstOrDefault();
                var latestLong = long.Parse(latest);
                if(since.HasValue && since >= latestLong)
                    return Content("NC", "text/plain"); // NB: NC=No Change
                return File($"~/App_Data/projects/{project}/{filename}/{latest}", "text/plain");
            }
        }
    }
}
