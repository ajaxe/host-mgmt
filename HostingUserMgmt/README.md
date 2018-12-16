# Hosting Management Application

## Handy Commands

### EF Core

Adding new migration

`dotnet ef migrations add --project . --context HostingManagementDbContext --output-dir .\Repository\Migrations\ <migration-name>`

Removing last migration: Only useful if the migration is not applied to database, otherwise a new migration should be created to revert the change.

`dotnet ef migrations remove --project . --context HostingManagementDbContext`

Updating database after adding the migration

`dotnet ef database update --project . --context HostingManagementDbContext`
