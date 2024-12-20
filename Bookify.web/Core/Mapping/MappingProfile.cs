﻿namespace Bookify.web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Category, CategoryFormViewModel>().ReverseMap();
        }
    }
}
