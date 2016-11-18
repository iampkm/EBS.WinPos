-- 商品
DROP TABLE Product;

CREATE TABLE Product (
    Id            INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
    Code          VARCHAR (20),
    Name          VARCHAR (50),
    BarCode       VARCHAR (50),
    Specification VARCHAR (200),
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
   Id                    INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
   Code                 varchar(20),
   StoreId              INTEGER ,
   Status               INTEGER ,
   PaidStatus           INTEGER ,
   CreatedOn            datetime ,
   CreatedBy            INTEGER,
   UpdatedOn            datetime ,
   UpdatedBy            INTEGER '
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
   Quantity             INTEGER
);

CREATE INDEX idx_SaleOrderItem_SaleOrderId ON SaleOrderItem (
    SaleOrderId
);

/*==============================================================*/
/* Table: SaleOrderItem                                         */
/*==============================================================*/
DROP TABLE PaidHistory;
create table PaidHistory
(
   Id                   INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
   SaleOrderId          INTEGER ,
   SaleOrderCode        varchar(20),
   PaymentWay           INTEGER,
   PaidDate             datetime ,
   OrderAmount          decimal(8,2),
   PayAmount            decimal(8,2),
   CreatedBy            INTEGER,
   CreatedOn            datetime
);

DROP TABLE UploadFailedOrder;
create table UploadFailedOrder
(
   Id                   INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
   SaleOrderId          INTEGER,
   CreatedOn            datetime
);

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
   EndByName            varchar(50)
);

create table Store
(
    Id                   INTEGER  NOT NULL  PRIMARY KEY,
    Code                 varchar(20),
    Name                 varchar(50)
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

