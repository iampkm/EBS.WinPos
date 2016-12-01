-- 商品
DROP TABLE Product;

CREATE TABLE Product (
    Id            INTEGER    NOT NULL  PRIMARY KEY ,
    Code          VARCHAR (20),
    Name          VARCHAR (50),
    BarCode       VARCHAR (50),
    Specification VARCHAR (200),
    Unit               varchar(50),
    SalePrice     DECIMAL (8,2),
    UpdatedOn     DATETIME
);

CREATE UNIQUE INDEX idx_product_code ON Product (
    Code
);
CREATE INDEX idx_product_barcode ON Product (
    BarCode
);

-- 订单
DROP TABLE SaleOrder;
create table SaleOrder
(
   Id                   INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
   Code                 varchar(20),
   StoreId              INTEGER ,
   PosId,               INTEGER,
   OrderType            INTEGER,
   OrderAmount          decimal(8,2),
   PayAmount            decimal(8,2),
   OnlinePayAmount      decimal(8,2),
   PaymentWay           INTEGER,
   PaidDate             datetime , 
   RefundAccount        varchar(50),
   Status               INTEGER ,
   CreatedOn            datetime ,
   CreatedBy            INTEGER,
   UpdatedOn            datetime,
   UpdatedBy            INTEGER,
   IsSync		INTEGER
);
CREATE UNIQUE INDEX idx_SaleOrder_code ON SaleOrder (
    Code
);

/*==============================================================*/
/* Table: SaleOrderItem                                         */
/*==============================================================*/
DROP TABLE SaleOrderItem;
create table SaleOrderItem
(
   Id                   INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
   SaleOrderId          INTEGER ,
   ProductId            INTEGER ,
   ProductCode          varchar(20),
   ProductName          varchar(50),
   SalePrice            decimal(8,2),
   RealPrice            DECIMAL (8, 2),
   Quantity             INTEGER
);

CREATE INDEX idx_SaleOrderItem_SaleOrderId ON SaleOrderItem (
    SaleOrderId
);

drop table WorkSchedule;
create table WorkSchedule
(
   Id                   INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
   StoreId              INTEGER , 
   PosId                INTEGER , 
   CashAmount           decimal(8,2),
   CreatedBy            INTEGER,
   CreatedByName        varchar(50),
   StartDate            datetime,
   EndDate              datetime,
   EndBy                INTEGER,
   EndByName            varchar(50),
   IsSync		INTEGER,
   IsSyncAmount         INTEGER
);

create table Store
(
    Id                   INTEGER  NOT NULL  PRIMARY KEY,
    Code                 varchar(20),
    Name                 varchar(50),
    LicenseCode         varchar(50)
)


CREATE TABLE Account (
    Id       INTEGER      PRIMARY KEY,
    UserName VARCHAR (50) UNIQUE   NOT NULL,
    Password VARCHAR (50) NOT NULL,
    StoreId  INTEGER,
    Status   INTEGER,
    RoleId   INTEGER,
    NickName VARCHAR (50) 
);

CREATE TABLE Setting (
    Id       INTEGER      PRIMARY KEY AUTOINCREMENT,
    Name   VARCHAR (50), 
    Key    VARCHAR (100), 
    Value  VARCHAR (300) 
);
-- 设置数据
INSERT INTO Setting ( Value,[Key], Name,Id)
                    VALUES (1,'CommonSetting.Store.StoreId','当前门店',1),
                    (1,'CommonSetting.Store.PosId','POS机编号',1);


CREATE TABLE VipCard (
    Id       INTEGER      PRIMARY KEY ,
    Code   VARCHAR (50), 
    Discount     decimal(8,2)
);

CREATE TABLE VipProduct (
    Id       INTEGER      PRIMARY KEY ,
    ProductId   INTEGER, 
    SalePrice   decimal(8,2)
);

CREATE TABLE ProductAreaPrice (
    Id       INTEGER      PRIMARY KEY ,
    ProductId   INTEGER, 
    AreaId         varchar(6), 
    SalePrice    decimal(8,2)
);

CREATE TABLE ProductStorePrice (
    Id       INTEGER      PRIMARY KEY ,
    ProductId   INTEGER, 
    StoreId     INTEGER, 
    SalePrice   decimal(8,2)
);




