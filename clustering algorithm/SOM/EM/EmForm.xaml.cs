using EMSplit;
using SOM.KM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
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
using System.Windows.Shapes;

namespace SOM.EM
{
    /// <summary>
    /// Логика взаимодействия для EmForm.xaml
    /// </summary>
    public partial class EmForm : Window
    {
        public EmForm()
        {
            InitializeComponent();
        }
        TData td;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
       
                //ДЕЛАЕМ ЗАПРОС НА СЕРВЕР
                List<string> content = serverGet("/history/claster/");  //открываем exel

     
                td = new TData(content.ToArray());

                //ВЫВОДИМ ТАБЛИЦУ
                printTable(td);

                }
        

        private void TablePoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //вызываем кластерный анализ
            TKM KM = new TKM(td);
        }


        private void printTable(TData td)
        {




            DataTable dt = new DataTable();

            DataColumn textColumnName = new DataColumn("field0");
            textColumnName.Caption = "Наименование";

            dt.Columns.Add(textColumnName);


            //ПЕЧАТАЕТСЯ ЗАГОЛОВК ТАБЛИЦЫ

            for (int i = 0; i < td.fields.Length; i++)
            {

                int j = i + 1;
                DataColumn textColumn = new DataColumn("field" + j);
                textColumn.Caption = td.fields[i];
                dt.Columns.Add(textColumn);


            }


        
                //ПАЧАТАЕТСЯ САМА ТАБЛИЦА 
            for (int m = 0; m < td.M; m++)

            {
                var row = dt.NewRow();
                
                for (int n = 0; n < td.N; n++)
                {
                    int j = n + 1;
                    row["field" + j] = td.XX[m, n];

                }
                row["field0"] = td.items[m];
                
                dt.Rows.Add(row);
            }
            


  


            //ВЫВОД НА ЭКРАН

            TablePoint.ItemsSource = dt.AsDataView();

           


            //ВЫВОД ПЕРВОГО СТОЛБЦА 

            for (int i = 0; i < TablePoint.Columns.Count; i++)
            {
                if (i == 0)
                {
                    TablePoint.Columns[i].Header = "Наименование";
                    continue;
                }
                TablePoint.Columns[i].Header = td.fields[i - 1];

            }




       


        }
        public static List<string> serverGet(string path)
        {
            var list = new List<string>();
            var lines = File.ReadAllLines("test.csv");
            foreach (var line in lines)
            {
                list.Add(line);
            }
            return list;
        }



    }
}
