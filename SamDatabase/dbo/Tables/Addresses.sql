CREATE TABLE [dbo].[Addresses] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [ComplexNumber] INT           NOT NULL,
    [Street]        NVARCHAR (50) NOT NULL,
    [Suburb]        NVARCHAR (50) NOT NULL,
    [City]          NVARCHAR (50) NOT NULL,
    [PostalCode]    NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

