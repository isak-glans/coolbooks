namespace CoolBooks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AvgRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "AvgRating", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "AvgRating");
        }
    }
}
