using AutoMapper;
using SWM.Application.Accounts;
using SWM.Core.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareWithMe
{
    public class CustomMapper<TInput, TOutput>
    {
        static MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TInput, TOutput>();
            cfg.CreateMissingTypeMaps = true;
            cfg.ValidateInlineMaps = false;
        });
        public static TOutput Map(TInput ob)
        {    
            return config.CreateMapper().Map<TOutput>(ob);
        }

        public static List<TOutput> MapList(List<TInput> list)
        {
            return list.Select(x => Map(x)).ToList();
        }

    }
}
