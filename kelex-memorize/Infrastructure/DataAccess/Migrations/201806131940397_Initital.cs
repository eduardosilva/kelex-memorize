namespace kelex_memorize.Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initital : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionAndAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Question = c.String(nullable: false),
                        Answer = c.String(),
                        NextExecution = c.DateTime(),
                        Level = c.Int(nullable: false),
                        Deck = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QuestionAndAnswers");
        }
    }
}
