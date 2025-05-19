using CsvHelper.Configuration;

namespace ExcelWorldChampionshipELO.Core.Domain.Parsing.Mapping;

public sealed class GameInputMap : ClassMap<GameInput>
{
    public GameInputMap()
    {
        Map(m => m.GameNumber).Name("GameNumber");
        Map(m => m.GameDescription).Name("GameDescription");
        Map(m => m.GameName).Name("GameName");
        Map(m => m.Author).Name("Author");
        Map(m => m.MaxPoints).Name("MaxPoints");
    }
}