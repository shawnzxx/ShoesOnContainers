using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ProductCatalogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PicController:ControllerBase
    {
        private readonly IHostingEnvironment _environment;

        public PicController(IHostingEnvironment environment)
        {
            //provide with application root path
            //provide with all injected env variables through command or docker
            _environment = environment;
        }

        // GET api/pic/5
        [HttpGet("{id}")]
        public IActionResult GetImage(int id) {
            var webRoot = _environment.WebRootPath;
            var path = Path.Combine(webRoot + "/Pics", "shoes-" + id + ".png");
            //read as file byte
            var buffer = System.IO.File.ReadAllBytes(path);
            return File(buffer, "image/png");
        }
    }
}
