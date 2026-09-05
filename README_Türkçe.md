# OmniPay API

OmniPay, .NET 8 kullanılarak geliştirdiğim basit bir ödeme platformu API projesidir.

Bu projede ödeme oluşturma, ödeme durumunu değiştirme ve ödeme işlemini simüle etme gibi işlemler yapılabiliyor. Projeyi geliştirirken Clean Architecture, CQRS, Entity Framework Core ve PostgreSQL gibi teknolojileri kullanmayı öğrendim.

## Projede Neler Var?

- Ödeme oluşturma
- Ödeme detayını ID ile getirme
- Ödemeyi işleme
- Ödemeyi başarılı olarak işaretleme
- Ödemeyi başarısız olarak işaretleme
- Ödemeyi iptal etme
- Simüle edilmiş ödeme gateway'i
- PostgreSQL veritabanı
- Entity Framework Core
- CQRS command ve query yapısı
- xUnit testleri
- Swagger ile API testi
- Docker desteği

## Proje Yapısı

Projede temel olarak şu katmanlar bulunuyor:

- `OmniPay.Domain`: Payment entity'si ve ödeme ile ilgili kurallar
- `OmniPay.Application`: Command, query, handler, DTO ve interface'ler
- `OmniPay.Infrastructure`: PostgreSQL, EF Core repository ve simüle edilmiş ödeme gateway'i
- `OmniPay.API`: API controller'ları ve uygulamanın başlangıç ayarları
- `OmniPay.Tests`: Unit testler

## Kullanılan Teknolojiler

- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- xUnit
- Swagger / OpenAPI
- Docker
- GitHub Actions

## Ödeme Akışı

Ödeme işlemleri CQRS handler'ları üzerinden ilerliyor.

Kullanılan bazı handler'lar:

- `CreatePaymentCommandHandler`
- `GetPaymentByIdQueryHandler`
- `ProcessPaymentCommandHandler`
- `MarkPaymentAsSuccessfulCommandHandler`
- `MarkPaymentAsFailedCommandHandler`
- `CancelPaymentCommandHandler`

Ödeme sağlayıcısı şu anda gerçek bir banka entegrasyonu yerine simüle edilmiş bir gateway kullanıyor.

## Projeyi Çalıştırma

### Gerekenler

- .NET SDK 8.0 veya üzeri
- PostgreSQL veya Docker

### Veritabanı bağlantısı

Veritabanı bağlantı bilgisini environment variable olarak verebilirsin:

```bash
export ConnectionStrings__OmniPayDatabase="Host=localhost;Port=5432;Database=omnipay;Username=postgres;Password=<your-password>"