/*
  Sales bill UPI payment modes.

  CardTransactions.Type stays C/D.
  CardTransactions.Mode and Receipt.Mode use @type.
  Existing Cash And Card callers still store Mode = Card.
*/

ALTER PROCEDURE dbo.savecardtransaction_Direct1
(
    @RequestId nvarchar(20),
    @bank varchar(100),
    @cardNumber varchar(100),
    @Amount varchar(20),
    @trasid varchar(30),
    @type varchar(200),
    @Result int out
)
AS
BEGIN
    IF @type = 'Cash And Card'
        SET @type = 'Card';

    INSERT INTO CardTransactions
        (TransId, Entity, EntitySource, TransDate, Amount, Type, Mode, InstrumentNumber, InstrumentBank, InstrumentDate, InstrumentSource)
    VALUES
        (NEWID(), @RequestId, 'ESTIMATION', GETDATE(), @Amount, 'C', @type, @cardNumber, @bank, GETDATE(), @trasid);

    DECLARE @val decimal;
    DECLARE @val1 decimal;
    DECLARE @val2 decimal;
    DECLARE @val3 decimal;
    DECLARE @transid uniqueidentifier;

    SET @transid = (SELECT TOP 1 TransId FROM CardTransactions ORDER BY TransDate DESC);

    INSERT INTO Receipt (TransactionId, Transdate, EntityId, CustomerName, Amount, Mode)
    SELECT c.TransId, c.TransDate, c.Entity, q.customername + '-' + q.City, c.Amount, @type
    FROM CardTransactions c
    JOIN QuotationEstimation q ON c.Entity = q.Estimationid
    WHERE TransId = @transid;

    SET @val = (SELECT GrnandTotal FROM QuotationEstimation WHERE Estimationid = @RequestId);
    SET @val1 = (SELECT Paid FROM QuotationEstimation WHERE Estimationid = @RequestId);
    SET @val2 = @val1 + @Amount;
    SET @val3 = @val - @val2;

    UPDATE QuotationEstimation SET Paid = @val2, Balance = @val3, IsBilled = 1 WHERE Estimationid = @RequestId;
    UPDATE Estimation SET Paid = @val2, Balance = @val3 WHERE Entityid = @RequestId;
    SET @Result = 1;
END
GO

ALTER PROCEDURE dbo.GetRRECash
AS
BEGIN
    SELECT EntitySource AS Type, CONVERT(numeric(18,2), ROUND(CONVERT(nvarchar(250), Amount), 0)) AS Amount, TransId
    INTO #1
    FROM CashTransaction
    WHERE IsDayEndClosed IS NULL AND DayEndCloseId IS NULL AND UPPER(Type) = 'C'
    ORDER BY TransDate DESC;

    SELECT EntitySource AS Type, CONVERT(numeric(18,2), ROUND(CONVERT(nvarchar(250), Amount), 0)) AS Amount, TransId
    INTO #2
    FROM CashTransaction
    WHERE IsDayEndClosed IS NULL AND DayEndCloseId IS NULL AND UPPER(Type) = 'D'
    ORDER BY TransDate DESC;

    SELECT [Type] 'CashType', SUM(Amount) Amount, 'In' 'Type' FROM #1 GROUP BY [Type]
    UNION ALL
    SELECT [Type] 'CashType', SUM(Amount) Amount, 'Out' 'Type' FROM #2 GROUP BY [Type]
    UNION ALL
    SELECT 'SALES' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), GrandTotal)), 0) 'Amount', 'Sales' 'Type'
    FROM Sales
    WHERE CONVERT(varchar(10), Updatedon, 120) = CONVERT(varchar(10), GETDATE(), 120)
    UNION ALL
    SELECT 'CASH' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), c.Amount)), 0) 'Amount', 'CashBill' 'Type'
    FROM QuotationEstimation q
    LEFT JOIN CashTransaction c ON q.Estimationid = c.EntityId
    WHERE Paymentmode = 'Cash Bill'
      AND CONVERT(varchar(10), date, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND CAST(Balance AS decimal) = 0
    UNION ALL
    SELECT 'CARD' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), Amount)), 0) 'Amount', 'CardBill' 'Type'
    FROM CardTransactions
    WHERE CONVERT(varchar(10), TransDate, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND ISNULL(Mode, '') <> 'UPI'
    UNION ALL
    SELECT 'UPI' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), Amount)), 0) 'Amount', 'UpiBill' 'Type'
    FROM CardTransactions
    WHERE CONVERT(varchar(10), TransDate, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND Mode = 'UPI'
    UNION ALL
    SELECT 'PARTIAL' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), c.Amount)), 0) 'Amount', 'CashBill' 'Type'
    FROM QuotationEstimation q
    LEFT JOIN CashTransaction c ON q.Estimationid = c.EntityId
    WHERE Paymentmode = 'Partial Credit Bill'
      AND CONVERT(varchar(10), date, 120) = CONVERT(varchar(10), GETDATE(), 120)
      AND CAST(Balance AS decimal) = 0
    UNION ALL
    SELECT 'CREDIT' 'CashType', ISNULL(SUM(CONVERT(decimal(18,2), GrnandTotal)), 0) 'Amount', 'CreditBill' 'Type'
    FROM QuotationEstimation
    WHERE Paymentmode = 'Credit Bill'
      AND CONVERT(varchar(10), date, 120) = CONVERT(varchar(10), GETDATE(), 120);
END
GO
