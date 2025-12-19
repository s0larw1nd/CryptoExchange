namespace WalletService.Migrations.Scripts;
using FluentMigrator;

[Migration(1)]
public class InitWallet: Migration 
{
    public override void Up()
    {
        var sql = @"    
        CREATE TABLE IF NOT EXISTS wallet (
            id bigserial not null primary key,
            uid bigserial not null,
            currency varchar not null,
            balance bigint not null
        );

        CREATE INDEX IF NOT EXISTS idx_wallet ON wallet(uid, currency, balance);

        CREATE TYPE v1_curr AS (
            id bigint,
            uid bigint,
            currency varchar,
            balance bigint
        );
        ";
        
        Execute.Sql(sql);
    }
    
    public override void Down()
    {
        throw new NotImplementedException();
    }
}