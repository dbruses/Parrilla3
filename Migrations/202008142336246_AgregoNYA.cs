namespace Parrilla3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregoNYA : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "nombre", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "apellido", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "apellido");
            DropColumn("dbo.AspNetUsers", "nombre");
        }
    }
}
