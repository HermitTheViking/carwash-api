namespace Api.Mappers
{
    public interface IMapper<TSource, TTarget>
    {
        TTarget Map(TSource source);
    }

    public interface IMapper<TSource, TTarget, TMapperContext>
    {
        TTarget Map(TSource source, TMapperContext context);
    }
}
