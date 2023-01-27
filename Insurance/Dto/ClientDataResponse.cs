namespace Insurance.Dto
{
    public class ClientDataResponse
    {
        public Data Data { get; set; }
        public int Status { get; set; }

    }

    public class Data
    {
        public DataOutput Output { get; set; }
        public string TaskStatus { get; set; }
        public string TaskStatusCode { get; set; }

    }

    public class DataOutput
    {
        public List<Result> Result { get; set; }
    }

    public class Result
    {
        public string ErorMessage { get; set; }
        public string ExtractedCarsFile { get; set; }
        public string ExtractedInsuranceFile { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }

    }
}