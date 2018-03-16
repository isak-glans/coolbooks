namespace CoolBooks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reviewTextRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reviews", "Title", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Reviews", "Text", c => c.String(nullable: false, maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reviews", "Text", c => c.String(maxLength: 4000));
            AlterColumn("dbo.Reviews", "Title", c => c.String(maxLength: 50));
        }
    }
}
