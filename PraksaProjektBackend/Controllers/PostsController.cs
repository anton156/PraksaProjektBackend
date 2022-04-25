using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using PraksaProjektBackend.Auth;
using PraksaProjektBackend.Models;

namespace PraksaProjektBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    public class PostsController : ControllerBase
    {
        public static IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("getallposts")]
        [EnableQuery]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost()
        {
            return await _context.Post.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Post.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPost]
        [Route("createpost")]
        public async Task<IActionResult> Create([FromForm] Post posts)
        {
            if(posts.PostImage.Length > 0)
            {
                string imgext = Path.GetExtension(posts.PostImage.FileName);
                if (imgext == ".jpg" || imgext == ".png")
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
                            return Ok(new Response { Status = "Success", Message = "Post created successfully" });
                        }


                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = ex.Message});
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new Response { Status = "Success", Message = "Post deleted" });
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.PostId == id);
        }


    }
}
