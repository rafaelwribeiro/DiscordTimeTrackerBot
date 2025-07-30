using DiscordTimeTracker.Application.DTOs;
using DiscordTimeTracker.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace DiscordTimeTracker.Infrastructure.Reporting;

public class QuestPdfMonthlyReportGenerator : IMonthlyReportPdfGenerator
{
    public byte[] Generate(string userName, int year, int month, List<TimeEntryDto> entries, TimeSpan totalWorked)
    {
        var file = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text($"Time Report - {userName}").FontSize(20).SemiBold();
                page.Content().Element(BuildTable);
                page.Footer().AlignCenter().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

                void BuildTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Timestamp").SemiBold();
                            header.Cell().Text("Type").SemiBold();
                        });

                        foreach (var entry in entries)
                        {
                            table.Cell().Text(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                            table.Cell().Text(entry.Type.ToString());
                        }

                        table.Cell().ColumnSpan(2).PaddingTop(20).Text($"Total worked: {totalWorked}");
                    });
                }
            });
        });

        return file.GeneratePdf();
    }
}
