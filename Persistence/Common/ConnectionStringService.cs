using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Persistence.Common
{
    public class ConnectionStringService()
    {
        public static string MSSQLConnectionString(IConfiguration config)
        {
            SqlConnectionStringBuilder sqlBuilder = [];
            {
                sqlBuilder.DataSource = config[""];
                sqlBuilder.InitialCatalog = config[""];
                sqlBuilder.UserID = config[""];
                sqlBuilder.Password = MSSQLPassword();
                sqlBuilder.Encrypt = true;
                sqlBuilder.TrustServerCertificate = true;
            };
            return sqlBuilder.ToString();
        }

        // Get MSSQL Password
        public static string MSSQLPassword()
        {
            if (!string.IsNullOrEmpty(Password))
            {
                return Password;
            }
            else
            {
                Console.Write("Enter password: ");
                string? userInputPassword = Console.ReadLine();
                return userInputPassword;
            }
        }
    }
}
