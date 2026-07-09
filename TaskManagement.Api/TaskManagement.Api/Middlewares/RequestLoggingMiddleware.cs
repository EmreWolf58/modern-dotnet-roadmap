namespace TaskManagement.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private RequestDelegate _next;
        /*
         Request Delegate nedir?

        Her middleware kendinden sonrakini bilir.
        İşte bu sonraki middleware'i temsil eden nesne: RequestDelegate

         */
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            /*
             ismi neden InvokeAsync?
            Çünkü ASP.NET Core middleware'i çalıştırırken bu metodu otomatik arıyor. İsmini rastgele veremezsin.

            parametre olarak HttpContext alıyor.

            Çünkü bütün Request ve Response burada.

            HttpContext nedir ?
            İçinde şunlar vardır:
                Request
                Response
                User
                Headers
                Cookies
                Path
                Method
                Services
                Connection
            Yani HTTP isteğine ait her şey.
             */

            Console.WriteLine("========== REQUEST ==========");
            Console.WriteLine($"Saat: {DateTime.Now}");
            Console.WriteLine($"Method: {context.Request.Method}");
            Console.WriteLine($"Path: {context.Request.Path}");

            await _next(context);

            Console.WriteLine("========== RESPONSE ==========");
        }
    }
}
