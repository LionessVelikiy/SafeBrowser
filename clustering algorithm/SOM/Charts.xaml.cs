using CefSharp;
using EMSplit;
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
using System.Windows.Shapes;

namespace SOM
{
    /// <summary>
    /// Логика взаимодействия для Charts.xaml
    /// </summary>
    public partial class Charts : Window
    {
        private TData td;
        public Charts(TData td)
        {
            InitializeComponent();

        
            this.td = td;
        }

        private void browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string table = "<table><tr><th> Значение </th><th> Кластер </th> </tr>";
            List<string> category = new List<string>();
            List<string> data = new List<string>();
            td.clusterNum = new int[] { 0, 1, 1, 1, 0, 0, 1,0,1 };

            //ПОСТРОЕНИЕ ТАБЛИЦЫ
            for (int n = 0; n < td.M; n++)
            {
                table += String.Format(@"<tr><td> {0}</td><td> {1}</td></tr>", td.items[n], td.clusterNum[n] == 0 ? "Блокировка" : "Не блокировка");
                //Console.WriteLine(String.Format(@"{0}-{1}",  td.items[n], td.clusterNum[n]));
            }
            table += "</table>";



            //ПОСТРОЕНИЕ ГРАФИКА
            for (int m = 0; m < td.N; m++)

            {
                string[] list = new string[td.M];
                for (int n = 0; n < td.M; n++)
                {
                    
                    list[n] =String.Format("{{" +
                        "y:{0},{1}" +
                        "}}", td.XX[n, m].ToString().Replace(",","."),td.clusterNum[n] != 0? getPattern(m%5) : getColor(m % 5));

                }
                category.Add(String.Format(@"{{
                        name: '{0}',
                        data: [{1}],
                visible:{2}
                    }}
            ", td.fields[m],String.Join(",", list),m==0? "true":"false"));
            }





        
            foreach (string s in td.items)
            {
               data.Add(String.Format("'{0}'", s ));
            }

            //html код страницы 
            if (browser.IsBrowserInitialized)
            {
                browser.LoadHtml(@" <html><head>

<meta http-equiv='Content-Type' content='text/html; charset = UTF-8' />
<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js'></script>
<script src='https://code.highcharts.com/highcharts.js'></script></script><script src='https://code.highcharts.com/modules/pattern-fill.js'></script></head ><body >
<div> <button id='show-table'> Таблица </button> <button id='show-chart'> График </button> <button id='save-result'> Создать </button> </div>
<div id='tableData' style='display:none;'>" + table + @"</div>
<div id = 'graphAvgclickcostTotal' > </div ><script > Highcharts.chart('graphAvgclickcostTotal',{
    chart: {
        type: 'column',
        height: 1000,
    },

    xAxis: {
        categories: [" + String.Join(",", data) + @"]
    },
title:'Заголовок',
    yAxis: {
        title: {
            text: ''
        }
    },
    plotOptions: {
        line: {
            dataLabels: {
                enabled: true
            },
            enableMouseTracking: false
        }
    },
    series: [" +
    String.Join(",", category)
    + @"]
})
$('#show-table').click(function(){
$('#graphAvgclickcostTotal').hide()
$('#tableData').show()
})
$('#show-chart').click(function(){
$('#graphAvgclickcostTotal').show()
$('#tableData').hide()
})

$('#save-result').click(function(){
var title = prompt('Название кластера','Кластер')
$.ajax({
	url: 'http://test.tasty-catalog.ru/history/addCluster/?title='+title,         
	method: 'get',            
	success: function(data){   
		alert('Группа успешно создана');           
	}
});

})
</script></body></html>", "http://localhost");
            }
        }

        string[] colors = new string[] { "#A30000", "#04b", "#16ad13", "#9c13ad", "#d7ea33" };
        public string getPattern(int index)
        {

          
            return String.Format(@"color: {{
                    pattern: {{
            path:
                {{
                d: 'M 0 0 L 10 10 M 9 -1 L 11 1 M -1 9 L 1 11',
                        strokeWidth: 4
                    }},
                    color: '{0}',
                    width: 10,
                    height: 10,
                    opacity: 0.8
                }}
        }}", this.colors[index]);
        }

        public string getColor(int index)
        {
            return String.Format("color:'{0}'",this.colors[index]);
        }
    }


}
