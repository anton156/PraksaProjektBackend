using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        public static IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Post posts)
        {
            if(posts.PostImage.Length > 0)
            {
                string imgext = Path.GetExtension(posts.PostImage.FileName);
                if (imgext == ".jpg" || imgext == ".jpg")
                {
                    try
                    {
                        if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\Images\\"))
                        {
                            Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\Images\\");
                        }

                        using (FileStream filestream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\Images\\" + posts.PostImage.FileName))
                        {
                            posts.PostImage.CopyTo(filestream);
                            filestream.Flush();
                            var imagename = "\\Images\\" + posts.PostImage.FileName;

                            var post = new Post
                            {
                                PostId = posts.PostId,
                                Title = posts.Title,
                                ImagePath = imagename,
                                Content = posts.Content,
                                CreatedDate = DateTime.Now
                            };
                            _context.Post.Add(post);
                            await _context.SaveChangesAsync();
                            return Ok(post);
                        }


                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Only support .jpg and .png" });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Creation Failed!" });
            }
        }
    }
}
