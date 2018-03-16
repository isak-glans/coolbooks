namespace CoolBooks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersDisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DisplayName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DisplayName");
        }
    }
}
