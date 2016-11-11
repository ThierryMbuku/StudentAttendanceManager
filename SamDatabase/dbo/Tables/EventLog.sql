CREATE TABLE [dbo].[EventLog]
(
	[ID] [int] IDENTITY(1,1) NOT NULL primary key,
	[EventTypeId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UserId] [int] NULL,
	[Severity] [varchar](50) NULL
)