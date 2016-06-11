namespace NetsizeWorldCup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTVBroadcast : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Game", "TVBroadcast", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Game", "TVBroadcast");
        }
    }
}
