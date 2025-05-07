using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TactiX.Migrations
{
    /// <inheritdoc />
    public partial class Th : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_comparison_comparison_result",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Comparison_Result",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropColumn(
                name: "avg_alphas",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "avg_charlies",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "avg_deltas",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "avg_misses",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "comparison_advice",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.AlterColumn<string>(
                name: "training_type",
                schema: "public",
                table: "training_analysis",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "avg_hit_factor",
                schema: "public",
                table: "training_analysis",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<decimal>(
                name: "alpha_percentage",
                schema: "public",
                table: "training_analysis",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "calculated_at",
                schema: "public",
                table: "training_analysis",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "charlie_percentage",
                schema: "public",
                table: "training_analysis",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "delta_percentage",
                schema: "public",
                table: "training_analysis",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "is_best_performance",
                schema: "public",
                table: "training_analysis",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "miss_percentage",
                schema: "public",
                table: "training_analysis",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "performance_score",
                schema: "public",
                table: "training_analysis",
                type: "numeric(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "total_alphas",
                schema: "public",
                table: "training_analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_charlies",
                schema: "public",
                table: "training_analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_deltas",
                schema: "public",
                table: "training_analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_misses",
                schema: "public",
                table: "training_analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_shots",
                schema: "public",
                table: "training_analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "training_id",
                schema: "public",
                table: "training_analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "training_analysis_id",
                schema: "public",
                table: "training",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "miss_diff",
                schema: "public",
                table: "comparison",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "hit_factor_diff",
                schema: "public",
                table: "comparison",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "delta_diff",
                schema: "public",
                table: "comparison",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "charlie_diff",
                schema: "public",
                table: "comparison",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "alpha_diff",
                schema: "public",
                table: "comparison",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AddColumn<int>(
                name: "MatchAnalysisId",
                schema: "public",
                table: "comparison",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchAnalysisId1",
                schema: "public",
                table: "comparison",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingAnalysisId",
                schema: "public",
                table: "comparison",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingAnalysisId1",
                schema: "public",
                table: "comparison",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_analysis_calculated_at",
                schema: "public",
                table: "training_analysis",
                column: "calculated_at");

            migrationBuilder.CreateIndex(
                name: "IX_training_analysis_is_best_performance",
                schema: "public",
                table: "training_analysis",
                column: "is_best_performance");

            migrationBuilder.CreateIndex(
                name: "IX_training_analysis_performance_score",
                schema: "public",
                table: "training_analysis",
                column: "performance_score");

            migrationBuilder.CreateIndex(
                name: "IX_training_analysis_training_id",
                schema: "public",
                table: "training_analysis",
                column: "training_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comparison_MatchAnalysisId",
                schema: "public",
                table: "comparison",
                column: "MatchAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_MatchAnalysisId1",
                schema: "public",
                table: "comparison",
                column: "MatchAnalysisId1");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_TrainingAnalysisId",
                schema: "public",
                table: "comparison",
                column: "TrainingAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_TrainingAnalysisId1",
                schema: "public",
                table: "comparison",
                column: "TrainingAnalysisId1");

            migrationBuilder.AddForeignKey(
                name: "FK_comparison_match_analysis_MatchAnalysisId",
                schema: "public",
                table: "comparison",
                column: "MatchAnalysisId",
                principalSchema: "public",
                principalTable: "match_analysis",
                principalColumn: "match_analysis_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comparison_match_analysis_MatchAnalysisId1",
                schema: "public",
                table: "comparison",
                column: "MatchAnalysisId1",
                principalSchema: "public",
                principalTable: "match_analysis",
                principalColumn: "match_analysis_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comparison_training_analysis_TrainingAnalysisId",
                schema: "public",
                table: "comparison",
                column: "TrainingAnalysisId",
                principalSchema: "public",
                principalTable: "training_analysis",
                principalColumn: "training_analysis_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comparison_training_analysis_TrainingAnalysisId1",
                schema: "public",
                table: "comparison",
                column: "TrainingAnalysisId1",
                principalSchema: "public",
                principalTable: "training_analysis",
                principalColumn: "training_analysis_id");

            migrationBuilder.AddForeignKey(
                name: "FK_training_analysis_training_training_id",
                schema: "public",
                table: "training_analysis",
                column: "training_id",
                principalSchema: "public",
                principalTable: "training",
                principalColumn: "training_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comparison_match_analysis_MatchAnalysisId",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropForeignKey(
                name: "FK_comparison_match_analysis_MatchAnalysisId1",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropForeignKey(
                name: "FK_comparison_training_analysis_TrainingAnalysisId",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropForeignKey(
                name: "FK_comparison_training_analysis_TrainingAnalysisId1",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropForeignKey(
                name: "FK_training_analysis_training_training_id",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropIndex(
                name: "IX_training_analysis_calculated_at",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropIndex(
                name: "IX_training_analysis_is_best_performance",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropIndex(
                name: "IX_training_analysis_performance_score",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropIndex(
                name: "IX_training_analysis_training_id",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropIndex(
                name: "IX_comparison_MatchAnalysisId",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropIndex(
                name: "IX_comparison_MatchAnalysisId1",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropIndex(
                name: "IX_comparison_TrainingAnalysisId",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropIndex(
                name: "IX_comparison_TrainingAnalysisId1",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropColumn(
                name: "alpha_percentage",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "calculated_at",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "charlie_percentage",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "delta_percentage",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "is_best_performance",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "miss_percentage",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "performance_score",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "total_alphas",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "total_charlies",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "total_deltas",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "total_misses",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "total_shots",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "training_id",
                schema: "public",
                table: "training_analysis");

            migrationBuilder.DropColumn(
                name: "training_analysis_id",
                schema: "public",
                table: "training");

            migrationBuilder.DropColumn(
                name: "MatchAnalysisId",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropColumn(
                name: "MatchAnalysisId1",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropColumn(
                name: "TrainingAnalysisId",
                schema: "public",
                table: "comparison");

            migrationBuilder.DropColumn(
                name: "TrainingAnalysisId1",
                schema: "public",
                table: "comparison");

            migrationBuilder.AlterColumn<string>(
                name: "training_type",
                schema: "public",
                table: "training_analysis",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<double>(
                name: "avg_hit_factor",
                schema: "public",
                table: "training_analysis",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AddColumn<double>(
                name: "avg_alphas",
                schema: "public",
                table: "training_analysis",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "avg_charlies",
                schema: "public",
                table: "training_analysis",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "avg_deltas",
                schema: "public",
                table: "training_analysis",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "avg_misses",
                schema: "public",
                table: "training_analysis",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "comparison_advice",
                schema: "public",
                table: "training_analysis",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "miss_diff",
                schema: "public",
                table: "comparison",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "hit_factor_diff",
                schema: "public",
                table: "comparison",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "delta_diff",
                schema: "public",
                table: "comparison",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "charlie_diff",
                schema: "public",
                table: "comparison",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "alpha_diff",
                schema: "public",
                table: "comparison",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_comparison_result",
                schema: "public",
                table: "comparison",
                column: "comparison_result");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Comparison_Result",
                schema: "public",
                table: "comparison",
                sql: "comparison_result IN ('better', 'worse', 'similar')");
        }
    }
}
