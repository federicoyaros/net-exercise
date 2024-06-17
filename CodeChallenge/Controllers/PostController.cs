using AutoMapper;
using CodeChallenge.Data;
using CodeChallenge.Dtos;
using CodeChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PostController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetBlogPosts()
        {            
            List<BlogPost> blogPosts = _context.BlogPosts
                .Include(p => p.Comments)
                .ToList();

            List<BasicBlogPostDto> basicBlogPostDto = _mapper.Map<List<BasicBlogPostDto>>(blogPosts);

            return Ok(basicBlogPostDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetBlogPostById(int id)
        {
            BlogPost? blogPost = _context.BlogPosts
                .Include(p => p.Comments)
                .SingleOrDefault(p => p.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            FullBlogPostDto fullBlogPostDto = _mapper.Map<FullBlogPostDto>(blogPost);

            return Ok(fullBlogPostDto);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateBlogPostDto createBlogPostDto)
        {
            if (createBlogPostDto == null)
            {
                return BadRequest("PostDto cannot be null");
            }

            try
            {                
                BlogPost blogPostToCreate = _mapper.Map<BlogPost>(createBlogPostDto);
                
                _context.BlogPosts.Add(blogPostToCreate);
                _context.SaveChanges();                

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("{id}/comments")]
        public IActionResult AddCommentToBlogPost(int id, [FromBody] CreateCommentDto createCommentDto)
        {
            BlogPost? blogPost = _context.BlogPosts
                .Include(p => p.Comments)
                .SingleOrDefault(p => p.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }
            
            Comment newComment = new Comment
            {
                Content = createCommentDto.Content,
                Date = DateTime.Now,
                BlogPostId = blogPost.Id
            };

            blogPost.Comments.Add(newComment);
            _context.SaveChanges();
            
            return Ok();
        }
    }
}
