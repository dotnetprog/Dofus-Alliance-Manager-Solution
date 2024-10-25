namespace DAM.Core.Abstractions.Mapping
{
    public interface IDAMMapper
    {

        TDEST Map<TDEST, TSOURCE>(TSOURCE Source);

    }
}
