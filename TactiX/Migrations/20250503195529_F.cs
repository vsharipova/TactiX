using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TactiX.Migrations
{
    /// <inheritdoc />
    public partial class F : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "training_analysis",
                schema: "public",
                columns: table => new
                {
                    training_analysis_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    training_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    avg_alphas = table.Column<double>(type: "double precision", nullable: false),
                    avg_deltas = table.Column<double>(type: "double precision", nullable: false),
                    avg_charlies = table.Column<double>(type: "double precision", nullable: false),
                    avg_misses = table.Column<double>(type: "double precision", nullable: false),
                    avg_hit_factor = table.Column<double>(type: "double precision", nullable: false),
                    comparison_advice = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_analysis", x => x.training_analysis_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    hash_password = table.Column<string>(type: "text", nullable: false),
                    practice_level = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "match",
                schema: "public",
                columns: table => new
                {
                    match_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    match_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    match_analysis_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match", x => x.match_id);
                    table.ForeignKey(
                        name: "FK_match_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training",
                schema: "public",
                columns: table => new
                {
                    training_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    training_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    type_of_training = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training", x => x.training_id);
                    table.ForeignKey(
                        name: "FK_training_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_analysis",
                schema: "public",
                columns: table => new
                {
                    match_analysis_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    total_shots = table.Column<int>(type: "integer", nullable: false),
                    total_alphas = table.Column<int>(type: "integer", nullable: false),
                    total_deltas = table.Column<int>(type: "integer", nullable: false),
                    total_charlies = table.Column<int>(type: "integer", nullable: false),
                    total_misses = table.Column<int>(type: "integer", nullable: false),
                    alpha_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    delta_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    charlie_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    miss_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    avg_hit_factor = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    performance_score = table.Column<decimal>(type: "numeric(3,1)", nullable: false),
                    is_best_performance = table.Column<bool>(type: "boolean", nullable: false),
                    calculated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_analysis", x => x.match_analysis_id);
                    table.ForeignKey(
                        name: "FK_match_analysis_match_match_id",
                        column: x => x.match_id,
                        principalSchema: "public",
                        principalTable: "match",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_stage",
                schema: "public",
                columns: table => new
                {
                    match_stage_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    stage_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    stage_type = table.Column<long>(type: "bigint", nullable: false),
                    hit_factor = table.Column<double>(type: "double precision", nullable: false),
                    num_of_spots = table.Column<long>(type: "bigint", nullable: false),
                    num_of_poppers = table.Column<long>(type: "bigint", nullable: false),
                    num_of_plates = table.Column<long>(type: "bigint", nullable: false),
                    alphas_count = table.Column<long>(type: "bigint", nullable: false),
                    deltas_count = table.Column<long>(type: "bigint", nullable: false),
                    charlies_count = table.Column<long>(type: "bigint", nullable: false),
                    misses_count = table.Column<long>(type: "bigint", nullable: false),
                    stage_time = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_stage", x => x.match_stage_id);
                    table.ForeignKey(
                        name: "FK_match_stage_match_match_id",
                        column: x => x.match_id,
                        principalSchema: "public",
                        principalTable: "match",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_stage",
                schema: "public",
                columns: table => new
                {
                    training_stage_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    training_id = table.Column<int>(type: "integer", nullable: false),
                    stage_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    stage_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    hit_factor = table.Column<double>(type: "double precision", nullable: false),
                    num_of_spots = table.Column<long>(type: "bigint", nullable: false),
                    num_of_poppers = table.Column<long>(type: "bigint", nullable: false),
                    num_of_plates = table.Column<long>(type: "bigint", nullable: false),
                    alphas_count = table.Column<long>(type: "bigint", nullable: false),
                    deltas_count = table.Column<long>(type: "bigint", nullable: false),
                    charlies_count = table.Column<long>(type: "bigint", nullable: false),
                    misses_count = table.Column<long>(type: "bigint", nullable: false),
                    stage_time = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_stage", x => x.training_stage_id);
                    table.ForeignKey(
                        name: "FK_training_stage_training_training_id",
                        column: x => x.training_id,
                        principalSchema: "public",
                        principalTable: "training",
                        principalColumn: "training_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "analysis",
                schema: "public",
                columns: table => new
                {
                    analysis_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    period_start = table.Column<DateOnly>(type: "date", nullable: false),
                    period_end = table.Column<DateOnly>(type: "date", nullable: false),
                    total_alphas = table.Column<long>(type: "bigint", nullable: false),
                    total_deltas = table.Column<long>(type: "bigint", nullable: false),
                    total_charlies = table.Column<long>(type: "bigint", nullable: false),
                    avg_hit_factor = table.Column<double>(type: "double precision", nullable: false),
                    training_analysis_id = table.Column<int>(type: "integer", nullable: false),
                    match_analysis_id = table.Column<int>(type: "integer", nullable: false),
                    overal_trend = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    general_advice = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis", x => x.analysis_id);
                    table.ForeignKey(
                        name: "FK_analysis_match_analysis_match_analysis_id",
                        column: x => x.match_analysis_id,
                        principalSchema: "public",
                        principalTable: "match_analysis",
                        principalColumn: "match_analysis_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_analysis_training_analysis_training_analysis_id",
                        column: x => x.training_analysis_id,
                        principalSchema: "public",
                        principalTable: "training_analysis",
                        principalColumn: "training_analysis_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_analysis_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "comparison",
                schema: "public",
                columns: table => new
                {
                    comparison_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    base_match_id = table.Column<int>(type: "integer", nullable: false),
                    compared_match_id = table.Column<int>(type: "integer", nullable: false),
                    alpha_diff = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    delta_diff = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    charlie_diff = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    miss_diff = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    hit_factor_diff = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    comparison_result = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    advice = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: true),
                    MatchId1 = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comparison", x => x.comparison_id);
                    table.CheckConstraint("CK_Comparison_Result", "comparison_result IN ('better', 'worse', 'similar')");
                    table.ForeignKey(
                        name: "FK_comparison_match_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "public",
                        principalTable: "match",
                        principalColumn: "match_id");
                    table.ForeignKey(
                        name: "FK_comparison_match_MatchId1",
                        column: x => x.MatchId1,
                        principalSchema: "public",
                        principalTable: "match",
                        principalColumn: "match_id");
                    table.ForeignKey(
                        name: "FK_comparison_match_analysis_base_match_id",
                        column: x => x.base_match_id,
                        principalSchema: "public",
                        principalTable: "match_analysis",
                        principalColumn: "match_analysis_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comparison_match_analysis_compared_match_id",
                        column: x => x.compared_match_id,
                        principalSchema: "public",
                        principalTable: "match_analysis",
                        principalColumn: "match_analysis_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comparison_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_analysis_match_analysis_id",
                schema: "public",
                table: "analysis",
                column: "match_analysis_id");

            migrationBuilder.CreateIndex(
                name: "IX_analysis_period_start_period_end",
                schema: "public",
                table: "analysis",
                columns: new[] { "period_start", "period_end" });

            migrationBuilder.CreateIndex(
                name: "IX_analysis_training_analysis_id",
                schema: "public",
                table: "analysis",
                column: "training_analysis_id");

            migrationBuilder.CreateIndex(
                name: "IX_analysis_UserId",
                schema: "public",
                table: "analysis",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_base_match_id",
                schema: "public",
                table: "comparison",
                column: "base_match_id");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_compared_match_id",
                schema: "public",
                table: "comparison",
                column: "compared_match_id");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_comparison_result",
                schema: "public",
                table: "comparison",
                column: "comparison_result");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_created_at",
                schema: "public",
                table: "comparison",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_MatchId",
                schema: "public",
                table: "comparison",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_MatchId1",
                schema: "public",
                table: "comparison",
                column: "MatchId1");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_UserId",
                schema: "public",
                table: "comparison",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_match_user_id",
                schema: "public",
                table: "match",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_analysis_calculated_at",
                schema: "public",
                table: "match_analysis",
                column: "calculated_at");

            migrationBuilder.CreateIndex(
                name: "IX_match_analysis_is_best_performance",
                schema: "public",
                table: "match_analysis",
                column: "is_best_performance");

            migrationBuilder.CreateIndex(
                name: "IX_match_analysis_match_id",
                schema: "public",
                table: "match_analysis",
                column: "match_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_analysis_performance_score",
                schema: "public",
                table: "match_analysis",
                column: "performance_score");

            migrationBuilder.CreateIndex(
                name: "IX_match_stage_match_id",
                schema: "public",
                table: "match_stage",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_stage_stage_name_hit_factor",
                schema: "public",
                table: "match_stage",
                columns: new[] { "stage_name", "hit_factor" });

            migrationBuilder.CreateIndex(
                name: "IX_training_user_id",
                schema: "public",
                table: "training",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_training_analysis_training_type",
                schema: "public",
                table: "training_analysis",
                column: "training_type");

            migrationBuilder.CreateIndex(
                name: "IX_training_stage_stage_name_hit_factor",
                schema: "public",
                table: "training_stage",
                columns: new[] { "stage_name", "hit_factor" });

            migrationBuilder.CreateIndex(
                name: "IX_training_stage_training_id",
                schema: "public",
                table: "training_stage",
                column: "training_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_name_surname",
                schema: "public",
                table: "user",
                columns: new[] { "name", "surname" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analysis",
                schema: "public");

            migrationBuilder.DropTable(
                name: "comparison",
                schema: "public");

            migrationBuilder.DropTable(
                name: "match_stage",
                schema: "public");

            migrationBuilder.DropTable(
                name: "training_stage",
                schema: "public");

            migrationBuilder.DropTable(
                name: "training_analysis",
                schema: "public");

            migrationBuilder.DropTable(
                name: "match_analysis",
                schema: "public");

            migrationBuilder.DropTable(
                name: "training",
                schema: "public");

            migrationBuilder.DropTable(
                name: "match",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user",
                schema: "public");
        }
    }
}
