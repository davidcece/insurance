using Microsoft.Data.SqlClient;

namespace Insurance.Model
{
    public class Database
    {
        private readonly string CS;

        public Database(string cs)
        {
            this.CS = cs;
        }


        public List<Client> GetClients()
        {
            List<Client> clients= new();

            string query = "SELECT * FROM [dbo].[Clients]";
            using SqlConnection con = new(CS);
            using SqlCommand cmd = new(query);
            cmd.Connection = con;
            con.Open();

            using SqlDataReader reader=cmd.ExecuteReader();
            while (reader.Read())
            {
                string id= reader.GetString(0);
                string? name= reader.IsDBNull(1) ? null : reader.GetString(1);
                string? dob= reader.IsDBNull(2) ? null : reader.GetString(2);
                string? dateOfIssue= reader.IsDBNull(3) ? null : reader.GetString(3);
                string? excellFile= reader.IsDBNull(4) ? null: reader.GetString(4);
                string? carFile= reader.IsDBNull(5) ? null: reader.GetString(5);
                bool hasPartner = reader.GetBoolean(6);

                Client client = new(id, name, dob, dateOfIssue, excellFile, carFile, hasPartner);
                clients.Add(client);
            }
            con.Close();

            return clients;
        }

        internal void DeleteClient(string id)
        {
            string query = "DELETE FROM [dbo].[Clients] WHERE Id=@Id";
            using SqlConnection con = new(CS);
            using SqlCommand cmd = new(query);
            cmd.Connection = con;
            con.Open();

            SqlParameter param = new SqlParameter("Id", id);
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        internal void SaveClients(List<Client> clients)
        {
            foreach(Client client in clients) { 
                SaveClient(client);
            }
        }

        internal void SaveClient(Client client)
        {
            string query = @"INSERT INTO [dbo].[Clients]
                               ([Id]
                               ,[Name]
                               ,[Dob]
                               ,[DateOfIssue]
                               ,[ExcelFile]
                               ,[CarFile]
                               ,[HasPartner])
                         VALUES
                               (@Id,@Name,@Dob,@DateOfIssue,@ExcelFile,@CarFile,@HasPartner)";
           
            using SqlConnection con = new(CS);
            using SqlCommand cmd = new(query);
            cmd.Connection = con;
            con.Open();

            cmd.Parameters.AddWithValue("Id",client.Id);
            cmd.Parameters.AddWithValue("Name", client.Name);
            cmd.Parameters.AddWithValue("Dob", client.Dob);
            cmd.Parameters.AddWithValue("DateOfIssue", client.DateOfIssue);
            cmd.Parameters.AddWithValue("ExcelFile", client.ExcellFile);
            cmd.Parameters.AddWithValue("CarFile", client.CarFile);
            cmd.Parameters.AddWithValue("HasPartner", client.HasPartner);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
