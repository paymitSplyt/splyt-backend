using System.Data.Entity;
using Backend.DataAccess.Models;
using Backend.Migrations;

namespace Backend.DataAccess
{
    internal class DbInitializer : MigrateDatabaseToLatestVersion<DataContext, Configuration>
    {
    }
}