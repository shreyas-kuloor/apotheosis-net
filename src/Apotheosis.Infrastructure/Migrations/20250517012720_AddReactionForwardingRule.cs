using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Apotheosis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReactionForwardingRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReactionForwardingRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmojiId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionForwardingRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReactionForwardingRules_EmojiId_Name_ChannelId",
                table: "ReactionForwardingRules",
                columns: new[] { "EmojiId", "Name", "ChannelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReactionForwardingRules");
        }
    }
}
