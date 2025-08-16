# AdvertisingPlatforms — Сервис подбора рекламных площадок

Веб-сервис на ASP.NET Core, который подбирает рекламные площадки для заданной локации

## Технологии

- .NET 8
- ASP.NET Core Minimal API
- xUnit

## Хранение данных

Данные хранятся в оперативной памяти в виде префиксного дерева (trie). загрузка файла полностью перезаписывает текущее состояние. сервис зарегистрирован как singleton, поэтому данные живут в процессе до перезапуска.

## API

1) Загрузка файла с площадками

- URL: `/upload`
- Метод: `POST`
- Тело: `multipart/form-data`
- Поле файла в HTML-форме (`wwwroot/html/index.html`) называется `uploads`. Сервер берёт первый файл из `Request.Form.Files`.

Пример с curl:
```bash
curl.exe -X POST -F "uploads=@locations.txt" http://localhost:5106/upload
```

2) Поиск площадок по локации

- URL: `/{location}` (например: `ru/svrd/revda`)
- Метод: `GET`

Примеры:
```bash
curl.exe -X GET http://localhost:5106/ru/svrd/revda
```

```bash
curl.exe -X GET http://localhost:5106/ru/msk
```

Также корневая страница `/` отдаёт простую HTML-форму для загрузки файла: `wwwroot/html/index.html`.

## Инструкция по запуску

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) или новее
- Git

### Запуск приложения

1. Клонируйте репозиторий:

```bash
git clone https://github.com/J0shlerB0y/AdvertisingPlatforms
cd AdvertisingPlatforms
```

2. Запустите проект из каталога `src`:

```bash
cd src
dotnet run
```

3. В консоли будут показаны адреса. Используйте указанный порт при вызове API.

### Запуск тестов

```bash
cd tests
dotnet test
```

## Формат входного файла

Файл должен содержать строки формата:

```
Название площадки:/ru/svrd/revda,/ru/svrd/pervik
```

- Название и список локаций разделены двоеточием `:`
- Локации разделяются запятыми `,`
- Пустые или неправильно сформатированные строки игнорируются