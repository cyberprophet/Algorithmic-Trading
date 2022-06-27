using Microsoft.AspNetCore.Mvc;

namespace ShareInvest.Server.Controllers
{
    [Route("api/[controller]"), Produces("application/json"), ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status204NoContent), ProducesResponseType(StatusCodes.Status400BadRequest), ApiController]
    public class FileController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContextAsync([FromQuery] long time, [FromQuery] string name)
        {
            try
            {
                if (new FileInfo(Path.Combine(path, update, string.Concat(name, zip))) is FileInfo info && info.Exists)
                    if (time > 0)
                        return Ok(new Interface.File
                        {
                            Name = info.LastWriteTime.Ticks > time ? string.Empty : info.Name,
                            Ticks = info.LastWriteTime.Ticks
                        });
                    else
                        return Ok(new Interface.File
                        {
                            Data = await System.IO.File.ReadAllBytesAsync(info.FullName),
                            Name = nameof(x86).Equals(name) ? x86 : string.Empty
                        });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> PostContextAsync([FromBody] Interface.File file)
        {
            try
            {
                var path = Path.Combine(this.path, update, string.Concat(file.Name, zip));
                await System.IO.File.WriteAllBytesAsync(path, file.Data);

                if (new FileInfo(path) is FileInfo info && info.Exists)
                    return Ok(new Interface.File
                    {
                        Name = info.FullName,
                        Ticks = info.LastWriteTime.Ticks
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }
        public FileController(IWebHostEnvironment env)
        {
            if (new DirectoryInfo(Path.Combine(env.WebRootPath, update)) is DirectoryInfo info &&
                info.Exists is false)
                info.Create();

            path = env.WebRootPath;
        }
        readonly string path;
        const string zip = ".zip";
        const string update = "update";
        const string x86 = " (x86)";
    }
}