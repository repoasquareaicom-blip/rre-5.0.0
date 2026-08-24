CREATE OR ALTER PROC [dbo].[GetIssued]
(
    @id varchar(100),
    @Location varchar(100) = NULL,
    @role varchar(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ir.CustomerName,
        ir.ReceivedID,
        ir.RefNo,
        ISNULL(u.UserName, ISNULL(qh.Updatedby, '')) AS PreparedBy
    FROM Issuedreceived ir
    LEFT JOIN QuotationHeader qh
        ON qh.Quotationid = ir.RefNo
    LEFT JOIN Users u
        ON CONVERT(varchar(10), u.UserId) = qh.Updatedby
    WHERE ir.ReceivedID = @id;

    SELECT
        ReceivedID,
        p.id AS Productid,
        p.DisplayName,
        Receiveqty,
        CASE (category) WHEN '124' THEN 17 ELSE 6 END AS Location,
        CASE (category) WHEN '124' THEN 'P' ELSE 'G' END AS LocationName,
        ISNULL(IssueQty, 0) AS IssueQty,
        TotalQty
    FROM Issuedreceiveddetails q
    LEFT JOIN ProductMaster p
        ON q.Productid = p.id
    LEFT JOIN Location l
        ON q.Location = l.LocationID
    WHERE ReceivedID = @id
      AND Receiveqty <> ISNULL(IssueQty, 0);
END
