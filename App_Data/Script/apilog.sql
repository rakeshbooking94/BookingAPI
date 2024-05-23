USE [master]
GO
/****** Object:  Database [BE_ServiceAPI]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE DATABASE [BE_ServiceAPI]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TravillioTest', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\BE_ServiceAPI.mdf' , SIZE = 63313792KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TravillioTest_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\BE_ServiceAPI_log.ldf' , SIZE = 14377984KB , MAXSIZE = 2048GB , FILEGROWTH = 10240KB )
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BE_ServiceAPI].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BE_ServiceAPI] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET ARITHABORT OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BE_ServiceAPI] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BE_ServiceAPI] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BE_ServiceAPI] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BE_ServiceAPI] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BE_ServiceAPI] SET  MULTI_USER 
GO
ALTER DATABASE [BE_ServiceAPI] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BE_ServiceAPI] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BE_ServiceAPI] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BE_ServiceAPI] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [BE_ServiceAPI]
GO
/****** Object:  User [Network Service]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [Network Service] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[Network Service]
GO
/****** Object:  User [IngUser]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [IngUser] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [IngDev]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [IngDev] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [Dev]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [Dev] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [BEXPDEV]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [BEXPDEV] FOR LOGIN [BEXPDEV] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [BEXP\Administrator]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [BEXP\Administrator] FOR LOGIN [BEXP\Administrator] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [being]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [being] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [bedev]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE USER [bedev] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [IngUser]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [IngUser]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [IngUser]
GO
ALTER ROLE [db_owner] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_datareader] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_denydatareader] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_denydatawriter] ADD MEMBER [IngDev]
GO
ALTER ROLE [db_owner] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_datareader] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_denydatareader] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_denydatawriter] ADD MEMBER [BEXP\Administrator]
GO
ALTER ROLE [db_owner] ADD MEMBER [being]
GO
ALTER ROLE [db_owner] ADD MEMBER [bedev]
GO
/****** Object:  Schema [Network Service]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE SCHEMA [Network Service]
GO
/****** Object:  FullTextCatalog [TestFullText]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE FULLTEXT CATALOG [TestFullText] WITH ACCENT_SENSITIVITY = OFF
GO
/****** Object:  FullTextCatalog [Tracknumber]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE FULLTEXT CATALOG [Tracknumber] WITH ACCENT_SENSITIVITY = OFF
GO
/****** Object:  UserDefinedTableType [dbo].[GiataDataType]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE TYPE [dbo].[GiataDataType] AS TABLE(
	[HotelID] [varchar](100) NULL,
	[SupplierID] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[MarkupType]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE TYPE [dbo].[MarkupType] AS TABLE(
	[USRID] [bigint] NULL,
	[SupplierID] [bigint] NULL,
	[MainAgentMarkType] [int] NULL,
	[MainAgentMrkupVal] [float] NULL,
	[SubAgentMrkupType] [int] NULL,
	[SubAgentMrkupVal] [float] NULL,
	[MainAgentID] [bigint] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[OfferChildRelationList]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE TYPE [dbo].[OfferChildRelationList] AS TABLE(
	[offerId] [bigint] NULL,
	[marketId] [bigint] NULL,
	[offerDate] [datetime] NULL,
	[roomId] [bigint] NULL,
	[mealId] [bigint] NULL,
	[countriesId] [nvarchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[SupplementChildRelationList]    Script Date: 5/23/2024 9:06:10 PM ******/
CREATE TYPE [dbo].[SupplementChildRelationList] AS TABLE(
	[suppId] [bigint] NULL,
	[marketID] [bigint] NULL,
	[suppDate] [datetime] NULL,
	[roomID] [bigint] NULL,
	[mealID] [bigint] NULL,
	[CountriesID] [nvarchar](max) NULL
)
GO
/****** Object:  UserDefinedFunction [dbo].[ConvertCommaSeperatedVal]    Script Date: 5/23/2024 9:06:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[ConvertCommaSeperatedVal] (@ID INT, @InStr VARCHAR(MAX))
RETURNS @TempTab TABLE
   (tempid int IDENTITY(1,1) not null, 
   Id int not null,
   Data NVARCHAR(MAX))
AS
BEGIN
    ;-- Ensure input ends with comma
	SET @InStr = REPLACE(@InStr + ',', ',,', ',')
	DECLARE @SP INT
DECLARE @VALUE VARCHAR(1000)
WHILE PATINDEX('%,%', @INSTR ) <> 0 
BEGIN
   SELECT  @SP = PATINDEX('%,%',@INSTR)
   SELECT  @VALUE = LEFT(@INSTR , @SP - 1)
   SELECT  @INSTR = STUFF(@INSTR, 1, @SP, '')
   INSERT INTO @TempTab(Id, Data) VALUES (@ID,@VALUE)
END
	RETURN
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetFolioNo]    Script Date: 5/23/2024 9:06:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetFolioNo]
(
	-- Add the parameters for the function here
	@branchID bigint,@FolioNo varchar(10) 
)
RETURNS varchar(10)
AS
BEGIN
     select  @FolioNo='rt45345'
	 --declare @currentseq int, @branchCode varchar(3),@currentyear int, @srvCode varchar(3),@rptCode varchar(3),@rptSeq int, @rptYear int

  --        set @currentyear=(select Right(Year(getDate()),2))
  --        set  @branchCode= (select usrDtSpInitial  from tblUserDetailSpecific where usrId=@branchId)

		--  select @rptYear=Digitofyear,@srvCode=SrvCode, @rptCode=RptCode, @currentseq=MAX(CurrentSeq), @rptSeq=RptSequence from tblRptSequence_Local where Usrid=@branchId and SrvID=@serviceId and RptId=@reportType group by Digitofyear, SrvID,RptId,Usrid, SrvCode, RptCode, RptSequence 
		--  if(@currentyear>@rptYear)
		--  begin
		--	set @FolioNo=(@branchCode+ CONVERT(varchar(10), @currentyear)+@srvCode+ @rptCode+CONVERT(varchar(10), REPLICATE('0',6-LEN(RTRIM(@rptSeq))) + RTRIM(@rptSeq)))
		--	set @currentseq=@rptSeq+1
		--	update tblRptSequence_Local set CurrentSeq=@currentseq, Digitofyear=@currentyear where Usrid=@branchId
		--  end
		--  else
		--  begin
		--	set @FolioNo=(@branchCode+ CONVERT(varchar(10), @rptYear)+@srvCode+ @rptCode+CONVERT(varchar(10), REPLICATE('0',6-LEN(RTRIM(@currentseq))) + RTRIM(@currentseq)))
		--	set @currentseq=@currentseq+1
		--	update tblRptSequence_Local set CurrentSeq=@currentseq where Usrid=@branchId
	
		--  end
	RETURN  ''

END
GO
/****** Object:  UserDefinedFunction [dbo].[ReplaceASCII]    Script Date: 5/23/2024 9:06:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[ReplaceASCII](@inputString VARCHAR(max))
RETURNS VARCHAR(55)
AS
     BEGIN
         DECLARE @badStrings VARCHAR(100);
         DECLARE @increment INT= 1;
         WHILE @increment <= DATALENGTH(@inputString)
             BEGIN
                 IF(ASCII(SUBSTRING(@inputString, @increment, 1)) > 150)
                     BEGIN
                         SET @badStrings = CHAR(ASCII(SUBSTRING(@inputString, @increment, 1)));
                         SET @inputString = REPLACE(@inputString, @badStrings, '');
                 END;
                 SET @increment = @increment + 1;
             END;
         RETURN @inputString;
     END;

GO
/****** Object:  UserDefinedFunction [dbo].[SplitString]    Script Date: 5/23/2024 9:06:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitString]
(    
      @Input NVARCHAR(MAX),
      @Character CHAR(1)
)
RETURNS @Output TABLE (
      Item NVARCHAR(1000)
)
AS
BEGIN
      DECLARE @StartIndex INT, @EndIndex INT
 
      SET @StartIndex = 1
      IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
      BEGIN
            SET @Input = @Input + @Character
      END
 
      WHILE CHARINDEX(@Character, @Input) > 0
      BEGIN
            SET @EndIndex = CHARINDEX(@Character, @Input)
           
            INSERT INTO @Output(Item)
            SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
           
            SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
      END
 
      RETURN
END
GO
/****** Object:  UserDefinedFunction [dbo].[udf_GetNumeric]    Script Date: 5/23/2024 9:06:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[udf_GetNumeric]
(@strAlphaNumeric VARCHAR(256))
RETURNS VARCHAR(256)
AS
BEGIN
DECLARE @intAlpha INT
SET @intAlpha = PATINDEX('%[^0-9]%', @strAlphaNumeric)
BEGIN
WHILE @intAlpha > 0
BEGIN
SET @strAlphaNumeric = STUFF(@strAlphaNumeric, @intAlpha, 1, '' )
SET @intAlpha = PATINDEX('%[^0-9]%', @strAlphaNumeric )
END
END
RETURN ISNULL(@strAlphaNumeric,0)
END
GO
/****** Object:  Table [dbo].[stubacity]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[stubacity](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[CityId] [bigint] NULL,
	[SupCityId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[cityLogitude] [varchar](100) NULL,
	[cityLatitude] [varchar](100) NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL,
	[cntid] [bigint] NULL,
	[stateID] [bigint] NULL,
	[CityName] [varchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[stubacountry]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[stubacountry](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[cntId] [bigint] NULL,
	[SupCntId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL,
 CONSTRAINT [PK_tblapilog] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_bookDarina]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_bookDarina](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_bookHB]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_bookHB](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_room]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_room](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL,
 CONSTRAINT [PK_tblapilog_room] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_room_iol]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_room_iol](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_search]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_search](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [nvarchar](max) NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL,
 CONSTRAINT [PK_tblapilog_search] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_search_iol]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_search_iol](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [nvarchar](max) NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_searchDarina]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_searchDarina](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilog_searchHB]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilog_searchHB](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilogFailTrans]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilogFailTrans](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [nvarchar](max) NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
 CONSTRAINT [PK_tblapilogFailTrans] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilogFailTransflt]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilogFailTransflt](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [nvarchar](max) NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NULL,
	[preID] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblapilogFailTransflt] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilogflt]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilogflt](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblapilogflt] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblapilogOut]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblapilogOut](
	[logID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[TrackNumber] [nvarchar](500) NULL,
	[logTypeID] [bigint] NULL,
	[logType] [nvarchar](max) NULL,
	[SupplierID] [bigint] NULL,
	[logMsg] [nvarchar](max) NULL,
	[logrequestXML] [nvarchar](max) NULL,
	[logresponseXML] [xml] NULL,
	[logStatus] [tinyint] NULL,
	[logcreatedOn] [datetime2](7) NULL,
	[logcreatedBy] [bigint] NULL,
	[logmodifyOn] [datetime2](7) NULL,
	[logmodifyBy] [bigint] NULL,
	[ip] [nvarchar](500) NOT NULL,
	[preID] [nvarchar](max) NULL,
	[HotelId] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCustomExceptionLogging]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCustomExceptionLogging](
	[Logid] [bigint] IDENTITY(1,1) NOT NULL,
	[ExceptionMsg] [varchar](100) NULL,
	[ExceptionType] [varchar](100) NULL,
	[ExceptionSource] [nvarchar](max) NULL,
	[customerID] [varchar](100) NULL,
	[Logdate] [datetime2](7) NULL,
	[TransID] [nvarchar](max) NULL,
	[MethodName] [varchar](100) NULL,
	[PageName] [varchar](100) NULL,
 CONSTRAINT [PK_tblCustomExceptionLogging] PRIMARY KEY CLUSTERED 
(
	[Logid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblsessionmgmt]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblsessionmgmt](
	[sessionID] [bigint] IDENTITY(1,1) NOT NULL,
	[customerID] [bigint] NULL,
	[sessionValue] [nvarchar](max) NULL,
	[sessionUserCount] [bigint] NULL,
	[sessionStatus] [tinyint] NULL,
	[sessioncreatedOn] [datetime2](7) NULL,
	[sessionexpiryOn] [datetime2](7) NULL,
 CONSTRAINT [PK_tblsessionmgmt] PRIMARY KEY CLUSTERED 
(
	[sessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblsessionmgmt_AM]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblsessionmgmt_AM](
	[AM_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[AM_customerID] [bigint] NULL,
	[AM_SessionId] [nvarchar](500) NULL,
	[AM_SequenceNumber] [int] NULL,
	[AM_SecurityToken] [nvarchar](500) NULL,
	[AM_sessioncreatedOn] [datetime] NULL,
	[AM_sessionexpiryOn] [datetime] NULL,
	[AM_TrackNumber] [nvarchar](500) NULL,
	[AM_sessionStatus] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TblSupplierCity]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblSupplierCity](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[CityId] [bigint] NULL,
	[SupCityId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[cityLogitude] [varchar](100) NULL,
	[cityLatitude] [varchar](100) NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL,
	[cntid] [bigint] NULL,
	[stateID] [bigint] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSupplierCity_rel_new]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSupplierCity_rel_new](
	[id] [bigint] NOT NULL,
	[CityId] [bigint] NULL,
	[SupCityId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[cityLogitude] [varchar](100) NULL,
	[cityLatitude] [varchar](100) NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL,
	[cntid] [bigint] NULL,
	[stateID] [bigint] NULL,
	[CityName] [varchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSupplierCity_rel_new_OLD270224]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSupplierCity_rel_new_OLD270224](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[CityId] [bigint] NULL,
	[SupCityId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[cityLogitude] [varchar](100) NULL,
	[cityLatitude] [varchar](100) NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL,
	[cntid] [bigint] NULL,
	[stateID] [bigint] NULL,
	[CityName] [varchar](250) NULL,
 CONSTRAINT [PK_tblSupplierCity_rel_new] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSupplierCountry_rel_new]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSupplierCountry_rel_new](
	[id] [bigint] NOT NULL,
	[cntId] [bigint] NULL,
	[SupCntId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSupplierCountry_rel_new_OLD270224]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSupplierCountry_rel_new_OLD270224](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[cntId] [bigint] NULL,
	[SupCntId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL,
 CONSTRAINT [PK_tblSupplierCountry_rel_new] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblXMLOutBookingDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblXMLOutBookingDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[outResp] [xml] NULL,
	[customerID] [bigint] NULL,
	[agencyID] [bigint] NULL,
	[bookingStatus] [int] NULL,
	[outstatus] [int] NULL,
	[CreatedBy] [bigint] NULL,
	[Createdatetime] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[ModifyDatetime] [datetime] NULL,
	[PID] [varchar](max) NULL,
	[tracknumber] [varchar](500) NULL,
	[VoucherComments] [varchar](1000) NULL,
	[specialRequest] [varchar](1000) NULL,
	[supplierConfNo] [varchar](500) NULL,
	[SupplierRefNo] [varchar](500) NULL,
	[Errortext] [varchar](500) NULL,
	[outcustid] [bigint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Votcity]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Votcity](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[CityId] [bigint] NULL,
	[SupCityId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[cityLogitude] [varchar](100) NULL,
	[cityLatitude] [varchar](100) NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL,
	[cntid] [bigint] NULL,
	[stateID] [bigint] NULL,
	[CityName] [varchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[votcounty]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[votcounty](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[cntId] [bigint] NULL,
	[SupCntId] [varchar](100) NULL,
	[supId] [bigint] NULL,
	[Createdby] [int] NULL,
	[Createdatetime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[XMLOutDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[XMLOutDetails](
	[XMLOutid] [int] IDENTITY(1,1) NOT NULL,
	[Outcustid] [bigint] NULL,
	[customerid] [bigint] NULL,
	[agencyID] [bigint] NULL,
	[levelid] [int] NULL,
	[supplierID] [bigint] NULL,
	[isinternal] [int] NULL,
	[parentlevel] [int] NULL,
	[BranchID] [bigint] NULL,
	[CustCurrencyID] [int] NULL,
	[CustCurrency] [varchar](50) NULL,
	[DbName] [varchar](100) NULL,
	[outsupplierid] [int] NULL,
	[outagencyid] [int] NULL,
	[status] [int] NULL,
	[outcustname] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[UserTypeList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UserTypeList]
AS  
SELECT x.usrtypId,y.usrtypLclName,x.usrtypParentId
FROM tblUserTypeMaster x Inner Join tblUserTypeMaster_Locale y on x.usrtypId=y.usrtypId and y.cultID=1
GO
/****** Object:  View [dbo].[View_SaleReport]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[View_SaleReport] AS
select 
MA_CCY AS MainAgentCurrency,
SA_CCY AS SubAgentCurrency,
rmSuplCcy AS SuupplierCurrency,
rmNytPrCharge as RoomPrice,
rmSuplBuyingRate AS SupplierBuyngRate,
rmNytPrCharge*rmSuplBuyingRate MainBuyngRate,
Case when MarkupType=2 then rmMarkup else (((rmNytPrCharge*rmSuplBuyingRate)*rmMarkup)/100) end as MainAgentMarkupvalue,
Case when MarkupType=2 then (rmNytPrCharge*rmSuplBuyingRate)+rmMarkup else  (rmNytPrCharge*rmSuplBuyingRate)+(((rmNytPrCharge*rmSuplBuyingRate)*rmMarkup)/100) end AS MainAgentSaleRate,
bokngSellingRate,
Case when MarkupType=2 then ((rmNytPrCharge*rmSuplBuyingRate)+rmMarkup)*bokngSellingRate else  (rmNytPrCharge*rmSuplBuyingRate)+(((rmNytPrCharge*rmSuplBuyingRate)*rmMarkup)/100)*bokngSellingRate END AS SubAgentCost,
Case when AgentMarkupType=2 then rmAgentMarkup else (((rmNytPrCharge*rmSuplBuyingRate)+(((rmNytPrCharge*rmSuplBuyingRate)*rmMarkup)/100)*bokngSellingRate)*rmAgentMarkup)/100 end AS SubAgentMarkupVal,
Case when AgentMarkupType=2 then (((rmNytPrCharge*rmSuplBuyingRate)+rmMarkup)*bokngSellingRate)+rmAgentMarkup else ((rmNytPrCharge*rmSuplBuyingRate)+(((rmNytPrCharge*rmSuplBuyingRate)*rmMarkup)/100)*bokngSellingRate)+(((rmNytPrCharge*rmSuplBuyingRate)+(((rmNytPrCharge*rmSuplBuyingRate)*rmMarkup)/100)*bokngSellingRate)*rmAgentMarkup)/100 END AS SubAgentSellingPrice,
rmMarkup AS MainAgentMarkup,
rmAgentMarkup as SubAgentMarkup,
MarkupType as MainAgentMarkupType,
AgentMarkupType as SubAgentMarkupType,
tb.bokgTransId,th.htlTransId
--tb.MA_ccy,tb.SA_ccy ,th.*,tr.*,trn.*
 from  tblhotelBooking as th join tblBooking as tb on th.bokngTransId=tb.bokgTransId 
join tblRoomDetails as tr on th.htlTransId=tr.htlTransId
join tblRoomNightPrice as trn on tr.rmDtTransId=trn.rmDtTransID

GO
/****** Object:  Index [IX_tblapilog_Supplierid]    Script Date: 5/23/2024 9:06:11 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblapilog_Supplierid] ON [dbo].[tblapilog]
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tracknumber]    Script Date: 5/23/2024 9:06:11 PM ******/
CREATE NONCLUSTERED INDEX [IX_Tracknumber] ON [dbo].[tblapilog]
(
	[TrackNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblapilog] ADD  DEFAULT ('0') FOR [ip]
GO
ALTER TABLE [dbo].[tblapilog] ADD  DEFAULT ((0)) FOR [preID]
GO
ALTER TABLE [dbo].[tblapilog_room] ADD  DEFAULT ('0') FOR [ip]
GO
ALTER TABLE [dbo].[tblapilog_room] ADD  DEFAULT ((0)) FOR [preID]
GO
ALTER TABLE [dbo].[tblapilog_search] ADD  DEFAULT ('0') FOR [ip]
GO
ALTER TABLE [dbo].[tblapilog_search] ADD  DEFAULT ((0)) FOR [preID]
GO
ALTER TABLE [dbo].[tblapilogflt] ADD  DEFAULT ('0') FOR [ip]
GO
ALTER TABLE [dbo].[tblapilogflt] ADD  DEFAULT ((0)) FOR [preID]
GO
ALTER TABLE [dbo].[tblsessionmgmt] ADD  DEFAULT ('1') FOR [sessionUserCount]
GO
ALTER TABLE [dbo].[tblsessionmgmt] ADD  DEFAULT ('1') FOR [sessionStatus]
GO
ALTER TABLE [dbo].[tblsessionmgmt_AM] ADD  DEFAULT (getdate()) FOR [AM_sessioncreatedOn]
GO
ALTER TABLE [dbo].[tblsessionmgmt_AM] ADD  DEFAULT (dateadd(minute,(15),getdate())) FOR [AM_sessionexpiryOn]
GO
/****** Object:  StoredProcedure [dbo].[AddLogTime]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <April 27, 2017>
-- Mofify date: <AUG 08, 2017>
-- Description:	<Procedure to insert api logs>
-- =============================================
Create PROCEDURE [dbo].[AddLogTime]
(

@TrackNumber nvarchar(max)
,@SupplierId bigint
,@StartTime datetime2(7)
,@EndTime datetime2(7)
,@retVal bit OUT
)
AS
BEGIN
        SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION
		BEGIN TRY
		INSERT INTO [dbo].[tblCalTime]
           ([TrackNumber]
           ,[SupplierId]
           ,[StartTime]
           ,[EndTime])
      VALUES
           (@TrackNumber,@SupplierId,@StartTime,@EndTime)
			set @retval=1
		COMMIT TRANSACTION
		END TRY
	BEGIN CATCH
	 IF (@@TRANCOUNT>0)
		BEGIN		
		   set @retval=0
		   ROLLBACK TRANSACTION 
		END
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[APIProc]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[APIProc]     
@flag int=NULL,    
@ExceptionMsg varchar(100)=NULL,   
@ExceptionType varchar(100)=NULL,   
@ExceptionSource nvarchar(max)=NULL,  
@customerID varchar(100)=NULL,   
@TransID nvarchar(max)=NULL,   
@MethodName varchar(100)=NULL,  
@PageName nvarchar(100)=NULL,   
@SuplId int=NULL,  
@HotelCode nvarchar(100)=NULL,   
@CreatedOn DateTime=NULL,  
@columnList varchar(100)=NULL,  
@table varchar(100)=NULL,  
@filter varchar(max)=NULL,  
@IpAddress varchar(50)=NULL,  
@logTypeID int=NULL,  
@retVal bit=null out  
  
  
AS      
BEGIN      
    
IF @flag = 1    
BEGIN    
SET @CreatedOn=GETDATE()  
INSERT INTO [dbo].[tblCustomExceptionLogging]  
([ExceptionMsg],[ExceptionType],[ExceptionSource],[customerID],[Logdate],[TransID],[MethodName],[PageName])  
VALUES(@ExceptionMsg,@ExceptionType,@ExceptionSource,@customerID,@CreatedOn,@TransID,@MethodName,@PageName)  
END   
  
IF @flag = 2    
BEGIN    
  
  
IF EXISTS (SELECT hotelid FROM [StaticData]..[tblgiatadetails] WHERE giataid=@HotelCode and localsupid=@SuplId)  
BEGIN   
select hotelid from [StaticData]..[tblgiatadetails]  
where giataid=@HotelCode and localsupid=@SuplId  
END   
ELSE  
BEGIN  
  
IF @table IS NOT NULL  
BEGIN  
  
Declare @sqlStr varchar(1000)  
SET @sqlStr = 'SELECT Top 50 ' + @columnList + ' FROM [StaticData].[dbo].' + @table + ' WHERE ' + @filter   
EXEC (@sqlStr)  
  
END   
  
END   
  
END    
  
IF @flag = 3    
BEGIN    
  
Declare @sqlCommand varchar(1000)  
--SET @columnList = 'HotelID,HotelName,StarRating'  
--SET @table = 'MikiStaticData'  
--SET @filter = 'CityID'  
--SET @filterValue = 10062  
  
SET @sqlCommand = 'SELECT Top 50 ' + @columnList + ' FROM [StaticData].[dbo].' + @table + ' WHERE ' + @filter   
  
EXEC (@sqlCommand)  
  
END   
  
  
IF @flag = 4    
BEGIN    
--WITH cte  
--AS (SELECT ROW_NUMBER() OVER (PARTITION BY HotelID ORDER BY ( SELECT 0)) RN,HotelID,ImageThumbnail,ImageLarge,ImageType  
--FROM   [StaticData].[dbo].[tblMikiImages] )  
--select * into #tempImage FROM cte  
--WHERE  RN = 1  
--select x.*,y.ImageThumbnail,y.ImageLarge,y.ImageType from [StaticData].[dbo].MikiStaticData  x inner join   
--#tempImage y on x.HotelID=y.HotelID  
--Where  y.HotelID IN (@filter)  
--drop table #tempImage  
Declare @mikiSql varchar(1000)  
--SET @mikiSql = 'WITH cte  
--AS (SELECT ROW_NUMBER() OVER (PARTITION BY HotelID ORDER BY ( SELECT 0)) RN,HotelID,ImageThumbnail,ImageLarge,ImageType  
--FROM   [StaticData].[dbo].[tblMikiImages] )  
--select x.*,cte.ImageThumbnail,cte.ImageLarge,cte.ImageType FROM cte inner join [StaticData].[dbo].MikiStaticData x  
--on cte.HotelID=x.HotelID and cte.RN=1  
--Where x.HotelID IN (' + @filter + ')'  
SET @mikiSql = 'select x.*,y.ImageThumbnail,y.ImageLarge,y.ImageType from [StaticData].[dbo].MikiStaticData  x   
inner join (SELECT ROW_NUMBER() OVER (PARTITION BY HotelID ORDER BY (SELECT 0)) RN,HotelID,ImageThumbnail,  
ImageLarge,ImageType FROM [StaticData].[dbo].[tblMikiImages] WITH(NOLOCK))  
y on x.HotelID=y.HotelID and y.RN=1 and y.HotelID IN (''' + @filter + ''')'  
EXEC (@mikiSql)  
  
  
END   
  
IF @flag = 5    
BEGIN    
  
Declare @sunSql varchar(1000)  
--SET @sunSql = 'WITH cte  
--AS (SELECT ROW_NUMBER() OVER (PARTITION BY HotelID ORDER BY ( SELECT 0)) RN,HotelID,FullSizeImage,SmallImage,ImageID  
--FROM   [StaticData].[dbo].[tblSunHotelImages] WITH(NOLOCK) )  
--select top 2 y.*,cte.FullSizeImage,cte.SmallImage,cte.ImageID   
--FROM cte  inner join [StaticData].[dbo].[tblSunHotelDetails] y   
--on cte.HotelID=y.HotelID and cte.RN=1  
--Where y.HotelID IN (' + @filter + ')'   
  
SET @sunSql ='select x.*,y.FullSizeImage,y.SmallImage,y.ImageID  FROM  [StaticData].[dbo].[tblSunHotelDetails] x WITH(NOLOCK)   
inner join (SELECT ROW_NUMBER() OVER (PARTITION BY HotelID ORDER BY (SELECT 0)) RN,  
HotelID,FullSizeImage,SmallImage,ImageID  
FROM [StaticData].[dbo].[tblSunHotelImages] WITH(NOLOCK)) y  
on x.HotelID=y.HotelID and y.RN=1 and y.HotelID IN (' + @filter + ')'  
EXEC (@sunSql)  
  
  
END  

--########################################################################

IF @flag = 6    
BEGIN    
SELECT Top  1 cast(IsNull(x.logresponseXML,'') as varchar(max)) as SearchResponse
FROM tblapilog x where x.SupplierID=@SuplId and x.TrackNumber=@TransID and x.logTypeID=@logTypeID and x.customerID=@customerID 
and x.ip=@IpAddress  
order by 1 desc

END  


 
  

  
  
  
END    
GO
/****** Object:  StoredProcedure [dbo].[dotwProc]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[dotwProc]     
@flag int=NULL,    
@SuplId bigint=NULL,      
@CityId bigint=NULL,    
@CountryId bigint=NULL,    
@LogTypeId int=NULL,    
@LogType varchar(50)=NULL,    
@TrackNO nvarchar(500)=NULL,  
@xmlKeys XML=NULL ,  
@HotelId varchar(200)=NULL    
AS      
BEGIN      
    
IF @flag = 1    
BEGIN    
Select SupCityId from tblSupplierCity_rel_new with(nolock) where CityId=@CityId and supId=@SuplId    
END      
    
IF @flag = 2    
BEGIN    
Select SupCntId from tblSupplierCountry_rel_new  with(nolock) where cntId=@CountryId and supId=@SuplId    
END    
    
    
IF @flag = 3    
BEGIN    
Select CityId from tblSupplierCity_rel_new with(nolock) where SupCityId=@CityId and supId=@SuplId    
END      
    
IF @flag = 4    
BEGIN    
Select cntId from tblSupplierCountry_rel_new with(nolock) where SupCntId=@CountryId and supId=@SuplId    
END    
      
IF @flag = 5    
BEGIN   
Select top 1  logresponseXML,logID  from tblapilog_room with(nolock)  
where logID= (Select Max(logID) from tblapilog_room where TrackNumber=@TrackNO and SupplierID=@SuplId and logTypeID=@logTypeId   
and logType=@LogType and logrequestXML like '%'+@HotelId+'%')    
  
END    
  
IF @flag =6   
BEGIN   
  
DECLARE @xmlResp XML   
Select top 1 @xmlResp=logresponseXML from tblapilog_room with(nolock) where   
logID= (Select Max(logID) from tblapilog_room where TrackNumber=@TrackNO and SupplierID=@SuplId and logTypeID=@logTypeId and logType=@LogType and logrequestXML like '%'+@HotelId+'%')  
Set @xmlResp=(Select @xmlResp, @xmlKeys for xml path ('response'))  
  
  
Select @xmlResp.query('<rates>{  
for $rate in //rateBasis,  
    $keys in //roomKeys  
where $rate/allocationDetails = $keys/roomKey  
return $rate}</rates>') as responseXML  
  
  
  
  
END   
  
  
IF @flag = 7    
BEGIN   
   
Select top 1  logresponseXML  from tblapilog with(nolock)   
where logID= @CityId  
  
END    
  
  
END
GO
/****** Object:  StoredProcedure [dbo].[dotwProc_cancel]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[dotwProc_cancel]       
@flag int=NULL,      
@SuplId bigint=NULL,        
@CityId bigint=NULL,      
@CountryId bigint=NULL,      
@LogTypeId int=NULL,      
@LogType varchar(50)=NULL,      
@TrackNO nvarchar(500)=NULL,    
@xmlKeys XML=NULL ,    
@HotelId varchar(200)=NULL      
AS        
BEGIN        
      
IF @flag = 1      
BEGIN      
Select SupCityId from tblSupplierCity_rel_new with(nolock) where CityId=@CityId and supId=@SuplId      
END        
      
IF @flag = 2      
BEGIN      
Select SupCntId from tblSupplierCountry_rel_new  with(nolock) where cntId=@CountryId and supId=@SuplId      
END      
      
      
IF @flag = 3      
BEGIN      
Select CityId from tblSupplierCity_rel_new with(nolock) where SupCityId=@CityId and supId=@SuplId      
END        
      
IF @flag = 4      
BEGIN      
Select cntId from tblSupplierCountry_rel_new with(nolock) where SupCntId=@CountryId and supId=@SuplId      
END      
        
IF @flag = 5      
BEGIN     
Select top 1  logresponseXML,logID  from tblapilog with(nolock)    
where logID= (Select Max(logID) from tblapilog where TrackNumber=@TrackNO and SupplierID=@SuplId and logTypeID=@logTypeId     
and logType=@LogType and logrequestXML like '%'+@HotelId+'%')      
    
END      
   
    
END
GO
/****** Object:  StoredProcedure [dbo].[dotwProc_OUT]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[dotwProc_OUT]   
@flag int=NULL,  
@SuplId bigint=NULL,   
@CityId bigint=NULL, 
@CountryId bigint=NULL,  
@LogTypeId int=NULL,  
@LogType varchar(50)=NULL,  
@TrackNO nvarchar(500)=NULL,
@custID varchar(100)=NULL,
@xmlKeys XML=NULL  ,
@HotelId varchar(200)=NULL  
AS    
BEGIN    
IF @flag = 1  
BEGIN  
Select SupCityId from tblSupplierCity_rel_new where CityId=@CityId and supId=@SuplId  
END    
IF @flag = 2  
BEGIN  
Select SupCntId from tblSupplierCountry_rel_new where cntId=@CountryId and supId=@SuplId  
END  
IF @flag = 3  
BEGIN  
Select CityId from tblSupplierCity_rel_new where SupCityId=@CityId and supId=@SuplId 
END    
IF @flag = 4  
BEGIN  
Select cntId from tblSupplierCountry_rel_new where SupCntId=@CountryId and supId=@SuplId  
END  
IF @flag = 5  
BEGIN 
Select top 1  logresponseXML,logID  from tblapilogOut with(nolock) 
where logID= (Select Max(logID) from tblapilogOut with(nolock) where TrackNumber=@TrackNO and SupplierID=@SuplId and logTypeID=@logTypeId and logType=@LogType and customerID=@custID and logrequestXML like '%'+@HotelId+'%')  
END  
IF @flag =6 
BEGIN 
DECLARE @xmlResp XML 
Select top 1 @xmlResp=logresponseXML from tblapilogOut with(nolock) where 
logID= (Select Max(logID) from tblapilogOut with(nolock) where TrackNumber=@TrackNO and SupplierID=@SuplId and logTypeID=@logTypeId and logType=@LogType and logrequestXML like '%'+@HotelId+'%')
Set @xmlResp=(Select @xmlResp, @xmlKeys for xml path ('response'))
Select @xmlResp.query('<rates>{
for $rate in //rateBasis,
    $keys in //roomKeys
where $rate/allocationDetails = $keys/roomKey
return $rate}</rates>') as responseXML
END 
IF @flag = 7  
BEGIN 
Select top 1  logresponseXML  from tblapilogOut with(nolock) 
where logID= @CityId
END  
END
GO
/****** Object:  StoredProcedure [dbo].[GetCurrencyRates]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetCurrencyRates]    
@userID bigint,@status varchar(50)    
AS    
BEGIN    
    
IF (@status='True')  --SAAS    
BEGIN    
    
 Select crncyCode,crmkpBuyingRate,crmkpAppliedSellingRate,c.crncyId     
 from travayoo.dbo.tblCurrencyMarkup as c join travayoo.dbo.tblCurrencyMaster CM on c.crncyId=cm.crncyId  where usrid=@userID    
                          
END    
ELSE --HA    
BEGIN    
 Select crncyCode,crmkpBuyingRate,crmkpAppliedSellingRate,c.crncyId     
 from travayoo.dbo.tblCurrencyMarkup as c join travayoo.dbo.tblCurrencyMaster CM on c.crncyId=cm.crncyId  where usrid=@userID    
END    
    
END 
GO
/****** Object:  StoredProcedure [dbo].[GetGiataData]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--getgiatadata'<Hotels><Hotel><HotelID >17415</HotelID><SupplierID >4</SupplierID></Hotel></Hotels>','<SupplierList>



--  <SupplierIDS>



--    <SupplierID>2</SupplierID>



--  </SupplierIDS>



--  <SupplierIDS>



--    <SupplierID>4</SupplierID>



--  </SupplierIDS>



--  <SupplierIDS>



--    <SupplierID>6</SupplierID>



--  </SupplierIDS>



--</SupplierList>'



CREATE proc [dbo].[GetGiataData] 



@XML XML,@CountryCode varchar(50)



AS







BEGIN







Select propar.value('(HotelID)[1]','varchar(100)') as HotelID,propar.value('(SupplierID)[1]','varchar(100)') as SupplierID
--,propar.value('(RequestID)[1]','varchar(100)') as RequestID 
into #HotelData from @XML.nodes('Hotels/Hotel') as  pros(propar)







--Select propar.value('(SupplierID)[1]','varchar(100)') as SupID  into #suplist from @SUPXML.nodes('SupplierList/SupplierIDS') as  pros(propar)







--select distinct tg.GiataID,H.HotelID as HotelID,isnull(tg.localsupid,0) as  Suppliers from staticdata.dbo.[tblGiataDetails] as tg with(nolock)



-- join #HotelData as h on tg.hotelid=h.HotelID and tg.localsupid=h.SupplierID





select distinct giataid,HotelID,localsupid,hotelname
--,Longitude,Latitude 
into #giatadetails from staticdata.dbo.tblGiataDetails with(nolock) where CountryCode=@CountryCode


select distinct tg.GiataID,tg.HotelID as HotelID,hotelname,isnull(tg.localsupid,0) as  Suppliers 
--,Longitude,Latitude
 from #giatadetails as tg

 join #HotelData as h on tg.hotelid=h.HotelID and tg.localsupid=h.SupplierID
 
END








GO
/****** Object:  StoredProcedure [dbo].[GetGiataData_New]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--GetGiataData_New 444
CREATE proc [dbo].[GetGiataData_New] 




--@CountryCode varchar(50),
@CityID varchar(20)
 
 
 --@tblCustomers GiataDataType READONLY



AS







BEGIN







--Select propar.value('(HotelID)[1]','varchar(100)') as HotelID,propar.value('(SupplierID)[1]','varchar(100)') as SupplierID,propar.value('(RequestID)[1]','varchar(100)') as RequestID into #HotelData from @XML.nodes('Hotels/Hotel') as  pros(propar)







--Select propar.value('(SupplierID)[1]','varchar(100)') as SupID  into #suplist from @SUPXML.nodes('SupplierList/SupplierIDS') as  pros(propar)







select distinct tg.GiataID,tg.HotelID as HotelID,isnull(tg.localsupid,0) as  Suppliers from staticdata.dbo.[tblGiataDetails] as tg with(nolock)

where cityid in(select cityid from staticdata..tblGiataCityMapping with(nolock) where localcityid=@CityID)






--select distinct giataid,a.HotelID,isnull(localsupid,0) as Supplier,Longitude,Latitude into #giatadetails from staticdata.dbo.tblGiataDetails as a with(nolock)  
-- where CountryCode=@CountryCode




--select distinct a.GiataID,a.HotelID as HotelID,isnull(a.Supplier,0) as  Suppliers,Longitude,Latitude from #giatadetails as a

--join  @tblCustomers as b on a.hotelid=b.HotelID and a.Supplier=b.SupplierID
 
END








GO
/****** Object:  StoredProcedure [dbo].[GetXMLOutSupplierCurrency]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetXMLOutSupplierCurrency]
@ActionType varchar(100),@Tracknumber varchar(max),@hotelcode varchar(100)
AS
Begin

--hit for room
if (@ActionType=1) 
begin

select top 1 resp.value('(@Suppliercurrency)[1]', 'varchar(20)') as CurrencyID from tblapilog as a 
Cross Apply a.logresponsexml.nodes('searchRS/hotels') as CountryRes(Res)
Cross Apply Res.nodes('hotel') as Htlresp(resp)
where tracknumber=@Tracknumber and logtype='search' and SupplierID=0 
and resp.value('(@code)[1]', 'varchar(100)')=@hotelcode
order by 1 desc

	

end
--for prebook
if (@ActionType=2)  
begin
select top 1 resp.value('(@Suppliercurrency)[1]', 'varchar(20)') as CurrencyID from tblapilog as a 
Cross Apply a.logresponsexml.nodes('roomRS/hotels') as CountryRes(Res)
Cross Apply Res.nodes('hotel') as Htlresp(resp)
where tracknumber=@Tracknumber and logtype='RoomAvail' and SupplierID=0 
and resp.value('(@code)[1]', 'varchar(100)')=@hotelcode
order by 1 desc	
end




End
GO
/****** Object:  StoredProcedure [dbo].[JuniperProc]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[JuniperProc]
@flag int=NULL,
@SuplId int=NULL,
@CityId bigint=null,
@HotelId varchar(50)=null  
AS
BEGIN
SET NOCOUNT ON;

IF @flag = 1
BEGIN

select x.HotelID,x.HotelName,y.ZoneID,x.ZoneName from [TestRakesh].[dbo].[tblJuniperHotelList] x 
inner join [TestRakesh].[dbo].[tblJuniperSuplHotelMapping] y
on x.HotelID=y.HotelID 
inner join [TravayooService].[dbo].[tblSupplierCity_rel_new] z on  
(y.ZoneID=z.SupCityID OR y.CityID=z.SupCityID) 
and y.SuplId=z.supId and z.CityId=@CityId and z.supId=@SuplId


END  


/*************************************************************************************************************/

IF @flag = 2
BEGIN

select x.*,y.CityID,y.ZoneID,y.SuplId from 
[TestRakesh].[dbo].[tblJuniperHotelDetails] x inner join 
[TestRakesh].[dbo].[tblJuniperSuplHotelMapping] y  
on x.HotelID=y.HotelID and y.SuplId=@SuplId and y.CityID=@CityID

END 
/*************************************************************************************************************/

IF @flag = 3
BEGIN

select x.*,y.CityID,y.ZoneID,y.SuplId from 
[TestRakesh].[dbo].[tblJuniperHotelDetails] x inner join 
[TestRakesh].[dbo].[tblJuniperSuplHotelMapping] y  
on x.HotelID=y.HotelID and y.SuplId=@SuplId and y.HotelID=@HotelId

END 
/*************************************************************************************************************/


IF @flag = 4
BEGIN

select * from [TestRakesh].[dbo].[tblJuniperFacility]
where HotelID=@HotelId

END 

/*************************************************************************************************************/


IF @flag =5
BEGIN

select * from [TestRakesh].[dbo].[tblJuniperHotelImages] where HotelID = @HotelId 

END 

IF @flag =6
BEGIN


 
select x.*,y.CityID,y.SuplId from [TestRakesh].[dbo].[tblJuniperHotelImages] x 
inner join [TestRakesh].[dbo].[tblJuniperSuplHotelMapping] y
on x.HotelID=y.HotelID 
inner join [TravayooService].[dbo].[tblSupplierCity_rel_new] z on  
y.CityID=z.SupCityID
and y.SuplId=z.supId and z.CityId=@CityId and z.supId=@SuplId 




END 






END  
GO
/****** Object:  StoredProcedure [dbo].[sp_callws]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_callws]
@parameter varchar(200)=null

as

declare @obj int
declare @valorderegreso int
declare @surl varchar(200)
declare @response varchar(8000)
declare @hr int
declare @src varchar(255)
declare @desc varchar(255)

set @surl = 'https://api-test.hotelspro.com/api/v2/search/?currency=USD&client_nationality=at&destination_code=18edc&pax=1&checkin=2018-01-29&checkout=2018-02-01postedDatahotel_code=1091d1,1091cc,1091c6,1091c4,118ad5,114d42,1147ac,11478c,111f37,111f36,111f35,111e46,111e45,111e3f,10b298,10b297,10b296,109d74,109d31,109d08,109ceb,109ce9,109cbb,109cad,106169,12128c,193971,193970,19396f,19396e,19396d,19396c,193969,193968,193965,193964,18bf2b,17172e,171627,1521c5,144cb9,13ddb0,1347bb,12d73b,12a3f9,12229a,118896,1151fa,110f8f,110f8e,110f8c,110f8b,110f89,110f88,110f81,110f7e,110f7c,110f76,109f6f,109e7f,1094f9,1094f7,10947e,109463,109455,10944f,1304c8,12b0c2,102de4,1025ed,19f6c6,19604f,19604d,19604c,19604a,196048,196047,196045,196044,196043,196042,19603c,19603a,196035,196033,196031,19602d,19602c,19602b,196029,196027,196026,196025,196023,196020,19601f,19601d,19601b,196017,196015,196013,196012,196011,196010,19600f,19600e,19600c'

exec sp_OACreate 'MSXML2.ServerXMLHttp' ,@obj OUT
exec sp_OAMethod @obj, 'Open', NULL, 'Get', @surl , false
exec sp_OAMethod @obj, 'send'
exec sp_OAGetProperty @obj, 'responseText', @response OUT

select @response [response]
exec sp_OADestroy @obj

return
GO
/****** Object:  StoredProcedure [dbo].[SP_CheckHotelstatic]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================







-- Author:		<Suraj Singh>







-- Create date: <Jul 21, 2017>







-- Mofify date: <AUG 22, 2017>







-- Description:	<Procedure to check hotel>







-- =============================================







CREATE PROCEDURE [dbo].[SP_CheckHotelstatic]







(







@HotelCode nvarchar(max)=null,







@type varchar(10)=null,







@retVal bit OUT







)







AS







BEGIN







        SET NOCOUNT ON;







		SET XACT_ABORT ON	







		BEGIN TRY







			if(@type=1)







			Begin







			select top 1 * from [StaticData].[dbo].[tblHB_Hotelstatic] with(nolock) where HotelCode=@HotelCode



			--and HBmodifyOn <= CAST(GETDATE()-2 AS DATE)







			set @retval=1	







			End







			if(@type=2)







			Begin







			select top 1 * from [StaticData].[dbo].[tblHPro_Hotelstatic] with(nolock) where HotelCode=@HotelCode







			set @retval=1	







			End







		END TRY







	BEGIN CATCH







	END CATCH







END
GO
/****** Object:  StoredProcedure [dbo].[SP_CheckHotelstaticNHprotbl]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================



-- Author:		<Suraj Singh>



-- Create date: <APR 03, 2019>



-- Mofify date: <APR 03, 2019>



-- Description:	<Procedure to check hotel>



-- =============================================



CREATE PROCEDURE [dbo].[SP_CheckHotelstaticNHprotbl]



(



@HotelCode nvarchar(max)=null,



@type varchar(10)=null,



@retVal bit OUT



)



AS



BEGIN



        SET NOCOUNT ON;



		SET XACT_ABORT ON	



		BEGIN TRY



			if(@type=1)



			Begin



			select * from [StaticData].[dbo].[tblHB_Hotelstatic] with(nolock) where HotelCode=@HotelCode



			--and HBmodifyOn <= CAST(GETDATE()-2 AS DATE)



			set @retval=1	



			End



			if(@type=2)



			Begin



			select top 1 * from [StaticData].[dbo].[tblHotelsProStaticData] with(nolock) where HotelCode=@HotelCode



			set @retval=1	



			End



		END TRY



	BEGIN CATCH



	END CATCH



END
GO
/****** Object:  StoredProcedure [dbo].[sp_CheckTrackstatus]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[sp_CheckTrackstatus]

@Tracknumber varchar(100),@Flag int=0,@outCustID bigint=0,@suplConfNo varchar(100)=''

AS

BEGIN

if (@Flag=0)

BEGIN

 select count(*) from tblXMLOutBookingDetails with(nolock) where tracknumber=@Tracknumber

 END

 Else

 BEGIN

 select count(*) from tblXMLOutBookingDetails with(nolock) where tracknumber=@Tracknumber and supplierConfNo=@suplConfNo and outcustid=@outCustID

 END

END
GO
/****** Object:  StoredProcedure [dbo].[SP_CustomExceptionLogging]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create Procedure [dbo].[SP_CustomExceptionLogging]
(  
@ExceptionMsg varchar(100)=null,  
@ExceptionType varchar(100)=null,
@ExceptionSource nvarchar(max)=null, 
@customerID varchar(100)=null  ,
@PageName varchar(100)=null  ,
@MethodName varchar(100)=null  ,
@TransID nvarchar(max)=null  
)  
as  
begin  
Insert into tblCustomExceptionLogging  
(  
ExceptionMsg ,  
ExceptionType,   
ExceptionSource,  
customerID,  
Logdate,TransID,MethodName,PageName
)  

select  

@ExceptionMsg,  

@ExceptionType,  

@ExceptionSource,  

@customerID,  

getdate()  ,@TransID,@MethodName,@PageName

End
GO
/****** Object:  StoredProcedure [dbo].[SP_ExceptionHPro_Hotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <June 06, 2017>
-- Mofify date: <aug 22, 2017>
-- Description:	<Procedure to insert HotelsPro Hotels>
-- =============================================
CREATE Procedure [dbo].[SP_ExceptionHPro_Hotels]  

(  

@ExceptionMsg varchar(100)=null,  

@ExceptionType varchar(100)=null,  

@ExceptionSource nvarchar(max)=null,  

@HotelCode nvarchar(max)=null  

)  

as  

begin  

Insert into [StaticData].[dbo].[tblException_HProHotels]

(  

ExceptionMsg ,  

ExceptionType,   

ExceptionSource,  

HotelCode,  

Logdate  

)  

select  

@ExceptionMsg,  

@ExceptionType,  

@ExceptionSource,  

@HotelCode,  

getdate()  

End
GO
/****** Object:  StoredProcedure [dbo].[SP_ExceptionLogging]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[SP_ExceptionLogging]  

(  

@ExceptionMsg varchar(100)=null,  

@ExceptionType varchar(100)=null,  

@ExceptionSource nvarchar(max)=null,  

@customerID varchar(100)=null  

)  

as  

begin  

Insert into tblExceptionLogging  

(  

ExceptionMsg ,  

ExceptionType,   

ExceptionSource,  

customerID,  

Logdate

)  

select  

@ExceptionMsg,  

@ExceptionType,  

@ExceptionSource,  

@customerID,  

getdate()  

End
GO
/****** Object:  StoredProcedure [dbo].[SP_GadouCityMapping]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GadouCityMapping]    
@CityID nvarchar(50) = null,     
@SupplierID nvarchar(10) = null,    
@retVal bit out    
as    
begin    
 begin try    
  select cr.id,cr.cityid,cr.supcityid,cr.supid,cr.cntid,cr.stateid,cl.ctyLclName from tblSupplierCity_rel_new  cr with(nolock)    
  join [Travayoo].[dbo].[tblCityMaster_Locale] cl with(nolock) on cr.CityID = cl.ctyId    
  where cr.CityID = @CityID and cr.supId = @SupplierID    
  set @retVal = 1    
 end try    
 begin catch    
  select @@TRANCOUNT    
 end catch    
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetAll]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[SP_GetAll]
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[SalTours_HotelsList] with (nolock)
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end



GO
/****** Object:  StoredProcedure [dbo].[sp_getAvailablelimit]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[sp_getAvailablelimit]  
@MainAgentID VARCHAR(100),  
@agencyID VARCHAR(100),  
@outcustomerid varchar(100)  
AS  
BEGIN  
declare @dbname varchar(50)  
set @dbname=(select distinct dbname from XMLOutDetails where customerid=@MainAgentID and agencyid=@agencyID and Outcustid=@outcustomerid)  
exec TRAVAYOO..uspXmlOutAgentInfo @MainAgentID,@agencyID,1,@dbname  
END  
  
GO
/****** Object:  StoredProcedure [dbo].[SP_GetCity_HotelsPro]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  

-- Author:  <Suraj Singh>  

-- Create date: <July 07, 2017>  

-- Description: <Procedure to get city name using citycode>  

-- =============================================  

CREATE PROCEDURE [dbo].[SP_GetCity_HotelsPro]  

@CityCode nvarchar(150)=null,  

@retVal bit OUT  

AS  

BEGIN  

    BEGIN TRY  

          select SupCityId as citycode from tblSupplierCity_rel_new with(nolock) where supId=6 and cityid=@CityCode  

    set @retval=1  

    END TRY  

 BEGIN CATCH  

        select @@TRANCOUNT  

     END CATCH  

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetCityCountyDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  
  
  
CREATE proc [dbo].[sp_GetCityCountyDetails]  
@CityID bigint  
AS  
BEGIN  
   
 SELECT c.ctyId,c.ctyLclName,isnull(tm.ctyCode,'') as CityCode,isnull(cnt.cntCode,'') as CountryCode,cntl.cntLclName as CountryName,cnt.cntId as CountyID 
 from Travayoo..tblCityMaster_Locale as c join Travayoo..tblCityMaster as tm on c.ctyId=tm.ctyId  
 
 join Travayoo..tblCountryMaster as cnt on tm.cntId=cnt.cntId join Travayoo..tblCountryMaster_Locale as cntl on cnt.cntId=cntl.cntId  
 where c.ctyId=@CityID  
  
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetCityHotelBeds]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================= 
-- Author:  <Suraj Singh>  
-- Create date: <Oct 06, 2017> 
-- Description: <Procedure to get city code using common citycode> 
-- =============================================  
CREATE PROCEDURE [dbo].[SP_GetCityHotelBeds]  
@CityCode nvarchar(150)=null,  
@retVal bit OUT  
AS  
BEGIN  
    BEGIN TRY  
          select SupCityId as citycode from tblSupplierCity_rel_new with(nolock) where supId=4 and cityid=@CityCode 
    set @retval=1  
    END TRY  
 BEGIN CATCH  
        select @@TRANCOUNT  
     END CATCH  
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetCountry_HotelsPro]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Author:  <Suraj Singh>  
-- Create date: <July 24, 2017>  
-- Description: <Procedure to get country code using common country id>  
-- =============================================  
CREATE PROCEDURE [dbo].[SP_GetCountry_HotelsPro]  
@Countryid nvarchar(150)=null,  
@retVal bit OUT  
AS  
BEGIN  
    BEGIN TRY  
          select SupCntId as countrycode from tblSupplierCountry_rel_new with(nolock) where supId=6 and cntId=@Countryid  
    set @retval=1  
    END TRY  
 BEGIN CATCH 
        select @@TRANCOUNT  
     END CATCH  
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetDarina_HotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <Dec 21, 2017>
-- Description:	<Procedure to get hotel detail using hotelid>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetDarina_HotelDetail]
@transid nvarchar(150)=null,
@retVal bit OUT
AS
BEGIN
   	BEGIN TRY
          select logresponseXML from tblapilog with(nolock) where TrackNumber=@transid and logTypeID=10
		  set @retval=1
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetDarina_HotelDetailOut]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <Feb 14, 2020>
-- Description:	<Procedure to get hotel detail using hotelid>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetDarina_HotelDetailOut]
@transid nvarchar(150)=null,
@retVal bit OUT
AS
BEGIN
   	BEGIN TRY
          select logresponseXML from tblapilogOut with(nolock) where TrackNumber=@transid and logTypeID=10
		  set @retval=1
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetExtranet_HotelDetail_Test]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_GetExtranet_HotelDetail_Test]

@HotelCode nvarchar(150)=null,
@HotelName nvarchar(200)=null,

@CityID nvarchar(150)=null,



@retVal bit OUT

AS
begin

	if LEN(ISNULL(@HotelCode,''))=0        
				set @HotelCode=null
	    if LEN(ISNULL(@HotelName,''))=0        
				set @HotelName=null



 if(@HotelCode is not null  )
	begin
   -- supplierid against giata id for hotel
	select hotelid into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where giataid=@HotelCode and localsupid=3 
	 declare @count int =(select count(*) from #temp2)
	if(@count >0)
	begin
	 select * from #temp2;
     set @retval=1
	 end

	 --Hotel Name if Supplier HotelCode not find against giata


	else if(@HotelName is not null )
	begin
			
	
	select hotelid  from [StaticData]..[tblgiatadetails] with (nolock) where hotelname like '%'+@HotelName+'%' and localsupid=3 and cityid= @CityID
  
	  set @retval=1


	end



	end
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetGadou_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_GetGadou_HotelList]
@CityID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)
		where CityID = @CityID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetGodou_HotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetGodou_HotelDetails]
@CityID nvarchar(20) = null,
@retVal bit out
as
begin 
	begin try
		select * from [StaticData].[dbo].[GodouHotelDetailsNew] with (nolock)
		where CityID = @CityID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetGodou_HotelDetailsSingle]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetGodou_HotelDetailsSingle]
@HotelID nvarchar(100) = null,
@retVal bit out
as
begin 
	begin try
		select * from [StaticData].[dbo].[GodouHotelDetailsNew] with (nolock)
		where HtlUniqueID = @HotelID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetGoGlobal]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================     
-- Author:  <gaurav>      
-- Create date: <Oct 16, 2019 >     
-- Description: <Procedure to get supplierId from GiataId     
-- =============================================      
CREATE PROCEDURE [dbo].[SP_GetGoGlobal]      
@cityid nvarchar(150)=null,      
@countryid nvarchar(150)=null,      
@hotelid nvarchar(150)=null,      
@TransId nvarchar(150)=null,      
@Type nvarchar(150)=null,      
@code nvarchar(150)=null,      
@retVal bit OUT      
AS      
BEGIN      
    BEGIN TRY      
    
 if(@Type='cityid')    
       select SupCityId as citycode from tblSupplierCity_rel_new with(nolock) where supId=27 and cityid=@cityid     
 else if(@Type='countryid')    
     select SupCntId as countrycode from tblSupplierCountry_rel_new with(nolock) where supId=27 and cntId=@countryid     
  else if(@Type='hotelid')    
     select hotelid  from [StaticData]..[tblgiatadetails] with (nolock) where giataid=@hotelid and localsupid=27    
  else if(@Type='roomlog')    
     select top 1  logresponseXML from tblapilog_room with(nolock) where TrackNumber = @TransId and logType = 'RoomAvail'  and SupplierID=0  order by logID desc    
   else if(@Type='suproomlog')    
     select top 1 logresponseXML from tblapilog_room with(nolock) where TrackNumber = @TransId and logType = 'RoomAvail'  and SupplierID=27 order by logID desc    
    else if(@Type='supsearchlog')    
     select Res.value('(Offers/item/HotelSearchCode)[1]', 'varchar(200)') as code,Res.value('(Thumbnail)[1]', 'varchar(max)') as images from tblapilog l    
Cross Apply logresponseXML.nodes('root') as BookRes(BRES)    
Cross Apply BRES.nodes('Hotels/item') as Resp(Res)    
    
 where TrackNumber = @TransId and logType = 'Search'  and SupplierID=27     
 and  Res.value('(HotelCode)[1]', 'varchar(100)')=@hotelid    
 else if(@Type='countrysupcode')    
    select SUBSTRING(isocode, 2, LEN(isocode)-2)  as code from [StaticData]..tblGoGlobalcountryonly where countryid in  ( select '"'+SupCntId+'"'  from tblSupplierCountry_rel_new with(nolock) where supId=27 and cntId=@countryid )    
       else if(@Type='remark')    
    select top 1  Res.value('(Remark)[1]', 'varchar(max)') as remark,Res.value('(Special)[1]', 'varchar(max)') as special from tblapilog_room l    
Cross Apply logresponseXML.nodes('root') as BookRes(BRES)    
Cross Apply BRES.nodes('Hotels/item/Offers/item') as Resp(Res)    
 where TrackNumber = @TransId and logType = 'RoomAvail'  and SupplierID=27     
 and  Res.value('(HotelSearchCode)[1]', 'varchar(100)')=@code order by logID desc    
     
    
    set @retval=1      
    END TRY      
 BEGIN CATCH      
        select @@TRANCOUNT      
     END CATCH      
END    
GO
/****** Object:  StoredProcedure [dbo].[SP_GetGoGlobal_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
  
-- Author:  <Gaurav>  
  
-- Create date: <oct 31, 2019>  
  
  
-- Description: <Procedure to get hotel static data from a particular city>  
  
-- =============================================  
  
CREATE PROCEDURE [dbo].[SP_GetGoGlobal_HotelList]  
  
@HotelCode nvarchar(150)=null,  
  
@HotelName nvarchar(200)=null,  
@CityCode nvarchar(150)=null,  
  
  
@retVal bit OUT  
  
AS  
  
BEGIN  
  
    BEGIN TRY  
  
 SET NOCOUNT ON;  
  
      
  
           if LEN(ISNULL(@HotelCode,''))=0    
            set @HotelCode=null;        
     if LEN(ISNULL(@HotelName,''))=0       
      set @HotelName=null;        
     
  
    if( @HotelCode is null  )  
    begin  
    select * from [StaticData]..[tblGoGlobalHotelList] with(nolock) where cityid=@CityCode;  
  
      
    end  
    else  --Search By Giata HotelCode  
    begin  
    -- supplierid against giata id for hotel  
           select hotelid into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where giataid=@HotelCode and localsupid=27  
     declare @count int =(select count(*) from #temp2)  
            if(@count >0)  
             begin  
      
  
     select * from [StaticData]..[tblGoGlobalHotelList] with(nolock) where cityid=@CityCode  
     and hotelid in(select hotelid from #temp2)  
    end  
    else    if(@HotelName is not null)  --Search By Giata HotelName if Supplier Hotel Id not Find in Giata  
    begin  
      
  
     select * from [StaticData]..[tblGoGlobalHotelList] with(nolock) where cityid=@CityCode  
      and Name like '%'+@HotelName+'%'  
    end  
  
  
  
    end  
  
  
    set @retval=1  
  
    SET NOCOUNT OFF;  
  
    END TRY  
  
 BEGIN CATCH  
  
        select @@TRANCOUNT  
  
     END CATCH  
  
END  
GO
/****** Object:  StoredProcedure [dbo].[SP_GetHB_HotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <Jul 21, 2017>

-- Mofify date: <feb 05, 2019>

-- Description:	<Procedure to get hotel details>

-- =============================================

CREATE PROCEDURE [dbo].[SP_GetHB_HotelDetail]

@HotelCode nvarchar(150)=null,

@CityCode nvarchar(150)=null,
@CityID nvarchar(150)=null,

@MinRating nvarchar(150)=null,

@MaxRating nvarchar(150)=null,

@retVal bit OUT

AS

BEGIN

   	BEGIN TRY

	SET NOCOUNT ON;

	declare @cityname nvarchar(500)
	set @cityname=(select CityName from [TravayooService]..[tblSupplierCity_rel_new] with(nolock) where CityID=@CityID and supid=4)	
	if(@MinRating = '0' and @MaxRating='5')
	Begin
		 --   select HotelCode,HotelXML,cast(SUBSTRING(propn.value('(description)[1]','varchar(100)'), PATINDEX('%[0-9]%', propn.value('(description)[1]','varchar(100)')), 1) as int)
			--as res into #hb1 from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock) 
			--CROSS APPLY HotelXML.nodes('hotelDetailsRS/hotel') as props(prop)
			--CROSS APPLY prop.nodes('category') as propsn(propn)
			--where
			--HotelCode=@HotelCode or CityCode=@CityCode
			--select * from #hb1 with(nolock)
			select HotelCode,HotelXML,
			(case 
		   when [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') is not null and PATINDEX('%[0-9]%', [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') ) > 0 then 
		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1) else '0'
		   end 
		   when [HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)') is not null then 
		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1) else '0'
		   end 
		   else '0' 
		   end) 
			as res 
			from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock) 
			where case when  @cityname is null then 1
			 when HotelID like '%'+@cityname+'%'
			 then 1    
			 else 0
		     end = 1
		     and 
			 CityCode=@CityCode OR HotelCode=@HotelCode
			set @retval=1
	End
	else
	Begin 

	  --      select HotelCode,HotelXML,cast(SUBSTRING(propn.value('(description)[1]','varchar(100)'), PATINDEX('%[0-9]%', propn.value('(description)[1]','varchar(100)')), 1) as int)

			--as res into #hb2 from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock) 

			--CROSS APPLY HotelXML.nodes('hotelDetailsRS/hotel') as props(prop)

			--CROSS APPLY prop.nodes('category') as propsn(propn)

			--where

			--HotelCode=@HotelCode or CityCode=@CityCode

			--select * from #hb2 with(nolock)

			--where  res>=@MinRating  and  res<=@MaxRating

			select HotelCode,HotelXML,

			(case 

		   when [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') is not null and PATINDEX('%[0-9]%', [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') ) > 0 then 

		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1) else '0'

		   end 

		   when [HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)') is not null then 

		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1) else '0'

		   end 

		   else '0' 

		   end) 

			as res into #hb2 

			from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock) 			

			where case when  @cityname is null then 1
			 when HotelID like '%'+@cityname+'%'
			 then 1    
			 else 0
		     end = 1
		     and CityCode=@CityCode
			or HotelCode=@HotelCode   

			select * from #hb2 with(nolock)

			where   res>=@MinRating  and  res<=@MaxRating

			set @retval=1

	End
	SET NOCOUNT OFF;
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetHB_HotelDetail_Test]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <Jul 21, 2017>

-- Mofify date: <feb 05, 2019>

-- Description:	<Procedure to get hotel details>

-- =============================================

CREATE PROCEDURE [dbo].[SP_GetHB_HotelDetail_Test]

@HotelCode nvarchar(150)=null,
@HotelName nvarchar(200)=null,
@CityCode nvarchar(150)=null,
@CityID nvarchar(150)=null,

@MinRating nvarchar(150)=null,

@MaxRating nvarchar(150)=null,

@retVal bit OUT

AS

BEGIN

   	BEGIN TRY

	SET NOCOUNT ON;
	declare @cityname nvarchar(500)
		if LEN(ISNULL(@HotelCode,''))=0        
				set @HotelCode=null
	    if LEN(ISNULL(@HotelName,''))=0        
				set @HotelName=null

	if(@HotelCode is  null 	 )
	begin
	
	set @cityname=(select CityName from [tblSupplierCity_rel_new] with(nolock) where CityID=@CityID and supid=4)	
	if(@MinRating = '0' and @MaxRating='5')
	Begin 
		
			select HotelCode,HotelXML
			--,(case 
		 --  when [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') is not null and PATINDEX('%[0-9]%', [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') ) > 0 then 
		 --  case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1) else '0'
		 --  end 
		 --  when [HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)') is not null then 
		 --  case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1) else '0'
		 --  end 
		 --  else '0' 
		 --  end) 
			-- as res 
			from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock) 
			where case when  @cityname is null then 1
			 when HotelID like '%'+@cityname+'%'
			 then 1    
			 else 0
		     end = 1
		     and 
			 CityCode=@CityCode 
			 --OR HotelCode=@HotelCode
			set @retval=1
	End
	else
	Begin 

	

			select HotelCode,HotelXML,

			(case 

		   when [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') is not null and PATINDEX('%[0-9]%', [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') ) > 0 then 

		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1) else '0'

		   end 

		   when [HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)') is not null then 

		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1) else '0'

		   end 

		   else '0' 

		   end) 

			as res into #hb2 

			from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock) 			

			where case when  @cityname is null then 1
			 when HotelID like '%'+@cityname+'%'
			 then 1    
			 else 0
		     end = 1
		     and CityCode=@CityCode
			--or HotelCode=@HotelCode   

			select * from #hb2 with(nolock)

			where   res>=@MinRating  and  res<=@MaxRating

			set @retval=1

	End
	end

	--Hotel Code = giata id for hotel
	else if(@HotelCode is not null  )
	begin
   -- supplierid against giata id for hotel
	select hotelid into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where giataid=@HotelCode and localsupid=4
	 declare @count int =(select count(*) from #temp2)
	if(@count >0)
	begin
	select HotelCode,HotelXML
			from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock)	
			where HotelCode in(select hotelid from #temp2)
     set @retval=1
	 end

	 --Hotel Name if Supplier HotelCode not find against giata


	else if(@HotelName is not null )
	begin
		
	set @cityname=(select CityName from [tblSupplierCity_rel_new] with(nolock) where CityID=@CityID and supid=4)	


	if(@MinRating = '0' and @MaxRating='5')
	begin
	
	select HotelCode,HotelXML
	
	,([HotelXML].value('(//hotel/name)[1]', 'varchar(300)')) as nm  into #hb3
			from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock)	
			
			 where
			 case when  @cityname is null then 1
			 when HotelID like '%'+@cityname+'%'
			 then 1    
			 else 0
		     end = 1
		     and 
			 CityCode=@CityCode ;

			 select HotelCode,HotelXML from #hb3 with(nolock) 
		 	 where   nm like '%'+@HotelName+'%'
			 set @retval=1
    end
	else
	begin

	
    select HotelCode,HotelXML
	,(case 
		   when [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') is not null and PATINDEX('%[0-9]%', [HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)') ) > 0 then 
		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/category/@code)[1]', 'varchar(100)'),1) else '0'
		   end 
		   when [HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)') is not null then 
		   case when PATINDEX('%[0-9]%',left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1)) > 0 then left([HotelXML].value('(//hotel/@categoryCode)[1]', 'varchar(100)'),1) else '0'
		   end 
		   else '0' 
		   end) 
			as res 
	,([HotelXML].value('(//hotel/name)[1]', 'varchar(300)')) as nm
	  into #hb4
			from [StaticData].[dbo].[tblHB_Hotelstatic] as a with(nolock)	
			
			 where
			 case when  @cityname is null then 1
			 when HotelID like '%'+@cityname+'%'
			 then 1    
			 else 0
		     end = 1
		     and 
			 CityCode=@CityCode ;

			 select HotelCode,HotelXML from #hb4 with(nolock) 
		 	 where   
			 nm like '%'+@HotelName+'%'
		     and 
			 res>=@MinRating  and  res<=@MaxRating
			set @retval=1
			
       end



	end



	end


	

	SET NOCOUNT OFF;
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetHB_HtlDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_GetHB_HtlDetail]
@HotelCode nvarchar(150)=null,
@CityCode nvarchar(150)=null,
@retVal bit OUT
AS
BEGIN
   	BEGIN TRY
          select HotelCode,HotelXML from [StaticData].[dbo].[tblHB_Hotelstatic] with(nolock) where HotelCode=@HotelCode or CityCode=@CityCode
		  set @retval=1
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetHotelsPro_HotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================



-- Author:		<Suraj Singh>



-- Create date: <July 03, 2017>



-- Description:	<Procedure to get hotel static data from a particular hotel>



-- =============================================



CREATE PROCEDURE [dbo].[SP_GetHotelsPro_HotelDetail]



@HotelCode nvarchar(150)=null,



@retVal bit OUT



AS



BEGIN



   	BEGIN TRY

	SET NOCOUNT ON;

          select * from [StaticData].[dbo].[tblHPro_Hotelstatic] with(nolock) where HotelCode=@HotelCode;



				set @retval=1

SET NOCOUNT OFF;

    END TRY



	BEGIN CATCH



	       select @@TRANCOUNT



     END CATCH



END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetHotelsPro_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <June 14, 2017>

-- Modify date: <OCT 25, 2018>

-- Description:	<Procedure to get hotel static data from a particular city>

-- =============================================

CREATE PROCEDURE [dbo].[SP_GetHotelsPro_HotelList]

@CityCode nvarchar(150)=null,

@MinStarRating nvarchar(100)=null,

@MaxStarRating nvarchar(100)=null,

@retVal bit OUT

AS

BEGIN

   	BEGIN TRY

	SET NOCOUNT ON;

				declare @RegionCode nvarchar(50)

				set @RegionCode=(select distinct regions from [StaticData].[dbo].[HPro_CityDataRaw] with(nolock) where (citycode=@CityCode or parent=@CityCode) and regions is not null and regions <> '')	

				select CountryCode,HotelCode,HotelName,Star,Address,a.Latitude,a.Longitude, replace(MainImage,'small','large') MainImage ,b.cityname as area from [StaticData].[dbo].[tblHotelsProStaticData] a with(nolock) 

				inner join [StaticData].[dbo].[HPro_CityDataRaw] b with(nolock) on a.area=b.CityCode and (b.CityCode=@CityCode or b.parent=@CityCode or b.regions=@RegionCode) 

				and  CAST (a.Star as decimal)>=@MinStarRating AND CAST (a.Star as decimal)<=@MaxStarRating	

				--select JSON_VALUE(HotelJson,'$.country') as CountryCode,

				--HotelCode,HotelName,JSON_VALUE(HotelJson,'$.stars') as Star,

				--JSON_VALUE(HotelJson,'$.address') as Address,

				--JSON_VALUE(HotelJson,'$.latitude') as Latitude,

				--JSON_VALUE(HotelJson,'$.longitude') as Longitude,

				--JSON_VALUE(HotelJson,'$.images[0].thumbnail_images.mid') as MainImage,

				--b.cityname as area

				--from [StaticData].[dbo].[tblHPro_Hotelstatic] a with(nolock) 

				--inner join [StaticData].[dbo].[HPro_CityDataRaw] b with(nolock) on a.CityCode=b.CityCode and (b.CityCode=@CityCode or b.parent=@CityCode or b.regions=@RegionCode) 

				--and  CAST (JSON_VALUE(HotelJson,'$.stars') as decimal)>=@MinStarRating AND CAST (JSON_VALUE(HotelJson,'$.stars') as decimal)<=@MaxStarRating	

				set @retval=1

				SET NOCOUNT OFF;

    END TRY

	BEGIN CATCH

	       select @@TRANCOUNT

     END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetHotelsPro_HotelList_Test]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <June 14, 2017>

-- Modify date: <OCT 25, 2018>
-- Modify date: <Apr 24, 2019> 
-- Description:	<Procedure to get hotel static data from a particular city>

-- =============================================

CREATE PROCEDURE [dbo].[SP_GetHotelsPro_HotelList_Test]

@HotelCode nvarchar(150)=null,

@HotelName nvarchar(200)=null,
@CityCode nvarchar(150)=null,

@MinStarRating nvarchar(100)=null,

@MaxStarRating nvarchar(100)=null,

@retVal bit OUT

AS

BEGIN

   	BEGIN TRY

	SET NOCOUNT ON;

				declare @RegionCode nvarchar(50)

				--set @RegionCode=(select distinct regions from [StaticData].[dbo].[HPro_CityDataRaw] with(nolock) where (citycode=@CityCode or parent=@CityCode) and regions is not null and regions <> '')	

				if LEN(ISNULL(@HotelCode,''))=0  
				set @HotelCode=null;      
					if LEN(ISNULL(@HotelName,''))=0     
						set @HotelName=null;      
			

				if(@HotelCode is null  )
				begin
				select CountryCode,HotelCode,HotelName,Star,Address,a.Latitude,a.Longitude, replace(MainImage,'small','large') MainImage ,b.cityname as area from [StaticData].[dbo].[tblHotelsProStaticData] a with(nolock) 

				inner join [StaticData].[dbo].[HPro_CityDataRaw] b with(nolock) on a.area=b.CityCode and (b.CityCode=@CityCode or b.parent=@CityCode) -- or b.regions=@RegionCode) 

				and  CAST (a.Star as decimal)>=@MinStarRating AND CAST (a.Star as decimal)<=@MaxStarRating	

				
				end
				else  --Search By Giata HotelCode
				begin
				-- supplierid against giata id for hotel
	          select hotelid into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where giataid=@HotelCode and localsupid=6
			  declare @count int =(select count(*) from #temp2)
	           if(@count >0)
	            begin
				

					select CountryCode,HotelCode,HotelName,Star,Address,a.Latitude,a.Longitude, replace(MainImage,'small','large') MainImage ,b.cityname as area from [StaticData].[dbo].[tblHotelsProStaticData] a with(nolock) 

					inner join [StaticData].[dbo].[HPro_CityDataRaw] b with(nolock) on a.area=b.CityCode and (b.CityCode=@CityCode or b.parent=@CityCode) -- or b.regions=@RegionCode) 

					--and  CAST (a.Star as decimal)>=@MinStarRating AND CAST (a.Star as decimal)<=@MaxStarRating	
					and HotelCode in(select hotelid from #temp2)
				end
				else    if(@HotelName is not null)  --Search By Giata HotelName if Supplier Hotel Id not Find in Giata
				begin
				

						select CountryCode,HotelCode,HotelName,Star,Address,a.Latitude,a.Longitude, replace(MainImage,'small','large') MainImage ,b.cityname as area,b.citycode from [StaticData].[dbo].[tblHotelsProStaticData] a with(nolock) 

						inner join [StaticData].[dbo].[HPro_CityDataRaw] b with(nolock) on a.area=b.CityCode and (b.CityCode=@CityCode or b.parent=@CityCode) -- or b.regions=@RegionCode) 

						--and  CAST (a.Star as decimal)>=@MinStarRating AND CAST (a.Star as decimal)<=@MaxStarRating	
						and HotelName like '%'+@HotelName+'%'
				end



				end


				set @retval=1

				SET NOCOUNT OFF;

    END TRY

	BEGIN CATCH

	       select @@TRANCOUNT

     END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetHotelsUsingTrackNum]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Ragini Awasthi>
-- Create date: <Jun 01, 2018>
-- Mofify date: <Jun 01, 2018>
-- Description:	<Procedure to get room list>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetHotelsUsingTrackNum]
@TrackNumber nvarchar(500)=null,
@retVal bit OUT
AS
BEGIN
   	BEGIN TRY	
	Begin
			select logresponseXML from tblapilog with(nolock)
			where TrackNumber=@TrackNumber
			and supplierID=0 and logType='Search'
			
	End
    END TRY
	BEGIN CATCH
	     
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetJuniper_HSearchData]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetJuniper_HSearchData]
@HotelID nvarchar(max) = null,
@sqlCommand nvarchar(max) = null,
@SupID nvarchar(20),
@retVal bit out
as	 
begin
	begin try
	 --   select @HotelID
		--select * from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)
		--where HotelID in( @HotelID) and Caption ='HOTEL'
		--set @retVal = 1
		
		if(@SupID=16)
		begin
			-- W2M--
			SET @sqlCommand = 'select * from [StaticData].[dbo].[tblW2MImages] with (nolock)
				where HotelID in ('+  @HotelID+')'
		end	
		if(@SupID=17)
		begin
			-- Egypt Express --
			SET @sqlCommand = 'select * from [StaticData].[dbo].[tblEgyptExpImages] with (nolock)
				where HotelID in ('+  @HotelID+')'
		end	
		if(@SupID=41)
		begin
			-- Alpha Tours--
			SET @sqlCommand = 'select * from [StaticData].[dbo].[tblAlphatourImages] with (nolock)
				where HotelID in ('+  @HotelID+')'
		end	

		
		EXEC (@sqlCommand)
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end



GO
/****** Object:  StoredProcedure [dbo].[SP_GetLog_XMLs]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetLog_XMLs]      
      
@transid nvarchar(60) = null,      
      
@logtypeID int,      
      
@Supplier int,      
      
@retVal bit out      
      
as      
      
begin       
      
 begin try      
    if(@logtypeID=1)  
 begin  
 select logresponseXML,  logrequestXML,logType,logcreatedOn, TrackNumber from tblapilog_search with (nolock)      
 where TrackNumber = @transid and logTypeID = @logtypeID and SupplierID = @Supplier       
 order by logid desc       
 set @retVal =1      
    end 
	if(@logtypeID=2)  
 begin  
 select logresponseXML,  logrequestXML,logType,logcreatedOn, TrackNumber from tblapilog_room with (nolock)      
 where TrackNumber = @transid and logTypeID = @logtypeID and SupplierID = @Supplier       
 order by logid desc       
 set @retVal =1      
    end 
 else  
 begin  
 select logresponseXML,  logrequestXML,logType,logcreatedOn, TrackNumber from tblapilog with (nolock)      
 where TrackNumber = @transid and logTypeID = @logtypeID and SupplierID = @Supplier       
 order by logid desc       
 set @retVal =1    
 end  
 end try      
      
      
      
 begin catch      
      
  select @@TRANCOUNT      
      
 end catch      
      
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetLog_XMLs_Out]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE proc [dbo].[SP_GetLog_XMLs_Out]

@transid nvarchar(60) = null,

@logtypeID int,

@Supplier int,

@custID varchar(100)=null,

@retVal bit out

as

begin 



	begin try



	select logresponseXML,  logrequestXML,logType,logcreatedOn, TrackNumber from tblapilogOut with (nolock)

	where TrackNumber = @transid and logTypeID = @logtypeID and SupplierID = @Supplier 

	and customerID=@custID

	order by logid desc 

	set @retVal =1

	end try

	begin catch

		select @@TRANCOUNT





	end catch



end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetLog_XMLs_Out_tbo]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




create proc [dbo].[SP_GetLog_XMLs_Out_tbo]

@transid nvarchar(60) = null,

@logtypeID int,

@Supplier int,

@custID varchar(100)=null,

@retVal bit out

as

begin 



	begin try



	select top 1 logresponseXML,  logrequestXML,logType,logcreatedOn, TrackNumber from tblapilogOut with (nolock)

	where TrackNumber = @transid and logTypeID = @logtypeID and SupplierID = @Supplier 

	and customerID=@custID

	order by logid desc 

	set @retVal =1

	end try

	begin catch

		select @@TRANCOUNT





	end catch



end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetLog_XMLsOut]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetLog_XMLsOut]
@transid nvarchar(60) = null,
@logtypeID int,
@Supplier int,
@custID varchar(100)=null,
@retVal bit out
as
begin 
	begin try
	select logresponseXML,  logrequestXML,logType,logcreatedOn, TrackNumber from tblapilogOut with (nolock)
	where TrackNumber = @transid and logTypeID = @logtypeID and SupplierID = @Supplier and customerID = @custID 
	order by logid desc 
	set @retVal =1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetMiki_Facilities]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetMiki_Facilities]
@CityID nvarchar(50) = null,
@retVal bit out
as 
begin
	begin try
			select * from [StaticData].[dbo].[MikiFacilityData] with (nolock)
			where CityID = @CityID
			set @retVal = 1
	end try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetMiki_HotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetMiki_HotelDetails]
@CityID nvarchar(50) = null,
@retVal bit out
as
begin	
	begin try
			select * from [StaticData].[dbo].MikiStaticData with (nolock)
			where CityID = @CityID and HotelID='CAN873500'
			set @retVal = 1
	end try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetMiki_Images]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[SP_GetMiki_Images]
@CityID nvarchar(20) = null,
@retVal bit OUT
 as
 begin
	begin try
			select  *  from [StaticData].[dbo].[tblMikiImages] with (nolock)
			where CityID = @CityID
			set @retVal =1
	End try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetMiki_RoomList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  
CREATE proc [dbo].[SP_GetMiki_RoomList]  
@transid nvarchar(60) = null,  
@retVal bit out  
as  
begin   
begin try  
select logresponseXML, TrackNumber from tblapilog_search with (nolock)  
 where TrackNumber =@transid and logTypeID = 1 and SupplierID = 11  
 set @retVal = 1  
end try  
begin catch  
select @@TRANCOUNT  
end catch  
end  
  
GO
/****** Object:  StoredProcedure [dbo].[SP_GetMiki_RoomList_Out]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE proc [dbo].[SP_GetMiki_RoomList_Out]

@transid nvarchar(60) = null,

@retVal bit out

as

begin 

begin try

select logresponseXML, TrackNumber from tblapilogOut with (nolock)

 where TrackNumber =@transid and logTypeID = 1 and SupplierID = 11

 set @retVal = 1

end try

begin catch

select @@TRANCOUNT

end catch

end


GO
/****** Object:  StoredProcedure [dbo].[sp_getprebookResp]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[sp_getprebookResp]

 @tracknumber varchar(500),

 @custid varchar(500)

AS 

begin

select top 1 logid,logresponsexml,logrequestXML from tblapilogout where tracknumber=@tracknumber and preid=@custid and logTypeID=4 and SupplierID=0   order by logid desc

END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRestel_Facilities]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[SP_GetRestel_Facilities]
@CityID nvarchar(20)= null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblRestelFacilityData]
		where CityID = @CityID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRestel_HotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_GetRestel_HotelDetails]
@HotelID nvarchar(20) = null,
@retVal bit out
as
begin 
	begin try
		select * from [StaticData].[dbo].[RestelHotelDetails] with (nolock)
		where HotelID = @HotelID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRestel_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetRestel_HotelList]
@CityID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
	    select * from [StaticData].[dbo].[tblRestelHotelList] with (nolock)
		where CityID in (select SupCityID from tblSupplierCity_rel_new where CityId = @CityID and supId = 13)
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRestel_HotelList_Test]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetRestel_HotelList_Test]
@CityID nvarchar(10) = null,
@HotelCode nvarchar(10) = null,
@HotelName nvarchar(200)=null,
@retVal bit out
as
begin
	begin try
	if LEN(ISNULL(@HotelCode,''))=0        
				set @HotelCode=null
    if LEN(ISNULL(@HotelName,''))=0        
				set @HotelName=''


    -- declare @city nvarchar(50)=(select SupCityID from [tblSupplierCity_rel_new] with(nolock) where CityID=@CityID and supid=13)	
     if(@HotelCode is null)--Search By City
	    begin
		select * from [StaticData].[dbo].[tblRestelHotelList] with (nolock) --select * from [TravayooDummy].[dbo].Restelhotellist_new_13112018 with (nolock)
		where CityID in (select SupCityID from [tblSupplierCity_rel_new] with(nolock) where CityID=@CityID and supid=13)
		set @retVal = 1
		end
		else --Search By GiataHotelId
		begin
		  select hotelid  into #temp from [StaticData]..[tblgiatadetails]  with (nolock) where 	giataid=@HotelCode and	 localsupid=13
		  declare @count int =(select count(*) from #temp)
		  if(@count >0)
			 begin
				select *  from [StaticData].[dbo].[tblRestelHotelList] with (nolock) --select * from [TravayooDummy].[dbo].Restelhotellist_new_13112018 with (nolock)
				where HotelId in (select hotelid from #temp)
				set @retVal = 1
			end
		 -- else if(@HotelName is not null )--Search By Name
			--begin
			--   select hotelid  into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where 	cityid=@CityID and	 localsupid=13 and hotelname like '%'+@HotelName+'%'
			--	select *  from [StaticData].[dbo].[tblRestelHotelList] with (nolock) 
			--	where HotelId in (select hotelid from #temp2)
			--	set @retval=1
			--end
		end
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end

GO
/****** Object:  StoredProcedure [dbo].[SP_GetRestel_RoomList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetRestel_RoomList]  
@transid nvarchar(60) = null,  
@retVal bit out  
as  
begin   
begin try  
select logresponseXML, TrackNumber from tblapilog_search with (nolock)  
 where TrackNumber =@transid and logTypeID = 1 and SupplierID = 13  
 set @retVal = 1  
end try  
begin catch  
select @@TRANCOUNT  
end catch  
end  
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRestel_RoomList_Out]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetRestel_RoomList_Out]

@transid nvarchar(60) = null,

@custID nvarchar(60) = null,

@retVal bit out

as

begin 

begin try

select logresponseXML, TrackNumber from tblapilogOut with (nolock)

 where TrackNumber =@transid and logTypeID = 1 and SupplierID = 13
 and customerID=@custID

 set @retVal = 1

end try

begin catch

select @@TRANCOUNT

end catch

end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_Darina]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <Jun 01, 2018>
-- Mofify date: <Jun 01, 2018>
-- Description:	<Procedure to get room list>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetRoom_Darina]
@TrackNumber nvarchar(500)=null,
@retVal bit OUT
AS
BEGIN
   	BEGIN TRY	
	Begin
			select logresponseXML from tblapilog with(nolock)
			where TrackNumber=@TrackNumber
			and supplierID=1 and logTypeID=1
			set @retval=1
	End
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_Darina_merge]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================      
      
-- Author:  <Suraj Singh>      
      
-- Create date: <Mar 14, 2019>      
      
-- Mofify date: <Mar 14, 2019>      
      
-- Description: <Procedure to get room list>      
      
-- =============================================      
      
CREATE PROCEDURE [dbo].[SP_GetRoom_Darina_merge]      
      
@TrackNumber nvarchar(500)=null,      
      
@cusdID nvarchar(500)=null,      
      
@retVal bit OUT      
      
AS      
      
BEGIN      
      
    BEGIN TRY       
      
 Begin      
      
   select logresponseXML from tblapilog_search with(nolock)      
      
   where TrackNumber=@TrackNumber and customerID=@cusdID      
      
   and supplierID=1 and logTypeID=1      
      
   set @retval=1      
      
 End      
      
    END TRY      
      
 BEGIN CATCH      
      
        select @@TRANCOUNT      
      
     END CATCH      
      
END      
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_Darina_Out]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================

-- Author:		<Manisha/Surajh>

-- Create date: <Jun 07, 2019>



-- Description:	<Procedure to get room list>

-- =============================================

CREATE PROCEDURE [dbo].[SP_GetRoom_Darina_Out]

@TrackNumber nvarchar(500)=null,

@preID varchar(500)=null,

@retVal bit OUT

AS

BEGIN

   	BEGIN TRY	

	Begin

			select logresponseXML from tblapilog with(nolock)

			where TrackNumber=@TrackNumber

			and supplierID=1 and logTypeID=1 and preID=@preID

			set @retval=1

	End

    END TRY

	BEGIN CATCH

	       select @@TRANCOUNT

     END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_Darina_xmlOut]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================



-- Author:		<Suraj Singh>



-- Create date: <Jun 01, 2018>



-- Mofify date: <Jun 01, 2018>



-- Description:	<Procedure to get room list>



-- =============================================



CREATE PROCEDURE [dbo].[SP_GetRoom_Darina_xmlOut]

@TrackNumber nvarchar(500)=null,
@custID varchar(100)=null,

@retVal bit OUT


AS



BEGIN



   	BEGIN TRY	



	Begin



			select logresponseXML from tblapilogOut with(nolock)



			where TrackNumber=@TrackNumber



			and supplierID=1 and logTypeID=1
			and customerID=@custID



			set @retval=1



	End



    END TRY



	BEGIN CATCH



	       select @@TRANCOUNT



     END CATCH



END

GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_HB]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================



-- Author:		<Suraj Singh>



-- Create date: <June 14, 2017>



-- Mofify date: <June 11, 2018>



-- Description:	<Procedure to get rooms using hotelcode>



--exec [SP_GetRoom_HB] 'bd3503d5-e795-4177-9fe0-0b44714f2f62','84242','getroom',0



-- =============================================



CREATE PROCEDURE [dbo].[SP_GetRoom_HB]



@tracknumber nvarchar(500)=null,



@xmltype nvarchar(500)=null,



@hotelcode nvarchar(150)=null,



@type nvarchar(50)=null,



@retVal bit OUT



AS



BEGIN



   	BEGIN TRY       



	if(@type='getroom') 



	begin  

	if(@xmltype is null)

	begin

	select @xmltype=''

	end



		select logresponseXML from tblapilog with(nolock) where TrackNumber=@tracknumber and logTypeID=1 and SupplierID=4 and logMsg=@xmltype



		and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1



		set @retval=1



	end



	if(@type='cxlpolicy') 



	begin  

	if(@xmltype=null)

	begin

	select @xmltype=''

	end

		select logresponseXML from tblapilog with(nolock) where TrackNumber=@tracknumber and logTypeID=3 and SupplierID=4  and logMsg=@xmltype



		and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1



		set @retval=1



	end



    END TRY



	BEGIN CATCH



	       select @@TRANCOUNT



     END CATCH



END
































GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_HB_Giata]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <June 14, 2017>
-- Mofify date: <June 11, 2018>
-- Description:	<Procedure to get rooms using hotelcode>
--exec [SP_GetRoom_HB] 'bd3503d5-e795-4177-9fe0-0b44714f2f62','84242','getroom',0
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetRoom_HB_Giata]
@tracknumber nvarchar(500)=null,
@xmltype nvarchar(500)=null,
@hotelcode nvarchar(150)=null,
@type nvarchar(50)=null,
@retVal bit OUT
AS
BEGIN
   	BEGIN TRY 

	if(@type='getroom') 


	begin  



	if(@xmltype is null)



	begin



	select @xmltype=''



	end







		--select logresponseXML from tblapilog with(nolock) where TrackNumber=@tracknumber and logTypeID=1 and SupplierID=4 and logMsg=@xmltype







		--and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1





		
		select x.query('.') as logresponseXML  from tblapilog as a with(nolock)  
		CROSS APPLY A.logresponseXML.nodes('availabilityRS/hotels/hotel') as N(X)
        WHERE TrackNumber=@tracknumber 
		and logTypeID=1 and SupplierID=4 and logMsg=@xmltype 
		and X.value('@code','varchar(100)')=@hotelcode




		set @retval=1







	end







	if(@type='cxlpolicy') 







	begin  



	if(@xmltype=null)



	begin



	select @xmltype=''



	end



		select logresponseXML from tblapilog with(nolock) where TrackNumber=@tracknumber and logTypeID=3 and SupplierID=4  and logMsg=@xmltype







		and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1







		set @retval=1







	end







    END TRY







	BEGIN CATCH







	       select @@TRANCOUNT







     END CATCH







END
































































GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_HB_Giata_merge]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================        
-- Author:  <Suraj Singh>        
-- Create date: <June 14, 2017>        
-- Mofify date: <June 11, 2018>        
-- Mofify date: <Feb 13, 2024>        
-- Description: <Procedure to get rooms using hotelcode>        
--exec [SP_GetRoom_HB] 'bd3503d5-e795-4177-9fe0-0b44714f2f62','84242','getroom',0        
-- =============================================        
CREATE PROCEDURE [dbo].[SP_GetRoom_HB_Giata_merge]        
@tracknumber nvarchar(500)=null,        
@xmltype nvarchar(500)=null,        
@hotelcode nvarchar(150)=null,        
@custID nvarchar(150)=null,        
@type nvarchar(50)=null,        
@retVal bit OUT        
AS        
BEGIN        
    BEGIN TRY         
 if(@type='getroom')         
 begin          
 if(@xmltype is null)        
 begin        
 select @xmltype=''        
 end        
  ----select logresponseXML from tblapilog with(nolock) where TrackNumber=@tracknumber and logTypeID=1 and SupplierID=4 and logMsg=@xmltype        
  ----and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1        
  --select x.query('.') as logresponseXML  from tblapilog_search as a with(nolock)          
  --CROSS APPLY A.logresponseXML.nodes('availabilityRS/hotels/hotel') as N(X)        
  --      WHERE TrackNumber=@tracknumber         
  --and logTypeID=1 and SupplierID=4 and logMsg=@xmltype and customerID=@custID        
  --and X.value('@code','varchar(100)')=@hotelcode  
  
  
		IF OBJECT_ID('tempdb..#xmlresp') IS NOT NULL DROP TABLE #xmlresp
		select cast(logresponseXML as xml) as logresponseXML  into #xmlresp from tblapilog_search with(nolock) where 
		 TrackNumber=@tracknumber         
		and logTypeID=1 and SupplierID=4 and logMsg=@xmltype and customerID=@custID

		 select x.query('.') as logresponseXML  
		from #xmlresp as a with(nolock)          
		CROSS APPLY  
			 A.logresponseXML.nodes('availabilityRS/hotels/hotel')   as N(X) 
		WHERE      
		 X.value('@code','varchar(100)')=@hotelcode 


  set @retval=1        
 end        
 if(@type='cxlpolicy')         
 begin          
 if(@xmltype=null)        
 begin        
 select @xmltype=''        
 end        
  select logresponseXML from tblapilog with(nolock) where TrackNumber=@tracknumber and logTypeID=3 and SupplierID=4  and logMsg=@xmltype and customerID=@custID        
  and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1        
  set @retval=1        
 end        
    END TRY        
 BEGIN CATCH        
        select @@TRANCOUNT        
     END CATCH        
END 
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoom_HB_Giata_Out]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <Aug 13, 2019>

-- Mofify date: <Aug 13, 2019>

-- Description:	<Procedure to get rooms using hotelcode>

--exec [SP_GetRoom_HB] 'bd3503d5-e795-4177-9fe0-0b44714f2f62','84242','getroom',0

-- =============================================

CREATE PROCEDURE [dbo].[SP_GetRoom_HB_Giata_Out]

@tracknumber nvarchar(500)=null,

@xmltype nvarchar(500)=null,

@hotelcode nvarchar(150)=null,
@type nvarchar(50)=null,
@custID varchar(100)=null,
@retVal bit OUT

AS

BEGIN
   	BEGIN TRY 
	if(@type='getroom') 
	begin  
	if(@xmltype is null)
	begin
	select @xmltype=''
	end

		--select logresponseXML from tblapilog with(nolock) where TrackNumber=@tracknumber and logTypeID=1 and SupplierID=4 and logMsg=@xmltype

		--and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1
		select x.query('.') as logresponseXML  from tblapilogOut as a with(nolock)  

		CROSS APPLY A.logresponseXML.nodes('availabilityRS/hotels/hotel') as N(X)

        WHERE TrackNumber=@tracknumber and customerID=@custID

		and logTypeID=1 and SupplierID=4 and logMsg=@xmltype 

		and X.value('@code','varchar(100)')=@hotelcode
		set @retval=1
	end
	if(@type='cxlpolicy')
	begin  
	if(@xmltype=null)
	begin
	select @xmltype=''
	end
		select logresponseXML from tblapilogOut with(nolock) where TrackNumber=@tracknumber and logTypeID=3 and SupplierID=4  and logMsg=@xmltype
		and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1
		and customerID=@custID
		set @retval=1
	end
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT

     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetRoomGiata_HB]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <Nov 24, 2017>

-- Description:	<Procedure to get rooms using hotelcode and xml type>

--exec [SP_GetRoomGiata_HB] 'bd3503d5-e795-4177-9fe0-0b44714f2f62','84242','getroom',0

-- =============================================

CREATE PROCEDURE [dbo].[SP_GetRoomGiata_HB]

@tracknumber nvarchar(500)=null,

@hotelcode nvarchar(150)=null,

@xmltype nvarchar(150)=null,

@type nvarchar(50)=null,

@retVal bit OUT

AS

BEGIN

   	BEGIN TRY   

	if(@type='getroom') 

	begin  

	if(@xmltype is not null)
	Begin
		select logresponseXML from tblapilog where TrackNumber=@tracknumber and logTypeID=1 and SupplierID=4 and logMsg=@xmltype

		and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1

				set @retval=1
				End
				else
				begin
				select logresponseXML from tblapilog where TrackNumber=@tracknumber and logTypeID=1 and SupplierID=4 --and logMsg=@xmltype

		and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1

				set @retval=1
				end

	end

	if(@type='cxlpolicy') 

	begin  

		select logresponseXML from tblapilog where TrackNumber=@tracknumber and logTypeID=3 and SupplierID=4 and logMsg=@xmltype

		and logresponseXML.exist('(/availabilityRS/hotels/hotel[@code=sql:variable("@hotelcode")])') = 1

				set @retval=1

	end

    END TRY

	BEGIN CATCH

	       select @@TRANCOUNT

     END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetSalTour_Facilities]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[SP_GetSalTour_Facilities]
@HotelID nvarchar(40) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblSalToursHotelFacilities] with (nolock)
		where HotelID  = @HotelID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetSalTour_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetSalTour_HotelList]
@CityID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[SalTours_HotelsList] with (nolock)
		where [City Code] = @CityID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end



GO
/****** Object:  StoredProcedure [dbo].[SP_GetSalTour_Images]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetSalTour_Images]
@HotelID nvarchar(max) = null,
@sqlCommand varchar(max) = null,
@retVal bit out
as	 
begin
	begin try
	 --   select @HotelID
		--select * from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)
		--where HotelID in( @HotelID) and Caption ='HOTEL'
		--set @retVal = 1
		
		SET @sqlCommand = 'select HotelID,	HotelName,	ImageUrl,	Caption from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)
				where HotelID in ('+  @HotelID+')'
		EXEC (@sqlCommand)
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end



GO
/****** Object:  StoredProcedure [dbo].[SP_GetSalTour_Images1]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_GetSalTour_Images1]
@HotelID nvarchar(max) = null,
@sqlCommand varchar(max) = null,
@retVal bit out
as	 
begin
	begin try
	 --   select @HotelID
		--select * from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)
		--where HotelID in( @HotelID) and Caption ='HOTEL'
		--set @retVal = 1
		
		SET @sqlCommand = 'select HotelID,	HotelName,	ImageUrl,	Caption from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)
				where HotelID in ('+  @HotelID+')'
		EXEC (@sqlCommand)
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end



GO
/****** Object:  StoredProcedure [dbo].[SP_GetSalTour_SingleHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetSalTour_SingleHotelDetail]
@HotelID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[SalTours_HotelsList] with (nolock)
		where [HotelCode]  = @HotelID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end



GO
/****** Object:  StoredProcedure [dbo].[SP_GetSmyData]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [dbo].[SP_GetSmyData]
@retVal bit OUT
 as
 begin
	begin try
			select  *  from staticdata.dbo.SmyHotelDetails with (nolock)			
			set @retVal =1
	End try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetSunHotel_Facilities]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_GetSunHotel_Facilities]
@HotelID nvarchar(max) = null,
@sqlCommand varchar(max) = null,
@retVal bit out
as	 
begin
	begin try
	 --   select @HotelID
		--select * from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)
		--where HotelID in( @HotelID) and Caption ='HOTEL'
		--set @retVal = 1
		
		SET @sqlCommand = 'select * from [StaticData].[dbo].[tblSunHotelFacilities] with (nolock)
				where HotelID in ('+  @HotelID+')'
		EXEC (@sqlCommand)
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetSunHotel_HotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetSunHotel_HotelDetails]
@CityID nvarchar(max) = null,
@sqlCommand varchar(max) = null,
@retVal bit out
as
begin
	begin try
		--select * from [StaticData].[dbo].[tblSunHotelDetails] with (nolock)
		--where CityID = @CityID


		SET @sqlCommand = 'select * from [StaticData].[dbo].[tblSunHotelDetails] with (nolock)
				where CityID in('+@CityID+')'
		EXEC (@sqlCommand)
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetSunHotel_HotelDetails_Unique]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_GetSunHotel_HotelDetails_Unique]
@HotelID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblSunHotelDetails] with (nolock)
		where HotelID = @HotelID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetSunHotel_Images]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetSunHotel_Images]

@HotelID nvarchar(max) = null,

@CityID nvarchar(max) = null,

@sqlCommand varchar(max) = null,

@retVal bit out

as	 

begin

	begin try

	 --   

		SET @sqlCommand = 'select * from [StaticData].[dbo].[tblSunHotelImages] with (nolock)
				where CityID in('+@CityID+') and HotelID in ('+  @HotelID+')'

--		SET @sqlCommand = 'SELECT  HotelID,HotelName,FullSizeImage,SmallImage,ImageID,CityID,CityName
--FROM    (SELECT HotelID,HotelName,FullSizeImage,SmallImage,ImageID,CityID,CityName,
--                ROW_NUMBER() OVER (PARTITION BY HotelID ORDER BY HotelID) AS RowNumber
--         FROM   [StaticData].[dbo].[tblSunHotelImages] with(nolock)
--         WHERE  CityID in('+@CityID+') and HotelID in ('+  @HotelID+')) AS a
--WHERE   a.RowNumber = 1'

		EXEC (@sqlCommand)

		set @retVal = 1

	end try

	begin catch

		select @@TRANCOUNT

	end catch

end






GO
/****** Object:  StoredProcedure [dbo].[SP_GetSunHotel_ResortList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[SP_GetSunHotel_ResortList]
@CityID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblSunHotelResortList] with (nolock)
		where CityID = @CityID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetSunHotel_SingleHotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetSunHotel_SingleHotelDetails]
--@HotelID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblSunHotelRoomDetails] with (nolock)
		--	where HotelID = @HotelID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetTBO_CityData]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_GetTBO_CityData]
@CityID varchar(10) = null,
@retval bit out
as
begin 
	begin try
		select * from StaticData.dbo.tblTBoCityData with(nolock) where CityID = @CityID
		set @retval = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetTBO_HotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create Proc [dbo].[SP_GetTBO_HotelDetail]
@HotelID nvarchar(20) = null,
@retVal bit OUT
 as
 begin
	begin try
			select  *  from [StaticData].[dbo].[tblTBOStaticData] with (nolock)
			where HotelID = @HotelID
			set @retVal =1
	End try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetTBO_Images]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create Proc [dbo].[SP_GetTBO_Images]
@HotelID nvarchar(20) = null,
@retVal bit OUT
 as
 begin
	begin try
			select  *  from [StaticData].[dbo].[tblTBOImages] with (nolock)
			where HotelID = @HotelID
			set @retVal =1
	End try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetTBO_TagInfo]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[SP_GetTBO_TagInfo]
@CountryID int,
@retVal bit OUT
 as
 begin
	begin try
			select  *  from [StaticData].[dbo].[tblTboTagInfo] with (nolock)
			where localcountryid = @CountryID
			set @retVal =1
	End try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetTourico_HotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================



-- Author:		<Suraj Singh>



-- Create date: <June 14, 2017>



-- Description:	<Procedure to get hotel detail using hotelid>



-- =============================================



CREATE PROCEDURE [dbo].[SP_GetTourico_HotelDetail]



@hotelid nvarchar(150)=null,



@retVal bit OUT



AS



BEGIN



   	BEGIN TRY



          select HotelId,HotelInfoRespone from [staticdata].[dbo].[tblHotelMaster] with(nolock) where HotelId=@hotelid and Deleted=0

				set @retval=1

    END TRY

	BEGIN CATCH

	       select @@TRANCOUNT

     END CATCH







END
GO
/****** Object:  StoredProcedure [dbo].[SP_GetW2M_Facilities]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create proc [dbo].[SP_GetW2M_Facilities]
@CityID nvarchar(20)= null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblW2MFacilities]
		where CityID = @CityID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetW2M_HotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[SP_GetW2M_HotelDetails]
@HotelID nvarchar(20) = null,
@retVal bit out
as
begin 
	begin try
		select * from [StaticData].[dbo].[tblW2MHotelDetails] with (nolock)
		where HotelID = @HotelID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_GetW2M_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[SP_GetW2M_HotelList]
@CityID nvarchar(10) = null,
@retVal bit out
as
begin
	begin try
		select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)
		where CityID = @CityID
		set @retVal = 1
	end try
	begin catch
		select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_HBCXLPolicyTransfer]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <Nov 06, 2017>
-- Mofify date: <Nov 13, 2017>
-- Description:	<Procedure to GET cancellation policies for hotelbeds transfers>
-- =============================================
CREATE PROCEDURE [dbo].[SP_HBCXLPolicyTransfer]
@TrackNumber nvarchar(1500)=null,
@retVal bit OUT
AS
BEGIN
   	BEGIN TRY
          select logresponsexml from tblapilog with(nolock) 
		  where TrackNumber=@TrackNumber and SupplierID=10 
		  set @retval=1
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertAPILog]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <April 27, 2017>
-- Mofify date: <APR 10, 2018>
-- Description:	<Procedure to insert api logs>

-- =============================================

CREATE PROCEDURE [dbo].[SP_InsertAPILog]
	(
		@customerID bigint=null,
		@TrackNumber nvarchar(max)=null,
		@logTypeID bigint=null,
		@logType nvarchar(max)=null,
		@SupplierID bigint=null,
		@logMsg nvarchar(max)=null,
		@logrequestXML nvarchar(max)=null,
		@logresponseXML nvarchar(max)=null,
		@logStatus tinyint=null,
		@StartTime datetime2(7)=null,
		@EndTime datetime2(7)=null,
		@retVal bit OUT
	)
	AS
	BEGIN
		if(@StartTime is null)
			BEGIN
				select @StartTime=getDate()
			END
		if(@EndTime is null)
			BEGIN
				select @EndTime=getDate()
			END
		SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION
		BEGIN TRY
				declare @ip nvarchar(100)=null;
				set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));
				Insert into tblapilog (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip)
				values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip)
				set @retval=1
				COMMIT TRANSACTION
				SET NOCOUNT OFF;
		END TRY
		BEGIN CATCH
		
		       declare @error nvarchar(2000), @errNo nvarchar(2000)
			   set @errNo= ERROR_NUMBER()
               set @error = error_message()
			   print @error
			   print @errNo
				IF (@@TRANCOUNT>0)
				BEGIN		
					Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)
					values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)
					set @retval=0
					--ROLLBACK TRANSACTION
				END
		END CATCH
	END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertApilogFail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_InsertApilogFail]
(
@customerID bigint=null,
@TrackNumber nvarchar(max)=null,
@logTypeID bigint=null,
@logType nvarchar(max)=null,
@SupplierID bigint=null,
@logMsg nvarchar(max)=null,
@logrequestXML nvarchar(max)=null,
@logresponseXML nvarchar(max)=null,
@logStatus tinyint=null,
@StartTime datetime2(7)=null,
@EndTime datetime2(7)=null,
@retVal bit OUT
)
AS
BEGIN
if(@StartTime is null)
BEGIN
select @StartTime=getDate()
END
if(@EndTime is null)
BEGIN
select @EndTime=getDate()
END
       
			Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)
			values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)
		   set @retval=0
		  
END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertAPILogflt]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <April 17, 2019>

-- Mofify date: <APR 17, 2019>

-- Description:	<Procedure to insert api logs>

-- =============================================

CREATE PROCEDURE [dbo].[SP_InsertAPILogflt]

(

@preID nvarchar(max)=null,

@customerID bigint=null,

@TrackNumber nvarchar(max)=null,

@logTypeID bigint=null,

@logType nvarchar(max)=null,

@SupplierID bigint=null,

@logMsg nvarchar(max)=null,

@logrequestXML nvarchar(max)=null,

@logresponseXML nvarchar(max)=null,

@logStatus tinyint=null,

@StartTime datetime2(7)=null,

@EndTime datetime2(7)=null,

@retVal bit OUT

)

AS

BEGIN

if(@StartTime is null)

BEGIN

select @StartTime=getDate()

END

if(@EndTime is null)

BEGIN

select @EndTime=getDate()

END

        SET NOCOUNT ON;

		SET XACT_ABORT ON

		BEGIN TRANSACTION

		BEGIN TRY

		declare @ip nvarchar(100)=null;

		set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));

			Insert into tblapilogflt (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip,preID)

			values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip,@preID)

			set @retval=1

		COMMIT TRANSACTION

		 SET NOCOUNT OFF;

		END TRY

	BEGIN CATCH

	 IF (@@TRANCOUNT>0)

		BEGIN		

			Insert into tblapilogFailTransflt (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip,preID)

			values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip,@preID)

		   set @retval=0

		   --ROLLBACK TRANSACTION

		END

	END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertAPILogfltfail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <JUL 22, 2019>

-- Mofify date: <JUL 22, 2019>

-- Description:	<Procedure to insert api logs>

-- =============================================

CREATE PROCEDURE [dbo].[SP_InsertAPILogfltfail]

(

@preID nvarchar(max)=null,

@customerID bigint=null,

@TrackNumber nvarchar(max)=null,

@logTypeID bigint=null,

@logType nvarchar(max)=null,

@SupplierID bigint=null,

@logMsg nvarchar(max)=null,

@logrequestXML nvarchar(max)=null,

@logresponseXML nvarchar(max)=null,

@logStatus tinyint=null,

@StartTime datetime2(7)=null,

@EndTime datetime2(7)=null,

@retVal bit OUT

)

AS

BEGIN

if(@StartTime is null)

BEGIN

select @StartTime=getDate()

END

if(@EndTime is null)

BEGIN

select @EndTime=getDate()

END

        SET NOCOUNT ON;

		SET XACT_ABORT ON

		BEGIN TRANSACTION

		BEGIN TRY

		declare @ip nvarchar(100)=null;

		set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));

			Insert into tblapilogFailTransflt (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip,preID)

			values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip,@preID)

			set @retval=1

		COMMIT TRANSACTION

		 SET NOCOUNT OFF;

		END TRY

	BEGIN CATCH

	 IF (@@TRANCOUNT>0)

		BEGIN

		   set @retval=0

		END

	END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertCountryInOfferId]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_InsertCountryInOfferId]   
 @offerID bigint,
 @Result VARCHAR(50) OUTPUT
AS  
BEGIN  

 Declare @offrID bigint ,@CountriesID nvarchar(max)
 DECLARE @start INT, @end INT ,@delimiter nvarchar(1)
 select @delimiter=',';

 BEGIN
 DELETE FROM htlExtPromotion_CountriesInOfferId WHERE offrID = @offerID
 END
 DECLARE curCountries CURSOR FOR SELECT DISTINCT offerID, CountriesID from htlExtPromotionChildRelation where offerID = @offerID
 OPEN curCountries

 FETCH NEXT FROM curCountries INTO @offerID,@CountriesID

 WHILE @@FETCH_STATUS = 0 BEGIN
  SELECT @start = 1, @end = CHARINDEX(@delimiter, @CountriesID) 
  WHILE @start < LEN(@CountriesID) + 1 BEGIN 
   IF @end = 0  
    SET @end = LEN(@CountriesID) + 1
       
	   IF (SELECT COUNT(*) FROM htlExtPromotion_CountriesInOfferId WHERE countryID= CAST(SUBSTRING(@CountriesID, @start, @end - @start) AS BIGINT) AND offrID =@offerID ) < 1
    BEGIN   
   INSERT INTO htlExtPromotion_CountriesInOfferId(offrID,countryID)
   VALUES(@offerID,SUBSTRING(@CountriesID, @start, @end - @start)) 
   END
   SET @start = @end + 1 
   SET @end = CHARINDEX(@delimiter, @CountriesID, @start)
   SET @Result = 'Inserted Successfully'

   
   END
   FETCH NEXT FROM curCountries INTO @offerID, @CountriesID
   END
   CLOSE curCountries 
	DEALLOCATE curCountries
END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertHB_Hotelstatic]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
  
-- Author:  <Suraj Singh>  
  
-- Create date: <Jul 20, 2017>  
  
-- Mofify date: <AUG 22, 2017>  
  
-- Description: <Procedure to insert HotelsPro Hotels>  
  
-- =============================================  
  
CREATE PROCEDURE [dbo].[SP_InsertHB_Hotelstatic]  
  
(  
  
@HotelCode nvarchar(max)=null,  
@CityName nvarchar(max)=null,  
@HotelName nvarchar(max)=null,  
  
@citycode nvarchar(max)=null,  
  
@HotelXML xml=null,  
  
@retVal bit OUT  
  
)  
  
AS  
  
BEGIN  
  
        SET NOCOUNT ON;  
  
  SET XACT_ABORT ON  
  
  BEGIN TRANSACTION   
  
  BEGIN TRY  
  if not exists(select * from [StaticData].[dbo].[tblHB_Hotelstatic_AddData] where HotelCode=@HotelCode)
  begin
   Insert into [StaticData].[dbo].[tblHB_Hotelstatic_AddData](HotelID, HotelCode, HotelName,CityCode,HotelXML,HBStatus,HBcreatedOn,HBcreatedBy,HBmodifyOn,HBmodifyBy)   
  
   values(@CityName,@HotelCode,@HotelName,@citycode,@HotelXML,1,getdate(),1,getdate(),1)  
  
   set @retval=1  
  
  COMMIT TRANSACTION  
  End
  END TRY  
  
 BEGIN CATCH  
  
  IF (@@TRANCOUNT>0)  
  
  BEGIN   
  
     set @retval=0  
  
     ROLLBACK TRANSACTION   
  
  END  
  
 END CATCH  
  
END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertHPro_Hotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <June 06, 2017>
-- Mofify date: <June 06, 2017>
-- Description:	<Procedure to insert HotelsPro Hotels>
-- =============================================
CREATE PROCEDURE [dbo].[SP_InsertHPro_Hotels]
(
@HotelCode nvarchar(max)=null,
@HotelName nvarchar(max)=null,
@citycode nvarchar(max)=null,
@HotelXML xml=null,
@retVal bit OUT
)
AS
BEGIN
        SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION 
		BEGIN TRY
			Insert into [StaticData].[dbo].[tblHPro_Hotels](HotelID, HotelCode, HotelName,CityCode,HotelXML,HProStatus,HProcreatedOn,HProcreatedBy,HPromodifyOn,HPromodifyBy) 
			values(null,@HotelCode,@HotelName,@citycode,@HotelXML,1,getdate(),1,getdate(),1)
			set @retval=1
		COMMIT TRANSACTION
		END TRY
	BEGIN CATCH
	 IF (@@TRANCOUNT>0)
		BEGIN	
		   set @retval=0
		   ROLLBACK TRANSACTION 
		END
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertHPro_Hotelstatic]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <June 06, 2017>
-- Mofify date: <June 13, 2017>
-- Description:	<Procedure to insert HotelsPro Hotels>
-- =============================================
CREATE PROCEDURE [dbo].[SP_InsertHPro_Hotelstatic]
(
@HotelCode nvarchar(max)=null,
@HotelName nvarchar(max)=null,
@citycode nvarchar(max)=null,
@HotelJson nvarchar(max)=null,
@retVal bit OUT
)
AS
BEGIN
        SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION 
		BEGIN TRY
			Insert into [StaticData].[dbo].[tblHPro_Hotelstatic](HotelID, HotelCode, HotelName,CityCode,HotelJson,HProStatus,HProcreatedOn,HProcreatedBy,HPromodifyOn,HPromodifyBy) 
			values(null,@HotelCode,@HotelName,@citycode,@HotelJson,1,getdate(),1,getdate(),1)
			set @retval=1
		COMMIT TRANSACTION
		END TRY
	BEGIN CATCH
	 IF (@@TRANCOUNT>0)
		BEGIN	
		   set @retval=0
		   ROLLBACK TRANSACTION 
		END
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertHProFac_static]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <June 13, 2017>
-- Mofify date: <AUG 22, 2017>
-- Description:	<Procedure to insert HotelsPro Facilities>
-- =============================================
CREATE PROCEDURE [dbo].[SP_InsertHProFac_static]
(
@FCode nvarchar(max)=null,
@FName nvarchar(max)=null,
@FType nvarchar(max)=null,
@FCategory nvarchar(max)=null,
@FScope nvarchar(max)=null,
@FJson nvarchar(max)=null,
@retVal bit OUT
)
AS
BEGIN
        SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION 
		BEGIN TRY
			Insert into [StaticData].[dbo].[tblHProFacility_static](FCode, FName, FType,FCategory,FScope,FJson,FStatus,FcreatedOn,FcreatedBy,FmodifyOn,FmodifyBy) 
			values(@FCode,@FName,@FType,@FCategory,@FScope,@FJson,1,getdate(),1,getdate(),1)
			set @retval=1
		COMMIT TRANSACTION
		END TRY
	BEGIN CATCH
	 IF (@@TRANCOUNT>0)
		BEGIN	
		   set @retval=0
		   ROLLBACK TRANSACTION 
		END
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertPromotionChildRelation]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_InsertPromotionChildRelation]

 @offerChild AS dbo.OfferChildRelationList READONLY
 
AS  
BEGIN  
DECLARE @lastid INT;

 INSERT INTO htlExtPromotionChildRelation 
     SELECT p.offerId, p.marketId, p.offerDate, p.roomId, p.mealId, p.countriesId
     FROM @offerChild p;

	 SET @lastid = @@IDENTITY;

RETURN @lastid;

END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertSupplementChildRelation]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_InsertSupplementChildRelation]

 @suppofferChild AS dbo.SupplementChildRelationList READONLY
 
AS  
BEGIN  
DECLARE @lastid INT;

 INSERT INTO htlExtSupplimentChildRelation 
     SELECT p.suppId, p.marketID, p.suppDate, p.roomID, p.mealID,p.CountriesID
     FROM @suppofferChild p;

	 SET @lastid = @@IDENTITY;

RETURN @lastid;

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_Facilities]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_Juniper_Facilities]  
@HotelID nvarchar(20)= null,  
@SupID nvarchar(20),  
@retVal bit out  
as  
begin  
 begin try  
  
  if(@SupID=160)  
  begin  
   -- W2M--  
   select * from [StaticData].[dbo].[tblW2MFacility] with (nolock)  
   where HotelID = @HotelID  
   set @retVal = 1  
  end   
  if(@SupID=170)  
  begin  
   -- Egypt Express --  
   select * from [StaticData].[dbo].[tblEgyptExpFacility] with (nolock)  
   where HotelID = @HotelID  
   set @retVal = 1  
  end   

  else         
  begin  
    declare @Supplier int  
   set @Supplier=PARSE(@SupID AS int)  
   select * from [StaticData].[dbo].[tblJuniperFacility] with (nolock)  
   where HotelID = @HotelID 
   --and SuplId=@Supplier  
   set @retVal = 1  
  end  
 end try  
 begin catch  
  select @@TRANCOUNT  
 end catch  
end
GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_HotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_Juniper_HotelDetails]  
@CityID nvarchar(20) = null,  
@SupID nvarchar(20),  
@retVal bit out  
as  
begin   
 begin try  
  
  if(@SupID=16)  
  begin  
   -- W2M--  
   select * from [StaticData].[dbo].[tblW2MHotelDetails] with (nolock)  
   where CityID = @CityID  
   set @retVal = 1  
  end   
  if(@SupID=17)  
  begin  
   -- Egypt Express --  
   select * from [StaticData].[dbo].[tblEgyptExpHotelDetails] with (nolock)  
   where CityID = @CityID  
   set @retVal = 1  
  end   
    
  
  else         
  begin  
  
    declare @Supplier int  
   set @Supplier=PARSE(@SupID AS int)  
  
  
  
   
   select * from [StaticData].[dbo].[tblJuniperHotelDetails] with (nolock)  
   where CityID = @CityID and SuplId=@Supplier  
   set @retVal = 1  
  end  
  
 end try  
 begin catch  
  select @@TRANCOUNT  
 end catch  
end
GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_HotelImage]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_Juniper_HotelImage]    
--@HotelID nvarchar(max) = null,    
--@sqlCommand varchar(max) = null,    
@SupID bigint,    
@CityID bigint,    
@retVal bit out    
as    
begin    
begin try    
    
if(@SupID=160)    
begin    
-- W2M--    
-- SET @sqlCommand = 'select * from [StaticData].[dbo].[tblw2mHotelImage] with (nolock)    
--  where cityid in (select supcityid from tblSupplierCity_rel_new where CityID = '+ @CityID +' and supId = ' + @SupID +' ) HotelID in ('+  @HotelID+')'    
--EXEC (@sqlCommand)    
select * from [StaticData].[dbo].[tblw2mHotelImage] with (nolock)    
where cityid in (select supcityid from tblSupplierCity_rel_new where CityID =  @CityID and supId = @SupID )    
or zoneid in (select supcityid from tblSupplierCity_rel_new where CityID =  @CityID and supId = @SupID )    
set @retVal = 1    
end     
if(@SupID=170)    
begin    
-- Egypt Express --    
-- SET @sqlCommand = 'select * from [StaticData].[dbo].[tblEgyptExpHotelImage] with (nolock)    
--  where HotelID in ('+  @HotelID+')'    
--EXEC (@sqlCommand)    
select * from [StaticData].[dbo].tblEgyptExpHotelImage with (nolock)    
where cityid in (select supcityid from tblSupplierCity_rel_new where CityID =  @CityID and supId = @SupID )    
or zoneid in (select supcityid from tblSupplierCity_rel_new where CityID =  @CityID and supId = @SupID )    
set @retVal = 1    
end     
 
      
else           
begin    

--    select img.* from [StaticData].[dbo].tblJuniperHotelImages img with(nolock)  where img.hotelid in (
--	select hotelid FROM [StaticData].[dbo].[tblJuniperHotelList] with(nolock) where cityid in (select supcityid from tblSupplierCity_rel_new with (nolock)  where CityID =  @CityID and supId = @SupID )    
--or zoneid in (select supcityid from tblSupplierCity_rel_new with (nolock)  where CityID =  @CityID and supId = @SupID )  ) and   
--img.SuplId = @SupID 


  
select x.* from [StaticData].[dbo].[tblJuniperHotelImages] x with (nolock)    
Inner Join (select x.HotelID from [StaticData].[dbo].[tblJuniperHotelList] x with (nolock)    
Inner Join tblSupplierCity_rel_new y with (nolock) on (x.ZoneID=y.SupCityID OR x.CityID=y.SupCityID)    
and x.SuplId=y.supId and y.CityId=@CityID and y.supId=@SupID     
group By x.HotelID) y on x.HotelID=y.HotelID
-- and x.SuplId=@SupID  commnted by manisha    
       
       
--select top 10 x.* from [StaticData].[dbo].tblJuniperHotelImages x inner Join  
--tblSupplierCity_rel_new y on x.CityID = y.supcityid and x.SuplId=y.supId      
--where y.CityId= @CityID and y.supId  = @SupID     
     
       
set @retVal = 1    
    
    
    
    
    
    
    
end    
      
      
      
      
      
            
      
end try    
begin catch    
select @@TRANCOUNT    
end catch    
end    



GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[SP_Juniper_HotelList]    
@CityID nvarchar(10) = null,    
@SupID nvarchar(20),    
@minRating varchar(20)=null, 
@maxRating varchar(20)=null,
@retVal bit out    
as    
begin    
 begin try    
     
  if(@SupID=160)    
  begin    
   -- W2M--    
   --select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)    
   --where CityID = @CityID    
   --set @retVal = 1    
   select SupCityID into #temp1 from tblSupplierCity_rel_new where CityId = @CityID and supId = @SupID     
   select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)     
   where (CityID in (select SupCityID from #temp1) or ZoneID in (select supcityid from #temp1))  
   --and (rating>=@minRating and rating<=@maxRating)  
   set @retVal = 1    
  end     
  if(@SupID=170)    
  begin    
   -- Egypt Express --    
   --select * from [StaticData].[dbo].[tblEgyptExpHotelList] with (nolock)    
   --where CityID = @CityID or ZoneID = @CityID    
   --set @retVal = 1    
   select SupCityID into #temp2 from tblSupplierCity_rel_new where CityId = @CityID and supId = @SupID     
   select * from [StaticData].[dbo].[tblEgyptExpHotelList] with (nolock)     
   where (CityID in (select SupCityID from #temp2) or ZoneID in (select supcityid from #temp2) )   
    -- and (rating>=@minRating and rating<=@maxRating)
   set @retVal = 1    
  end     

    
    
    
  else         
  begin  
   
   declare @Supplier int  
   set @Supplier=PARSE(@SupID AS int)  
  
   select x.HotelID,x.HotelName,x.ZoneID,x.ZoneName  from [StaticData].[dbo].[tblJuniperHotelList] x   
Inner Join tblSupplierCity_rel_new y  on (x.ZoneID=y.SupCityID OR x.CityID=y.SupCityID)  
and x.SuplId=y.supId and  
y.CityId=@CityID and y.supId=@Supplier    
--and (rating>=@minRating and rating<=@maxRating)
group By x.HotelID,x.HotelName,x.ZoneID,x.ZoneName 
  
   set @retVal = 1    
   
    
    
    
  end  
    
    
    
    
    
    
    
    
    
  
  --select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)    
  --where CityID = @CityID    
  --set @retVal = 1    
 end try    
 begin catch    
  select @@TRANCOUNT    
 end catch    
end


  
GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_HotelList_Test]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--declare @Val bit
--exec [SP_Juniper_HotelList_Test] '1312','16','','','4','4',@Val

CREATE proc [dbo].[SP_Juniper_HotelList_Test]    
@CityID nvarchar(10) = null,    
@SupID nvarchar(20),   
@HotelCode nvarchar(150)=null,  
@HotelName nvarchar(200)=null, 
@minRating varchar(20)=null, 
@maxRating varchar(20)=null,
@retVal bit out    
as    
begin    
 begin try    
     
   if LEN(ISNULL(@HotelCode,''))=0          
    set @HotelCode=null  
   if LEN(ISNULL(@HotelName,''))=0          
    set @HotelName=null  
  if(@SupID=160)    
        begin    
   -- W2M--    
   --select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)    
   --where CityID = @CityID    
   --set @retVal = 1    
   if(@HotelCode is null)  
   begin  
   select SupCityID into #temp1 from tblSupplierCity_rel_new  with (nolock) where CityId = @CityID and supId = @SupID     
   select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)     
   where (CityID in (select * from #temp1) or ZoneID in (select * from #temp1) )   
   --and (rating>=@minRating and rating<=@maxRating)
     
   end  
   else--Search by GiataId  
   begin  
  
    select hotelid into #temp2 from [StaticData]..[tblgiatadetails]  with (nolock) where giataid=@HotelCode and localsupid=@SupID  
  declare @count int =(select count(*) from #temp2)  
 if(@count >0)  
 begin   
    select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)     
    where HotelID in (select hotelid from #temp2)  
    end  
 else if(@HotelName is  not null)--Search by Name  
 begin  
 select SupCityID into #temp3 from tblSupplierCity_rel_new  with (nolock) where CityId = @CityID and supId = @SupID     
  
    select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)     
    where  (CityID in (select SupCityID from #temp3) or ZoneID in (select supcityid from #temp3)  )  
 and HotelName like '%'+@HotelName+'%'  
 end  
  
  
   end  
   set @retVal = 1    
  
  end     
  
  
  
 else if(@SupID=170)    
        begin    
  
   -- Egypt Express --    
   --select * from [StaticData].[dbo].[tblEgyptExpHotelList] with (nolock)    
   --where CityID = @CityID or ZoneID = @CityID    
   --set @retVal = 1    
   if(@HotelCode is null)  
   begin  
   select SupCityID into #17temp1 from tblSupplierCity_rel_new  with (nolock) where CityId = @CityID and supId = @SupID     
   select * from [StaticData].[dbo].[tblEgyptExpHotelList] with (nolock)     
   where (CityID in (select SupCityID from #17temp1) or ZoneID in (select supcityid from #17temp1)   )
   --and (rating>=@minRating and rating<=@maxRating) 
    
   end  
   else--Search by GiataId  
   begin  
    select hotelid into #17temp2 from [StaticData]..[tblgiatadetails]  with (nolock) where giataid=@HotelCode and localsupid=@SupID  
  declare @17count int =(select count(*) from #17temp2)  
 if(@17count >0)  
 begin   
    select * from [StaticData].[dbo].[tblEgyptExpHotelList]  with (nolock)   
    where HotelID in (select hotelid from #17temp2)  
    end  
 else if(@HotelName is  not null)--Search by Name  
 begin  
 select SupCityID into #17temp3 from tblSupplierCity_rel_new  with (nolock) where CityId = @CityID and supId = @SupID     
  
    select * from [StaticData].[dbo].[tblEgyptExpHotelList] with (nolock)     
    where  (CityID in (select SupCityID from #17temp3) or ZoneID in (select supcityid from #17temp3)  )  
 and HotelName like '%'+@HotelName+'%'  
 end  
  set @retVal = 1    
  
   end  
  end     
  
    
  else         
  begin  
   
   declare @Supplier int  
   set @Supplier=PARSE(@SupID AS int)  
    if(@HotelCode is null)  
 begin  
	if(@Supplier= 24 )
	begin
	set @Supplier=16
	end
	

   select top 100 x.HotelID,x.HotelName,x.ZoneID,x.ZoneName from [StaticData].[dbo].[tblJuniperHotelList] x  with (nolock)  
    Inner Join tblSupplierCity_rel_new y  with (nolock)  on (x.ZoneID=y.SupCityID OR x.CityID=y.SupCityID)  
    and x.SuplId=y.supId and  
    y.CityId=@CityID and y.supId=@Supplier 
	--and (rating>=@minRating and rating<=@maxRating)
    group By x.HotelID,x.HotelName,x.ZoneID,x.ZoneName  
   end  
    else--Search by GiataId  
   begin  
    select hotelid into #47temp2 from [StaticData]..[tblgiatadetails]  with (nolock) where giataid=@HotelCode and localsupid=@SupID  
    declare @47count int =(select count(*) from #47temp2)  
 if(@47count >0)  
 begin   
    select top 100 HotelID,HotelName,ZoneID,ZoneName from [StaticData].[dbo].[tblJuniperHotelList]  with (nolock)       
    where HotelID in (select hotelid from #47temp2)  
    end  
 else if(@HotelName is  not null)--Search by Name  
 begin  
   select x.HotelID,x.HotelName,x.ZoneID,x.ZoneName from [StaticData].[dbo].[tblJuniperHotelList] x  with (nolock)  
    Inner Join tblSupplierCity_rel_new y  with (nolock) on (x.ZoneID=y.SupCityID OR x.CityID=y.SupCityID)  
    and x.SuplId=y.supId and  
    y.CityId=@CityID and y.supId=@Supplier and  HotelName like '%'+@HotelName+'%'  
    group By x.HotelID,x.HotelName,x.ZoneID,x.ZoneName  
 end  
 end  
  
  
  
   set @retVal = 1    
   
  --return 55  
    
    
  end  
    
    
    
    
    
    
    
    
    
  
  --select * from [StaticData].[dbo].[tblW2MHotelList] with (nolock)    
  --where CityID = @CityID    
  --set @retVal = 1    
 end try    
 begin catch    
  select @@TRANCOUNT    
 end catch    
end

GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_Images]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_Juniper_Images]  
@CityID nvarchar(20)= null,  
@SupID nvarchar(20),  
@retVal bit out  
as  
begin  
 begin try  
  
  if(@SupID=16)  
  begin  
   -- W2M--  
   select * from [StaticData].[dbo].[tblw2mHotelImage] with (nolock)  
   where CityID = @CityID  
   set @retVal = 1  
  end   
  if(@SupID=17)  
  begin  
   -- Egypt Express --  
   select * from [StaticData].[dbo].[tblEgyptExpImages] with (nolock)  
   where CityID = @CityID  
   set @retVal = 1  
  end   

    
  else         
  begin  
   
   declare @Supplier int  
   set @Supplier=PARSE(@SupID AS int)  
   select * from [StaticData].[dbo].[tblJuniperHotelImages] with (nolock)  
    where CityID = @CityID and SuplId=@Supplier  
   set @retVal = 1  
  end  
         
    
 end try  
 begin catch  
  select @@TRANCOUNT  
 end catch  
end
GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_SingleHotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_Juniper_SingleHotelDetails]  
@HotelID nvarchar(100) = null,  
@SupID nvarchar(20),  
@retVal bit out  
as  
begin   
 begin try  
  
  if(@SupID=160)  
  begin  
   -- W2M--  
   select * from [StaticData].[dbo].[tblW2MHotelDetails] with (nolock)  
   where HotelID = @HotelID  
   set @retVal = 1  
  end   
  if(@SupID=170)  
  begin  
   -- Egypt Express --  
   select * from [StaticData].[dbo].[tblEgyptExpHotelDetails] with (nolock)  
   where HotelID = @HotelID  
   set @retVal = 1  
  end
  else         
  begin  
   
   declare @Supplier int  
   set @Supplier=PARSE(@SupID AS int)  
   select * from [StaticData].[dbo].[tblJuniperHotelDetails] with (nolock)  
   where HotelID = @HotelID 
   --and SuplId=@Supplier  (done by manisha)
   set @retVal = 1  
  end  
  
 end try  
 begin catch  
  select @@TRANCOUNT  
 end catch  
end
GO
/****** Object:  StoredProcedure [dbo].[SP_Juniper_SingleHotelImages]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  proc [dbo].[SP_Juniper_SingleHotelImages]  
@HotelID nvarchar(20)= null,  
@SupID nvarchar(20),  
@retVal bit out  
as  
begin  
 begin try  
  
  if(@SupID=160)  
  begin  
   -- W2M--  
   select * from [StaticData].[dbo].[tblW2MImages] with (nolock)  
   where HotelID = @HotelID  
   set @retVal = 1  
  end   
  if(@SupID=170)  
  begin  
   -- Egypt Express --  
   select * from [StaticData].[dbo].[tblEgyptExpImages] with (nolock)  
   where HotelID = @HotelID  
   set @retVal = 1  
  end   
 
  else         
  begin  
    
   declare @Supplier int  
   set @Supplier=PARSE(@SupID AS int)  
   
   select * from [StaticData].[dbo].[tblJuniperImages] with (nolock)  
   where HotelID = @HotelID 
   --and SuplId=@Supplier  done by manisha
   set @retVal = 1  
  end         
    
 end try  
 begin catch  
  select @@TRANCOUNT  
 end catch  
end
GO
/****** Object:  StoredProcedure [dbo].[SP_ManageSession]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <January 24, 2018>
-- Mofify date: <January 24, 2018>
-- Description:	<Procedure to manage session for air>
-- truncate table tblsessionmgmt
-- EXEC SP_ManageSession '110022',null,1,null,NULL
-- =============================================
CREATE PROCEDURE [dbo].[SP_ManageSession]
(
@customerID bigint=null,
@sessionValue nvarchar(max)=null,
@type tinyint=null,
@session nvarchar(max) OUT,
@retVal tinyint OUT
)
AS
BEGIN
        SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION
		BEGIN TRY
		IF (SELECT COUNT(*) FROM tblsessionmgmt where customerID=@customerID) = 0
		BEGIN
			if(@type=2)
			BEGIN
			Insert into tblsessionmgmt(customerID,sessionValue,sessionUserCount,sessionStatus,sessioncreatedOn,sessionexpiryOn)
			values(@customerID,@sessionValue,1,1,getdate(),dateadd(minute, 19, getdate()))
			set @session = @sessionValue
			set @retval=1
			SELECT @retVal AS retVal,@session as sessionid
			END
			ELSE
			BEGIN
				set @retval=2
				SELECT @retVal AS retVal,@session as sessionid
			END
		END
		ELSE
		BEGIN
			--Insert into tblsessionmgmt(customerID,sessionValue,sessionUserCount,sessionStatus,sessioncreatedOn,sessionexpiryOn)
			--values(@customerID,@sessionValue,1,1,getdate(),dateadd(minute, 19, getdate()))
			set @retval=1
			SELECT @retVal AS retVal,@session as sessionid
		END
		COMMIT TRANSACTION
		END TRY
	BEGIN CATCH
	 IF (@@TRANCOUNT>0)
		BEGIN		
		   set @retval=0
		   SELECT @retVal AS retVal,@session as sessionid
		   ROLLBACK TRANSACTION
		END
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_movedataoncloud]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[sp_movedataoncloud]
as
Begin
insert into [35.158.121.213].travayooserviceHA.dbo.tblSupplierCity_rel_new(CityId
,SupCityId
,supId
,cityLogitude
,cityLatitude
,Createdby
,Createdatetime
,cntid
,stateID)
select CityId
,SupCityId
,supId
,cityLogitude
,cityLatitude
,Createdby
,Createdatetime
,cntid
,stateID from tblSupplierCity_rel_new where convert(varchar(10),Createdatetime,121)='2018-07-06'
end
GO
/****** Object:  StoredProcedure [dbo].[sp_pushXMLOutBooking]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[sp_pushXMLOutBooking]  
  
  
  
AS  
  
  
  
BEGIN  
  
  
  
  
  
  
  
declare @count int  
select outResp,a.customerID,b.agencyID,VoucherComments,specialRequest,supplierConfNo,SupplierRefNo,tracknumber,DbName,0 as status,bookingStatus,Errortext ,a.ID as UniqueNO,outstatus  
  
into #bokngdetails from tblXMLOutBookingDetails as a JOIN  XMLOutDetails as b on a.customerid=b.customerid and a.agencyID=b.agencyID and a.outcustid=b.Outcustid where outstatus=1  
  
  
  
alter table #bokngdetails add ID int identity(1,1)  
  
  
  
BEGIN TRY  
  
  
  
    BEGIN TRANSACTION  
  
  
  
 while (select count(*) from #bokngdetails where status=0 )>0  
  
  
  
 BEGIN  
  
  
  
 declare @ID int  
 declare @DbName varchar(100)  
 declare @bkngResp XML  
 declare @supplierConfNo varchar(100)  
 declare @VoucherComments varchar(1000)  
 declare @specialRequest varchar(1000)  
 declare @SupplierRefNo varchar(200)   
 declare @tracknumber varchar(200)  
 declare @custid varchar(200)   
 declare @agencyid varchar(200)   
 declare @bookingStatus varchar(200)  
 declare @Error varchar(200)  
 declare @unique int  
    declare @status int  
 set @id=(select top 1 ID from #bokngdetails where status=0)  
  
 SELECT @DbName=dbname,@bkngResp=outResp,@supplierConfNo=supplierConfNo,  
 @VoucherComments=VoucherComments,@specialRequest=specialRequest,@SupplierRefNo=SupplierRefNo  
 ,@tracknumber=tracknumber,@bookingStatus=bookingStatus,@Error= Errortext,@unique=UniqueNO , @custid=customerID,@agencyid=agencyID FROM #bokngdetails WHERE ID=@ID  
   
 if (select COUNT(*) from tblXMLOutBookingDetails where customerID=@custid and agencyID=@agencyid  and tracknumber=@tracknumber and outstatus=1 )>0   
 begin  
 if (@DbName='SAAS')  
  
  BEGIN  
     exec Travayoo.dbo.SP_ImportXMLOutDataSAAS @bkngResp,0  
     exec Travayoo.dbo.uspUpdateWhiteLabelBookingSAAS @tracknumber,@bookingStatus,@supplierConfNo,@SupplierRefNo,'','',@Error,@VoucherComments,@specialRequest,0  
       
  END  
    
 ELSE  
  BEGIN  
     exec Travayoo.dbo.SP_ImportXMLOutData @bkngResp,0  
     exec Travayoo.dbo.uspUpdateWhiteLabelBooking @tracknumber,@bookingStatus,@supplierConfNo,@SupplierRefNo,'','',@Error,@VoucherComments,@specialRequest,0  
       
  END  
   
  
 END  
  update #bokngdetails set status=1 where ID=@id  
  update tblXMLOutBookingDetails set outstatus=2 ,Modifyby=createdby,ModifyDatetime=getdate() where ID=@unique  
  
     end  
  
    COMMIT  
  
  
  
END TRY  
  
  
  
BEGIN CATCH       
  
  
  
select  ERROR_MESSAGE()  
  
  
  
 ROLLBACK  
  
  
  
END CATCH  
  
  
  
  
  
  
  
END  
  
  
  
  
GO
/****** Object:  StoredProcedure [dbo].[sp_SaveB2bXML]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE Proc [dbo].[sp_SaveB2bXML]
@custID bigint,

@agencyID bigint,

@Resp XML,

@PID varchar(500),

@tracknumber varchar(500)

AS

Begin

begin try


DECLARE @CityID varchar(100)
DECLARE @CountryID varchar(100)
DECLARE @CountryCode varchar(100)
Create table #countrydetails(ctyId varchar(100),ctyLclName varchar(200),CityCode varchar(20),CountryCode varchar(10),CountryName varchar(100),CountyID varchar(50))

SET @CityID=(select 
   t.x.value('(City/@Id)[1]','varchar(100)') as CityID
from @resp.nodes('BookingResponse/HotelBookings/HotelBooking/HotelDetails') t(x))

INSERT INTO #countrydetails
EXEC sp_GetCityCountyDetails @CityID

if (select count(*) from #countrydetails)>0
begin
set @CountryID =(select top 1 CountyID from #countrydetails)
set @CountryCode =(select top 1 CountryCode from #countrydetails)


SET @resp.modify('  
  replace value of (BookingResponse/HotelBookings/HotelBooking/HotelDetails/Country/@Id)[1]  
  with     sql:variable("@CountryID")
');  
SET @resp.modify('  
  replace value of (BookingResponse/HotelBookings/HotelBooking/HotelDetails/Country/@Code)[1]  
  with     sql:variable("@CountryCode")
');  
end


INSERT into tblXMLOutBookingDetails(outResp,customerID,agencyID,bookingStatus,outstatus,CreatedBy,Createdatetime,PID,tracknumber)
select @Resp,@custID,@agencyID,1,0,@agencyID,getdate(),@PID,@tracknumber

select 1

end try

begin catch

 select 0

end catch

END
GO
/****** Object:  StoredProcedure [dbo].[sp_SaveSesionData_AM]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[sp_SaveSesionData_AM]

@custID bigint,
@sessionID nvarchar(200),
@sessionNo int,
@sessionToken nvarchar(200),
@tracknumber nvarchar(200),
@Flag int
AS
BEGIN
 IF (@Flag=0)
 BEGIN
	 INSERT INTO tblsessionmgmt_AM(AM_customerID,AM_SessionId,AM_SequenceNumber,AM_SecurityToken,AM_TrackNumber,AM_sessionStatus)
	 SELECT @custID,@sessionID,@sessionNo,@sessionToken,@tracknumber,0

	 select 1
	 END
 ELSE IF(@Flag=1)
 BEGIN
	UPDATE tblsessionmgmt_AM set AM_sessionStatus=1 where AM_customerID=@custID and AM_SessionId=@sessionID and AM_TrackNumber=AM_TrackNumber and AM_sessionStatus=0
	select 1
 END
 ELSE IF(@Flag=2)
 BEGIN
	UPDATE tblsessionmgmt_AM set AM_SequenceNumber=@sessionNo where AM_customerID=@custID and AM_SessionId=@sessionID and AM_TrackNumber=AM_TrackNumber and AM_sessionStatus=0
 END
 ELSE
 BEGIN
  SELECT AM_SessionId as sessionID,AM_SequenceNumber as sequenceNo,AM_SecurityToken as securityToken ,case when datediff(MINUTE,getdate(),AM_sessionexpiryOn)<=0 then 0 else 1 end as interval
    FROM tblsessionmgmt_AM WHERE AM_TrackNumber=@tracknumber and AM_customerID=@custID  and AM_sessionStatus=0  order by AM_sessioncreatedOn desc
 END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SaveXMLOutBookings]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[sp_SaveXMLOutBookings]

@outResp XML,
@customerID bigint,
@agencyID bigint,
@bookingStatus int,
@CreatedBy bigint
AS
Begin
insert into tblXMLOutBookingDetails(outResp,customerID,agencyID,bookingStatus,outstatus,CreatedBy,Createdatetime)
select  @outResp,@customerID,@agencyID,@bookingStatus,0,@CreatedBy,getdate()
End
GO
/****** Object:  StoredProcedure [dbo].[SP_SmyHotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE proc [dbo].[SP_SmyHotelDetails]      
@HotelID nvarchar(max) = null,      
@sqlCommand varchar(max) = null,     
@supID int,    
@retVal bit out      
as        
begin      
 begin try      
  --   select @HotelID      
  --select * from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)      
  --where HotelID in( @HotelID) and Caption ='HOTEL'      
  --set @retVal = 1      
    if(@supID=29)    
 begin    
  SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[cosmoHotelDetails] with (nolock)      
    where HotelID in ('+  @HotelID+')'      
  EXEC (@sqlCommand)      
  set @retVal = 1      
  end    
  else if(@supID=30)    
 begin    
  SET @sqlCommand = 'select cityID+''#''+ HotelID as HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[stubaHotelDetails] with (nolock)      
    where CONCAT_WS(''#'',CityID,HotelID) in ('+  @HotelID+')'      
  EXEC (@sqlCommand)      
  set @retVal = 1      
  end   
  else if(@supID=32)    
 begin    
  SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[yalagoHotelDetails] with (nolock)      
    where HotelID in ('+  @HotelID+')'      
  EXEC (@sqlCommand)      
  set @retVal = 1      
  end   
  else if(@supID=39)    
  begin    
  SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[SmyHotelDetails] with (nolock)      
    where HotelID in ('+  @HotelID+')'      
  EXEC (@sqlCommand)      
  set @retVal = 1     
  end  
   else if(@supID=48)    
 begin    
  SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[wteHotelDetails] with (nolock)      
    where HotelID in ('+  @HotelID+')'      
  EXEC (@sqlCommand)      
  set @retVal = 1      
  end   
 end try      
 begin catch      
  select @@TRANCOUNT      
 end catch      
end 
GO
/****** Object:  StoredProcedure [dbo].[SP_SmyHotelDetails_new]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
      
CREATE proc [dbo].[SP_SmyHotelDetails_new]          
@CityID nvarchar(max) = null,          
@supID int,        
@retVal bit out          
as            
begin          
 begin try          
           
    if(@supID=29)        
 begin        
  --SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[cosmoHotelDetails] with (nolock)          
  --  where HotelID in ('+  @HotelID+')'          
  --EXEC (@sqlCommand)          
  select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[cosmoHotelDetails] with (nolock) where cityid = @CityID order by 1    
  set @retVal = 1          
  end        
  else if(@supID=30)        
 begin        
  --SET @sqlCommand = 'select cityID+''#''+ HotelID as HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[stubaHotelDetails] with (nolock)          
  --  where CONCAT_WS(''#'',CityID,HotelID) in ('+  @HotelID+')'          
  --EXEC (@sqlCommand)       
  select cityID+'#'+ HotelID as HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[stubaHotelDetails] with(nolock) where cityid = @CityID order by 1       
  set @retVal = 1          
  end       
  else if(@supID=32)        
 begin        
  --SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[yalagoHotelDetails] with (nolock)          
  --  where HotelID in ('+  @HotelID+')'          
  --EXEC (@sqlCommand)      
  select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[yalagoHotelDetails] with (nolock) where cityid = @CityID order by 1          
  set @retVal = 1          
  end       
  else if(@supID=39)        
  begin        
  --SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[SmyHotelDetails] with (nolock)          
  --  where HotelID in ('+  @HotelID+')'          
  --EXEC (@sqlCommand)        
  select HotelID, HotelName, Images, address,latitude,longitude,star from StaticData.dbo.SmyHotelDetails with (nolock) where cityid = @CityID order by 1     
  set @retVal = 1         
  end    
    
   else if(@supID=48)        
 begin        
  --SET @sqlCommand = 'select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[yalagoHotelDetails] with (nolock)          
  --  where HotelID in ('+  @HotelID+')'          
  --EXEC (@sqlCommand)      
  select HotelID, HotelName, Images, address,latitude,longitude,star from [StaticData].[dbo].[wteHotelDetails] with (nolock) where cityid = @CityID order by 1          
  set @retVal = 1          
  end    
  
 end try          
 begin catch          
  select @@TRANCOUNT          
 end catch          
end     
    
GO
/****** Object:  StoredProcedure [dbo].[Sp_SmyHotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Sp_SmyHotelList]      
@CityID nvarchar(50)= null,     
@supID int,    
@retVal bit out      
as      
begin      
 begin try      
  if(@supID=39)    
  begin    
  select * from StaticData.dbo.smyhotellist with (nolock) where cityid = @CityID order by 1      
  set @retVal = 1      
  end    
  if(@supID=29)    
  begin    
  select * from StaticData.dbo.cosmohotellist with (nolock) where cityid = @CityID order by 1      
  set @retVal = 1     
  end    
   if(@supID=30)    
  begin    
  select * from StaticData.dbo.stubahotellist with (nolock) where cityid = @CityID order by 1      
  set @retVal = 1     
  end  
  if(@supID=32)    
  begin    
  select * from StaticData.dbo.yalagohotellist with (nolock) where cityid = @CityID order by 1      
  set @retVal = 1     
  end  
  if(@supID=48)    
  begin    
  select * from StaticData.dbo.twehotellist with (nolock) where cityid = @CityID order by 1      
  set @retVal = 1     
  end  
 end try      
 begin catch      
  select @@TRANCOUNT      
 end catch      
end 

GO
/****** Object:  StoredProcedure [dbo].[SP_SmySingleHotelDetails]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE proc [dbo].[SP_SmySingleHotelDetails]    
@HotelID nvarchar(max) = null,    
@sqlCommand varchar(max) = null,  
@supID int,  
@retVal bit out    
as      
begin    
 begin try    
  --   select @HotelID    
  --select * from [StaticData].[dbo].[tblSalToursHotelImages] with (nolock)    
  --where HotelID in( @HotelID) and Caption ='HOTEL'    
  --set @retVal = 1    
    if(@supID=29)  
 begin  
  SET @sqlCommand = 'select HotelID, HotelName, Images, facilities,description,star,telephone,fax from [StaticData].[dbo].[cosmoHotelDetails] with (nolock)    
    where HotelID in ('+  @HotelID+')'    
  EXEC (@sqlCommand)    
  set @retVal = 1    
  end  
  else if(@supID=30)  
 begin  
  SET @sqlCommand = 'select HotelID, HotelName, Images, facilities,description,star,telephone,fax from [StaticData].[dbo].[stubaHotelDetails] with (nolock)    
    where HotelID in ('+  @HotelID+')'    
  EXEC (@sqlCommand)    
  set @retVal = 1    
  end  
  else if(@supID=32)  
 begin  
  SET @sqlCommand = 'select HotelID, HotelName, Images, facilities,description,star,telephone,fax from [StaticData].[dbo].[yalagoHotelDetails] with (nolock)    
    where HotelID in ('+  @HotelID+')'    
  EXEC (@sqlCommand)    
  set @retVal = 1    
  end  
  else if(@supID=39)  
  begin   
  SET @sqlCommand = 'select HotelID, HotelName, Images, facilities,description,star,telephone,fax from [StaticData].[dbo].[SmyHotelDetails] with (nolock)    
    where HotelID in ('+  @HotelID+')'    
  EXEC (@sqlCommand)    
  set @retVal = 1    
  end
  else if(@supID=48)  
  begin   
  SET @sqlCommand = 'select HotelID, HotelName, Images, facilities,description,star,telephone,fax from [StaticData].[dbo].[wteHotelDetails] with (nolock)    
    where HotelID in ('+  @HotelID+')'    
  EXEC (@sqlCommand)    
  set @retVal = 1    
  end
  
 end try    
 begin catch    
  select @@TRANCOUNT    
 end catch    
end 
GO
/****** Object:  StoredProcedure [dbo].[SP_SunHotel_Facilities]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[SP_SunHotel_Facilities]
@HotelID nvarchar(20) = null,
@retVal bit OUT
 as
 begin
	begin try
			select  *  from [StaticData].[dbo].[tblSunHotelFacilities] with (nolock)
			where HotelID = @HotelID
			set @retVal =1
	End try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[SP_SunHotel_Images]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[SP_SunHotel_Images]
@HotelID nvarchar(20) = null,
@retVal bit OUT
 as
 begin
	begin try
			select  *  from [StaticData].[dbo].[tbplSunHotelImages] with (nolock)
			where HotelID = @HotelID
			set @retVal =1
	End try
	begin catch
	select @@TRANCOUNT
	end catch
end
GO
/****** Object:  StoredProcedure [dbo].[sp_updateB2BResponse]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[sp_updateB2BResponse]

@supplierConfNo varchar(500),

@SupplierRefNo  varchar(500),

@specialRequest  varchar(1000),

@voucherRemarks  varchar(1000),

@PID  varchar(500),

@tracknumber  varchar(500),
@Errortext varchar(500),
@outCustID bigint



AS

BEGIN

update tblXMLOutBookingDetails set bookingstatus=case when @supplierConfNo='' then 5 else 3 end,supplierConfNo=@supplierConfNo,SupplierRefNo=@SupplierRefNo,specialRequest=@specialRequest,VoucherComments=@voucherRemarks,Errortext=@Errortext,outstatus=1,outCustID=@outCustID
WHERE PID=@PID AND tracknumber=@tracknumber and outstatus=0





END
GO
/****** Object:  StoredProcedure [dbo].[sp_updateB2BResponseCXL]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[sp_updateB2BResponseCXL]
@PID varchar(500),
@tracknumber  varchar(500),
@status  varchar(1000),
@outCustID bigint

AS

BEGIN

update tblXMLOutBookingDetails set bookingstatus=@status,outstatus=3
WHERE PID=@PID AND tracknumber=@tracknumber and outcustid= @outCustID and outstatus=2

END
GO
/****** Object:  StoredProcedure [dbo].[SP_UpdateHB_Hotelstatic]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <FEB 21, 2018>
-- Mofify date: <FEB 21, 2018>
-- Description:	<Procedure to update Hotels>
-- =============================================
CREATE PROCEDURE [dbo].[SP_UpdateHB_Hotelstatic]
(
@HotelCode nvarchar(max)=null,
@HotelXML xml=null,
@retVal bit OUT
)
AS
BEGIN
        SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION 
		BEGIN TRY
			update [StaticData].[dbo].[tblHB_Hotelstatic] set HotelXML=@HotelXML,HBmodifyOn=getdate() where HotelCode=@HotelCode 
			set @retval=1
		COMMIT TRANSACTION
		END TRY
	BEGIN CATCH
	 IF (@@TRANCOUNT>0)
		BEGIN	
		   set @retval=0
		   ROLLBACK TRANSACTION 
		END
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SP_UpdateHPro_Hotelstatic]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <July 12, 2018>
-- Mofify date: <July 12, 2018>
-- Description:	<Procedure to update HotelsPro Hotels>
-- =============================================
CREATE PROCEDURE [dbo].[SP_UpdateHPro_Hotelstatic]
(
@HotelCode nvarchar(max)=null,
@HotelName nvarchar(max)=null,
@citycode nvarchar(max)=null,
@HotelJson nvarchar(max)=null,
@retVal bit OUT
)
AS
BEGIN
        SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRANSACTION 
		BEGIN TRY
			update [StaticData].[dbo].[tblHPro_Hotelstatic] set HotelName=@HotelName,HotelJson=@HotelJson,HPromodifyOn=getdate() where HotelCode=@HotelCode and CityCode=@citycode
			set @retval=1
		COMMIT TRANSACTION
		END TRY
	BEGIN CATCH
	 IF (@@TRANCOUNT>0)
		BEGIN	
		   set @retval=0
		   ROLLBACK TRANSACTION 
		END
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_xmlOutCancellation]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  
CREATE Proc [dbo].[sp_xmlOutCancellation]  
  
  
  
AS  
  
  
  
BEGIN  
  
  
  
  
  
  
  
declare @count int  
  
  
  
  
  
  
  
select outResp,a.customerID,b.agencyID,VoucherComments,specialRequest,supplierConfNo,SupplierRefNo,tracknumber,DbName,0 as status,bookingStatus,Errortext ,a.ID as UniqueNO  
  
  
  
into #bokngdetails from tblXMLOutBookingDetails as a JOIN  XMLOutDetails as b on a.customerid=b.customerid and a.agencyID=b.agencyID and a.outcustid=b.Outcustid where outstatus=3 and bookingStatus=4  
  
  
  
  
  
  
  
alter table #bokngdetails add ID int identity(1,1)  
  
  
  
  
  
  
  
BEGIN TRY  
  
  
  
    BEGIN TRANSACTION  
  
  
  
 while (select count(*) from #bokngdetails where status=0 )>0  
  
  
  
 BEGIN  
  
  
  
 declare @ID int  
  
  
  
 declare @DbName varchar(100)   
  
  
  
 declare @tracknumber varchar(200)  
  
  
  
 declare @bookingStatus varchar(200)  
  
  
  
    declare @agencyID varchar(200)  
  
  
  
 declare @unique int  
  
  
  
  
  
  
  
 set @id=(select top 1 ID from #bokngdetails where status=0)  
  
  
  
 SELECT @DbName=dbname  
  
  
  
 ,@tracknumber=tracknumber,@unique=UniqueNO,@agencyID=agencyID FROM #bokngdetails WHERE ID=@ID  
  
  
  
  
  
  
  
 if (@DbName='SAAS')  
  
  
  
  BEGIN  
  
  
  
    
  
  
  
     exec Travayoo.dbo.uspXMLOutBookingCancellationSAAS @tracknumber,1,0,@agencyID      
  
  
  
    
  
  
  
  END  
  
  
  
 ELSE  
  
  
  
  BEGIN  
  
  
  
      exec Travayoo.dbo.uspXMLOutBookingCancellation @tracknumber,1,0,@agencyID  
  
  
  
       
  
  
  
  END  
  
  
  
  update #bokngdetails set status=1 where ID=@id  
  
  
  
  update tblXMLOutBookingDetails set outstatus=4 ,Modifyby=createdby,ModifyDatetime=getdate() where ID=@unique  
  
  
  
 END  
  
  
  
  
  
  
  
  
  
  
  
    COMMIT  
  
  
  
END TRY  
  
  
  
BEGIN CATCH       
  
  
  
select  ERROR_MESSAGE()  
  
  
  
 ROLLBACK  
  
  
  
END CATCH  
  
  
  
  
  
  
  
END
GO
/****** Object:  StoredProcedure [dbo].[usp_checktktorder]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 -- =============================================

-- Author:		<Suraj Singh>

-- Create date: <Nov 27, 2018>

-- Mofify date: <APR 17, 2019>

-- Description:	<Procedure to check ticket ordering>

--exec usp_checktktorder '4f3535a7-9b92-4430-8c8d-6e93fde76c0e'

-- =============================================

 CREATE proc [dbo].[usp_checktktorder] 

 (

	@transid [nvarchar](max)

 )

 AS

 BEGIN

	select (1) from TravayooService..tblapilogflt with(nolock) where TrackNumber=@transid and logTypeID=11 and logType='AirTicketOrder' and supplierID=12 order by 1 desc

 END








GO
/****** Object:  StoredProcedure [dbo].[usp_GetBookingResponse]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================            
-- Author:  <Suraj Singh>            
-- Create date: <11 Feb, 2024>    
-- Modify date: <11 Feb, 2024>    
-- Description: <Get Booking Response to cancel the booking  >            
-- =============================================            
CREATE PROCEDURE [dbo].[usp_GetBookingResponse]             
@trackNumber nvarchar(500)=null
AS            
BEGIN            
            
  BEGIN TRY   
			 begin    
				   select logresponseXML from tblapilog with(nolock)    
			   where TrackNumber=@trackNumber and logtypeID=5 and logType='Book' and SupplierID=50    
			   order by logID desc    
			 end      
       END TRY            
    BEGIN CATCH            
     select null            
    END CATCH         
END 
GO
/****** Object:  StoredProcedure [dbo].[usp_getbookreq_tbo]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <23 JUL, 2019>
-- Modify date: <27 AUG, 2019>
-- Description:	<Get book request for ticketing >
-- =============================================
CREATE PROCEDURE [dbo].[usp_getbookreq_tbo] 
@TrackNumber nvarchar(500)=null
AS
BEGIN
		BEGIN TRY
            select top 2 logrequestXML,logID from TravayooService..tblapilogflt with(nolock)
			where TrackNumber=@TrackNumber and logTypeID=5 and SupplierID in (51,0)
			order by 2 asc
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_getbookreqcommonout]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[usp_getbookreqcommonout]  
@transid nvarchar(500) = null, 
@custID  varchar(100) = null, 
@retVal bit out    
as    
begin
 begin try 
 select top 1 logrequestXML,logID from TravayooService..tblapilogOut with(nolock) where TrackNumber=@transid and customerID=@custID and logType='Book' and logTypeID=5 order by 2
 end try
 begin catch 
  select @@TRANCOUNT 
 end catch   
end
GO
/****** Object:  StoredProcedure [dbo].[usp_getbookreqout]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[usp_getbookreqout]  

@transid nvarchar(500) = null,  

@custID  varchar(100) = null, 

@retVal bit out    

as    

begin

 begin try 

 select top 1 logrequestXML,logID from TravayooService..tblapilogOut with(nolock) where TrackNumber=@transid and customerID=@custID and logType='Book' and logTypeID=5 order by 2 desc

 end try

 begin catch 

  select @@TRANCOUNT 

 end catch   

end
GO
/****** Object:  StoredProcedure [dbo].[usp_GetcitycodeRTS]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <13 AUG, 2018>
-- Description:	<Get City Code using city id >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetcitycodeRTS] 
@cityid nvarchar(50)=null
AS
BEGIN
		SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRY	
		select top 1 SupCityId from tblSupplierCity_rel_new with(nolock) where supId=9 and CityId=@cityid
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetcitycodeRTS_tmp]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <13 AUG, 2018>
-- Description:	<Get City Code using city id >
-- =============================================
Create PROCEDURE [dbo].[usp_GetcitycodeRTS_tmp] 
@cityid nvarchar(50)=null
AS
BEGIN
		SET NOCOUNT ON;
		SET XACT_ABORT ON
		BEGIN TRY	
			select STUFF((SELECT ', ' + [SupCityId] from tblSupplierCity_rel_new with(nolock) where supId=9 and CityId=@cityid FOR XML PATH('')),1,1,'') as [SupCityId]
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH
END




GO
/****** Object:  StoredProcedure [dbo].[USP_getcitylist_wte]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================= 
-- Author:  <Suraj Singh> 
-- Create date: <Jul 11, 2023>
-- Mofify date: <Jul 11, 2023> 
-- Description: <Procedure to get all cities> 
-- =============================================  
CREATE PROCEDURE [dbo].[USP_getcitylist_wte]  
(    
@retVal bit OUT  
)   
AS   
BEGIN  
        SET NOCOUNT ON;
  SET XACT_ABORT ON   
  BEGIN TRY   
   Begin  
   select cityid from [StaticData].[dbo].[tbl_wtecity_master] with(nolock) where 1=1 
   set @retval=1 
   End 
  END TRY 
 BEGIN CATCH  
 END CATCH 
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetCurrencyXMLOut]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  
CREATE Proc [dbo].[USP_GetCurrencyXMLOut]  
  
  
  
@usrID bigint,@ClientType varchar(50)='HA'  
  
  
  
AS  
  
  
  
/*  
  
  procedure Created By : Manisha Khanna  
  
  Created on :15/04/2019  
  
  Purpose : To Get Currency Conversion Rates  dynamically, along with Admin/TA Currency  
  
*/  
  
  
  
BEGIN  
  
  
  
  
  
  
  
DECLARE  @mainagentid bigint  
  
  
  
SET @mainagentid=0  
  
  
  
CREATE TABLE #tempTABLE(usrId varchar(100))   
  
CREATE TABLE #Currency(MACrncy varchar(10),SAcrncy varchar(10))   
  
DECLARE @dynamicSQL varchar(MAX)  
  
  
  
DECLARE @server varchar(100)  
  
  
  
  
  
  
  
if (@ClientType='HA')  
  
  
  
BEGIN  
  
  
  
 set @server='TravayooHA'  
  
  
  
END  
  
  
  
ELSE  
  
  
  
Begin  
  
  
  
 set @server='Travayoo'  
  
  
  
END  
  
  
  
  
  
  
  
  
  
  
  
Set @dynamicSQL='INSERT INTO #tempTABLE  
  
  
  
EXEC '+@server+'.dbo.GetMainAgentID '+cast(@usrID as varchar(50))+''  
  
  
  
  
  
exec(@dynamicSQL)  
  
  
  
  if (select count(*) FROM #tempTABLE)>0  
  
  
  
   BEGIN  
  
   SET  @mainagentid = (select usrId FROM #tempTABLE )     
  
  
  
   END  
  
  
  
   SET @dynamicSQL='SELECT currencymst.crncyCode,isnull(currencymrkup.crmkpBuyingRate,1) as BuyingRate,isnull(currencymrkup.crmkpAppliedSellingRate,1) as AppliedSellingRate,currencymrkup.crncyId   
  
  
  
   from  '+@server+'.dbo.tblCurrencyMarkup as currencymrkup with(nolock)  
  
  
  
          join  '+@server+'.dbo.tblCurrencyMaster as currencymst with(nolock) on currencymrkup.crncyId = currencymst.crncyId  
  
  
  
    WHERE currencymrkup.usrId='+cast(@mainagentid as varchar(50))+''  
  
  
  
  
  
  
  
   exec(@dynamicSQL)  
  
     
  
  
  
      SET @dynamicSQL='DECLARE @MACurrencyID varchar(10) SET @MACurrencyID=(SELECT TCM.crncyCode from  '+@server+'.dbo.tblUserDetailSpecific as dc with(nolock) join  '+@server+'.dbo.tblCurrencyMaster as TCM with(nolock)  
  
  
  
     on dc.usrDtSpBaseCurrency=TCM.crncyId where usrid='+cast(@mainagentid as varchar(50))+')  
  
       
  
    DECLARE @SACurrencyID varchar(10) SET @SACurrencyID=(SELECT TCM.crncyCode from  '+@server+'.dbo.tblUserDetailSpecific as dc with(nolock) join  '+@server+'.dbo.tblCurrencyMaster as TCM with(nolock)  
  
  
  
    on dc.usrDtSpBaseCurrency=TCM.crncyId where usrid='+cast(@usrID as varchar(50))+')  
  
       
  
     insert into #Currency  
  
     SELECT @MACurrencyID,@SACurrencyID '  
  
  
  
   exec(@dynamicSQL)  
  
  
  
   select * from #Currency  
  
  
  
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetDidaHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================                  
-- Author:  <Suraj Singh>                  
-- Create date: <18 Apr, 2024>                  
-- Description: <Get static hotels on the basis of city code, country code and star rating  >                  
-- =============================================                  
CREATE PROCEDURE [dbo].[usp_GetDidaHotels]                   
@HotelCode nvarchar(50)=null,                  
@HotelName nvarchar(50)=null,                  
@cityCode nvarchar(50)=null,                  
@CountryCode nvarchar(50)=null,                  
@MinStarRating int=null,                  
@MaxStarRating int=null                  
                  
AS                  
BEGIN                  
                  
  BEGIN TRY                  
                   
   begin                  
       select hotelid as hotelcode,hotelname,Rating,'' as address,latitude,longitude,'' as HotelFrontImage,countrycode as CountryId,CityName                 
    from  [StaticData].[dbo].[tbldidatravel]  with(nolock)                  
    where localcityid=@CityCode and localcntid=@CountryCode and  CAST (rating as decimal(2,1))>=@MinStarRating AND CAST (rating as decimal(2))<=@MaxStarRating         
         or giataid=@HotelCode
         
        
 end                  
                    
                 
                     
       END TRY                  
    BEGIN CATCH                  
     select null                  
    END CATCH                  
                  
                  
END     
GO
/****** Object:  StoredProcedure [dbo].[usp_GetDidaSingleHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
     
-- =============================================          
-- Author:  <Suraj Singh>          
-- Create date: <23 Apr, 2024>     
-- Modify date: <23 Apr, 2024>     
-- Description: <Get static hotel details  >          
-- =============================================          
CREATE PROCEDURE [dbo].[usp_GetDidaSingleHotelDetail]           
@HotelCode nvarchar(50)=null         
          
AS          
BEGIN          
          
  BEGIN TRY          
           
   begin          
       select '' as HotelFrontImage,HotelID,hotelname,latitude,longitude,'' address,rating ,'' checkintime,'' checkouttime,'' description     
    from  [StaticData].[dbo].[tbldidatravel]  with(nolock)          
    where HotelID=@HotelCode         
 end          
            
         
             
       END TRY          
    BEGIN CATCH          
     select null          
    END CATCH          
          
          
END  
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEBookHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <14 May, 2019>
-- Description:	<Get EBooking Center hotel details on the basis of hotel code>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetEBookHotelDetail] 
@HotelId nvarchar(50)=null

AS
BEGIN

		BEGIN TRY

		
		    select  det.HotelId, htl.HotelName, det.phone, det.Description, det.Gallery, det.Facility
		    from [StaticData].[dbo].EBookingHotelList htl  WITH (NOLOCK) left join [StaticData].[dbo].EBookingHotelDetails det  WITH (NOLOCK) on htl.HotelId=det.HotelId
		    where htl.HotelId=@HotelId

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH

	  
	  
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetEBookingHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <14 May, 2019>
-- Description:	<Get static hotels of EBooking Center on the basis of city code, country code and star rating  >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetEBookingHotels] 
@CityCode nvarchar(50)=null,
@CountryCode nvarchar(50)=null,
@MinStarRating int=null,
@MaxStarRating int=null

AS
BEGIN

		BEGIN TRY
				 
		   select htl.HotelId,htl.HotelName,htlDtl.Address, htl.CityId as CityId, htl.CountryId, htl.StarRating,htlDtl.Location , htlDtl.Longitude, htlDtl.Latitude, htlDtl.Image
		   from [StaticData].[dbo].EBookingHotelList htl  WITH (NOLOCK) left join [StaticData].[dbo].EBookingHotelDetails htlDtl  WITH (NOLOCK) on htl.HotelId=htlDtl.HotelId
		   where (StarRating>=@MinStarRating and StarRating<=@MaxStarRating)  and CityId=@CityCode 
		   --and CountryId=@CountryCode
	         
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetExpediaCity]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <15 July, 2020>
-- Description:	<Get expedia city and country code >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetExpediaCity] 
@cityId varchar(200),
@countryId varchar(200),
@suplId int=null
AS
BEGIN

		BEGIN TRY
		  
		   select top 1 tcity.CityName as City, tcnt.SupCntId as CountryCode from tblSupplierCity_rel_new tcity inner join tblSupplierCountry_rel_new tcnt on tcity.cntid=tcnt.cntId and tcity.supId=tcnt.supId 
		   where tcity.supId=@suplId and tcity.CityId=@cityId and tcnt.cntId=@countryId

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END


GO
/****** Object:  StoredProcedure [dbo].[usp_GetExpediaHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <02 July, 2020>
-- Description:	<Get hotel details of expedia on the basis of hotel code>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetExpediaHotelDetail] 
@HotelCode nvarchar(50)=null

AS
BEGIN

		BEGIN TRY

		   select property_id as HotelId,  name as HotelName, htl.phone, htl.Fax, htl.checkin, htl.checkout , (description  + rates_information+'<p><b>Check-In Instructions:</b> '+ checkin_instructions+'</p>'+'<p><b>Special Instructions:</b> '+ specialcheckin_instructions+'</p>') as Details ,
		   (select facility as Facility from [StaticData].[dbo].tblExpediaHotelFacility  where property_id= @HotelCode FOR XML PATH(''),ROOT ('Facilities')) as Facilities,
		   (select ImageUrl as Path from [StaticData].[dbo].tblExpediaImages where property_id=@HotelCode  FOR XML RAW('Image'), ROOT('Images')) as Images
		   , htl.address1, htl.latitude , htl.longitude
		   from [StaticData].[dbo].tblExpediaHotels htl with(nolock) where htl.property_id=@HotelCode

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END
--exec usp_GetExpediaHotelDetail '27527551'
 
GO
/****** Object:  StoredProcedure [dbo].[usp_GetExpediaHotelPolicy]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================    
-- Author:  <Rajiv Kumar>    
-- Create date: <02 June, 2020>    
-- Description: <Get Terms and condition for a  hotel >    
-- =============================================    
CREATE PROCEDURE [dbo].[usp_GetExpediaHotelPolicy]     
@HotelCode nvarchar(50)=null    
AS    
BEGIN    
    
  BEGIN TRY    
  select  (ISNULL('<b>Check-In Instructions: </b> '+ checkin_instructions,'')+ ISNULL('<br/><b>Special Check-in Instructions: </b>'+ specialcheckin_instructions+'<br/><br/>','')+fees+policies+
  '<br/>'+'<a href="https://developer.expediapartnersolutions.com/terms/en" style="font-size: 15px; color:#04197d;">The partners Terms and Conditions - "https://developer.expediapartnersolutions.com/terms/en" </a>') as hotelpolicy from [StaticData].[dbo].[tblExpediaHotels]  with(nolock) where property_id=@HotelCode    
    
     
       END TRY    
    BEGIN CATCH    
     select null    
    END CATCH    
    
    
END    
    
--select * from [StaticData].[dbo].[tblExpediaHotels] where name like '%Flora Inn Hotel Dubai Airport%'    



GO
/****** Object:  StoredProcedure [dbo].[usp_GetExpediaHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec usp_GetExpediaHotels '73806','Admiral Plaza Hotel','','',3, 5
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <08 Oct, 2018>
-- Description:	<Get static hotels of Hoojoozat on the basis of city code, country code and star rating  >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetExpediaHotels] 
@HotelCode nvarchar(50)=null,
@HotelName nvarchar(50)=null,
@cityCode nvarchar(50)=null,
@CountryCode nvarchar(50)=null,
@MinStarRating int=null,
@MaxStarRating int=null

AS
BEGIN

		BEGIN TRY
			if LEN(ISNULL(@HotelCode,''))=0  
				set @HotelCode=null;      
			if LEN(ISNULL(@HotelName,''))=0     
						set @HotelName=null;   
						   
			if(@HotelCode is null  )
			begin
			    if(@CountryCode='MV')
				begin
					select top 1250 property_id as hotelcode,name as hotelname,rating, ISNULL(address1,'')+ ISNULL(', '+ address2,'')  as address,latitude,longitude, image, country_code, city from  [StaticData].[dbo].[tblExpediaHotels]  with(nolock)
					where country_code=@CountryCode and  CAST (rating as decimal(2,1))>=@MinStarRating AND CAST (rating as decimal(2))<=@MaxStarRating	order by rank
		 
				end
				else
				begin
					select top 1250 property_id as hotelcode,name as hotelname,rating, ISNULL(address1,'')+ ISNULL(', '+ address2,'')  as address,latitude,longitude, image, country_code, city from  [StaticData].[dbo].[tblExpediaHotels]  with(nolock)
					where city=@CityCode and country_code=@CountryCode and  CAST (rating as decimal(2,1))>=@MinStarRating AND CAST (rating as decimal(2))<=@MaxStarRating	order by rank
		        end
		   end
			else  --Search By Giata HotelCode
			begin
			   select hotelid into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where giataid=@HotelCode and localsupid=20
			   declare @count int =(select count(*) from #temp2)
	           if(@count >0)
	            begin
				 	select property_id as hotelcode,name as hotelname,rating, ISNULL(address1,'')+ ISNULL(', '+ address2,'')  as address,latitude,longitude, image, country_code, city from  [StaticData].[dbo].[tblExpediaHotels]  with(nolock)
				    where  CAST (rating as decimal(2,1))>=@MinStarRating AND CAST (rating as decimal(2))<=@MaxStarRating and property_id in(select hotelid from #temp2)
		
				end
				--else if(@HotelName is not null)  
				--begin
				--	select property_id as hotelcode,name as hotelname,rating, ISNULL(address1,'')+ ISNULL(', '+ address2,'')  as address,latitude,longitude, image, country_code, city from  [StaticData].[dbo].[tblExpediaHotels]  with(nolock)
				--    where city=@CityCode and country_code=@CountryCode and  CAST (rating as decimal(2,1))>=@MinStarRating AND CAST (rating as decimal(2))<=@MaxStarRating and name like '%'+@HotelName+'%'
				--end
			end
   
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetHoojHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <10 Oct, 2018>
-- Description:	<Get hotel details on the basis of hotel code>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetHoojHotelDetail] 
@HotelCode nvarchar(50)=null

AS
BEGIN

		BEGIN TRY

		   select Name Collate SQL_Latin1_General_CP1253_CI_AI as Name, htl.Phone, htl.Fax, (select 'https://www.hoojoozat.com/pictures/hotelpic/mainnew/'+ MainPic as Path  from [StaticData].[dbo].tblHoojoozatHotels where code=@HotelCode FOR XML RAW('Image'), ROOT('Images')) as Images
		   from [StaticData].[dbo].tblHoojoozatHotels htl 
		   where htl.Code=@HotelCode

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetHoojHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <08 Oct, 2018>
-- Description:	<Get static hotels of Hoojoozat on the basis of city code, country code and star rating  >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetHoojHotels] 
@DestinationCode nvarchar(50)=null,
@CountryCode nvarchar(50)=null,
@MinStarRating int=null,
@MaxStarRating int=null

AS
BEGIN

		BEGIN TRY
		  declare @norating int=0
		  if(@MinStarRating=0)
		  begin
		  set @norating=6
		  end

		  select Code, Name Collate SQL_Latin1_General_CP1253_CI_AI as Name,Address, DestinationCode as CityCode, Destination as CityName, CountryCode, Country, CategoryCode as StarRating, Longitude, Latitude, MainPic from [StaticData].[dbo].tblHoojoozatHotels where ((categorycode>=@MinStarRating and CategoryCode<=@MaxStarRating) or CategoryCode=@norating) and Destinationcode=@DestinationCode and CountryCode=@CountryCode
           
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END

GO
/****** Object:  StoredProcedure [dbo].[usp_gethtlidsbygiatadata_test]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[usp_gethtlidsbygiatadata_test]
@hotelcode varchar(50)= null,
@hotelname varchar(50)= null,
@city varchar(50)= null
as
begin

	if LEN(ISNULL(@HotelCode,''))=0        
				set @HotelCode=null

    if LEN(ISNULL(@HotelName,''))=0        
				set @HotelName=null

 select hotelid  into #temp from [StaticData]..[tblgiatadetails]  with (nolock) where 
	 giataid=@hotelcode and 
	 localsupid=9 
 
       declare @count int =(select count(*) from #temp)
	     if(@count >0)
		 begin
		select ItemCode as hotelid  from StaticData..tblRTS_Inventory with (nolock) 
		where ItemCode in (select hotelid from #temp)
		
		end
		else if(@hotelname is not null )--Search By Name
	    begin
		
	      select hotelid  into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where 	cityid=@city and	 localsupid=9 and hotelname like '%'+@hotelname+'%'
		select ItemCode as hotelid  from StaticData..tblRTS_Inventory with (nolock) 
		where ItemCode in (select hotelid from #temp2)
	
		--select ItemCode as hotelid  from StaticData..tblRTS_Inventory with (nolock) 
      -- where ItemName like  '%'+@hotelname+'%'

	end




 end



GO
/****** Object:  StoredProcedure [dbo].[usp_GetIOLCity]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================        
-- Author:  <Suraj Singh>        
-- Create date: <04 Feb, 2024>        
-- Description: <Get IOL city and country code >        
-- =============================================        
CREATE PROCEDURE [dbo].[usp_GetIOLCity]         
@cityId varchar(200),        
@countryId varchar(200),        
@suplId int=null        
AS        
BEGIN        
        
  BEGIN TRY        
            
     select distinct tcity.SupCityId as CityID,tcity.CityName as City, tcnt.SupCntId as CountryCode     
  from tblSupplierCity_rel_new tcity with(nolock)      
  inner join tblSupplierCountry_rel_new tcnt with(nolock) on tcity.cntid=tcnt.cntId and tcity.supId=tcnt.supId        
     where tcity.supId=@suplId and tcity.CityId=@cityId and tcnt.cntId=@countryId        
        
       END TRY        
    BEGIN CATCH        
     select null        
    END CATCH        
        
        
END   
  
GO
/****** Object:  StoredProcedure [dbo].[usp_GetIOLHotelFromSearch]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================          
-- Author:  <Suraj Singh>          
-- Create date: <06 Feb, 2024>  
-- Modify date: <07 Feb, 2024>  
-- Description: <Get room logs from room  >          
-- =============================================          
CREATE PROCEDURE [dbo].[usp_GetIOLHotelFromSearch]           
@trackNumber nvarchar(500)=null,          
@preID nvarchar(500)=null,  
@type int  
AS          
BEGIN          
          
  BEGIN TRY          
           
 begin     
 if(@type=1)  
 begin  
       select logresponseXML from tblapilog_room_iol with(nolock)  
	  where TrackNumber=@trackNumber and preID=@preID and SupplierID=50  
	  order by logID desc  
 end  
 end          
            
         
             
       END TRY          
    BEGIN CATCH          
     select null          
    END CATCH          
          
          
END 
GO
/****** Object:  StoredProcedure [dbo].[usp_GetIOLHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================            
-- Author:  <Suraj Singh>            
-- Create date: <04 Feb, 2024>            
-- Description: <Get static hotels on the basis of city code, country code and star rating  >            
-- =============================================            
CREATE PROCEDURE [dbo].[usp_GetIOLHotels]             
@HotelCode nvarchar(50)=null,            
@HotelName nvarchar(50)=null,            
@cityCode nvarchar(50)=null,            
@CountryCode nvarchar(50)=null,            
@MinStarRating int=null,            
@MaxStarRating int=null            
            
AS            
BEGIN            
            
  BEGIN TRY            
             
   begin            
       select iwtx_code as hotelcode,hotel_name as hotelname,star_rating as Rating,address,latitude,longitude,image as HotelFrontImage,country_code as CountryId,city_name as CityName           
    from  [StaticData].[dbo].[tbliol_hotellist]  with(nolock)            
    where city_code=@CityCode and country_code=@CountryCode and  CAST (star_rating as decimal(2,1))>=@MinStarRating AND CAST (star_rating as decimal(2))<=@MaxStarRating   
   
   
 --select a.hotelid as hotelcode,a.hotelname as hotelname,isnull(a.rating,0) as Rating,b.address,a.latitude,a.longitude,b.image as HotelFrontImage,a.countrycode as CountryId,a.destinationname as CityName      
 --from [StaticData].[dbo].tbliodata_giata a with(nolock)   
 --left join  [StaticData].[dbo].[tbliol_hotellist] b with(nolock)  on a.hotelid=b.iwtx_code  and a.countrycode=b.country_code  
 --where a.localsupid=50 and a.localcityid=@CityCode and a.localcntid=@CountryCode and  CAST (a.rating as decimal(2,1))>=@MinStarRating AND CAST (a.rating as decimal(2))<=@MaxStarRating   
  
  
 end            
              
           
               
       END TRY            
    BEGIN CATCH            
     select null            
    END CATCH            
            
            
END 
GO
/****** Object:  StoredProcedure [dbo].[usp_GetIOLSingleHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
   
-- =============================================        
-- Author:  <Suraj Singh>        
-- Create date: <07 Feb, 2024>   
-- Modify date: <13 Feb, 2024>   
-- Description: <Get static hotel details  >        
-- =============================================        
CREATE PROCEDURE [dbo].[usp_GetIOLSingleHotelDetail]         
@HotelCode nvarchar(50)=null       
        
AS        
BEGIN        
        
  BEGIN TRY        
         
   begin        
       select image as HotelFrontImage,iwtx_code,hotel_name,latitude,longitude,address,star_rating , checkintime,checkouttime,description   
    from  [StaticData].[dbo].[tbliol_hotellist]  with(nolock)        
    where iwtx_code=@HotelCode       
 end        
          
       
           
       END TRY        
    BEGIN CATCH        
     select null        
    END CATCH        
        
        
END 
GO
/****** Object:  StoredProcedure [dbo].[usp_Getprcheck_gal]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================



-- Author:		<Suraj Singh>



-- Create date: <21 Dec, 2018>

-- Modify date: <17 Apr, 2019>

-- Description:	<Get price check response >



-- =============================================



CREATE PROCEDURE [dbo].[usp_Getprcheck_gal] 



@TrackNumber nvarchar(50)=null



AS



BEGIN



		BEGIN TRY



            select logID,logresponseXML from TravayooService..tblapilogflt with(nolock)



			where TrackNumber=@TrackNumber and logTypeID=4 and SupplierID=50



			order by 1 desc



       END TRY



	   BEGIN CATCH



	    select null



	   END CATCH



END
GO
/****** Object:  StoredProcedure [dbo].[usp_Getprcheck_tbo]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <17 Apr, 2019>
-- Modify date: <17 Apr, 2019>
-- Description:	<Get price check response >
-- =============================================
CREATE PROCEDURE [dbo].[usp_Getprcheck_tbo] 
@TrackNumber nvarchar(500)=null,
@preID nvarchar(500)=null
AS
BEGIN
		BEGIN TRY
            select top 1 logresponseXML,logID from TravayooService..tblapilogflt with(nolock)
			where TrackNumber=@TrackNumber and preID=@preID and logTypeID=4 and SupplierID=51
			order by 2 desc
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Getprcheckfail_tbo]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Suraj Singh>

-- Create date: <17 Apr, 2019>

-- Modify date: <17 Apr, 2019>

-- Description:	<Get price check response >

-- =============================================

create PROCEDURE [dbo].[usp_Getprcheckfail_tbo] 

@TrackNumber nvarchar(500)=null,

@preID nvarchar(500)=null

AS

BEGIN

		BEGIN TRY

            select top 1 logresponseXML,logID from TravayooService..tblapilogFailTransflt with(nolock)

			where TrackNumber=@TrackNumber and preID=@preID and logTypeID=4 and SupplierID=51

			order by 2 desc

       END TRY

	   BEGIN CATCH

	    select null

	   END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[usp_getprebookreq]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <16 JAN, 2020>
-- Modify date: <16 JAN, 2020>
-- Description:	<Get prebook request for ticketing >
-- =============================================
CREATE PROCEDURE [dbo].[usp_getprebookreq]
@TrackNumber nvarchar(500)=null
AS
BEGIN
		BEGIN TRY
            select top 1 logrequestXML,logID from TravayooService..tblapilogflt with(nolock)
			where TrackNumber=@TrackNumber and logTypeID=4 and SupplierID=0
			order by 2 asc
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_getprebookresptbo]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <AUG 27, 2019>
-- Mofify date: <AUG 27, 2019>
-- Description:	<Procedure to get prebook response>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getprebookresptbo]
@TrackNumber nvarchar(500)=null
AS
BEGIN
		BEGIN TRY
           select top 1 logresponseXML,logID from TravayooService..tblapilogflt with(nolock) 
		   where TrackNumber=@tracknumber and logType='PreBook'
	       order by 2 desc
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_getrtsdata]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[usp_getrtsdata]
@code varchar(50)= null
as
begin
select inv.ItemCode,
Address,SmallFileName,Lotion.LocationName
--,BusinessCenter,FitnessCenter,
--Swimmingpool,Sauna,Spa,Restaurant,Tennis,Lotion.LocationName,Golf,Disabledfacilities,
--laundry,Babysitting,Porter,Parking,Roomservice,Carrenting,TourService,Exchange,Shop,BarLounge,
--Freenewspaper,Meetingroom,KidsClub,Luggagestorage
 from StaticData..tblRTS_Inventory as  inv with(nolock)
 --left join tblRTS_htlFacility  faci with (nolock) on inv.ItemCode=faci.ItemCode 
 left join StaticData..tblRTS_Img rtsimg with (nolock) on inv.ItemCode=rtsimg.ItemCode 
 left join StaticData..tblRTS_LocationMapping  Lotion with (nolock) on rtsimg.ItemCode=Lotion.ItemCode 
 where inv.CityCode=@code
 
 end
GO
/****** Object:  StoredProcedure [dbo].[usp_getrtsdata_tmp]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[usp_getrtsdata_tmp]
@code varchar(50)= null
as
begin
select inv.ItemCode,[Address],SmallFileName,Lotion.LocationName
--,BusinessCenter,FitnessCenter,
--Swimmingpool,Sauna,Spa,Restaurant,Tennis,Lotion.LocationName,Golf,Disabledfacilities,
--laundry,Babysitting,Porter,Parking,Roomservice,Carrenting,TourService,Exchange,Shop,BarLounge,
--Freenewspaper,Meetingroom,KidsClub,Luggagestorage
 from StaticData..tblRTS_Inventory as  inv with(nolock)
 --left join tblRTS_htlFacility  faci with (nolock) on inv.ItemCode=faci.ItemCode 
 left join StaticData..tblRTS_Img rtsimg with (nolock) on inv.ItemCode=rtsimg.ItemCode 
 left join StaticData..tblRTS_LocationMapping  Lotion with (nolock) on rtsimg.ItemCode=Lotion.ItemCode 
 where inv.CityCode in (@code)
  
 end
GO
/****** Object:  StoredProcedure [dbo].[usp_getsearchreqout]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[usp_getsearchreqout]  
@transid nvarchar(500) = null,    
@retVal bit out    
as    
begin
 begin try 
 select top 1 logrequestXML from TravayooService..tblapilogOut with(nolock) where TrackNumber=@transid and logType='TimeStart' and logTypeID=0 order by 1 
 end try
 begin catch 
  select @@TRANCOUNT 
 end catch   
end


GO
/****** Object:  StoredProcedure [dbo].[usp_GetSessionValue]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 -- =============================================
-- Author:		<Suraj Singh>
-- Create date: <Nov 26, 2018>
-- Mofify date: <JUL 29, 2019>
-- Description:	<Procedure to fetch/create session id>
-- =============================================
 CREATE proc [dbo].[usp_GetSessionValue]  -- 10017,0,'',0,''
 (
	@CustomerID bigint,
	@InsertStatus INT,
	@SessionVal [nvarchar](max),
	@ChkStatus INT OUTPUT,
	@SessionValue [nvarchar](max) OUTPUT
 )
 AS
 BEGIN
 IF(@InsertStatus = 1)
 BEGIN
    UPDATE [TravayooService].[dbo].[tblsessionmgmt] SET sessionStatus = 0 WHERE customerID =  @CustomerID AND sessionStatus =1
	INSERT INTO [TravayooService].[dbo].[tblsessionmgmt]([customerID],[sessionValue],[sessionUserCount],[sessionStatus],[sessioncreatedOn],[sessionexpiryOn])
	VALUES(@CustomerID,@SessionVal,1,1,GETDATE(),DATEADD(MINUTE,1200,GETDATE()))
	SET @ChkStatus = 1
	SET @SessionValue = @SessionVal
 END
 ELSE
 BEGIN
	Declare @RecordCount INT
	Declare @SessionCreOn DATETIME
	Declare @SessionExpOn DATETIME
	--Declare @SessionCount INT
	--Declare @SessionUserCount INT
	Declare @SessVal [nvarchar](max)
	SET @RecordCount = (SELECT COUNT(sessionID)sessionID FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	IF(@RecordCount = 0)
	BEGIN
		SET @ChkStatus = 0
	END
	ELSE
    BEGIN
	SET @SessionCreOn = (SELECT sessioncreatedOn FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	SET @SessionExpOn = (SELECT sessionexpiryOn FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	SET @SessVal = (SELECT sessionValue FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	--SET @SessionCount = (SELECT sessionUserCount FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	--SET @SessionUserCount = (SELECT sessionUserCount FROM [TravayooService].[dbo].[tblsessionmgmt] WHERE customerID =  @CustomerID AND sessionStatus = 1)
	--IF((GETDATE() > @SessionExpOn) OR (@SessionUserCount = 200))
	IF((GETDATE() > @SessionExpOn))
	BEGIN
		SET @ChkStatus = 0
	END 
	ELSE
	BEGIN
	    --UPDATE [TravayooService].[dbo].[tblsessionmgmt] SET sessionUserCount = (@SessionCount + 1) WHERE customerID =  @CustomerID AND sessionStatus = 1
		SET @ChkStatus = 1
		SET @SessionValue = @SessVal
	END
    END
	END
 END




GO
/****** Object:  StoredProcedure [dbo].[usp_GetSessionValueAMA]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 -- =============================================
-- Author:		<Suraj Singh>
-- Create date: <Nov 26, 2018>
-- Mofify date: <JUL 29, 2019>
-- Description:	<Procedure to fetch/create session id>
-- =============================================
 create proc [dbo].[usp_GetSessionValueAMA]  -- 10017,0,'',0,''
 (
	@CustomerID bigint,
	@InsertStatus INT,
	@SessionVal [nvarchar](max),
	@ChkStatus INT OUTPUT,
	@SessionValue [nvarchar](max) OUTPUT
 )
 AS
 BEGIN
 IF(@InsertStatus = 1)
 BEGIN
    UPDATE [TravayooService].[dbo].[tblsessionmgmt] SET sessionStatus = 0 WHERE customerID =  @CustomerID AND sessionStatus =1
	INSERT INTO [TravayooService].[dbo].[tblsessionmgmt]([customerID],[sessionValue],[sessionUserCount],[sessionStatus],[sessioncreatedOn],[sessionexpiryOn])
	VALUES(@CustomerID,@SessionVal,1,1,GETDATE(),DATEADD(MINUTE,1200,GETDATE()))
	SET @ChkStatus = 1
	SET @SessionValue = @SessionVal
 END
 ELSE
 BEGIN
	Declare @RecordCount INT
	Declare @SessionCreOn DATETIME
	Declare @SessionExpOn DATETIME
	--Declare @SessionCount INT
	--Declare @SessionUserCount INT
	Declare @SessVal [nvarchar](max)
	SET @RecordCount = (SELECT COUNT(sessionID)sessionID FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	IF(@RecordCount = 0)
	BEGIN
		SET @ChkStatus = 0
	END
	ELSE
    BEGIN
	SET @SessionCreOn = (SELECT sessioncreatedOn FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	SET @SessionExpOn = (SELECT sessionexpiryOn FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	SET @SessVal = (SELECT sessionValue FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	--SET @SessionCount = (SELECT sessionUserCount FROM [TravayooService].[dbo].[tblsessionmgmt] WITH (NOLOCK) WHERE customerID =  @CustomerID AND sessionStatus = 1)
	--SET @SessionUserCount = (SELECT sessionUserCount FROM [TravayooService].[dbo].[tblsessionmgmt] WHERE customerID =  @CustomerID AND sessionStatus = 1)
	--IF((GETDATE() > @SessionExpOn) OR (@SessionUserCount = 200))
	IF((GETDATE() > @SessionExpOn))
	BEGIN
		SET @ChkStatus = 0
	END 
	ELSE
	BEGIN
	    --UPDATE [TravayooService].[dbo].[tblsessionmgmt] SET sessionUserCount = (@SessionCount + 1) WHERE customerID =  @CustomerID AND sessionStatus = 1
		SET @ChkStatus = 1
		SET @SessionValue = @SessVal
	END
    END
	END
 END




GO
/****** Object:  StoredProcedure [dbo].[usp_getslabfltmarkup]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <Jan 09, 2019>
-- Mofify date: <Jan 09, 2019>
-- Description:	<Procedure to get slab basis markup>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getslabfltmarkup]
@custID nvarchar(500)=null
AS
BEGIN
   	BEGIN TRY  		
        select rangeFrom,rangeTo,mrkupTypeId,mrkupValue from Travayooqa..tblFlexFlightMarkups with(nolock)
		where mrkupStatus=1 and CustID=@custID
    END TRY
	BEGIN CATCH
	       select @@TRANCOUNT
     END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Getsupplierlist]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usp_Getsupplierlist]    
    
@customerid varchar(100)    
    
,@agentid varchar(100)    
    
AS     
    
Begin    
    
declare @server varchar(100)    
    
declare @dbname varchar(100)    
    
declare @query varchar(max)    
    
declare @branchid varchar(50)    
    
    
    
set @server =(select top 1 dbname from xmloutdetails where customerid=@customerid  and supplierid=0 and agencyid=@agentid  and dbname is not null) --    
    
set @branchid =(select top 1 BranchID from xmloutdetails where customerid=@customerid and agencyid=@agentid  and dbname is not null) --and supplierid=0    
    
    
    
if (@server='SAAS')    
    
Begin    
    
set @dbname='Travayoo'    
    
END    
    
    
    
else    
    
    
    
begin    
    
set @dbname='TravayooHA'    
    
end    
    
if ((@CustomerId=51935) and (@agentid=82273))    
begin    
    
print 'aaa'    
    
set @query='Select supl.suplId    
    
  from '+@dbname+'.[dbo].tblUsr_Supplier_Rel  usrSupl inner join  '+@dbname+'.[dbo].tblSupplierMaster supl on (usrSupl.suplId = supl.suplId and  usrSupl.usrId = '+@agentid+' and supl.suplStatus=1 and supl.supCatId=1 and supl.srvId=1)    
    
  inner join  '+@dbname+'.[dbo].tblSupplierMaster_Locale suplLoc on usrSupl.suplId = suplLoc.suplId  and suplLoc.cultId =1  where     
    
  usrSupl.suplId IN (SELECT SupplierID FROM '+@dbname+'.dbo.tblusr_Suppl_Status WHERE usrid='+@branchid+' AND SupStatus=1)    
    
  AND usrSupl.suplId IN (SELECT SupplierID FROM '+@dbname+'.dbo.tblusr_Suppl_Status WHERE usrid='+@CustomerId+' AND SupStatus=1 )and  supl.suplid in(1,13)'    
    
    
end    
else    
begin    
    
     
    
set @query ='Select supl.suplId    
    
  from '+@dbname+'.[dbo].tblUsr_Supplier_Rel  usrSupl inner join  '+@dbname+'.[dbo].tblSupplierMaster supl on (usrSupl.suplId = supl.suplId and  usrSupl.usrId = '+@agentid+' and supl.suplStatus=1 and supl.supCatId=1 and supl.srvId=1)    
    
  inner join  '+@dbname+'.[dbo].tblSupplierMaster_Locale suplLoc on usrSupl.suplId = suplLoc.suplId  and suplLoc.cultId =1  where     
    
  usrSupl.suplId IN (SELECT SupplierID FROM '+@dbname+'.dbo.tblusr_Suppl_Status WHERE usrid='+@branchid+' AND SupStatus=1)    
    
  AND usrSupl.suplId IN (SELECT SupplierID FROM '+@dbname+'.dbo.tblusr_Suppl_Status WHERE usrid='+@CustomerId+' AND SupStatus=1 )and  supl.suplid in    
  (36,4,13,6,39,21,5,11,1,37,7,3,20)' --16,17,23,    
       
print @query    
    
end    
    
    
exec(@query)    
    
    
    
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetTABalance]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <07 Sept, 2018>
-- Description:	<Get travel agent's balance>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetTABalance] 
@customerID varchar(900)=null,
@agencyID varchar(900)=null
AS
BEGIN
		BEGIN TRY
		SELECT (crdtConvertedBalanceCreditLimit + crdtConvertedBalanceCashAmount) AS AvailableLimit  FROM TravayooQA.dbo.tblCreditLimit 
		WHERE crdtMainAgentId=@customerID and crdtSubAgentId=@agencyID
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetTravcoCity]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <13 Aug, 2018>
-- Description:	<Get travco city code and country code>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetTravcoCity] 
@cityId varchar(900),
@suplId int=null
AS
BEGIN

		BEGIN TRY
		   if(@suplId is null)
		   begin
			set @suplId=7
		   end
		   select tcity.SupCityId as CityCode, tcnt.SupCntId as CountryCode, tcity.CityName as TownCode from tblSupplierCity_rel_new tcity inner join tblSupplierCountry_rel_new tcnt on tcity.cntid=tcnt.cntId and tcity.supId=tcnt.supId 
		   where tcity.supId=@suplId and tcity.CityId=@cityId

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetTravcoHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <10 July, 2018>
-- Description:	<Get hotel details on the basis of hotel code>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetTravcoHotelDetail] 
@HotelCode nvarchar(50)=null

AS
BEGIN

		BEGIN TRY

		   select HotelName, htl.Telephone, htl.Fax, htl.CheckInTime, htl.CheckOutTime , (Overview +''+ Location +''+ det.Exterior +''+ LobbyAndInterior +''+ LeisureFacilities +''+ Rooms +''+ RestaurantsAndBars +''+ FamilyInformation +''+ OtherInformation) as Details ,
		   (select HotelAmenityName as Facility from [StaticData].[dbo].tblTravcoHotelFacilities  where tblTravcoHotelFacilities.HotelCode = htl.HotelCode FOR XML PATH(''),ROOT ('Facilities')) as Facilities,
		   (select ImagePath as Path from [StaticData].[dbo].tblTravcoHotelImages where HotelCode=@HotelCode and ImageID!='map' order by ImageID DESC FOR XML RAW('Image'), ROOT('Images')) as Images
		   from [StaticData].[dbo].tblTravcoHotels htl left join [StaticData].[dbo].tblTravcoHotelDescription det on htl.HotelCode=det.HotelCode 
		   where htl.HotelCode=@HotelCode

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetTravcoHotelRatePlan]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <8 Aug, 2018>
-- Description:	<Get hotel rate plan details>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetTravcoHotelRatePlan] 
@HotelCode nvarchar(50)=null

AS
BEGIN

		BEGIN TRY
		   select  HotelCode, RatePlanCode, RatePlanName,BoardBasisCode, BoardBasisName from [StaticData].[dbo].tblTravcoHotelRatePlan where HotelCode=@HotelCode

	       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetTravcoHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <09 July, 2018>
-- Description:	<Get static hotels on the basis of city code, country code and star rating  >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetTravcoHotels] 
@CityCode nvarchar(50)=null,
@CountryCode nvarchar(50)=null,
@MinStarRating nvarchar(2)=null,
@MaxStarRating nvarchar(2)=null

AS
BEGIN

		BEGIN TRY

           select htl.HotelCode, HotelName, Address,CityCode, CityName, CountryCode, CountryName, CityAreaName, LocationName, Longitude, Latitude, StarRatingCode, StarRatingName, img.ImagePath,
		   (select HotelAmenityName as Facility from [StaticData].[dbo].tblTravcoHotelFacilities fac where fac.HotelCode=htl.HotelCode FOR XML PATH(''), ROOT ('Facilities')) as Facilities
		   from [StaticData].[dbo].tblTravcoHotels htl left join [StaticData].[dbo].tblTravcoHotelImages img on htl.HotelCode=img.HotelCode and ImageID='front' 
		   where htl.CityCode=@CityCode and htl.CountryCode=@CountryCode and (CAST(htl.StarRatingCode as int)>=CAST(@MinStarRating as int) and CAST(htl.StarRatingCode as int)<=CAST(@MaxStarRating as int))

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetTravcoHotels_Test]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <09 July, 2018>
-- Description:	<Get static hotels on the basis of city code, country code and star rating  >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetTravcoHotels_Test] 
@HotelCode nvarchar(50)=null,
@HotelName nvarchar(50)=null,
@CityCode nvarchar(50)=null,
@CountryCode nvarchar(50)=null,
@MinStarRating nvarchar(2)=null,
@MaxStarRating nvarchar(2)=null

AS
BEGIN

		BEGIN TRY
			if LEN(ISNULL(@HotelCode,''))=0        
				set @HotelCode=null
	       if LEN(ISNULL(@HotelName,''))=0        
				set @HotelName=null



	if(@HotelCode is  null 	 )
	begin
           select htl.HotelCode, HotelName, Address,CityCode, CityName, CountryCode, CountryName, CityAreaName, LocationName, Longitude, Latitude, StarRatingCode, StarRatingName, img.ImagePath,
		   (select HotelAmenityName as Facility from [StaticData].[dbo].tblTravcoHotelFacilities fac where fac.HotelCode=htl.HotelCode FOR XML PATH(''), ROOT ('Facilities')) as Facilities
		   from [StaticData].[dbo].tblTravcoHotels htl  with (nolock) left join [StaticData].[dbo].tblTravcoHotelImages img  with (nolock) on htl.HotelCode=img.HotelCode and ImageID='front' 
		   where htl.CityCode=@CityCode and htl.CountryCode=@CountryCode and (CAST(htl.StarRatingCode as int)>=CAST(@MinStarRating as int) and CAST(htl.StarRatingCode as int)<=CAST(@MaxStarRating as int))
	 end
	 else  if(@HotelCode is not null  )
	 begin
	 -- supplierid against giata id for hotel
	--select '"'+hotelid+'"' as h  into #temp2 from [StaticData]..[tblgiatadetails] where giataid=@HotelCode and localsupid=7
	select hotelid  into #temp2 from [StaticData]..[tblgiatadetails] where giataid=@HotelCode and localsupid=7
	 declare @count int =(select count(*) from #temp2)
	if(@count >0)
	begin
	
           select htl.HotelCode, HotelName, Address,CityCode, CityName, CountryCode, CountryName, CityAreaName, LocationName, Longitude, Latitude, StarRatingCode, StarRatingName, img.ImagePath,
		   (select HotelAmenityName as Facility from [StaticData].[dbo].tblTravcoHotelFacilities fac  with (nolock) where fac.HotelCode=htl.HotelCode FOR XML PATH(''), ROOT ('Facilities')) as Facilities
		   from [StaticData].[dbo].tblTravcoHotels htl  with (nolock) left join [StaticData].[dbo].tblTravcoHotelImages img  with (nolock) on htl.HotelCode=img.HotelCode and ImageID='front' 
		   where htl.HotelCode in(select * from #temp2 )
	 end
	
	 else if(@HotelName is not null )
	 begin
	-- select '"'+hotelid+'"' as h into #temp3 from [StaticData]..[tblgiatadetails] where  cityid=@CityCode and	 localsupid=7 and hotelname like '%'+@HotelName+'%'
	 select hotelid into #temp3 from [StaticData]..[tblgiatadetails] where  cityid=@CityCode and	 localsupid=7 and hotelname like '%'+@HotelName+'%'
	   select htl.HotelCode, HotelName, Address,CityCode, CityName, CountryCode, CountryName, CityAreaName, LocationName, Longitude, Latitude, StarRatingCode, StarRatingName, img.ImagePath,
		   (select HotelAmenityName as Facility from [StaticData].[dbo].tblTravcoHotelFacilities fac  with (nolock) where fac.HotelCode=htl.HotelCode FOR XML PATH(''), ROOT ('Facilities')) as Facilities
		   from [StaticData].[dbo].tblTravcoHotels htl  with (nolock) left join [StaticData].[dbo].tblTravcoHotelImages img  with (nolock) on htl.HotelCode=img.HotelCode and ImageID='front' 
		   where htl.HotelCode in(select * from #temp3 )
	
	 end
	  end
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetVotHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <26 Dec, 2018>
-- Description:	<Get hotel details on the basis of hotel code>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetVotHotelDetail] 
@HotelCode nvarchar(50)=null

AS
BEGIN

		BEGIN TRY
		
		    select htl.HotelName, det.phone, det.Fax, (det.Description +''+ det.DescriptionLocation +''+ det.DescriptionRemarks +''+ det.DescriptionRoom +''+ det.DescriptionAdditional +''+ det.DescritionSport) as Details ,
		   (select ( select Picture as Path from (
                     select  mainpic as Picture from [StaticData].[dbo].tblvothoteldetail where hotelcode=@HotelCode
					 UNION
					 select 'http://www.votbookings.com/Pictures/HotelPic/'+ Picture as Picture from [StaticData].[dbo].tblvothotelimages where hotelcode=@HotelCode
            ) A  FOR XML RAW('Image'), ROOT('Images') )) As Images
		   from [StaticData].[dbo].tblvothotellist htl left join [StaticData].[dbo].tblvothoteldetail det on htl.HotelCode=det.HotelCode 
		   where htl.HotelCode=@HotelCode

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH

	  
	  
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetVotHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <05 Dec, 2018>
-- Description:	<Get static hotels of VOT on the basis of city code, country code and star rating  >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetVotHotels] 
@CityCode nvarchar(50)=null,
@CountryCode nvarchar(50)=null,
@MinStarRating int=null,
@MaxStarRating int=null

AS
BEGIN

		BEGIN TRY
		--declare @CityCode nvarchar(50)='1'
		--declare @CountryCode nvarchar(50)='1' 
		--declare @MinStarRating int=0
		--declare  @MaxStarRating int=3

		  declare @norating int=0, @norating2 int=0, @FourPlusRating int=0
		  if(@MinStarRating=0)
		  begin
			set @norating=0
			set @norating2=13
		  end
		  if(@MaxStarRating=5)
		  begin
			set @MaxStarRating=16
		  end
		  
		   select htl.HotelCode,htl.HotelName Collate SQL_Latin1_General_CP1253_CI_AI as HotelName,htl.Address, htl.DestinationCode as CityCode, htlDtl.Destination as CityName, htl.CountryCode, htlDtl.Country as CountryName,
		    htl.CategoryCode StarRating,htlDtl.Zone as Area, htlDtl.Longitude, htlDtl.Latitude, htlDtl.MainPic
		   from [StaticData].[dbo].tblvothotellist htl left join [StaticData].[dbo].tblvothoteldetail htlDtl  on htl.Hotelcode=htlDtl.HotelCode
		   where ((categorycode>=@MinStarRating and CategoryCode<=@MaxStarRating and CategoryCode!=15 and CategoryCode!=14 and CategoryCode!=11 and CategoryCode!=10 and CategoryCode!=12) or CategoryCode=@norating or CategoryCode=@norating2) and Destinationcode=@CityCode and CountryCode=@CountryCode
	         
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetVotSearchResponse]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Author:  <Rajiv Kumar>  
-- Create date: <09 Dec, 2018>  
-- Description: <Get Vot Search response to bind rooms>  
-- =============================================  
CREATE PROCEDURE [dbo].[usp_GetVotSearchResponse]   
@TrackNumber nvarchar(500),  
@SupplierId int,  
@logTypeId int=null  
AS  
BEGIN  
  
  BEGIN TRY  
   if(@logTypeId is null)  
     begin  
   set @logTypeId=1  
     end  
  
       select top 1 logresponseXML from tblapilog_search with(nolock) where tracknumber=@TrackNumber and supplierid=@SupplierId and logtypeid=@logTypeId order by logid desc  
             
       END TRY  
    BEGIN CATCH  
     select null  
    END CATCH  
  
  
END  
  
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWBHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <18 March, 2020>
-- Description:	<Get hotel details on the basis of hotel code>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetWBHotelDetail] 
@HotelCode nvarchar(50)=null

AS
BEGIN

		BEGIN TRY
		
		  select htl.ID_Hotel as HotelId, htl.NOMBRE_HOTEL as HotelName,htl.HotelDetail as Details, htl.PHONE, htl.FAX, '' as details, htl.CHECKIN, htl.CHECKOUT
		  ,
		  (select AttributeDetaildescription as Facility from [StaticData].[dbo].[tblWelcomebeds_facility] with(nolock)  where hotelcode = @HotelCode FOR XML PATH(''),ROOT ('Facilities')) as Facilities,
		    (select ( select Images as Path from (
                      select top 15 URLIMG as Images from [StaticData].[dbo].tblwelcomebedshotelimages with(nolock) where ID_HOTEL=@HotelCode and urlimg not like '%thumbnail%'
            ) A  FOR XML RAW('Image'), ROOT('Images') )) As Images
		   from [StaticData].[dbo].tblwelcomebedshotellist htl with(nolock) where htl.ID_Hotel =@HotelCode

       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH

END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetWBHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rajiv Kumar>
-- Create date: <02 March, 2020>
-- Description:	<Get static hotels of WelcomeBeds on the basis of city code, country code and star rating  >
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetWBHotels] 
@CityCode nvarchar(50)=null,
@CountryCode nvarchar(50)=null,
@TownCode nvarchar(50)=null,
@MinStarRating int=null,
@MaxStarRating int=null

AS
BEGIN

		BEGIN TRY

		if (@TownCode is null or @TownCode = '' or @TownCode = '""')
		begin
				 
		   --select distinct htl.ID_HOTEL as HotelId,htl.NOMBRE_HOTEL as HotelName,ID_COUNTRY as CountryId, ID_PROVINCE as CityId, htl.NAME_STREET as Address,  htl.Longitude, htl.Latitude, htl.NAME_TOWN as Area
		   --from [StaticData].[dbo].tblWelcomebedsHotelList htl  WITH (NOLOCK) inner join [StaticData].[dbo].[tblWelcomebedsHotelImages] img on img.ID_HOTEL=htl.ID_HOTEL
		   --where ID_PROVINCE=@CityCode and ID_COUNTRY=@CountryCode
		   WITH cte_customers AS
		    (
				   SELECT ROW_NUMBER() OVER (PARTITION BY id_hotel ORDER BY SEQUENCE) row_num,ID_HOTEL ,NAME_HOTEL,SEQUENCE,URLIMG
					FROM [StaticData].[dbo].[tblWelcomebedsHotelImages] 
			)  
			select distinct htl.ID_HOTEL as HotelId,htl.NOMBRE_HOTEL as HotelName,ID_COUNTRY as CountryId, ID_PROVINCE as CityId, htl.NAME_STREET as Address,  htl.Longitude, htl.Latitude, htl.NAME_TOWN as Area ,img.URLIMG as Img 
			from cte_customers img
			inner join [StaticData].[dbo].tblWelcomebedsHotelList htl  WITH (NOLOCK) on img.ID_HOTEL = htl.ID_HOTEL
			where row_num=1 and ID_PROVINCE=@CityCode and ID_COUNTRY=@CountryCode

		 end
		 else
		 begin
		   --select distinct htl.ID_HOTEL as HotelId,htl.NOMBRE_HOTEL as HotelName,ID_COUNTRY as CountryId, ID_PROVINCE as CityId, htl.NAME_STREET as Address,  htl.Longitude, htl.Latitude, htl.NAME_TOWN as Area
		   --from [StaticData].[dbo].tblWelcomebedsHotelList htl  WITH (NOLOCK) 
		   --where ID_PROVINCE=@CityCode and ID_COUNTRY=@CountryCode and ID_TOWN=@TownCode
		   	WITH cte_customers AS
		    (
				   SELECT ROW_NUMBER() OVER (PARTITION BY id_hotel ORDER BY SEQUENCE) row_num,ID_HOTEL ,NAME_HOTEL,SEQUENCE,URLIMG
					FROM [StaticData].[dbo].[tblWelcomebedsHotelImages] 
			)  
			select distinct htl.ID_HOTEL as HotelId,htl.NOMBRE_HOTEL as HotelName,ID_COUNTRY as CountryId, ID_PROVINCE as CityId, htl.NAME_STREET as Address,  htl.Longitude, htl.Latitude, htl.NAME_TOWN as Area ,img.URLIMG as Img 
			from cte_customers img
			inner join [StaticData].[dbo].tblWelcomebedsHotelList htl  WITH (NOLOCK) on img.ID_HOTEL = htl.ID_HOTEL
			where row_num=1 and ID_PROVINCE=@CityCode and ID_COUNTRY=@CountryCode and ID_TOWN=@TownCode
	
		 end
		 
	         
       END TRY
	   BEGIN CATCH
	    select null
	   END CATCH


END

GO
/****** Object:  StoredProcedure [dbo].[usp_GetWTECity]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================        
-- Author:  <Suraj Singh>        
-- Create date: <09 Aug, 2023>        
-- Description: <Get Withinearth city and country code >        
-- =============================================        
CREATE PROCEDURE [dbo].[usp_GetWTECity]         
@cityId varchar(200),        
@countryId varchar(200),    
@suplId int=null        
AS        
BEGIN        
        
  BEGIN TRY        
            
     select top 1 tcity.SupCityId as CityID,tcity.CityName as City, tcnt.SupCntId as CountryCode from tblSupplierCity_rel_new tcity with(nolock)      
  inner join tblSupplierCountry_rel_new tcnt with(nolock) on tcity.cntid=tcnt.cntId and tcity.supId=tcnt.supId        
     where tcity.supId=48 and tcity.CityId=251 and tcnt.cntId=56        
         
    
       END TRY        
    BEGIN CATCH        
     select null        
    END CATCH        
        
        
END 
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWTECountry]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================      
-- Author:  <Suraj Singh>      
-- Create date: <08 Feb, 2024>      
-- Description: <Get Withinearth country name >      
-- =============================================      
CREATE PROCEDURE [dbo].[usp_GetWTECountry]          
@nationalityid varchar(200),  
@suplId int=null      
AS      
BEGIN      
      
  BEGIN TRY      
       
  select top 1 wtec.name as PaxNationality from tblSupplierCountry_rel_new a with(nolock)  
  inner join StaticData..tbl_wtecountry_master wtec with(nolock) on a.SupCntId=wtec.CountryId  
  where cntid=@nationalityid and supid=@suplId  
  
       END TRY      
    BEGIN CATCH      
     select null      
    END CATCH      
      
      
END      
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWTEHotels]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================    
-- Author:  <Suraj Singh>    
-- Create date: <09 Aug, 2023>    
-- Description: <Get static hotels on the basis of city code, country code and star rating  >    
-- =============================================    
CREATE PROCEDURE [dbo].[usp_GetWTEHotels]     
@HotelCode nvarchar(50)=null,    
@HotelName nvarchar(50)=null,    
@cityCode nvarchar(50)=null,    
@CountryCode nvarchar(50)=null,    
@MinStarRating int=null,    
@MaxStarRating int=null    
    
AS    
BEGIN    
    
  BEGIN TRY    
     
   begin    
       select HotelId as hotelcode,HotelName as hotelname,Rating, address,latitude,longitude, HotelFrontImage, CountryId, CityName   
    from  [StaticData].[dbo].[tbl_wteHotelList_master]  with(nolock)    
    where CityId=@CityCode and CountryId=@CountryCode and  CAST (rating as decimal(2,1))>=@MinStarRating AND CAST (rating as decimal(2))<=@MaxStarRating   
	
 end    
      
   
       
       END TRY    
    BEGIN CATCH    
     select null    
    END CATCH    
    
    
END    

select * from  [StaticData].[dbo].[tbl_wteHotelList_master] 
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWTEPolicy]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================    
-- Author:  <Suraj Singh>    
-- Create date: <20 Aug, 2023>    
-- Description: <Get Withinearth CXL Policy >    
-- =============================================    
CREATE PROCEDURE [dbo].[usp_GetWTEPolicy]     
@tracknumber varchar(200),    
@hotelid varchar(200)  
AS    
BEGIN    
    SET NOCOUNT ON;        
  SET XACT_ABORT ON  
  BEGIN TRY    
        
     select top 1 logresponseXML from tblapilog_room with(nolock)    
     where TrackNumber=@tracknumber and HotelId=@hotelid order by logID desc
       END TRY    
    BEGIN CATCH    
     select null    
    END CATCH    
    
    
END    
GO
/****** Object:  StoredProcedure [dbo].[usp_GetWTESingleHotelDetail]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================    
-- Author:  <Suraj Singh>    
-- Create date: <26 Oct, 2023>    
-- Description: <Get static hotel details  >    
-- =============================================    
CREATE PROCEDURE [dbo].[usp_GetWTESingleHotelDetail]     
@HotelCode nvarchar(50)=null   
    
AS    
BEGIN    
    
  BEGIN TRY    
     
   begin    
       select HotelFrontImage  ,HotelId 
    from  [StaticData].[dbo].[tbl_wteHotelList_master]  with(nolock)    
    where HotelId=@HotelCode   
 end    
      
   
       
       END TRY    
    BEGIN CATCH    
     select null    
    END CATCH    
    
    
END 
GO
/****** Object:  StoredProcedure [dbo].[USP_GeTXMLOUTCreds]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[USP_GeTXMLOUTCreds] @outcustid varchar(100),@outagencyID varchar(100)='',@outsupplierID varchar(100)=''
As
Begin
 if (@outsupplierID<>'')
 BEGIN
 select * FROM XMLOutDetails WITH(NOLOCK) where outcustid=@outcustid and  outagencyid= @outagencyID and outsupplierid=@outsupplierID and isnull(status,0)=0
 END
 else
 Begin
  select * FROM XMLOutDetails WITH(NOLOCK) where outcustid=@outcustid  and isnull(status,0)=0
	and agencyID = @outagencyID
 END
end
GO
/****** Object:  StoredProcedure [dbo].[USP_GeTXMLOUTHostName]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[USP_GeTXMLOUTHostName] @outcustid varchar(100),@customerid varchar(100)

As

Begin

	select 
	DbName FROM XMLOutDetails where outcustid=@outcustid and customerid=@customerid and supplierID=0 and levelid=0

end
GO
/****** Object:  StoredProcedure [dbo].[USP_GeTXMLOUTMarkup]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  
  
  
CREATE PROC   
  
  
  
[dbo].[USP_GeTXMLOUTMarkup]  @UsrID BIGINT, @SupplierID VARCHAR(100), @MarkupType VARCHAR(50),@SrvID int=1  
  
  
  
AS  
  
  
  
  
  
  
  
--[USP_GeTXMLOUTMarkup]  10005,'','HA',1  
  
  
  
  
  
  
  
BEGIN  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
   DECLARE @server varchar(100)  
  
  
  
   DECLARE @MainAgentID BIGINT  
  
  
  
   DECLARE @dynamicXML VARCHAR(MAX)  
  
  
  
   CREATE TABLE #tempXMLOUT(MainAgentID BIGINT)   
  
  
  
   CREATE TABLE #usrID(UsrID BIGINT,tempUSRID BIGINT,partenid BIGINT,usrtype BIGINT)  
  
   
  
  
  
  
  
  
  
    Create Table #tempTable  (  
  
  
  
 [USRID] [bigint] NULL,  
  
  
  
 [SubAgentCurrency] [int] NULL,  
  
  
  
 [CurrencyCode] VARCHAR(50) NULL,   
  
  
  
 [SuplID] [int] NULL,   
  
  
  
 [MainAgentMarkupType] [int] NULL,  
  
  
  
 [MainAgentMrkupVal] [float] NULL,  
  
  
  
 [MainAgentMrkupCancellationVal] [float] NULL,  
  
  
  
 [SubAgentMrkupType] [int] NULL,  
  
  
  
 [SubAgentMrkupVal] [float] NULL,  
  
  
  
 [SubAgentMrkupCancellationVal] [float] NULL,  
  
  
  
 [MainAgentID] [bigint] NULL,  
  
  
  
 SupplierBuffer [bigint] NULL,  
  
  
  
 [StaffUsrMrkupType] [int] NULL,  
  
  
  
 [StaffUsrMrkup] [float] NULL,  
  
  
  
 [StaffUsrCXLMrkup] [float] NULL)  
  
  
  
  
  
  
  
   INSERT INTO #usrID   
  
  
  
   SELECT @UsrID,0,0,0  
  
  
  
   
  
  
  
  
  
  
  
   IF (@MarkupType='HA')  
  
  
  
   Begin  
  
  
  
    SET @server='TravayooHA'  
  
  
  
   END  
  
  
  
   ELSE  
  
  
  
   BEGIN  
  
  
  
   SET @server='Travayoo'  
  
  
  
   END  
  
  
  
  
  
  
  
   SET @dynamicXML='update #usrID set partenid=usrparentid,usrtype=usrtypid from '+@server+'.dbo.tbluser as t where t.usrid in(select usrid from #usrID)'  
  
  
  
   EXEC(@dynamicXML)  
  
  
  
  
  
  
  
      IF ((select usrtype from #usrID) =12)  
  
  
  
   BEGIN  
  
  
  
   UPDATE #usrID set tempUSRID=UsrID,UsrID=partenid      
  
  
  
     
  
  
  
   END  
  
  
  
      SET @dynamicXML='INSERT INTO #tempXMLOUT(MainAgentID)      
  
  
  
    EXEC '+@server+'.dbo.[GetMainAgentID] '+cast(@UsrID as varchar(50))+''  
  
  
  
    EXEC (@dynamicXML)  
  
  
  
  
  
  
  
  
  
    IF (@SupplierID<>'')  
    BEGIN  
    INSERT INTO #tempTable(SuplID)  
    SELECT item from dbo.SplitString(@SupplierID, ',')   
    END  
    ELSE  
    BEGIN  
     SET @dynamicXML='INSERT INTO #tempTable(SuplID) select SUPLID from '+@server+'.DBO.tblUsr_Supplier_Rel WHERE usrId='+cast(@UsrID as varchar(50))+''  
        EXEC(@dynamicXML)  
    END  
  
  
  
      
  
       
  
  
  
  
  
  
  
  
  
    UPDATE #tempTable SET [USRID]=@UsrID,[MainAgentID]=(SELECT * FROM #tempXMLOUT)  
  
  
  
  
  
  
  
  
  
  
  
    
  
  
  
      SET @dynamicXML=' UPDATE T SET MainAgentMarkupType=T1.mrkupTypeid,MainAgentMrkupVal=T1.mrkupValue,[MainAgentMrkupCancellationVal]=t1.mrkupCancellationValue  
  
  
  
        FROM '+@server+'.dbo.tblMarkup AS T1 JOIN #tempTable AS T ON T1.mrkupusrId=T.USRID  
  
  
  
  and T1.mrkupSupplierid=T.SuplID  
  
  
  
  where mrkupserviceid='+cast(@SrvID as varchar(10))+''  
  
  
  
  
  
  
  
    
  
  
  
         EXEC (@dynamicXML)  
  
  
  
  
  
  
  
    
  
  
  
    SET @dynamicXML='UPDATE T SET SupplierBuffer=TS.BufferTime FROM #tempTable as T JOIN '+@server+'.dbo.tblSupplierDetails as TS on T.SuplID=TS.suplId'  
  
  
  
  
  
  
  
    SET @dynamicXML='UPDATE T SET [SubAgentCurrency]=TS.usrDtSpBaseCurrency ,[CurrencyCode]=TM.crncyCode FROM #tempTable as T JOIN '+@server+'.dbo.tblUserDetailSpecific as TS on T.USRID=TS.usrId  
  
  
  
  JOIN '+@server+'.dbo.tblCurrencyMaster as TM on TM.crncyId=TS.usrDtSpBaseCurrency'  
  
  
  
    
  
  
  
   SET @dynamicXML='UPDATE T SET SubAgentMrkupType=mrkupTypeid,SubAgentMrkupVal=mrkupValue,[SubAgentMrkupCancellationVal]=Mrkup.mrkupCancellationValue  
  
  
  
  FROM #tempTable AS T JOIN '+@server+'.dbo.tblMarkup AS Mrkup ON T.USRID=Mrkup.mrkupusrId  
  
  
  
  WHERE mrkupSupplierid=0 and mrkupserviceid='+cast(@SrvID as varchar(10))+''  
  
  
  
  
  
  
  
   exec(@dynamicXML)  
  
  
  
  
  
  
  
  
  
  
  
     IF ((select usrtype from #usrID) =12)  
  
  
  
    BEGIN  
  
  
  
     
  
  
  
   UPDATE #tempTable SET USRID=(select tempUSRID from #usrID )  
  
  
  
  
  
  
  
     SET @dynamicXML='UPDATE T SET [SubAgentCurrency]=TS.usrDtSpBaseCurrency ,[CurrencyCode]=TM.crncyCode FROM #tempTable as T JOIN '+@server+'.dbo.tblUserDetailSpecific as TS on @tempUSRID=TS.usrId  
  
  
  
      JOIN '+@server+'.dbo.tblCurrencyMaster as TM on TM.crncyId=TS.usrDtSpBaseCurrency'  
  
  
  
     
  
  
  
       exec(@dynamicXML)  
  
  
  
    
  
  
  
     SET @dynamicXML='UPDATE T SET [StaffUsrMrkupType]=mrkupTypeid,[StaffUsrMrkup]=mrkupValue,[StaffUsrCXLMrkup]=Mrkup.mrkupCancellationValue  
  
  
       FROM #tempTable AS T JOIN '+@server+'.dbo.tblMarkup AS Mrkup ON @tempUSRID=Mrkup.mrkupusrId  
  
  
  
      WHERE mrkupSupplierid=0 and  mrkupserviceid='+cast(@SrvID as varchar(10))+''  
  
  
  
     
  
  
  
       exec(@dynamicXML)  
  
  
  
  
  
  
  
    END  
  
  
  
  
  
  
  
  
  
  
  
  SELECT USRID,ISNULL([SubAgentCurrency],'') AS [SubAgentCurrency],ISNULL([CurrencyCode],'') AS [CurrencyCode],ISNULL(SuplID,0) AS SuplID,ISNULL(MainAgentMarkupType,0) AS MainAgentMarkupType,  
  
  
  
  ISNULL(MainAgentMrkupVal,0) AS MainAgentMrkupVal ,ISNULL([MainAgentMrkupCancellationVal],0) AS [MainAgentMrkupCancellationVal],  
  
  
  
  ISNULL(SubAgentMrkupType,0) AS SubAgentMrkupType,ISNULL(SubAgentMrkupVal,0) AS SubAgentMrkupVal,ISNULL([SubAgentMrkupCancellationVal],0) AS [SubAgentMrkupCancellationVal],ISNULL(MainAgentID,0) AS MainAgentID,  
  
  
  
  ISNULL([StaffUsrMrkupType],0) AS StaffUsrMrkupType,ISNULL([StaffUsrMrkup],0) AS StaffUsrMrkup,ISNULL([StaffUsrCXLMrkup],0) AS StaffUsrCXLMrkup,  
  
  
  
  ISNULL(SupplierBuffer,2) AS SupplierBuffer  
  
  
  
  FROM #tempTable  
  
  
  
  
  
  
  
    
  
  
  
END  
  
  
  
  
  
  
  
  
GO
/****** Object:  StoredProcedure [dbo].[USP_GeTXMLOUTMarkup_FLT]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Suraj Singh>
-- Create date: <SEP 03, 2019>
-- Mofify date: <SEP 03, 2019>
-- Description:	<Procedure to get markup for flights>
-- =============================================
CREATE PROC 
[dbo].[USP_GeTXMLOUTMarkup_FLT]  @UsrID BIGINT, @SupplierID VARCHAR(100), @MarkupType VARCHAR(50),@SrvID int=1
AS
--[USP_GeTXMLOUTMarkup]  10005,'','HA',1
BEGIN
   DECLARE @server varchar(100)
   DECLARE @MainAgentID BIGINT
   DECLARE @dynamicXML VARCHAR(MAX)
   CREATE TABLE #tempXMLOUT(MainAgentID BIGINT)	
   CREATE TABLE #usrID(UsrID BIGINT,tempUSRID BIGINT,partenid BIGINT,usrtype BIGINT)
   	Create Table #tempTable  (
	[USRID] [bigint] NULL,
	[SubAgentCurrency] [int] NULL,
	[CurrencyCode] VARCHAR(50) NULL, 
	[SuplID] [int] NULL,
	[MainAgentMarkupType] [int] NULL,
	[MainAgentMrkupVal] [float] NULL,
	[MainAgentMrkupCancellationVal] [float] NULL,
	[SubAgentMrkupType] [int] NULL,
	[SubAgentMrkupVal] [float] NULL,
	[SubAgentMrkupCancellationVal] [float] NULL,
	[MainAgentID] [bigint] NULL,
	SupplierBuffer [bigint] NULL,
	[StaffUsrMrkupType] [int] NULL,
	[StaffUsrMrkup] [float] NULL,
	[StaffUsrCXLMrkup] [float] NULL)
   INSERT INTO #usrID 
   SELECT @UsrID,0,0,0
   IF (@MarkupType='HA')
      Begin
    SET @server='TravayooHA'
   END
   ELSE
   BEGIN
   SET @server='TravayooQA'
   END
   SET @dynamicXML='update #usrID set partenid=usrparentid,usrtype=usrtypid from '+@server+'.dbo.tbluser as t where t.usrid in(select usrid from #usrID)'
   EXEC(@dynamicXML)
      IF ((select usrtype from #usrID) =12)
	  BEGIN
	  UPDATE #usrID set tempUSRID=UsrID,UsrID=partenid  
	  END
      SET @dynamicXML='INSERT INTO #tempXMLOUT(MainAgentID)	  
	   EXEC '+@server+'.dbo.[GetMainAgentID] '+cast(@UsrID as varchar(50))+''
	   EXEC (@dynamicXML)
	   IF (@SupplierID<>'')
	   BEGIN
	   INSERT INTO #tempTable(SuplID)
	   SELECT item from dbo.SplitString(@SupplierID, ',')	
	   END
	   ELSE
	   BEGIN
	    SET @dynamicXML='INSERT INTO #tempTable(SuplID) select SUPLID from '+@server+'.DBO.tblUsr_Supplier_Rel WHERE usrId='+cast(@UsrID as varchar(50))+''
        EXEC(@dynamicXML)
	   END
	   UPDATE #tempTable SET [USRID]=@UsrID,[MainAgentID]=(SELECT * FROM #tempXMLOUT)
      SET @dynamicXML=' UPDATE T SET MainAgentMarkupType=T1.mrkupTypeid,MainAgentMrkupVal=T1.mrkupValue,[MainAgentMrkupCancellationVal]=t1.mrkupCancellationValue
        FROM '+@server+'.dbo.tblMarkup AS T1 JOIN #tempTable AS T ON T1.mrkupusrId=T.USRID
		and T1.mrkupSupplierid=T.SuplID
		where mrkupserviceid='+cast(@SrvID as varchar(10))+''
         EXEC (@dynamicXML)
		  SET @dynamicXML='UPDATE T SET SupplierBuffer=TS.BufferTime FROM #tempTable as T JOIN '+@server+'.dbo.tblSupplierDetails as TS on T.SuplID=TS.suplId'
		  SET @dynamicXML='UPDATE T SET [SubAgentCurrency]=TS.usrDtSpBaseCurrency ,[CurrencyCode]=TM.crncyCode FROM #tempTable as T JOIN '+@server+'.dbo.tblUserDetailSpecific as TS on T.USRID=TS.usrId
		JOIN '+@server+'.dbo.tblCurrencyMaster as TM on TM.crncyId=TS.usrDtSpBaseCurrency'
		 SET @dynamicXML='UPDATE T SET SubAgentMrkupType=mrkupTypeid,SubAgentMrkupVal=mrkupValue,[SubAgentMrkupCancellationVal]=Mrkup.mrkupCancellationValue
		FROM #tempTable AS T JOIN '+@server+'.dbo.tblMarkup AS Mrkup ON T.USRID=Mrkup.mrkupusrId
		WHERE mrkupSupplierid=0 and mrkupserviceid='+cast(@SrvID as varchar(10))+''
		 exec(@dynamicXML)
		   IF ((select usrtype from #usrID) =12)
		  BEGIN
			UPDATE #tempTable SET USRID=(select tempUSRID from #usrID )
			  SET @dynamicXML='UPDATE T SET [SubAgentCurrency]=TS.usrDtSpBaseCurrency ,[CurrencyCode]=TM.crncyCode FROM #tempTable as T JOIN '+@server+'.dbo.tblUserDetailSpecific as TS on @tempUSRID=TS.usrId
		    JOIN '+@server+'.dbo.tblCurrencyMaster as TM on TM.crncyId=TS.usrDtSpBaseCurrency'
		     exec(@dynamicXML)
			  SET @dynamicXML='UPDATE T SET [StaffUsrMrkupType]=mrkupTypeid,[StaffUsrMrkup]=mrkupValue,[StaffUsrCXLMrkup]=Mrkup.mrkupCancellationValue
	     	FROM #tempTable AS T JOIN '+@server+'.dbo.tblMarkup AS Mrkup ON @tempUSRID=Mrkup.mrkupusrId
		    WHERE mrkupSupplierid=0 and  mrkupserviceid='+cast(@SrvID as varchar(10))+''
		     exec(@dynamicXML)
		  END
		SELECT USRID,ISNULL([SubAgentCurrency],'') AS [SubAgentCurrency],ISNULL([CurrencyCode],'') AS [CurrencyCode],ISNULL(SuplID,0) AS SuplID,ISNULL(MainAgentMarkupType,0) AS MainAgentMarkupType,
		ISNULL(MainAgentMrkupVal,0) AS MainAgentMrkupVal ,ISNULL([MainAgentMrkupCancellationVal],0) AS [MainAgentMrkupCancellationVal],
		ISNULL(SubAgentMrkupType,0) AS SubAgentMrkupType,ISNULL(SubAgentMrkupVal,0) AS SubAgentMrkupVal,ISNULL([SubAgentMrkupCancellationVal],0) AS [SubAgentMrkupCancellationVal],ISNULL(MainAgentID,0) AS MainAgentID,
		ISNULL([StaffUsrMrkupType],0) AS StaffUsrMrkupType,ISNULL([StaffUsrMrkup],0) AS StaffUsrMrkup,ISNULL([StaffUsrCXLMrkup],0) AS StaffUsrCXLMrkup,
		ISNULL(SupplierBuffer,2) AS SupplierBuffer
		FROM #tempTable
END








GO
/****** Object:  StoredProcedure [dbo].[USP_hotelcode_IOL]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================     
-- Author:  <Suraj Singh>     
-- Create date: <Feb 08, 2024>    
-- Mofify date: <Feb 08, 2024>     
-- Description: <Procedure to get all hotelcodes>     
-- =============================================      
CREATE PROCEDURE [dbo].[USP_hotelcode_IOL]      
(        
@retVal bit OUT      
)       
AS       
BEGIN      
        SET NOCOUNT ON;    
  SET XACT_ABORT ON       
  BEGIN TRY       
   Begin      
   select iwtx_code as hotelcode from [StaticData].[dbo].[tbliol_hotellist_130224] with(nolock) where 1=1     
   set @retval=1     
   End     
  END TRY     
 BEGIN CATCH      
 END CATCH     
END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_bookDarina]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================    
-- Author:  <Suraj Singh>    
-- Create date: <April 27, 2017>    
-- Mofify date: <APR 10, 2018>    
-- Description: <Procedure to insert api logs>    
    
-- =============================================    
    
CREATE PROCEDURE [dbo].[USP_InsertAPILog_bookDarina]    
 (    
  @customerID bigint=null,    
  @TrackNumber nvarchar(max)=null,    
  @logTypeID bigint=null,    
  @logType nvarchar(max)=null,    
  @SupplierID bigint=null,    
  @logMsg nvarchar(max)=null,    
  @logrequestXML nvarchar(max)=null,    
  @logresponseXML nvarchar(max)=null,    
  @logStatus tinyint=null,    
  @StartTime datetime2(7)=null,    
  @EndTime datetime2(7)=null,    
  @retVal bit OUT    
 )    
 AS    
 BEGIN    
  if(@StartTime is null)    
   BEGIN    
    select @StartTime=getDate()    
   END    
  if(@EndTime is null)    
   BEGIN    
    select @EndTime=getDate()    
   END    
  SET NOCOUNT ON;    
  SET XACT_ABORT ON    
  BEGIN TRANSACTION    
  BEGIN TRY    
    declare @ip nvarchar(100)=null;    
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));    
    Insert into tblapilog_bookDarina (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip)    
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip)    
    set @retval=1    
    COMMIT TRANSACTION    
    SET NOCOUNT OFF;    
  END TRY    
  BEGIN CATCH    
      
         declare @error nvarchar(2000), @errNo nvarchar(2000)    
      set @errNo= ERROR_NUMBER()    
               set @error = error_message()    
      print @error    
      print @errNo    
    IF (@@TRANCOUNT>0)    
    BEGIN      
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)    
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)    
     set @retval=0    
     --ROLLBACK TRANSACTION    
    END    
  END CATCH    
 END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_bookHB]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Author:  <Suraj Singh>  
-- Create date: <April 27, 2017>  
-- Mofify date: <APR 10, 2018>  
-- Description: <Procedure to insert api logs>  
  
-- =============================================  
  
CREATE PROCEDURE [dbo].[USP_InsertAPILog_bookHB]  
 (  
  @customerID bigint=null,  
  @TrackNumber nvarchar(max)=null,  
  @logTypeID bigint=null,  
  @logType nvarchar(max)=null,  
  @SupplierID bigint=null,  
  @logMsg nvarchar(max)=null,  
  @logrequestXML nvarchar(max)=null,  
  @logresponseXML nvarchar(max)=null,  
  @logStatus tinyint=null,  
  @StartTime datetime2(7)=null,  
  @EndTime datetime2(7)=null,  
  @retVal bit OUT  
 )  
 AS  
 BEGIN  
  if(@StartTime is null)  
   BEGIN  
    select @StartTime=getDate()  
   END  
  if(@EndTime is null)  
   BEGIN  
    select @EndTime=getDate()  
   END  
  SET NOCOUNT ON;  
  SET XACT_ABORT ON  
  BEGIN TRANSACTION  
  BEGIN TRY  
    declare @ip nvarchar(100)=null;  
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));  
    Insert into tblapilog_bookHB (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip)  
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip)  
    set @retval=1  
    COMMIT TRANSACTION  
    SET NOCOUNT OFF;  
  END TRY  
  BEGIN CATCH  
    
         declare @error nvarchar(2000), @errNo nvarchar(2000)  
      set @errNo= ERROR_NUMBER()  
               set @error = error_message()  
      print @error  
      print @errNo  
    IF (@@TRANCOUNT>0)  
    BEGIN    
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)  
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)  
     set @retval=0  
     --ROLLBACK TRANSACTION  
    END  
  END CATCH  
 END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_room]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================        
-- Author:  <Suraj Singh>        
-- Create date: <April 27, 2017>        
-- Mofify date: <AUG 2023>        
-- Description: <Procedure to insert api logs>        
        
-- =============================================        
        
CREATE PROCEDURE [dbo].[USP_InsertAPILog_room]        
 (        
  @customerID bigint=null,        
  @TrackNumber nvarchar(max)=null,        
  @logTypeID bigint=null,        
  @logType nvarchar(max)=null,        
  @SupplierID bigint=null,        
  @logMsg nvarchar(max)=null,        
  @logrequestXML nvarchar(max)=null,        
  @logresponseXML nvarchar(max)=null,        
  @logStatus tinyint=null,        
  @StartTime datetime2(7)=null,        
  @EndTime datetime2(7)=null,   
  @HotelId nvarchar(500)=null,
  @retVal bit OUT        
 )        
 AS        
 BEGIN        
  if(@StartTime is null)        
   BEGIN        
    select @StartTime=getDate()        
   END        
  if(@EndTime is null)        
   BEGIN        
    select @EndTime=getDate()        
   END        
  SET NOCOUNT ON;        
  SET XACT_ABORT ON        
  BEGIN TRANSACTION        
  BEGIN TRY        
    declare @ip nvarchar(100)=null;        
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));        
    Insert into tblapilog_room (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip,HotelId)        
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip,@HotelId)        
    set @retval=1        
    COMMIT TRANSACTION        
    SET NOCOUNT OFF;        
  END TRY        
  BEGIN CATCH        
          
         declare @error nvarchar(2000), @errNo nvarchar(2000)        
      set @errNo= ERROR_NUMBER()        
               set @error = error_message()        
      print @error        
      print @errNo        
    IF (@@TRANCOUNT>0)        
    BEGIN          
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)        
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)        
     set @retval=0        
     --ROLLBACK TRANSACTION        
    END        
  END CATCH        
 END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_room_iol]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================            
-- Author:  <Suraj Singh>            
-- Create date: <Feb 06, 2024>            
-- Mofify date: <Feb 07, 2024>            
-- Description: <Procedure to insert api logs for IOL>            
            
-- =============================================            
            
CREATE PROCEDURE [dbo].[USP_InsertAPILog_room_iol]            
 (            
  @customerID bigint=null,            
  @TrackNumber nvarchar(max)=null,            
  @logTypeID bigint=null,            
  @logType nvarchar(max)=null,            
  @SupplierID bigint=null,            
  @logMsg nvarchar(max)=null,            
  @logrequestXML nvarchar(max)=null,            
  @logresponseXML nvarchar(max)=null,            
  @logStatus tinyint=null,            
  @StartTime datetime2(7)=null,            
  @EndTime datetime2(7)=null,       
  @preID nvarchar(500)=null,    
  @retVal bit OUT            
 )            
 AS            
 BEGIN            
  if(@StartTime is null)            
   BEGIN            
    select @StartTime=getDate()            
   END            
  if(@EndTime is null)            
   BEGIN            
    select @EndTime=getDate()            
   END            
  SET NOCOUNT ON;            
  SET XACT_ABORT ON            
  BEGIN TRANSACTION            
  BEGIN TRY            
    declare @ip nvarchar(100)=null;            
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));            
    Insert into tblapilog_room_iol (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip,preID)            
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip,@preID)            
    set @retval=1            
    COMMIT TRANSACTION            
    SET NOCOUNT OFF;            
  END TRY            
  BEGIN CATCH            
              
         declare @error nvarchar(2000), @errNo nvarchar(2000)            
      set @errNo= ERROR_NUMBER()            
               set @error = error_message()            
      print @error            
      print @errNo            
    IF (@@TRANCOUNT>0)            
    BEGIN       
	ROLLBACK TRANSACTION  
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)            
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)            
     set @retval=0            
     --ROLLBACK TRANSACTION            
    END            
  END CATCH            
 END  
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_search]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================    
-- Author:  <Suraj Singh>    
-- Create date: <April 27, 2017>    
-- Mofify date: <APR 10, 2018>    
-- Description: <Procedure to insert api logs>    
    
-- =============================================    
    
CREATE PROCEDURE [dbo].[USP_InsertAPILog_search]    
 (    
  @customerID bigint=null,    
  @TrackNumber nvarchar(max)=null,    
  @logTypeID bigint=null,    
  @logType nvarchar(max)=null,    
  @SupplierID bigint=null,    
  @logMsg nvarchar(max)=null,    
  @logrequestXML nvarchar(max)=null,    
  @logresponseXML nvarchar(max)=null,    
  @logStatus tinyint=null,    
  @StartTime datetime2(7)=null,    
  @EndTime datetime2(7)=null,    
  @retVal bit OUT    
 )    
 AS    
 BEGIN    
  if(@StartTime is null)    
   BEGIN    
    select @StartTime=getDate()    
   END    
  if(@EndTime is null)    
   BEGIN    
    select @EndTime=getDate()    
   END    
  SET NOCOUNT ON;    
  SET XACT_ABORT ON    
  BEGIN TRANSACTION    
  BEGIN TRY    
    declare @ip nvarchar(100)=null;    
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));    
    Insert into tblapilog_search (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip)    
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip)    
    set @retval=1    
    COMMIT TRANSACTION    
    SET NOCOUNT OFF;    
  END TRY    
  BEGIN CATCH    
      
         declare @error nvarchar(2000), @errNo nvarchar(2000)    
      set @errNo= ERROR_NUMBER()    
               set @error = error_message()    
      print @error    
      print @errNo    
    IF (@@TRANCOUNT>0)    
    BEGIN      
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)    
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)    
     set @retval=0    
     ROLLBACK TRANSACTION    
    END    
  END CATCH    
 END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_search_iol]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================      
-- Author:  <Suraj Singh>      
-- Create date: <Feb 06, 2024>      
-- Mofify date: <Feb 06 2024>      
-- Description: <Procedure to insert api logs for IOL>      
      
-- =============================================      
      
CREATE PROCEDURE [dbo].[USP_InsertAPILog_search_iol]      
 (      
  @customerID bigint=null,      
  @TrackNumber nvarchar(max)=null,      
  @logTypeID bigint=null,      
  @logType nvarchar(max)=null,      
  @SupplierID bigint=null,      
  @logMsg nvarchar(max)=null,      
  @logrequestXML nvarchar(max)=null,      
  @logresponseXML nvarchar(max)=null,      
  @logStatus tinyint=null,      
  @StartTime datetime2(7)=null,      
  @EndTime datetime2(7)=null, 
  @preID nvarchar(500)=null, 
  @retVal bit OUT      
 )      
 AS      
 BEGIN      
  if(@StartTime is null)      
   BEGIN      
    select @StartTime=getDate()      
   END      
  if(@EndTime is null)      
   BEGIN      
    select @EndTime=getDate()      
   END      
  SET NOCOUNT ON;      
  SET XACT_ABORT ON      
  BEGIN TRANSACTION      
  BEGIN TRY      
    declare @ip nvarchar(100)=null;      
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));      
    Insert into tblapilog_search_iol (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip,preID)      
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip,@preID)      
    set @retval=1      
    COMMIT TRANSACTION      
    SET NOCOUNT OFF;      
  END TRY      
  BEGIN CATCH      
        
         declare @error nvarchar(2000), @errNo nvarchar(2000)      
      set @errNo= ERROR_NUMBER()      
               set @error = error_message()      
      print @error      
      print @errNo      
    IF (@@TRANCOUNT>0)      
    BEGIN        
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)      
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)      
     set @retval=0      
     ROLLBACK TRANSACTION      
    END      
  END CATCH      
 END

GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_searchDarina]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================    
-- Author:  <Suraj Singh>    
-- Create date: <April 27, 2017>    
-- Mofify date: <APR 10, 2018>    
-- Description: <Procedure to insert api logs>    
    
-- =============================================    
    
CREATE PROCEDURE [dbo].[USP_InsertAPILog_searchDarina]    
 (    
  @customerID bigint=null,    
  @TrackNumber nvarchar(max)=null,    
  @logTypeID bigint=null,    
  @logType nvarchar(max)=null,    
  @SupplierID bigint=null,    
  @logMsg nvarchar(max)=null,    
  @logrequestXML nvarchar(max)=null,    
  @logresponseXML nvarchar(max)=null,    
  @logStatus tinyint=null,    
  @StartTime datetime2(7)=null,    
  @EndTime datetime2(7)=null,    
  @retVal bit OUT    
 )    
 AS    
 BEGIN    
  if(@StartTime is null)    
   BEGIN    
    select @StartTime=getDate()    
   END    
  if(@EndTime is null)    
   BEGIN    
    select @EndTime=getDate()    
   END    
  SET NOCOUNT ON;    
  SET XACT_ABORT ON    
  BEGIN TRANSACTION    
  BEGIN TRY    
    declare @ip nvarchar(100)=null;    
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));    
    Insert into tblapilog_searchDarina (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip)    
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip)    
    set @retval=1    
    COMMIT TRANSACTION    
    SET NOCOUNT OFF;    
  END TRY    
  BEGIN CATCH    
      
         declare @error nvarchar(2000), @errNo nvarchar(2000)    
      set @errNo= ERROR_NUMBER()    
               set @error = error_message()    
      print @error    
      print @errNo    
    IF (@@TRANCOUNT>0)    
    BEGIN      
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)    
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)    
     set @retval=0    
     --ROLLBACK TRANSACTION    
    END    
  END CATCH    
 END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILog_searchHB]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Author:  <Suraj Singh>  
-- Create date: <April 27, 2017>  
-- Mofify date: <APR 10, 2018>  
-- Description: <Procedure to insert api logs>  
  
-- =============================================  
  
CREATE PROCEDURE [dbo].[USP_InsertAPILog_searchHB]  
 (  
  @customerID bigint=null,  
  @TrackNumber nvarchar(max)=null,  
  @logTypeID bigint=null,  
  @logType nvarchar(max)=null,  
  @SupplierID bigint=null,  
  @logMsg nvarchar(max)=null,  
  @logrequestXML nvarchar(max)=null,  
  @logresponseXML nvarchar(max)=null,  
  @logStatus tinyint=null,  
  @StartTime datetime2(7)=null,  
  @EndTime datetime2(7)=null,  
  @retVal bit OUT  
 )  
 AS  
 BEGIN  
  if(@StartTime is null)  
   BEGIN  
    select @StartTime=getDate()  
   END  
  if(@EndTime is null)  
   BEGIN  
    select @EndTime=getDate()  
   END  
  SET NOCOUNT ON;  
  SET XACT_ABORT ON  
  BEGIN TRANSACTION  
  BEGIN TRY  
    declare @ip nvarchar(100)=null;  
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));  
    Insert into tblapilog_searchHB (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip)  
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip)  
    set @retval=1  
    COMMIT TRANSACTION  
    SET NOCOUNT OFF;  
  END TRY  
  BEGIN CATCH  
    
         declare @error nvarchar(2000), @errNo nvarchar(2000)  
      set @errNo= ERROR_NUMBER()  
               set @error = error_message()  
      print @error  
      print @errNo  
    IF (@@TRANCOUNT>0)  
    BEGIN    
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)  
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)  
     set @retval=0  
     --ROLLBACK TRANSACTION  
    END  
  END CATCH  
 END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertAPILogout]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
  
-- Author:  <Suraj Singh>  
  
-- Create date: <Jan 01, 2023>  
  
-- Mofify date: <Feb 10, 2023>  
  
-- Description: <Procedure to insert api logs>  
  
-- =============================================  
  
CREATE PROCEDURE [dbo].[USP_InsertAPILogout]  
  
 (  
  
  @customerID bigint=null,  
  
  @TrackNumber nvarchar(max)=null,  
  
  @logTypeID bigint=null,  
  
  @logType nvarchar(max)=null,  
  
  @SupplierID bigint=null,  
  
  @logMsg nvarchar(max)=null,  
  
  @logrequestXML nvarchar(max)=null,  
  
  @logresponseXML nvarchar(max)=null,  
  
  @logStatus tinyint=null,  
  
  @StartTime datetime2(7)=null,  
  
  @EndTime datetime2(7)=null,  
  @preID varchar(500)=null,  
  @retVal bit OUT  
  
  
 )  
  
 AS  
  
 BEGIN  
  
  if(@StartTime is null)  
  
   BEGIN  
  
    select @StartTime=getDate()  
  
   END  
  
  if(@EndTime is null)  
  
   BEGIN  
  
    select @EndTime=getDate()  
  
   END  
  
  SET NOCOUNT ON;  
  
  SET XACT_ABORT ON  
  
  BEGIN TRANSACTION  
  
  BEGIN TRY  
  
    declare @ip nvarchar(100)=null;  
  
    set @ip = (SELECT CONVERT(char(15), CONNECTIONPROPERTY('client_net_address')));  
  
    Insert into tblapilogOut(customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy,ip,preID)  
  
    values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID,@ip,@preID)  
  
    set @retval=1  
  
    COMMIT TRANSACTION  
  
    SET NOCOUNT OFF;  
  
  END TRY  
  
  BEGIN CATCH  
  
    IF (@@TRANCOUNT>0)  
  
    BEGIN    
  
     Insert into tblapilogFailTrans (customerID,TrackNumber,logTypeID,logType,SupplierID,logMsg,logrequestXML,logresponseXML,logStatus,logcreatedOn,logcreatedBy,logmodifyOn,logmodifyBy)  
  
     values(@customerID,@TrackNumber,@logTypeID,@logType,@SupplierID,@logMsg,@logrequestXML,@logresponseXML,@logStatus,@StartTime,@customerID,@EndTime,@customerID)  
  
     set @retval=0  
  
     --ROLLBACK TRANSACTION  
  
    END  
  
  END CATCH  
  
 END
GO
/****** Object:  StoredProcedure [dbo].[USP_Insertiol_Hotelstatic]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================                  
                  
-- Author:  <Suraj Singh>                  
                  
-- Create date: <Feb 08, 2024>                  
                  
-- Mofify date: <Feb 13, 2024>                  
                  
-- Description: <Procedure to insert IOL Hotels>                  
                  
-- =============================================                  
                  
CREATE PROCEDURE [dbo].[USP_Insertiol_Hotelstatic]                  
                  
(                  
                  
@HotelCode nvarchar(max)=null,                  
@HotelName nvarchar(max)=null,              
@HotelChain nvarchar(max)=null,              
@HotelStarRating nvarchar(max)=null,              
@HotelPhone nvarchar(max)=null,         
@HotelPostCode nvarchar(max)=null,         
@HotelWeb nvarchar(max)=null,              
@CityCode nvarchar(max)=null,              
@CountryCode nvarchar(max)=null,              
@HotelCity nvarchar(max)=null,              
@HotelAddress nvarchar(max)=null,              
   @Description nvarchar(max)=null,              
   @CheckInTime nvarchar(max)=null,              
   @CheckOutTime nvarchar(max)=null,              
   @Latitude nvarchar(max)=null,              
   @Longitude nvarchar(max)=null,              
   @Image nvarchar(max)=null,                
@retVal bit OUT                 
)                  
AS                 
BEGIN                 
  SET NOCOUNT ON;              
  SET XACT_ABORT ON             
              
  BEGIN TRY            
            
  BEGIN TRANSACTION                
                    
  if not exists(select 1 from [StaticData].[dbo].[tbliol_hotellist_130224] with(nolock) where iwtx_code=@HotelCode and city_code=@CityCode and country_code=@CountryCode)                
  begin                
    Insert into [StaticData].[dbo].[tbliol_hotellist_130224](iwtx_code, city_code,city_name,country_code,name,chain_name,hotel_name,latitude,longitude,address,post_code,star_rating,property_type,        
 phone,website,image,checkintime,checkouttime,description)                   
                  
    values(@HotelCode, @CityCode, @HotelCity,@CountryCode,'',@HotelChain,@HotelName,@Latitude,@Longitude,@HotelAddress,@HotelPostCode,@HotelStarRating,'', @HotelPhone,   @HotelWeb,  @Image,@CheckInTime,        
 @CheckOutTime,@Description)                  
                  
       set @retval=1             
  End        
  else      
  update [StaticData].[dbo].[tbliol_hotellist_130224] set address=@HotelAddress,post_code=@HotelPostCode,phone=@HotelPhone,website=@HotelWeb,
  image=@image,checkintime=@checkintime,checkouttime=@CheckOutTime,description=@Description      
  where iwtx_code=@HotelCode and city_code=@CityCode and country_code=@CountryCode      
  COMMIT TRANSACTION             
  END TRY              
 BEGIN CATCH              
  IF (@@TRANCOUNT>0)              
  BEGIN                 
     set @retval=0               
     --ROLLBACK TRANSACTION               
  END              
 END CATCH                
END    
GO
/****** Object:  StoredProcedure [dbo].[USP_Insertwte_Hotelstatic]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================        
        
-- Author:  <Suraj Singh>        
        
-- Create date: <Jul 11, 2023>        
        
-- Mofify date: <Jul 11, 2023>        
        
-- Description: <Procedure to insert WTE Hotels>        
        
-- =============================================        
        
CREATE PROCEDURE [dbo].[USP_Insertwte_Hotelstatic]        
        
(        
        
@HotelId nvarchar(max)=null,        
@HotelName nvarchar(max)=null,    
@Description nvarchar(max)=null,    
@Latitude nvarchar(max)=null,    
@Longitude nvarchar(max)=null,    
@Address nvarchar(max)=null,    
@Rating nvarchar(max)=null,    
@CountryId nvarchar(max)=null,    
@CountryName nvarchar(max)=null,    
@CityId nvarchar(max)=null,    
   @CityName nvarchar(max)=null,    
   @HotelFrontImage nvarchar(max)=null,    
   @IsRecomondedHotel nvarchar(max)=null,    
   @IsActive nvarchar(max)=null,    
   @UpdatedDate nvarchar(max)=null,    
   @GiataId nvarchar(max)=null,      
@retVal bit OUT       
)        
AS       
BEGIN       
  SET NOCOUNT ON;    
  SET XACT_ABORT ON   
    
  BEGIN TRY  
  
  BEGIN TRANSACTION      
          
  if not exists(select 1 from [StaticData].[dbo].[tbl_wteHotelList_master] with(nolock) where HotelId=@HotelId and CityId=@CityId and CountryId=@CountryId)      
  begin      
    Insert into [StaticData].[dbo].[tbl_wteHotelList_master](HotelId, HotelName, Description,Latitude,Longitude,Address,Rating,CountryId,CountryName,CityId    
    ,CityName,HotelFrontImage,IsRecomondedHotel,IsActive,UpdatedDate,GiataId,Created_Date)         
        
    values(@HotelId, @HotelName, @Description,@Latitude,@Longitude,@Address,@Rating,@CountryId,@CountryName,@CityId    
    ,@CityName,@HotelFrontImage,@IsRecomondedHotel,@IsActive,@UpdatedDate,@GiataId,getdate())        
        
       set @retval=1   
  End   
  COMMIT TRANSACTION   
  END TRY    
 BEGIN CATCH    
  IF (@@TRANCOUNT>0)    
  BEGIN       
     set @retval=0     
     --ROLLBACK TRANSACTION     
  END    
 END CATCH      
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Inxed_Statistics_Maintenance]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Inxed_Statistics_Maintenance]
@DBName AS NVARCHAR(128)
AS

DECLARE @ERRORE INT
--Check Database Error
DBCC CHECKDB WITH NO_INFOMSGS
SET @ERRORE = @@ERROR
IF @ERRORE = 0 
BEGIN
	DECLARE @RC INT
	DECLARE @Messaggio VARCHAR(MAX)
	DECLARE @Rebild AS VARCHAR(MAX)
	DECLARE @Reorganize AS VARCHAR(MAX)

	SET @Reorganize = ''
	SET @Rebild = ''

	SELECT  @Reorganize = @Reorganize + ' ' + 
	'ALTER INDEX [' + i.[name] + '] ON [dbo].[' + t.[name] + '] 
			REORGANIZE WITH ( LOB_COMPACTION = ON )'
	FROM	sys.dm_db_index_physical_stats
			  (DB_ID(@DBName ), NULL, NULL, NULL , 'DETAILED') fi
			inner join sys.tables t
			 on fi.[object_id] = t.[object_id]
			inner join sys.indexes i
			 on fi.[object_id] = i.[object_id] and
				fi.index_id = i.index_id
	where t.[name] is not null and i.[name] is not null 
			and avg_fragmentation_in_percent > 10   
			and avg_fragmentation_in_percent <=35
	order by t.[name]

	EXEC (@Reorganize)

	SELECT  @Rebild = @Rebild + ' ' + 
	'ALTER INDEX [' + i.[name] + '] ON [dbo].[' + t.[name] + '] 
			REBUILD WITH (ONLINE = OFF )'
	FROM	sys.dm_db_index_physical_stats
			  (DB_ID(@DBName ), NULL, NULL, NULL , 'DETAILED') fi
			inner join sys.tables t
			 on fi.[object_id] = t.[object_id]
			inner join sys.indexes i
			 on fi.[object_id] = i.[object_id] and
				fi.index_id = i.index_id
	where avg_fragmentation_in_percent > 35 and t.[name] is not null and i.[name] is not null
	order by t.[name]

	EXEC (@Rebild)
END

-- if there are not error update statistics
SET @ERRORE = @@ERROR
IF @ERRORE = 0
	BEGIN
		EXEC sp_updatestats
	END
GO
/****** Object:  StoredProcedure [dbo].[uspGetExpedia_HotelList]    Script Date: 5/23/2024 9:06:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================

-- Author:		<Rajiv Kumar>
-- Create date: <May 15,2020>
-- Description:	<Procedure to get hotel static data for a particular city>

-- =============================================

--exec uspGetExpedia_HotelList '','','London','GB','3','5'
CREATE PROCEDURE [dbo].[uspGetExpedia_HotelList]

@HotelCode nvarchar(150)=null,
@HotelName nvarchar(200)=null,
@citycode nvarchar(150)=null,
@countrycode nvarchar(10)=null,
@MinStarRating nvarchar(100)=null,
@MaxStarRating nvarchar(100)=null

AS

BEGIN

   	BEGIN TRY

			if LEN(ISNULL(@HotelCode,''))=0  
			 set @HotelCode=null  
			   
			if LEN(ISNULL(@HotelName,''))=0     
			 set @HotelName=null     
		

				if(@HotelCode is null  )
				begin
				select property_id as hotelcode,name as hotelname,rating,(address1 +',' + address2) as address,latitude,longitude, image, country_code from  [StaticData].[dbo].[tblExpediaHotels]  with(nolock) where city=@citycode and country_code=@countrycode and  CAST (rating as decimal(2,1))>=@MinStarRating AND CAST (rating as decimal(2))<=@MaxStarRating	

				
				end
				else  --Search By Giata HotelCode
				begin
				-- supplierid against giata id for hotel
	           select hotelid into #temp2 from [StaticData]..[tblgiatadetails] with (nolock) where giataid=@HotelCode and localsupid=20
			   declare @count int =(select count(*) from #temp2)
	           if(@count >0)
	            begin
				   select property_id as hotelcode,name as hotelname,rating,(address1 +',' + address2) as address,latitude,longitude, image, country_code from  [StaticData].[dbo].[tblExpediaHotels]  with(nolock) where city=@citycode and country_code=@countrycode	
				   	and property_id in(select hotelid from #temp2)
				end
				else if(@HotelName is not null)  --Search By Giata HotelName if Supplier Hotel Id not found in Giata
				begin
			    	select property_id as hotelcode,name as hotelname,rating,(address1 +',' + address2) as address,latitude,longitude, image, country_code from  [StaticData].[dbo].[tblExpediaHotels]  with(nolock) where city=@citycode and country_code=@countrycode	
					and name like '%'+@HotelName+'%'
				end

			end


    END TRY

	BEGIN CATCH

	       select null

     END CATCH

END
GO
USE [master]
GO
ALTER DATABASE [BE_ServiceAPI] SET  READ_WRITE 
GO
