using DAM.Core.Abstractions.Mapping;
using MapsterMapper;

namespace DAM.Core.Mappings
{
    public class MapsterDAMMapper : IDAMMapper
    {
        private readonly IMapper _Mapper;
        public MapsterDAMMapper(IMapper mapper)
        {
            this._Mapper = mapper;
        }
        public TDEST Map<TDEST, TSOURCE>(TSOURCE Source)
        {
            return _Mapper.From<TSOURCE>(Source).AdaptToType<TDEST>();
        }
    }
}
