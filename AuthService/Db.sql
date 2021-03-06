﻿USE [master]
GO
/****** Object:  Database [Auth]    Script Date: 24/05/2020 23:33:29 ******/
CREATE DATABASE [Auth]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Auth', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\Auth.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Auth_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\Auth_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Auth] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Auth].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Auth] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Auth] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Auth] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Auth] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Auth] SET ARITHABORT OFF 
GO
ALTER DATABASE [Auth] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Auth] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Auth] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Auth] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Auth] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Auth] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Auth] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Auth] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Auth] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Auth] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Auth] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Auth] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Auth] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Auth] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Auth] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Auth] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Auth] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Auth] SET RECOVERY FULL 
GO
ALTER DATABASE [Auth] SET  MULTI_USER 
GO
ALTER DATABASE [Auth] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Auth] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Auth] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Auth] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Auth] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Auth', N'ON'
GO
ALTER DATABASE [Auth] SET QUERY_STORE = OFF
GO
USE [Auth]
GO
/****** Object:  Table [dbo].[Application]    Script Date: 24/05/2020 23:33:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Application](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[AppId] [uniqueidentifier] NOT NULL,
	[AppSecret] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK__Applicat__3214EC0718795AFA] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Applicat__737584F68A0E426F] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Application_Id] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Secret]    Script Date: 24/05/2020 23:33:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Secret](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[Token] [nvarchar](1024) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ExpiryOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Secret_ApplicationId] UNIQUE NONCLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SecretKey]    Script Date: 24/05/2020 23:33:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SecretKey](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](64) NOT NULL,
	[IV] [nvarchar](32) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Application] ADD  CONSTRAINT [DF__Applicati__AppId__44FF419A]  DEFAULT (newid()) FOR [AppId]
GO
ALTER TABLE [dbo].[Application] ADD  DEFAULT (newid()) FOR [AppSecret]
GO
ALTER TABLE [dbo].[Secret]  WITH CHECK ADD  CONSTRAINT [FK__Secret__Applicat__3A81B327] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([Id])
GO
ALTER TABLE [dbo].[Secret] CHECK CONSTRAINT [FK__Secret__Applicat__3A81B327]
GO
USE [master]
GO
ALTER DATABASE [Auth] SET  READ_WRITE 
GO
