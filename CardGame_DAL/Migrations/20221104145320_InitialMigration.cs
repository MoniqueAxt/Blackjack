using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardGame_DAL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DECKS Table
            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                });

            // GAME SETTINGS Table
            migrationBuilder.CreateTable(
                name: "GameSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberDecks = table.Column<int>(type: "int", nullable: false),
                    NumberRounds = table.Column<int>(type: "int", nullable: false),
                    NumberPlayersToCreate = table.Column<int>(type: "int", nullable: false),
                    NumberHumanPlayers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSettings", x => x.Id);
                });

            // CARDS Table
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Suit = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    DeckId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id");
                });

            // GAME EVENTS Table
            migrationBuilder.CreateTable(
                name: "GameEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    HandId = table.Column<int>(type: "int", nullable: true),
                    DiscardedCardId = table.Column<int>(type: "int", nullable: true),
                    ToHandId = table.Column<int>(type: "int", nullable: true),
                    FromDeckId = table.Column<int>(type: "int", nullable: true),
                    DrawnCardId = table.Column<int>(type: "int", nullable: true),
                    ReceiveCardEvent_ToHandId = table.Column<int>(type: "int", nullable: true),
                    ReceiveCardEvent_FromDeckId = table.Column<int>(type: "int", nullable: true),
                    DealtCardId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameEvents_Cards_DealtCardId",
                        column: x => x.DealtCardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Cards_DiscardedCardId",
                        column: x => x.DiscardedCardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Cards_DrawnCardId",
                        column: x => x.DrawnCardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Decks_FromDeckId",
                        column: x => x.FromDeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Decks_HandId",
                        column: x => x.HandId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Decks_ReceiveCardEvent_FromDeckId",
                        column: x => x.ReceiveCardEvent_FromDeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Decks_ReceiveCardEvent_ToHandId",
                        column: x => x.ReceiveCardEvent_ToHandId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Decks_ToHandId",
                        column: x => x.ToHandId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // GAMES Table
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivePlayerId = table.Column<int>(type: "int", nullable: false),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    GameDeckId = table.Column<int>(type: "int", nullable: false),
                    GameOver = table.Column<bool>(type: "bit", nullable: false),
                    GameSettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    // deck
                    table.ForeignKey(
                        name: "FK_Games_Decks_GameDeckId",
                        column: x => x.GameDeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    // game settings
                    table.ForeignKey(
                        name: "FK_Games_GameSettings_GameSettingsId",
                        column: x => x.GameSettingsId,
                        principalTable: "GameSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // PLAYERS Table
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsHuman = table.Column<bool>(type: "bit", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: true),
                    Money = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    // deck
                    table.ForeignKey(
                        name: "FK_Players_Decks_HandId",
                        column: x => x.HandId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    // game ID
                    table.ForeignKey(
                        name: "FK_Players_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeckId",
                table: "Cards",
                column: "DeckId");
            // --- Game Events ---
            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_DealtCardId",
                table: "GameEvents",
                column: "DealtCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_DiscardedCardId",
                table: "GameEvents",
                column: "DiscardedCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_DrawnCardId",
                table: "GameEvents",
                column: "DrawnCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_FromDeckId",
                table: "GameEvents",
                column: "FromDeckId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_GameId",
                table: "GameEvents",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_HandId",
                table: "GameEvents",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_OwnerId",
                table: "GameEvents",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_ReceiveCardEvent_FromDeckId",
                table: "GameEvents",
                column: "ReceiveCardEvent_FromDeckId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_ReceiveCardEvent_ToHandId",
                table: "GameEvents",
                column: "ReceiveCardEvent_ToHandId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_ToHandId",
                table: "GameEvents",
                column: "ToHandId");
            // --- end (GameEvents) ---

            // --- Games ---
            migrationBuilder.CreateIndex(
                name: "IX_Games_ActivePlayerId",
                table: "Games",
                column: "ActivePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_DealerId",
                table: "Games",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameDeckId",
                table: "Games",
                column: "GameDeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameSettingsId",
                table: "Games",
                column: "GameSettingsId");
            // --- end (Games) ---

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_HandId",
                table: "Players",
                column: "HandId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEvents_Games_GameId",
                table: "GameEvents",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            // delete game event if its owner (player) is deleted
            migrationBuilder.AddForeignKey(
                name: "FK_GameEvents_Players_OwnerId",
                table: "GameEvents",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Restrict deletion (Game) ActivePlayer if player (Player) is (not?) deleted
            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_ActivePlayerId",
                table: "Games",
                column: "ActivePlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Restrict deletion (Game) Dealer if player (Player) is (not?) deleted
            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_DealerId",
                table: "Games",
                column: "DealerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Decks_GameDeckId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Decks_HandId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "GameEvents");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Decks");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GameSettings");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
