create table deals
(
    deal_number        nvarchar(255)  not null
        constraint deals_pk
            primary key,
    deal_date          date          not null,
    seller_name        nvarchar(255) not null,
    seller_inn         nvarchar(255),
    buyer_name         nvarchar(255) not null,
    buyer_inn          nvarchar(255),
    wood_volume_seller float         not null,
    wood_volume_buyer  float
)
go
