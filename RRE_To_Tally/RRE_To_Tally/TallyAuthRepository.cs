using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RRE_To_Tally;

public sealed class TallyAuthRepository
{
    private const string AccessTableName = "RRETallyExportUserAccess";

    public async Task<UserSession?> LoginAsync(string userName, string password)
    {
        return await Task.Run(delegate
        {
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand("[Getlogindetails]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@UserName", SqlDbType.VarChar, 100).Value = userName.Trim();
                command.Parameters.Add("@Password", SqlDbType.VarChar, 100).Value = password;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new UserSession
                    {
                        UserId = ToInt(reader["UserId"]),
                        UserName = ToStringValue(reader["UserName"]),
                        UserFullName = ToStringValue(reader["UserFullName"]),
                        Role = ToStringValue(reader["URole"])
                    };
                }
            }
        }).ConfigureAwait(false);
    }

    public async Task<bool> HasTallyAccessAsync(UserSession user)
    {
        if (user.IsAdmin) return true;
        return await Task.Run(delegate
        {
            EnsureAccessTable();
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM " + AccessTableName + " WHERE UserId = @UserId AND IsAllowed = 1", connection))
            {
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = user.UserId;
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }).ConfigureAwait(false);
    }

    public async Task<List<UserAccessRow>> LoadUsersForAccessAsync()
    {
        return await Task.Run(delegate
        {
            EnsureAccessTable();
            List<UserAccessRow> users = new List<UserAccessRow>();
            string sql = @"
SELECT
    U.UserId,
    U.UserName,
    U.UserFullName,
    U.URole,
    CAST(ISNULL(A.IsAllowed, 0) AS bit) AS HasAccess
FROM Users U
LEFT JOIN RRETallyExportUserAccess A ON A.UserId = U.UserId
WHERE ISNULL(U.IsDeleted, 0) = 0
ORDER BY U.UserName;";
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserAccessRow
                        {
                            UserId = ToInt(reader["UserId"]),
                            UserName = ToStringValue(reader["UserName"]),
                            UserFullName = ToStringValue(reader["UserFullName"]),
                            Role = ToStringValue(reader["URole"]),
                            HasAccess = reader["HasAccess"] != DBNull.Value && Convert.ToBoolean(reader["HasAccess"])
                        });
                    }
                }
            }

            return users;
        }).ConfigureAwait(false);
    }

    public async Task SaveUserAccessAsync(IList<UserAccessRow> users, UserSession adminUser)
    {
        await Task.Run(delegate
        {
            EnsureAccessTable();
            using (SqlConnection connection = CreateConnection())
            {
                connection.Open();
                foreach (UserAccessRow user in users)
                {
                    using (SqlCommand command = new SqlCommand(@"
MERGE RRETallyExportUserAccess AS target
USING (SELECT @UserId AS UserId) AS source
ON target.UserId = source.UserId
WHEN MATCHED THEN UPDATE SET IsAllowed = @IsAllowed, UpdatedOn = GETDATE(), UpdatedBy = @UpdatedBy
WHEN NOT MATCHED THEN INSERT (UserId, IsAllowed, UpdatedOn, UpdatedBy) VALUES (@UserId, @IsAllowed, GETDATE(), @UpdatedBy);", connection))
                    {
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = user.UserId;
                        command.Parameters.Add("@IsAllowed", SqlDbType.Bit).Value = user.HasAccess;
                        command.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = adminUser.UserId;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }).ConfigureAwait(false);
    }

    private void EnsureAccessTable()
    {
        using (SqlConnection connection = CreateConnection())
        using (SqlCommand command = new SqlCommand(@"
IF OBJECT_ID('dbo.RRETallyExportUserAccess', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RRETallyExportUserAccess
    (
        UserId int NOT NULL PRIMARY KEY,
        IsAllowed bit NOT NULL CONSTRAINT DF_RRETallyExportUserAccess_IsAllowed DEFAULT(0),
        UpdatedOn datetime NOT NULL CONSTRAINT DF_RRETallyExportUserAccess_UpdatedOn DEFAULT(GETDATE()),
        UpdatedBy int NULL
    );
END", connection))
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    private static SqlConnection CreateConnection()
    {
        ConnectionStringSettings? setting = ConfigurationManager.ConnectionStrings["con"];
        if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
        {
            throw new ConfigurationErrorsException("Connection string 'con' was not found.");
        }

        return new SqlConnection(setting.ConnectionString);
    }

    private static int ToInt(object value)
    {
        if (value == DBNull.Value) return 0;
        return Convert.ToInt32(value);
    }

    private static string ToStringValue(object value)
    {
        return value == DBNull.Value ? "" : Convert.ToString(value) ?? "";
    }
}
