namespace PriceFeed.migrations.Scripts;
using FluentMigrator;

[Migration(2)]
public class UpdType: Migration 
{
    public override void Up()
    {
        var sql = @"    
        DROP TYPE v1_prices_raw;

        CREATE TYPE v1_prices_raw AS (
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