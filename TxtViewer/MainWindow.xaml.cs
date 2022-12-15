using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TxtViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DataTable? _dataTable;
    public MainWindow()
    {
        InitializeComponent();
#if NET
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
    }

    private async void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "数据文件 (*.txt)|*.txt"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            var path = openFileDialog.FileName;
            try
            {
                var table = new DataTable();
                List<string> columns = new();
                List<List<string>> rows = new();
                await Task.Run(async () =>
                {
                    await LoadTxtFileAsync(path, columns, rows);
                });
                for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var column = new DataColumn()
                    {
                        DataType = typeof(string),
                        ColumnName = $"f{columnIndex}",
                        Caption = columns[columnIndex]
                    };
                    if (columnIndex == 0)
                    {
                        column.DataType = typeof(int);
                    }
                    table.Columns.Add(column);
                }
                foreach (var rowData in rows)
                {
                    var row = table.NewRow();
                    for (var i = 0; i < rowData.Count; i++)
                    {
                        if (i == 0)
                        {
                            row[$"f{i}"] = int.Parse(rowData[i]);
                        }
                        else
                        {
                            row[$"f{i}"] = rowData[i];
                        }

                    }
                    table.Rows.Add(row);
                }
                _dataTable = table;
                grid.ItemsSource = table.DefaultView;
                grid.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} {ex.StackTrace}", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private async Task LoadTxtFileAsync(string path, List<string> columns, List<List<string>> rows)
    {
        var textEncoding = Encoding.GetEncoding("GB18030");
        using (var stream = File.OpenRead(path))
        {
            using (var reader = new StreamReader(stream, textEncoding))
            {
                var firstLine = await reader.ReadLineAsync();
                var labelLine = await reader.ReadLineAsync();
                if ((firstLine is null) || (labelLine is null))
                {
                    throw new Exception("null at head line");
                }
                var columnTypes = firstLine.Split('\t');
                var columnLabels = labelLine.Split('\t');
                for (var i = 0; i < columnTypes.Length; i++)
                {
                    var columnType = columnTypes[i];
                    if (string.IsNullOrEmpty(columnType))
                    {
                        continue;
                    }
                    var label = string.Empty;
                    if (i < columnLabels.Length)
                    {
                        label = columnLabels[i];
                    }
                    columns.Add($"({i},{columnType}){label}");
                }
                //
                while (true)
                {
                    var lineContent = await reader.ReadLineAsync();
                    if (lineContent is null)
                    {
                        break;
                    }
                    if (lineContent.StartsWith("#"))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(lineContent))
                    {
                        continue;
                    }
                    var row = ParseRow(lineContent, columns.Count);
                    if (row != null)
                    {
                        rows.Add(row);
                    }
                }
            }
        }
    }

    private List<string>? ParseRow(string lineContent, int columnCount)
    {
        var fields = lineContent.Split('\t');
        if (string.IsNullOrEmpty(fields[0]))
        {
            return null;
        }
        var rowFields = new List<string>();
        for (var i = 0; i < columnCount; i++)
        {
            if (i < fields.Length)
            {
                rowFields.Add(fields[i]);
            }
            else
            {
                rowFields.Add(string.Empty);
            }
        }
        return rowFields;
    }

    private void grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (_dataTable != null)
        {
            e.Column.Header = _dataTable.Columns[e.PropertyName]?.Caption ?? e.PropertyName;
        }
    }
}
