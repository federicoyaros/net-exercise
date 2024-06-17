namespace CodeChallenge.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        public DateTime Date { get; set; }

        public virtual BlogPost BlogPost { get; set; }

        public int BlogPostId { get; set; }
    }
}
