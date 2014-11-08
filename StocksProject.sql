/* Create StocksProject database manually, then run this script */
/* Yerkebulan Tulibergenov, 11/06/14 */
USE [StocksProject]
GO
/*CREATE DATABASE [StocksProject]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'StocksProject', FILENAME = N'C:\Users\User\StocksProject.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'StocksProject_log', FILENAME = N'C:\Users\User\StocksProject_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
*/
ALTER DATABASE [StocksProject] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [StocksProject].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [StocksProject] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [StocksProject] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [StocksProject] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [StocksProject] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [StocksProject] SET ARITHABORT OFF 
GO
ALTER DATABASE [StocksProject] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [StocksProject] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [StocksProject] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [StocksProject] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [StocksProject] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [StocksProject] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [StocksProject] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [StocksProject] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [StocksProject] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [StocksProject] SET  DISABLE_BROKER 
GO
ALTER DATABASE [StocksProject] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [StocksProject] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [StocksProject] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [StocksProject] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [StocksProject] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [StocksProject] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [StocksProject] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [StocksProject] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [StocksProject] SET  MULTI_USER 
GO
ALTER DATABASE [StocksProject] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [StocksProject] SET DB_CHAINING OFF 
GO
ALTER DATABASE [StocksProject] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [StocksProject] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [StocksProject] SET DELAYED_DURABILITY = DISABLED 
GO
USE [StocksProject]
GO
/****** Object:  Table [dbo].[Client]    Script Date: 10/28/2014 9:19:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Client](
	[ClientId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Phone] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED 
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Holding]    Script Date: 10/28/2014 9:19:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Holding](
	[HoldingId] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[StockId] [int] NOT NULL,
	[Quantity] [bigint] NOT NULL,
	[LastChangeDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Holding] PRIMARY KEY CLUSTERED 
(
	[HoldingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stock]    Script Date: 10/28/2014 9:19:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stock](
	[StockId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[CompanyName] [nvarchar](50) NOT NULL,
	[LastPrice] [money] NOT NULL,
 CONSTRAINT [PK_Stock] PRIMARY KEY CLUSTERED 
(
	[StockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Client] ON 

INSERT [dbo].[Client] ([ClientId], [Code], [FirstName], [LastName], [Phone], [Address]) VALUES (1, N'BG', N'Bill', N'Gates', N'(650)1112222', N'111 Vista Blvd, SF, CA')
INSERT [dbo].[Client] ([ClientId], [Code], [FirstName], [LastName], [Phone], [Address]) VALUES (2, N'LP', N'Larry', N'Page', N'(424)6667777', N'25 Wilshire Blvd, LA, CA')
INSERT [dbo].[Client] ([ClientId], [Code], [FirstName], [LastName], [Phone], [Address]) VALUES (3, N'JD', N'John', N'Doe', N'(333)7778888', N'51 Broadway Blvd, Washington, DC')
SET IDENTITY_INSERT [dbo].[Client] OFF
SET IDENTITY_INSERT [dbo].[Holding] ON 

INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (1, 1, 1, 600500300, CAST(N'2014-01-01 00:00:00.000' AS DateTime))
INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (2, 1, 2, 50000, CAST(N'2014-08-01 00:00:00.000' AS DateTime))
INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (3, 1, 3, 10000, CAST(N'2014-01-10 00:00:00.000' AS DateTime))
INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (4, 2, 2, 980500, CAST(N'2014-05-10 00:00:00.000' AS DateTime))
INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (5, 2, 3, 699870, CAST(N'2014-08-06 00:00:00.000' AS DateTime))
INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (6, 3, 1, 25000, CAST(N'2014-10-01 00:00:00.000' AS DateTime))
INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (7, 3, 2, 16000, CAST(N'2014-05-07 00:00:00.000' AS DateTime))
INSERT [dbo].[Holding] ([HoldingId], [ClientId], [StockId], [Quantity], [LastChangeDate]) VALUES (8, 3, 3, 500, CAST(N'2014-10-15 00:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[Holding] OFF
SET IDENTITY_INSERT [dbo].[Stock] ON 

INSERT [dbo].[Stock] ([StockId], [Code], [CompanyName], [LastPrice]) VALUES (1, N'MSFT', N'Microsoft Inc.', 46.2900)
INSERT [dbo].[Stock] ([StockId], [Code], [CompanyName], [LastPrice]) VALUES (2, N'AMZN', N'Amazon.com, Inc.', 295.5900)
INSERT [dbo].[Stock] ([StockId], [Code], [CompanyName], [LastPrice]) VALUES (3, N'TSLA', N'Tesla Motors Inc.', 242.7700)
SET IDENTITY_INSERT [dbo].[Stock] OFF
ALTER TABLE [dbo].[Holding]  WITH CHECK ADD  CONSTRAINT [FK_Holding_Client] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([ClientId])
GO
ALTER TABLE [dbo].[Holding] CHECK CONSTRAINT [FK_Holding_Client]
GO
ALTER TABLE [dbo].[Holding]  WITH CHECK ADD  CONSTRAINT [FK_Holding_Stock] FOREIGN KEY([StockId])
REFERENCES [dbo].[Stock] ([StockId])
GO
ALTER TABLE [dbo].[Holding] CHECK CONSTRAINT [FK_Holding_Stock]
GO
USE [master]
GO
ALTER DATABASE [StocksProject] SET  READ_WRITE 
GO
