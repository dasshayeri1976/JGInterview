-- =============================================
-- Author:		<Dipavali>
-- Create date: <7-Mar-2017>
-- Description:	<Description,,>
-- =============================================

ALTER PROCEDURE [dbo].[AddSubTasks] 
	-- Add the parameters for the stored procedure here
	@TaskId int,
	@Title varchar(400),
	@Description varchar(max),
	@Status int,
	@IsDeleted int,
    @CreatedOn datetime,
	@CreatedBy int,
	@TaskLevel int,
	@ParentTaskId int,
	@InstallId varchar(50),
	@MainParentId int,
	@TaskType int,
	@TaskPriority int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @TaskId=0 
		begin 
			-- Insert statements for procedure here
			insert into dbo.tblTask 
			(Title,[Description],[Status],IsDeleted,CreatedOn,CreatedBy,TaskLevel,ParentTaskId,
			InstallId,MainParentId,TaskType,TaskPriority  )
			values
			(@Title,@Description,@Status,@IsDeleted,@CreatedOn,@CreatedBy,@TaskLevel,@ParentTaskId,
			@InstallId,@MainParentId,@TaskType,@TaskPriority )
		end
	else
		begin

				-- Insert statements for procedure here
			update dbo.tblTask 
			 set Title=@Title,[Description]=@Description,InstallId=@InstallId ,TaskPriority=@TaskPriority,
			 TaskType=@TaskType
			where TaskId=@TaskId
		end 

END


-- =============================================
-- Author:		<Dipavali>
-- Create date: <30-Jan-2017>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetClosedTasks] 
	-- Add the parameters for the stored procedure here
	@userid int,
	@desigid int,
	@search varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @str nvarchar(700)
	set @str = ''
	if @search<>''
		begin
			SELECT a.TaskId,[Description],a.[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],a.InstallId ,ParentTaskId
			from dbo.tblTask as a, tbltaskassignedusers as b,tblInstallUsers as t
			where a.[Status]  in (7,8,9,10,11,12,14) and a.TaskId=b.TaskId and b.UserId=t.Id
			AND  (
			t.FristName LIKE '%'+ @search + '%'  or
			t.LastName LIKE '%'+ @search + '%'  or
			t.Email LIKE '%' + @search +'%'  
			) and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
   else if @userid=0 and @desigid=0
		begin
			SELECT TaskId,[Description],[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],InstallId,ParentTaskId from dbo.tblTask 
			where [Status]  in (7,8,9,10,11,12,14) and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
	else if @userid>0  
		begin
			SELECT a.TaskId,[Description],[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],InstallId ,ParentTaskId
			from dbo.tblTask as a, tbltaskassignedusers as b
			where [Status]  in (7,8,9,10,11,12,14) and a.TaskId=b.TaskId and b.UserId=@userid
			and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
	else if @userid=0 and @desigid>0
		begin
			SELECT a.TaskId,[Description],[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],InstallId,ParentTaskId
			from dbo.tblTask as a, tblTaskdesignations as b 
			where [Status]  in (7,8,9,10,11,12,14) and a.TaskId=b.TaskId and b.DesignationID=@desigid
			and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
END

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetInProgressTasks] 
	-- Add the parameters for the stored procedure here
	@userid int,
	@desigid int,
	@search varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @str nvarchar(700)
	set @str = ''
	if @search<>''
		begin
			SELECT a.TaskId,[Description],a.[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],a.InstallId ,ParentTaskId
			from dbo.tblTask as a, tbltaskassignedusers as b,tblInstallUsers as t
			where a.[Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			AND  (
			t.FristName LIKE '%'+ @search + '%'  or
			t.LastName LIKE '%'+ @search + '%'  or
			t.Email LIKE '%' + @search +'%'  
			) and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
    else if @userid=0 and @desigid=0
		begin
			SELECT TaskId,[Description],[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],InstallId,ParentTaskId from dbo.tblTask 
			where [Status]  in (1,2,3,4) and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
	else if @userid>0  
		begin
			SELECT a.TaskId,[Description],[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],InstallId ,ParentTaskId
			from dbo.tblTask as a, tbltaskassignedusers as b
			where [Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=@userid
			and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
	else if @userid=0 and @desigid>0
		begin
			SELECT a.TaskId,[Description],[Status],convert(Date,DueDate ) as DueDate,
			Title,[Hours],InstallId,ParentTaskId
			from dbo.tblTask as a, tblTaskdesignations as b 
			where [Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.DesignationID=@desigid
			and  tasklevel=1 and parenttaskid is not null
			order by [Status] desc
		end
END

-- =============================================
-- Author:		<Dipavali>
-- Create date: <8-Mar-2017>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[GetTaskUsers] 
	@SearchTerm varchar(15) 	  
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	; WITH Suggestion (label,Category)
	AS
	(
	
	   SELECT  t.InstallId AS AutoSuggest , 'Id#' AS Category 
	   FROM dbo.tblInstallUsers t ,tblTask as a, tbltaskassignedusers as b
	   WHERE 
			a.[Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			and  tasklevel=1 and parenttaskid is not null  
			AND t.InstallId LIKE '%'+ @SearchTerm + '%'   

	   UNION

	   SELECT DISTINCT t.FristName AS AutoSuggest , 'FirstName' AS Category 
	   FROM dbo.tblInstallUsers t ,tblTask as a, tbltaskassignedusers as b
	   WHERE 
			a.[Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			and  tasklevel=1 and parenttaskid is not null  
			AND t.FristName LIKE '%'+ @SearchTerm + '%'   

	   UNION

	   SELECT DISTINCT  t.LastName AS AutoSuggest , 'LastName' AS Category 
	   FROM dbo.tblInstallUsers t ,tblTask as a, tbltaskassignedusers as b
	   WHERE 
			a.[Status]   in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			and  tasklevel=1 and parenttaskid is not null  
			AND t.LastName LIKE '%'+ @SearchTerm + '%'

	   UNION

	   SELECT DISTINCT  t.Email AS AutoSuggest, 'Email' AS Category 
	   from dbo.tblInstallUsers t,tblTask as a, tbltaskassignedusers as b
	   WHERE 
			a.[Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			and  tasklevel=1 and parenttaskid is not null  
			AND t.Email LIKE '%' + @SearchTerm +'%'
   
	   UNION
	   
	   SELECT DISTINCT  t.Phone AS AutoSuggest, 'Phone' AS Category 
	   from dbo.tblInstallUsers t,tblTask as a, tbltaskassignedusers as b
	   WHERE 
			a.[Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			and  tasklevel=1 and parenttaskid is not null  
			AND t.Phone LIKE '%' + @SearchTerm +'%'
   
	   UNION

	   SELECT DISTINCT t.CountryCode AS AutoSuggest , 'CountryCode' AS Category 
	   FROM dbo.tblInstallUsers t,tblTask as a, tbltaskassignedusers as b
	   WHERE 
			a.[Status] in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			and  tasklevel=1 and parenttaskid is not null  
			AND t.CountryCode LIKE '%' + @SearchTerm + '%'

	   UNION

	   SELECT DISTINCT t.Zip AS AutoSuggest , 'Zip' AS Category 
	   FROM dbo.tblInstallUsers t,tblTask as a, tbltaskassignedusers as b
	   WHERE 
			a.[Status]  in (1,2,3,4) and a.TaskId=b.TaskId and b.UserId=t.Id
			and  tasklevel=1 and parenttaskid is not null  
			AND t.Zip LIKE '%' + @SearchTerm + '%'

	)

	SELECT * FROM Suggestion s ORDER BY s.Category DESC, s.label

END



-- =============================================
-- Author:		Yogesh Keraliya
-- Create date: 04/07/2016
-- Description:	Load all sub tasks of a task.
-- =============================================
-- usp_GetSubTasks 115, 1, 'Description DESC'
ALTER PROCEDURE [dbo].[usp_GetSubTasks] 
(
	@TaskId INT,
	@Admin BIT,
	@SortExpression	VARCHAR(250) = 'CreatedOn DESC',
	@searchterm  as varchar(300),
	@OpenStatus		TINYINT = 1,
    @RequestedStatus	TINYINT = 2,
    @AssignedStatus	TINYINT = 3,
    @InProgressStatus	TINYINT = 4,
    @PendingStatus	TINYINT = 5,
    @ReOpenedStatus	TINYINT = 6,
    @ClosedStatus	TINYINT = 7,
    @SpecsInProgressStatus	TINYINT = 8,
    @DeletedStatus	TINYINT = 9
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
if @searchterm = '' 
	begin
		;WITH 
		Tasklist AS
		(	


			SELECT
				Tasks.*,
				(SELECT TOP 1 EstimatedHours 
					FROM [TaskApprovalsView] TaskApprovals 
					WHERE Tasks.TaskId = TaskApprovals.TaskId AND TaskApprovals.IsAdminOrITLead = 1) AS AdminOrITLeadEstimatedHours,
				(SELECT TOP 1 EstimatedHours 
					FROM [TaskApprovalsView] TaskApprovals 
					WHERE Tasks.TaskId = TaskApprovals.TaskId AND TaskApprovals.IsAdminOrITLead = 0) AS UserEstimatedHours,
				Row_number() OVER
				(
					ORDER BY
						CASE WHEN @SortExpression = 'InstallId DESC' THEN Tasks.InstallId END DESC,
						CASE WHEN @SortExpression = 'InstallId ASC' THEN Tasks.InstallId END ASC,
						CASE WHEN @SortExpression = 'TaskId DESC' THEN Tasks.TaskId END DESC,
						CASE WHEN @SortExpression = 'TaskId ASC' THEN Tasks.TaskId END ASC,
						CASE WHEN @SortExpression = 'Title DESC' THEN Tasks.Title END DESC,
						CASE WHEN @SortExpression = 'Title ASC' THEN Tasks.Title END ASC,
						CASE WHEN @SortExpression = 'Description DESC' THEN Tasks.Description END DESC,
						CASE WHEN @SortExpression = 'Description ASC' THEN Tasks.Description END ASC,
						CASE WHEN @SortExpression = 'TaskDesignations DESC' THEN Tasks.TaskDesignations END DESC,
						CASE WHEN @SortExpression = 'TaskDesignations ASC' THEN Tasks.TaskDesignations END ASC,
						CASE WHEN @SortExpression = 'TaskAssignedUsers DESC' THEN Tasks.TaskAssignedUsers END DESC,
						CASE WHEN @SortExpression = 'TaskAssignedUsers ASC' THEN Tasks.TaskAssignedUsers END ASC,
						CASE WHEN @SortExpression = 'Status ASC' THEN Tasks.StatusOrder END ASC,
						CASE WHEN @SortExpression = 'Status DESC' THEN Tasks.StatusOrder END DESC,
						CASE WHEN @SortExpression = 'CreatedOn DESC' THEN Tasks.CreatedOn END DESC,
						CASE WHEN @SortExpression = 'CreatedOn ASC' THEN Tasks.CreatedOn END ASC
				) AS RowNo_Order
			FROM
				(
					SELECT 
						Tasks.*,
						CASE Tasks.[Status]
							WHEN @AssignedStatus THEN 1
							WHEN @RequestedStatus THEN 1

							WHEN @InProgressStatus THEN 2
							WHEN @PendingStatus THEN 2
							WHEN @ReOpenedStatus THEN 2

							WHEN @OpenStatus THEN 
								CASE 
									WHEN ISNULL([TaskPriority],'') <> '' THEN 3
									ELSE 4
								END

							WHEN @SpecsInProgressStatus THEN 4

							WHEN @ClosedStatus THEN 5

							WHEN @DeletedStatus THEN 6

							ELSE 7

						END AS StatusOrder,
						TaskApprovals.Id AS TaskApprovalId,
						TaskApprovals.EstimatedHours AS TaskApprovalEstimatedHours,
						TaskApprovals.Description AS TaskApprovalDescription,
						TaskApprovals.UserId AS TaskApprovalUserId,
						TaskApprovals.IsInstallUser AS TaskApprovalIsInstallUser
					FROM 
						[TaskListView] Tasks 
							LEFT JOIN [TaskApprovalsView] TaskApprovals ON Tasks.TaskId = TaskApprovals.TaskId AND TaskApprovals.IsAdminOrITLead = @Admin
					WHERE
						Tasks.ParentTaskId = @TaskId 
						-- condition added by DP 23-jan-17 ---
						and Tasks.TaskLevel=1
				) Tasks
			)
			-- get records
			SELECT * 
			FROM Tasklist 
			ORDER BY RowNo_Order
	end
else
	begin
		;WITH 
		Tasklist AS
		(	


			SELECT
				Tasks.*,
				(SELECT TOP 1 EstimatedHours 
					FROM [TaskApprovalsView] TaskApprovals 
					WHERE Tasks.TaskId = TaskApprovals.TaskId AND TaskApprovals.IsAdminOrITLead = 1) AS AdminOrITLeadEstimatedHours,
				(SELECT TOP 1 EstimatedHours 
					FROM [TaskApprovalsView] TaskApprovals 
					WHERE Tasks.TaskId = TaskApprovals.TaskId AND TaskApprovals.IsAdminOrITLead = 0) AS UserEstimatedHours,
				Row_number() OVER
				(
					ORDER BY
						CASE WHEN @SortExpression = 'InstallId DESC' THEN Tasks.InstallId END DESC,
						CASE WHEN @SortExpression = 'InstallId ASC' THEN Tasks.InstallId END ASC,
						CASE WHEN @SortExpression = 'TaskId DESC' THEN Tasks.TaskId END DESC,
						CASE WHEN @SortExpression = 'TaskId ASC' THEN Tasks.TaskId END ASC,
						CASE WHEN @SortExpression = 'Title DESC' THEN Tasks.Title END DESC,
						CASE WHEN @SortExpression = 'Title ASC' THEN Tasks.Title END ASC,
						CASE WHEN @SortExpression = 'Description DESC' THEN Tasks.Description END DESC,
						CASE WHEN @SortExpression = 'Description ASC' THEN Tasks.Description END ASC,
						CASE WHEN @SortExpression = 'TaskDesignations DESC' THEN Tasks.TaskDesignations END DESC,
						CASE WHEN @SortExpression = 'TaskDesignations ASC' THEN Tasks.TaskDesignations END ASC,
						CASE WHEN @SortExpression = 'TaskAssignedUsers DESC' THEN Tasks.TaskAssignedUsers END DESC,
						CASE WHEN @SortExpression = 'TaskAssignedUsers ASC' THEN Tasks.TaskAssignedUsers END ASC,
						CASE WHEN @SortExpression = 'Status ASC' THEN Tasks.StatusOrder END ASC,
						CASE WHEN @SortExpression = 'Status DESC' THEN Tasks.StatusOrder END DESC,
						CASE WHEN @SortExpression = 'CreatedOn DESC' THEN Tasks.CreatedOn END DESC,
						CASE WHEN @SortExpression = 'CreatedOn ASC' THEN Tasks.CreatedOn END ASC
				) AS RowNo_Order
			FROM
				(
					SELECT 
						Tasks.*,
						CASE Tasks.[Status]
							WHEN @AssignedStatus THEN 1
							WHEN @RequestedStatus THEN 1

							WHEN @InProgressStatus THEN 2
							WHEN @PendingStatus THEN 2
							WHEN @ReOpenedStatus THEN 2

							WHEN @OpenStatus THEN 
								CASE 
									WHEN ISNULL([TaskPriority],'') <> '' THEN 3
									ELSE 4
								END

							WHEN @SpecsInProgressStatus THEN 4

							WHEN @ClosedStatus THEN 5

							WHEN @DeletedStatus THEN 6

							ELSE 7

						END AS StatusOrder,
						TaskApprovals.Id AS TaskApprovalId,
						TaskApprovals.EstimatedHours AS TaskApprovalEstimatedHours,
						TaskApprovals.Description AS TaskApprovalDescription,
						TaskApprovals.UserId AS TaskApprovalUserId,
						TaskApprovals.IsInstallUser AS TaskApprovalIsInstallUser
					FROM 
						tblinstallusers as a,
						[TaskListView] Tasks 
							LEFT JOIN [TaskApprovalsView] TaskApprovals ON Tasks.TaskId = TaskApprovals.TaskId AND TaskApprovals.IsAdminOrITLead = @Admin
							left join [tbltaskassignedusers] t on Tasks.TaskId=t.TaskId 
					WHERE
						Tasks.ParentTaskId = @TaskId 
						and a.Id=t.UserId and a.Fristname like '%'+@searchterm+'%'
						-- condition added by DP 23-jan-17 ---
						and Tasks.TaskLevel=1
				) Tasks
			)
			-- get records
			SELECT * 
			FROM Tasklist 
			ORDER BY RowNo_Order


	end

END




