using System;
using System.Collections.Generic;

namespace UfaSmartCity.Models
{
    public class InformationTable
    {
        public string Id { get; set; }
        public virtual ModuleType ModuleType { get; set; }
        public int WidthWithModule { get; set; }
        public int HeightWithModule { get; set; }
        public int RowCount { get; set; }
        public virtual List<Content> Contents {get;set;}

        
        public InformationTableViewModel GetViewModel(string stationId, string city)
        {
            return new InformationTableViewModel(this, stationId, RowCount, city);
        }

        public void SetInformationTable(InformationTable informationTable)
        {
            Id = informationTable.Id ?? Guid.NewGuid()
                                            .ToString();
            WidthWithModule = informationTable.WidthWithModule;
            HeightWithModule = informationTable.HeightWithModule;
            RowCount = informationTable.RowCount;            
        }

        public InformationTable()
        {
            Contents = new List<Content>();
        }
    }
}