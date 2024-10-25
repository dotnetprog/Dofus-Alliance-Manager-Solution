echo off
set /P migName=Enter MigrationName: 

echo on
call cd ..
call dotnet ef migrations add %migName% -- --environment Development