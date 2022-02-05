using CefSharp;
using CefSharp.WinForms;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp12;

namespace WindowsFormsApp12
{
    public partial class Form1 : MaterialForm
    {
        private int indexOfItemUnderMouseToDrop;



        public Form1()
        {
            InitializeComponent();
            //ОФОРМЛЕНИЕ
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialTabControl1.SelectedIndexChanged += SelectIndexChanged;
            materialLabel1.ForeColor = Color.White;
        }


        //загрузка браузера
        private void Form1_Load(object sender, EventArgs e)
        {

            //ЕСЛИ ID РЕБЕНКА ЕЩЕ НЕ ЗАДАНО ТО ВЫВЕСТИ ОКНО
            if (!File.Exists("CHILD_ID.data"))
            {
                Form3 f3 = new Form3();
                f3.ShowDialog();
            }
            if (File.Exists("CHILD_ID.data"))
            {
                
          
            User.ID = File.ReadAllText("CHILD_ID.data");

            //ОБРАЩЕНИЕ К СЕРВЕРУ ЗА ФИЛЬТРАМИ
            CheckerData.UploadFilters();

            }
            else
            {
                Application.Exit();

                return;
            }
            //ОЧИСТКА ВКЛАДОК 
            materialTabControl1.Controls.Clear();
            //СОЗДАНИЕ ПЕРВОЙ ВКЛАДКИ
            newTab();
        }

        //ВВОД ЗАПРОСА В СТРОКУ
        private void button1_Click(object sender, EventArgs e)
        {
            OpenButton();
        }


        private void OpenButton()
        {
            //выбирается текущая вкладка
            ChromiumWebBrowser chrom = materialTabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
            //БРАУЗЕР СКРЫВАЕТСЯ
            chrom.Visible = false;
            string query = materialSingleLineTextField1.Text;

            if (query.IndexOf(".") != -1)
            {
                chrom.Load(query);
            }
            else
            {
                chrom.Load("yandex.ru?q=" + query);
            }
        }




        //смена адреса 
        private void chromiumWebBrowser1_AddressChanged(object sender, AddressChangedEventArgs e)
        {

            this.Invoke(new MethodInvoker(() =>
            {
                materialSingleLineTextField1.Text = e.Address;
             ;
            
            }));
        
        }


        //СОЗДАНИЕ ВКЛАДКИ 
        public void newTab(string url = "https://yandex.ru")
        {
            this.Invoke(new MethodInvoker(() =>
            {
                TabPage tab = new TabPage();
            tab.Text = "";
            //ВИЗУАЛЬНОЕ СОЗДАНИЕ ВКЛАДКИ
            materialTabControl1.Controls.Add(tab);
            materialTabControl1.SelectTab(materialTabControl1.TabCount - 1);

            //СОЗДАЕТСЯ БРАУЗЕР
            ChromiumWebBrowser chrom = new ChromiumWebBrowser(url);
            chrom.Parent = materialTabControl1.SelectedTab;
            chrom.Dock = DockStyle.Fill;

            //ФИКСИРУЮТСЯ СОБЫТИЯ 
               // chrom.AddressChanged += Chrom_AddressChanged;
                chrom.LoadingStateChanged += Chrom_LoadingStateChanged1;
                chrom.ConsoleMessage += chromiumWebBrowser1_ConsoleMessage;
                chrom.ExecuteScriptAsyncWhenPageLoaded("test35");
                chrom.LifeSpanHandler = new SampleLifeSpanHandler(this);
                chrom.AddressChanged += chromiumWebBrowser1_AddressChanged;
                chrom.TitleChanged += chromiumWebBrowser1_TitleChanged;
                chrom.DownloadHandler = new DownloadHandler();
            }));
        }

        private string lastURL = "";
        
        
        //СОБЫТИЕ ЗАГРУЗКИ КОНТЕНТА
        private async void Chrom_LoadingStateChanged1(object sender, LoadingStateChangedEventArgs e)
        {


          
            if (e.IsLoading)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    ChromiumWebBrowser chrom = materialTabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
                    if (lastURL != chrom.Address)
                    {
                        chrom.Visible = false;
                    }

                }));
            }
            else
            {
               
                //ЧТЕНИЕ ИСХОДНОГО КОДА
                this.Invoke(new MethodInvoker(() =>
                {
                    GetSource(materialTabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser);
                }));

            }
        }


        //анализ страницы 
        private async void GetSource(ChromiumWebBrowser chrom)
        {

           
            string block_id = "";
            try
            {

                //СКАЧИВАЕМ ИСХОДНЫЙ КОД 
                string html = await chrom.GetSourceAsync();
                //ИЩЕМ БЛОКИРОВКИ 
                block_id = CheckerData.Parsing(chrom.Address, html);
                //ЕСЛИ ИХ НЕТ
                if (block_id == "-" || block_id.IndexOf("_watch") != -1)
                {
                    //ВЫВОДИМ БРАУЗЕР
                    chrom.Visible = true;
                }
                else
                {
                    //выводится html код ОШИБКИ 
                    chrom.EvaluateScriptAsync("document.body.innerHTML='<h2>400 error</h2>';console.log('show_frame')");
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, block_id);
            }


            //отправка адреса в историю
            //с пометкой id блокировки или наблюдения 
            block_id = block_id.Replace("_watch", "");
            if (lastURL != chrom.Address )
            {

                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = System.Text.Encoding.UTF8;
                    string status = wc.DownloadString(String.Format("http://test.tasty-catalog.ru/users/getStatusChildren/?type=watch&number={0}", User.ID));


                    User.STATUS = status == "True";
                }

                if (block_id != "--") {
                    //отпра текущего URL
                    lastURL = chrom.Address;
                    User.SendURL(chrom.Address, block_id);
                }
            }

        }





        //обработчики кнопок
        //СОЗДАНИЕ НОВОЙ ВКЛАДКИ
        private void button2_Click(object sender, EventArgs e)
        {
            newTab();
        }
        


        Dictionary<int, string> tabTitle = new Dictionary<int, string>();


        private void chromiumWebBrowser1_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {


            if (e.Message.IndexOf("show_frame") != -1)
            {

                this.Invoke(new MethodInvoker(() =>
                {
                    ChromiumWebBrowser chrom = materialTabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
                    chrom.Visible = true;
                }));
            }
        }


        //смена вкладки 
        public void SelectIndexChanged(object sender, EventArgs e)
        {
            materialLabel1.ForeColor = Color.White;
            if (tabTitle.ContainsKey(materialTabControl1.SelectedIndex))

                //ChromiumWebBrowser chrom = materialTabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
                if (tabTitle.ContainsKey(materialTabControl1.SelectedIndex))
                    materialLabel1.Text = tabTitle[materialTabControl1.SelectedIndex];
        }


        //СМЕНА ИМЕНИ БРАУЗЕРА ПРИ СМЕНЕ СТРАНИЦЫ
        private void chromiumWebBrowser1_TitleChanged(object sender, TitleChangedEventArgs e)
        {


            this.Invoke(new MethodInvoker(() =>
            {
                Control control =  ((ChromiumWebBrowser) sender ).Parent;
                int tabIndex = control.TabIndex;
                if (tabTitle.ContainsKey(tabIndex))
                {
                    tabTitle[tabIndex] = e.Title;
                }
                else
                {
                    tabTitle.Add(tabIndex, e.Title);
                }
                int currentTabIndex = materialTabControl1.SelectedIndex;
                if (materialTabControl1.SelectedIndex == tabIndex )
                {
                    //this.Text = e.Title;
                }

                materialLabel1.Text = tabTitle[currentTabIndex];
                materialLabel1.Refresh();
                materialTabControl1.TabPages[tabIndex].Text = e.Title.Length > 15 ? e.Title.Substring(0, 15) + "..." : e.Title;
                materialTabControl1.Refresh();
                this.Refresh();
                Application.DoEvents();

            }));

         
        }






        //КНОПКА ДАЛЕЕ
        private void materialFlatButton1_Click_2(object sender, EventArgs e)
        {
            ChromiumWebBrowser chrom = materialTabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
            chrom.Forward();
        }
        //КНОПКА НАЗАД
        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            ChromiumWebBrowser chrom = materialTabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
            chrom.Back();
        }


        private void materialSingleLineTextField1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenButton();
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            Form2 historyForm = new Form2(this);
            historyForm.ShowDialog();
        }
    }
}


//НОВАЯ ВКЛАДКА 
public class SampleLifeSpanHandler : ILifeSpanHandler
{
    public event Action<string> PopupRequest;

    Form1 form;

    public SampleLifeSpanHandler(Form1 form)
    {
        this.form = form;
    }

    public bool OnBeforePopup(IWebBrowser browser, string sourceUrl, string targetUrl, ref int x, ref int y, ref int width,
        ref int height)
    {
        if (PopupRequest != null)
            PopupRequest(targetUrl);

        return true;
    }

    public void OnBeforeClose(IWebBrowser browser)
    {

    }

    bool ILifeSpanHandler.OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
    {
        if (PopupRequest != null)
            PopupRequest(targetUrl);

        newBrowser = null;
      
   
        form.newTab(targetUrl);
        return true;
    }

    void ILifeSpanHandler.OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
    {

    }



    void ILifeSpanHandler.OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
    {
 
    }

    void NotImplementedException()
    {
        //Console.WriteLine("12");
    }

    bool ILifeSpanHandler.DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
    {
        return true;
    }
}

//ОБРАБОТЧИК ЗАГРУЗКИ ФАЙЛОВ 
public class DownloadHandler : IDownloadHandler
{
    public event EventHandler<DownloadItem> OnBeforeDownloadFired;

    public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

    public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
    {
        OnBeforeDownloadFired?.Invoke(this, downloadItem);

        if (!callback.IsDisposed)
        {
            using (callback)
            {
                callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
            }
        }
    }

    public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
    {
        OnDownloadUpdatedFired?.Invoke(this, downloadItem);
    }
}