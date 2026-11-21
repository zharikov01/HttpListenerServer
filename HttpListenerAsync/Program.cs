using System.Net;
using System.Text;

/*
 * Итоговое задание. Асинхронная реализация.
 *
 * Реализовать консольное приложение HTTP-сервер для обработки запросов.
 *   • http://localhost:6789/ - вывод страницы wwwroot/index.html с текстом Index и ссылками
 *     для запуска остальных запросов
 *   • http://localhost:6789/seasons - вывод страницы wwwroot/htdocs/seasons.html списка
 *     названий времен года, без использования шаблона, на странице также должны быть
 *     ссылки для запуска остальных запросов
 *   • http://localhost:6789/toys - вывод страницы wwwroot/htdocs/toys.html списка названий
 *     игрушек (5 наименований), с использованием шаблона, на странице также должны быть
 *     ссылки для запуска остальных запросов
 */

Console.Title = "Итоговое задание - класс HttpListener, основа для веб-сервера, асинхронная обработка";
Console.SetWindowSize(64, 26);

// вызов асинхронного метода
await ListenAsync();

Console.ReadKey(true);
return;


// асинхронная реализация слушателя HTTP-протокола
static async Task ListenAsync() {
    var listener = new HttpListener();

    listener.Prefixes.Add("http://127.0.0.1:6789/");
    listener.Prefixes.Add("http://localhost:6789/");

    listener.Prefixes.Add("http://127.0.0.1:6789/seasons/");
    listener.Prefixes.Add("http://localhost:6789/seasons/");

    listener.Prefixes.Add("http://127.0.0.1:6789/toys/");
    listener.Prefixes.Add("http://localhost:6789/toys/");


    // И также чтобы постоянно прослушивать всходящие подключения, можно
    // заключить код слушателя в бесконечный цикл while
    while (true) {
        Console.WriteLine("Ожидание подключений...");
        listener.Start();

        // !!! запуск операции подключения запроса клиента !!!
        HttpListenerContext context = await listener.GetContextAsync();

        // RawUrl - текст запроса, с параметрами
        HttpListenerRequest request = context.Request;
        Console.WriteLine($"Обработка подключения {request.RemoteEndPoint.Address}\n" +
                          $"Текст запроса: {request.RawUrl}\n");

        // создаем объект ответа из запроса браузера
        HttpListenerResponse response = context.Response;

        // создаем ответ в виде кода html - из файла с разметкой
        var responseStr = "";

        // RawUrl - текст запроса, выбираем файл, по запросу
        var fileName = "";

        // маршрутизация - разрешение маршрута в конкретное имя отдаваемого файла
        switch (request.RawUrl) {
            case "/":
                fileName = "wwwroot/index.html";
                break;

            case "/seasons":
            case "/seasons/":
                fileName = "wwwroot/htdocs/seasons.html";
                break;

            case "/toys":
            case "/toys/":
                fileName = "wwwroot/htdocs/toys.html";
                break;

            default:
                fileName = "wwwroot/htdocs/error.html";
                break;

            // реакция на запрос favicon - попытка передать файл
            case "/favicon.ico":
                fileName = "wwwroot/images/favicon.ico";
                var buf = await File.ReadAllBytesAsync(fileName);
                var output1 = response.OutputStream;
                response.ContentLength64 = buf.Length;

                await output1.WriteAsync(buf, 0, buf.Length);
                output1.Close();

                // завершаем обработку текущего запроса
                listener.Stop();
                Console.WriteLine();
                continue;
        } // switch

        responseStr = await File.ReadAllTextAsync(fileName);

        // Формирование разметки с вставкой данных в шаблон
        if (fileName == "wwwroot/htdocs/toys.html") {
            List<string> toys = [
                "мяч резиновый",
                "кукла фарфоровая",
                "машинка радиоуправляемая",
                "конструктор деревянный",
                "медведь плюшевый",
            ];

            var repl = "<li>" + string.Join("</li><li>", toys) + "</li>";
            var template = "{{ toys }}";
            while (responseStr.Contains(template)) {
                responseStr = responseStr.Replace(template, repl);
            } // while
        } // if

        // формируем массив байтов для отправки клиенту
        var buffer = Encoding.UTF8.GetBytes(responseStr);

        // получаем поток ответа и пишем в него ответ
        using var output = response.OutputStream;
        response.ContentLength64 = buffer.Length;

        await output.WriteAsync(buffer, 0, buffer.Length);
        output.Close();
        listener.Stop();
        Console.WriteLine();
    } // while
} // async Task Listen
