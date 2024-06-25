namespace UKG_Integration_Application.API_UKG
{
   public class HyperfindStaffRef
    {
        public required int id { get; set; }
    }

    public class HyperfindResult
    {
        public List<HyperfindStaffRef> refs { get; set; }
    }

    public class HyperfindDTO
    {
        public int count { get; set; }
        public HyperfindResult result { get; set; }
    }
}
