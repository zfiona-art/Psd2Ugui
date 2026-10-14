public class CsvUtil
{
    public static CSVTable GetCSVTable(string csvName)
    {
        return new CSVLoader().LoadCSV("Config/" + csvName);
    }
}