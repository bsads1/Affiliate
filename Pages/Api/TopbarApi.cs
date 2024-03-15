using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class TopbarApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapGet("/api/topbar-content/{key}/{value}", (HttpContext context) =>
        {
            var key = context.Request.RouteValues["key"]?.ToString();
            var value = context.Request.RouteValues["value"]?.ToString();
            if (key == "live")
            {
                var htmlResult = $@"<div class=""wrapper flex justify-between"">
            <div class=""flex gap-4"">
                <a href=""/admin/live/edit/{value}"" class=""text-blue-600 hover:text-blue-800 visited:text-purple-600 font-semibold underline transition duration-300 ease-in-out"">
                    Edit Live
                </a>
                <a href=""/admin/live/update/{value}"" class=""text-blue-600 hover:text-blue-800 visited:text-purple-600 font-semibold underline transition duration-300 ease-in-out"">
                    Update Live Result
                </a>
                <label for=""update-seo-btn"" class=""text-white text-blue-600 hover:text-blue-800 visited:text-purple-600 font-semibold underline transition duration-300 ease-in-out"">
                    Update SEO
                </a>
            </div>
            <div>
              <a href=""/admin/"" class=""text-blue-600 hover:text-blue-800 visited:text-purple-600 font-semibold underline transition duration-300 ease-in-out"">
                Admin Dashboard
              </a>
            </div>
        </div>";
                return Results.Content(htmlResult);
            }
            else if (key == "page")
            {
                var htmlResult = $"""
                                  <div class="wrapper flex justify-between">
                                  <div>
                                  <a href="/admin/page/edit/{value}" class="text-blue-600 hover:text-blue-800 visited:text-purple-600 font-semibold underline transition duration-300 ease-in-out">
                                    Edit Page
                                  </a>
                                  </div>
                                  <div>
                                  <a href="/admin/" class="text-blue-600 hover:text-blue-800 visited:text-purple-600 font-semibold underline transition duration-300 ease-in-out">
                                    Admin Dashboard
                                  </a>
                                  </div>
                                  </div>
                                  """;
                return Results.Content(htmlResult);
            }

            return Results.Content("");
        });
    }
}