using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    public class DataController : Controller
    {
        [HttpPost]
        [Route("api/Data/UploadFileAsJson/")]
        public async Task<IActionResult> UploadFileAsJson([FromBody] SharedModels.FileDataModel fileDataModel)
        {
            var filePath = System.IO.Path.GetTempFileName();
            var fileBytes = Convert.FromBase64String(fileDataModel.Base64String);
            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

            return Ok();
        }

        [HttpPost]
        [Route("api/Data/FileUploadAsByteArray")]
        public async Task<IActionResult> FileUploadAsByteArray()
        {
            var files = Request.Form.Files;

            if (files.Count > 0)
            {
                var file = files[0];
                var filePath = System.IO.Path.GetTempFileName();

                using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    using (var sf = file.OpenReadStream())
                    {
                        await sf.CopyToAsync(stream);
                    }
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/Data/GetFileData/")]
        public async Task<IActionResult> GetFileData()
        {
            var temp = System.IO.Path.GetTempPath();
            var fileName = $"{Guid.NewGuid().ToString()}.txt";
            var filePath = System.IO.Path.Combine(temp, fileName);
            var text = "sgfsdgsdgsdgdsg";
            await System.IO.File.WriteAllTextAsync(filePath, text);

            SharedModels.FileDataModel model = new SharedModels.FileDataModel()
            {
                Extension = ".tmp",
                FileName = System.IO.Path.GetFileName(filePath)
            };

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            model.Base64String = Convert.ToBase64String(fileBytes);

            return Ok(model);
        }

        [HttpGet]
        [Route("api/Data/DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(string id)
        {
            var temp = System.IO.Path.GetTempPath();
            var fileName = $"{Guid.NewGuid().ToString()}.txt";
            var filePath = System.IO.Path.Combine(temp, fileName);
            var text = "sgfsdgsdgsdgdsg";
            await System.IO.File.WriteAllTextAsync(filePath, text);

            var stream = System.IO.File.OpenRead(filePath);
            var response = File(stream, "application/octet-stream");

            return response;
        }

    }
}
