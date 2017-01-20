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

        protected long _Utc { get { return DateTime.Now.ToUniversalTime().Ticks; } }
        public IActionResult Utc() {
            return Content(_Utc.ToString(), "text/plain");
        }

        [HttpPut]
        public IActionResult Index(string project, string filename) {
            try {
                var webRoot = _env.WebRootPath;
                var path = Path.Combine(webRoot, "App_Data\\projects");
                var projectPath = Path.Combine(webRoot, project);
                var filePath = Path.Combine(projectPath, filename);

                if(!Directory.Exists(projectPath))
                    Directory.CreateDirectory(projectPath);
                if(!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var versionPath = Path.Combine(filePath, _Utc.ToString());
                var fileContents = (new System.IO.StreamReader(Request.Body) as TextReader).ReadToEnd();
                System.IO.File.WriteAllText(versionPath, fileContents);

                return Content("OK", "text/plain");
            } catch(Exception ex) {
                return Content(ex.Message, "text/plain");
            }
        }

        [HttpGet]
        public IActionResult Index(string project, string filename = null, long? since = null, bool list = false)
        {
            if(project == "utc")
                // NB: This will hit here before the default routing, so manually return the method
                return Utc();

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
