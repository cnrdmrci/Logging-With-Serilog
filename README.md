# Serilog ile Loglama 

### Kurulum

> PM> Install-Package Serilog.AspNetCore__
> PM> Install-Package Serilog.Sinks.Console__
> PM> Install-Package Serilog.Sinks.MSSqlServer__
> PM> Install-Package Serilog.Sinks.Seq

### SeriLog Nedir?
Serilog 2013’de yayınlanan çeşitli platformlara loglama yapmayı basite indirgeyen open source bir kütüphanedir. Seri logu diğer kütüphanelerden ayıran şey oldukça kullanışlı bir şekilde kurgulanmış structured logging özelliğidir.

### Structured Logging Nedir?
Bir veriyi nesne yapısında kaydetmek ve bu yapısal veriyi kolayca sorgulamaktır.

```csharp
var person = new { Name = "Caner" , City = "İstanbul" };
Log.Debug("Info {@Person}",person);
```

'@Person' ifadesinin başındaki @ verinin nesne yapısında kaydedilmesini sağlar. Başında @ olmasaydı string olarak kayıt edecekti. Verinin nesne olarak kayıt edilmesi, aramakta kolaylık sağlamaktadır.

### Serilog Sinks Nedir?
Serilog kütüphanesi içerisinde farklı formatlardaki kaynaklara log yazmamızı sağlayan provider’lara Sink adı verilmektedir.  SQL Server, PostgreSQL, Elasticsearch, MongoDB, RabbitMQ gibi kaynakların yanında Amazon DynamoDB, Azure DocumentDB gibi bulut tabanlı veri hizmetlerine kadar çok sayıda kaynağa log atmamıza olanak sağlayan yapılara Serilog Sink adı verilmektedir.

### Serilog Sql Server Log Entegrasyonu
Sql server'a log kaydı atabilmemiz için mevcut veritabanımızda bir tablo olması gerekli. Bunun için ilk önce aşağıdaki komutu çalıştırmalıyız.

```sql
CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[MessageTemplate] [nvarchar](max) NULL,
	[Level] [nvarchar](128) NULL,
	[TimeStamp] [datetimeoffset](7) NOT NULL,
	[Exception] [nvarchar](max) NULL,
	[Properties] [xml] NULL,
	[LogEvent] [nvarchar](max) NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (	PAD_INDEX = OFF, 
			STATISTICS_NORECOMPUTE = OFF, 
			IGNORE_DUP_KEY = OFF, 
			ALLOW_ROW_LOCKS = ON, 
			ALLOW_PAGE_LOCKS = ON
		   ) 
	ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
```

Tablomuzu oluşturduk. Şimdi aşağıdaki kod ile log kaydının sql servera kaydedilmesi için tanımlama yapabiliriz ve kayıt ekleyebiliriz.

```csharp
Log.Logger = new LoggerConfiguration()
                 .WriteTo.MSSqlServer("Server=localhost;Database=SeriLog;User Id=userName;Password=password;","Logs")
                 .CreateLogger();
Log.Information("Test info kaydı.");
Log.Error("Test Error kaydı.");
Log.ForContext("User", "Caner").Error("Test Error Ekstra.");
```

### Serilog Log Oluşturma Detayı
- MinimumLevel kaydedilecek en düşük log seviyesini belirtir.
> Not:MinumLevel belirtmezsek default olarak Information set edilmektedir.
- MinimumLevel.Override("Namespace",LogEventLevel.Warning) şeklinde belirli bir namespace'e farklı log seviyesi kayıt edilebilir.
- Enricher’lar basit olarak log eventlere sabit veya dinamik property eklemek için kullandığımız yapıdır diyebiliriz.

```csharp
Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console()
                 .MinimumLevel.Information()
                 .MinimumLevel.Override("Microsoft.AspNetCore",LogEventLevel.Warning)
                 .Enrich.WithProperty("AppName","Serilog Sample")
               	 .Enrich.WithProperty("Environment","development")
                 .CreateLogger();
var person = new { Name = "Caner" , City = "İstanbul" };
Log.Debug("Info {@Person}",person);
```

Yukarıdaki log kaydını konsol ekranında görebiliriz.

### Log Seviye sıralaması
- 0-Verbose 
- 1-Debug
- 2-Information
- 3-Warning
- 4-Error
- 5-Fatal


### Datalus/Seq Nedir?
Kaydedilen logları gösteren bir platformdur. Kendi içerisinde arama ve filtreleme yapabilmektedir. Hatta sql sorguları ile arama da yapılabilmektedir. Bildirim gönderimi olarak mail gönderebilir veya 3. parti bir uygulama entegre ederek işlem yapılabilmektedir.

> Docker üzerinde seq çalıştırma
> docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

Seq çalışma adresi : http://localhost:5341 

```csharp
Log.Logger = new LoggerConfiguration()
             .WriteTo.Seq("http://localhost:5341")
             .CreateLogger();
var person = new { Name = "Caner" , City = "İstanbul" };
Log.Debug("Info {@Person}",person);
```
Yukarıda görüldüğü üzere seq ortamına kayıt ekledik.

### Serilog Seq Arama Örnekleri

```csharp
var person = new { Name = "Caner" , City = "İstanbul" , Age = 15 };
Log.Debug("Info {@Person}",person);
Log.ForContext("UserName", "Caner").Error("Test Error Ekstra.");
```
Arama Anahtarı;
> Person.Name = "Caner"__
> Person.Name = "Caner" && Person.City = "İstanbul" && Person.Age > 15__
> UserName = "Caner"__
> @Level = "Error"__
> select names() from stream__
> select @Message from stream__