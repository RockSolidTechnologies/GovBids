/****** Object:  Table [dbo].[Bid]    Script Date: 6/6/2013 4:28:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bid]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Bid](
	[BidID] [int] NOT NULL,
	[ItemID] [int] NULL,
	[Title] [varchar](4000) NULL,
	[Agency] [varchar](250) NULL,
	[Category] [varchar](250) NULL,
	[Location] [varchar](250) NULL,
	[DatePosted] [datetime2](7) NULL,
	[DateOpened] [datetime2](7) NULL,
	[PreBidDate] [datetime2](7) NULL,
	[SpecificationsDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Bid] PRIMARY KEY CLUSTERED 
(
	[BidID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO


