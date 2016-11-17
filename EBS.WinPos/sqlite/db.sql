-- 商品
DROP TABLE Product;

CREATE TABLE Product (
    Id            INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
    Code          VARCHAR (20),
    Name          VARCHAR (50),
    BarCode       VARCHAR (50),
    Specification VARCHAR (200),
    SalePrice     DECIMAL (8, 2),
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
   StoreId              INTEGER comment '门店',
   Status               INTEGER comment '状态',
   PaidStatus           INTEGER comment '支付状态',
   CreatedOn            datetime comment '创建时间',
   CreatedBy            INTEGER comment '创建人',
   UpdatedOn            datetime comment '修改时间',
   UpdatedBy            INTEGER comment '修改人'
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
   PaymentWay           INTEGER comment '支付方式',
   PaidDate             datetime comment '支付时间',
   OrderAmount          decimal(8,2),
   PayAmount            decimal(8,2),
   ChargeAmount         decimal(8,2),
   CreatedBy            INTEGER
   CreatedOn            datetime
);

DROP TABLE UploadFailedOrder;
create table UploadFailedOrder
(
   Id                   INTEGER    NOT NULL  PRIMARY KEY AUTOINCREMENT,
   SaleOrderId          INTEGER,
   CreatedOn            datetime
);
