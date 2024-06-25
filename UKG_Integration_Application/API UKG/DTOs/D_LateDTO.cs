namespace UKG_Integration_Application.API_UKG
{
    public class PunchKeyValuePair
    {
        public string alias { get; set; }
        public string rawValue { get; set; }
        public string value { get; set; }
    }

    public class PunchObject
    {
        public Key key { get; set; }
        public CoreEntityKey coreEntityKey { get; set; }
        public List<PunchKeyValuePair> attributes { get; set; }
    }

    public class CoreEntityKey
    {
        public EMP EMP { get; set; }
    }

    public class EMP
    {
        public string id { get; set; }
    }

    public class PunchData
    {
        public List<PunchObject> children { get; set; }
    }

    public class Key
    {
        public string TKTIMECARD_TRANSACTION { get; set; }
    }

    public class PunchMetadata
    {
        public string totalElements { get; set; }
    }

    public class D_LateDTO
    {
        public PunchMetadata metadata { get; set; }
        public PunchData data { get; set; }
    }
}
