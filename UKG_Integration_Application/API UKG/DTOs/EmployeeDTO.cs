namespace UKG_Integration_Application.API_UKG
{
    public class EmpKeyValuePair
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class EmpObject
    {
        public List<EmpKeyValuePair> attributes { get; set; }

    }

    public class EmpData
    {
        public List<EmpObject> children { get; set; }
    }

    public class EmpMetadata
    {
        public string totalElements { get; set; }
    }

    public class EmployeeDTO
    {
        public EmpMetadata metadata { get; set; }
        public EmpData data { get; set; }
    }
}
