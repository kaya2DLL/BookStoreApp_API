using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //Folder exist?
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "MediaManager");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            //path
            var path = Path.Combine(folder, file?.FileName);

            //Stream
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //response body
            var response = new
            {
                file = file.FileName,
                path = path,
                size = file.Length

            };
            return Ok(response);
        }

        [HttpGet("downland")]
        public async Task<IActionResult>Dowland(string fileNAme)
        {
            //filePath
            var filePAth = Path.Combine(Directory.GetCurrentDirectory(),"MediaManager",fileNAme);

            //ContentType
            var provide = new FileExtensionContentTypeProvider();
            if(!provide.TryGetContentType(fileNAme, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            //Read
            var bytes = await System.IO.File.ReadAllBytesAsync(filePAth);
            return File(bytes, contentType, Path.GetFileName(filePAth));
        }
    }
}
