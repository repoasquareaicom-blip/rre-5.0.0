using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting.WindowsServices;

const string ReportingCorsPolicy = "ReportingWebCors";

string? contentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : null;

WebApplicationOptions webApplicationOptions = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRootPath
};

var builder = WebApplication.CreateBuilder(webApplicationOptions);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "RRE Branch API";
});

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    string[] localReportingOrigins =
    {
        "http://localhost:5173",
        "http://127.0.0.1:5173"
    };

    HashSet<string> allowedOrigins = new HashSet<string>(localReportingOrigins, StringComparer.OrdinalIgnoreCase);
    string[] configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    foreach (string origin in configuredOrigins)
    {
        if (!string.IsNullOrWhiteSpace(origin))
        {
            allowedOrigins.Add(origin.Trim());
        }
    }

    options.AddPolicy(ReportingCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins.ToArray())
            .WithMethods("GET", "POST", "OPTIONS")
            .WithHeaders("Content-Type", "X-Api-Key");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(ReportingCorsPolicy);

app.MapGet("/api/stock/available", async (int productId, string itemName, IConfiguration configuration) =>
{
    string branchCode = GetBranchCode(configuration);
    string connectionString = GetDefaultConnectionString(configuration);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Json(new { message = "Branch database connection is not configured." }, statusCode: StatusCodes.Status500InternalServerError);
    }

    AvailableStockResponse? result = await GetAvailableStockAsync(connectionString, branchCode, productId, itemName);
    if (result == null)
    {
        return Results.Json(new { message = "No response received from database." }, statusCode: StatusCodes.Status500InternalServerError);
    }

    return Results.Ok(result);
});

app.MapPost("/api/getdata", async (HttpRequest request, IConfiguration configuration) =>
{
    string branchCode = GetBranchCode(configuration);
    string connectionString = GetDefaultConnectionString(configuration);

    if (!ValidateConfiguredHeaderKey(request, configuration, "ApiKey", "X-Api-Key"))
    {
        return Results.Json(new GenericApiResponse(false, branchCode, "Unauthorized."), statusCode: StatusCodes.Status401Unauthorized);
    }

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Json(new GenericApiResponse(false, branchCode, "Branch database connection is not configured."), statusCode: StatusCodes.Status500InternalServerError);
    }

    ReportDataRequest? dataRequest;
    try
    {
        dataRequest = await JsonSerializer.DeserializeAsync<ReportDataRequest>(
            request.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch
    {
        return Results.Json(new GenericApiResponse(false, branchCode, "Invalid JSON payload."), statusCode: StatusCodes.Status400BadRequest);
    }

    if (dataRequest == null || string.IsNullOrWhiteSpace(dataRequest.QueryText))
    {
        return Results.Json(new GenericApiResponse(false, branchCode, "queryText is required."), statusCode: StatusCodes.Status400BadRequest);
    }

    if (!IsSafeStoredProcedureName(dataRequest.QueryText))
    {
        return Results.Json(new GenericApiResponse(false, branchCode, "Invalid stored procedure name."), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        await EnsureProductSyncReadProceduresAsync(connectionString, dataRequest.QueryText.Trim());

        List<Dictionary<string, object?>> rows = await ExecuteStoredProcedureAsync(
            connectionString,
            dataRequest.QueryText.Trim(),
            dataRequest.Parameters);

        return Results.Json(new ReportDataResponse(true, branchCode, rows));
    }
    catch (SqlException ex)
    {
        return Results.Json(new GenericApiResponse(false, branchCode, SanitizeErrorMessage(ex)), statusCode: StatusCodes.Status500InternalServerError);
    }
    catch (Exception ex)
    {
        return Results.Json(new GenericApiResponse(false, branchCode, SanitizeErrorMessage(ex)), statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapPost("/api/admin/deployscript", async (HttpRequest request, IConfiguration configuration) =>
{
    string branchCode = GetBranchCode(configuration);
    string connectionString = GetDefaultConnectionString(configuration);

    if (!ValidateConfiguredHeaderKey(request, configuration, "DeploymentApiKey", "X-Deployment-Key"))
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, null, "Unauthorized."), statusCode: StatusCodes.Status401Unauthorized);
    }

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, null, "Branch database connection is not configured."), statusCode: StatusCodes.Status500InternalServerError);
    }

    DeploymentScriptRequest? deployRequest;
    try
    {
        deployRequest = await JsonSerializer.DeserializeAsync<DeploymentScriptRequest>(
            request.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, null, "Invalid JSON payload."), statusCode: StatusCodes.Status400BadRequest);
    }

    if (deployRequest == null || string.IsNullOrWhiteSpace(deployRequest.DeploymentId) || string.IsNullOrWhiteSpace(deployRequest.Script))
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, deployRequest?.DeploymentId, "deploymentId and script are required."), statusCode: StatusCodes.Status400BadRequest);
    }

    if (deployRequest.DeploymentId.Length > 100)
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, deployRequest.DeploymentId, "deploymentId must be 100 characters or less."), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        DeploymentExecutionResult result = await ExecuteDeploymentScriptAsync(
            connectionString,
            branchCode,
            deployRequest.DeploymentId.Trim(),
            deployRequest.Script);

        return Results.Json(new DeploymentScriptResponse(true, branchCode, deployRequest.DeploymentId.Trim(), result.Message));
    }
    catch (DeploymentConflictException ex)
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, deployRequest.DeploymentId.Trim(), ex.Message), statusCode: StatusCodes.Status409Conflict);
    }
    catch (SqlException ex)
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, deployRequest.DeploymentId.Trim(), SanitizeErrorMessage(ex)), statusCode: StatusCodes.Status500InternalServerError);
    }
    catch (Exception ex)
    {
        return Results.Json(new DeploymentScriptResponse(false, branchCode, deployRequest.DeploymentId.Trim(), SanitizeErrorMessage(ex)), statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapPost("/api/productmaster/upsert", async (HttpRequest request, IConfiguration configuration) =>
{
    string branchCode = configuration["BranchCode"] ?? string.Empty;
    string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? configuration["ConnectionString"]
        ?? string.Empty;

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Json(new ProductUpsertResponse(false, branchCode, null, "Branch database connection is not configured."), statusCode: StatusCodes.Status500InternalServerError);
    }

    ProductSyncRequest? syncRequest;
    try
    {
        syncRequest = await JsonSerializer.DeserializeAsync<ProductSyncRequest>(
            request.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch (Exception ex)
    {
        return Results.Json(new ProductUpsertResponse(false, branchCode, null, "Invalid JSON payload: " + ex.Message), statusCode: StatusCodes.Status400BadRequest);
    }

    if (syncRequest?.Records == null || syncRequest.Records.Count == 0)
    {
        return Results.Json(new ProductUpsertResponse(false, branchCode, null, "No ProductMaster records supplied."), statusCode: StatusCodes.Status400BadRequest);
    }

    int applied = 0;
    object? lastProductId = null;

    try
    {
        foreach (Dictionary<string, object?> record in syncRequest.Records)
        {
            object? productId = UpsertProductMaster(connectionString, record);
            lastProductId = productId ?? lastProductId;
            applied++;
        }

        return Results.Json(new ProductUpsertResponse(true, branchCode, lastProductId, applied + " ProductMaster record(s) applied."));
    }
    catch (Exception ex)
    {
        if (ex is ProductValidationException validationException)
        {
            return Results.Json(new ProductUpsertResponse(false, branchCode, validationException.ProductId, validationException.Message), statusCode: StatusCodes.Status409Conflict);
        }

        return Results.Json(new ProductUpsertResponse(false, branchCode, lastProductId, "Database update failed: " + ex.Message), statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapPost("/api/productmaster/queue/ack-synced", async (HttpRequest request, IConfiguration configuration) =>
{
    string branchCode = GetBranchCode(configuration);
    string connectionString = GetDefaultConnectionString(configuration);

    if (!ValidateConfiguredHeaderKey(request, configuration, "ApiKey", "X-Api-Key"))
    {
        return Results.Json(new GenericApiResponse(false, branchCode, "Unauthorized."), statusCode: StatusCodes.Status401Unauthorized);
    }

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Json(new ProductQueueAckResponse(false, branchCode, null, null, null, "Branch database connection is not configured."), statusCode: StatusCodes.Status500InternalServerError);
    }

    ProductQueueAckRequest? ackRequest;
    try
    {
        ackRequest = await JsonSerializer.DeserializeAsync<ProductQueueAckRequest>(
            request.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch
    {
        return Results.Json(new ProductQueueAckResponse(false, branchCode, null, null, null, "Invalid JSON payload."), statusCode: StatusCodes.Status400BadRequest);
    }

    if (ackRequest == null || ackRequest.QueueId <= 0 || string.IsNullOrWhiteSpace(ackRequest.ProductId) || string.IsNullOrWhiteSpace(ackRequest.TargetBranchCode))
    {
        return Results.Json(new ProductQueueAckResponse(false, branchCode, ackRequest?.QueueId, ackRequest?.ProductId, ackRequest?.TargetBranchCode, "queueId, productId, and targetBranchCode are required."), statusCode: StatusCodes.Status400BadRequest);
    }

    try
    {
        ProductQueueAckResult result = await AcknowledgeProductQueueSyncedAsync(
            connectionString,
            ackRequest.QueueId,
            ackRequest.ProductId.Trim(),
            ackRequest.TargetBranchCode.Trim());

        if (!result.Success)
        {
            return Results.Json(new ProductQueueAckResponse(false, branchCode, ackRequest.QueueId, ackRequest.ProductId.Trim(), ackRequest.TargetBranchCode.Trim(), result.Message), statusCode: result.StatusCode);
        }

        return Results.Json(new ProductQueueAckResponse(true, branchCode, ackRequest.QueueId, ackRequest.ProductId.Trim(), ackRequest.TargetBranchCode.Trim(), result.Message));
    }
    catch (SqlException ex)
    {
        return Results.Json(new ProductQueueAckResponse(false, branchCode, ackRequest.QueueId, ackRequest.ProductId.Trim(), ackRequest.TargetBranchCode.Trim(), SanitizeErrorMessage(ex)), statusCode: StatusCodes.Status500InternalServerError);
    }
    catch (Exception ex)
    {
        return Results.Json(new ProductQueueAckResponse(false, branchCode, ackRequest.QueueId, ackRequest.ProductId.Trim(), ackRequest.TargetBranchCode.Trim(), SanitizeErrorMessage(ex)), statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.Run();

static async Task EnsureProductSyncReadProceduresAsync(string connectionString, string procedureName)
{
    string normalized = procedureName.Trim();
    if (!string.Equals(normalized, "dbo.sp_product_sync_pending_by_branch", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(normalized, "sp_product_sync_pending_by_branch", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(normalized, "dbo.sp_product_sync_full_product", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(normalized, "sp_product_sync_full_product", StringComparison.OrdinalIgnoreCase))
    {
        return;
    }

    await using SqlConnection conn = new SqlConnection(connectionString);
    await conn.OpenAsync();

    await using (SqlCommand cmd = conn.CreateCommand())
    {
        cmd.CommandText = @"
CREATE OR ALTER PROCEDURE dbo.sp_product_sync_pending_by_branch
    @TargetBranchCode varchar(30)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @mrpExpression nvarchar(200) = CASE WHEN COL_LENGTH('dbo.ProductMaster', 'MRP') IS NULL THEN 'CAST(NULL AS decimal(18,2))' ELSE 'p.[MRP]' END;
    DECLARE @gstExpression nvarchar(200) =
        CASE
            WHEN COL_LENGTH('dbo.ProductMaster', 'GST') IS NOT NULL THEN 'p.[GST]'
            WHEN COL_LENGTH('dbo.ProductMaster', 'Tax') IS NOT NULL THEN 'p.[Tax]'
            WHEN COL_LENGTH('dbo.ProductMaster', 'SGST') IS NOT NULL THEN 'p.[SGST]'
            ELSE 'CAST(NULL AS varchar(50))'
        END;

    DECLARE @sql nvarchar(max) = N'
SELECT q.QueueId,
       q.ProductId,
       COALESCE(NULLIF(CONVERT(varchar(255), p.DisplayName), ''''), CONVERT(varchar(255), p.ItemName), q.ItemName) AS DisplayName,
       p.SalesPrice,
       ' + @mrpExpression + N' AS MRP,
       ' + @gstExpression + N' AS GST,
       q.Status,
       q.ChangeType,
       q.AttemptCount,
       q.LastError,
       q.LastTriedOn,
       q.TargetBranchCode
FROM dbo.ProductMasterCloudQueue q
INNER JOIN dbo.ProductMaster p ON CONVERT(varchar(50), p.id) = q.ProductId
WHERE q.TargetBranchCode = @TargetBranchCode
  AND q.Status IN (''Pending'', ''Failed'')
ORDER BY q.ModifiedOn DESC, q.QueueId DESC';

    EXEC sp_executesql @sql, N'@TargetBranchCode varchar(30)', @TargetBranchCode;
END;";
        await cmd.ExecuteNonQueryAsync();
    }

    await using (SqlCommand cmd = conn.CreateCommand())
    {
        cmd.CommandText = @"
CREATE OR ALTER PROCEDURE dbo.sp_product_sync_full_product
    @ProductId varchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.ProductMaster
    WHERE CONVERT(varchar(50), id) = @ProductId;
END;";
        await cmd.ExecuteNonQueryAsync();
    }
}

static async Task<List<Dictionary<string, object?>>> ExecuteStoredProcedureAsync(
    string connectionString,
    string procedureName,
    Dictionary<string, JsonElement>? parameters)
{
    List<Dictionary<string, object?>> rows = new List<Dictionary<string, object?>>();

    await using SqlConnection conn = new SqlConnection(connectionString);
    await conn.OpenAsync();

    await using SqlCommand cmd = conn.CreateCommand();
    cmd.CommandText = procedureName;
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.CommandTimeout = 120;

    if (parameters != null)
    {
        foreach (KeyValuePair<string, JsonElement> item in parameters)
        {
            string parameterName = NormalizeParameterName(item.Key);
            if (!IsSafeParameterName(parameterName))
            {
                throw new InvalidOperationException("Invalid parameter name.");
            }

            cmd.Parameters.AddWithValue("@" + parameterName, NormalizeJsonValue(item.Value) ?? DBNull.Value);
        }
    }

    await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        Dictionary<string, object?> row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < reader.FieldCount; i++)
        {
            string columnName = GetUniqueColumnName(row, reader.GetName(i), i);
            row[columnName] = await reader.IsDBNullAsync(i) ? null : NormalizeSqlValue(reader.GetValue(i));
        }

        rows.Add(row);
    }

    return rows;
}

static async Task<AvailableStockResponse?> GetAvailableStockAsync(
    string connectionString,
    string branchCode,
    int productId,
    string itemName)
{
    await using SqlConnection connection = new SqlConnection(connectionString);
    await using SqlCommand command = new SqlCommand("GetAvailableStockByProductId", connection);

    command.CommandType = CommandType.StoredProcedure;
    command.Parameters.AddWithValue("@ProductId", productId);
    command.Parameters.AddWithValue("@ItemName", itemName);

    await connection.OpenAsync();

    await using SqlDataReader reader = await command.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        return new AvailableStockResponse(
            branchCode,
            reader["ReturnCode"]?.ToString(),
            reader["Message"]?.ToString(),
            reader["ProductId"] == DBNull.Value ? null : Convert.ToInt32(reader["ProductId"]),
            reader["AvailableStock"] == DBNull.Value ? null : Convert.ToDecimal(reader["AvailableStock"]));
    }

    return null;
}

static async Task<ProductQueueAckResult> AcknowledgeProductQueueSyncedAsync(
    string connectionString,
    int queueId,
    string productId,
    string targetBranchCode)
{
    await using SqlConnection conn = new SqlConnection(connectionString);
    await conn.OpenAsync();

    await using SqlTransaction transaction = (SqlTransaction)await conn.BeginTransactionAsync();
    try
    {
        await using (SqlCommand select = conn.CreateCommand())
        {
            select.Transaction = transaction;
            select.CommandText = @"
SELECT ProductId, TargetBranchCode, Status
FROM dbo.ProductMasterCloudQueue WITH (UPDLOCK, HOLDLOCK)
WHERE QueueId = @QueueId";
            select.Parameters.Add("@QueueId", SqlDbType.Int).Value = queueId;

            await using SqlDataReader reader = await select.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await transaction.RollbackAsync();
                return new ProductQueueAckResult(false, StatusCodes.Status404NotFound, "QueueId was not found.");
            }

            string existingProductId = Convert.ToString(reader["ProductId"])?.Trim() ?? string.Empty;
            string existingTargetBranchCode = Convert.ToString(reader["TargetBranchCode"])?.Trim() ?? string.Empty;
            string existingStatus = Convert.ToString(reader["Status"])?.Trim() ?? string.Empty;

            if (!string.Equals(existingProductId, productId, StringComparison.OrdinalIgnoreCase))
            {
                await transaction.RollbackAsync();
                return new ProductQueueAckResult(false, StatusCodes.Status409Conflict, "ProductId does not match the queue row.");
            }

            if (!string.Equals(existingTargetBranchCode, targetBranchCode, StringComparison.Ordinal))
            {
                await transaction.RollbackAsync();
                return new ProductQueueAckResult(false, StatusCodes.Status409Conflict, "TargetBranchCode does not match the queue row.");
            }

            if (string.Equals(existingStatus, "Synced", StringComparison.OrdinalIgnoreCase))
            {
                await transaction.CommitAsync();
                return new ProductQueueAckResult(true, StatusCodes.Status200OK, "Queue row was already synced.");
            }
        }

        await using (SqlCommand update = conn.CreateCommand())
        {
            update.Transaction = transaction;
            update.CommandText = @"
UPDATE dbo.ProductMasterCloudQueue
SET Status = 'Synced',
    SyncedOn = GETDATE(),
    LastTriedOn = GETDATE(),
    LastError = NULL
WHERE QueueId = @QueueId";
            update.Parameters.Add("@QueueId", SqlDbType.Int).Value = queueId;
            await update.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
        return new ProductQueueAckResult(true, StatusCodes.Status200OK, "Queue row marked as synced.");
    }
    catch
    {
        try
        {
            await transaction.RollbackAsync();
        }
        catch
        {
        }

        throw;
    }
}

static async Task<DeploymentExecutionResult> ExecuteDeploymentScriptAsync(
    string connectionString,
    string branchCode,
    string deploymentId,
    string script)
{
    string scriptHash = ComputeSha256Hash(script);
    List<string> batches = SplitSqlBatches(script);

    await using SqlConnection conn = new SqlConnection(connectionString);
    await conn.OpenAsync();

    await EnsureDeploymentHistoryTableAsync(conn);

    DeploymentHistoryEntry? existing = await GetDeploymentHistoryAsync(conn, deploymentId);
    if (existing != null)
    {
        if (!string.Equals(existing.ScriptHash, scriptHash, StringComparison.OrdinalIgnoreCase))
        {
            throw new DeploymentConflictException("DeploymentId already exists with a different script hash.");
        }

        if (string.Equals(existing.Status, "Succeeded", StringComparison.OrdinalIgnoreCase))
        {
            return new DeploymentExecutionResult("Deployment already applied with the same script hash.");
        }
    }

    await using SqlTransaction transaction = (SqlTransaction)await conn.BeginTransactionAsync();
    try
    {
        await UpsertDeploymentHistoryAsync(conn, transaction, deploymentId, branchCode, scriptHash, "Running", null);

        foreach (string batch in batches)
        {
            string batchText = batch.Trim();
            if (string.IsNullOrWhiteSpace(batchText))
            {
                continue;
            }

            await using SqlCommand cmd = new SqlCommand(batchText, conn, transaction);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 300;
            await cmd.ExecuteNonQueryAsync();
        }

        await UpsertDeploymentHistoryAsync(conn, transaction, deploymentId, branchCode, scriptHash, "Succeeded", null);
        await transaction.CommitAsync();
        return new DeploymentExecutionResult("Database script executed successfully.");
    }
    catch (Exception ex)
    {
        string message = SanitizeErrorMessage(ex);
        try
        {
            await transaction.RollbackAsync();
        }
        catch
        {
        }

        await UpsertDeploymentHistoryAfterFailureAsync(conn, deploymentId, branchCode, scriptHash, message);
        throw;
    }
}

static async Task EnsureDeploymentHistoryTableAsync(SqlConnection conn)
{
    string commandText = @"
IF OBJECT_ID('dbo.BranchDatabaseDeploymentHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BranchDatabaseDeploymentHistory
    (
        DeploymentId varchar(100) NOT NULL PRIMARY KEY,
        BranchCode varchar(30) NOT NULL,
        ScriptHash varchar(128) NOT NULL,
        ExecutedOn datetime NOT NULL,
        Status varchar(20) NOT NULL,
        ErrorMessage varchar(2000) NULL
    );
END";

    await using SqlCommand cmd = new SqlCommand(commandText, conn);
    cmd.CommandType = CommandType.Text;
    cmd.CommandTimeout = 60;
    await cmd.ExecuteNonQueryAsync();
}

static async Task<DeploymentHistoryEntry?> GetDeploymentHistoryAsync(SqlConnection conn, string deploymentId)
{
    string commandText = @"
SELECT TOP 1 ScriptHash, Status
FROM dbo.BranchDatabaseDeploymentHistory
WHERE DeploymentId = @DeploymentId";

    await using SqlCommand cmd = new SqlCommand(commandText, conn);
    cmd.CommandType = CommandType.Text;
    cmd.CommandTimeout = 60;
    cmd.Parameters.Add("@DeploymentId", SqlDbType.VarChar, 100).Value = deploymentId;

    await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
    if (!await reader.ReadAsync())
    {
        return null;
    }

    return new DeploymentHistoryEntry(reader.GetString(0), reader.GetString(1));
}

static async Task UpsertDeploymentHistoryAsync(
    SqlConnection conn,
    SqlTransaction transaction,
    string deploymentId,
    string branchCode,
    string scriptHash,
    string status,
    string? errorMessage)
{
    string commandText = @"
UPDATE dbo.BranchDatabaseDeploymentHistory
SET BranchCode = @BranchCode,
    ScriptHash = @ScriptHash,
    ExecutedOn = GETDATE(),
    Status = @Status,
    ErrorMessage = @ErrorMessage
WHERE DeploymentId = @DeploymentId;

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.BranchDatabaseDeploymentHistory
        (DeploymentId, BranchCode, ScriptHash, ExecutedOn, Status, ErrorMessage)
    VALUES
        (@DeploymentId, @BranchCode, @ScriptHash, GETDATE(), @Status, @ErrorMessage);
END";

    await using SqlCommand cmd = new SqlCommand(commandText, conn, transaction);
    cmd.CommandType = CommandType.Text;
    cmd.CommandTimeout = 60;
    AddDeploymentHistoryParameters(cmd, deploymentId, branchCode, scriptHash, status, errorMessage);
    await cmd.ExecuteNonQueryAsync();
}

static async Task UpsertDeploymentHistoryAfterFailureAsync(
    SqlConnection conn,
    string deploymentId,
    string branchCode,
    string scriptHash,
    string errorMessage)
{
    string commandText = @"
UPDATE dbo.BranchDatabaseDeploymentHistory
SET BranchCode = @BranchCode,
    ScriptHash = @ScriptHash,
    ExecutedOn = GETDATE(),
    Status = @Status,
    ErrorMessage = @ErrorMessage
WHERE DeploymentId = @DeploymentId;

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.BranchDatabaseDeploymentHistory
        (DeploymentId, BranchCode, ScriptHash, ExecutedOn, Status, ErrorMessage)
    VALUES
        (@DeploymentId, @BranchCode, @ScriptHash, GETDATE(), @Status, @ErrorMessage);
END";

    await using SqlCommand cmd = new SqlCommand(commandText, conn);
    cmd.CommandType = CommandType.Text;
    cmd.CommandTimeout = 60;
    AddDeploymentHistoryParameters(cmd, deploymentId, branchCode, scriptHash, "Failed", Truncate(errorMessage, 2000));
    await cmd.ExecuteNonQueryAsync();
}

static void AddDeploymentHistoryParameters(
    SqlCommand cmd,
    string deploymentId,
    string branchCode,
    string scriptHash,
    string status,
    string? errorMessage)
{
    cmd.Parameters.Add("@DeploymentId", SqlDbType.VarChar, 100).Value = deploymentId;
    cmd.Parameters.Add("@BranchCode", SqlDbType.VarChar, 30).Value = Truncate(branchCode, 30);
    cmd.Parameters.Add("@ScriptHash", SqlDbType.VarChar, 128).Value = scriptHash;
    cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value = status;
    cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 2000).Value = string.IsNullOrEmpty(errorMessage) ? DBNull.Value : Truncate(errorMessage, 2000);
}

static List<string> SplitSqlBatches(string script)
{
    List<string> batches = new List<string>();
    StringBuilder current = new StringBuilder();
    bool inString = false;
    bool inBlockComment = false;

    string normalized = script.Replace("\r\n", "\n").Replace('\r', '\n');
    string[] lines = normalized.Split('\n');
    foreach (string line in lines)
    {
        bool lineStartsOutsideString = !inString && !inBlockComment;
        bool isGoSeparator = lineStartsOutsideString && IsGoBatchLine(line);
        if (isGoSeparator)
        {
            batches.Add(current.ToString());
            current.Clear();
            continue;
        }

        current.AppendLine(line);
        UpdateSqlParserState(line, ref inString, ref inBlockComment);
    }

    if (current.Length > 0)
    {
        batches.Add(current.ToString());
    }

    return batches;
}

static bool IsGoBatchLine(string line)
{
    string trimmed = line.Trim();
    if (trimmed.Length == 0)
    {
        return false;
    }

    int commentIndex = trimmed.IndexOf("--", StringComparison.Ordinal);
    if (commentIndex >= 0)
    {
        trimmed = trimmed.Substring(0, commentIndex).Trim();
    }

    return string.Equals(trimmed, "GO", StringComparison.OrdinalIgnoreCase);
}

static void UpdateSqlParserState(string line, ref bool inString, ref bool inBlockComment)
{
    for (int i = 0; i < line.Length; i++)
    {
        char current = line[i];
        char next = i + 1 < line.Length ? line[i + 1] : '\0';

        if (inBlockComment)
        {
            if (current == '*' && next == '/')
            {
                inBlockComment = false;
                i++;
            }

            continue;
        }

        if (inString)
        {
            if (current == '\'' && next == '\'')
            {
                i++;
                continue;
            }

            if (current == '\'')
            {
                inString = false;
            }

            continue;
        }

        if (current == '-' && next == '-')
        {
            return;
        }

        if (current == '/' && next == '*')
        {
            inBlockComment = true;
            i++;
            continue;
        }

        if (current == '\'')
        {
            inString = true;
        }
    }
}

static bool ValidateConfiguredHeaderKey(HttpRequest request, IConfiguration configuration, string configKey, string headerName)
{
    string configuredKey = configuration[configKey] ?? string.Empty;
    if (string.IsNullOrWhiteSpace(configuredKey))
    {
        return false;
    }

    string suppliedKey = request.Headers[headerName].FirstOrDefault() ?? string.Empty;
    if (string.IsNullOrEmpty(suppliedKey))
    {
        return false;
    }

    byte[] configuredBytes = Encoding.UTF8.GetBytes(configuredKey);
    byte[] suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey);
    return configuredBytes.Length == suppliedBytes.Length && CryptographicOperations.FixedTimeEquals(configuredBytes, suppliedBytes);
}

static bool IsSafeStoredProcedureName(string procedureName)
{
    string value = procedureName.Trim();
    if (!Regex.IsMatch(value, @"^(?:dbo\.)?[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant))
    {
        return false;
    }

    string unqualified = value.StartsWith("dbo.", StringComparison.OrdinalIgnoreCase) ? value.Substring(4) : value;
    string[] blockedNames =
    {
        "select", "insert", "update", "delete", "drop", "alter", "create", "exec", "execute", "merge", "truncate", "union"
    };

    return !blockedNames.Contains(unqualified, StringComparer.OrdinalIgnoreCase);
}

static bool IsSafeParameterName(string parameterName)
{
    return Regex.IsMatch(parameterName, @"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant);
}

static string NormalizeParameterName(string parameterName)
{
    return parameterName.Trim().TrimStart('@');
}

static string GetUniqueColumnName(Dictionary<string, object?> row, string columnName, int ordinal)
{
    string baseName = string.IsNullOrWhiteSpace(columnName) ? "Column" + (ordinal + 1).ToString() : columnName;
    string candidate = baseName;
    int suffix = 2;
    while (row.ContainsKey(candidate))
    {
        candidate = baseName + "_" + suffix.ToString();
        suffix++;
    }

    return candidate;
}

static object? NormalizeSqlValue(object value)
{
    if (value == DBNull.Value)
    {
        return null;
    }

    if (value is DateTime dateTime)
    {
        return dateTime;
    }

    if (value is DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset;
    }

    if (value is byte or short or int or long or decimal or double or float or bool or string)
    {
        return value;
    }

    return value.ToString();
}

static string GetBranchCode(IConfiguration configuration)
{
    return configuration["BranchCode"] ?? configuration["Branch:Code"] ?? string.Empty;
}

static string GetDefaultConnectionString(IConfiguration configuration)
{
    return configuration.GetConnectionString("DefaultConnection")
        ?? configuration["ConnectionString"]
        ?? string.Empty;
}

static string ComputeSha256Hash(string value)
{
    byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
    return Convert.ToHexString(hashBytes);
}

static string SanitizeErrorMessage(Exception ex)
{
    string message = ex.Message;
    message = Regex.Replace(message, @"(?i)(password|pwd)\s*=\s*[^;]+", "$1=***");
    message = Regex.Replace(message, @"(?i)(server|data source|initial catalog|database|user id|uid)\s*=\s*[^;]+", "$1=***");
    return Truncate(message, 2000);
}

static string Truncate(string value, int maxLength)
{
    return value.Length <= maxLength ? value : value.Substring(0, maxLength);
}

static object? UpsertProductMaster(string connectionString, Dictionary<string, object?> payload)
{
    if (!payload.ContainsKey("id") || payload["id"] == null)
    {
        throw new InvalidOperationException("ProductMaster id is missing.");
    }

    using SqlConnection conn = new SqlConnection(connectionString);
    conn.Open();

    List<string> columns = GetTableColumns(conn, "ProductMaster");
    if (columns.Count == 0)
    {
        throw new InvalidOperationException("ProductMaster table was not found.");
    }

    Dictionary<string, string> columnByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    foreach (string column in columns)
    {
        columnByName[column] = column;
    }

    Dictionary<string, object?> data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    foreach (KeyValuePair<string, object?> item in payload)
    {
        if (columnByName.TryGetValue(item.Key, out string? actualColumn))
        {
            data[actualColumn] = NormalizeJsonValue(item.Value);
        }
    }

    if (!data.ContainsKey("id"))
    {
        throw new InvalidOperationException("ProductMaster id is not a valid target column.");
    }

    using SqlTransaction transaction = conn.BeginTransaction();
    try
    {
        ValidateProductMasterRecord(conn, transaction, data);

        int updated = UpdateProductMaster(conn, transaction, data);
        if (updated == 0)
        {
            InsertProductMaster(conn, transaction, data);
        }

        transaction.Commit();
    }
    catch
    {
        try
        {
            transaction.Rollback();
        }
        catch
        {
        }

        throw;
    }

    return data["id"];
}

static List<string> GetTableColumns(SqlConnection conn, string tableName)
{
    List<string> columns = new List<string>();
    using SqlCommand cmd = conn.CreateCommand();
    cmd.CommandText = @"
SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @table
ORDER BY ORDINAL_POSITION";
    cmd.Parameters.Add("@table", SqlDbType.VarChar, 128).Value = tableName;

    using SqlDataReader reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        columns.Add(reader.GetString(0));
    }

    return columns;
}

static void ValidateProductMasterRecord(SqlConnection conn, SqlTransaction transaction, Dictionary<string, object?> data)
{
    object? productId = data["id"];
    string incomingItemName = NormalizeBusinessString(GetValue(data, "ItemName"));
    if (string.IsNullOrEmpty(incomingItemName))
    {
        throw new ProductValidationException(productId, "Product validation failed. ItemName is missing.");
    }

    using (SqlCommand cmd = conn.CreateCommand())
    {
        cmd.Transaction = transaction;
        cmd.CommandText = "SELECT ItemName FROM ProductMaster WITH (UPDLOCK, HOLDLOCK) WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", productId ?? DBNull.Value);

        object? existingItemNameValue = cmd.ExecuteScalar();
        if (existingItemNameValue != null && existingItemNameValue != DBNull.Value)
        {
            return;
        }
    }

    using (SqlCommand cmd = conn.CreateCommand())
    {
        cmd.Transaction = transaction;
        cmd.CommandText = @"
SELECT TOP 1 id
FROM ProductMaster WITH (UPDLOCK, HOLDLOCK)
WHERE UPPER(LTRIM(RTRIM(CONVERT(nvarchar(255), ItemName)))) = @ItemName
  AND id <> @id";
        cmd.Parameters.AddWithValue("@ItemName", incomingItemName.ToUpperInvariant());
        cmd.Parameters.AddWithValue("@id", productId ?? DBNull.Value);

        object? conflictingId = cmd.ExecuteScalar();
        if (conflictingId != null && conflictingId != DBNull.Value)
        {
            throw new ProductValidationException(productId, "Product validation failed. ItemName already exists with a different ProductId.");
        }
    }
}

static object? GetValue(Dictionary<string, object?> data, string key)
{
    return data.TryGetValue(key, out object? value) ? value : null;
}

static string NormalizeBusinessString(object? value)
{
    return Convert.ToString(value)?.Trim() ?? string.Empty;
}

static int UpdateProductMaster(SqlConnection conn, SqlTransaction transaction, Dictionary<string, object?> data)
{
    List<string> sets = new List<string>();
    using SqlCommand cmd = conn.CreateCommand();
    cmd.Transaction = transaction;

    foreach (KeyValuePair<string, object?> item in data)
    {
        if (string.Equals(item.Key, "id", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        string parameter = "@p_" + item.Key.Replace(" ", "_");
        sets.Add("[" + item.Key.Replace("]", "]]") + "] = " + parameter);
        cmd.Parameters.AddWithValue(parameter, item.Value ?? DBNull.Value);
    }

    if (sets.Count == 0)
    {
        return 0;
    }

    cmd.CommandText = "UPDATE ProductMaster SET " + string.Join(",", sets) + " WHERE [id] = @id";
    cmd.Parameters.AddWithValue("@id", data["id"] ?? DBNull.Value);
    return cmd.ExecuteNonQuery();
}

static void InsertProductMaster(SqlConnection conn, SqlTransaction transaction, Dictionary<string, object?> data)
{
    bool idIsIdentity = IsIdentityColumn(conn, transaction, "ProductMaster", "id");
    List<string> columns = new List<string>();
    List<string> parameters = new List<string>();

    using SqlCommand cmd = conn.CreateCommand();
    cmd.Transaction = transaction;
    foreach (KeyValuePair<string, object?> item in data)
    {
        string parameter = "@p_" + item.Key.Replace(" ", "_");
        columns.Add("[" + item.Key.Replace("]", "]]") + "]");
        parameters.Add(parameter);
        cmd.Parameters.AddWithValue(parameter, item.Value ?? DBNull.Value);
    }

    string insertSql = "INSERT INTO ProductMaster (" + string.Join(",", columns) + ") VALUES (" + string.Join(",", parameters) + ")";
    if (idIsIdentity)
    {
        insertSql = "SET IDENTITY_INSERT ProductMaster ON; " + insertSql + "; SET IDENTITY_INSERT ProductMaster OFF;";
    }

    cmd.CommandText = insertSql;
    cmd.ExecuteNonQuery();
}

static bool IsIdentityColumn(SqlConnection conn, SqlTransaction transaction, string tableName, string columnName)
{
    using SqlCommand cmd = conn.CreateCommand();
    cmd.Transaction = transaction;
    cmd.CommandText = "SELECT COLUMNPROPERTY(OBJECT_ID(@table), @column, 'IsIdentity')";
    cmd.Parameters.Add("@table", SqlDbType.VarChar, 128).Value = tableName;
    cmd.Parameters.Add("@column", SqlDbType.VarChar, 128).Value = columnName;
    object? result = cmd.ExecuteScalar();
    return result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;
}

static object? NormalizeJsonValue(object? value)
{
    if (value is JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt32(out int intValue))
                {
                    return intValue;
                }
                if (element.TryGetInt64(out long longValue))
                {
                    return longValue;
                }
                if (element.TryGetDecimal(out decimal decimalValue))
                {
                    return decimalValue;
                }
                return element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            default:
                return element.ToString();
        }
    }

    return value;
}

sealed class ProductSyncRequest
{
    public List<Dictionary<string, object?>>? Records { get; set; }
}

sealed class ProductQueueAckRequest
{
    public int QueueId { get; set; }

    public string? ProductId { get; set; }

    public string? TargetBranchCode { get; set; }
}

sealed class ReportDataRequest
{
    public string? QueryText { get; set; }

    public Dictionary<string, JsonElement>? Parameters { get; set; }
}

sealed class DeploymentScriptRequest
{
    public string? DeploymentId { get; set; }

    public string? Script { get; set; }
}

sealed class ProductValidationException : Exception
{
    public ProductValidationException(object? productId, string message) : base(message)
    {
        ProductId = productId;
    }

    public object? ProductId { get; }
}

sealed class DeploymentConflictException : Exception
{
    public DeploymentConflictException(string message) : base(message)
    {
    }
}

sealed record ProductUpsertResponse(bool Success, string BranchCode, object? ProductId, string Message);

sealed record ProductQueueAckResponse(bool Success, string BranchCode, int? QueueId, string? ProductId, string? TargetBranchCode, string Message);

sealed record ProductQueueAckResult(bool Success, int StatusCode, string Message);

sealed record AvailableStockResponse(string BranchCode, string? ReturnCode, string? Message, int? ProductId, decimal? AvailableStock);

sealed record GenericApiResponse(bool Success, string BranchCode, string Message);

sealed record ReportDataResponse(bool Success, string BranchCode, List<Dictionary<string, object?>> Data);

sealed record DeploymentScriptResponse(bool Success, string BranchCode, string? DeploymentId, string Message);

sealed record DeploymentExecutionResult(string Message);

sealed record DeploymentHistoryEntry(string ScriptHash, string Status);
