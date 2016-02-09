using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileHelpers;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.ComponentModel;

namespace check
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string filePath;
        ReadItem[] readAll;
        static int euckrCodepage = 51949;
        static System.Text.Encoding euckr = System.Text.Encoding.GetEncoding(euckrCodepage);
        List<WriteItem> writeOne = new List<WriteItem>();
        List<WriteItem> writeAll = new List<WriteItem>();
        List<WriteItem> writeOut = new List<WriteItem>();
        ObservableCollection<GridModel> oneGridModel = new ObservableCollection<GridModel>();
        FileHelperEngine<ReadItem> engine = new FileHelperEngine<ReadItem>(euckr);
        FileHelperEngine<WriteItem> w_engine = new FileHelperEngine<WriteItem>(euckr);
      
        bool _isSearch;
        public bool IsSearch
        {
            get
            {
                return _isSearch;
            }
            set
            {
                _isSearch = value;
                 OnPropertyChanged("IsSearch");
            }
        }
    
        bool _isLoad;
        public bool IsLoad
        {
            get
            {
                return _isLoad;
            }
            set
            {
                _isLoad = value;
                OnPropertyChanged("IsLoad");
            }
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
            engine.ErrorManager.ErrorLimit = 1;
            w_engine.HeaderText = "날짜,발생시간,이름,호실,단말기 ID";
            DG1.DataContext = oneGridModel;
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SearchStudent();
            }
        }
           
        private void SaveOneButton_Click(object sender, RoutedEventArgs e)
        {
            saveFile(writeOne);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchStudent();
        }

        private void SearchStudent( )
        {
            if(readAll != null)
            {
               
                var query = from item in readAll
                            where item.name == SearchBox.Text
                            select item;

                if (query.Any())
                {
                    writeOne.Clear();
                    oneGridModel.Clear();
                    foreach (var item in query)
                    {
                        GridModel gm = new GridModel() { name = item.name
                        ,hms = item.hms.TimeOfDay
                        ,termId = item.termId
                        ,roomNum = item.roomNum
                        ,ymd = item.ymd.ToShortDateString()};
                     
                        WriteItem wi = new WriteItem() { ymd = item.ymd, hms = item.hms
                        , name = item.name, roomNum = item.roomNum, termId = item.termId };
                        writeOne.Add(wi);
                        oneGridModel.Add(gm);
                        IsSearch = true;
                    }
                }
            }
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (쉼표로분리) (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                InitSettings();
                filePath = openFileDialog.FileName;
                FilePathTextBlcok.Text = filePath;
                try
                {
                    readAll = engine.ReadFile(filePath);
                    ErrorTextBlock.Text = "로드 성공";
                    IsLoad = true;
                }
                catch (System.IO.IOException ex)
                {
                    ErrorTextBlock.Text = "파일이 열려있습니다.\n" + ex.Message;
                }
                catch (Exception ex)
                {
                    ErrorTextBlock.Text= "잘못된 파일입니다.\n" + ex.Message;
                }
              
            }
        }

        private void SaveAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (readAll != null)
            {
                if(!writeAll.Any())
                {
                    var query = readAll.OrderBy(i => i.hms).GroupBy(i => new { i.name, i.roomNum });
                    foreach (var items in query)
                    {
                        foreach (var item in items)
                        {
                            WriteItem wi = new WriteItem()
                            {
                                ymd = item.ymd,
                                hms = item.hms,
                                name = item.name,
                                roomNum = item.roomNum,
                                termId = item.termId
                            };
                            writeAll.Add(wi);
                        }
                    }
                }
                saveFile(writeAll);
            }
        }

        private void SaveOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (readAll != null)
            {
                if(!writeOut.Any())
                {
                    var query = readAll.OrderBy(i => i.hms).GroupBy(i => new { i.name, i.roomNum });
                    foreach (var item in query)
                    {
                        var lastItem = item.Last();
                        if (lastItem.termId == 2)
                        {
                            WriteItem wi = new WriteItem()
                            {
                                ymd = lastItem.ymd,
                                hms = lastItem.hms,
                                name = lastItem.name,
                                roomNum = lastItem.roomNum,
                                termId = lastItem.termId
                            };
                            writeOut.Add(wi);
                        }
                    }
                }
                saveFile(writeOut);  
            }
        }
        private void InitSettings()
        {
            readAll = null;
            IsSearch = false;
            IsLoad = false;
            writeOne.Clear();
            writeAll.Clear();
            writeOut.Clear();
        }

        private void saveFile(List<WriteItem> writeList)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV (쉼표로분리) (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                w_engine.WriteFile(saveFileDialog.FileName, writeList);
            }
        }
    }
}