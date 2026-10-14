using System;
using System.Collections.Generic;
public class EventTokenReader
{
    public Dictionary<string, string> GetEventNameTokenDic()
    {
        var csvName = "debug";
        CSVTable csvTable = CsvUtil.GetCSVTable(csvName);
        var dic = new Dictionary<string, string>();
        foreach (CSVRecord record in csvTable.Records)
        {
            string key = "";
            string value = "";
            var headers = csvTable.Headers;
            foreach (string header in csvTable.Headers)
            {
                switch (header)
                {
                    case "EventName":
                        key = record.GetField(header);
                        break;
                    case "EventToken":
                        value = record.GetField(header);
                        break;
                }
            }
            if (!String.IsNullOrEmpty(key))
                dic.TryAdd(key, value);
        }
        return dic;
    }
}