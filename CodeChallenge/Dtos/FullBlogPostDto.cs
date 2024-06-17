namespace CodeChallenge.Dtos
{
    public class FullBlogPostDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
