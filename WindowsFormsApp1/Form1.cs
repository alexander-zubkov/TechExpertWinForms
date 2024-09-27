//using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.TechExpertRef;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using TheArtOfDev.HtmlRenderer.WinForms;
using System.Collections;
using System.Security.Policy;
using HtmlAgilityPack;
using System.Xml;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        private HtmlPanel htmlPanel;
        //private StringBuilder tableContent;
        private HtmlAgilityPack.HtmlDocument htmlDoc;
        private HtmlNode tableNode;
        public Form1()
        {
            InitializeComponent();

            searchBut.Click += SubmitButton_Click;

            // Инициализация HtmlPanel для отображения HTML
            htmlPanel = new HtmlPanel
            {
                Dock = DockStyle.Fill
            };
            //htmlPanel.Controls.Add
            Controls.Add(htmlPanel);

            // Инициализация StringBuilder для хранения содержимого таблицы
            //tableContent = new StringBuilder();
            //tableContent = new StringBuilder();
            //tableContent.Append(@"
            //    <style>
            //        table {
            //            padding-top: 50px;
            //            width: 100%;
            //            border-collapse: collapse;
            //        }
            //        tr {
            //            height: 100px;
            //        }
            //        td {
            //            border: 1px solid black;
            //            text-align: center;
            //        }
            //    </style>
            //    <table>
            //        <tr><th>Найденные документы</th></tr>
            //    </table>"); // Заголовок таблицы

            // Создание нового HtmlDocument
            htmlDoc = new HtmlAgilityPack.HtmlDocument();
            

            // Создание тега <html>
            var htmlNode = HtmlNode.CreateNode("<html></html>");
            htmlDoc.DocumentNode.AppendChild(htmlNode);

            // Создание тега <head> и добавление CSS-ссылки
            var headNode = HtmlNode.CreateNode("<head></head>");
            var styleNode = HtmlNode.CreateNode("<link rel='stylesheet' type='text/css' href='C:\\Users\\LIKORIS001\\Desktop\\Winforms TechExpert\\WindowsFormsApp1\\WindowsFormsApp1\\src\\styles.css'/>");
            headNode.AppendChild(styleNode);
            htmlNode.AppendChild(headNode);

            // Создание тега <body>
            var bodyNode = HtmlNode.CreateNode("<body></body>");
            htmlNode.AppendChild(bodyNode);

            // Создание таблицы
            tableNode = HtmlNode.CreateNode("<table></table>");
            bodyNode.AppendChild(tableNode);

            // Создание заголовка таблицы
            //var headerRowNode = HtmlNode.CreateNode("<tr><th class=\"title\">Найденные документы</th></tr>");
            //tableNode.AppendChild(headerRowNode);

            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(htmlDoc.DocumentNode.OuterHtml);
            //foreach(XmlNode v in doc)
            //{
            //    var b = v.Attributes;
            //    Console.WriteLine();
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //HtmlDocument
        }



        // Обработчик клика по кнопке
        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            // Получаем текст из TextBox
            string userInput = searchTextBox.Text;

            // Добавляем текст в таблицу, если он не пустой
            if (!string.IsNullOrWhiteSpace(userInput))
            {
                //tableContent.Remove(tableContent.Length - 8, 8);

                //htmlDoc.DocumentNode.OuterHtml = userInput;
                //tableNode.AppendChild(tableNode.CreateNode(WriteToForm(userInput)));


                //var Node = htmlDoc.DocumentNode.SelectSingleNode("//div");

                if (tableNode != null)
                {
                    // Очищаем содержимое тега, удаляя все дочерние элементы
                    tableNode.RemoveAllChildren();
                }
                var headerRowNode = HtmlNode.CreateNode("<tr><th class=\"title\">Найденные документы</th></tr>");
                tableNode.AppendChild(headerRowNode);

                if (await WriteToForm(userInput) == true)
                {
                    //searchTextBox.Clear();
                    // Обновляем текст формы
                    htmlPanel.Text = htmlDoc.DocumentNode.OuterHtml;
                };
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите текст.");
            }
        }




        //public async Task<IEnumerable<string>> TestAsync(string userInput)
        //{
        //    string url2 = "http://192.168.0.14:81/kodeks/bparser?parse=" + Uri.EscapeDataString(userInput);
        //    using (HttpClient httpclient = new HttpClient())
        //    {
        //        HttpResponseMessage response = await httpclient.GetAsync(url2);
        //        response.EnsureSuccessStatusCode();
        //        string bparserResult = await response.Content.ReadAsStringAsync();

        //        Console.WriteLine();


        //        return GetChars(bparserResult, userInput);
        //    }
        //}

        //private static IEnumerable<string> GetChars(string bparserResult, string userInput)
        //{
        //    var client2 = new apiSoapClient();

        //    var resz = client2.FuzzySearch(userInput, null, 0, "searchbynames", bparserResult);


        //    var req2 = new TechExpertRef.GetSearchListNRequest(resz.id, null, 0, null, null, false);
        //    var resp2 = client2.GetSearchListN(req2);

        //    for (int i = 0; i < resp2.ArrayOfDocListItem.Length; i++)
        //    {
        //        DocListItem docInfo = resp2.ArrayOfDocListItem[i];
        //        yield return docInfo.nd.ToString();
        //    }
        //}

        //private async Task<StringBuilder> WriteToForm(string userInput, StringBuilder tableContent)
        //private async Task<StringBuilder> WriteToForm(string userInput)
        private async Task<bool> WriteToForm(string userInput)
        {
            //string text = "гражданский кодекс";
            string url2 = "http://192.168.0.14:81/kodeks/bparser?parse=" + Uri.EscapeDataString(userInput);

            using (HttpClient httpclient = new HttpClient())
            {
                HttpResponseMessage response = await httpclient.GetAsync(url2);
                response.EnsureSuccessStatusCode();
                string bparserResult = await response.Content.ReadAsStringAsync();

                var client2 = new apiSoapClient();

                var resz = client2.FuzzySearch(userInput, null, 0, "searchbynames", bparserResult);


                var req2 = new TechExpertRef.GetSearchListNRequest(resz.id, null, 0, null, null, false);
                var resp2 = client2.GetSearchListN(req2);

                var countFindRowsNode = HtmlNode.CreateNode($"<tr><th class=\"countFindRows\">В списке элементов: {resp2.ArrayOfDocListItem.Length}</th></tr>");
                tableNode.AppendChild(countFindRowsNode);
                /*Федеральный закон от 29.07.2017 N 217-ФЗ
                 * <br>
                 * <span class="refchapname">
                 * <a href="javascript:;" nd="436753181" mark="0000000000000000000000000000000000000000000000000246QHAJ">Статья 33.</a>
                 * О внесении изменений в часть первую Гражданского кодекса Российской Федерации</span>*/

                for (int i = 0; i < resp2.ArrayOfDocListItem.Length; i++)
                {
                    DocListItem docInfo = resp2.ArrayOfDocListItem[i];

                    var htmlRow = new HtmlAgilityPack.HtmlDocument();
                    //var newHtmlRow = new HtmlAgilityPack.HtmlDocument();
                    htmlRow.LoadHtml(docInfo.info);

                    // Находим все теги <a>
                    var anchorNodes = htmlRow.DocumentNode.SelectNodes("//a");

                    // Проверяем наличие тегов <a>
                    if (anchorNodes != null)
                    {
                        foreach (var anchorNode in anchorNodes)
                        {
                            // Получаем значение атрибута "mark"
                            string markValue = anchorNode.GetAttributeValue("mark", string.Empty);
                            anchorNode.SetAttributeValue("href", $"kodeks://link/d?nd={docInfo.nd}&mark={markValue}");
                        }
                    }
                    else
                    {
                        var linkNode = HtmlNode.CreateNode($"<a href='kodeks://link/d?nd={docInfo.nd}'>{htmlRow.Text}</a>");
                        htmlRow.DocumentNode.InnerHtml = linkNode.OuterHtml;
                    }


                    //Находим первый тег<br>

                    var brNode = htmlRow.DocumentNode.SelectSingleNode("//br");

                    if (brNode != null)
                    {
                        // Получаем родительский узел тега <br>
                        var parentNode = brNode.ParentNode;

                        // Извлекаем текст до <br>
                        string textBeforeBr = parentNode.InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.None)[0];

                        parentNode.RemoveChild(brNode);

                        // Создаем ссылку
                        var linkNode = HtmlNode.CreateNode($"<a href='kodeks://link/d?nd={docInfo.nd}'>{textBeforeBr}</a>");

                        // Заменяем текст на ссылку до тега <br>, прежде чем изменять другие узлы
                        parentNode.InnerHtml = parentNode.InnerHtml.Replace(textBeforeBr, linkNode.OuterHtml);

                        
                        //newHtmlRow.DocumentNode.AppendChild(linkNode);
                    }

                    // Получаем обновленный HTML
                    var tbRow = HtmlNode.CreateNode($"<tr><td class=\"tbRow\">{htmlRow.DocumentNode.OuterHtml}</td></tr>");
                    tableNode.AppendChild(tbRow);
                }
            }
            return true;
        }
    }
}

