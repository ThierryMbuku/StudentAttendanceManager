CREATE TABLE [dbo].[Users] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]    NVARCHAR (50)  NOT NULL,
    [LastName]     NVARCHAR (50)  NOT NULL,
    [Email]        NVARCHAR (100) NOT NULL,
    [Username]     VARCHAR (50)   NOT NULL,
    [PasswordHash] NVARCHAR (MAX) NOT NULL,
    [CellPhone]    NVARCHAR (50)  NULL,
    [IsAdmin]      BIT            NOT NULL,
    [AddressID]    INT            NOT NULL,
    [SecurityChallenge] VARCHAR(1000) NULL, 
    CONSTRAINT [PK__Users__3214EC27195199F1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK__Users__AddressID__29572725] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Addresses] ([ID])
);

