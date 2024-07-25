namespace MetaScraper
{
    public enum Phase
    {
        Init = 0,
        LoadUrl = 1,
        Loaded = 2,
        SelectCountry = 3,
        SelectedFirstCountry = 4,
        SelectDates = 5,
        SelectedDates = 6,
        Download = 7,
        Downloaded = 8,
        Select30Days = 9,
        Selected30Days = 10,
        Download30Days = 11,
        Downloaded30Days = 12,
        SelectNextCountry = 13,
        SelectedNextCountry = 14,
        SelectDatesAgain = 15,
        SelectedDatesAgain = 16,
    }
}
