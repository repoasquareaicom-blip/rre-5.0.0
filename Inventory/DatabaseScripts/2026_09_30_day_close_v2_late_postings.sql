/*
  Day Close V2 late postings.

  Requires 2026_09_29_day_close_v2_reconciliation.sql, 2026_09_30_day_close_v2_opening_goods_pdf.sql and
  2026_09_30_cash_transaction_narration.sql (this script replaces their Proc_DayCloseV2_GetExpectedSummary,
  GetDenominationRecon, GetReport, Submit and GetPaymentLedger).

  - DayCloseV2.TransactionCutoff: server time up to which a close considered transactions.
  - DayCloseV2Transaction: every transaction reconciled by a close (INCLUDED from its own business date,
    CARRIED_FORWARD from an earlier closed date) with the ledger snapshot needed to reproduce the submitted
    close even if the source row is later deleted or archived. UNIQUE (SourceTable, TransId) allows a
    transaction to be reconciled by at most one close.
  - DayCloseV2PaymentSummary.CarriedIn / CarriedOut: carried-forward portion of TotalIn / TotalOut.
  - A transaction dated on a link-tracked closed date that is not linked to that close is a post-close
    (late) transaction. It is shown on that close for information only and is carried, once, into the next
    Day Close submitted for a later business date.
  - Existing closes are back-filled (TransactionCutoff = EnteredOn plus INCLUDED links) only when a
    recomputation up to EnteredOn reproduces the saved Cash/UPI/Card IN and OUT exactly. Closes that do not
    reproduce stay untracked and never produce late postings.

  RISK: the legacy Cash End Close (Proc_CashEndCloseVerification) moves every CashTransaction row to
  CashTransactionHistory and deletes it. Submitted Day Close V2 ledgers keep their snapshot, but live views
  (open-day ledger, pending late postings, denomination movement) cannot see archived rows. Do not run the
  legacy close alongside Day Close V2 until that decision is made.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF DB_NAME() IN ('master', 'model', 'msdb', 'tempdb')
BEGIN
    RAISERROR('Refusing to change a system database.', 16, 1);
    RETURN;
END
GO

IF COL_LENGTH('dbo.DayCloseV2', 'TransactionCutoff') IS NULL
    ALTER TABLE dbo.DayCloseV2 ADD TransactionCutoff datetime NULL;
GO

IF COL_LENGTH('dbo.DayCloseV2PaymentSummary', 'CarriedIn') IS NULL
    ALTER TABLE dbo.DayCloseV2PaymentSummary ADD CarriedIn decimal(18, 2) NOT NULL
        CONSTRAINT DF_DayCloseV2PaymentSummary_CarriedIn DEFAULT (0);
GO

IF COL_LENGTH('dbo.DayCloseV2PaymentSummary', 'CarriedOut') IS NULL
    ALTER TABLE dbo.DayCloseV2PaymentSummary ADD CarriedOut decimal(18, 2) NOT NULL
        CONSTRAINT DF_DayCloseV2PaymentSummary_CarriedOut DEFAULT (0);
GO

IF OBJECT_ID('dbo.DayCloseV2Transaction', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DayCloseV2Transaction
    (
        Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_DayCloseV2Transaction PRIMARY KEY,
        DayCloseId int NOT NULL CONSTRAINT FK_DayCloseV2Transaction_DayCloseV2 REFERENCES dbo.DayCloseV2 (Id),
        SourceTable varchar(20) NOT NULL
            CONSTRAINT CK_DayCloseV2Transaction_SourceTable CHECK (SourceTable IN ('CashTransaction', 'CardTransactions')),
        TransId uniqueidentifier NOT NULL,
        Role varchar(20) NOT NULL
            CONSTRAINT CK_DayCloseV2Transaction_Role CHECK (Role IN ('INCLUDED', 'CARRIED_FORWARD')),
        OriginalBusinessDate date NOT NULL,
        PostedOn datetime NOT NULL,
        Channel varchar(10) NOT NULL
            CONSTRAINT CK_DayCloseV2Transaction_Channel CHECK (Channel IN ('CASH', 'UPI', 'CARD')),
        StoredType nvarchar(10) NULL,
        Direction varchar(3) NOT NULL
            CONSTRAINT CK_DayCloseV2Transaction_Direction CHECK (Direction IN ('IN', 'OUT')),
        EntityReference nvarchar(255) NULL,
        EntityType nvarchar(255) NULL,
        Description nvarchar(500) NULL,
        AmountIn decimal(18, 2) NOT NULL,
        AmountOut decimal(18, 2) NOT NULL,
        LinkedOn datetime NOT NULL CONSTRAINT DF_DayCloseV2Transaction_LinkedOn DEFAULT (GETDATE()),
        CONSTRAINT UQ_DayCloseV2Transaction_Source UNIQUE (SourceTable, TransId)
    );
    CREATE INDEX IX_DayCloseV2Transaction_DayClose ON dbo.DayCloseV2Transaction (DayCloseId, Role);
    CREATE INDEX IX_DayCloseV2Transaction_OriginalDate ON dbo.DayCloseV2Transaction (OriginalBusinessDate, Role);
END
GO

/* Live Cash/UPI/Card transactions posted in [@From, @To), with the same IN/OUT rules as the Day Close summary. */
IF OBJECT_ID('dbo.fn_DayCloseV2_SourceRows', 'IF') IS NULL
    EXEC('CREATE FUNCTION dbo.fn_DayCloseV2_SourceRows (@From datetime, @To datetime) RETURNS TABLE AS RETURN (SELECT 1 AS Placeholder)');
GO

ALTER FUNCTION dbo.fn_DayCloseV2_SourceRows
(
    @From datetime,
    @To datetime
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        CONVERT(varchar(20), 'CashTransaction') AS SourceTable,
        c.TransId,
        c.TransDate AS PostedOn,
        CONVERT(date, c.TransDate) AS BusinessDate,
        CONVERT(varchar(10), 'CASH') AS Channel,
        CONVERT(nvarchar(10), c.Type) AS StoredType,
        CONVERT(varchar(3), CASE WHEN UPPER(c.Type) = 'D' THEN 'OUT' ELSE 'IN' END) AS Direction,
        CONVERT(nvarchar(255), c.EntityId) AS EntityReference,
        CONVERT(nvarchar(255), c.EntitySource) AS EntityType,
        CONVERT(nvarchar(500), COALESCE(NULLIF(LTRIM(RTRIM(c.Narration)), ''), n.Narration, q.customername, '')) AS Description,
        CONVERT(decimal(18, 2), CASE WHEN UPPER(c.Type) = 'C' THEN REPLACE(c.Amount, ',', '') ELSE '0' END) AS AmountIn,
        CONVERT(decimal(18, 2), CASE WHEN UPPER(c.Type) = 'D' THEN REPLACE(c.Amount, ',', '') ELSE '0' END) AS AmountOut
    FROM dbo.CashTransaction c
    OUTER APPLY
    (
        SELECT TOP 1 qe.customername FROM dbo.QuotationEstimation qe WHERE qe.Estimationid = c.EntityId
    ) q
    OUTER APPLY
    (
        SELECT TOP 1 LEFT(NULLIF(LTRIM(RTRIM(r.Reason)), ''), 250) AS Narration
        FROM
        (
            SELECT cr.Reason FROM dbo.CashRequest cr WHERE c.EntitySource LIKE 'CASH REQUEST%' AND cr.RequestId = c.EntityId
            UNION ALL
            SELECT rc.Reason FROM dbo.CashReceipt rc WHERE c.EntitySource LIKE 'CASH RECEIPT%' AND rc.ReceiptId = c.EntityId
        ) r
    ) n
    WHERE c.TransDate >= @From AND c.TransDate < @To

    UNION ALL

    SELECT
        CONVERT(varchar(20), 'CardTransactions'),
        t.TransId,
        t.TransDate,
        CONVERT(date, t.TransDate),
        CONVERT(varchar(10), CASE WHEN t.Mode = 'UPI' THEN 'UPI' ELSE 'CARD' END),
        CONVERT(nvarchar(10), t.Type),
        CONVERT(varchar(3), CASE WHEN t.Type = 'D' THEN 'OUT' ELSE 'IN' END),
        CONVERT(nvarchar(255), t.Entity),
        CONVERT(nvarchar(255), t.EntitySource),
        CONVERT(nvarchar(500), COALESCE(
            q.customername,
            n.Narration + CASE WHEN NULLIF(LTRIM(RTRIM(t.Reference)), '') IS NULL THEN '' ELSE ' (Ref ' + LTRIM(RTRIM(t.Reference)) + ')' END,
            t.Reference,
            '')),
        CONVERT(decimal(18, 2), CASE WHEN ISNULL(t.Type, 'C') = 'C' THEN t.Amount ELSE 0 END),
        CONVERT(decimal(18, 2), CASE WHEN t.Type = 'D' THEN t.Amount ELSE 0 END)
    FROM dbo.CardTransactions t
    OUTER APPLY
    (
        SELECT TOP 1 qe.customername FROM dbo.QuotationEstimation qe WHERE qe.Estimationid = t.Entity
    ) q
    OUTER APPLY
    (
        SELECT TOP 1 LEFT(NULLIF(LTRIM(RTRIM(r.Reason)), ''), 250) AS Narration
        FROM
        (
            SELECT cr.Reason FROM dbo.CashRequest cr WHERE t.EntitySource LIKE 'CASH REQUEST%' AND cr.RequestId = t.Entity
            UNION ALL
            SELECT rc.Reason FROM dbo.CashReceipt rc WHERE t.EntitySource LIKE 'CASH RECEIPT%' AND rc.ReceiptId = t.Entity
        ) r
    ) n
    WHERE t.TransDate >= @From AND t.TransDate < @To
      AND t.Mode IN ('UPI', 'Card')
);
GO

/*
  Transactions an open Day Close for @BusinessDate reconciles when considered at @Cutoff:
  its own unlinked transactions plus unlinked late postings from earlier link-tracked closed dates.
*/
IF OBJECT_ID('dbo.fn_DayCloseV2_OpenPopulation', 'IF') IS NULL
    EXEC('CREATE FUNCTION dbo.fn_DayCloseV2_OpenPopulation (@BusinessDate date, @Cutoff datetime) RETURNS TABLE AS RETURN (SELECT 1 AS Placeholder)');
GO

ALTER FUNCTION dbo.fn_DayCloseV2_OpenPopulation
(
    @BusinessDate date,
    @Cutoff datetime
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        s.SourceTable, s.TransId, s.PostedOn, s.BusinessDate AS OriginalBusinessDate, s.Channel, s.StoredType,
        s.Direction, s.EntityReference, s.EntityType, s.Description, s.AmountIn, s.AmountOut,
        CONVERT(varchar(20), 'INCLUDED') AS Role
    FROM dbo.fn_DayCloseV2_SourceRows(CONVERT(datetime, @BusinessDate), DATEADD(day, 1, CONVERT(datetime, @BusinessDate))) s
    WHERE s.PostedOn <= @Cutoff
      AND NOT EXISTS (SELECT 1 FROM dbo.DayCloseV2Transaction l WHERE l.SourceTable = s.SourceTable AND l.TransId = s.TransId)

    UNION ALL

    SELECT
        s.SourceTable, s.TransId, s.PostedOn, s.BusinessDate, s.Channel, s.StoredType,
        s.Direction, s.EntityReference, s.EntityType, s.Description, s.AmountIn, s.AmountOut,
        CONVERT(varchar(20), 'CARRIED_FORWARD')
    FROM dbo.DayCloseV2 h
    CROSS APPLY dbo.fn_DayCloseV2_SourceRows(CONVERT(datetime, h.BusinessDate), DATEADD(day, 1, CONVERT(datetime, h.BusinessDate))) s
    WHERE h.Status = 'Completed'
      AND h.TransactionCutoff IS NOT NULL
      AND h.BusinessDate < @BusinessDate
      AND s.PostedOn <= @Cutoff
      AND s.TransId IS NOT NULL
      AND NOT EXISTS (SELECT 1 FROM dbo.DayCloseV2Transaction l WHERE l.SourceTable = s.SourceTable AND l.TransId = s.TransId)
);
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetExpectedSummary', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetExpectedSummary AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetExpectedSummary
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
    BEGIN
        SELECT
            1 AS HasPreviousClose,
            1 AS IsClosed,
            h.OpeningCash,
            cash.TotalIn AS CashIn,
            cash.TotalOut AS CashOut,
            cash.ExpectedNet AS ExpectedCash,
            upi.TotalIn AS UpiIn,
            upi.TotalOut AS UpiOut,
            upi.ExpectedNet AS ExpectedUpi,
            card.TotalIn AS CardIn,
            card.TotalOut AS CardOut,
            card.ExpectedNet AS ExpectedCard,
            h.ActualCashClosing,
            h.ActualUpiNet,
            h.ActualCardNet,
            h.Remarks,
            cash.CarriedIn AS CashCarriedIn,
            cash.CarriedOut AS CashCarriedOut,
            upi.CarriedIn AS UpiCarriedIn,
            upi.CarriedOut AS UpiCarriedOut,
            card.CarriedIn AS CardCarriedIn,
            card.CarriedOut AS CardCarriedOut,
            h.TransactionCutoff
        FROM dbo.DayCloseV2 h
        INNER JOIN dbo.DayCloseV2PaymentSummary cash ON cash.DayCloseId = h.Id AND cash.PaymentMode = 'CASH'
        INNER JOIN dbo.DayCloseV2PaymentSummary upi ON upi.DayCloseId = h.Id AND upi.PaymentMode = 'UPI'
        INNER JOIN dbo.DayCloseV2PaymentSummary card ON card.DayCloseId = h.Id AND card.PaymentMode = 'CARD'
        WHERE h.BusinessDate = @BusinessDate AND h.Status = 'Completed';
        RETURN;
    END

    DECLARE @opening decimal(18, 2);
    DECLARE @hasPrevious bit;
    DECLARE @cashIn decimal(18, 2);
    DECLARE @cashOut decimal(18, 2);
    DECLARE @upiIn decimal(18, 2);
    DECLARE @upiOut decimal(18, 2);
    DECLARE @cardIn decimal(18, 2);
    DECLARE @cardOut decimal(18, 2);
    DECLARE @cashCarriedIn decimal(18, 2);
    DECLARE @cashCarriedOut decimal(18, 2);
    DECLARE @upiCarriedIn decimal(18, 2);
    DECLARE @upiCarriedOut decimal(18, 2);
    DECLARE @cardCarriedIn decimal(18, 2);
    DECLARE @cardCarriedOut decimal(18, 2);

    SELECT TOP 1 @opening = ActualCashClosing
    FROM dbo.DayCloseV2
    WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
    ORDER BY BusinessDate DESC;

    SET @hasPrevious = CASE WHEN @opening IS NULL THEN 0 ELSE 1 END;
    IF @opening IS NULL SET @opening = 0;

    SELECT
        @cashIn = ISNULL(SUM(CASE WHEN Channel = 'CASH' THEN AmountIn ELSE 0 END), 0),
        @cashOut = ISNULL(SUM(CASE WHEN Channel = 'CASH' THEN AmountOut ELSE 0 END), 0),
        @upiIn = ISNULL(SUM(CASE WHEN Channel = 'UPI' THEN AmountIn ELSE 0 END), 0),
        @upiOut = ISNULL(SUM(CASE WHEN Channel = 'UPI' THEN AmountOut ELSE 0 END), 0),
        @cardIn = ISNULL(SUM(CASE WHEN Channel = 'CARD' THEN AmountIn ELSE 0 END), 0),
        @cardOut = ISNULL(SUM(CASE WHEN Channel = 'CARD' THEN AmountOut ELSE 0 END), 0),
        @cashCarriedIn = ISNULL(SUM(CASE WHEN Channel = 'CASH' AND Role = 'CARRIED_FORWARD' THEN AmountIn ELSE 0 END), 0),
        @cashCarriedOut = ISNULL(SUM(CASE WHEN Channel = 'CASH' AND Role = 'CARRIED_FORWARD' THEN AmountOut ELSE 0 END), 0),
        @upiCarriedIn = ISNULL(SUM(CASE WHEN Channel = 'UPI' AND Role = 'CARRIED_FORWARD' THEN AmountIn ELSE 0 END), 0),
        @upiCarriedOut = ISNULL(SUM(CASE WHEN Channel = 'UPI' AND Role = 'CARRIED_FORWARD' THEN AmountOut ELSE 0 END), 0),
        @cardCarriedIn = ISNULL(SUM(CASE WHEN Channel = 'CARD' AND Role = 'CARRIED_FORWARD' THEN AmountIn ELSE 0 END), 0),
        @cardCarriedOut = ISNULL(SUM(CASE WHEN Channel = 'CARD' AND Role = 'CARRIED_FORWARD' THEN AmountOut ELSE 0 END), 0)
    FROM dbo.fn_DayCloseV2_OpenPopulation(@BusinessDate, GETDATE());

    SELECT
        @hasPrevious AS HasPreviousClose,
        0 AS IsClosed,
        @opening AS OpeningCash,
        @cashIn AS CashIn,
        @cashOut AS CashOut,
        @opening + @cashIn - @cashOut AS ExpectedCash,
        @upiIn AS UpiIn,
        @upiOut AS UpiOut,
        @upiIn - @upiOut AS ExpectedUpi,
        @cardIn AS CardIn,
        @cardOut AS CardOut,
        @cardIn - @cardOut AS ExpectedCard,
        CAST(NULL AS decimal(18, 2)) AS ActualCashClosing,
        CAST(NULL AS decimal(18, 2)) AS ActualUpiNet,
        CAST(NULL AS decimal(18, 2)) AS ActualCardNet,
        CAST(NULL AS nvarchar(500)) AS Remarks,
        @cashCarriedIn AS CashCarriedIn,
        @cashCarriedOut AS CashCarriedOut,
        @upiCarriedIn AS UpiCarriedIn,
        @upiCarriedOut AS UpiCarriedOut,
        @cardCarriedIn AS CardCarriedIn,
        @cardCarriedOut AS CardCarriedOut,
        CAST(NULL AS datetime) AS TransactionCutoff;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetPaymentLedger', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetPaymentLedger AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetPaymentLedger
(
    @BusinessDate date,
    @Filter varchar(20),
    @OpeningCash decimal(18, 2)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Filter IS NULL OR @Filter = '' SET @Filter = 'ALL';
    IF @OpeningCash IS NULL SET @OpeningCash = 0;

    DECLARE @id int;
    DECLARE @cutoff datetime;
    SELECT @id = Id, @cutoff = TransactionCutoff
    FROM dbo.DayCloseV2
    WHERE BusinessDate = @BusinessDate AND Status = 'Completed';

    DECLARE @rows TABLE
    (
        Posting nvarchar(40) NOT NULL,
        SortGroup int NOT NULL,
        TransTime datetime NULL,
        Channel varchar(10) NOT NULL,
        Direction varchar(3) NOT NULL,
        EntityReference nvarchar(255) NULL,
        EntityType nvarchar(255) NULL,
        Description nvarchar(500) NULL,
        AmountIn decimal(18, 2) NOT NULL,
        AmountOut decimal(18, 2) NOT NULL
    );

    IF @id IS NOT NULL AND @cutoff IS NOT NULL
    BEGIN
        INSERT INTO @rows
        SELECT
            CASE WHEN Role = 'CARRIED_FORWARD' THEN 'CARRIED FWD ' + CONVERT(varchar(5), OriginalBusinessDate, 105) ELSE '' END,
            CASE WHEN Role = 'CARRIED_FORWARD' THEN 0 ELSE 1 END,
            PostedOn, Channel, Direction, EntityReference, EntityType, Description, AmountIn, AmountOut
        FROM dbo.DayCloseV2Transaction
        WHERE DayCloseId = @id;
    END
    ELSE IF @id IS NOT NULL
    BEGIN
        INSERT INTO @rows
        SELECT '', 1, PostedOn, Channel, Direction, EntityReference, EntityType, Description, AmountIn, AmountOut
        FROM dbo.fn_DayCloseV2_SourceRows(CONVERT(datetime, @BusinessDate), DATEADD(day, 1, CONVERT(datetime, @BusinessDate)));
    END
    ELSE
    BEGIN
        INSERT INTO @rows
        SELECT
            CASE WHEN Role = 'CARRIED_FORWARD' THEN 'CARRIED FWD ' + CONVERT(varchar(5), OriginalBusinessDate, 105) ELSE '' END,
            CASE WHEN Role = 'CARRIED_FORWARD' THEN 0 ELSE 1 END,
            PostedOn, Channel, Direction, EntityReference, EntityType, Description, AmountIn, AmountOut
        FROM dbo.fn_DayCloseV2_OpenPopulation(@BusinessDate, GETDATE());
    END

    ;WITH numbered AS
    (
        SELECT r.*, ROW_NUMBER() OVER (ORDER BY SortGroup, TransTime, Channel, EntityReference) AS Seq
        FROM @rows r
    ),
    balanced AS
    (
        SELECT numbered.*,
            @OpeningCash + SUM(CASE WHEN Channel = 'CASH' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY Seq ROWS UNBOUNDED PRECEDING) AS CashRunning,
            SUM(CASE WHEN Channel = 'UPI' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY Seq ROWS UNBOUNDED PRECEDING) AS UpiRunning,
            SUM(CASE WHEN Channel = 'CARD' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY Seq ROWS UNBOUNDED PRECEDING) AS CardRunning
        FROM numbered
    )
    SELECT
        Posting,
        TransTime,
        Channel,
        Direction,
        EntityReference,
        EntityType,
        Description,
        AmountIn,
        AmountOut,
        CASE Channel WHEN 'CASH' THEN CashRunning WHEN 'UPI' THEN UpiRunning ELSE CardRunning END AS RunningBalance
    FROM balanced
    WHERE @Filter = 'ALL' OR (Channel + ' ' + Direction) = @Filter
    ORDER BY Seq;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetPostCloseTransactions', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetPostCloseTransactions AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetPostCloseTransactions
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id int;
    DECLARE @cutoff datetime;
    SELECT @id = Id, @cutoff = TransactionCutoff
    FROM dbo.DayCloseV2
    WHERE BusinessDate = @BusinessDate AND Status = 'Completed';

    SELECT TransTime, Channel, Direction, EntityReference, EntityType, Description, AmountIn, AmountOut, Status
    FROM
    (
        SELECT
            l.PostedOn AS TransTime, l.Channel, l.Direction, l.EntityReference, l.EntityType, l.Description,
            l.AmountIn, l.AmountOut,
            CONVERT(nvarchar(60), 'Carried to DC-' + RIGHT('00000' + CONVERT(varchar(10), h.Id), 5)
                + ' (' + CONVERT(varchar(10), h.BusinessDate, 105) + ')') AS Status
        FROM dbo.DayCloseV2Transaction l
        INNER JOIN dbo.DayCloseV2 h ON h.Id = l.DayCloseId
        WHERE l.Role = 'CARRIED_FORWARD'
          AND l.OriginalBusinessDate = @BusinessDate
          AND @cutoff IS NOT NULL

        UNION ALL

        SELECT
            s.PostedOn, s.Channel, s.Direction, s.EntityReference, s.EntityType, s.Description,
            s.AmountIn, s.AmountOut,
            CONVERT(nvarchar(60), 'Pending - next Day Close')
        FROM dbo.fn_DayCloseV2_SourceRows(CONVERT(datetime, @BusinessDate), DATEADD(day, 1, CONVERT(datetime, @BusinessDate))) s
        WHERE @cutoff IS NOT NULL
          AND s.TransId IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM dbo.DayCloseV2Transaction l WHERE l.SourceTable = s.SourceTable AND l.TransId = s.TransId)
    ) x
    ORDER BY TransTime, Channel, EntityReference;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetDenominationRecon', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetDenominationRecon AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetDenominationRecon
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id int;
    SELECT @id = Id
    FROM dbo.DayCloseV2
    WHERE BusinessDate = @BusinessDate AND Status = 'Completed';

    IF @id IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.DayCloseV2DenominationLine WHERE DayCloseId = @id)
    BEGIN
        SELECT FaceValue, OpeningQty, InQty, OutQty, ExpectedQty, ActualQty
        FROM dbo.DayCloseV2DenominationLine
        WHERE DayCloseId = @id
        ORDER BY FaceValue DESC;
        RETURN;
    END

    DECLARE @prevId int;

    IF @id IS NULL
    BEGIN
        SELECT TOP 1 @prevId = Id
        FROM dbo.DayCloseV2
        WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
        ORDER BY BusinessDate DESC;
    END

    ;WITH cashRows AS
    (
        SELECT p.TransId, p.StoredType
        FROM dbo.fn_DayCloseV2_OpenPopulation(@BusinessDate, GETDATE()) p
        WHERE @id IS NULL AND p.SourceTable = 'CashTransaction'
        UNION ALL
        SELECT s.TransId, s.StoredType
        FROM dbo.fn_DayCloseV2_SourceRows(CONVERT(datetime, @BusinessDate), DATEADD(day, 1, CONVERT(datetime, @BusinessDate))) s
        WHERE @id IS NOT NULL AND s.SourceTable = 'CashTransaction'
    ),
    notes AS
    (
        SELECT UPPER(LTRIM(RTRIM(c.StoredType))) AS PayType, v.FaceValue,
            CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(v.Qty)), '')), 0)) AS Qty
        FROM cashRows c
        INNER JOIN CashTransactionDenomination d ON d.TransId = c.TransId
        CROSS APPLY (VALUES
            (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
            (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
        ) v (FaceValue, Qty)
    ),
    movement AS
    (
        SELECT FaceValue,
            SUM(CASE WHEN PayType = 'C' AND Qty > 0 THEN Qty WHEN PayType = 'D' AND Qty < 0 THEN -Qty ELSE 0 END) AS InQty,
            SUM(CASE WHEN PayType = 'C' AND Qty < 0 THEN -Qty WHEN PayType = 'D' AND Qty > 0 THEN Qty ELSE 0 END) AS OutQty
        FROM notes
        GROUP BY FaceValue
    ),
    faces AS
    (
        SELECT 2000 AS FaceValue UNION ALL SELECT 1000 UNION ALL SELECT 500 UNION ALL SELECT 200
        UNION ALL SELECT 100 UNION ALL SELECT 50 UNION ALL SELECT 20 UNION ALL SELECT 10
        UNION ALL SELECT 5 UNION ALL SELECT 2 UNION ALL SELECT 1
    ),
    counts AS
    (
        SELECT v.FaceValue, v.Qty, d.DayCloseId
        FROM dbo.DayCloseV2Denomination d
        CROSS APPLY (VALUES
            (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
            (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
        ) v (FaceValue, Qty)
        WHERE d.DayCloseId IN (ISNULL(@prevId, -1), ISNULL(@id, -1))
    )
    SELECT
        f.FaceValue,
        ISNULL(o.Qty, 0) AS OpeningQty,
        ISNULL(m.InQty, 0) AS InQty,
        ISNULL(m.OutQty, 0) AS OutQty,
        ISNULL(o.Qty, 0) + ISNULL(m.InQty, 0) - ISNULL(m.OutQty, 0) AS ExpectedQty,
        a.Qty AS ActualQty
    FROM faces f
    LEFT JOIN movement m ON m.FaceValue = f.FaceValue
    LEFT JOIN counts o ON o.FaceValue = f.FaceValue AND o.DayCloseId = @prevId
    LEFT JOIN counts a ON a.FaceValue = f.FaceValue AND a.DayCloseId = @id
    ORDER BY f.FaceValue DESC;
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_GetReport', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_GetReport AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_GetReport
(
    @BusinessDate date
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id int;
    SELECT @id = Id
    FROM dbo.DayCloseV2
    WHERE BusinessDate = @BusinessDate AND Status = 'Completed';

    SELECT
        h.Id, h.BusinessDate, h.OpeningCash, h.OpeningDefaultCash,
        h.ExpectedCashClosing, h.ActualCashClosing, h.CashDifference,
        h.ExpectedUpiNet, h.ActualUpiNet, h.UpiDifference,
        h.ExpectedCardNet, h.ActualCardNet, h.CardDifference,
        h.ExpectedTotal, h.ActualTotal, h.TotalDifference,
        h.Status, h.Remarks,
        ISNULL(NULLIF(eu.UserFullName, ''), ISNULL(eu.UserName, CONVERT(varchar(20), h.EnteredBy))) AS ClosedBy,
        h.EnteredOn AS ClosedOn,
        COALESCE(NULLIF(LTRIM(RTRIM(eu.UserFullName)), ''), NULLIF(LTRIM(RTRIM(eu.UserName)), '')) AS PreparedBy,
        ISNULL(NULLIF(ru.UserFullName, ''), ru.UserName) AS ReviewedBy,
        h.ReviewedOn,
        h.TransactionCutoff
    FROM dbo.DayCloseV2 h
    LEFT JOIN dbo.Users eu ON eu.UserId = h.EnteredBy
    LEFT JOIN dbo.Users ru ON ru.UserId = h.ReviewedBy
    WHERE h.Id = @id;

    SELECT PaymentMode, OpeningAmount, TotalIn, TotalOut, ExpectedNet, ActualAmount, Difference, CarriedIn, CarriedOut
    FROM dbo.DayCloseV2PaymentSummary
    WHERE DayCloseId = @id
    ORDER BY CASE PaymentMode WHEN 'CASH' THEN 1 WHEN 'UPI' THEN 2 ELSE 3 END;

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2DenominationLine WHERE DayCloseId = @id)
    BEGIN
        SELECT FaceValue, OpeningQty, InQty, OutQty, ExpectedQty, ActualQty
        FROM dbo.DayCloseV2DenominationLine
        WHERE DayCloseId = @id
        ORDER BY FaceValue DESC;
    END
    ELSE
    BEGIN
        SELECT v.FaceValue, CAST(NULL AS int) AS OpeningQty, CAST(NULL AS int) AS InQty, CAST(NULL AS int) AS OutQty,
            CAST(NULL AS int) AS ExpectedQty, v.Qty AS ActualQty
        FROM dbo.DayCloseV2Denomination d
        CROSS APPLY (VALUES
            (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
            (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
        ) v (FaceValue, Qty)
        WHERE d.DayCloseId = @id
        ORDER BY v.FaceValue DESC;
    END
END
GO

IF OBJECT_ID('dbo.Proc_DayCloseV2_Submit', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Proc_DayCloseV2_Submit AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.Proc_DayCloseV2_Submit
(
    @BusinessDate date,
    @OpeningCash decimal(18, 2),
    @ActualUpi decimal(18, 2),
    @ActualCard decimal(18, 2),
    @Remarks nvarchar(500),
    @EnteredBy int,
    @TwoThousand int,
    @Thousand int,
    @FiveHundred int,
    @TwoHundred int,
    @Hundred int,
    @Fifty int,
    @Twenty int,
    @Ten int,
    @Five int,
    @Two int,
    @One int,
    @Result int OUT,
    @Message nvarchar(400) OUT,
    @OpenTwoThousand int = NULL,
    @OpenThousand int = NULL,
    @OpenFiveHundred int = NULL,
    @OpenTwoHundred int = NULL,
    @OpenHundred int = NULL,
    @OpenFifty int = NULL,
    @OpenTwenty int = NULL,
    @OpenTen int = NULL,
    @OpenFive int = NULL,
    @OpenTwo int = NULL,
    @OpenOne int = NULL,
    @DayCloseId int = NULL OUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Result = 0;
    SET @Message = '';
    SET @DayCloseId = NULL;

    IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
    BEGIN
        SET @Message = 'This business date is already closed.';
        RETURN;
    END

    DECLARE @defaultOpening decimal(18, 2);
    DECLARE @opening decimal(18, 2);
    DECLARE @cashIn decimal(18, 2);
    DECLARE @cashOut decimal(18, 2);
    DECLARE @upiIn decimal(18, 2);
    DECLARE @upiOut decimal(18, 2);
    DECLARE @cardIn decimal(18, 2);
    DECLARE @cardOut decimal(18, 2);
    DECLARE @cashCarriedIn decimal(18, 2);
    DECLARE @cashCarriedOut decimal(18, 2);
    DECLARE @upiCarriedIn decimal(18, 2);
    DECLARE @upiCarriedOut decimal(18, 2);
    DECLARE @cardCarriedIn decimal(18, 2);
    DECLARE @cardCarriedOut decimal(18, 2);
    DECLARE @expectedCash decimal(18, 2);
    DECLARE @expectedUpi decimal(18, 2);
    DECLARE @expectedCard decimal(18, 2);
    DECLARE @actualCash decimal(18, 2);
    DECLARE @prevId int;
    DECLARE @openTotal decimal(18, 2);
    DECLARE @openSupplied bit;
    DECLARE @cutoff datetime;
    DECLARE @closeId int;

    SELECT TOP 1 @defaultOpening = ActualCashClosing, @prevId = Id
    FROM dbo.DayCloseV2
    WHERE Status = 'Completed' AND BusinessDate < @BusinessDate
    ORDER BY BusinessDate DESC;

    SET @openSupplied = CASE WHEN COALESCE(@OpenTwoThousand, @OpenThousand, @OpenFiveHundred, @OpenTwoHundred, @OpenHundred,
        @OpenFifty, @OpenTwenty, @OpenTen, @OpenFive, @OpenTwo, @OpenOne) IS NULL THEN 0 ELSE 1 END;

    IF @OpeningCash IS NULL
    BEGIN
        IF @prevId IS NULL
        BEGIN
            SET @Message = 'Enter the opening cash for the first Day Close.';
            RETURN;
        END
        SET @opening = @defaultOpening;
    END
    ELSE
        SET @opening = @OpeningCash;

    IF @opening < 0
    BEGIN
        SET @Message = 'Opening cash cannot be negative.';
        RETURN;
    END

    IF @openSupplied = 0 AND @prevId IS NOT NULL
    BEGIN
        SELECT
            @OpenTwoThousand = TwoThousand, @OpenThousand = Thousand, @OpenFiveHundred = FiveHundred,
            @OpenTwoHundred = TwoHundred, @OpenHundred = Hundred, @OpenFifty = Fifty,
            @OpenTwenty = Twenty, @OpenTen = Ten, @OpenFive = Five, @OpenTwo = Two, @OpenOne = One
        FROM dbo.DayCloseV2Denomination
        WHERE DayCloseId = @prevId;
    END

    IF ISNULL(@OpenTwoThousand, 0) < 0 OR ISNULL(@OpenThousand, 0) < 0 OR ISNULL(@OpenFiveHundred, 0) < 0
        OR ISNULL(@OpenTwoHundred, 0) < 0 OR ISNULL(@OpenHundred, 0) < 0 OR ISNULL(@OpenFifty, 0) < 0
        OR ISNULL(@OpenTwenty, 0) < 0 OR ISNULL(@OpenTen, 0) < 0 OR ISNULL(@OpenFive, 0) < 0
        OR ISNULL(@OpenTwo, 0) < 0 OR ISNULL(@OpenOne, 0) < 0
    BEGIN
        SET @Message = 'Opening denomination counts cannot be negative.';
        RETURN;
    END

    SET @openTotal =
        (ISNULL(@OpenTwoThousand, 0) * 2000) + (ISNULL(@OpenThousand, 0) * 1000) + (ISNULL(@OpenFiveHundred, 0) * 500)
        + (ISNULL(@OpenTwoHundred, 0) * 200) + (ISNULL(@OpenHundred, 0) * 100) + (ISNULL(@OpenFifty, 0) * 50)
        + (ISNULL(@OpenTwenty, 0) * 20) + (ISNULL(@OpenTen, 0) * 10) + (ISNULL(@OpenFive, 0) * 5)
        + (ISNULL(@OpenTwo, 0) * 2) + ISNULL(@OpenOne, 0);
    IF ABS(@openTotal - @opening) > 0.009
    BEGIN
        SET @Message = 'Opening denomination total does not match Opening Cash.';
        RETURN;
    END

    SET @actualCash =
        (ISNULL(@TwoThousand, 0) * 2000) + (ISNULL(@Thousand, 0) * 1000) + (ISNULL(@FiveHundred, 0) * 500)
        + (ISNULL(@TwoHundred, 0) * 200) + (ISNULL(@Hundred, 0) * 100) + (ISNULL(@Fifty, 0) * 50)
        + (ISNULL(@Twenty, 0) * 20) + (ISNULL(@Ten, 0) * 10) + (ISNULL(@Five, 0) * 5)
        + (ISNULL(@Two, 0) * 2) + ISNULL(@One, 0);

    /*
      The totals, denomination movement and link rows all come from @pop, captured once inside the transaction.
      A transaction committed after that capture is not linked to this close, so it becomes a post-close
      transaction of its own business date and is carried into the next close instead of being lost.
    */
    DECLARE @pop TABLE
    (
        SourceTable varchar(20) NOT NULL,
        TransId uniqueidentifier NULL,
        PostedOn datetime NOT NULL,
        OriginalBusinessDate date NOT NULL,
        Channel varchar(10) NOT NULL,
        StoredType nvarchar(10) NULL,
        Direction varchar(3) NOT NULL,
        EntityReference nvarchar(255) NULL,
        EntityType nvarchar(255) NULL,
        Description nvarchar(500) NULL,
        AmountIn decimal(18, 2) NOT NULL,
        AmountOut decimal(18, 2) NOT NULL,
        Role varchar(20) NOT NULL
    );
    DECLARE @move TABLE (FaceValue int PRIMARY KEY, InQty int, OutQty int);

    BEGIN TRY
        BEGIN TRAN;

        IF EXISTS (SELECT 1 FROM dbo.DayCloseV2 WITH (UPDLOCK, HOLDLOCK) WHERE BusinessDate = @BusinessDate AND Status = 'Completed')
        BEGIN
            ROLLBACK TRAN;
            SET @Message = 'This business date is already closed.';
            RETURN;
        END

        SET @cutoff = GETDATE();

        INSERT INTO @pop
            (SourceTable, TransId, PostedOn, OriginalBusinessDate, Channel, StoredType, Direction,
             EntityReference, EntityType, Description, AmountIn, AmountOut, Role)
        SELECT SourceTable, TransId, PostedOn, OriginalBusinessDate, Channel, StoredType, Direction,
            EntityReference, EntityType, Description, AmountIn, AmountOut, Role
        FROM dbo.fn_DayCloseV2_OpenPopulation(@BusinessDate, @cutoff);

        SELECT
            @cashIn = ISNULL(SUM(CASE WHEN Channel = 'CASH' THEN AmountIn ELSE 0 END), 0),
            @cashOut = ISNULL(SUM(CASE WHEN Channel = 'CASH' THEN AmountOut ELSE 0 END), 0),
            @upiIn = ISNULL(SUM(CASE WHEN Channel = 'UPI' THEN AmountIn ELSE 0 END), 0),
            @upiOut = ISNULL(SUM(CASE WHEN Channel = 'UPI' THEN AmountOut ELSE 0 END), 0),
            @cardIn = ISNULL(SUM(CASE WHEN Channel = 'CARD' THEN AmountIn ELSE 0 END), 0),
            @cardOut = ISNULL(SUM(CASE WHEN Channel = 'CARD' THEN AmountOut ELSE 0 END), 0),
            @cashCarriedIn = ISNULL(SUM(CASE WHEN Channel = 'CASH' AND Role = 'CARRIED_FORWARD' THEN AmountIn ELSE 0 END), 0),
            @cashCarriedOut = ISNULL(SUM(CASE WHEN Channel = 'CASH' AND Role = 'CARRIED_FORWARD' THEN AmountOut ELSE 0 END), 0),
            @upiCarriedIn = ISNULL(SUM(CASE WHEN Channel = 'UPI' AND Role = 'CARRIED_FORWARD' THEN AmountIn ELSE 0 END), 0),
            @upiCarriedOut = ISNULL(SUM(CASE WHEN Channel = 'UPI' AND Role = 'CARRIED_FORWARD' THEN AmountOut ELSE 0 END), 0),
            @cardCarriedIn = ISNULL(SUM(CASE WHEN Channel = 'CARD' AND Role = 'CARRIED_FORWARD' THEN AmountIn ELSE 0 END), 0),
            @cardCarriedOut = ISNULL(SUM(CASE WHEN Channel = 'CARD' AND Role = 'CARRIED_FORWARD' THEN AmountOut ELSE 0 END), 0)
        FROM @pop;

        SET @expectedCash = @opening + @cashIn - @cashOut;
        SET @expectedUpi = @upiIn - @upiOut;
        SET @expectedCard = @cardIn - @cardOut;

        IF (ABS(@expectedCash - @actualCash) > 0.009
            OR ABS(@expectedUpi - ISNULL(@ActualUpi, 0)) > 0.009
            OR ABS(@expectedCard - ISNULL(@ActualCard, 0)) > 0.009)
            AND (@Remarks IS NULL OR LTRIM(RTRIM(@Remarks)) = '')
        BEGIN
            ROLLBACK TRAN;
            SET @Message = 'Enter a variance reason. Actual closing does not match the expected total.';
            RETURN;
        END

        INSERT INTO @move (FaceValue, InQty, OutQty)
        SELECT FaceValue,
            SUM(CASE WHEN PayType = 'C' AND Qty > 0 THEN Qty WHEN PayType = 'D' AND Qty < 0 THEN -Qty ELSE 0 END),
            SUM(CASE WHEN PayType = 'C' AND Qty < 0 THEN -Qty WHEN PayType = 'D' AND Qty > 0 THEN Qty ELSE 0 END)
        FROM
        (
            SELECT UPPER(LTRIM(RTRIM(p.StoredType))) AS PayType, v.FaceValue,
                CONVERT(int, ISNULL(TRY_CONVERT(decimal(18, 2), NULLIF(LTRIM(RTRIM(v.Qty)), '')), 0)) AS Qty
            FROM @pop p
            INNER JOIN CashTransactionDenomination d ON d.TransId = p.TransId
            CROSS APPLY (VALUES
                (2000, d.TwoThousand), (1000, d.Thousand), (500, d.FiveHundred), (200, d.TwoHundred),
                (100, d.Hundred), (50, d.Fifty), (20, d.Twenty), (10, d.Ten), (5, d.Five), (2, d.Two), (1, d.One)
            ) v (FaceValue, Qty)
            WHERE p.SourceTable = 'CashTransaction'
        ) s
        GROUP BY FaceValue;

        INSERT INTO dbo.DayCloseV2
            (BusinessDate, OpeningCash, OpeningDefaultCash, ExpectedCashClosing, ActualCashClosing, CashDifference,
             ExpectedUpiNet, ActualUpiNet, UpiDifference, ExpectedCardNet, ActualCardNet, CardDifference,
             ExpectedTotal, ActualTotal, TotalDifference, Status, Remarks, EnteredBy, EnteredOn, TransactionCutoff)
        VALUES
            (@BusinessDate, @opening, @defaultOpening, @expectedCash, @actualCash, @actualCash - @expectedCash,
             @expectedUpi, ISNULL(@ActualUpi, 0), ISNULL(@ActualUpi, 0) - @expectedUpi,
             @expectedCard, ISNULL(@ActualCard, 0), ISNULL(@ActualCard, 0) - @expectedCard,
             @expectedCash + @expectedUpi + @expectedCard,
             @actualCash + ISNULL(@ActualUpi, 0) + ISNULL(@ActualCard, 0),
             (@actualCash + ISNULL(@ActualUpi, 0) + ISNULL(@ActualCard, 0)) - (@expectedCash + @expectedUpi + @expectedCard),
             'Completed', @Remarks, @EnteredBy, GETDATE(), @cutoff);

        SET @closeId = SCOPE_IDENTITY();

        INSERT INTO dbo.DayCloseV2PaymentSummary
            (DayCloseId, PaymentMode, OpeningAmount, TotalIn, TotalOut, ExpectedNet, ActualAmount, Difference, CarriedIn, CarriedOut)
        VALUES
            (@closeId, 'CASH', @opening, @cashIn, @cashOut, @expectedCash, @actualCash, @actualCash - @expectedCash, @cashCarriedIn, @cashCarriedOut),
            (@closeId, 'UPI', 0, @upiIn, @upiOut, @expectedUpi, ISNULL(@ActualUpi, 0), ISNULL(@ActualUpi, 0) - @expectedUpi, @upiCarriedIn, @upiCarriedOut),
            (@closeId, 'CARD', 0, @cardIn, @cardOut, @expectedCard, ISNULL(@ActualCard, 0), ISNULL(@ActualCard, 0) - @expectedCard, @cardCarriedIn, @cardCarriedOut);

        INSERT INTO dbo.DayCloseV2Denomination
            (DayCloseId, TwoThousand, Thousand, FiveHundred, TwoHundred, Hundred, Fifty, Twenty, Ten, Five, Two, One, DenominationTotal)
        VALUES
            (@closeId, ISNULL(@TwoThousand, 0), ISNULL(@Thousand, 0), ISNULL(@FiveHundred, 0), ISNULL(@TwoHundred, 0),
             ISNULL(@Hundred, 0), ISNULL(@Fifty, 0), ISNULL(@Twenty, 0), ISNULL(@Ten, 0), ISNULL(@Five, 0),
             ISNULL(@Two, 0), ISNULL(@One, 0), @actualCash);

        INSERT INTO dbo.DayCloseV2DenominationLine
            (DayCloseId, FaceValue, OpeningQty, InQty, OutQty, ExpectedQty, ActualQty)
        SELECT @closeId, f.FaceValue, f.OpenQty, ISNULL(m.InQty, 0), ISNULL(m.OutQty, 0),
            f.OpenQty + ISNULL(m.InQty, 0) - ISNULL(m.OutQty, 0), f.ActualQty
        FROM (VALUES
            (2000, ISNULL(@OpenTwoThousand, 0), ISNULL(@TwoThousand, 0)),
            (1000, ISNULL(@OpenThousand, 0), ISNULL(@Thousand, 0)),
            (500, ISNULL(@OpenFiveHundred, 0), ISNULL(@FiveHundred, 0)),
            (200, ISNULL(@OpenTwoHundred, 0), ISNULL(@TwoHundred, 0)),
            (100, ISNULL(@OpenHundred, 0), ISNULL(@Hundred, 0)),
            (50, ISNULL(@OpenFifty, 0), ISNULL(@Fifty, 0)),
            (20, ISNULL(@OpenTwenty, 0), ISNULL(@Twenty, 0)),
            (10, ISNULL(@OpenTen, 0), ISNULL(@Ten, 0)),
            (5, ISNULL(@OpenFive, 0), ISNULL(@Five, 0)),
            (2, ISNULL(@OpenTwo, 0), ISNULL(@Two, 0)),
            (1, ISNULL(@OpenOne, 0), ISNULL(@One, 0))
        ) f (FaceValue, OpenQty, ActualQty)
        LEFT JOIN @move m ON m.FaceValue = f.FaceValue;

        INSERT INTO dbo.DayCloseV2Transaction
            (DayCloseId, SourceTable, TransId, Role, OriginalBusinessDate, PostedOn, Channel, StoredType, Direction,
             EntityReference, EntityType, Description, AmountIn, AmountOut)
        SELECT @closeId, SourceTable, TransId, Role, OriginalBusinessDate, PostedOn, Channel, StoredType, Direction,
            EntityReference, EntityType, Description, AmountIn, AmountOut
        FROM @pop
        WHERE TransId IS NOT NULL;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN;
        SET @Result = 0;
        SET @DayCloseId = NULL;
        SET @Message = LEFT('Day Close was not saved. ' + ERROR_MESSAGE(), 400);
        RETURN;
    END CATCH

    SET @DayCloseId = @closeId;
    SET @Result = 1;
    SET @Message = 'Day Close saved.';
END
GO

/*
  Back-fill existing closes. TransactionCutoff = EnteredOn and INCLUDED links are written only for a close whose
  Cash, UPI and Card IN/OUT recomputed from transactions posted up to EnteredOn equal its saved summary exactly.
*/
IF OBJECT_ID('tempdb..#closeCheck') IS NOT NULL DROP TABLE #closeCheck;

SELECT
    h.Id,
    h.BusinessDate,
    h.EnteredOn,
    CASE WHEN
        (SELECT COUNT(*) FROM dbo.DayCloseV2PaymentSummary p WHERE p.DayCloseId = h.Id AND p.PaymentMode IN ('CASH', 'UPI', 'CARD')) = 3
        AND NOT EXISTS
        (
            SELECT 1
            FROM dbo.DayCloseV2PaymentSummary p
            OUTER APPLY
            (
                SELECT ISNULL(SUM(s.AmountIn), 0) AS TotalIn, ISNULL(SUM(s.AmountOut), 0) AS TotalOut
                FROM dbo.fn_DayCloseV2_SourceRows(CONVERT(datetime, h.BusinessDate), DATEADD(day, 1, CONVERT(datetime, h.BusinessDate))) s
                WHERE s.Channel = p.PaymentMode AND s.PostedOn <= h.EnteredOn
            ) r
            WHERE p.DayCloseId = h.Id
              AND p.PaymentMode IN ('CASH', 'UPI', 'CARD')
              AND (r.TotalIn <> p.TotalIn OR r.TotalOut <> p.TotalOut)
        )
    THEN 1 ELSE 0 END AS Reproduces
INTO #closeCheck
FROM dbo.DayCloseV2 h
WHERE h.Status = 'Completed'
  AND h.TransactionCutoff IS NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.DayCloseV2Transaction l WHERE l.DayCloseId = h.Id);

BEGIN TRAN;

UPDATE h
SET TransactionCutoff = h.EnteredOn
FROM dbo.DayCloseV2 h
INNER JOIN #closeCheck c ON c.Id = h.Id AND c.Reproduces = 1
WHERE h.TransactionCutoff IS NULL;

INSERT INTO dbo.DayCloseV2Transaction
    (DayCloseId, SourceTable, TransId, Role, OriginalBusinessDate, PostedOn, Channel, StoredType, Direction,
     EntityReference, EntityType, Description, AmountIn, AmountOut)
SELECT c.Id, s.SourceTable, s.TransId, 'INCLUDED', s.BusinessDate, s.PostedOn, s.Channel, s.StoredType, s.Direction,
    s.EntityReference, s.EntityType, s.Description, s.AmountIn, s.AmountOut
FROM #closeCheck c
CROSS APPLY dbo.fn_DayCloseV2_SourceRows(CONVERT(datetime, c.BusinessDate), DATEADD(day, 1, CONVERT(datetime, c.BusinessDate))) s
WHERE c.Reproduces = 1
  AND s.PostedOn <= c.EnteredOn
  AND s.TransId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.DayCloseV2Transaction l WHERE l.SourceTable = s.SourceTable AND l.TransId = s.TransId);

COMMIT TRAN;

SELECT
    Id AS DayCloseId,
    BusinessDate,
    EnteredOn,
    CASE Reproduces WHEN 1 THEN 'Back-filled (cutoff = EnteredOn)' ELSE 'Skipped - does not reproduce saved snapshot' END AS BackfillResult
FROM #closeCheck
ORDER BY BusinessDate;

DROP TABLE #closeCheck;
GO
