using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apotheosis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReactionForwardingRuleEmojiIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ReactionForwardingRules_EmojiId_Name",
                table: "ReactionForwardingRules",
                columns: new[] { "EmojiId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReactionForwardingRules_EmojiId_Name",
                table: "ReactionForwardingRules");
        }
    }
}
