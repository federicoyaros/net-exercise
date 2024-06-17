using AutoMapper;
using CodeChallenge.Controllers;
using CodeChallenge.Data;
using CodeChallenge.Dtos;
using CodeChallenge.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CodeChallenge.Tests
{
    public class PostControllerTests : IClassFixture<DbContextFixture>
    {
        private readonly DbContextFixture _fixture;        
        private readonly Mock<IMapper> _mockMapper;
        private readonly PostController _controller;

        public PostControllerTests(DbContextFixture fixture)
        {
            _fixture = fixture;
            _mockMapper = new Mock<IMapper>();
            _controller = new PostController(_fixture.Context, _mockMapper.Object);
        }

        [Fact]
        public void GetBlogPosts_ReturnsOkResultWithSingleBasicBlogPostDto()
        {            
            BlogPost blogPost = new BlogPost {
                Id = 1,
                Title = "Test post ",
                Content = "Test content",
                Comments = new List<Comment> { new Comment { Id = 1, Content = "Test comment", Date = DateTime.Now } }
            };

            _fixture.Context.BlogPosts.Add(blogPost);
            _fixture.Context.SaveChanges();

            BasicBlogPostDto basicBlogPostDto = new BasicBlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                CommentCount = blogPost.Comments.Count
            };

            _mockMapper.Setup(m => m.Map<List<BasicBlogPostDto>>(It.IsAny<List<BlogPost>>()))
                .Returns(new List<BasicBlogPostDto> { basicBlogPostDto });

            OkObjectResult result = _controller.GetBlogPosts() as OkObjectResult;
           
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var blogPostDtos = result.Value as List<BasicBlogPostDto>;
            blogPostDtos.Should().NotBeNull();
            
            blogPostDtos.Should().HaveCount(1);
            
            BasicBlogPostDto blogPostDto = blogPostDtos.First();
            blogPostDto.Id.Should().Be(blogPost.Id); 
            blogPostDto.Title.Should().Be(blogPost.Title); 
            blogPostDto.CommentCount.Should().Be(blogPost.Comments.Count); 
        }

        [Fact]
        public void GetBlogPostById_ExistingId_ReturnsOkResultWithFullBlogPostDto()
        {
            int blogPostId = 1;
            BlogPost blogPost = new BlogPost
            {
                Id = blogPostId,
                Title = "Test post",
                Content = "Test content",
                Comments = new List<Comment> { new Comment { Id = 1, Content = "Test comment" } }
            };

            _fixture.Context.BlogPosts.Add(blogPost);
            _fixture.Context.SaveChanges();
            
            _mockMapper.Setup(m => m.Map<FullBlogPostDto>(It.IsAny<BlogPost>()))
                .Returns((BlogPost source) => new FullBlogPostDto
                {
                    Id = source.Id,
                    Title = source.Title,
                    Content = source.Content,
                    Comments = source.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content                        
                    }).ToList()
                });
            
            IActionResult actionResult = _controller.GetBlogPostById(blogPostId);
            OkObjectResult result = actionResult as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            FullBlogPostDto fullBlogPostDto = result.Value as FullBlogPostDto;
            fullBlogPostDto.Should().NotBeNull();
            fullBlogPostDto.Id.Should().Be(blogPost.Id);
            fullBlogPostDto.Title.Should().Be(blogPost.Title);
            fullBlogPostDto.Content.Should().Be(blogPost.Content);
            fullBlogPostDto.Comments.Should().HaveCount(blogPost.Comments.Count);
        }

        [Fact]
        public void GetBlogPostById_NonExistingId_ReturnsNotFound()
        {            
            int nonExistingId = 1000;            
            
            NotFoundResult result = _controller.GetBlogPostById(nonExistingId) as NotFoundResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void Post_ValidData_ReturnsOkResult()
        {
            CreateBlogPostDto createBlogPostDto = new CreateBlogPostDto { Title = "Test post", Content = "Test content" };
            BlogPost mockBlogPost = new BlogPost { Id = 1, Title = createBlogPostDto.Title, Content = createBlogPostDto.Content };

            _mockMapper.Setup(m => m.Map<BlogPost>(createBlogPostDto)).Returns(mockBlogPost);
         
            OkResult result = _controller.Post(createBlogPostDto) as OkResult;
 
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);                                 
        }       

        [Fact]
        public void AddCommentToBlogPost_ExistingBlogPostAndValidData_ReturnsOkResult()
        {            
            int blogPostId = 1;
            BlogPost mockBlogPost = new BlogPost { Id = blogPostId, Title = "Test post", Content = "Test content", Comments = new List<Comment>() };
            CreateCommentDto createCommentDto = new CreateCommentDto { Content = "Test comment" };

            _mockMapper.Setup(m => m.Map<Comment>(createCommentDto)).Returns(new Comment { Content = createCommentDto.Content });

            _fixture.Context.BlogPosts.Add(mockBlogPost);
            _fixture.Context.SaveChanges();
            
            IActionResult actionResult = _controller.AddCommentToBlogPost(blogPostId, createCommentDto);
            OkResult result = actionResult as OkResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);            

            mockBlogPost.Comments.Should().ContainSingle();
            mockBlogPost.Comments.First().Content.Should().Be(createCommentDto.Content);
        }

        [Fact]
        public void AddCommentToBlogPost_BlogPostNotFound_ReturnsNotFoundResult()
        {            
            int nonExistingBlogPostId = 1000;
            CreateCommentDto createCommentDto = new CreateCommentDto { Content = "Test comment" };

            NotFoundResult result = _controller.AddCommentToBlogPost(nonExistingBlogPostId, createCommentDto) as NotFoundResult;
            
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);            

            int changes = _fixture.Context.SaveChanges();
            changes.Should().Be(0);
        }
    }
}
