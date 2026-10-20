/*
  Payment list: show unpaid cash bills in both directions.
  Positive balance remains due. Negative balance is an unpaid refund.
  A settled bill has balance 0 and stays hidden.
*/
ALTER PROCEDURE dbo.searchpay  
(  
 @firstname VarChar(100)=null,  
 @firstvale VarChar(100)=null,  
 @secondname VarChar(100)=null,  
 @secondvalue VarChar(100)=null,  
 @thirdname VarChar(100)=null,  
 @thirdvalue VarChar(100)=null,  
 @UserId VarChar(100)=null,  
 @Role VarChar(100)=null  
)  
AS  
Begin  
 Declare @SQL varchar(max)  
 Declare @SQLval varchar(max)  
   
if(@Role='Emp')  
 Begin  
 IF ISDATE(CONVERT( VARCHAR(30), @secondvalue ,110)) = 1  
 Begin  
   SELECT @SQL = 'select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
   where  ((Status=''Estimation Completed'' or Status=''Return Complete'') or Status=''Return Complete'') and Paid=''0'' and Paymentmode=''Cash Bill''   and  cast (Balance as decimal)<>0  and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
   (SELECT CONVERT(char(10), '+@secondname+',126)) = (SELECT CONVERT(char(10), '''+@secondvalue+''',126)) and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0   
   union all  
   select Estimationid,,Quotationid,GrnandTotal as Amount from QuotationEstimation  
   where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0'' and Paymentmode=''Partial Credit Bill''  and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
   (SELECT CONVERT(char(10), '+@secondname+',126)) = (SELECT CONVERT(char(10), '''+@secondvalue+''',126)) and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '    
 End  
else if(@secondvalue!='' and @secondvalue!='Today' and @secondvalue!='Yesterday')  
 Begin  
 SET @SQLval=(SELECT dbo.Datevaluefunction(@secondvalue))  
     SELECT @SQL = 'select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
   where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0'' and Paymentmode=''Cash Bill''   and  cast (Balance as decimal)<>0 and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and '  
   +@secondname+' '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' )  and iscancel=0  
   union all  
   select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
   where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0'' and Paymentmode=''Partial Credit Bill'' and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and '  
   +@secondname+' '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '    
 End  
Else if(@secondvalue='Today' or @secondvalue='Yesterday')  
 Begin  
 SET @SQLval=(SELECT dbo.Datevaluefunction(@secondvalue))  
   SELECT @SQL = 'select Estimationid ,Quotationid,GrnandTotal as Amount from QuotationEstimation  
   where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0''  and Paymentmode=''Cash Bill''   and  cast (Balance as decimal)<>0  and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
   (SELECT CONVERT(char(10), '+@secondname+',126)) = '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' )  and iscancel=0  
   union all  
   select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
   where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0''  and Paymentmode=''Partial Credit Bill''   and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
   (SELECT CONVERT(char(10), '+@secondname+',126)) = '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '      
  
  End  
     
Else  
 Begin  
   SELECT @SQL = 'select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
   where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0'' and Paymentmode=''Cash Bill''  and cast (Balance as decimal)<>0  and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
   '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0   
   union all  
   select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
   where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0'' and Paymentmode=''Partial Credit Bill''    and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
     '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '      
 End  
   --print  @SQL  
   
End  
 Else if(@Role='Admin')  
 IF ISDATE(CONVERT( VARCHAR(30), @secondvalue ,110)) = 1  
 Begin  
   SELECT @SQL = 'select Estimationid ,Quotationid,GrnandTotal as Amount from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'')  and Paid=''0'' and Paymentmode=''Cash Bill''   and  cast (Balance as decimal)<>0   and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and         (SELECT CONVERT(char(10), '+@secondname+',126)) = (SELECT CONVERT(char(10), '''+@secondvalue+''',126))and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' )  and iscancel=0  
      union all  
      select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'')  and Paid=''0'' and Paymentmode=''Partial Credit Bill''    and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
      (SELECT CONVERT(char(10), '+@secondname+',126)) = (SELECT CONVERT(char(10), '''+@secondvalue+''',126))and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '     
 End  
  
 else if(@secondvalue!='' and @secondvalue!='Today' and @secondvalue!='Yesterday')  
  Begin  
    SET @SQLval=(SELECT dbo.Datevaluefunction(@secondvalue))  
        SELECT @SQL = 'select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'')  and Paid=''0'' and Paymentmode=''Cash Bill''  and  cast (Balance as decimal)<>0   and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and '  
      +@secondname+' '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' )  and iscancel=0  
      union all  
      select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'')  and Paid=''0'' and Paymentmode=''Partial Credit Bill''  and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and '  
      +@secondname+' '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '    
  End  
  Else if(@secondvalue='Today' or @secondvalue='Yesterday')  
  Begin  
    SET @SQLval=(SELECT dbo.Datevaluefunction(@secondvalue))  
   SELECT @SQL = 'select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0''  and Paymentmode=''Cash Bill''    and  cast (Balance as decimal)<>0   and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   

      (SELECT CONVERT(char(10), '+@secondname+',126)) = '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' )  and iscancel=0  
      union all  
      select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0''  and Paymentmode=''Partial Credit Bill''    and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
      (SELECT CONVERT(char(10), '+@secondname+',126)) = '+@SQLval+'  and  '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '      
  
   End  
     
  Else  
   Begin  
      SELECT @SQL = 'select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0''  and Paymentmode=''Cash Bill''    and  cast (Balance as decimal)<>0   and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   

        '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0   
      union all  
      select Estimationid  as ''Est No'',Quotationid  as ''Quo No'',GrnandTotal as ''Amount'' from QuotationEstimation  
      where  (Status=''Estimation Completed'' or Status=''Return Complete'') and Paid=''0''  and Paymentmode=''Partial Credit Bill''  and '+@firstname+'=COALESCE( (select NULLIF('''+@firstvale+''','''')),'+@firstname+' ) and   
       '+@thirdname+'=COALESCE( (select NULLIF('''+@thirdvalue+''','''')),'+@thirdname+' ) and iscancel=0 '      
  End  
 print @SQL  
 Exec ( @SQL)  
End



