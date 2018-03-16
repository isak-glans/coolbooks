namespace CoolBooks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoubleAvgRating : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Books", "AvgRating", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Books", "AvgRating", c => c.Int());
        }
    }
}
