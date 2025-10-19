using FluentMigrator;
namespace PriceFeed.migrations.Scripts;

[Migration(3)]
public class UpdTimestamp: Migration  {
    public override void Up()
    {
        var sql = @"    
        DROP TYPE v1_prices_raw;
        DROP TABLE prices_raw;

        CREATE TYPE v1_prices_raw AS (
            id bigint,
            pair varchar,
            bid bigint,
            ask bigint,
            time timestamp with time zone
        );

        CREATE TABLE prices_raw (
            id bigserial not null primary key,
            pair varchar not null,
            bid bigint not null,
            ask bigint not null,
            time timestamp with time zone not null
        );

        CREATE INDEX IF NOT EXISTS idx_prices_raw_pair_time ON prices_raw(pair, time DESC);
        ";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}