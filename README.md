//this is the sp used to get transaction

USE [JWTPayment]
GO
/****** Object:  StoredProcedure [dbo].[GetFilteredTransactions]    Script Date: 7/7/2025 1:12:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[GetFilteredTransactions]
    --@StartDate DATETIME = NULL,
   -- @EndDate DATETIME = NULL,
    @Status NVARCHAR(50) = NULL,
    @UserId NVARCHAR(100) = NULL
AS
BEGIN
    SELECT TransactionId, Amount, Status,Currency, UserId,Timestamp
    FROM Transactions
    WHERE 
	--(@StartDate IS NULL OR CreatedAt >= @StartDate)
      --AND (@EndDate IS NULL OR CreatedAt <= @EndDate)
      (@Status IS NULL OR Status = @Status)
      AND (@UserId IS NULL OR UserId = @UserId)
END


///////////For Individual Payment History////////////

USE [Payment]
GO
/****** Object:  StoredProcedure [dbo].[GetTransactionById]    Script Date: 7/8/2025 10:02:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetTransactionById]
    @UserId nvarchar(70)
AS
BEGIN
    SELECT 
        t.TransactionId,
        t.UserId,
        t.Amount,
        t.Status
    FROM Transactions t
    WHERE t.UserId = @UserId;
END



