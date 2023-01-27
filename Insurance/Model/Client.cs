namespace Insurance.Model
{
    public class Client
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Dob { get; set; }
        public string? DateOfIssue { get; set; }
        public string? ExcellFile { get; set; }
        public string? CarFile { get; set; }
        public bool HasPartner { get; set; }

        public Client(string id, string? name, string? dob, string? dateOfIssue, string? excellFile, string? carFile, bool hasPartner)
        {
            Id=id;
            Name=name;
            Dob=dob;
            DateOfIssue=dateOfIssue;
            ExcellFile=excellFile;
            CarFile=carFile;
            HasPartner=hasPartner;
        }
    }
}
