namespace UKG_Integration_Application.API_UKG.DTOs
{
    public class HoursWorkedPersonIdContainer
    {
        public string Value { get; set; } // PersonId
    }

    public class HoursWorkedIdHourPair
    {
        public List<HoursWorkedPersonIdContainer> Attributes { get; set; } // has personID
        public List<HoursWorkedSum> SummaryListDisplay { get; set; } // has hour totals
    }

    public class HoursWorkedData
    {
        public List<HoursWorkedIdHourPair> Children { get; set; }
    }

    public class HoursWorkedDTO
    {
        public HoursWorkedData Data { get; set; }
    }

    public class HoursWorkedSum
    {
        public string Sum { get; set; }
    }


}
