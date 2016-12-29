delete from WorkSchedule;   
select * from sqlite_sequence;   
update sqlite_sequence set seq=0 where name='WorkSchedule';


delete from SaleOrderItem;   

update sqlite_sequence set seq=0 where name='SaleOrderItem';