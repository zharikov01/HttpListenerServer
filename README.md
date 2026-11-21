# HttpListenerServer

Итоговое задание: консольное приложение HTTP-сервер на классе `HttpListener`.
Решение содержит две реализации одного и того же сервера:

| Проект | Реализация |
|---|---|
| `HttpListenerSync` | синхронная: `GetContext()` блокирует поток до прихода запроса |
| `HttpListenerAsync` | асинхронная: `GetContextAsync()` и `await`, поток не блокируется |

## Запросы

| Адрес | Страница | Как формируется |
|---|---|---|
| http://localhost:6789/ | `wwwroot/index.html` | текст Index и ссылки на остальные запросы |
| http://localhost:6789/seasons | `wwwroot/htdocs/seasons.html` | список времен года прямо в разметке, без шаблона |
| http://localhost:6789/toys | `wwwroot/htdocs/toys.html` | шаблон: метка `{{ toys }}` заменяется списком из 5 игрушек |
| любой другой | `wwwroot/htdocs/error.html` | страница ошибки |

## Запуск

Открыть `HttpListenerServer.slnx` в Visual Studio, выбрать запускаемый проект
(`HttpListenerSync` или `HttpListenerAsync`) и нажать F5. Затем открыть в браузере
http://localhost:6789/
