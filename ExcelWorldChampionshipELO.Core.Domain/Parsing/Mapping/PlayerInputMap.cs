using CsvHelper.Configuration;

namespace ExcelWorldChampionshipELO.Core.Domain.Parsing.Mapping;

public sealed class PlayerInputMap : ClassMap<PlayerInput>
{
    public PlayerInputMap()
    {
        Map(m => m.GameNumber).Name("GameNumber");
        Map(m => m.PlayerName).Name("PlayerName");
        Map(m => m.Country).Name("Country");
        Map(m => m.Score).Name("Score");
        Map(m => m.SecondaryScore).Name("SecondaryScore");
    }
}