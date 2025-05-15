using liuguang.Dbc;
using Microsoft.Win32;
using System;
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
                var dbcFile = await Task.Run(async () =>
                {
                    return await LoadTxtFileAsync(path);
                });
                var table = new DataTable();
                for (var columnIndex = 0; columnIndex < dbcFile.FieldTypes.Count; columnIndex++)
                {
                    var column = new DataColumn()
                    {
                        ColumnName = $"f{columnIndex}",
                    };
                    var fieldName = string.Empty;
                    if (dbcFile.FieldNames != null)
                    {
                        fieldName = dbcFile.FieldNames[columnIndex];
                    }
                    switch (dbcFile.FieldTypes[columnIndex])
                    {
                        case DbcFieldType.T_INT:
                            column.DataType = typeof(int);
                            column.Caption = $"({columnIndex},INT){fieldName}";
                            break;
                        case DbcFieldType.T_FLOAT:
                            column.DataType = typeof(float);
                            column.Caption = $"({columnIndex},FLOAT){fieldName}";
                            break;
                        case DbcFieldType.T_STRING:
                            column.DataType = typeof(string);
                            column.Caption = $"({columnIndex},STRING){fieldName}";
                            break;
                    }
                    table.Columns.Add(column);
                }
                foreach (var rowData in dbcFile.DataMap.Values)
                {
                    var row = table.NewRow();
                    for (var i = 0; i < rowData.Count; i++)
                    {
                        var nodeField = rowData[i];
                        switch (nodeField.FieldType)
                        {
                            case DbcFieldType.T_INT:
                                row[$"f{i}"] = nodeField.IntValue;
                                break;
                            case DbcFieldType.T_FLOAT:
                                row[$"f{i}"] = nodeField.FloatValue;
                                break;
                            case DbcFieldType.T_STRING:
                                row[$"f{i}"] = nodeField.StringValue;
                                break;
                        }
                    }
                    table.Rows.Add(row);
                }
                _dataTable = table;
                grid.ItemsSource = table.DefaultView;
                grid.Visibility = Visibility.Visible;
                Title = "txt表格查看工具(" + path + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} {ex.StackTrace}", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private async Task<DbcFile> LoadTxtFileAsync(string path)
    {
        DbcFile fileInfo;
        using (var stream = File.OpenRead(path))
        {
            fileInfo = await DbcFile.ReadAsync(stream, (uint)stream.Length);
        }
        return fileInfo;
    }

    private void grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (_dataTable != null)
        {
            e.Column.Header = _dataTable.Columns[e.PropertyName]?.Caption ?? e.PropertyName;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
#if NET
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
    }
}
