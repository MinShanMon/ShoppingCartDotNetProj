
CREATE DATABASE ShoppingCartDB
GO

USE ShoppingCartDB
GO

/*UserId will be auto incremented (IDENTITY keyword) when a user record is inserted. 
Please insert only username and password to this table*/
CREATE TABLE [dbo].[Users] (
	[UserId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Username] VARCHAR (100) NOT NULL,
	[Password] VARCHAR (100) NOT NULL,
);

CREATE TABLE [dbo].[Sessions] (
	[SessionId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[UserId] INT FOREIGN KEY REFERENCES Users(UserId)
)

/*ProductId will be auto incremented (IDENTITY keyword) when a product record is inserted. 
Please insert only name, desc, imgurl, price to this table*/
CREATE TABLE [dbo].[Products] (
	[ProductId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[ProductName] VARCHAR (100) NOT NULL,
	[ProductDesc] VARCHAR (300) NOT NULL,
	[ProductImg] VARCHAR (100) NOT NULL,
	[ProductPrice] DEC (6, 2) NOT NULL
);


/*OrderId will be auto incremented (IDENTITY keyword) when a product record is inserted. 
Please insert only userid, timestamp to this table*/
CREATE TABLE [dbo].[Orders] (
	[OrderId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
	[Timestamp] DATE NOT NULL
);

CREATE TABLE [dbo].[OrderDetails] (
	[OrderId] INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderId),
	[ProductId] INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
	[Quantity] INT NOT NULL,
	CONSTRAINT PK_OrderDetail PRIMARY KEY (OrderId, ProductId)
);

CREATE TABLE [dbo].[ActivationCodeDetails] (
	[ActivationCode] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ProductId] INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
	[OrderId] INT NULL FOREIGN KEY REFERENCES Orders(OrderId)
);

CREATE TABLE [dbo].[Reviews] (
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
	[ProductId] INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
	[Rating] INT NOT NULL,
	CONSTRAINT PK_Review PRIMARY KEY (UserId, ProductId)
);