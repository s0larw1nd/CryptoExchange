namespace PriceFeed.migrations.Scripts;
using FluentMigrator;

[Migration(1)]
public class InitPrices: Migration
{
    public override void Up()
    {
        var sql = @"    
        CREATE TABLE IF NOT EXISTS prices_raw (
            id bigserial not null primary key,
            pair varchar not null,
            bid bigint not null,
            ask bigint not null,
            time timestamp with time zone not null
        );

        create index if not exists idx_prices_raw_pair_time on prices_raw(pair, time DESC);

        create type v1_prices_raw as (
            id bigint,
            pair varchar,
            bid bigint,
            ask bigint,
            time timestamp with time zone
        );
        ";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}