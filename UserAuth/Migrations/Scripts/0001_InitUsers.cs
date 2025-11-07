namespace UserAuth.Migrations.Scripts;
using FluentMigrator;

[Migration(1)]
public class InitUsers: Migration 
{
    public override void Up()
    {
        var sql = @"    
        CREATE TABLE IF NOT EXISTS users (
            id bigserial not null primary key,
            username varchar not null unique,
            password varchar not null
        );

        CREATE INDEX IF NOT EXISTS idx_users_username_password ON users(username, password);

        CREATE TYPE v1_users AS (
            id bigint,
            username varchar,
            password varchar
        );
        ";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}