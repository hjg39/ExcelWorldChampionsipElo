using ExcelWorldChampionshipELO.Core.Domain;

namespace ExcelWorldChampionshipELO.Core.Storage;

public static class TourneyStorage
{
    public static Tourney? LastRunTourney { get; set; }
}