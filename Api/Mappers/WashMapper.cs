using Api.Models;
using Domain.Databse.Models;

namespace Api.Mappers
{
    public class WashMapper : IMapper<WashDbModel, WashDto>
    {
        public WashDto Map(WashDbModel source)
        {
            return new WashDto
            {
                Id = source.Id,
                Type = source.Type,
                Duration = source.Duration,
                Done = source.Done
            };
        }
    }
}
