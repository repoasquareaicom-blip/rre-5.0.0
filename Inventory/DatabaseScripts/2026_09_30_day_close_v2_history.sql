-- Day Close V2 History (read only).
-- Lists business dates in a range with their Day Close status:
--     CLOSED     = a DayCloseV2 row with Status = 'Completed' (saved values shown)
--     NOT CLOSED = any other date that has a DayCloseV2 row, payment transactions or goods movements
-- Business date convention is the same as Day Close V2 (calendar date of the transaction).
-- For NOT CLOSED dates the expected total is the current Day Close V2 calculation
-- (previous actual closing + open population net), exactly as Proc_DayCloseV2_GetExpectedSummary.
-- Nothing is written.

SET NOCOUNT ON;
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetHistory', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetHistory AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetHistory
(
    @FromDate date,
    @ToDate date,
    @Status varchar(20) = 'ALL'
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @FromDate IS NULL OR @ToDate IS NULL OR @ToDate < @FromDate
    BEGIN
        SELECT CAST(NULL AS date) AS BusinessDate, CAST(NULL AS varchar(20)) AS DayCloseStatus, CAST(NULL AS int) AS DayCloseId,
            CAST(NULL AS decimal(18, 2)) AS ExpectedTotal, CAST(NULL AS decimal(18, 2)) AS ActualTotal,
            CAST(NULL AS decimal(18, 2)) AS TotalDifference, CAST(NULL AS nvarchar(200)) AS EnteredBy, CAST(NULL AS datetime) AS EnteredOn
        WHERE 1 = 0;
        RETURN;
    END

    SET @Status = UPPER(LTRIM(RTRIM(ISNULL(@Status, 'ALL'))));

    DECLARE @from datetime = CONVERT(datetime, @FromDate);
    DECLARE @to datetime = DATEADD(day, 1, CONVERT(datetime, @ToDate));

    DECLARE @dates TABLE (BusinessDate date NOT NULL PRIMARY KEY);
    INSERT INTO @dates (BusinessDate)
    SELECT h.BusinessDate FROM dbo.DayCloseV2 h WHERE h.BusinessDate >= @FromDate AND h.BusinessDate <= @ToDate
    UNION
    SELECT s.BusinessDate FROM dbo.fn_DayCloseV2_SourceRows(@from, @to) s
    UNION
    SELECT CONVERT(date, mt.TranscationDate) FROM dbo.MaterialTranscation mt WHERE mt.TranscationDate >= @from AND mt.TranscationDate < @to;

    SELECT
        d.BusinessDate,
        CONVERT(varchar(20), CASE WHEN h.Id IS NOT NULL THEN 'CLOSED' ELSE 'NOT CLOSED' END) AS DayCloseStatus,
        h.Id AS DayCloseId,
        CONVERT(decimal(18, 2), CASE WHEN h.Id IS NOT NULL THEN h.ExpectedTotal ELSE ISNULL(o.Opening, 0) + ISNULL(x.Net, 0) END) AS ExpectedTotal,
        h.ActualTotal,
        h.TotalDifference,
        CONVERT(nvarchar(200), CASE WHEN h.Id IS NOT NULL
            THEN ISNULL(NULLIF(LTRIM(RTRIM(eu.UserFullName)), ''), ISNULL(eu.UserName, CONVERT(varchar(20), h.EnteredBy))) END) AS EnteredBy,
        h.EnteredOn
    FROM @dates d
    LEFT JOIN dbo.DayCloseV2 h ON h.BusinessDate = d.BusinessDate AND h.Status = 'Completed'
    LEFT JOIN dbo.Users eu ON eu.UserId = h.EnteredBy
    OUTER APPLY (SELECT TOP 1 p.ActualCashClosing AS Opening
                 FROM dbo.DayCloseV2 p
                 WHERE h.Id IS NULL AND p.Status = 'Completed' AND p.BusinessDate < d.BusinessDate
                 ORDER BY p.BusinessDate DESC) o
    OUTER APPLY (SELECT SUM(op.AmountIn - op.AmountOut) AS Net
                 FROM dbo.fn_DayCloseV2_OpenPopulation(d.BusinessDate, GETDATE()) op
                 WHERE h.Id IS NULL) x
    WHERE @Status = 'ALL'
       OR (@Status = 'CLOSED' AND h.Id IS NOT NULL)
       OR (@Status = 'NOT CLOSED' AND h.Id IS NULL)
    ORDER BY d.BusinessDate DESC;
END
GO
