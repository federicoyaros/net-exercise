using AutoMapper;
using CodeChallenge.Dtos;
using CodeChallenge.Models;

namespace CodeChallenge.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BlogPost, BasicBlogPostDto>()
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));

            CreateMap<CreateBlogPostDto, BlogPost>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

            CreateMap<BlogPost, FullBlogPostDto>()
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            CreateMap<Comment, CommentDto>();
        }
    }
}
