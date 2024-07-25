using System;
using AutoMapper;

namespace Inspector.BusinessLogic.Data.Reporting.Results.Automapper
{
    public class MapperClass
    {
        private static readonly Lazy<MapperClass> _instance = new Lazy<MapperClass>(() => new MapperClass());
        public static MapperClass Instance => _instance.Value;

        public IMapper Mapper { get; }

        private MapperClass()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            Mapper = config.CreateMapper();
        }
    }
}
