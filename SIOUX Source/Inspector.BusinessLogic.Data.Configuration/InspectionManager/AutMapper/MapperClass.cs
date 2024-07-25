using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Inspector.BusinessLogic.Data.Configuration.InspectionManager.XmlLoaders;
using JSONParser;

namespace Inspector.BusinessLogic.Data.Configuration.InspectionManager.AutMapper
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
