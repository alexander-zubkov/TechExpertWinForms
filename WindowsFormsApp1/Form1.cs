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
        private HtmlNode containerNode;
        private HtmlNode buttonWrapperNode;
        private HtmlNode tableNode;
        private HtmlNode headerRowNode;
        private string userInput;
        private string bparser;

        private Tuple<int, string> firstSearchRes = new Tuple<int, string>(0, "");

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

            //htmlPanel.LinkClicked += (sender, e) =>
            //{
            //    throw new NotImplementedException();
            //};

            htmlPanel.LinkClicked += (sender, e) =>
            {
                if (e.Link == "button")
                {
                    var thNode = buttonWrapperNode.SelectSingleNode("//th[@class='loadMoreButton']");
                    if (thNode != null)
                        buttonWrapperNode.RemoveChild(tableNode.SelectSingleNode("//th[@class='loadMoreButton']"));

                    var res = WriteToForm(userInput, bparser, firstSearchRes.Item1, firstSearchRes.Item2);



                }
                //string val = "";
                //if (e.Attributes.TryGetValue(""))
                //{

                //}
            };

            //htmlPanel.
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
            containerNode = HtmlNode.CreateNode("<div class='container'></div>");
            bodyNode.AppendChild(containerNode);
            
            // Создание таблицы
            tableNode = HtmlNode.CreateNode("<table></table>");
            containerNode.AppendChild(tableNode);

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



        // Обработчик клика по кнопке Поиск
        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            // Получаем текст из TextBox
            userInput = searchTextBox.Text;

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
                headerRowNode = HtmlNode.CreateNode("<tr><th class=\"title\">Найденные документы</th></tr>");
                tableNode.AppendChild(headerRowNode);

                //var countFindRowsNode = HtmlNode.CreateNode("<tr><th class=\"countFindRows\"></th></tr>");
                //tableNode.AppendChild(countFindRowsNode);

                //string fuzzySearchId = "";

                bparser = await Getbparser(userInput);

                firstSearchRes = WriteToForm(userInput, bparser);

                buttonWrapperNode = HtmlNode.CreateNode($"<div class='buttonWrapper'><a class='loadMoreButton' href='button'>Загрузить ещё</a></div>");
                containerNode.AppendChild(buttonWrapperNode);

                //if (await WriteToForm(userInput) == true)
                //{
                //    //searchTextBox.Clear();
                //    // Обновляем текст формы
                htmlPanel.Text = htmlDoc.DocumentNode.OuterHtml;
                //};
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите текст.");
            }
        }

        private async Task<string> Getbparser(string userInput)
        {
            string url2 = "http://192.168.0.14:81/kodeks/bparser?parse=" + Uri.EscapeDataString(userInput);

            using (HttpClient httpclient = new HttpClient())
            {
                HttpResponseMessage response = await httpclient.GetAsync(url2);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }


        private Tuple<int, string> WriteToForm(string userInput, string bparserResult, int mainParts = 0, string firstFuzzySearchId = "")
        {
            bool firstCall = false;
            var client2 = new apiSoapClient();

            List<GetSearchListNResponse> responses = new List<GetSearchListNResponse>();
            int returnableRowsCount = 0;

            // Если вызываем метод впервые
            if (firstFuzzySearchId == "")
            {
                firstCall = true;
                var mainInfo = client2.FuzzySearch(userInput, null, 0, "searchbynames", bparserResult);
                responses.Add(client2.GetSearchListN(new GetSearchListNRequest(mainInfo.id, null, 0, null, null, false)));

                mainParts = mainInfo.parts;
                firstFuzzySearchId = mainInfo.id;
                //returnableRowsCount = responses[0].ArrayOfDocListItem.Length;
                var countFindRowsNode = HtmlNode.CreateNode($"<tr><th class=\"countFindRows\">В списке элементов: {responses[0].ArrayOfDocListItem.Length}</th></tr>");
                tableNode.AppendChild(countFindRowsNode);
            }

            // если пользователь нажал Загрузить ещё
            else
            {
                //returnableRowsCount = 0;

                if (mainParts > 1)
                {
                    returnableRowsCount = 20;

                    // Добавляем остальные основные части (если они есть)
                    for (int mainPartNumber = 1; mainPartNumber < mainParts; mainPartNumber++)
                        responses.Add(client2.GetSearchListN(new GetSearchListNRequest(firstFuzzySearchId, null, mainPartNumber, null, null, false)));
                }

                var dopInfo = client2.FuzzySearch(userInput, null, 1, "searchbynames", bparserResult);
                // Добавляем дополнительные части (комментарии, образцы и тд)
                for (int dopPartNumber = 0; dopPartNumber < dopInfo.parts; dopPartNumber++)
                    responses.Add(client2.GetSearchListN(new GetSearchListNRequest(dopInfo.id, null, dopPartNumber, null, null, false)));
                
            }


            foreach (var resp in responses)
            {
                returnableRowsCount += resp.ArrayOfDocListItem.Length;
                Console.WriteLine(1);
                for (int i = 0; i < resp.ArrayOfDocListItem.Length; i++)
                {
                    DocListItem docInfo = resp.ArrayOfDocListItem[i];

                    //returnableRowsCount += i;

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
            if (firstCall == false)
            {
                var thNode = tableNode.SelectSingleNode("//th[@class='countFindRows']");

                if (thNode != null)
                {
                    // Изменяем внутренний текст элемента <th>
                    thNode.InnerHtml = $"В списке элементов: {returnableRowsCount}";
                }

                //    tableNode.RemoveChild(tableNode.ChildNodes.First(x => x.ChildAttributes("class").First().Value == "countFindRows"));
                //var newCountFindRowsNode = HtmlNode.CreateNode($"<tr><th class=\"countFindRows\">В списке элементов: {returnableRowsCount}</th></tr>");
                //tableNode.InsertAfter(newCountFindRowsNode, headerRowNode);
            }
            //tableNode.InsertAfter(HtmlNode.CreateNode($"<tr><th class=\"countFindRows\">В списке элементов: {responses[0].ArrayOfDocListItem.Length}</th></tr>"));
            //var countFindRowsNode = HtmlNode.CreateNode($"<tr><th class=\"countFindRows\">В списке элементов: {responses[0].ArrayOfDocListItem.Length}</th></tr>");
            //var newCountFindRowsNode = tableNode.ChildNodes.First(x => x.ChildAttributes("class").First().Value == "countFindRows");
            htmlPanel.Text = htmlDoc.DocumentNode.OuterHtml;
            return new Tuple<int, string>(mainParts, firstFuzzySearchId);
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

    //private async Task<int> WriteToForm(string userInput, ref List<GetSearchListNResponse> responses, string firstFuzzySearchId = "")
    //{
    //    int countMainParts = -1;
    //    //string text = "гражданский кодекс";
    //    string url2 = "http://192.168.0.14:81/kodeks/bparser?parse=" + Uri.EscapeDataString(userInput);

    //    using (HttpClient httpclient = new HttpClient())
    //    {
    //        HttpResponseMessage response = await httpclient.GetAsync(url2);
    //        response.EnsureSuccessStatusCode();
    //        string bparserResult = await response.Content.ReadAsStringAsync();

    //        var client2 = new apiSoapClient();

    //        List<GetSearchListNResponse> responses = new List<GetSearchListNResponse>();

    //        if (writeMore)
    //        {
    //            for(int mainPartNumber = 1; mainPartNumber < mainParts; mainPartNumber++)
    //            {
    //                responses.Add(client2.GetSearchListN(new GetSearchListNRequest(mai.id, null, dopPartNumber, null, null, false)));
    //            }

    //            var dopInfo = client2.FuzzySearch(userInput, null, 1, "searchbynames", bparserResult);

    //            for (int dopPartNumber = 0; dopPartNumber < dopInfo.parts; dopPartNumber++)
    //            {
    //                responses.Add(client2.GetSearchListN(new GetSearchListNRequest(dopInfo.id, null, dopPartNumber, null, null, false)));
    //            }

    //        }
    //        var mainInfo = client2.FuzzySearch(userInput, null, 0, "searchbynames", bparserResult);
    //        countMainParts = resz.parts;

    //        //var resz2 = client2.FuzzySearch(userInput, null, 1, "searchbynames", bparserResult);


    //        var req2 = new TechExpertRef.GetSearchListNRequest(resz.id, null, 0, null, null, false);
    //        var resp2 = client2.GetSearchListN(req2);

    //        var req3 = new TechExpertRef.GetSearchListNRequest(resz.id, null, 1, null, null, false);
    //        var resp3 = client2.GetSearchListN(req3);

    //        var req4 = new TechExpertRef.GetSearchListNRequest(resz2.id, null, 0, null, null, false);
    //        var resp4 = client2.GetSearchListN(req4);


    //        var countFindRowsNode = HtmlNode.CreateNode($"<tr><th class=\"countFindRows\">В списке элементов: {resp2.ArrayOfDocListItem.Length}</th></tr>");
    //        tableNode.AppendChild(countFindRowsNode);
    //        /*Федеральный закон от 29.07.2017 N 217-ФЗ
    //         * <br>
    //         * <span class="refchapname">
    //         * <a href="javascript:;" nd="436753181" mark="0000000000000000000000000000000000000000000000000246QHAJ">Статья 33.</a>
    //         * О внесении изменений в часть первую Гражданского кодекса Российской Федерации</span>*/

    //        for (int i = 0; i < resp2.ArrayOfDocListItem.Length; i++)
    //        {
    //            DocListItem docInfo = resp2.ArrayOfDocListItem[i];

    //            var htmlRow = new HtmlAgilityPack.HtmlDocument();
    //            //var newHtmlRow = new HtmlAgilityPack.HtmlDocument();
    //            htmlRow.LoadHtml(docInfo.info);

    //            // Находим все теги <a>
    //            var anchorNodes = htmlRow.DocumentNode.SelectNodes("//a");

    //            // Проверяем наличие тегов <a>
    //            if (anchorNodes != null)
    //            {
    //                foreach (var anchorNode in anchorNodes)
    //                {
    //                    // Получаем значение атрибута "mark"
    //                    string markValue = anchorNode.GetAttributeValue("mark", string.Empty);
    //                    anchorNode.SetAttributeValue("href", $"kodeks://link/d?nd={docInfo.nd}&mark={markValue}");
    //                }
    //            }
    //            else
    //            {
    //                var linkNode = HtmlNode.CreateNode($"<a href='kodeks://link/d?nd={docInfo.nd}'>{htmlRow.Text}</a>");
    //                htmlRow.DocumentNode.InnerHtml = linkNode.OuterHtml;
    //            }


    //            //Находим первый тег<br>

    //            var brNode = htmlRow.DocumentNode.SelectSingleNode("//br");

    //            if (brNode != null)
    //            {
    //                // Получаем родительский узел тега <br>
    //                var parentNode = brNode.ParentNode;

    //                // Извлекаем текст до <br>
    //                string textBeforeBr = parentNode.InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.None)[0];

    //                parentNode.RemoveChild(brNode);

    //                // Создаем ссылку
    //                var linkNode = HtmlNode.CreateNode($"<a href='kodeks://link/d?nd={docInfo.nd}'>{textBeforeBr}</a>");

    //                // Заменяем текст на ссылку до тега <br>, прежде чем изменять другие узлы
    //                parentNode.InnerHtml = parentNode.InnerHtml.Replace(textBeforeBr, linkNode.OuterHtml);


    //                //newHtmlRow.DocumentNode.AppendChild(linkNode);
    //            }

    //            // Получаем обновленный HTML
    //            var tbRow = HtmlNode.CreateNode($"<tr><td class=\"tbRow\">{htmlRow.DocumentNode.OuterHtml}</td></tr>");
    //            tableNode.AppendChild(tbRow);
    //        }
    //        htmlDoc.DocumentNode.AppendChild(HtmlNode.CreateNode($"<a class='loadMoreButton' href='button'>Загрузить ещё</a>"));
    //    }
    //    return countMainParts;
    //    //return true;
    //}




    //private async Task<bool> WriteToForm(string userInput)
    //{
    //    //string text = "гражданский кодекс";
    //    string url2 = "http://192.168.0.14:81/kodeks/bparser?parse=" + Uri.EscapeDataString(userInput);

    //    using (HttpClient httpclient = new HttpClient())
    //    {
    //        HttpResponseMessage response = await httpclient.GetAsync(url2);
    //        response.EnsureSuccessStatusCode();
    //        string bparserResult = await response.Content.ReadAsStringAsync();

    //        var client2 = new apiSoapClient();

    //        var resz = client2.FuzzySearch(userInput, null, 0, "searchbynames", bparserResult);
    //        var resz2 = client2.FuzzySearch(userInput, null, 1, "searchbynames", bparserResult);


    //        var req2 = new TechExpertRef.GetSearchListNRequest(resz.id, null, 0, null, null, false);
    //        var resp2 = client2.GetSearchListN(req2);

    //        var req3 = new TechExpertRef.GetSearchListNRequest(resz.id, null, 1, null, null, false);
    //        var resp3 = client2.GetSearchListN(req3);

    //        var req4 = new TechExpertRef.GetSearchListNRequest(resz2.id, null, 0, null, null, false);
    //        var resp4 = client2.GetSearchListN(req4);


    //        var countFindRowsNode = HtmlNode.CreateNode($"<tr><th class=\"countFindRows\">В списке элементов: {resp2.ArrayOfDocListItem.Length}</th></tr>");
    //        tableNode.AppendChild(countFindRowsNode);
    //        /*Федеральный закон от 29.07.2017 N 217-ФЗ
    //         * <br>
    //         * <span class="refchapname">
    //         * <a href="javascript:;" nd="436753181" mark="0000000000000000000000000000000000000000000000000246QHAJ">Статья 33.</a>
    //         * О внесении изменений в часть первую Гражданского кодекса Российской Федерации</span>*/

    //        for (int i = 0; i < resp2.ArrayOfDocListItem.Length; i++)
    //        {
    //            DocListItem docInfo = resp2.ArrayOfDocListItem[i];

    //            var htmlRow = new HtmlAgilityPack.HtmlDocument();
    //            //var newHtmlRow = new HtmlAgilityPack.HtmlDocument();
    //            htmlRow.LoadHtml(docInfo.info);

    //            // Находим все теги <a>
    //            var anchorNodes = htmlRow.DocumentNode.SelectNodes("//a");

    //            // Проверяем наличие тегов <a>
    //            if (anchorNodes != null)
    //            {
    //                foreach (var anchorNode in anchorNodes)
    //                {
    //                    // Получаем значение атрибута "mark"
    //                    string markValue = anchorNode.GetAttributeValue("mark", string.Empty);
    //                    anchorNode.SetAttributeValue("href", $"kodeks://link/d?nd={docInfo.nd}&mark={markValue}");
    //                }
    //            }
    //            else
    //            {
    //                var linkNode = HtmlNode.CreateNode($"<a href='kodeks://link/d?nd={docInfo.nd}'>{htmlRow.Text}</a>");
    //                htmlRow.DocumentNode.InnerHtml = linkNode.OuterHtml;
    //            }


    //            //Находим первый тег<br>

    //            var brNode = htmlRow.DocumentNode.SelectSingleNode("//br");

    //            if (brNode != null)
    //            {
    //                // Получаем родительский узел тега <br>
    //                var parentNode = brNode.ParentNode;

    //                // Извлекаем текст до <br>
    //                string textBeforeBr = parentNode.InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.None)[0];

    //                parentNode.RemoveChild(brNode);

    //                // Создаем ссылку
    //                var linkNode = HtmlNode.CreateNode($"<a href='kodeks://link/d?nd={docInfo.nd}'>{textBeforeBr}</a>");

    //                // Заменяем текст на ссылку до тега <br>, прежде чем изменять другие узлы
    //                parentNode.InnerHtml = parentNode.InnerHtml.Replace(textBeforeBr, linkNode.OuterHtml);


    //                //newHtmlRow.DocumentNode.AppendChild(linkNode);
    //            }

    //            // Получаем обновленный HTML
    //            var tbRow = HtmlNode.CreateNode($"<tr><td class=\"tbRow\">{htmlRow.DocumentNode.OuterHtml}</td></tr>");
    //            tableNode.AppendChild(tbRow);
    //        }
    //        htmlDoc.DocumentNode.AppendChild(HtmlNode.CreateNode($"<a class='loadMoreButton' href='button'>Загрузить ещё</a>"));
    //    }
    //    return true;
    //}






}


