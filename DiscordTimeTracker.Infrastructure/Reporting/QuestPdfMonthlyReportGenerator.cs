using DiscordTimeTracker.Application.DTOs;
using DiscordTimeTracker.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace DiscordTimeTracker.Infrastructure.Reporting;

public class QuestPdfMonthlyReportGenerator : IMonthlyReportPdfGenerator
{
    public byte[] Generate(string userName, int year, int month, List<TimeEntryDto> entries, TimeSpan totalWorked)
    {
        var file = Report(userName, entries, totalWorked);

        return file.GeneratePdf();
    }

    private static Document Report(string userName, List<TimeEntryDto> entries, TimeSpan totalWorked)
    {
        var groupedEntries = entries
            .GroupBy(e => e.Timestamp.Date)
            .OrderBy(g => g.Key)
            .ToList();

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Monthly Report - {userName}")
                    .SemiBold()
                    .FontSize(20)
                    .FontColor(Colors.Blue.Medium);

                page.Content().Column(column =>
                {
                    if (!groupedEntries.Any())
                    {
                        column.Item().Text("No entries available.");
                        return;
                    }

                    //for (global::System.Int32 i = 0; i < 10; i++)
                    foreach (var group in groupedEntries)
                    {
                        var date = group.Key.ToString("yyyy-MM-dd");

                        column.Item().PaddingBottom(5).Text(date).Bold().FontSize(14);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100); // Hora Entrada
                                columns.ConstantColumn(100); // Hora Saída
                                columns.RelativeColumn();    // Saldo
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("In").SemiBold();
                                header.Cell().Text("Out").SemiBold();
                                header.Cell().Text("Balance").SemiBold();
                            });

                            var punches = group.OrderBy(e => e.Timestamp).ToList();
                            for (int i = 0; i < punches.Count; i += 2)
                            {
                                var entrada = punches[i];
                                var saida = (i + 1 < punches.Count) ? punches[i + 1] : null;

                                var entradaHora = entrada.Timestamp.ToLocalTime().ToString("HH:mm");
                                var saidaHora = saida?.Timestamp.ToLocalTime().ToString("HH:mm") ?? "--";

                                TimeSpan saldo = TimeSpan.Zero;
                                if (saida != null)
                                    saldo = saida.Timestamp - entrada.Timestamp;

                                table.Cell().Text(entradaHora);
                                table.Cell().Text(saidaHora);
                                table.Cell().Text(saldo == TimeSpan.Zero ? "--" : saldo.ToString(@"hh\:mm"));
                            }
                        });

                        column.Item().PaddingBottom(10); // espaçamento entre dias
                    }

                    // Total Geral
                    column.Item().PaddingTop(20)
                        .Text($"Total Worked: {totalWorked:hh\\:mm}")
                        .SemiBold().FontSize(14);
                });
            });
        });
    }

}
