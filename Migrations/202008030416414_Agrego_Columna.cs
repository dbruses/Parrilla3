namespace Parrilla3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agrego_Columna : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "direccion", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "telefono", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "telefono");
            DropColumn("dbo.AspNetUsers", "direccion");
        }
    }
}
