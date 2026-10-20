/*
  Cash transaction narration.

  Requires 2026_09_29_day_close_v2_reconciliation.sql (replaces its Proc_DayCloseV2_GetPaymentLedger).
  Adds CashTransaction.Narration nvarchar(250) NULL. Existing rows stay NULL.
  SaveTransactionCashPayment and SaveTransactionCashReceiptPayment copy the narration already saved
  on CashRequest.Reason / CashReceipt.Reason for the same @RequestId. Parameters are unchanged,
  so older EXEs keep working. Amount, Type, EntityId, EntitySource and denominations are unchanged.
  The Day Close ledger falls back to CashRequest.Reason / CashReceipt.Reason (joined on EntityId)
  when Narration is NULL, so rows saved before this script also show their reason.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;

IF DB_NAME() IN ('master', 'model', 'msdb', 'tempdb')
BEGIN
    RAISERROR('Refusing to change a system database.', 16, 1);
    RETURN;
END
GO

IF COL_LENGTH('dbo.CashTransaction', 'Narration') IS NULL
    ALTER TABLE dbo.CashTransaction ADD Narration nvarchar(250) NULL;
GO

ALTER proc [dbo].[SaveTransactionCashPayment]
(
	@RequestId varchar(100)=null,
	@Amount nvarchar(100)=null,
	@oAmount nvarchar(100)=null,
	@PaymentMode nvarchar(50)=null,
	@ChequeNo nvarchar(255)=null,
	@ChequeDate nvarchar(100)=null,
	@BankName nvarchar(255)=null,
	@FromAccount nvarchar(255)=null,
	@ToAccount nvarchar(255)=null,
	@TwoThousand nvarchar(255)=null,	
	@Thousand nvarchar(255)=null,
	@FiveHundred nvarchar(255)=null,
	@Hundred nvarchar(255)=null,
	@Fifty nvarchar(255)=null,
	@Twenty nvarchar(255)=null,
	@Ten nvarchar(255)=null,
	@Five nvarchar(255)=null,
	@Coin nvarchar(255)=null,
	@one nvarchar(255)=null,
	@CashPaidBy int =null,
	@Result varchar(500) out,
    @AccountHead nvarchar(250)=null
)

As

Begin

    Declare @Type Varchar(1)
	Declare @EntitySource Varchar(25)
    Declare @Narration nvarchar(250)
    if @RequestId like '%RCT%'
	Begin
	   Set @Type = 'C'
	   Set @EntitySource = 'CASH RECEIPT' 
	   SET @PaymentMode='Cash'
	   Select top 1 @Narration = LEFT(NULLIF(LTRIM(RTRIM(Reason)), ''), 250) from CashReceipt where ReceiptId=@RequestId
	End
	Else
	Begin
	   Set @Type = 'D'
	    Set @EntitySource = 'CASH REQUEST' 
	   Select top 1 @Narration = LEFT(NULLIF(LTRIM(RTRIM(Reason)), ''), 250) from CashRequest where RequestId=@RequestId
	End
	Update CashRequest set CashPaidBy=@CashPaidBy,CashPaidOn=GETDATE(),PaymentMode=@PaymentMode,ChequeNo=@ChequeNo,ChequeDate=@ChequeDate,

	BankName=@BankName,FromAccount=@FromAccount,ToAccount=@ToAccount,Status='Closed' where RequestId=@RequestId



	if(@PaymentMode='Cash')

	Begin
		Insert into CashTransaction(TransId,EntityId,EntitySource,TransDate,Amount,Type,Narration) values (NEWID(),@RequestId,@EntitySource+'-'+ISNULL(@AccountHead,''),GETDATE(),@Amount,@Type,@Narration)

	Declare @TransId varchar(max)

	set @TransId = (select top 1 TransId from CashTransaction  order by TransDate desc)

		Insert into CashTransactionDenomination(Id,TransId,TwoThousand,Thousand,FiveHundred,Hundred,Fifty,Twenty,Ten,Five,Two,One,Amount) values

		(NEWID(),@TransId,@TwoThousand,@Thousand,@FiveHundred,@Hundred,@Fifty,@Twenty,@Ten,@Five,@Coin,@one,@oAmount)
		
	End

	Else if(@PaymentMode='Cheque')
	Begin

	  insert into CardTransactions
              (TransId,Entity,EntitySource,TransDate,Amount,Type,Mode,ChequeNumber,InstrumentDate,InstrumentBank,Reference)values(NEWID(),@RequestId,'CASH REQUEST' ,getdate(),@Amount,'D',@PaymentMode,@ChequeNo,
			  @ChequeDate,@BankName,'CardPayment')
    set @TransId=NEWID()
	End

	Else if(@PaymentMode='Transfer')
	Begin
	insert into CardTransactions
              (TransId,Entity,EntitySource,TransDate,Amount,Type,Mode,InstrumentDate,InstrumentBank,InstrumentSource,Reference)values(NEWID(),@RequestId,'CASH REQUEST' ,getdate(),@Amount,'D',
			  @PaymentMode,GETDATE(),@FromAccount,@ToAccount,'Transfer')
    set @TransId=NEWID()

	End



	set @Result =@TransId

    

End
GO

ALTER proc [dbo].[SaveTransactionCashReceiptPayment]
(
	@RequestId Varchar(50),
	@Amount nvarchar(100),
	@PaymentMode nvarchar(50),
	--@ToAccount nvarchar(255)=null,	
	@TwoThousand nvarchar(255)=null,
	@Thousand nvarchar(255)=null,
	@FiveHundred nvarchar(255)=null,
	@Hundred nvarchar(255)=null,
	@Fifty nvarchar(255)=null,
	@Twenty nvarchar(255)=null,
	@Ten nvarchar(255)=null,
	@Five nvarchar(255)=null,
	@Coin nvarchar(255)=null,
	@one nvarchar(255)=null,
	@oamount nvarchar(255)=null,
	@Result varchar(200) out
	

)

As

Begin

	Declare @Narration nvarchar(250)
	Select top 1 @Narration = LEFT(NULLIF(LTRIM(RTRIM(Reason)), ''), 250) from CashReceipt where ReceiptId=@RequestId

	Insert into CashTransaction(TransId,EntityId,EntitySource,TransDate,Amount,Type,Narration) values 
	(NEWID(),@RequestId,'CASH RECEIPT',GETDATE(),@oamount,'C',@Narration)

	Declare @TransId varchar(max)

	set @TransId = (select top 1 TransId from CashTransaction  order by TransDate desc)

	if(@PaymentMode='Cash')

	Begin

		Insert into CashTransactionDenomination(Id,TransId,TwoThousand,Thousand,FiveHundred,Hundred,Fifty,Twenty,Ten,Five,Two,One,Amount) values

		(NEWID(),@TransId,@TwoThousand,@Thousand,@FiveHundred,@Hundred,@Fifty,@Twenty,@Ten,@Five,@Coin,@one,@Amount)
		
		
		
insert into Receipt (TransactionId,Transdate,EntityId,CustomerName,Amount,Mode) 
   select c.TransId,c.TransDate,c.EntityId,q.ReceiptFrom+'-'+q.AccountHead,c.Amount,'Cash' from CashTransaction c join CashReceipt q 
   on c.EntityId=q.ReceiptId
   where TransId=@transid
		
	End
	set @Result = @TransId;

    

End
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

    DECLARE @start datetime;
    DECLARE @end datetime;
    SET @start = @BusinessDate;
    SET @end = DATEADD(day, 1, @BusinessDate);
    IF @Filter IS NULL OR @Filter = '' SET @Filter = 'ALL';
    IF @OpeningCash IS NULL SET @OpeningCash = 0;

    ;WITH raw AS
    (
        SELECT
            c.TransDate AS TransTime,
            'CASH' AS Channel,
            CASE WHEN UPPER(c.Type) = 'D' THEN 'OUT' ELSE 'IN' END AS Direction,
            c.EntityId AS EntityReference,
            c.EntitySource AS EntityType,
            COALESCE(NULLIF(LTRIM(RTRIM(c.Narration)), ''), n.Narration, q.customername, '') AS Description,
            CASE WHEN UPPER(c.Type) = 'D' THEN CONVERT(decimal(18, 2), 0) ELSE CONVERT(decimal(18, 2), REPLACE(c.Amount, ',', '')) END AS AmountIn,
            CASE WHEN UPPER(c.Type) = 'D' THEN CONVERT(decimal(18, 2), REPLACE(c.Amount, ',', '')) ELSE CONVERT(decimal(18, 2), 0) END AS AmountOut
        FROM CashTransaction c
        LEFT JOIN QuotationEstimation q ON q.Estimationid = c.EntityId
        OUTER APPLY
        (
            SELECT TOP 1 LEFT(NULLIF(LTRIM(RTRIM(r.Reason)), ''), 250) AS Narration
            FROM
            (
                SELECT cr.Reason FROM CashRequest cr WHERE c.EntitySource LIKE 'CASH REQUEST%' AND cr.RequestId = c.EntityId
                UNION ALL
                SELECT rc.Reason FROM CashReceipt rc WHERE c.EntitySource LIKE 'CASH RECEIPT%' AND rc.ReceiptId = c.EntityId
            ) r
        ) n
        WHERE c.TransDate >= @start AND c.TransDate < @end

        UNION ALL

        SELECT
            t.TransDate,
            CASE WHEN t.Mode = 'UPI' THEN 'UPI' ELSE 'CARD' END,
            CASE WHEN t.Type = 'D' THEN 'OUT' ELSE 'IN' END,
            t.Entity,
            t.EntitySource,
            COALESCE(
                q.customername,
                n.Narration + CASE WHEN NULLIF(LTRIM(RTRIM(t.Reference)), '') IS NULL THEN '' ELSE ' (Ref ' + LTRIM(RTRIM(t.Reference)) + ')' END,
                t.Reference,
                ''),
            CASE WHEN t.Type = 'D' THEN CONVERT(decimal(18, 2), 0) ELSE CONVERT(decimal(18, 2), t.Amount) END,
            CASE WHEN t.Type = 'D' THEN CONVERT(decimal(18, 2), t.Amount) ELSE CONVERT(decimal(18, 2), 0) END
        FROM CardTransactions t
        LEFT JOIN QuotationEstimation q ON q.Estimationid = t.Entity
        OUTER APPLY
        (
            SELECT TOP 1 LEFT(NULLIF(LTRIM(RTRIM(r.Reason)), ''), 250) AS Narration
            FROM
            (
                SELECT cr.Reason FROM CashRequest cr WHERE t.EntitySource LIKE 'CASH REQUEST%' AND cr.RequestId = t.Entity
                UNION ALL
                SELECT rc.Reason FROM CashReceipt rc WHERE t.EntitySource LIKE 'CASH RECEIPT%' AND rc.ReceiptId = t.Entity
            ) r
        ) n
        WHERE t.TransDate >= @start AND t.TransDate < @end
          AND t.Mode IN ('UPI', 'Card')
    ),
    numbered AS
    (
        SELECT raw.*, ROW_NUMBER() OVER (ORDER BY TransTime, Channel, EntityReference) AS Seq
        FROM raw
    ),
    balanced AS
    (
        SELECT numbered.*,
            @OpeningCash + SUM(CASE WHEN Channel = 'CASH' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY TransTime, Seq ROWS UNBOUNDED PRECEDING) AS CashRunning,
            SUM(CASE WHEN Channel = 'UPI' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY TransTime, Seq ROWS UNBOUNDED PRECEDING) AS UpiRunning,
            SUM(CASE WHEN Channel = 'CARD' THEN AmountIn - AmountOut ELSE 0 END)
                OVER (ORDER BY TransTime, Seq ROWS UNBOUNDED PRECEDING) AS CardRunning
        FROM numbered
    )
    SELECT
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
    ORDER BY TransTime, Seq;
END
GO
