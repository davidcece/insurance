using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Insurance.Dto
{
    public class ClientDataRequest
    {
        public List<ClientData> ClientsData { get; set; }

        public ClientDataRequest(FormInput input)
        {

            ClientsData = new List<ClientData>();

            bool hasSpouse = !string.IsNullOrEmpty(input.SpouseId) &&
                             !string.IsNullOrEmpty(input.SpouseIssued) &&
                             !string.IsNullOrEmpty(input.SpouseDob);

            ClientData customer = new ClientData(input.Id, input.Issued, input.Dob, input.Cars,input.Name,hasSpouse);
            ClientsData.Add(customer);

            if(hasSpouse)
            {
                ClientData spouse = new ClientData(input.SpouseId, input.SpouseIssued, input.SpouseDob, input.SpouseCars,input.SpouseName,hasSpouse);
                ClientsData.Add(spouse);
            }

        }
    }

    public class ClientData
    {
        [JsonIgnore]
        public string? Name { get; set; }
        [JsonIgnore]
        public bool HasPartner { get; set; }

        public string IdNumber { get; set; }
        public string Type { get; set; }
        public string IssueDate { get; set; }
        public string BirthDate { get; set; }
        public bool DidLeaveCountryLately { get; set; }
        public bool DidIssuePassportLately { get; set; }
        public string[] CarNumbers { get; set; }

        public ClientData(string idNumber, string issueDate, string birthDate, string[] cars, string name, bool hasPartner)
        {
            IdNumber = idNumber;
            Type = "adult";
            IssueDate = issueDate;
            BirthDate = birthDate;
            DidLeaveCountryLately = true;
            DidIssuePassportLately = false;
            CarNumbers = cars;
            Name=name;
            HasPartner=hasPartner;
        }
    }


    public class FormInput
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Issued { get; set; }
        public string? Dob { get; set; }
        public string[]? Cars { get; set; }

        public string? SpouseId { get; set; }
        public string? SpouseName { get; set; }
        public string? SpouseIssued { get; set; }
        public string? SpouseDob { get; set; }
        public string[]? SpouseCars { get; set; }
    }




}
