namespace CoolBooks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueISBN : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Books", "ISBN", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Books", new[] { "ISBN" });
        }
    }
}
