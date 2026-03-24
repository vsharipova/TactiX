using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TactiX.Migrations
{
    public partial class AddExerciseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise",
                schema: "public",
                columns: table => new
                {
                    exercise_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tags = table.Column<string>(type: "jsonb", nullable: true),
                    difficulty = table.Column<string>(type: "text", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise", x => x.exercise_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_category",
                schema: "public",
                table: "exercise",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_difficulty",
                schema: "public",
                table: "exercise",
                column: "difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_name",
                schema: "public",
                table: "exercise",
                column: "name");

            migrationBuilder.InsertData(
                schema: "public",
                table: "exercise",
                columns: new[] { "name", "description", "category", "image_url", "tags", "difficulty", "duration" },
                values: new object[,]
                {
                    {
                        "Точные выстрелы по мишени",
                        "Стрельба по статичной мишени с акцентом на точность попадания в центр.",
                        "Accuracy",
                        null,
                        "[\"статичная\", \"точность\"]",
                        "Beginner",
                        new TimeSpan(0, 0, 5, 0)
                    },
                    {
                        "Быстрые переносы между целями",
                        "Тренировка быстрого переноса оружия между двумя мишенями на расстоянии 5 метров.",
                        "Transfers",
                        null,
                        "[\"перенос\", \"скорость\"]",
                        "Intermediate",
                        new TimeSpan(0, 0, 3, 0)
                    },
                    {
                        "Вскидка из кобуры",
                        "Отработка быстрой вскидки оружия из кобуры и первого выстрела по мишени.",
                        "DrawSpeed",
                        null,
                        "[\"вскидка\", \"кобура\"]",
                        "Intermediate",
                        new TimeSpan(0, 0, 2, 30)
                    },
                    {
                        "Стрельба в движении вперед",
                        "Движение вперед с ведением огня по мишеням на дистанции.",
                        "MovementSpeed",
                        null,
                        "[\"движение\", \"стрельба\"]",
                        "Advanced",
                        new TimeSpan(0, 0, 4, 0)
                    },
                    {
                        "Стрельба с боковым перемещением",
                        "Боковое перемещение с одновременной стрельбой по появляющимся мишеням.",
                        "ShootingOnMove",
                        null,
                        "[\"боковое\", \"динамика\"]",
                        "Advanced",
                        new TimeSpan(0, 0, 3, 30)
                    }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise",
                schema: "public");
        }
    }
}
