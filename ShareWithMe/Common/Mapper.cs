using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using SWM.Application.Accounts;
using SWM.Application.Users;
using SWM.Core.Accounts;
using SWM.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareWithMe
{
    public class Mapper
    {
        readonly MapperConfiguration config;
        readonly PasswordHasher<User> Hasher = new PasswordHasher<User>();
        public Mapper(IWebHostEnvironment env)
        {
            config = new MapperConfiguration(cfg =>
            {
                /* cfg.CreateMap<CreateProductDto, Product>().ForMember(p => p.ProductCategories, x => x.MapFrom((m, p) => m.Categories?.Select(i => new ProductCategory() { CategoryId = i })));
                 cfg.CreateMap<EditProductDto, Product>().ForMember(p => p.ProductCategories, x => x.MapFrom((m, p) => m.Categories?.Select(i => new ProductCategory() { CategoryId = i })));
                 cfg.CreateMap<Product, ProductDto>()
                   .ForMember(p => p.Images,
                         x => x.MapFrom((m, n) =>
                           new DirectoryInfo(Path.Combine(env.WebRootPath, m.ImagesFolderPath))
                           .GetFiles()
                           .Select(f => f.FullName.Replace(env.WebRootPath, "").Replace("\\", "/"))))
                   .ForMember(p => p.Categories, x => x.MapFrom(p => p.ProductCategories.Select(c => c.Category)));
                 cfg.CreateMap<CategoryDto, Category>();
                 cfg.CreateMap<Category, CategoryDto>().ForMember(c => c.Products, x => x.MapFrom(c => c.CategoryProducts.Count));
                 cfg.CreateMap<CreateCategoryDto, Category>();
                 cfg.CreateMap<EditCategoryDto, Category>();*/
                 cfg.CreateMap<User, UserDto>();
                 cfg.CreateMap<Account, AccountDto>();
                 cfg.CreateMap<UpdateUserDto, User>()
                       .ForMember(u => u.PasswordHash, r => r.MapFrom((e, u) => string.IsNullOrEmpty(e.Password) ? null : Hasher.HashPassword(u, e.Password)));
                 cfg.CreateMap<CreateUserDto, User>()
                       .ForMember(u => u.PasswordHash, r => r.MapFrom((e, u) => string.IsNullOrEmpty(e.Password) ? null : Hasher.HashPassword(u, e.Password)));
                cfg.AllowNullCollections = false;
            });
        }


        public TOutput Map<TInput, TOutput>(TInput obj)
        {
            return config.CreateMapper().Map<TInput, TOutput>(obj);
        }

    }
}
